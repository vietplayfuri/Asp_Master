using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using GoEat.Core;
using GoEat.Dal.Common;
using GoEat.Web.ActionFilter;
using GoEat.Web.Identity;
using GoEat.Web.Models;
using GoEat.Models;
using GoEat.Utility;
using GoEat.Utility.Crytography;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace GoEat.Web.Controllers
{
    [RoutePrefix("account")]
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated && CurrentUser != null && CurrentUser.Id > 0)
            {
                return RedirectToAction("profile", "account");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (Request.IsAuthenticated && CurrentUser != null && CurrentUser.Id > 0)
            {
                return RedirectToAction("profile", "account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var loginApi = GoEatApi.Instance;

            // TODO: Get IP Address //
            IPAddress ip = IPAddress.Any; //  YN: This is temp to make it compilable. Need to get ip address from Request //
            var user = await loginApi.Login(model.username.ToLower().Trim(), model.password, "");
            if (user.Succeeded)
            {
                var applicationUser = new ApplicationUser
                {
                    Id = user.Data.profile.uid,
                    Email = user.Data.profile.email,
                    UserName = user.Data.profile.account
                };

                await SignInAsync(applicationUser, model.remember_account == "on" ? true : false);

                if (!String.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToLocal(returnUrl);
                }
                return Redirect("profile");
            }
            ModelState.AddModelError("CustomError", Resources.Resources.Username_and_or_password_are_not_correct);
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [Route("register/{referralName?}")]
        public ActionResult Register(string referralName = null)
        {
            if (Request.IsAuthenticated && CurrentUser != null && CurrentUser.Id > 0)
            {
                return RedirectToAction("profile", "account");
            }

            var model = new RegisterViewModel
            {
                referralID = referralName

            };
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        [Route("register/")]
        public async Task<JsonResult> Register(RegisterViewModel model)
        {
            try
            {
                if (Request.IsAuthenticated && CurrentUser != null && CurrentUser.Id > 0)
                {
                    return Json(new { success = true, redirect = Url.Action("profile", "account") });
                }

                model.email = model.email.Split('@')[0] + "@" + model.email.Split('@')[1].ToLower();

                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(model.submit) && model.submit == "submit")
                    {
                        var api = GoEatApi.Instance;
                        string ip = Request.UserHostAddress == null || Request.UserHostAddress == "::1"
                            ? IPAdressExtension.GetDefaultIp()
                            : Request.UserHostAddress;

                        var result = await api.Register(model.username, model.password, String.Empty,
                            model.email, String.Empty, Genders.Male.ToString(), model.referralID, ip);

                        if (!result.Succeeded || !result.HasData)
                        {
                            switch (result.Error)
                            {
                                case ErrorCodes.UserNameOrEmailExist:
                                    ModelState.AddModelError("username", Messages.USERNAME_OR_EMAIL_EXIST);
                                    break;
                                case ErrorCodes.InvalidReferrerId:
                                    ModelState.AddModelError("referralID", Messages.REFERAL_CODE_ERROR);
                                    break;
                                case ErrorCodes.MissingFields:
                                default:
                                    ModelState.AddModelError("CustomError", Messages.MISSING_FIELDS);
                                    break;
                            }
                        }
                        else
                        {

                            //Login User
                            var applicationUser = new ApplicationUser
                            {
                                Id = result.Data.profile.uid,
                                Email = result.Data.profile.email,
                                UserName = result.Data.profile.account
                            };

                            await SignInAsync(applicationUser, false);

                            return Json(new { success = true, redirect = Url.Action("profile", "account") });
                        }
                    }
                }

                //Something failed, display errors
                // Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { errors = Errors(ModelState) });
            }
            catch
            {
                ModelState.AddModelError("CustomError", Messages.MISSING_FIELDS);
                return Json(new { errors = Errors(ModelState) });
            }
        }

        [Authorize]
        [Route("discount")]
        public ActionResult Discount()
        {
            if (this.HasPermission("access_transaction"))
            {
                return RedirectToAction("index", "transaction");
            }
            //add discount
            var userApi = GoEatApi.Instance;
            var user = userApi.GetRolesByUserId(CurrentUser.Id);
            if (user.Succeeded)
            {
                if (user.Data.Count == 0)
                {
                    var goEatApi = GoEatApi.Instance;
                    var currentDiscount = goEatApi.GetCurrentDiscount(CurrentUser.Id, Urls.GetRestaurantId());
                    if (!currentDiscount.Succeeded)
                    {
                        goEatApi.RegisterDiscount(CurrentUser.Id, Urls.GetDiscountId());
                    }
                }
            }

            var model = new DiscountViewModel();
            var api = GoEatApi.Instance;
            var discount = api.GetCurrentDiscount(CurrentUser.Id, Urls.GetRestaurantId());
            if (discount.Succeeded)
            {
                model = new DiscountViewModel
                {
                    RestaurentId = Urls.GetRestaurantId(),
                    UserId = CurrentUser.Id,
                    DiscountId = discount.Data.id
                };

                model.qrUrl = Cryptogahpy.Base64Encode(String.Format("{0}-{1}-{2}", model.RestaurentId, model.UserId, model.DiscountId));
            }
            return View(model);
        }

        public ActionResult GenerateBarcode(String barcodeText)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(barcodeText, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            Stream memoryStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream, new Point { X = 500, Y = 500 });

            memoryStream.Position = 0;

            var resultStream = new FileStreamResult(memoryStream, "image/png");
            resultStream.FileDownloadName = String.Format("{0}.png", barcodeText);

            return resultStream;

        }

        [Authorize]
        [Route("profile")]
        public new ActionResult Profile()
        {
            if (this.HasPermission("access_transaction"))
            {
                return RedirectToAction("index", "transaction");
            }

            var model = new ProfileViewModel();
            var goEatAPI = GoEatApi.Instance;
            var creditBalance = goEatAPI.GetCustomerTotalToken(CurrentUser.Id);
            var discountModal = GetDiscount();
            if (creditBalance.HasData)
            {
                model.TotalToken = creditBalance.Data.token;
            }
            model.qrUrl = discountModal != null ? discountModal.qrUrl : String.Empty;

            var customer = goEatAPI.GetUserById(CurrentUser.Id);
            model.Profile = new EditProfileModel
            {
                Nickname = customer.Data.Profile.nickname,
                ShortBio = customer.Data.Profile.bio,
                Email = customer.Data.Profile.email,
                CountryCode = customer.Data.Profile.country_code,
                Country = customer.Data.Profile.country_name,
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [Route("edit-profile")]
        public async Task<JsonResult> EditProfile(EditProfileModel model)
        {
            try
            {
                model.Email = model.Email.Split('@')[0] + "@" + model.Email.Split('@')[1].ToLower();
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = Messages.INPUT_WRONG_DATA
                    });
                }

                //call to api: api/1/account/edit-profile
                var goEatApi = GoEatApi.Instance;
                var customer = goEatApi.GetUserById(CurrentUser.Id);
                if (!customer.Succeeded)
                {
                    return Json(new { success = false, message = Messages.SERVER_ERROR });
                }

                var isSuccess = await goEatApi.GTokenEditProfile(customer.Data.session, model.Email, model.Nickname,
                    customer.Data.Profile.gender, model.ShortBio, model.CountryCode, model.Country, customer.Data.Profile.inviter);
                if (!isSuccess.Succeeded)
                {
                    return Json(new { success = false, message = Messages.SERVER_ERROR });
                }

                return Json(new { success = true });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = Messages.SERVER_ERROR
                });
            }
        }

        private DiscountViewModel GetDiscount()
        {
            var userApi = GoEatApi.Instance;
            var user = userApi.GetRolesByUserId(CurrentUser.Id);
            if (user.Succeeded)
            {
                if (user.Data.Count == 0)
                {
                    var goEatApi = GoEatApi.Instance;
                    var currentDiscount = goEatApi.GetCurrentDiscount(CurrentUser.Id, Urls.GetRestaurantId());
                    if (!currentDiscount.Succeeded)
                    {
                        goEatApi.RegisterDiscount(CurrentUser.Id, Urls.GetDiscountId());
                    }
                }
            }

            var model = new DiscountViewModel();
            var api = GoEatApi.Instance;
            var discount = api.GetCurrentDiscount(CurrentUser.Id, Urls.GetRestaurantId());
            if (discount.Succeeded)
            {
                model = new DiscountViewModel
                {
                    RestaurentId = Urls.GetRestaurantId(),
                    UserId = CurrentUser.Id,
                    DiscountId = discount.Data.id
                };

                model.qrUrl = Cryptogahpy.Base64Encode(String.Format("{0}-{1}-{2}", model.RestaurentId, model.UserId, model.DiscountId));
            }
            return model;

        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [Authorize]
        [Route("change-password")]
        public ActionResult ChangePassword()
        {
            var modal = new ChangePasswordViewModel();
            return View(modal);
        }

        [HttpPost]
        [Authorize]
        [Route("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = Messages.INPUT_WRONG_DATA
                    });
                }

                var customer = GoEatApi.Instance.GetUserById(CurrentUser.Id);
                //call to API: /api/1/account/change-password
                var result = await GoEatApi.Instance.ChangePasswordGToken(customer.Data.session, model.OldPassword, model.NewPassword, model.ConfirmPassword);

                //return data
                return Json(new
                {
                    success = result.Succeeded,
                    message = Messages.INVALID_PASSWORD
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    message = Messages.SERVER_ERROR
                });
            }
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/LogOff
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("index", "main");
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
        [Route("account/update_locale)")]
        public ActionResult updateLocale()
        {
            var culture = Request["locale"];
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a session
            Session["_culture"] = culture;
            if (Request.Params["next"] != null)
            {
                return Redirect(Request.Params["next"]);
            }
            return RedirectToAction("Index", "Main");
        }

        #region Helpers

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(_userManager));
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

        [Route("contact")]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [Route("contact/send-feedback")]
        public JsonResult SendFeedback(FeedbackModel model)
        {
            model.Email = model.Email.Split('@')[0] + "@" + model.Email.Split('@')[1].ToLower();
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please input the required fields." });
            }

            try
            {
                //Build email header
                string to = ConfigurationManager.AppSettings["receiverEmail"];
                string from = model.Email;
                string cc = ConfigurationManager.AppSettings["CCEmail"];
                MailMessage message = new MailMessage(from, to);
                message.Subject = ConfigurationManager.AppSettings["FeedbackSubject"];

                if (!String.IsNullOrEmpty(cc))
                {
                    message.CC.Add(cc);
                }
                //Build email content
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendLine("Customer: ");
                strBuilder.AppendLine("Name: " + model.Name);
                strBuilder.AppendLine("Email: " + model.Email);
                strBuilder.AppendLine("Phone: " + model.Phone);
                strBuilder.AppendLine(string.Empty);
                strBuilder.AppendLine("Message:");
                strBuilder.Append(model.Message);
                message.Body = strBuilder.ToString();

                SmtpClient client = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };

                client.Send(message);
            }
            catch
            {
                return Json(new { success = false, message = "Please input the required fields." });
            }
            return Json(new { success = true, message = "Your feeback was sent successfully." });
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
    }
}