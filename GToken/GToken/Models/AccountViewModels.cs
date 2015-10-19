using System;
using System.Net;
using System.Web.Http;
using Platform.Core;
using Platform.Utility;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GToken.Models;
using Platform.Models;

namespace GToken.Web.Models
{
    #region WebViewModal
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "* This field is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username is between 3-20 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+[\w\-\.]*$", ErrorMessage = "Username can only contain letters, numbers, \"_\", \"-\" and \".\"")]
        [UniqueUserName(ErrorMessage = "This username is already taken")]
        public string username { get; set; }

        [Required(ErrorMessage = "* This field is required")]
        [Email(ErrorMessage = "Invalid email address")]
        [Display(Name = "email")]
        public string email { get; set; }

        [Required(ErrorMessage = "* This field is required")]
        [StringLength(100, ErrorMessage = "* Password must be more than 2 characters", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string password { get; set; }


        [Required(ErrorMessage = "* This field is required")]
        [Compare("password", ErrorMessage = "* Confirm password does not match")]
        public string confirmPassword { get; set; }

        public string countryCode { get; set; }

        public string countryName { get; set; }

        [ReferralID(ErrorMessage = "Referral Code does not exist")]
        public string referralID { get; set; }

        [Required(ErrorMessage = "* This field is required")]
        public bool acceptTOS { get; set; }

        public string submit { get; set; }
    }

    public class EditProfileForm : IValidatableObject
    {
        public string username { get; set; }

        [Required(ErrorMessage = "* This field is required")]
        public string nickname { get; set; }

        [Required(ErrorMessage = "* This field is required")]
        [Display(Name = "email")]
        [Email(ErrorMessage = "* Invalid email address")]
        public string email { get; set; }

        public string country_code { get; set; }

        public string country_name { get; set; }
        public string inviter_username { get; set; }

        [Required(ErrorMessage = "* This field is required")]
        public string dob { get; set; }
        public string avatar_filename { get; set; }

        public string password { get; set; }

        [StringLength(100, ErrorMessage = "* Password must be more than 2 characters", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string new_password { get; set; }

        [Compare("new_password", ErrorMessage = "* Confirm password does not match")]
        public string confirm_new_password { get; set; }
        public string submit { get; set; }
        public string dob_epoch { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var api = Platform.Core.Api.Instance;

            if (!string.IsNullOrEmpty(inviter_username))
            {
                var inviter = api.GetUserByUserName(inviter_username.Trim()).Data;
                if (inviter == null)
                {
                    yield return new ValidationResult(ErrorCodes.NON_EXISTING_REFERRAL_CODE.ToErrorMessage(), new[] { "inviter_username" });
                }
            }

            if (!String.IsNullOrEmpty(password))
            {
                var result = api.GetUserByUserName(username);
                if (!(result.HasData && result.Data.CheckPassword(password)))
                {
                    yield return new ValidationResult("* Password is not correct", new[] { "password" });
                }
                else
                {
                    if (string.IsNullOrEmpty(new_password))
                    {
                        yield return new ValidationResult("* This field is required", new[] { "new_password" });
                    }
                    if (string.IsNullOrEmpty(confirm_new_password))
                    {
                        yield return new ValidationResult("* This field is required", new[] { "confirm_new_password" });
                    }
                }
            }
        }
    }

    public class EnterNewPasswordForm
    {
        public string code { get; set; }
        public string next { get; set; }
        [Required(ErrorMessage = "* This field is required")]
        [StringLength(100, ErrorMessage = "* Password must be more than 2 characters", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "* This field is required")]
        [Compare("password", ErrorMessage = "* Confirm password does not match")]
        public string confirmPassword { get; set; }

    }
    #endregion
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
        [Required(ErrorMessage = "* Username is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "* Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string password { get; set; }

        public string partner_id { get; set; }

        public string rememberMe { get; set; }

        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public string device_id { get; set; }
        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public int? game_id { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel()
        {
            Message = new MessageViewModel();
        }
        public string username { get; set; }
        public MessageViewModel Message { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RegisterModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string partner_id { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string referral_code { get; set; }
        public string ip_address { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }

        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public string device_id { get; set; }
        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public int? game_id { get; set; }
    }

    public class OauthModel
    {
        public string partner_id { get; set; }
        public string partner { get; set; }
        public string service { get; set; }
        public string token { get; set; }
        public string ip_address { get; set; }
        public string session { get; set; }

        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public string device_id { get; set; }
        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public int? game_id { get; set; }
    }

    public class RetrieveProfileModel
    {
        public string partner_id { get; set; }
        public string session { get; set; }
        public string username { get; set; }
    }
    public class ProfileModel
    {
        public string partner_id { get; set; }
        public string session { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string bio { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string referral_code { get; set; }
    }

    public class UniqueUserNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            var api = Platform.Core.Api.Instance;
            var result = api.GetUserByUserName(value.ToString().ToLower().Trim());
            return result.HasData == false;
        }
    }

    /// <summary>
    /// Used for /api/1/account/check-oauth-connection
    /// </summary>
    public class APIcheckOauthConnectionModel
    {
        public string session { get; set; }
        public string partner_id { get; set; }
        public string service { get; set; }
        public string token { get; set; }
    }

    /// <summary>
    /// Used for /api/1/account/connect-oauth
    /// </summary>
    public class APIConnectOauthModel
    {
        public string session { get; set; }
        public string partner_id { get; set; }
        public string service { get; set; }
        public string token { get; set; }
    }

    public class ResetPasswordTemplate
    {
        public string username { get; set; }
        public string reset_password_link { get; set; }
        public string main_support { get; set; }
        public string main_index { get; set; }
        public string header_logo { get; set; }
        public string grey_texture { get; set; }
        public string diamond_texture { get; set; }
    }

    public class EmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            return Helper.IsEmailValid(value.ToString());
        }
    }

    public class ReferralIDAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                var api = Platform.Core.Api.Instance;
                if (api.GetUserByUserName(value.ToString().ToLower()).Data == null)
                    return false;
            }
            return true;
        }
    }
}
