using System;
using GoEat.Models;
using System.Collections.Generic;

namespace GoEat.Dal.Models
{
    public class GRecordTokenTransaction : ModelBase
    {
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string token_type { get; set; }
        public string transaction_type { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }
        public decimal tax { get; set; }
        public decimal service_charge { get; set; }
    }

    //this model user for calling Gtoken API
    public class GTokenTransaction : ModelBase
    {
        public int id { get; set; }
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public decimal price { get; set; }
        public decimal final_amount { get; set; }
        public string currency { get; set; }
        public decimal original_price { get; set; }
        public decimal original_final_amount { get; set; }
        public string original_currency { get; set; }
        public decimal discount_percentage { get; set; }
        public string status { get; set; }
        public string payment_method { get; set; }
        public string description { get; set; }
        public bool is_venvici_applicable { get; set; }
        public decimal original_tax { get; set; }
        public decimal original_service_charge { get; set; }
        public decimal revenue_percentage { get; set; }
    }

    /// <summary>
    /// cash_transaction table
    /// </summary>
    public class CashTransaction : ModelBase
    {
        public int id { get; set; }
        public decimal amount { get; set; }
        public string original_currency { get; set; }
        public string currency { get; set; }
        public decimal original_price { get; set; }
        public decimal discount_percentage { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string description { get; set; }
        public decimal gst { get; set; }
        public decimal service_charge { get; set; }
    }

    /// <summary>
    /// main_transaction table
    /// </summary>
    public class MainTransaction : ModelBase
    {
        public int id { get; set; }
        public int cashier_id { get; set; }
        public int food_status { get; set; }
        public int drink_status { get; set; }
        public DateTime created_date { get; set; }
        public string order_id { get; set; }
        public decimal amount { get; set; }
        public string method { get; set; }
        public int restaurant_id { get; set; }
        public int customer_id { get; set; }
        public string food_gtoken_transaction_id { get; set; }
        public string drink_gtoken_transaction_id { get; set; }
        public string original_currency { get; set; }
        public string currency { get; set; }
        public decimal original_price { get; set; }
        public decimal discount_percentage { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string description { get; set; }
        public decimal gst { get; set; }
        public decimal service_charge { get; set; }
        public decimal token_amount { get; set; }
        public decimal cash_amount { get; set; }
        public int drinks { get; set; }
        public int beer { get; set; }
        public int others { get; set; }
        public string misc { get; set; }
    }

    /// <summary>
    /// token_transaction table
    /// </summary>
    public class TokenTransaction : ModelBase
    {
        public int id { get; set; }
        public decimal amount { get; set; }
        public string original_currency { get; set; }
        public string currency { get; set; }
        public decimal original_price { get; set; }
        public decimal discount_percentage { get; set; }
        public string paypal_redirect_urls { get; set; }
        public string paypal_payment_id { get; set; }
        public int? token_package_id { get; set; }

        public string description { get; set; }
        public decimal gst { get; set; }
        public decimal service_charge { get; set; }
        public int quantity { get; set; }
    }

    public class ConfirmTransaction : ModelBase
    {
        public int id { get; set; }
        public int cash_transaction_id { get; set; }
        public int token_transaction_id { get; set; }
        public int customer_id { get; set; }
        public decimal original_price { get; set; }
        public decimal gst { get; set; }
        public decimal service_charge { get; set; }
        public decimal discount_percentage { get; set; }
        public int restaurant_id { get; set; }
        public string status { get; set; }
        public int cashier_id { get; set; }
        public string order_id { get; set; }
        public decimal subtotal { get; set; }
        public decimal token_amount { get; set; }
        public decimal cash_amount { get; set; }
        public string method { get; set; }
        public string paypal_payment_id { get; set; }
    }

    public class ModifyTransaction : ModelBase
    {
        public int id { get; set; }
        public int customer_id { get; set; }
        public decimal original_price { get; set; }
        public decimal discount_percentage { get; set; }
        public decimal gst { get; set; }
        public decimal service_charge { get; set; }
        public int restaurant_id { get; set; }
        public int drink_status { get; set; }
        public int cashier_id { get; set; }
        public string order_id { get; set; }
        public decimal token_amount { get; set; }
        public decimal cash_amount { get; set; }
        public int drinks { get; set; }
    }

    public class HistoryTransaction : ModelBase
    {
        public string restaurant_name { get; set; }
        //public int restaurant_id { get; set; }
        public string order_id { get; set; }
        public int customer_id { get; set; }
        public string username { get; set; }

        /// <summary>
        /// final amount which user have to pay by cash
        /// </summary>
        public decimal amount { get; set; }
        public decimal original_price { get; set; }
        public decimal token_amount { get; set; }
        public string method { get; set; }
        public DateTime created_date { get; set; }
    }

    public class ExportTransaction : ModelBase
    {
        public int id { get; set; }
        public string restaurant_name { get; set; }
        public string username { get; set; }
        public string order_id { get; set; }
        public int food_status { get; set; }
        public decimal amount { get; set; }
        public string method { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string currency { get; set; }
        public string original_currency { get; set; }
        public decimal original_price { get; set; }
        public decimal discount_percentage { get; set; }
        public decimal gst { get; set; }
        public decimal service_charge { get; set; }
        public string description { get; set; }
        public DateTime created_date { get; set; }
    }

    /// <summary>
    /// Used for edit profile API: /api/1/account/edit-profile
    /// </summary>
    public class GEditProfile : ModelBase
    {
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string token_type { get; set; }
        public string transaction_type { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }
    }
    public class TokenPackage : ModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public decimal price { get; set; }
        public string currency { get; set; }
        public decimal token_amount { get; set; }
        public bool is_deleted { get; set; }
        public string sku { get; set; }
    }

    /// <summary>
    /// Used for update final transaction
    /// </summary>
    public class ReconcileTransaction
    {
        public ReconcileTransaction()
        {
            success_ids = new List<string>();
            fail_ids = new List<string>();
        }
        public List<string> success_ids { get; set; }
        public List<string> fail_ids { get; set; }
    }

    public class ReconcileTransactionResult
    {
        public ReconcileTransactionResult()
        {
            success = new List<string>();
            fail = new List<ReconcileTransactionItem>();
        }
        public List<string> success { get; set; }
        public List<ReconcileTransactionItem> fail { get; set; }
    }

    public class ReconcileTransactionItem
    {
        public string id { get; set; }
        public string reason { get; set; }
    }

    /// <summary>
    /// Used for returning from api get all transactions by status
    /// </summary>
    public class ReconcileTransactionModel
    {
        public int id { get; set; }
        public DateTime created_date { get; set; }
    }

    public class DirectChargeGtokenModel : ModelBase
    {
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }
    }

    public class SimpleTransaction : ModelBase
    {
        public int id { get; set; }
        public string username { get; set; }
        public decimal token_amount { get; set; }
        public string order_id { get; set; }
        public string description { get; set; }
    }
}
