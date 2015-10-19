
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GoEat.Core;
using GoEat.Web;
using GoEat.Web.Api.V1;
using GoEat.Web.Identity;
using GoEat.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace GoEat.WebApi.V1
{
    public class AccountController : BaseApiController
    {

        private readonly ApplicationUserManager _userManager;

        public AccountController(IUserStore<ApplicationUser, int> userStore)
        {
            _userManager = new ApplicationUserManager(userStore);
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("login-password")]
        public async Task<object> LoginPassword([FromBody] LoginParam param)
        {
            if (param == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var api = GoEatApi.Instance;
            var user = await api.Login(param.username, param.password, param.game_id);
            if (user.Succeeded)
            {
                var applicationUser = new ApplicationUser
                {
                    Id = user.Data.profile.uid,
                    Email = user.Data.profile.email,
                    UserName = user.Data.profile.account
                };

                await SignInAsync(applicationUser, true);
                var gtokenApi = GoEatApi.Instance;
                var roles = gtokenApi.GetRolesByUserId(applicationUser.Id);
                
                
                // For Restaurant Login, we are just going to return something simple //
                return Json(new
                {   
                    success = true, user.Data.session,
                    roles = roles.Data.Select(x => x.id)
                });
            }
            return Json(new
            {
                success = false,
                error_code = ErrorCodes.InvalidUserNameOrPassword.ToString()
            });

        }


        [HttpPost]
        [AllowAnonymous]
        [ActionName("reset-password")]
        public async Task<object> ResetPassword([FromBody] LoginParam param)
        {
            if (param == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var api = GoEatApi.Instance;
            bool ok = await api.ResetPassword(param.username);
          
            return Json(new
            {
                success = ok,
                error_code = ok ? string.Empty : ErrorCodes.InvalidUserNameOrPassword.ToString()
            });

        }

        #region helper

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
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        #endregion helper

    }
}