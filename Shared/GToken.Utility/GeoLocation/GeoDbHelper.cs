using System;
using System.Net;
using MaxMind.Db;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Platform.Utility
{
    public class GeoDbHelper
    {
        private static readonly Reader Reader;

        #region Singleton
        private GeoDbHelper() { }

        public static readonly GeoDbHelper Instance;


        static GeoDbHelper()
        {
            Instance = new GeoDbHelper();
            Reader = new Reader(AppDomain.CurrentDomain.BaseDirectory + "/Data/geoip_db/GeoLite2-Country.mmdb", FileAccessMode.Memory);
        }
        #endregion



        public JToken Find(IPAddress ip)
        {
            if (Reader == null)
                return null;

            return Reader.Find(ip);
        }
    }
}
