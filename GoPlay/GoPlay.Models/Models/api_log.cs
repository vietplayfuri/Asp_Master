namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class api_log
    {
        public api_log() { created_at = DateTime.UtcNow; }
        public int id { get; set; }

        public string action { get; set; }

        public string version { get; set; }

        public string ip_address { get; set; }

        public int? game_id { get; set; }

        public string data { get; set; }

        public string message { get; set; }

        public DateTime created_at { get; set; }

        public bool status { get; set; }

        public string user_agent { get; set; }

        public string country_code { get; set; }

        public int? customer_account_id { get; set; }
    }


    public class SimpleApiLog
    {
        public string action { get; set; }

        public string ip_address { get; set; }

        public DateTime created_at { get; set; }

        public bool status { get; set; }

        public string country_code { get; set; }

        public string game_name { get; set; }
    }

    public class CustomApiLog
    {
        public int game_id { get; set; }
        public DateTime created_at { get; set; }
    }
}
