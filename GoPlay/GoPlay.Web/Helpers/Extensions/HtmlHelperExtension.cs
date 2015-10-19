using GoPlay.Web.Const;
using GoPlay.Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
namespace GoPlay.Web
{
    public static class HtmlHelperExtension
    {
        public static ApplicationUser GetCurrentUser(this HtmlHelper helper)
        {
            return HttpContext.Current.Items[Constants.CurrentUserHttpContextKey] as ApplicationUser;
        }
    }
}