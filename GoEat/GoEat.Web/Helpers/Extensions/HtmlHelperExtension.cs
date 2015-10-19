using System.Web;
using System.Web.Mvc;
using GoEat.Web.Const;
using GoEat.Web.Identity;

namespace GoEat.Web.Helpers.Extensions
{
    public static class HtmlHelperExtension
    {
        public static ApplicationUser GetCurrentUser(this HtmlHelper helper)
        {
            return HttpContext.Current.Items[Constants.CurrentUserHttpContextKey] as ApplicationUser;
        }
    }
}