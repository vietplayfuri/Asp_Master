using GoPlay.Core;
using GoPlay.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Platform.Models;
using System.Configuration;
using GoPlay.Web.Identity;
using Platform.Utility;

namespace GoPlay.Web.Models
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
        public LoginViewModel()
        {
            loginFB = false;
        }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Username_is_required")]
        public string username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Password_is_required")]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string password { get; set; }

        public string remember_account { get; set; }

        public bool loginFB { get; set; }
        public string email { get; set; }
        public string session { get; set; }
        public bool Validate()
        {
            var api = GoPlayApi.Instance;
            var partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"];
            var user = api.GTokenAPIAccount(new GtokenModelAccountAction
                        {
                            enumAction = EGtokenAction.Login,
                            username = username,
                            pwd = password,
                            partnerId = partnerId
                        }).Result;

            if (user.HasData && user.Data.success)
            {
                session = user.Data.session;
                var data = user.Data;
                var profile = api.GTokenAPIAccount(new GtokenModelAccountAction
                        {
                            enumAction = EGtokenAction.Profile,
                            username = username,
                            session = data.session,
                            partnerId = partnerId
                        });
                if (profile.Result.HasData)
                {
                    api.UpdateProfile(profile.Result.Data);
                }
                return true;
            }
            return false;
        }

        public string returnURL { get; set; }
        public bool errLoginOauth { get; set; }

       
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Username_is_between_characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+[\w\-\.]*$", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Username_can_only_contain_letters_numbers")]
        [UniqueUserName(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_username_is_already_taken")]
        public string username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Email(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Invalid_email_address")]
        [Display(Name = "email")]
        public string email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Compare("email", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Confirm_email_does_not_match")]
        public string confirmEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Password_must_be_more_than_characters", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public string countryCode { get; set; }
        public string countryName { get; set; }

        [IsValidReferral(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Referral_Code_does_not_exist")]
        public string referralID { get; set; }

        public string facebook_email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public bool acceptTOS { get; set; }

        public string submit { get; set; }
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
        public string profile { get; set; }
        public string session { get; set; }
    }
    public class WelcomeMessageTemplate
    {
        public string main_index { get; set; }
        public string logoImg_src { get; set; }
        public string username { get; set; }
        public string login_link { get; set; }
        public string lastestGame_link { get; set; }
        public string facebook_link { get; set; }
        public string twitter_link { get; set; }
        public string KennethImg_src { get; set; }
        public string home_link { get; set; }

    }

    public class APILoginModel
    {
        public GtokenAPILoginModel profile { get; set; }
        public customer_account user { get; set; }
        public Game game { get; set; }
    }

    public class APIFriendModel
    {
        public GtokenAPIFriend result { get; set; }
        public customer_account user { get; set; }
        public Game game { get; set; }
    }

    public class APIGrantChatTokenModel
    {
        public string game_id { get; set; }
        public string session { get; set; }
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
    public class EnterNewPasswordForm
    {
        public string code { get; set; }
        public string next { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Password_must_be_more_than_characters", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Compare("password", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Confirm_password_does_not_match")]
        public string confirmPassword { get; set; }

    }
    public class UniqueUserNameAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            return !Platform.Core.Api.Instance.GetUserByUserName(value.ToString().ToLower().Trim()).HasData;
        }
    }
    public class EmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            return Helper.IsEmailValid(value.ToString());
        }
    }

    public class IsValidReferralAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return GoPlayApi.Instance.GetUserByUserName(value.ToString().ToLower().Trim()).HasData;
            }
            return true;
        }
    }

    public class EditProfileForm
    {
        public string nickname
        {
            get;
            set;

        }
        public string country_code { get; set; }
        public string country_name { get; set; }
        private string _bio;
        public string bio
        {
            get;
            set;
        }
        public string referralID { get; set; }

        public bool forValidate { get; set; }

        public bool Validate(string session)
        {
            if (!string.IsNullOrEmpty(referralID))
            {
                var api = GoPlayApi.Instance;
                var partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"];
                var profile = api.GTokenAPIAccount(new GtokenModelAccountAction
                        {
                            enumAction = EGtokenAction.Profile,
                            username = referralID,
                            partnerId = partnerId,
                            session = session
                        }).Result;
                return profile.HasData && profile.Data.success;
            }
            return true;
        }



    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string oldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Password_must_be_more_than_characters", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }

        [Compare("newPassword", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Confirm_password_does_not_match")]
        public string confirmPassword { get; set; }
        public string session { get; set; }

        public bool IsValidOldPassWord(string _username)
        {
            var result = GoPlayApi.Instance.GTokenAPIAccount(new GtokenModelAccountAction()
            {
                enumAction = EGtokenAction.Login,
                username = _username,
                pwd = oldPassword,
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]

            }).Result;

            if (!result.HasData || !result.Data.success)
            {
                return false;
            }
            else
            {
                session = result.Data.session;
            }
            return true;
        }
    }

    public class ChangeEmailViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Email(ErrorMessageResourceType = typeof(Resources.Resources),ErrorMessageResourceName="Invalid_email_address")]
        [Display(Name = "email")]
        public string email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Compare("email", ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Confirm_email_does_not_match")]
        public string confirmEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public string submit { get; set; }

        public string session { get; set; }
        public bool IsValidPassWord(string _username)
        {
            var result = GoPlayApi.Instance.GTokenAPIAccount(new GtokenModelAccountAction()
            {
                enumAction = EGtokenAction.Login,
                username = _username,
                pwd = password,
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]

            }).Result;

            if (!result.HasData || !result.Data.success)
            {
                return false;
            }
            else
            {
                session = result.Data.session;
            }
            return true;
        }
    }

    public class ShowPopupViewModel
    {
        public string popupNumber { get; set; }
    }
}
