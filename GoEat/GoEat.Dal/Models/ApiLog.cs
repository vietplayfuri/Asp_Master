using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Dal.Models
{
    public class ApiLog : ModelBase
    {
        public int id { get; set; }
        public string action { get; set; }
        public string version { get; set; }
        public string ip_address { get; set; }
        public Nullable<int> game_id { get; set; }
        public string data { get; set; }
        public string message { get; set; }
        public DateTime created_at { get; set; }
        public bool status { get; set; }
        public string user_agent { get; set; }
        public string country_code { get; set; }
        public Nullable<int> customer_account_id { get; set; }

    }

  
}
