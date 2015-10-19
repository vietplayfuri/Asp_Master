using GToken.Web.Const;
using GToken.Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GToken.Web.Helpers.Extensions
{
    public static class HtmlHelperExtension
    {
        public static ApplicationUser GetCurrentUser(this HtmlHelper helper)
        {
            return HttpContext.Current.Items[Constants.CurrentUserHttpContextKey] as ApplicationUser;
        }
        public static MvcHtmlString SmallValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return MvcHtmlString.Create(htmlHelper.ValidationMessageFor(expression).ToString().Replace("span", "small").Replace("field-validation-error", "error"));
        }
    }
}