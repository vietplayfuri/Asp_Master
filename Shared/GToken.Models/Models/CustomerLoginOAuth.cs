using System;
namespace Platform.Models
{


    public class CustomerLoginOAuth : ModelBase
    {
        public int id { get; set; }
        public string customer_username { get; set; }
        public string service { get; set; }
        public string identity { get; set; }
     
    }

    public class AccessToken 
    {
        public string customer_username { get; set; }
        public string partner_identifier { get; set; }
        public string token { get; set; }
        public DateTime saved_at { get; set; }
    }
}
