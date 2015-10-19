using System;
using System.Web.Http;
using Platform.Models;
using Platform.Models.Models;
using Platform.Utility;
using Platform.Core;
using GToken.Web.Helpers.Extensions;
using GToken.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using GToken.Web.Api.Filters;

namespace GToken.Web.Api.V1
{

    [RoutePrefix("transaction")]

    public class TransactionController : ApiController
    {

        [HttpPost]
        [ActionName("get-conversion-rate")]
        [ExecutionMeter]
        public object GetConversion([FromBody] ConversionModel param)
        {
            var api = Platform.Core.Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            string sourceCurrency = param.source_currency;
            string destinationCurrency = param.destination_currency;
            ErrorCodes? error = null;
            decimal rate = 0m;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (!partner.IsValidHash(param.hashed_token))
                error = ErrorCodes.INVALID_HASHED_TOKEN;
            else if (string.IsNullOrEmpty(sourceCurrency) || string.IsNullOrEmpty(destinationCurrency))
                error = ErrorCodes.MISSING_FIELDS;
            else
            {
                var rateResult = api.GetExchangeRate(sourceCurrency, destinationCurrency, null);
                if (rateResult.Succeeded)
                    rate = rateResult.Data.exchange_rate;
                else
                    error = rateResult.Error;
            }

            // Log //
            string errorCode = error.ToErrorCode();
            api.LogApi("1", "/transaction/get-conversion-rate", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, string.Empty,
                IpHelper.GetClientIp(Request),
                errorCode, JsonConvert.SerializeObject(param));

            // Output //
            return new
            {
                success = (error == null),
                exchange_rate = rate,
                source_currency = sourceCurrency,
                destination_currency = destinationCurrency,
                error_code = errorCode,
                message = error.ToErrorMessage()
            };
        }


        [HttpPost]
        [ActionName("create-transaction")]
        [ExecutionMeter]
        public object CreateTransaction([FromBody] TransactionModel param)
        {
            var api = Platform.Core.Api.Instance;
            string partnerUID = param.partner_id;
            Partner partner = api.GetPartnerById(partnerUID).Data;
            string ip_address = !string.IsNullOrEmpty(param.ip_address) ? param.ip_address : GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request);
            if (string.IsNullOrEmpty(ip_address))
            {
                ip_address = "127.0.0.1";
            }
            string hashed_token = param.hashed_token;
            string transactionJSON = param.transaction;
            ErrorCodes? error = null;
            TransactionJsonModel transactionData = null;
            Result<Transaction> newTrans = null;
            if (partner == null)
            {
                error = ErrorCodes.INVALID_PARTNER_ID;
            }
            else
            {
                if (!partner.IsValidHash(hashed_token))
                {
                    error = ErrorCodes.INVALID_HASHED_TOKEN;
                }
                else if (string.IsNullOrEmpty(transactionJSON))
                {
                    error = ErrorCodes.MISSING_FIELDS;
                }
                else
                {
                    try
                    {
                        transactionData = JsonConvert.DeserializeObject<TransactionJsonModel>(param.transaction);
                        if (string.IsNullOrEmpty(transactionData.username)
                            || string.IsNullOrEmpty(transactionData.order_id)
                            || transactionData.original_price == null
                            || transactionData.original_final_amount == null
                            || string.IsNullOrEmpty(transactionData.original_currency)
                            || transactionData.discount_percentage == null
                            || string.IsNullOrEmpty(transactionData.description)
                            || string.IsNullOrEmpty(transactionData.payment_method))
                        {
                            error = ErrorCodes.MISSING_FIELDS;
                        }
                        else if (!api.GetUserByUserName(transactionData.username).HasData)
                        {
                            error = ErrorCodes.NON_EXISTING_USER;
                        }
                        else if (api.GetTransaction(partner.identifier, transactionData.order_id).HasData)
                        {
                            error = ErrorCodes.EXISTING_ORDERID;
                        }
                        else
                        {
                            string currencyName = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                               .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == transactionData.original_currency)
                               .Select(c => new RegionInfo(c.LCID).CurrencyEnglishName)
                               .FirstOrDefault();
                            if (string.IsNullOrEmpty(currencyName))
                            {
                                error = ErrorCodes.INVALID_CURRENCY_CODE;
                            }
                        }
                    }
                    catch
                    {
                        error = ErrorCodes.INVALID_JSON_TRANSACTION;
                    }
                }
            }
            if (error == null)
            {
                newTrans = api.CreateTransaction(partner.identifier, ip_address, transactionData);
                if (!newTrans.HasData)
                {
                    error = newTrans.Error;
                }

                if (newTrans.Data.payment_method == Helper.GetDescription(PaymentMethod.TopUpCard) && newTrans.Data.is_venvici_applicable)
                {
                    VenviciAPI.Instance.UpdateVenviciBalance(partner, newTrans.Data);
                }

            }

            string errorName = error.ToErrorCode();
            api.LogApi("1", "/transaction/create-transaction", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id,
                transactionData != null ? transactionData.username : string.Empty,
                IpHelper.GetClientIp(Request), errorName, JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = errorName,
                transaction = error == null ? newTrans.Data.ToDictationary() : null
            };
        }

        [HttpPost]
        [ActionName("retrieve-transaction")]
        [ExecutionMeter]
        public object RetrieveTransaction([FromBody] APIRetrievTransactioneModel param)
        {
            var api = Platform.Core.Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            string transactionId = param.gtoken_transaction_id;
            string orderId = param.order_id;
            ErrorCodes? error = null;
            Transaction transaction = null;


            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (!partner.IsValidHash(param.hashed_token))
                error = ErrorCodes.INVALID_HASHED_TOKEN;
            else
            {
                var result = api.GetTransaction(partner.identifier, orderId, transactionId);
                transaction = result.Data;
                error = result.Error;
            }

            // Log //
            string errorCode = error.ToErrorCode();
            api.LogApi("1", "/transaction/retrieve-transaction", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, string.Empty,
                IpHelper.GetClientIp(Request),
                errorCode, JsonConvert.SerializeObject(param));

            // Output //
            return new
            {
                success = (error == null),
                transaction = transaction != null ? transaction.ToDictationary() : null,
                error_code = errorCode,
                message = error.ToErrorMessage()
            };
        }

        [HttpPost]
        [ActionName("execute-transaction")]
        [ExecutionMeter]
        public object ExecuteTransaction([FromBody] APIExecuteTransactioneModel param)
        {
            var api = Platform.Core.Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            string transactionId = param.gtoken_transaction_id;
            string status = param.status;
            TransactionStatus? stat = EnumConverter.EnumFromDescription<TransactionStatus>(status);
            ErrorCodes? error = null;
            Transaction transaction = null;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (!partner.IsValidHash(param.hashed_token))
                error = ErrorCodes.INVALID_HASHED_TOKEN;
            else if (string.IsNullOrEmpty(status) || string.IsNullOrEmpty(transactionId))
                error = ErrorCodes.MISSING_FIELDS;
            else if (!ConstantValues.ListOfValidStatus.Contains(status) || !stat.HasValue)
                error = ErrorCodes.INVALID_TRANSACTION_STATUS;
            else
            {
                var result = api.GetTransaction(partner.identifier, null, transactionId);
                transaction = result.Data;
                error = result.Error;
            }

            // Log //
            string errorCode = error.ToErrorCode();
            api.LogApi("1", "/transaction/execute-transaction", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, string.Empty,
                IpHelper.GetClientIp(Request),
                errorCode, JsonConvert.SerializeObject(param));

            if (error == null)
            {
                if (api.UpdateTransactionStatus(transaction.id, stat.Value))
                {
                    if (status == ConstantValues.S_SUCCESS && transaction.is_venvici_applicable)
                    {
                        VenviciAPI.Instance.UpdateVenviciBalance(partner, transaction);
                    }

                    transaction.status = status;
                }
            }

            // Output //
            return new
            {
                success = error == null,
                transaction = transaction != null ? transaction.ToDictationary() : null,
                error_code = errorCode,
                message = error.ToErrorMessage()
            };
        }

        [HttpPost]
        [ActionName("record-token-transaction")]
        [ExecutionMeter]
        public object RecordTransaction([FromBody] APIRecordTokenTransactionModel param)
        {
            var api = Platform.Core.Api.Instance;
            string partnerUID = param.partner_id;
            Partner partner = api.GetPartnerById(partnerUID).Data;
            string ip_address = !string.IsNullOrEmpty(param.ip_address) ? param.ip_address : GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request);
            if (string.IsNullOrEmpty(ip_address))
            {
                ip_address = "127.0.0.1";
            }
            string hashed_token = param.hashed_token;

            ErrorCodes? error = null;
            TokenTransactionJson transactionData = null;
            Result<TokenTransaction> newTrans = null;
            if (partner == null)
            {
                error = ErrorCodes.INVALID_PARTNER_ID;
            }
            else
            {
                if (!partner.IsValidHash(hashed_token))
                {
                    error = ErrorCodes.INVALID_HASHED_TOKEN;
                }
                else if (string.IsNullOrEmpty(param.token_transaction))
                {
                    error = ErrorCodes.MISSING_FIELDS;
                }
                else
                {
                    try
                    {
                        transactionData = JsonConvert.DeserializeObject<TokenTransactionJson>(param.token_transaction);
                    }
                    catch
                    {
                        error = ErrorCodes.INVALID_JSON_TRANSACTION;
                    }

                    if (error == null)
                    {
                        if (string.IsNullOrEmpty(transactionData.username)
                            || string.IsNullOrEmpty(transactionData.order_id)
                            || transactionData.amount == null
                            || string.IsNullOrEmpty(transactionData.token_type)
                            || string.IsNullOrEmpty(transactionData.transaction_type)
                            || string.IsNullOrEmpty(transactionData.description))
                        {
                            error = ErrorCodes.MISSING_FIELDS;
                        }
                        else if (!api.GetUserByUserName(transactionData.username).HasData)
                        {
                            error = ErrorCodes.NON_EXISTING_USER;
                        }
                        else if (api.GetTransaction(partner.identifier, transactionData.order_id).HasData)
                        {
                            error = ErrorCodes.EXISTING_ORDERID;
                        }
                        else if (!ConstantValues.ListOfTransactionType.Contains(transactionData.transaction_type))
                        {
                            error = ErrorCodes.INVALID_TRANSACTION_TYPE;
                        }
                    }
                }
            }

            if (error == null)
            {
                newTrans = api.CreateTokenTransaction(partner.identifier, ip_address, transactionData);
                if (!newTrans.HasData)
                {
                    error = newTrans.Error;
                }
            }

            api.LogApi("1", "/transaction/record-token-transaction", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, transactionData != null ? transactionData.username : string.Empty,
                GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request), error.ToErrorCode(),
                JsonConvert.SerializeObject(param));
            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                token_transaction = error == null ? newTrans.Data.ToDictationary() : null
            };
        }


        [HttpPost]
        [ActionName("direct-charge-gtoken")]
        [ExecutionMeter]
        public object DirectChargeTransaction([FromBody] APIDirectChargeGtokenModel param)
        {
            var api = Platform.Core.Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            string ip_address = !string.IsNullOrEmpty(param.ip_address) ? param.ip_address : IpHelper.GetClientIp(Request);
            if (string.IsNullOrEmpty(ip_address))
            {
                ip_address = "127.0.0.1";
            }
            ErrorCodes? error = null;
            Result<DirectGtokenTransaction> newTrans = null;
            APIDirectGTokenTransactionResult transactionData = JsonHelper.DeserializeObject<APIDirectGTokenTransactionResult>(param.direct_gtoken_transaction);

            if (partner == null)
            {
                error = ErrorCodes.INVALID_PARTNER_ID;
            }
            else if (!partner.IsValidHash(param.hashed_token))
            {
                error = ErrorCodes.INVALID_HASHED_TOKEN;
            }
            else if (transactionData == null)
            {
                error = ErrorCodes.INVALID_JSON_TRANSACTION;
            }
            else if (string.IsNullOrEmpty(transactionData.username)
                    || string.IsNullOrEmpty(transactionData.order_id)
                    || transactionData.amount == null
                    || string.IsNullOrEmpty(transactionData.description))
            {
                error = ErrorCodes.MISSING_FIELDS;
            }
            else if (!api.GetUserByUserName(transactionData.username).HasData)
            {
                error = ErrorCodes.NON_EXISTING_USER;
            }
            else if (api.GetDirectGTokenTransaction(partner.identifier, transactionData.order_id).HasData)
            {
                error = ErrorCodes.EXISTING_ORDERID;
            }
            else if (!VenviciAPI.Instance.CheckGToken(transactionData.username, transactionData.amount ?? 0))
            {
                error = ErrorCodes.INSUFFICIENT_AMOUNT;
            }

            if (error == null)
            {
                newTrans = api.CreateDirectGTokenTransaction(partner.identifier, transactionData.username, ip_address, transactionData);
                if (!newTrans.HasData)
                {
                    error = newTrans.Error;
                }
            }

            api.LogApi("1", "/transaction/direct-charge-gtoken", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id,
                transactionData != null ? transactionData.username : string.Empty,
                IpHelper.GetClientIp(Request), error.ToErrorCode(),
                JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                token_transaction = (error == null && newTrans.HasData) ? newTrans.Data.ToDictionary() : null
            };
        }
    }
}
