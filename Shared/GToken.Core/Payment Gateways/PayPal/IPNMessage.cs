using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Platform.Core.Payment_Gateways.PayPal
{
    public class IPNMessage
    {
        public string ReceiverEmail { get; set; }
        public string ReceiverId { get; set; }
        public string ResidenceCountry { get; set; }
        public string PlayerEmail { get; set; }
        public string PlayerId { get; set; }
        public string PlayerStatus { get; set; }
        public string TransactionId { get; set; }

        public static IPNMessage Parse(string message)
        {
            String sKey, sValue;
            var msg = new IPNMessage();
            try
            {
                String[] StringArray = message.Split('\n');
                int i;
                for (i = 1; i < StringArray.Length - 1; i++)
                {
                    String[] StringArray1 = StringArray[i].Split('=');

                    sKey = StringArray1[0];
                    sValue = HttpUtility.UrlDecode(StringArray1[1]);

                    switch (sKey)
                    {
                        case "receiver_email":
                            msg.ReceiverEmail = sValue;
                            break;
                        case "receiver_id":
                            msg.ReceiverId = sValue;
                            break;
                        case "residence_country":
                            msg.ReceiverId = sValue;
                            break;
                        case "payer_email":
                            msg.PlayerEmail = sValue;
                            break;
                        case "payer_id":
                            msg.PlayerId = sValue;
                            break;
                        case "payer_status":
                            msg.PlayerStatus = sValue;
                            break;
                        case "txn_id":
                            msg.TransactionId = sValue;
                            break;
                    }
                }

                return msg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
