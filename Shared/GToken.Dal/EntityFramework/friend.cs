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
    
    public partial class friend
    {
        public int friend1_id { get; set; }
        public int friend2_id { get; set; }
        public System.DateTime sent_at { get; set; }
        public string status { get; set; }
    
        public virtual customer_account customer_account { get; set; }
        public virtual customer_account customer_account1 { get; set; }
    }
}