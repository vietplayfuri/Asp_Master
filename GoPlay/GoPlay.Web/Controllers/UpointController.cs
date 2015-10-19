using GoPlay.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay.Web.Models;
using GoPlay.Core;
using GoPlay.Models;
using Platform.Utility;
using Platform.Models;
using System.Configuration;
using System.Net;
using System.Threading;
using GoPlay.Web.Helpers;
using System.Threading.Tasks;

namespace GoPlay.Web.Controllers
{
    public class UpointController : BaseController
    {
        // GET: Upoint
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("upoint/balance-deduction")]
        //[RequiredLogin]
        public JsonResult UpointBalanceDeduction(UPointBalanceDeductionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { errors = Errors(ModelState) });
            }
            string err = null;
            string sms_code = null;
            var api = GoPlayApi.Instance;
            var gtokenPackage = GoPlayApi.Instance.GetGTokenPackage(model.GtokenPackageSKU).Data;
            if (gtokenPackage == null)
                return FailResult("GtokenPackageSKU", ErrorCodes.INVALID_PACKAGE_ID.ToErrorMessage());
            var transaction = GenerateCoinTransaction(gtokenPackage);
            transaction.payment_method = Helper.GetDescription(PaymentMethod.UPoint_Deduction);
            transaction.status = Helper.GetDescription(TransactionStatus.Phone_number_pending);
            transaction = api.CreateCoinTransaction(transaction).Data;
            if (transaction == null)
                err = ErrorCodes.ServerError.ToErrorMessage();
            else
            {
                var result = api.UpointAPI(new APIUpointParamModel(transaction)
                    {
                        enumAction = EUpointAction.BalanceDeduction,
                        phone_number = model.phoneNumber,
                        callback_url = Url.Action("callback", "upoint", null, Request.Url.Scheme)
                    }).Result;
                if (!result.HasData)
                    err = ErrorCodes.ServerError.ToErrorMessage();
                else
                {
                    if (result.Data.result)
                    {
                        transaction.telkom_order_id = result.Data.t_id;
                        sms_code = result.Data.sms_code;
                    }
                    else
                    {
                        transaction.status = Helper.GetDescription(TransactionStatus.Failure);
                        transaction.description += "Error: " + result.Data.error_info;
                        err = result.Data.error_info;
                    }
                    api.UpdateCoinTransaction(transaction.id, transaction.status, transaction.description, transaction.telkom_order_id);
                }
            }
            if (string.IsNullOrEmpty(err))
                return Json(new { correct = true, sms_code = sms_code });
            else
            {
                return Json(new { errors = new { phoneNumber = err } });
            }
        }
        [Route("upoint/speedy")]
        //[RequiredLogin]
        public async Task<JsonResult> UpointSpeedy(UPointSpeedyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { errors = Errors(ModelState) });
            }
            var api = GoPlayApi.Instance;
            var gtokenPackage = GoPlayApi.Instance.GetGTokenPackage(model.GtokenPackageSKU).Data;
            if (gtokenPackage == null)
                return FailResult("GtokenPackageSKU", ErrorCodes.INVALID_PACKAGE_ID.ToErrorMessage());
            var transaction = GenerateCoinTransaction(gtokenPackage);
            bool debug = false;
            bool.TryParse(ConfigurationManager.AppSettings["UPOINT_DEBUG"], out debug);
            if (debug)//# Speedy API doesn't accept amount of 0...
            {
                transaction.amount = decimal.Parse("0.04");
                transaction.price = decimal.Parse("0.04");
            }
            transaction.payment_method = Helper.GetDescription(PaymentMethod.UPoint_Speedy);
            transaction.status = Helper.GetDescription(TransactionStatus.Speedy_pending);
            transaction = api.CreateCoinTransaction(transaction).Data;
            if (transaction == null)
                return FailResult("GtokenPackageSKU", ErrorCodes.ServerError.ToErrorMessage());
            var result = api.UpointAPI(new APIUpointParamModel(transaction)
               {
                   enumAction = EUpointAction.Speedy,
                   phone_number = model.phoneNumber,
                   speedy_number = model.speedyNumber,
                   ip = Request.UserHostAddress
               }).Result;
            if (!result.HasData)
            {
                return FailResult("GtokenPackageSKU", ErrorCodes.ServerError.ToErrorMessage());
            }
            if (!result.Data.result)
            {
                UpdateTransactionFail(transaction, result.Data.error_info);
                return FailResult("phoneNumber", result.Data.error_info);
            }
            var transactionUUID = Guid.Parse(transaction.order_id);
            if (result.Data.trx_id != transactionUUID.ToString("N"))
            {
                var errorMessage = "UPoint incorrect transaction order id: " + result.Data.trx_id;
                UpdateTransactionFail(transaction, errorMessage);
                return FailResult("phoneNumber", errorMessage);
            }
            decimal GtokenRate = 0;
            decimal.TryParse(ConfigurationManager.AppSettings["IDR_PER_GTOKEN_RATE"], out GtokenRate);
            if (result.Data.amount / GtokenRate != transaction.amount)
            {
                var errorMessage = "UPoint incorrect transaction amount " + result.Data.amount;
                UpdateTransactionFail(transaction, errorMessage);
                return FailResult("phoneNumber", errorMessage);
            }
            //TODO
            // # Add view invoice permission for user
            //identity = Identity(transaction.customerAccount.id)
            //identity.provides.add(ItemNeed('action', 'view_invoice', transaction.order_id))
            transaction.telkom_order_id = result.Data.t_id;
            transaction.status = Helper.GetDescription(TransactionStatus.Success);
            api.UpdateCoinTransaction(transaction.id, transaction.status, transaction.description, transaction.telkom_order_id);

            var invoiceTemplate = new InvoiceViewModel()
            {
                transaction = transaction,
                payer = api.GetUserById(transaction.customer_account_id).Data,
                package = gtokenPackage,
                topupCard = transaction.topup_card_id.HasValue ? api.GetTopUpCard(transaction.topup_card_id.Value).Data : null
            };
            if (!(await EmailHelper.SendMailInvoice(invoiceTemplate)))
            {
                string customer_account_id = string.Empty;
                string email = string.Empty;
                if (invoiceTemplate.payer != null)
                {
                    customer_account_id = invoiceTemplate.payer.id.ToString();
                    email = invoiceTemplate.payer.email;
                }
                var errorMessage = "This guy purchased GToken with an improper email " + customer_account_id + " " + email;
                return FailResult("phoneNumber", errorMessage);
            }
            return Json(new { success = true, redirect = "transaction/invoice", order_id = transaction.order_id });
        }

        [HttpPost]
        [Route("upoint/t-money")]
        //[RequiredLogin]
        public JsonResult UpointTMoney(UPointTMoneyModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { errors = Errors(ModelState) });
            }
            var api = GoPlayApi.Instance;
            var gtokenPackage = GoPlayApi.Instance.GetGTokenPackage(model.GtokenPackageSKU).Data;
            if (gtokenPackage == null)
                return FailResult("phoneNumber", ErrorCodes.INVALID_PACKAGE_ID.ToErrorMessage());
            var transaction = GenerateCoinTransaction(gtokenPackage);
            transaction.payment_method = Helper.GetDescription(PaymentMethod.UPoint_TMoney);
            transaction.status = Helper.GetDescription(TransactionStatus.Tmoney_pending);
            transaction = api.CreateCoinTransaction(transaction).Data;
            if (transaction == null)
                return FailResult("phoneNumber", ErrorCodes.ServerError.ToErrorMessage());
            var result = api.UpointAPI(new APIUpointParamModel(transaction)
            {
                enumAction = EUpointAction.TMoney,
                phone_number = model.phoneNumber,
                callback_url = Url.Action("callback", "upoint", null, Request.Url.Scheme)
            }).Result;
            if (!result.HasData)
                return FailResult("phoneNumber", ErrorCodes.ServerError.ToErrorMessage());
            if (!result.Data.result)
            {
                UpdateTransactionFail(transaction, result.Data.error_info);
                return FailResult("phoneNumber", result.Data.error_info);
            }
            var transactionUUID = Guid.Parse(transaction.order_id);
            if (result.Data.trx_id != transactionUUID.ToString("N"))
            {
                var errorMessage = "UPoint incorrect transaction order id: " + result.Data.trx_id;
                UpdateTransactionFail(transaction, errorMessage);
                return FailResult("phoneNumber", errorMessage);
            }
            decimal GtokenRate = 0;
            decimal.TryParse(ConfigurationManager.AppSettings["IDR_PER_GTOKEN_RATE"], out GtokenRate);
            if (result.Data.amount / GtokenRate != transaction.amount)
            {
                var errorMessage = "UPoint incorrect transaction amount " + result.Data.amount;
                UpdateTransactionFail(transaction, errorMessage);
                return FailResult("phoneNumber", errorMessage);
            }
            transaction.telkom_order_id = result.Data.t_id;
            api.UpdateCoinTransaction(transaction.id, transaction.status, transaction.description, transaction.telkom_order_id);

            return Json(new { correct = true, redirect = result.Data.redirect });
        }

        [HttpPost]
        [Route("upoint/callback")]
        //[RequiredLogin]
        public async Task<JsonResult> UpointCallback(APIUpointModel model)
        {
            var api = GoPlayApi.Instance;
            Guid order_id = default(Guid);
            Guid.TryParse(model.trx_id, out order_id);//hex to guid
            coin_transaction transaction = api.GetCoinTransactionByOderId(order_id.ToString()).Data;
            if (transaction == null)
            {
                var err = "UPoint incorrect transaction order id: " + model.trx_id;
                return FailResult("errors", err);
            }
            var user = api.GetUserById(transaction.customer_account_id).Data;
            if (!model.result)
            {
                UpdateTransactionFail(transaction, model.error_info);
                string errorMessage = model.error_info;
                if (user == null || !(await EmailHelper.SendUpointCallBack(model.error_info, user.email)))
                    errorMessage = "This guy purchased GToken with an improper email " + user.id + " " + user.email;
                return FailResult("phoneNumber", errorMessage);
            }

            decimal GtokenRate = 0;
            decimal.TryParse(ConfigurationManager.AppSettings["IDR_PER_GTOKEN_RATE"], out GtokenRate);
            if (model.amount / GtokenRate != transaction.amount)
            {
                var errorMessage = "UPoint incorrect transaction amount " + model.amount;
                return FailResult("phoneNumber", errorMessage);
            }
            //# Add view invoice permission for user
            //identity = Identity(transaction.customerAccount.id)
            //identity.provides.add(ItemNeed('action', 'view_invoice', transaction.order_id))
            if (user.HasDiscount())
            {
                transaction.description += "; Extra 10% on GoPlay Token amount.";
                user.is_discount_permanent = true;
                api.UpdateCustomerAccount(user.id, true);
                if (transaction.amount.HasValue)
                    transaction.amount = transaction.amount.Value / decimal.Parse("1.1");
            }
            transaction.telkom_order_id = model.t_id;
            transaction.status = Helper.GetDescription(TransactionStatus.Success);
            api.UpdateCoinTransaction(transaction.id, transaction.status, transaction.description, transaction.telkom_order_id);
            var invoiceTemplate = new InvoiceViewModel()
            {
                transaction = transaction,
                payer = user,
                package = transaction.gtoken_package_id.HasValue?api.GetGTokenPackage(transaction.gtoken_package_id.Value).Data:null,
                topupCard = transaction.topup_card_id.HasValue ? api.GetTopUpCard(transaction.topup_card_id.Value).Data : null
            };
            if (!(await EmailHelper.SendMailInvoice(invoiceTemplate)))
            {
                var errorMessage = "This guy purchased GToken with an improper email " + user.id + " " + user.email;
                return FailResult("hrn", errorMessage);
            }
            return Json(new { result = true });
        }

        [HttpPost]
        [Route("upoint/telkomsel-voucher")]
        //[RequiredLogin]
        public async Task<JsonResult> UpointTelkomselVoucher(UPointTelkomselVoucherViewModel model)
        {
            var api = GoPlayApi.Instance;
            if (!ModelState.IsValid)
            {
                return Json(new { errors = Errors(ModelState) });
            }

            var result = api.UpointAPI(new APIUpointParamModel()
            {
                enumAction = EUpointAction.GetTicketTelkomsel,
            }).Result;
            if (!result.HasData || result.Data.code != "201")
                return FailResult("hrn", !result.HasData ? ErrorCodes.ServerError.ToErrorMessage() : result.Data.info);

            model.hrn = model.hrn.Replace(" ", "");
            var transaction = GenerateCoinTransaction();
            var transactionUUID = Guid.Parse(transaction.order_id);
            transaction.telkom_order_id = result.Data.ticket;
            transaction.payment_method = Helper.GetDescription(PaymentMethod.UPoint_TelkomselVoucher);
            transaction.description = "HRN: " + model.hrn;
            transaction.status = Helper.GetDescription(TransactionStatus.Telkomsel_voucher_pending);
            transaction = api.CreateCoinTransaction(transaction).Data;
            if (transaction == null)
                return FailResult("hrn", ErrorCodes.ServerError.ToErrorMessage());
            string err = null;
            var upointParam = new APIUpointParamModel(transaction, "GToken")
                {
                    enumAction = EUpointAction.TelkomselVoucher,
                    amount = -1,
                    hrn = model.hrn,
                    ticket = result.Data.ticket
                };
            for (int i = 0; i < 5; i++)
            {
                result = api.UpointAPI(upointParam).Result;
                if (!result.HasData)
                {
                    err = ErrorCodes.ServerError.ToErrorCode();
                    break;
                }
                if (result.Data.code == "200")
                {
                    err = null;
                    break;
                }
                err = result.Data.info;
                Thread.Sleep(5000);//5s
                //# For result code of 100 - Voucher burning is still on progress
                //# Retry 5 times max
            }
            if (err != null)
            {
                UpdateTransactionFail(transaction, err);
                return FailResult("hrn", err);
            }
            if (result.Data.trx_id != transactionUUID.ToString("N"))
            {
                var errorMessage = "UPoint incorrect transaction order id: " + result.Data.trx_id;
                UpdateTransactionFail(transaction, errorMessage);

                return FailResult("hrn", errorMessage);
            }
            var voucherValue = result.Data.nominal.HasValue ? result.Data.nominal.Value : 0;
            var gtokenPackage = api.GetGTokenPackage(voucherValue, "IDR").Data;
            if (gtokenPackage == null)
            {
                var errorMessage = "UPoint incorrect transaction amount: " + voucherValue;
                UpdateTransactionFail(transaction, errorMessage);

                return FailResult("hrn", errorMessage);
            }
            var user = api.GetUserById(transaction.customer_account_id).Data;// require login first, user cannot null
            bool debug = false;
            bool.TryParse(ConfigurationManager.AppSettings["UPOINT_DEBUG"], out debug);
            decimal? totalAmount = 0;
            decimal? gtokenAmount = 0;
            if (!debug)
            {
                gtokenAmount = gtokenPackage.GetPlayToken(user);
                totalAmount = gtokenPackage.price;
            }
            transaction.gtoken_package_id = gtokenPackage.id;
            transaction.description += "; GToken Package: " + gtokenPackage.name;
            if (user.HasDiscount())
            {
                transaction.description += "; Extra 10% on GoPlay Token amount.";
                user.is_discount_permanent = true;
                api.UpdateCustomerAccount(user.id, true);
                if (transaction.amount.HasValue)
                    transaction.amount = transaction.amount.Value / decimal.Parse("1.1");
            }
            //# Add view invoice permission for user
            //identity = Identity(transaction.customerAccount.id)
            //identity.provides.add(ItemNeed('action', 'view_invoice', transaction.order_id))

            transaction.status = Helper.GetDescription(TransactionStatus.Success);
            api.UpdateCoinTransaction(transaction.id, transaction.status, transaction.description, transaction.telkom_order_id);
            var invoiceTemplate = new InvoiceViewModel()
            {
                transaction = transaction,
                payer = user,
                package = gtokenPackage,
                topupCard = transaction.topup_card_id.HasValue ? api.GetTopUpCard(transaction.topup_card_id.Value).Data : null
            };
            if (!(await EmailHelper.SendMailInvoice(invoiceTemplate)))
            {
                var errorMessage = "This guy purchased GToken with an improper email " + user.id + " " + user.email;
                return FailResult("hrn", errorMessage);
            }
            return Json(new { correct = true });
        }

        [HttpPost]
        [Route("upoint/standard-voucher/{voucherType}")]
        //[RequiredLogin]
        public JsonResult UpointTelkomselVoucher(UPointStandardVoucherViewModel model, string voucherType)
        {
            voucherType = voucherType.ToLower();
            if (voucherType != Helper.GetDescription(EVoucherType.Spin) && voucherType != Helper.GetDescription(EVoucherType.Telkom))
                return FailResult("formError", ErrorCodes.INVALID_VOUCHER_TYPE.ToErrorMessage());

            model.hrn = model.hrn.Replace(" ", "");
            model.vsn = model.vsn.Replace(" ", "");

            var api = GoPlayApi.Instance;
            var transaction = GenerateCoinTransaction();
            var transactionUUID = Guid.Parse(transaction.order_id);

            transaction.payment_method = voucherType == Helper.GetDescription(EVoucherType.Telkom)
                                           ? Helper.GetDescription(PaymentMethod.UPoint_Telkom)
                                           : Helper.GetDescription(PaymentMethod.UPoint_SPIN);

            transaction.description = "HRN: " + model.hrn + "; VSN: " + model.vsn;

            transaction.status = voucherType == Helper.GetDescription(EVoucherType.Telkom)
                                           ? Helper.GetDescription(TransactionStatus.Telkom_voucher_pending)
                                           : Helper.GetDescription(TransactionStatus.Spin_voucher_pending);

            transaction = api.CreateCoinTransaction(transaction).Data;
            if (transaction == null)
                return FailResult("hrn", ErrorCodes.ServerError.ToErrorMessage());
            var upointParam = new APIUpointParamModel(transaction, transactionUUID.ToString("N"))
            {
                enumAction = EUpointAction.StandardVoucher,
                hrn = model.hrn,
                vsn = model.vsn
            };
            if (!string.IsNullOrEmpty(model.phoneNumber))
                upointParam.phone_number = model.phoneNumber;
            var result = api.UpointAPI(upointParam).Result;
            if (!result.HasData)
                return FailResult("hrn", ErrorCodes.ServerError.ToErrorMessage());
            if (!string.IsNullOrEmpty(result.Data.error_code))
            {
                UpdateTransactionFail(transaction, result.Data.error_info);
                return FailResult("hrn", result.Data.error_info);
            }
            if (result.Data.trx_id != transactionUUID.ToString("N"))
            {
                var errorMessage = "UPoint incorrect transaction order id: " + result.Data.trx_id;
                UpdateTransactionFail(transaction, errorMessage);

                return FailResult("hrn", errorMessage);
            }
            var nominal = result.Data.nominal.HasValue ? result.Data.nominal.Value : 0;
            var voucherValue = nominal > result.Data.amount ? nominal : result.Data.amount;

            var gtokenPackage = api.GetGTokenPackage(voucherValue, "IDR").Data;
            if (gtokenPackage == null)
            {
                var errorMessage = "UPoint incorrect transaction amount: " + voucherValue;
                UpdateTransactionFail(transaction, errorMessage);

                return FailResult("hrn", errorMessage);
            }
            var user = api.GetUserById(transaction.customer_account_id).Data;// require login first, user cannot null
            bool debug = false;
            bool.TryParse(ConfigurationManager.AppSettings["UPOINT_DEBUG"], out debug);
            decimal? totalAmount = 0;
            decimal? gtokenAmount = 0;
            if (!debug)
            {
                gtokenAmount = gtokenPackage.GetPlayToken(user);
                totalAmount = gtokenPackage.price;
            }
            transaction.gtoken_package_id = gtokenPackage.id;
            transaction.description += "; GToken Package: " + gtokenPackage.name;
            if (user.HasDiscount())
            {
                transaction.description += "; Extra 10% on GoPlay Token amount.";
                user.is_discount_permanent = true;
                if (transaction.amount.HasValue)
                    transaction.amount = transaction.amount.Value / decimal.Parse("1.1");
                api.UpdateCustomerAccount(user.id, true);
            }
            transaction.telkom_order_id = result.Data.t_id;
            transaction.status = Helper.GetDescription(TransactionStatus.Success);
            api.UpdateCoinTransaction(transaction.id, transaction.status, transaction.description, transaction.telkom_order_id);

            return Json(new { success = true });
        }

        private JsonResult FailResult(string key, string err)
        {
            ModelState.AddModelError(key, err);
            return Json(new { errors = Errors(ModelState) });
        }
        private void UpdateTransactionFail(coin_transaction trans, string err)
        {
            trans.status = Helper.GetDescription(TransactionStatus.Failure);
            trans.description += "Error: " + err;
            GoPlayApi.Instance.UpdateCoinTransaction(trans.id, trans.status, trans.description, trans.telkom_order_id);
        }

        private coin_transaction GenerateCoinTransaction(GtokenPackage package = null)
        {
            bool debug = false;
            bool.TryParse(ConfigurationManager.AppSettings["UPOINT_DEBUG"], out debug);
            decimal? totalAmount = 0;
            decimal? gtokenAmount = 0;
            var transaction = new coin_transaction()
            {
                order_id = Guid.NewGuid().ToString(),
                customer_account_id = 3,//TODO/CurrentUser.Id,
            };
            if (package != null)
            {
                transaction.gtoken_package_id = package.id;
                transaction.description = package.name;
            }
            if (!debug && package != null)
            {
                var user = GoPlayApi.Instance.GetUserById(CurrentUser.Id).Data;
                gtokenAmount = package.GetPlayToken(user);
                totalAmount = package.price;
            }
            transaction.amount = gtokenAmount;
            transaction.price = totalAmount;
            return transaction;
        }
    }
}