using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal.Api;

namespace GoEat.Web.Helpers
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
    }
}