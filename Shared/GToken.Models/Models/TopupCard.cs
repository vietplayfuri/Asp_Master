﻿using System;

namespace Platform.Models
{
    public class TopUpCard : ModelBase
    {
        public int id { get; set; }
        public int? customer_account_id { get; set; }
        public string card_number { get; set; }
        public string card_password { get; set; }
        public int amount { get; set; }
        public DateTime validity_date { get; set; }
        public string status { get; set; }
        public DateTime? used_at { get; set; }
        public DateTime created_at { get; set; }
        public bool? is_free { get; set; }
        public bool? is_bv { get; set; }
        public string currency { get; set; }
        public decimal? price { get; set; }

    }

  
}
