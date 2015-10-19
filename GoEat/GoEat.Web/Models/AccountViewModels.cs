using GoEat.Web.Helpers.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoEat.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "*Username is required")]
        public string username { get; set; }

        [MostlyRequired(ErrorMessage = "*Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string password { get; set; }

        public string remember_account { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username is between 3-20 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+[\w\-\.]*$", ErrorMessage = "Username can only contain letters, numbers, \"_\", \"-\" and \".\"")]
        public string username { get; set; }

        [MostlyRequired(ErrorMessage = "This field is required")]
        [Display(Name = "email")]
        //[RegularExpression(@"^[_a-zA-Z0-9-]+((\.[_a-zA-Z0-9-]+)|(\+[_a-zA-Z0-9-]+))*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$", ErrorMessage = "Invalid email address")]
        [IsEmail(ErrorMessage = "Invalid email address")]
        public string email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Compare("email", ErrorMessage = "Confirm email does not match")]
        public string confirmEmail { get; set; }

        [MostlyRequired(ErrorMessage = "This field is required")]
        [StringLength(20, ErrorMessage = "Password is between from 3 to 20 characters", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string password { get; set; }

        //public string countryCode { get; set; }

        public string referralID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public bool acceptTOS { get; set; }

        public string submit { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "*The New Password field is required.")]
        [StringLength(20, ErrorMessage = "*The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "*The Confirm New Password field is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "*The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "*The Old Password field is required.")]
        [StringLength(20, ErrorMessage = "*The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "*Please input your username or email")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
    }

    public class DiscountViewModel
    {
        public int RestaurentId { get; set; }
        public int UserId { get; set; }
        public int DiscountId { get; set; }
        public string qrUrl { get; set; }
    }

    public class FeedbackModel
    {
        [Required(ErrorMessage = "*Please input your name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "*Please input your Email")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        //[RegularExpression(@"^[_a-zA-Z0-9-]+((\.[_a-zA-Z0-9-]+)|(\+[_a-zA-Z0-9-]+))*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$", ErrorMessage = "Invalid email address")]
        [IsEmail(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "*Please input your message")]
        [StringLength(250, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Message { get; set; }
    }

    public class ProfileViewModel
    {
        public decimal TotalToken { get; set; }
        public string country_code { get; set; }
        public string qrUrl { get; set; }

        public EditProfileModel Profile { get; set; }
    }

    public class EditProfileModel
    {
        [Required(ErrorMessage = "*Please input your Nickname")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Nickname { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string Country { get; set; }

        [Required(ErrorMessage = "*Please input your Email")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        //[RegularExpression(@"^[_a-zA-Z0-9-]+((\.[_a-zA-Z0-9-]+)|(\+[_a-zA-Z0-9-]+))*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$", ErrorMessage = "Invalid email address")]
        [IsEmail(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [StringLength(225, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string ShortBio { get; set; }
        public string CountryCode { get; set; }
    }

}
