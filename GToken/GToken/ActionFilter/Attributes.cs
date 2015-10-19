using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using GToken.Web.Identity;

namespace GToken.Web.ActionFilter
{
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class RequiredNotLoginAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/transaction/index");
            }
        }
    }

    public class RBACAttribute : AuthorizeAttribute
    {
        public string AccessAction { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                RBAC requestingUser = new RBAC(filterContext.RequestContext
                                                       .HttpContext.User.Identity.GetUserId<int>());

                if (!requestingUser.HasPermission(AccessAction))
                {
                    filterContext.Result = new HttpStatusCodeResult(403);
                }
            }
        }
    }
}