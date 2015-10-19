using GToken.Web.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Platform.Utility;
using System.Net;
using GToken.Web.ActionFilter;
using GToken.Web.Identity;
using Platform.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using AutoMapper;
using System.Globalization;
using System.IO;
using GToken.Models;


namespace GToken.Web.Controllers
{
    public class AccountController : BaseController
    {
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

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("account/update-locale")]
        [ValidateAntiForgeryToken]
        public ActionResult updateLocale()
        {
            var culture = Request["locale"];
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a session
            Session["locale"] = culture;

            var api = Platform.Core.Api.Instance;
            if (CurrentUser != null)
            {
                api.SetUserLocale(CurrentUser.Id, culture);
            }
            if (Request.Params["next"] != null)
            {
                return Redirect(Request.Params["next"]);
            }
            return RedirectToAction("Index", "Main");
        }

        [AllowAnonymous]
        [RequiredNotLogin]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //  [ValidateAntiForgeryToken]
        [RequiredNotLogin]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginApi = Platform.Core.Api.Instance;
                IPAddress ip = GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request);
                var user = loginApi.Login(model.username.ToLower().Trim(), model.password, ip);
                if (user.Succeeded && user.HasData)
                {
                    checkUserLocale(loginApi, user.Data);
                    var applicationUser = new ApplicationUser
                    {
                        Id = user.Data.id,
                        Email = user.Data.email,
                        UserName = user.Data.username
                    };

                    await SignInAsync(applicationUser, model.rememberMe == "on" ? true : false);

                    if (Request.Params["next"] != null)
                    {
                        return Redirect(Request.Params["next"]);
                    }
                    return RedirectToAction("index", "transaction");
                }

                ModelState.AddModelError("CustomError", Resources.Resources.Username_and_or_password_are_not_correct);
            }
            ViewBag.Errors = ModelState.Values.SelectMany(m => m.Errors).First().ErrorMessage;
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            AuthenticationManager.SignOut();
            return RedirectToAction("index", "main");
        }

        [AllowAnonymous]
        [Route("account/register/{referralID?}")]
        [RequiredNotLogin]
        public ActionResult Register(string referralID = null)
        {
            var api = Platform.Core.Api.Instance;
            if (!api.IsUserExist(referralID))
            {
                referralID = string.Empty;
            }
            var countryCode = string.Empty;
            var countryName = string.Empty;

            IPAddress ip = GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request);
            if (ip.Equals(IPAddress.Parse("127.0.0.1")))
            {
                countryCode = "SG";
                countryName = "Singapore";
            }
            else
            {
                ip.GetCountryCode(c => countryCode = c, n => countryName = n);
            }
            var model = new RegisterViewModel
            {
                referralID = referralID,
                countryCode = countryCode,
                countryName = countryName
            };
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        [Route("account/register")]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid
                    || String.IsNullOrEmpty(model.submit) || model.submit != "submit")
                {
                    //Something failed, display errors
                    return Json(new { errors = Errors(ModelState) });
                }

                var api = Platform.Core.Api.Instance;
                string ip = Request.UserHostAddress == null || Request.UserHostAddress == "::1"
                    ? IPAdressExtension.GetDefaultIp()
                    : Request.UserHostAddress;

                var result = api.Register(model.username.ToLower(),
                    model.password,
                    null,
                    model.username.ToLower(),
                    model.email.ToLower(),
                    Genders.Male,
                    model.referralID,
                    ip,
                    model.countryCode,
                    model.countryName);
                api.UpdateCustomerAccount(result.Data.id, result.Data.country_code, result.Data.country_name, DateTime.UtcNow);

                //Login User
                var applicationUser = new ApplicationUser()
                {
                    Id = result.Data.id,
                    Email = result.Data.email,
                    UserName = result.Data.username
                };

                await SignInAsync(applicationUser, false);

                return Json(new { success = true, redirect = Url.Action("index", "transaction") });
            }
            catch
            {
                ModelState.AddModelError("CustomError", Messages.MISSING_FIELDS);
                return Json(new { errors = Errors(ModelState) });
            }
        }

        [Authorize]
        [Route("account/edit-profile")]
        public ActionResult editProfile()
        {
            var api = Platform.Core.Api.Instance;
            var result = api.GetUserById(CurrentUser.Id);
            var editProfileForm = new EditProfileForm();
            if (result.HasData)
            {
                Mapper.CreateMap<CustomerAccount, EditProfileForm>();
                editProfileForm = Mapper.Map<CustomerAccount, EditProfileForm>(result.Data);
            }
            return View(editProfileForm);
        }

        [HttpGet]
        [Route("account/enter-new-password")]
        public ActionResult EnterNewPassword(string code, string next)
        {

            var Verifytoken = Platform.Core.Api.Instance.GetValidVerificationTokenByCode(code);
            if (Verifytoken.HasData)
            {
                EnterNewPasswordForm model = new EnterNewPasswordForm();
                model.next = next;
                model.code = code;
                //model.is_valid = Verifytoken.Data.is_valid;
                //model.validation_time = Verifytoken.Data.validation_time;
                //model.username = Verifytoken.Data.customer_username;
                return View(model);
            }
            else
            {
                return RedirectToAction("reset-password", "account");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("account/enter-new-password")]
        public async Task<ActionResult> EnterNewPassword(EnterNewPasswordForm model)
        {
            var api = Platform.Core.Api.Instance;
            var Verifytoken = Platform.Core.Api.Instance.GetValidVerificationTokenByCode(model.code);
            if (Verifytoken.HasData
                && Verifytoken.Data.is_valid
                && Verifytoken.Data.validation_time >= DateTime.UtcNow)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Errors = ModelState.Values.SelectMany(m => m.Errors).First().ErrorMessage;
                    return View(model);
                }

                var user = api.GetUserByUserName(Verifytoken.Data.customer_username).Data;
                if (user != null)
                {
                    api.SetPassword(user, model.password);
                    //return RedirectToAction("", model.next);

                    //Login User
                    var applicationUser = new ApplicationUser()
                    {
                        Id = user.id,
                        Email = user.email,
                        UserName = user.username
                    };

                    await SignInAsync(applicationUser, false);

                    if (!string.IsNullOrEmpty(model.next))
                        return Redirect(model.next);

                    return Json(new { success = true, redirect = Url.Action("index", "transaction") });
                }
            }

            return RedirectToAction("reset-password", "account");
        }

        [HttpPost]
        [Route("account/edit-profile")]
        [Authorize]
        public JsonResult UpdateProfile(EditProfileForm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { errors = Errors(ModelState) });
            }
            var api = Platform.Core.Api.Instance;
            var user = api.GetUserById(CurrentUser.Id);
            user.Data.nickname = model.nickname;
            user.Data.country_code = model.country_code;
            user.Data.country_name = GetCountryNameByCode(model.country_code);
            user.Data.email = string.IsNullOrEmpty(model.email)?string.Empty:model.email.ToLower();
            user.Data.dob = DateTime.Parse(model.dob);
            if (!string.IsNullOrEmpty(model.inviter_username))
            {
                user.Data.inviter_username = model.inviter_username;
            }
            var result = api.UpdateProfile(user.Data.inviter_username != null ? model.inviter_username : String.Empty, user.Data);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("inviter_username", ErrorCodes.TWO_WAY_REFERRING.ToString());
                return Json(new { errors = Errors(ModelState) });
            }
            if (!string.IsNullOrEmpty(model.new_password))
            {
                api.SetPassword(user.Data, model.new_password);
            }
            return Json(new
            {
                success = true,
                userJSON = Platform.Models.Profile.GetFrom(user.Data),
                redirect = @Url.Action("index", "transaction")
            });
        }



        #region Helpers
        public string GetCountryNameByCode(string code)
        {
            RegionInfo myRI1 = new RegionInfo(code);
            return myRI1.EnglishName;
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(_userManager));
        }

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

        private void checkUserLocale(Platform.Core.Api api, CustomerAccount user)
        {
            if (Session["locale"] != null)
            {
                if (user.locale != Session["locale"].ToString())
                {
                    api.SetUserLocale(user.id, Session["locale"].ToString());
                }
            }
        }

        #endregion

        [HttpPost]
        [Route("account/reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Message.message.Clear();
                var api = Platform.Core.Api.Instance;
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

                string next = string.IsNullOrEmpty(Request.Params["next"])
                    ? rootUrl + "/transaction"
                    : Request.Params["next"];
                string resetPwdLink = rootUrl
                    + Url.Action("enter-new-password", "account", new { code, next });

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

                if ((await EmailHelper.SendMailResetPasword(user.email, template)))
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
        [Route("account/reset-password")]
        public ActionResult ResetPassword()
        {
            return View(new ResetPasswordViewModel());
        }

        [HttpPost]
        [Route("account/upload-avatar")]
        [Authorize]
        public JsonResult UploadAvatar()
        {
            var file = Request.Files["file"];
            if (file == null)
            {
                return Json(new
                {
                    success = false
                });
            }
            string path = HttpContext.Server.MapPath(Platform.Models.ConstantValues.S_AVATAR_UPLOAD_DIR);
            string filename = Platform.Core.Api.Instance.HandleFile(HttpContext.Server.MapPath("~"), file.InputStream, path, CurrentUser.UserName);
            if (!string.IsNullOrEmpty(filename))
            {
                Platform.Core.Api.Instance.UpdateCustomerAccount(CurrentUser.Id, filename);
            }
            return Json(new
            {
                success = true,
                filename = filename
            });
        }
    }
}