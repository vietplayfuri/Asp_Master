using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace GoPlay.Web.Helpers.Extensions
{
    public static class WebIpHelper
    {
        /// <summary>
        /// Get user ip from current request
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetClientIp(this HttpRequestBase request)
        {
            string ipList = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return IPAddress.Parse(ipList.Split(',')[0]);
            }

            return IPAddress.Parse(request.ServerVariables["REMOTE_ADDR"]);
        }
    }
}