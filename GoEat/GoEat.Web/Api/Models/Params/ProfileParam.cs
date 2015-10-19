using System;
using System.ComponentModel.DataAnnotations;

namespace GoEat.WebApi
{
    public class ProfileParam
    {
        [Required]
        public string session { get; set; }

        [Required]
        public Guid game_id { get; set; }

    }
}