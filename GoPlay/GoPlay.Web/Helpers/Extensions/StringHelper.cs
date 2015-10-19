using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Helpers.Extensions
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
}