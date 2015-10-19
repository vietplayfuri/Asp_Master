using System.Web;
using Microsoft.AspNet.Identity.Owin;
using GoPlay.Web.Identity;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using GoPlay.Web.Models;
using GoPlay.Core;
namespace GoPlay.Web.Controllers
{
    [RoutePrefix("oauth")]
    public class OauthController : BaseController
    {
        private ApplicationUserManager _userManager;

        public OauthController(IUserStore<ApplicationUser, int> userStore)
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


        [Route("save_token")]
        public ActionResult SaveToken(string token, OauthViewModels model)
        {
            var api = GoPlayApi.Instance;

            //var oauthBearerToken = api.

            return null;
        }
        


    }
}