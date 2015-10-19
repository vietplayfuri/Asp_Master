using System;
using System.ComponentModel.DataAnnotations;

namespace GoEat.WebApi
{
    public class ProfileEditParam
    {
        [Required]
        public string session { get; set; }

        [Required]
        public Guid game_id { get; set; }


        public string email { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }

    }
}