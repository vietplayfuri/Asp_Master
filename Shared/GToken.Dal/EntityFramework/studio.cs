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
    
    public partial class studio
    {
        public studio()
        {
            this.admin_account = new HashSet<admin_account>();
            this.games = new HashSet<game>();
            this.customer_account = new HashSet<customer_account>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public System.DateTime created_at { get; set; }
        public System.DateTime updated_at { get; set; }
        public bool is_archived { get; set; }
    
        public virtual ICollection<admin_account> admin_account { get; set; }
        public virtual ICollection<game> games { get; set; }
        public virtual ICollection<customer_account> customer_account { get; set; }
    }
}