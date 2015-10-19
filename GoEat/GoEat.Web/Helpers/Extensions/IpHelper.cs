using System.Net;
using System.Web;

namespace GoEat.Web.Helpers.Extensions
{
    public static class IpHelper
    {
        /// <summary>
        /// Get user ip from current request
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetRequestIP(this HttpRequestBase request)
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