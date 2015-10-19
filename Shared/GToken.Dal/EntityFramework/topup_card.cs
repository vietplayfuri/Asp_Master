//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Platform.Dal.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class topup_card
    {
        public topup_card()
        {
            this.coin_transaction = new HashSet<coin_transaction>();
            this.free_coin_transaction = new HashSet<free_coin_transaction>();
        }
    
        public int id { get; set; }
        public Nullable<int> customer_account_id { get; set; }
        public string card_number { get; set; }
        public string card_password { get; set; }
        public int amount { get; set; }
        public System.DateTime validity_date { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> used_at { get; set; }
        public System.DateTime created_at { get; set; }
        public Nullable<bool> is_free { get; set; }
        public Nullable<bool> is_bv { get; set; }
        public string currency { get; set; }
        public Nullable<decimal> price { get; set; }
    
        public virtual ICollection<coin_transaction> coin_transaction { get; set; }
        public virtual customer_account customer_account { get; set; }
        public virtual ICollection<free_coin_transaction> free_coin_transaction { get; set; }
    }
}