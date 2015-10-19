namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class verification_token
    {
        public int customer_account_id { get; set; }
        public string code { get; set; }
        public DateTime validation_time { get; set; }
        public bool is_valid { get; set; }
    }
}
