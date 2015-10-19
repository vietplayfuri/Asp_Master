using CsvHelper;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Platform.Utility
{
    public class Helper
    {
        public static bool IsEmailValid(string value)
        {
            var emailRegex1 = new Regex(@"^[_a-zA-Z0-9-]+((\.[_a-zA-Z0-9-]+)|(\+[_a-zA-Z0-9-]+))*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$");
            var emailRegex2 = new Regex(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$");

            return (emailRegex1.Match(value).Success || emailRegex2.Match(value).Success);
        }

        public static string GetDescription(Enum en)
        {
            try
            {
                Type type = en.GetType();

                MemberInfo[] memInfo = type.GetMember(en.ToString());

                if (memInfo != null && memInfo.Length > 0)
                {
                    object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        return ((DescriptionAttribute)attrs[0]).Description;
                    }
                }
                return en.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetEnumName(Type type, object value)
        {
            try
            {
                return Enum.GetName(type, value);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string CalculateSHA1(string input)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(input);
            SHA1 sha = new SHA1CryptoServiceProvider();
            var result = sha.ComputeHash(data);
            HMACSHA1 hmac = new HMACSHA1(result);

            return Convert.ToBase64String(hmac.Key);

            //Dont use below code because its output has special character of UTF-8
            //return Encoding.UTF8.GetString(hmac.Key);
        }

        public static string OnewayHash(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                return BitConverter.ToString(new SHA512CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty).ToLower();
            }
            return String.Empty;
        }

        public static IEnumerable<TimeZoneInfo> GetTimeZones()
        {
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            return timeZones.AsEnumerable();
        }

        public static DateTime timeFromString(string dateTimeString, string format = "yyyy-MM-dd HH:mm", string timeZone = null)
        {
            DateTime dt = DateTime.ParseExact(dateTimeString, format, CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(timeZone))
            {
                TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTimeToUtc(dt, tst);
            }
            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }

        public static DateTime timeFromString(string dateTimeString, string timeZone = null)
        {
            DateTime dt = DateTime.UtcNow;
            DateTime.TryParse(dateTimeString, out dt);
            if (!string.IsNullOrEmpty(timeZone))
            {
                TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTimeToUtc(dt, tst);
            }
            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }

        public static DateTime ConvertTimeFromUtc(string datetimeString, string timezone)
        {
            DateTime date = DateTime.UtcNow;
            try
            {
                DateTime.TryParse(datetimeString, out date);
                if (!string.IsNullOrEmpty(timezone))
                {
                    return TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById(timezone));
                }
                return date;
            }
            catch
            { return date; }
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            s = s.Trim();
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string displayDecimal(decimal? d)
        {
            if (!d.HasValue)
                return "0";
            if (d.ToString().Contains("."))
            {
                return d.ToString().TrimEnd('0').TrimEnd('.');
            }
            return d.Value.ToString();
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
        public static TimeSpan TimeDelta(int weeks = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            return new TimeSpan(days: weeks * 7 + days, hours: hours, minutes: minutes, seconds: seconds, milliseconds: milliseconds);
        }

        public static string FindCountryName(string countryCode, string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = AppDomain.CurrentDomain.BaseDirectory + "/Data/country/countries-20140629.csv";
            }
            using (StreamReader sr = new StreamReader(filePath))
            {
                var csv = new CsvReader(sr);
                while (csv.Read())
                {
                    var codeField = csv.GetField<string>(0);
                    if (countryCode == codeField.Trim())
                    {
                        var nameField = csv.GetField<string>(1);
                        return nameField;
                    }
                }
            }
            return string.Empty;
        }

        public static T FullFillEmptyFields<T>(object obj) where T : class
        {
            foreach (PropertyInfo pinfo in obj.GetType().GetProperties())
            {
                var value = pinfo.GetValue(obj, null);
                if (value == null && pinfo.PropertyType == typeof(string))
                {
                    pinfo.SetValue(obj, "");
                }
            }
            return (T)obj;
        }
    }
    public static class RandomHelper
    {
        /// <summary>
        ///  random number generator for the enumerable sampler.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        ///  returns a random sample of the elements in an IEnumerable
        /// </summary>
        public static IEnumerable<T> Sample<T>(this IEnumerable<T> population, int sampleSize)
        {
            if (population == null)
            {
                return null;
            }

            List<T> localPopulation = population.ToList();
            if (localPopulation.Count < sampleSize) return localPopulation;

            List<T> sample = new List<T>(sampleSize);

            while (sample.Count < sampleSize)
            {
                int i = random.Next(0, localPopulation.Count);
                sample.Add(localPopulation[i]);
                localPopulation.RemoveAt(i);
            }

            return sample;
        }
    }

    public static class ObjectExtensions
    {
        public static T CastObject<T>(this object obj) where T : class, new()
        {
            try
            {
                T someObject = new T();
                Type someObjectType = someObject.GetType();

                var source = obj.GetType()
                 .GetProperties()
                 .ToDictionary(p => p.Name, p => p.GetValue(obj, null));

                foreach (KeyValuePair<string, object> item in source)
                {
                    if (item.Value != null && someObjectType.GetProperty(item.Key) != null)
                        someObjectType.GetProperty(item.Key).SetValue(someObject, item.Value, null);
                }

                return someObject;
            }
            catch
            { return default(T); }
        }
    }
}
