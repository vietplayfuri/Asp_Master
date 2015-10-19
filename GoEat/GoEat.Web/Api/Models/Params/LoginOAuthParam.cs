using System;
using System.ComponentModel.DataAnnotations;

namespace GoEat.WebApi
{
    public class LoginOAuthParam
    {
        [Required]
        public string service { get; set; }

        [Required]
        public string token { get; set; }

        [Required]
        public Guid game_id { get; set; }

   }
}