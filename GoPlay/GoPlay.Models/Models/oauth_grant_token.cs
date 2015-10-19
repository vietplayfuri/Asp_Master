namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class oauth_grant_token
    {
        public int id { get; set; }
        
        public string client_id { get; set; }

        public int user_id { get; set; }
        
        public string code { get; set; }

        public string C_scopes { get; set; }

        public DateTime expires { get; set; }

        public string redirect_uri { get; set; }

    }
}
