using System.ComponentModel.DataAnnotations;

namespace GoEat.WebApi
{
    public class LoginParam
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string game_id { get; set; }
     
    }
}