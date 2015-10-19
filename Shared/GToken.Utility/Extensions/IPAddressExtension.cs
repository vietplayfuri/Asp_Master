using System;
using System.Net;

namespace Platform.Utility
{
    static public class IPAdressExtension
    {
        public static bool GetCountryCode(this IPAddress ip, Action<string> setCode, Action<string> setName)
        {
            bool success = false;

            var data = GeoDbHelper.Instance.Find(ip);
            if (data != null)
            {
                var country = data["country"];
                if (country != null)
                {
                    var names = country.SelectToken("names");
                    if (names != null)
                    {
                        setName ( names.SelectToken("en").ToString() );
                        setCode( country.SelectToken("iso_code").ToString() );
                        success = true;
                    }
                }
            }

            if (!success)
            {

                // Set Default as per the old python codes.Maintain backward compatibility //
                setCode("SG");
                setName("Singapore");
            }         
            return success;
        }

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
