using System;
using System.ComponentModel;

namespace Platform.Models
{
    public class VerificationToken : ModelBase
    {
        public VerificationToken()
        {
            is_valid = true;
        }
        public string customer_username { get; set; }
        
        public string code { get; set; }
        
        public DateTime validation_time { get; set; }

        public bool is_valid { get; set; }
    }


}
