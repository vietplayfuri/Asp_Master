using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Utility
{
    public class TimeHelper
    {
        public static int epoch()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            return secondsSinceEpoch;
        }

        public static DateTime convertDatetimeToCountryTimezone(DateTime dt, string countryCode)
        {
            DateTime utc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

            var CountryInfo = (from location in TzdbDateTimeZoneSource.Default.ZoneLocations
                               where location.CountryCode.Equals(countryCode,
                                          StringComparison.OrdinalIgnoreCase)
                               select new { location.ZoneId, location.CountryName })
             .FirstOrDefault();
            if (CountryInfo != null)
            {
                var TimeZone = DateTimeZoneProviders.Tzdb[CountryInfo.ZoneId];
                DateTime localDate = Instant.FromDateTimeUtc(utc)
                          .InZone(TimeZone)
                          .ToDateTimeUnspecified();
                return localDate;
            }
            return new DateTime();
        }

        public static string localizeDatetime(DateTime dt, string format=null, string country_code = null)
        {
            /*  Utility function to return babel formatted datetime string.
             If :attr:`country_code` is specified, convert `dt` to timezone datetime
             using :meth:`convertDatetimeToCountryTimezone()`

             :param dt: the input datetime object
             :param format: default to "MMM d, yyyy"
             :param country_code: default to None
             :return: the babel formatted datetime string
             */
            if (dt == null || dt == DateTime.MinValue)
                return string.Empty;
            if(string.IsNullOrEmpty(format))
                format = "MMM d, yyyy";
            if (!String.IsNullOrEmpty(country_code))
            {
                dt = convertDatetimeToCountryTimezone(dt, country_code);
                return dt.ToString(format);
            }
            return dt.ToString(format);
        }

        public static int EpochFromDatetime(DateTime datetime)
        {
            TimeSpan t = datetime - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            return secondsSinceEpoch;
        }
    }
}
