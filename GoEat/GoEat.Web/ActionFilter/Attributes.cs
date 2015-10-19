using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using GoEat.Web.Identity;

namespace GoEat.Web.ActionFilter
{
    public class Attributes
    {
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            AntiForgery.Validate(cookie != null ? cookie.Value : null, httpContext.Request.Headers["X-CSRFToken"]);
        }
    }

    public class RBACAttribute : AuthorizeAttribute
    {
        public string AccessAction { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            RBAC requestingUser = new RBAC(filterContext.RequestContext
                                                   .HttpContext.User.Identity.GetUserId<int>());

            if (!requestingUser.HasPermission(AccessAction))
            {
                filterContext.Result = new RedirectToRouteResult(
                                               new RouteValueDictionary { 
                                                { "action", "Index" }, 
                                                { "controller", "Unauthorised" } });
            }
        }
    }
}