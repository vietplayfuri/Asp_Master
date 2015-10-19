using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GoPlay.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        //Builds URL by finding the best matching route that corresponds to the current URL,
        //with given parameters added or replaced.
        public static MvcHtmlString Current(this UrlHelper helper, object substitutes)
        {
            //get the route data for the current URL e.g. /Research/InvestmentModelling/RiskComparison
            //this is needed because unlike UrlHelper.Action, UrlHelper.RouteUrl sets includeImplicitMvcValues to false
            //which causes it to ignore current ViewContext.RouteData.Values
            var rd = new RouteValueDictionary(helper.RequestContext.RouteData.Values);

            //get the current query string e.g. ?BucketID=17371&amp;compareTo=123
            var qs = helper.RequestContext.HttpContext.Request.QueryString;

            //add query string parameters to the route value dictionary
            foreach (string param in qs)
                if (!string.IsNullOrEmpty(qs[param]))
                    rd[param] = qs[param];

            //override parameters we're changing
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(substitutes.GetType()))
            {
                var value = property.GetValue(substitutes).ToString();
                if (string.IsNullOrEmpty(value)) rd.Remove(property.Name); else rd[property.Name] = value;

            }
            //UrlHelper will find the first matching route
            //(the routes are searched in the order they were registered).
            //The unmatched parameters will be added as query string.
            var url = helper.RouteUrl(rd);
            return new MvcHtmlString(url);

        }

        /// <summary>
        /// + means a space only in application/x-www-form-urlencoded content
        /// </summary>
        public static string GetSession(string sessionInRequest)
        {
            if (string.IsNullOrEmpty(sessionInRequest)) return sessionInRequest;

            return sessionInRequest.Contains(' ')
                ? sessionInRequest.Replace(' ', '+')
                : sessionInRequest;
        }

        public static bool GetFileName(string inputFilename, out string outputFilename)
        {
            outputFilename = string.Empty;
            try
            {

                if (string.IsNullOrEmpty(inputFilename))
                    return true;

                if (inputFilename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) == -1) //filename is valid
                {
                    outputFilename = inputFilename;
                    return true;
                }
                else
                {
                    outputFilename = Path.GetFileName(inputFilename);
                }

                if (outputFilename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1) //filename is invalid
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}