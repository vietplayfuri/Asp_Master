using System;
using System.ComponentModel.DataAnnotations;

namespace GoEat.WebApi
{
    public class ConnectionOAuthParam
    {
        [Required]
        public string session { get; set; }

        [Required]
        public Guid game_id { get; set; }

        [Required]
        public string service { get; set; }

        [Required]
        public string token { get; set; }

     

   }
}