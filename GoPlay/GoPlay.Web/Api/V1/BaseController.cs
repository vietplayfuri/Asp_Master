using System.Linq;
using GoPlay.Core;
using GoPlay.Web.Identity;
using Platform.Models;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Security.Claims;
using GoPlay.Web.Const;

namespace GoPlay.WebApi.V1
{
    public class BaseApiController : ApiController
    {
        protected ApplicationUser CurrentUser { get; private set; }

        protected override void Initialize(HttpControllerContext requestContext)
        {
            base.Initialize(requestContext);
            var api = GoPlayApi.Instance;

            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                if (requestContext.Request.Headers.Contains(ConstantValues.S_SESSION))
                {
                    var tokens = requestContext.Request.Headers.GetValues(ConstantValues.S_SESSION).ToList();
                    var user = api.GetUserByUserName(tokens.First());
                    if (user == null)
                    {
                        return;
                    }

                    CurrentUser = new ApplicationUser(user.Data);

                    var identity = new ClaimsIdentity(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
                    identity.AddClaim(
                        new Claim(ClaimTypes.Name, CurrentUser.UserName)
                    );

                    HttpContext.Current.User = new ClaimsPrincipal(identity);

                    // Add user to current request so it can be retrieved elsewhere
                    if (!HttpContext.Current.Items.Contains(Constants.CurrentUserHttpContextKey))
                        HttpContext.Current.Items.Add(Constants.CurrentUserHttpContextKey, CurrentUser);
                }
                else
                {
                    return;
                }
                return;
            }
            else
            {
                var id = User.Identity.GetUserId<int>();
                var user = api.GetUserById(id).Data;
                if (user == null)
                {
                    return;
                }

                CurrentUser = new ApplicationUser(user);
                // Add user to current request so it can be retrieved elsewhere
                if (!HttpContext.Current.Items.Contains(Constants.CurrentUserHttpContextKey))
                    HttpContext.Current.Items.Add(Constants.CurrentUserHttpContextKey, CurrentUser);
            }
        }
    }
}
