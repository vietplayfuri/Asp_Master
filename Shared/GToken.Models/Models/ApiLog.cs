using System;
namespace Platform.Models
{
    public class ApiLog : ModelBase
    {
        public int id { get; set; }
        public string action { get; set; }
        public string version { get; set; }
        public string ip_address { get; set; }
        public string partner_identifier { get; set; }
        public string customer_username { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public string data { get; set; }
        public DateTime created_at { get; set; }
        public string user_agent { get; set; }
        public string country_code { get; set; }        
    }  
}
