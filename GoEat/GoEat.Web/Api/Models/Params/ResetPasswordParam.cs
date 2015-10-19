using System.ComponentModel.DataAnnotations;

namespace GoEat.WebApi
{
    public class ResetPasswordParam
    {
        [Required]
        public string username { get; set; }

    }
}