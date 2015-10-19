using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace GoEat.Web.Helpers.Extensions
{
    public static class StringHelper
    {
        public static string GetValueFromJsonString(this string jsonString, string key)
        {
            var result = String.Empty;
            try
            {
                var json = JObject.Parse(jsonString);
                if (json[key] != null) return json[key].ToString();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
    public class MostlyRequiredAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            return !string.IsNullOrEmpty(value.ToString());
        }
    }
    public class IsEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) {
                return false;
            }

            var emailRegex1 = new Regex(@"^[_a-zA-Z0-9-]+((\.[_a-zA-Z0-9-]+)|(\+[_a-zA-Z0-9-]+))*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$");

            var emailRegex2 = new Regex(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$");

            if (emailRegex1.Match(value.ToString()).Success || emailRegex2.Match(value.ToString()).Success)
            {
                return true;
            }
            return false;
        }
    }

}