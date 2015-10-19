using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using GoPlay.Web.Identity;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Helpers;

namespace GoPlay.Web.ActionFilter
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
                filterContext.Result = new RedirectResult("~/account/profile");
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class RequiredLoginAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                var returnUrl = filterContext.HttpContext.Request.Url.AbsolutePath;
                filterContext.Result = new RedirectResult("~/account/login?returnUrl=" + returnUrl);
            }
        }
    }

    public class RBACAttribute : AuthorizeAttribute
    {
        public string AccessAction { get; set; }
        public string AccessRole { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var roles = UserHelper.GetRoles(HttpContext.Current.User.Identity.GetUserId<int>());
                if (!roles.Any(r => r.RoleName == "admin"))
                {
                    if (!roles.Any(r => r.RoleName == AccessRole) && !roles.Any(r => r.Permissions.Any(a => a.action == AccessAction)))
                    {
                        filterContext.Result = new HttpStatusCodeResult(403);
                    }
                }
            }
        }
    }
}