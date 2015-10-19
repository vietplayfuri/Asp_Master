using System;

namespace Platform.Models
{
    public class CreditTransaction : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public int? coin_transaction_id { get; set; }
        public decimal? amount { get; set; }
        public int? game_id { get; set; }
        public int? credit_type_id { get; set; }
        public int? package_id { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public int? free_coin_transaction_id { get; set; }
       
    }

  
}
