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
    
    public partial class credit_balance
    {
        public int id { get; set; }
        public int credit_type_id { get; set; }
        public int customer_account_id { get; set; }
        public int balance { get; set; }
    
        public virtual credit_type credit_type { get; set; }
        public virtual customer_account customer_account { get; set; }
    }
}
