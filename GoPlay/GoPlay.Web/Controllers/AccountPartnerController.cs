using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.Models;
using GoPlay.Web.ActionFilter;
using Newtonsoft.Json;
using Platform.Models;
using Platform.Models.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Controllers
{
    public class AccountPartnerController : BaseController
    {
        [Route("partner/1/profile")]
        public string profile()
        {
            return "Hello world!";
        }

        [HttpPost]
        [Route("partner/0/update-vip-status")]
        public object updateVIPStatus(PartnerViewModel param)
        {
            var api = GoPlayApi.Instance;
            string username = param.username;
            var user = api.GetUserByUserName(username).Data;
            string status = param.status;
            var partner = api.GetPartner(ConfigurationManager.AppSettings["PARTNER_CLIENT_ID"]).Data;
            ErrorCodes? err = null;

            if (partner == null)
                err = ErrorCodes.INVALID_PARTNER_ID;
            else if (user == null)
                err = ErrorCodes.NON_EXISTING_USER;
            else
            {
                string tokenRawString = username + status + partner.client_id;
                string generatedToken = Helper.CalculateMD5Hash(tokenRawString);

                if (string.Compare(generatedToken, param.token, true) != 0)
                    err = ErrorCodes.INVALID_TOKEN;
            }
            if (err == null)
            {
                if (!api.UpdateCustomerAccount(user.id, user.email, user.nickname, user.gender, status))
                    err = ErrorCodes.ServerError;
                else
                {
                    var notification = new user_notification();
                    if (status == EUserStatus.ClassicReseller.ToDescription())
                        notification.game_notification_id = (int)EUserStatus.ClassicReseller;
                    else if (status == EUserStatus.GoldReseller.ToDescription())
                        notification.game_notification_id = (int)EUserStatus.GoldReseller;
                    notification.customer_account_id = user.id;
                    api.CreateUserNotification(notification);
                }
            }
            api.LogApi("1", "/partner/update-vip-status", err == null,
               Request.UserAgent != null ? Request.UserAgent.ToString() : string.Empty,
               0, user == null ? 0 : user.id, HttpContext.Request.UserHostAddress,
               err.ToErrorCode(), JsonConvert.SerializeObject(param));
            return JsonConvert.SerializeObject(new
            {
                success = err == null,
                message = err.ToErrorMessage(),
                error_code = err.ToErrorCode()
            });
        }

        [HttpPost]
        [Route("partner/0/purchase-play-token")]
        public object purchasePlayToken(PartnerViewModel param)
        {
            var api = GoPlayApi.Instance;
            string username = param.username;
            var user = api.GetUserByUserName(username).Data;
            //string status = param.status;
            string token = param.token;
            decimal amount = 0;

            var partner = api.GetPartner(ConfigurationManager.AppSettings["PARTNER_CLIENT_ID"]).Data;
            ErrorCodes? err = null;
            if (partner == null)
            {
                err = ErrorCodes.INVALID_PARTNER_ID;
            }
            else if (user == null)
            {
                err = ErrorCodes.NON_EXISTING_USER;
            }
            else
            {
                string tokenRawString = username + param.amount + partner.client_id;
                string generatedToken = Helper.CalculateMD5Hash(tokenRawString);

                if (generatedToken != token)
                    err = ErrorCodes.INVALID_TOKEN;

                decimal.TryParse(param.amount, out amount);
                if (amount <= 0)
                    err = ErrorCodes.INSUFFICIENT_AMOUNT;
            }
            if (err == null)
            {
                var transaction = new coin_transaction()
                {
                    order_id = Guid.NewGuid().ToString(),
                    customer_account_id = user.id,
                    amount = amount,
                    price = amount,
                    partner_account_id = partner.id,
                    description = string.Format("You received {0} Play Token from {1}", Helper.displayDecimal(amount), partner.name),
                    status = Helper.GetDescription(TransactionStatus.Success)
                };
                var result = api.CreateCoinTransaction(transaction);
                if (!result.HasData)
                    err = result.Error;
                else
                {
                    var tokenTrans = new TokenTransactionJson()
                    {
                        token_type = GoPlayConstantValues.S_PLAY_TOKEN,
                        transaction_type = ConstantValues.S_TRANSACTION_TYPE_CONSUMPTION,
                        username = user.username,
                        order_id = transaction.order_id,
                        description = transaction.description,
                        amount = transaction.amount
                    };
                    api.GTokenAPITransaction(new GtokenModelTransactionAction()
                    {
                        enumAction = EGtokenAction.RecordTransaction,
                        ip_address = Request.UserHostAddress,
                        token_transaction = tokenTrans
                    });
                }

            }

            api.LogApi("1", "/partner/purchase-play-token",
               err == null,
               Request.UserAgent != null ? Request.UserAgent.ToString() : string.Empty,
               0, user == null ? 0 : user.id,
               HttpContext.Request.UserHostAddress,
               err.ToErrorCode(), JsonConvert.SerializeObject(param));

            return JsonConvert.SerializeObject(new
            {
                success = err == null,
                message = err.ToErrorMessage(),
                error_code = err.ToErrorCode()
            });
        }
    }
}