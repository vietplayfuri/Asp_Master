using System;

namespace Platform.Models
{
    public class GCoinTransaction : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public decimal? amount { get; set; }
        public int? game_id { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
        public string order_id { get; set; }
        public string status { get; set; }
        public string ip_address { get; set; }
        public string country_code { get; set; }
        public string payment_method { get; set; }
        public string sender_email { get; set; }
        public string receiver_email { get; set; }
        public string pay_key { get; set; }
        public DateTime? pay_key_expiration_date { get; set; }
        public DateTime? updated_at { get; set; }
       
    }

  
}
