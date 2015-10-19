using System;

namespace Platform.Models
{
    public class CoinTransaction : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public int? receiver_account_id { get; set; }
        public decimal? amount { get; set; }
        public int? partner_account_id { get; set; }
        public int? game_id { get; set; }
        public int? credit_type_id { get; set; }
        public int? package_id { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public int? topup_card_id { get; set; }
        public string order_id { get; set; }
        public string payment_method { get; set; }
        public int? sender_account_id { get; set; }
        public int? gtoken_package_id { get; set; }
        public string paypal_redirect_urls { get; set; }
        public string paypal_payment_id { get; set; }
        public decimal? price { get; set; }
        public string ip_address { get; set; }
        public string country_code { get; set; }
        public string telkom_order_id { get; set; }
        public bool use_gtoken { get; set; }
       
    }

  
}
