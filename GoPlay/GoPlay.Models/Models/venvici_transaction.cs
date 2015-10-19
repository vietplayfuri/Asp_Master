namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class venvici_transaction
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public string username { get; set; }
        public decimal? gtoken_deduct_amount { get; set; }
        public decimal? commission_credit_amount { get; set; }
        public decimal? pushbv_amount { get; set; }

        public DateTime created_at { get; set; }
    }
}
