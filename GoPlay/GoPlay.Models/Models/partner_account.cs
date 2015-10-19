namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class partner_account
    {
        public partner_account()
        {
        }

        public int id { get; set; }

        public string name { get; set; }

        public string client_id { get; set; }
        
        public string client_secret { get; set; }

        public string C_redirect_uris { get; set; }

        public string C_default_scopes { get; set; }
        
        public string client_type { get; set; }

    }
}
