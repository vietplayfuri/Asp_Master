using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Globalization;
using NodaTime.TimeZones;

namespace GoPlay.Web.Helpers
{
    public class DateTimeHelper
    {
        public static string localizeDatetime(DateTime? dt = null, string format = null, string country_code = null)
        {
            if (!dt.HasValue) return String.Empty;
            if (String.IsNullOrEmpty(format))
            {
                format = ConfigurationManager.AppSettings["BABEL_DATE_READABLE_FORMAT"];
            }

            if (!String.IsNullOrEmpty(country_code))
            {
                DateTime localDate = convertDatetimeToCountryTimezone(dt.Value, country_code);
                return localDate.ToString(format);
            }
            return dt.Value.ToString(format);
        }
        public static DateTime convertDatetimeToCountryTimezone(DateTime dt, string countryCode)
        {
            var zoneId = TzdbDateTimeZoneSource.Default.ZoneLocations
    .Where(x => x.CountryCode == countryCode)
    .Select(x => x.ZoneId).SingleOrDefault();
            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(zoneId.ToString());
            return TimeZoneInfo.ConvertTimeFromUtc(dt, tst);

        }
    }
}