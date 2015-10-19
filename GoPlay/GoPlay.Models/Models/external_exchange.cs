namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    
    public class external_exchange
    {
        public int id { get; set; }

        public int customer_account_id { get; set; }

        public int game_id { get; set; }

        public int? package_id { get; set; }

        public int? credit_type_id { get; set; }

        public string exchange_option_identifier { get; set; }

        public DateTime created_at { get; set; }

        public string transaction_id { get; set; }

    }
}
