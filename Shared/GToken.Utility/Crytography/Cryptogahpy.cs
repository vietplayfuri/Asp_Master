using System;
using System.Text;

namespace Platform.Utility.Crytography
{
    public static class Cryptogahpy
    {
        public static string Base64Encode(string plainText)
        {
            try
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch
            {
            }
            return null;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
            }
            return null;
        }
    }
}
