using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Platform.Core.Payment_Gateways.PayPal
{
    public class AutoReturlParam
    {
        public const string TransactionId = "tx";
        public const string PaymentStatus = "st";
        public const string PaymentAmount = "amt";
        public const string CurrencyCode = "cc";
    }

    public class PayPal
    {
        public PayPalSettings Settings { get; private set; }
        public PayPal(PayPalSettings settings)
        {
            Settings = settings;
        }

        public PayPalRedirect CreateBuyNowRedirect(PayPalOrder orer)
        {
            NameValueCollection values = new NameValueCollection();
            values["business"] = Settings.Business;
            values["cmd"] = "_xclick";
            values["item_name"] = orer.ItemName;
            values["amount"] = orer.Price.ToString();
            values["return"] = HttpUtility.UrlEncode(Settings.ReturnUrl);
            values["notify_url"] = HttpUtility.UrlEncode(Settings.IpnUrl);

            var url = GetUrl(values, Settings.WebscrDomain);
            
            return new PayPalRedirect()
            {
                Url = url
            };
        }

        public PDTHolder GetPaymentDataTransfer(string transactionID)
        {
            var authToken = Settings.PDTToken;
            var query = string.Format("cmd=_notify-synch&tx={0}&at={1}", transactionID, authToken);

            // Create the request back
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Settings.WebscrDomain);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = query.Length;

            // Write the request back IPN strings
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(),System.Text.Encoding.ASCII);
            stOut.Write(query);
            stOut.Close();

            // Do the request to PayPal and get the response
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = stIn.ReadToEnd();
            stIn.Close();

            if (strResponse.StartsWith("SUCCESS"))
            {
                PDTHolder pdt = PDTHolder.Parse(strResponse);
                return pdt;
            }
            else
            {
                return null;
            }
        }

        public bool ValidateIpnMessage(string message)
        {
            string postUrl = Settings.WebscrDomain;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(postUrl);
            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            message += "&cmd=_notify-validate";
            req.ContentLength = message.Length;

            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(),System.Text.Encoding.ASCII);
            streamOut.Write(message);
            streamOut.Close();

            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();

            if (strResponse == "VERIFIED")
            {
                return true;
            }
            else if (strResponse == "INVALID")
            {
                return false;
            }
            return false;
        }
        private string GetUrl(NameValueCollection values, string requestUrl)
        {
            string data = String.Join("&", values.Cast<string>()
                .Select(key => String.Format("{0}={1}", key, HttpUtility.UrlEncode(values[key]))));
            return String.Format("{0}/{1}", requestUrl, data);
        }

        private NameValueCollection Submit(NameValueCollection values,string requestUrl)
        {
            string data = String.Join("&", values.Cast<string>()
                .Select(key => String.Format("{0}={1}", key, HttpUtility.UrlEncode(values[key]))));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = "POST";
            request.ContentLength = data.Length;

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(data);
            }

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return HttpUtility.ParseQueryString(reader.ReadToEnd());
            }
        }
    }
}
