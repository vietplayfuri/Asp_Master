using System;
using System.Text;
namespace Platform.Models
{
    public class Partner : ModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string uid { get; set; }
        public string endpoint { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string _redirect_uris { get; set; }
        public string _default_scopes { get; set; }
        public string identifier { get; set; }
        public string secret_key { get; set; }


        public bool IsValidHash(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var input = uid + secret_key;
            var realToken = System.BitConverter.ToString(new System.Security.Cryptography.SHA512CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty).ToLower();

            return string.Compare(token, realToken, StringComparison.OrdinalIgnoreCase) == 0;
        }
      
    }
}
