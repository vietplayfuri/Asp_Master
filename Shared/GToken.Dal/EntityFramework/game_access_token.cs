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
    
    public partial class game_access_token
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public int game_id { get; set; }
        public string token { get; set; }
        public string data { get; set; }
        public string meta { get; set; }
        public System.DateTime saved_at { get; set; }
        public string stats { get; set; }
    
        public virtual customer_account customer_account { get; set; }
        public virtual game game { get; set; }
    }
}
