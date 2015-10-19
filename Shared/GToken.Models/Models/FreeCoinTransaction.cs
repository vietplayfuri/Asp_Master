using System;

namespace Platform.Models
{
    public class FreeCoinTransaction : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public decimal? amount { get; set; }
        public int? game_id { get; set; }
        public int? credit_type_id { get; set; }
        public int? package_id { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string order_id { get; set; }
        public int? topup_card_id { get; set; }
        public string payment_method { get; set; }
        public decimal? price { get; set; }
        public string ip_address { get; set; }
        public string country_code { get; set; }
       
    }

  
}
