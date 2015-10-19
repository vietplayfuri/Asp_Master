using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using GToken.Web.Const;
using GToken.Web.Identity;

namespace GToken.Web.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationUser CurrentUser { get; private set; }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            cultureName = Session["locale"] != null ? Session["locale"].ToString() : null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName);

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            // Parse TimeZoneOffset.
            ViewBag.TimeZoneOffset = TimeSpan.FromMinutes(0); // Default offset (Utc) if cookie is missing.
            var timeZoneCookie = Request.Cookies["_timeZoneOffset"];
            if (timeZoneCookie != null)
            {

                double offsetMinutes = 0;
                if (double.TryParse(timeZoneCookie.Value, out offsetMinutes))
                {
                    ViewBag.TimeZoneOffset = TimeSpan.FromMinutes(offsetMinutes);
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
            var api = Platform.Core.Api.Instance;
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
    }
}