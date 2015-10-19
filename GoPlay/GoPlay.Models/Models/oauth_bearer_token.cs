namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    
    public class oauth_bearer_token
    {
        public int id { get; set; }
        
        public string access_token { get; set; }
        
        public string refresh_token { get; set; }
        
        public string token_type { get; set; }
        
        public string client_id { get; set; }

        public int user_id { get; set; }

        public string C_scopes { get; set; }

        public DateTime expires { get; set; }

    }
}
