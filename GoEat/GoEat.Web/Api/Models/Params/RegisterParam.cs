using System;
using System.ComponentModel.DataAnnotations;

namespace GoEat.WebApi
{
    public class RegisterParam
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public Guid game_id { get; set; }

        public string  gender { get; set; }
        public string email  { get; set; }
        public string  referral_code { get; set; }
        public string ip_address { get; set; }
     
    }
}