using GoEat.Web.Const;
using GoEat.Web.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using GoEat.Dal;
using GoEat.Models;

namespace GoEat.Web.Api.V1
{
    public class BaseApiController : ApiController
    {
        protected ApplicationUser CurrentUser { get; private set; }

        protected override void Initialize(HttpControllerContext requestContext)
        {
            base.Initialize(requestContext);

            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                if (requestContext.Request.Headers.Contains(ConstantValues.S_SESSION))
                {
                    var tokens = requestContext.Request.Headers.GetValues(ConstantValues.S_SESSION).ToList<string>();
                    var api = GoEat.Core.GoEatApi.Instance;
                    var user = api.GetUserSession(tokens.First());
                    if (!user.Succeeded)
                    {
                        return;
                    }

                    CurrentUser = new ApplicationUser(user.Data.Profile);

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
                var api = GoEat.Core.GoEatApi.Instance;
                var user = api.GetUserById(id);
                if (user.Data == null)
                {
                    return;
                }

                CurrentUser = new ApplicationUser(user.Data.Profile);
                // Add user to current request so it can be retrieved elsewhere
                if (!HttpContext.Current.Items.Contains(Constants.CurrentUserHttpContextKey))
                    HttpContext.Current.Items.Add(Constants.CurrentUserHttpContextKey, CurrentUser);
            }
        }
    }
}
