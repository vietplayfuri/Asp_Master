using GoPlay.Core;
using GoPlay.Web.Const;
using GoPlay.Web.Identity;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace GoPlay.Web.Controllers
{
    public class BaseController : Controller
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected ApplicationUser CurrentUser { get; private set; }

        //protected bool IsAuthenticated
        //{
        //    get { return CurrentUser != null ? true : false; }
        //    private set { CurrentUser = value; }
        //}

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;


            cultureName = Session["locale"] != null ? Session["locale"].ToString() : null;
            if (string.IsNullOrEmpty(cultureName) && CurrentUser != null)
                cultureName = CurrentUser.locale;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            // Parse TimeZoneOffset.
            ViewBag.TimeZoneOffset = TimeSpan.FromMinutes(0); // Default offset (Utc) if cookie is missing.
            var timeZoneCookie = Request.Cookies["_timeZoneOffset"];
            if (timeZoneCookie != null)
            {

                double offsetMinutes = 0;
                if (double.TryParse(timeZoneCookie.Value, out offsetMinutes))
                {
                    ViewBag.TimeZoneOffset = TimeSpan.FromMinutes(offsetMinutes); // Store in ViewBag. You can use Session, TempData, or anything else.
                }
            }

            return base.BeginExecuteCore(callback, state);
        }

        public static Hashtable Errors(ModelStateDictionary modelState)
        {
            var errors = new Hashtable();
            foreach (var pair in modelState)
            {
                if (pair.Value.Errors.Count > 0)
                {
                    errors[pair.Key] = pair.Value.Errors.Select(error => error.ErrorMessage).ToList();
                }
            }
            return errors;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (!Request.IsAuthenticated)
            {
                return;
            }
            var id = User.Identity.GetUserId<int>();
            var api = GoPlayApi.Instance;
            var user = api.GetUserById(id);
            if (user.Data == null)
            {
                return;
            }
            CurrentUser = new ApplicationUser(user.Data);
            // Add user to current request so it can be retrieved elsewhere
            if (!HttpContext.Items.Contains(Constants.CurrentUserHttpContextKey))
                HttpContext.Items.Add(Constants.CurrentUserHttpContextKey, CurrentUser);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            StringBuilder errorBuilder = new StringBuilder();
            errorBuilder.AppendLine("--------------------------: Controller ");
            string url = string.Empty;
            if (filterContext.HttpContext != null
                && filterContext.HttpContext.Request != null
                && filterContext.HttpContext.Request.Url != null)
            {
                url = filterContext.HttpContext.Request.Url.ToString();
            }
            errorBuilder.AppendLine("URL:  " + url);
            errorBuilder.AppendLine("Form:  " + filterContext.HttpContext.Request.Form.ToString());
            errorBuilder.AppendLine(Environment.NewLine);
            errorBuilder.AppendLine(filterContext.Exception.StackTrace);

            //Log file and send email
            logger.Error(errorBuilder.ToString());
            base.OnException(filterContext);
        }
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

}