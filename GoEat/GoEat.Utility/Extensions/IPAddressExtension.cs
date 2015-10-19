using System;
using System.Net;

namespace GoEat.Utility
{
    static public class IPAdressExtension
    {
        public static string GetDefaultCountryCode(this IPAddress ip)
        {
            return "SG";
        }

        public static string GetDefaultCountryName(this IPAddress ip)
        {
            return "Singapore";
        }


        public static string GetDefaultIp()
        {
            return "118.189.35.242";
        }
    }

}
