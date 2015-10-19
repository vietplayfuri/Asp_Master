using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Core.Payment_Gateways.PayPal
{
    public class PayPalSettings
    {
        private bool _isSandbox;

        public string Business { get; set; }
        public string PDTToken { get; set; }
        public string ReturnUrl { get; set; }
        public string IpnUrl { get; set; }

        public string WebscrDomain
        {
            get
            {
                return (_isSandbox) ? @"https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";
            }
        }

        public PayPalSettings(bool isSandBox)
        {
            _isSandbox = isSandBox;
        }
    }
}
