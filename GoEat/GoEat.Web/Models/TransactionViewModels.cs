using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GoEat.Dal.Models;
using System;
using GoEat.Models;
using GoEat.Utility.Crytography;
using System.Linq;

namespace GoEat.Web.Models
{
    public class TransactionViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string order_id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public decimal price { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(0, 1, ErrorMessage = "Discount rate must be from 0 to 1")]
        public decimal discount_percentage { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public decimal final_amount { get; set; }

        [Required]
        public int customer_id { get; set; }

        public string nickname { get; set; }

        public int cashier_id { get; set; }

        [Required]
        public int restaurant_id { get; set; }

        public decimal gst { get; set; }
        public decimal service_charge { get; set; }
        public decimal rateTokenWithSGD { get; set; }
        public decimal token_balance { get; set; }
        public bool is_venvici_member { get; set; }
        public string username { get; set; }
    }

    public class TransactionClient
    {
        [Required(ErrorMessage = "This field is required")]
        public string order_id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public decimal original_price { get; set; }

        public string original_currency { get; set; }//default is SGD

        [Required]
        public int customer_id { get; set; }

        public int cashier_id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int restaurant_id { get; set; }

        public string payment_method { get; set; }
    }

    public class TransactionsViewModel
    {
        public TransactionsViewModel()
        {
            timeZone = "Singapore Standard Time";
        }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string status { get; set; }
        public string export { get; set; }
        public string timeZone { get; set; }
        public List<HistoryTransaction> transactions { get; set; }
    }
    public class BarcodeToken
    {
        [IsValidBarcode]
        public string token { get; set; }
    }

    public class TransactionModel
    {
        [Required]
        public int customer_id { get; set; }
        public decimal original_price { get; set; }
        public decimal discount_percentage { get; set; }
        public int restaurant_id { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public int main_transaction_id { get; set; }
        public decimal cash_amount { get; set; }
        public int drinks_quantity { get; set; }
    }

    /// <summary>
    /// Used for CheckTransactionStatus and CancelTransaction API
    /// </summary>
    public class MainTransactionInfo
    {
        public int MainTransactionId { get; set; }
    }
    public class BuyTokenViewModel
    {
        public decimal TotalToken { get; set; }
    }

    /// <summary>
    /// This class will be shown for customer confirm
    /// </summary>
    public class TransactionInformation
    {
        public decimal amount { get; set; }
        public string order_id { get; set; }
        public int main_transaction_id { get; set; }
        public string token_amount { get; set; }
        public string cash_amount { get; set; }
        public decimal service_charge { get; set; }
        public decimal gst { get; set; }
        public decimal discount { get; set; }
        public decimal original_price { get; set; }
        public int quantity { get; set; }
    }

    public class ResendTransactionInformation
    {
        public decimal amount { get; set; }
        public string order_id { get; set; }
        public int main_transaction_id { get; set; }
        public string token_amount { get; set; }
        public string cash_amount { get; set; }
        public decimal service_charge { get; set; }
        public decimal gst { get; set; }
        public decimal discount { get; set; }
        public decimal original_price { get; set; }
    }

    public class TransactionPayPal
    {
        public string sku { get; set; }
    }
    public class PaypalSuccess
    {
        public decimal amount { get; set; }
    }

    public class UserConfirmTransaction
    {
        public int id { get; set; }

    }

    /// <summary>
    /// Used for get-transactions api
    /// </summary>
    public class APIGetTransactionsModel
    {
        /// <summary>
        /// It must be in TransactionStatus
        /// </summary>
        public string status { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
    }

    public class IsValidMethodAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;

            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return ConstantValues.KindsOfTransactionType.Contains(value.ToString().ToLower());
            }
            return false;
        }
    }

    public class IsValidBarcodeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;

            if (!string.IsNullOrEmpty(value.ToString()))
            {
                var tokenDecode = Cryptogahpy.Base64Decode(value.ToString());
                if (string.IsNullOrEmpty(tokenDecode) && tokenDecode.Split('-').Count() == 3)
                {
                    return true;
                }
            }
            return false;
        }
    }
}