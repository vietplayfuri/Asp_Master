using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GoPlay.Web.Identity;
using System.Net;
using System.Collections;
using Platform.Models;
using GoPlay.Web.Models;
using Platform.Utility;
using GoPlay.Core;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Helpers.Extensions;
using GoPlay.Models;
using System.Configuration;
using AutoMapper;
using System.Collections.Generic;
using Newtonsoft.Json;
using GoPlay.Web.Helpers;
using Resources;
namespace GoPlay.Web.Controllers
{
    public class AccountController : BaseController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController(IUserStore<ApplicationUser, int> userStore)
        {
            _userManager = new ApplicationUserManager(userStore);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [RequiredNotLogin]
        public ActionResult Login(string returnUrl)
        {

            var model = new LoginViewModel();
            model.returnURL = returnUrl;
            model.loginFB = false;
            if (Session["facebook_token"] != null)
            {
                model.loginFB = true;
                Session["facebook_token"] = null;
            }
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [RequiredNotLogin]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Validate())
            {
                Session["gtoken_section"] = model.session;
                var api = GoPlayApi.Instance;
                var loginOauth = CreateLoginOauth();
                if (!loginOauth)
                {
                    model.loginFB = false;
                    model.errLoginOauth = true;
                    return View(model);
                }
                var user = api.GetUserByUserName(model.username);
                if (user.HasData)
                {
                    return await loginUser(api, user.Data, model.returnURL, model.remember_account == "on" ? true : false);
                }
            }
            ModelState.AddModelError("CustomError", Resources.Resources.Username_and_or_password_are_not_correct);
            return View(model);
        }

        private async Task<ActionResult> loginUser(GoPlayApi api, customer_account user, string returnURL, bool remember_account)
        {
            checkUserLocale(api, user);
            var applicationUser = new ApplicationUser()
            {
                Id = user.id,
                Email = user.email,
                UserName = user.username
            };

            await SignInAsync(applicationUser, remember_account);
            if (string.IsNullOrEmpty(user.country_code) || user.country_code == "ZW")
            {
                IPAddress ip = WebIpHelper.GetClientIp(Request);
                if (ip.Equals(IPAddress.Parse("127.0.0.1")))
                {
                    user.country_code = "SG";
                    user.country_name = "Singapore";
                }
                else
                {
                    ip.GetCountryCode(c => user.country_code = c, n => user.country_name = n);
                }
            }
            api.UpdateCustomerAccount(user.id, user.country_code, user.country_name, DateTime.UtcNow);


            if (!String.IsNullOrEmpty(returnURL))
            {
                return RedirectToLocal(returnURL);
            }
            return Redirect("Profile");
        }

        [HttpPost]
        [RequiredNotLogin]
        [Route("account/reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Message.message.Clear();
                var api = GoPlayApi.Instance;
                var user = api.GetUserByUserName(model.username).Data;
                string rootUrl = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, string.Empty);
                string supportLink = rootUrl + Url.Action("support", "main");

                if (user == null)
                {
                    model.Message.message.Add(new Tuple<string, string>(ConstantValues.S_ALERT,
                       string.Format(ConstantValues.S_ACCOUNT_NOT_FOUNT, supportLink)));
                    return View(model);
                }

                if (string.IsNullOrEmpty(user.email))
                {
                    model.Message.message.Add(new Tuple<string, string>(ConstantValues.S_ALERT,
                        string.Format(ConstantValues.S_EMAIL_NOT_FOUNT, supportLink)));
                    return View(model);
                }

                string code = api.GenPasswordResetCode(user);
                if (string.IsNullOrEmpty(code))
                {
                    model.Message.message.Add(new Tuple<string, string>(ConstantValues.S_ALERT,
                        string.Format(ConstantValues.S_SERVER_ERROR, ErrorCodes.ServerError.ToErrorMessage(), supportLink)));
                    return View(model);
                }

                string resetPwdLink = rootUrl
                    + Url.Action("enter-new-password", "account", new { code });

                var template = new ResetPasswordTemplate
                {
                    main_index = rootUrl + Url.Action("index", "main"),
                    main_support = supportLink,
                    reset_password_link = resetPwdLink,
                    username = user.username,
                    header_logo = rootUrl + ConstantValues.S_IMAGE_IN_EMAIL_HEADER_LOGO,
                    grey_texture = rootUrl + ConstantValues.S_IMAGE_IN_EMAIL_GREY_TEXTURE,
                    diamond_texture = rootUrl + ConstantValues.S_IMAGE_IN_EMAIL_DIAMOND_TEXTURE

                    //demo show image with localhost
                    //header_logo = "http://gtoken.com/static/images/header_logo.png",
                    //grey_texture = "http://gtoken.com/static/images/header_logo.png",
                    //diamond_texture = "http://gtoken.com/static/images/header_logo.png"
                };

                if ((await EmailHelper.SendMailResetPassword(user.email, template)))
                {
                    model.Message.message.Add(new Tuple<string, string>(ConstantValues.S_SUCCESS,
                        string.Format(Platform.Models.ConstantValues.S_RESET_EMAIL_SUCCESS, user.email)));
                }
                else
                {
                    model.Message.message.Add(new Tuple<string, string>(ConstantValues.S_ALERT,
                        string.Format(ConstantValues.S_SERVER_ERROR, ErrorCodes.ServerError.ToErrorMessage(), supportLink)));
                }
            }

            model.username = string.Empty;
            return View(model);
        }

        [HttpGet]
        [RequiredNotLogin]
        [Route("account/reset-password")]
        public ActionResult ResetPassword()
        {
            return View(new ResetPasswordViewModel());
        }

        [HttpGet]
        [RequiredNotLogin]
        [Route("account/enter-new-password")]
        public ActionResult EnterNewPassword(string code)
        {
            var Verifytoken = GoPlayApi.Instance.GetValidVerificationTokenByCode(code);
            if (Verifytoken.HasData)
            {
                EnterNewPasswordForm model = new EnterNewPasswordForm();
                model.code = code;
                return View(model);
            }
            else
            {
                return RedirectToAction("reset-password", "account");
            }
        }
        [HttpPost]
        [RequiredNotLogin]
        [AllowAnonymous]
        [Route("account/enter-new-password")]
        public ActionResult EnterNewPassword(EnterNewPasswordForm model)
        {
            var api = GoPlayApi.Instance;
            var Verifytoken = api.GetValidVerificationTokenByCode(model.code);
            if (Verifytoken.HasData
                && Verifytoken.Data.is_valid
                && Verifytoken.Data.validation_time >= DateTime.UtcNow)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Errors = ModelState.Values.SelectMany(m => m.Errors).First().ErrorMessage;
                    return View(model);
                }

                var user = api.GetUserById(Verifytoken.Data.customer_account_id).Data;
                if (user != null)
                {
                    api.SetPassword(user, model.password);
                    this.Flash(Resources.Resources.Password_changed_successfully, FlashLevel.Success);
                    return RedirectToAction("profile", "account");
                }
            }
            this.Flash(Resources.Resources.Verification_code_is_invalid_or_expired, FlashLevel.Warning);
            return RedirectToAction("reset-password", "account");
        }

        [RequiredLogin]
        [Route("account/profile")]
        public ActionResult profile()
        {
            //It is redundant in python code
            //var games = RandomHelper.Sample(Helpers.GameHelper.GetGamesForCurrentUser(CurrentUser), 3).ToList();
            return View();
        }

        [HttpPost]
        [RequiredLogin]
        [Route("account/edit-profile")]
        public JsonResult EditProfile(EditProfileForm model)
        {
            string inviter_username = string.Empty;
            string session = Session["gtoken_section"] != null ? Session["gtoken_section"].ToString() : string.Empty;
            if (string.IsNullOrEmpty(session))
            {
                ModelState.AddModelError("referralID", "Your session is lost. Please re-login.");
                return Json(new { errors = Errors(ModelState) });
            }

            if (!model.Validate(session))
            {
                ModelState.AddModelError("referralID", ErrorCodes.NON_EXISTING_REFERRAL_CODE.ToErrorMessage());
                return Json(new { errors = Errors(ModelState) });
            }
            else if (model.forValidate)
            {
                return Json(new { correct = true });
            }
            else
            {
                customer_account user = null;
                Mapper.CreateMap<ApplicationUser, customer_account>();
                user = Mapper.Map<ApplicationUser, customer_account>(CurrentUser);
                var api = GoPlayApi.Instance;
                if (!string.IsNullOrEmpty(model.referralID))
                {
                    var inviter = api.GetUserByUserName(model.referralID).Data;
                    if (inviter == null)
                    {
                        ModelState.AddModelError("referralID", ErrorCodes.NON_EXISTING_REFERRAL_CODE.ToErrorMessage());
                        return Json(new { errors = Errors(ModelState) });
                    }
                    inviter_username = inviter.username;
                    if (inviter.username == user.username)
                    {
                        ModelState.AddModelError("referralID", ErrorCodes.TWO_WAY_REFERRING.ToErrorMessage());
                        return Json(new { errors = Errors(ModelState) });
                    }
                    else
                    {
                        api.AddInviter(session, user, inviter.id, inviter.username);
                    }
                }
                if (!string.IsNullOrEmpty(model.bio) && model.bio.Length > ConstantCommon.BIO_MAX_LENGTH)
                {
                    model.bio = model.bio.Substring(0, ConstantCommon.BIO_MAX_LENGTH);
                }
                if (!string.IsNullOrEmpty(model.nickname) && model.nickname.Length > ConstantCommon.NICKNAME_MAX_LENGTH)
                {
                    model.nickname = model.nickname.Substring(0, ConstantCommon.NICKNAME_MAX_LENGTH);
                }

                var result = api.GTokenAPIAccount(new GtokenModelAccountAction()
                {
                    enumAction = EGtokenAction.EditProfile,
                    nickname = model.nickname,
                    gender = user.gender,
                    bio = model.bio,
                    country_code = model.country_code,
                    country_name = model.country_name,
                    referral_code = model.referralID,
                    session = session,
                    partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]
                }).Result;
                if (result.HasData)
                {
                    api.UpdateCustomerAccount(user.id, model.country_code, model.country_name, model.bio, model.nickname, inviter_username);
                    return Json(new { success = true, userJSON = result.Data.profile });
                }
                else
                {
                    ModelState.AddModelError("referralID", result.Error.ToErrorMessage());
                    return Json(new { errors = Errors(ModelState) });
                }
            }
        }

        [RequiredLogin]
        public ActionResult Logout()
        {
            Session.Remove("facebook_token");
            if (CurrentUser != null)
                Session["locale"] = CurrentUser.locale;
            AuthenticationManager.SignOut();
            return Redirect("/");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }

            base.Dispose(disposing);
        }


        [HttpPost]
        [Route("account/update-locale")]
        public ActionResult updateLocale()
        {
            try
            {
                var culture = Request["locale"];
                // Validate input
                culture = CultureHelper.GetImplementedCulture(culture);
                // Save culture in a session
                Session["locale"] = culture;

                var api = GoPlayApi.Instance;
                if (CurrentUser != null)
                {
                    api.SetUserLocale(CurrentUser.Id, culture);
                }
                if (Request.Params["next"] != null)
                {
                    return Redirect(Request.Params["next"]);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);

            }
            return RedirectToAction("Index", "Main");
        }

        #region Helpers

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(_userManager));
        }


        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        [AllowAnonymous]
        [Route("account/register/{referralID?}")]
        [RequiredNotLogin]
        public ActionResult Register(string referralID = null)
        {
            var api = GoPlayApi.Instance;
            if (!api.IsUserExist(referralID))
            {
                referralID = string.Empty;
            }
            var countryCode = string.Empty;
            var countryName = string.Empty;

            IPAddress ip = WebIpHelper.GetClientIp(Request);
            if (ip.Equals(IPAddress.Parse("127.0.0.1")))
            {
                countryCode = "SG";
                countryName = "Singapore";
            }
            else
            {
                ip.GetCountryCode(c => countryCode = c, n => countryName = n);
            }

            string facebook_email = string.Empty;
            if (HttpContext.Session["facebook_email"] != null)
            {
                facebook_email = HttpContext.Session["facebook_email"].ToString();
            }

            var model = new RegisterViewModel
            {
                referralID = referralID,
                countryCode = countryCode,
                countryName = countryName,
                facebook_email = facebook_email
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        [Route("account/register")]
        [RequiredNotLogin]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid
                    || !String.IsNullOrEmpty(model.submit) && model.submit != "submit")
                {
                    return Json(new { errors = Errors(ModelState) });
                }

                var api = GoPlayApi.Instance;
                string partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"];

                //Calling to Gtoken api to create user and adding friend in Gtoken
                var gtokenResultRegister = await api.GTokenAPIAccount(new GtokenModelAccountAction
                {
                    enumAction = EGtokenAction.Register,
                    username = model.username,
                    pwd = model.password,
                    partnerId = partnerId,
                    email = model.email,
                    referral_code = model.referralID,
                    ip_address = WebIpHelper.GetClientIp(Request).ToString(),
                    country_code = model.countryCode,
                    country_name = model.countryName
                });


                if (!gtokenResultRegister.HasData)
                {
                    ModelState.AddModelError("CustomError", gtokenResultRegister.Error.ToErrorMessage());
                    return Json(new { errors = Errors(ModelState) });
                }

                //store Gtoken session for futher using ( used in edit profile)
                HttpContext.Session["gtoken_section"] = gtokenResultRegister.Data.session;

                //Create user and adding friend
                var user = api.UpdateProfile(gtokenResultRegister.Data).Data;

                CreateLoginOauth();
                var applicationUser = new ApplicationUser()
                {
                    Id = user.id,
                    Email = user.email,
                    UserName = user.username
                };

                await SignInAsync(applicationUser, false);

                //sending mail
                string rootUrl = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, String.Empty);
                var template = new WelcomeMessageTemplate
                {
                    main_index = rootUrl + Url.Action("index", "main"),
                    logoImg_src = rootUrl + ConstantValues.S_IMAGE_IN_EMAIL_LOGO_GOPLAY,
                    username = user.username,
                    login_link = rootUrl + Url.Action("login", "account"),
                    lastestGame_link = rootUrl + Url.Action("index", "game"),
                    facebook_link = ConfigurationManager.AppSettings["LINK_TO_FACEBOOK_GO_PLAY"],
                    twitter_link = ConfigurationManager.AppSettings["LINK_TO_TWITTER_GO_PLAY"],
                    KennethImg_src = rootUrl + ConstantValues.S_IMAGE_IN_EMAIL_KENNETH,
                    home_link = rootUrl
                };
                await EmailHelper.SendMailWelcome(user.email, template);
                //end sending mail
                return Json(new { success = true, redirect = Url.Action("profile", "account") });
            }
            catch
            {
                ModelState.AddModelError("CustomError", Messages.MISSING_FIELDS);
                return Json(new { errors = Errors(ModelState) });
            }
        }

        [HttpPost]
        [Route("account/change-password")]
        [RequiredLogin]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!model.IsValidOldPassWord(CurrentUser.UserName))
            {
                ModelState.AddModelError("oldPassword", "Old password is incorrect");
            }
            else if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                var result = GoPlayApi.Instance.GTokenAPIAccount(new GtokenModelAccountAction()
                {

                    enumAction = EGtokenAction.ChangePassword,
                    partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"],
                    pwd = model.newPassword,
                    old_pwd = model.oldPassword,
                    confirm_pwd = model.confirmPassword,
                    session = model.session
                }).Result;
                if (result.HasData)
                {
                    if (result.Data.success == true)
                    {
                        return Redirect("profile");
                    }
                    else
                    {
                        ModelState.AddModelError("confirmPassword", result.Data.message);
                    }
                }
                else
                {
                    ModelState.AddModelError("confirmPassword", ErrorCodes.ServerError.ToErrorMessage());
                }
            }
            return View(model);
        }

        [HttpGet]
        [Route("account/change-password")]
        [RequiredLogin]
        public ActionResult ChangePassword()
        {
            return View();
        }


        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            string url = returnUrl;
            if (string.IsNullOrEmpty(returnUrl))
            {
                url = "/account/login";
            }
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                this.Flash(Resources.Resources.You_dont_want_to_sign_in_with_Facebook, FlashLevel.Warning);
                return Redirect(url);
            }
            var externalIdentity = HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            var result = externalIdentity.Result.Claims;
            string email = string.Empty;
            if (loginInfo.Email != null)
            {
                email = loginInfo.Email;
            }
            //result.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var FacebookAccessToken = result.FirstOrDefault(c => c.Type == "FacebookAccessToken").Value;
            var fb_id = result.FirstOrDefault(c => c.Type == "urn:facebook:id").Value;
            Session["facebook_token"] = FacebookAccessToken;
            Session["access_token"] = FacebookAccessToken;
            var api = GoPlayApi.Instance;
            var loginOauth = api.GTokenAPIAccount(new GtokenModelAccountAction()
            {
                enumAction = EGtokenAction.LoginOauth,
                service = "Facebook",
                token = FacebookAccessToken,
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]
            }).Result;

            //var loginOauth = api.GetCustomerLoginOauth("Facebook", fb_id).Data;
            if (loginOauth.Data == null || !loginOauth.Data.success)
            {
                Session["facebook_email"] = email;
                Session["facebook_identity"] = fb_id;
                return Redirect(url);
            }
            var user = api.UpdateProfile(loginOauth.Data).Data;
            Session["gtoken_section"] = loginOauth.Data.session;
            Session.Remove("facebook_token");
            Session.Remove("access_token");
            return await loginUser(api, user, returnUrl, false);
        }

        [HttpPost]
        [Route("account/change-email")]
        [RequiredLogin]
        public JsonResult ChangeEmail(ChangeEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.IsValidPassWord(CurrentUser.UserName))
                {
                    ModelState.AddModelError("password", Resources.Resources.Incorrect_password_Please_try_again);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.submit))
                    {
                        return Json(new { correct = true });
                    }
                    var api = GoPlayApi.Instance;
                    var result = api.GTokenAPIAccount(new GtokenModelAccountAction()
                        {
                            enumAction = EGtokenAction.EditProfile,
                            nickname = CurrentUser.nickname,
                            gender = CurrentUser.gender,
                            email = model.email,
                            bio = CurrentUser.bio,
                            country_code = CurrentUser.country_code,
                            country_name = CurrentUser.country_name,
                            referral_code = CurrentUser.referralID,
                            session = model.session,
                            partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]
                        }).Result;

                    if (result.HasData)
                    {

                        api.UpdateCustomerAccount(CurrentUser.Id, model.email, CurrentUser.nickname, CurrentUser.gender, CurrentUser.vip);
                        return Json(new { success = true, redirect = "account/profile" });
                    }
                    else
                    {
                        ModelState.AddModelError("password", result.Error.ToErrorMessage());
                    }
                }
            }
            return Json(new { errors = Errors(ModelState) });
        }

        [HttpGet]
        [Route("account/change-email")]
        [RequiredLogin]
        public ActionResult ChangeEmail()
        {
            return View();
        }
        private void checkUserLocale(GoPlayApi api, customer_account user)
        {
            if (Session["locale"] != null)
            {
                if (user.locale != Session["locale"].ToString())
                {
                    api.SetUserLocale(user.id, Session["locale"].ToString());
                }
            }
        }

        private bool CreateLoginOauth()
        {
            var api = GoPlayApi.Instance;
            string msg = string.Empty;
            if (Session["facebook_identity"] != null)
            {
                var modelAccount = new GtokenModelAccountAction()
                {
                    enumAction = EGtokenAction.ConnectOauth,
                    service = "Facebook",
                    session = Session["gtoken_section"] == null ? string.Empty : Session["gtoken_section"].ToString(),
                    token = Session["access_token"] == null ? string.Empty : Session["access_token"].ToString(),
                    partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]
                };
                var result = api.GTokenAPIAccount(modelAccount).Result;
                Session.Remove("facebook_identity");
                Session.Remove("facebook_email");
                Session.Remove("facebook_token");
                Session.Remove("access_token");
                return result.Data != null;
            }
            return true;
        }

        [Route("account/find-customer-account")]
        [RequiredLogin]
        public JsonResult FindCustomerAccount(string term)
        {
            term = !string.IsNullOrEmpty(term) ? term.Trim().ToLower().Trim('"') : string.Empty;
            var customers = GoPlayApi.Instance.GetUserByTerm(CurrentUser.Id, term);

            List<object> results = new List<object>();
            if (customers.Data != null && customers.Data.Any())
            {
                customers.Data.ForEach(user => results.Add(new
                    {
                        id = user.id,
                        avatar_filename = user.GetValidAvatarUrl(),
                        display_name = user.GetDisplayName(),
                        username = user.username
                    }));
            }

            return Json(new { results });
        }

        [HttpPost]
        [Route("account/get-session")]
        public JsonResult checkShowPopup(ShowPopupViewModel model)
        {
            bool show = false;
            string popupNumber = model.popupNumber;
            if (Session[popupNumber] == null)
            {
                show = true;
                Session["popupNumber"] = true;
            }
            return Json(new { success = show });
        }
        [HttpGet]
        [Route("account/get-session")]
        public JsonResult checkShowPopup()
        {
            return Json(new { success = false });
        }

    }
}