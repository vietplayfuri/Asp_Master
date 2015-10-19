using GoPlay.Web.Api.Filters;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new ParameterNameAttribute());
        }
    }
}
