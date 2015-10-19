namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    
    public class paypal_preapproval
    {
        public int id { get; set; }

        public DateTime created_at { get; set; }

        public DateTime starting_date { get; set; }

        public DateTime ending_date { get; set; }

        
        public decimal max_amount_per_payment { get; set; }

        public int max_number_of_payments { get; set; }

        
        public decimal max_total_amount_of_all_payments { get; set; }

        public string sender_email { get; set; }

        public string preapproval_key { get; set; }

        public int current_number_of_payments { get; set; }

        
        public decimal current_total_amount_of_all_payments { get; set; }

        public bool is_active { get; set; }
        public string flag { get; set; }
    }
}
