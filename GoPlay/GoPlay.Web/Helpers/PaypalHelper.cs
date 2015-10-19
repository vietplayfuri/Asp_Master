using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal.AdaptiveAccounts;
using System.Configuration;

namespace GoPlay.Web.Helpers
{
    public class PaypalHelper
    {
        public static string GetPaypalConfig(string key)
        {
            var config = ConfigManager.Instance.GetProperties();
            return config[key];
        }

        public APIContext GetGetAPIContext()
        {
            var config = ConfigManager.Instance.GetProperties();
            string accessToken = new OAuthTokenCredential(config["clientId"], config["clientSecret"], config).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken);
            apiContext.Config = config;
            return apiContext;
        }

        public RedirectUrls CreateRedirectUrls(string returnUrl, string cancelUrl)
        {
            return new RedirectUrls()
            {
                return_url = returnUrl,
                cancel_url = cancelUrl
            };
        }
        public Payment CreatePayment(APIContext apiContext, List<Transaction> paypalTransactions, RedirectUrls redirectUrls)
        {

            var payment = Payment.Create(apiContext, new Payment()
            {
                intent = "sale",
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                transactions = paypalTransactions,
                redirect_urls = redirectUrls
            });
            return payment;
        }

        public Transaction CreateTransaction(string txDescription, Amount txAmount, List<Item> txItems)
        {
            return new Transaction()
            {
                description = txDescription,
                amount = txAmount,
                item_list = new ItemList()
                {
                    items = txItems
                }
            };
        }
        public Transaction CreateTransaction(string txDescription, string txInvoiceNumber, Amount txAmount, List<Item> txItems)
        {
            return new Transaction()
            {
                description = txDescription,
                invoice_number = txInvoiceNumber,
                amount = txAmount,
                item_list = new ItemList()
                {
                    items = txItems
                }
            };
        }

        public Amount CreateAmount(string amoutCurrency, decimal amoutTotal)
        {
            return new Amount()
            {
                currency = amoutCurrency,
                total = amoutTotal.ToString("#.##")
            };
        }
        public Amount CreateAmount(string amoutCurrency, float amoutTotal, int detailTax, int detailShipping, int detailSubtotal)
        {
            return new Amount()
            {
                currency = amoutCurrency,
                total = amoutTotal.ToString(),
                details = new Details()
                {
                    tax = detailTax.ToString(),
                    shipping = detailShipping.ToString(),
                    subtotal = detailSubtotal.ToString()
                }
            };
        }
        public Item CreateItem(string itemName, string itemCurrency, decimal itemPrice, int itemQuantity, string itemSKU)
        {
            return new Item()
            {
                name = itemName,
                currency = itemCurrency,
                price = itemPrice.ToString("#.##"),
                quantity = itemQuantity.ToString(),
                sku = itemSKU
            };
        }

        public Payment Find(APIContext apiContext, string paymentId)
        {
            return Payment.Get(apiContext, paymentId);
        }

        public static bool isReceiverEmailVerified(string email)
        {
            AdaptiveAccountsService adapAccount = new AdaptiveAccountsService(GetHeader());
            PayPal.AdaptiveAccounts.Model.RequestEnvelope envelope = new PayPal.AdaptiveAccounts.Model.RequestEnvelope()
            {
                errorLanguage = "en_US",
                detailLevel = PayPal.AdaptiveAccounts.Model.DetailLevelCode.RETURNALL
            };
            PayPal.AdaptiveAccounts.Model.GetVerifiedStatusRequest verifyModel = new PayPal.AdaptiveAccounts.Model.GetVerifiedStatusRequest()
            {
                emailAddress = email,
                requestEnvelope = envelope,
                matchCriteria = "NONE"
            };
            var response = adapAccount.GetVerifiedStatus(verifyModel);
            return response.responseEnvelope.ack == PayPal.AdaptiveAccounts.Model.AckCode.SUCCESS &&
                   response.accountStatus == "VERIFIED";
        }

        public static Dictionary<string, string> GetHeader()
        {

            var config = ConfigManager.Instance.GetProperties();

            //#Set our headers
            var headers = new Dictionary<string, string>() {
                {"account1.apiUsername", config["account0.apiUsername"]},
                {"account1.apiPassword",config["account0.apiPassword"]},
                {"account1.apiSignature",config["account0.apiSignature"]},
                {"account1.applicationId", config["account0.applicationId"]},
                //#'X-PAYPAL-SERVICE-VERSION':'1.1.0',
                //{"X-PAYPAL-REQUEST-DATA-FORMAT", ConfigurationManager.AppSettings["PAYPAL_NV"]},
                //{"X-PAYPAL-RESPONSE-DATA-FORMAT", ConfigurationManager.AppSettings["PAYPAL_RESPONSE_DATA_FORMAT"]},
            };

            if (!String.IsNullOrEmpty(config["mode"]))
            {
                headers["mode"] = config["mode"];
            }

            return headers;
        }

    }
}