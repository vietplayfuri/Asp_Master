using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace GoPlay.Models
{
    public class APIUpointModel
    {
        public bool result { get; set; }
        public string trx_id { get; set; }
        public string error_code { get; set; }
        public string error_info { get; set; }
        public string sms_code { get; set; }
        public string t_id { get; set; }
        public decimal amount { get; set; }
        public string redirect { get; set; }
        public string code { get; set; }
        public string ticket { get; set; }
        public string info { get; set; }
        public decimal? nominal { get; set; }

    }
    public class APIUpointParamModel
    {
        public APIUpointParamModel(coin_transaction trans, string itemDescription = null)
        {
            secret_token = ConfigurationManager.AppSettings["UPOINT_SECRET_TOKEN"];
            Guid order_temp = Guid.NewGuid();
            Guid.TryParse(trans.order_id, out order_temp);
            trx_id = order_temp.ToString("N");//convert guid to hex
            item = !string.IsNullOrEmpty(itemDescription) ? itemDescription : trans.description;
            decimal GtokenRate = 0;
            decimal.TryParse(ConfigurationManager.AppSettings["IDR_PER_GTOKEN_RATE"], out GtokenRate);
            amount = trans.amount.HasValue ? trans.amount.Value * GtokenRate : 0;
        }
        public APIUpointParamModel()
        {
            secret_token = ConfigurationManager.AppSettings["UPOINT_SECRET_TOKEN"];
        }
        public EUpointAction enumAction { get; set; }
        public string secret_token { get; set; }
        public string trx_id { get; set; }
        public string item { get; set; }
        public decimal amount { get; set; }
        public string phone_number { get; set; }
        public string speedy_number { get; set; }
        public string callback_url { get; set; }
        public string ip { get; set; }
        public string ticket { get; set; }
        public string hrn { get; set; }
        public string vsn { get; set; }
    }

    public class AdaptivePayments
    {
        public AdaptivePayments()
        {
            receiverList = new List<AdaptivePaymentsReceiver>();
            requestEnvelope = new AdaptivePaymentsRequestEnvelope();
        }
        public string actionType { get; set; }
        public List<AdaptivePaymentsReceiver> receiverList { get; set; }
        public string currencyCode { get; set; }
        public string cancelUrl { get; set; }
        public string ReturnUrl { get; set; }
        public AdaptivePaymentsRequestEnvelope requestEnvelope { get; set; }
        public bool reverseAllParallelPaymentsOnError { get; set; }
        public string feesPayer { get; set; }
        public string preapprovalKey { get; set; }

        public string senderEmail { get; set; }
        public string payKeyDuration { get; set; }
    }

    public class AdaptivePaymentsRequestEnvelope
    {
        public string errorLanguage { get; set; }
        public string detailLevel { get; set; }
        public DateTime timestamp { get; set; }
        public string ack { get; set; }
        public string correlationId { get; set; }
        public string build { get; set; }
        public string payKey { get; set; }
        public string paymentExecStatus { get; set; }
    }
    public class AdaptivePaymentsReceiver
    {
        public string email { get; set; }
        public decimal amount { get; set; }
    }

    public class AdaptivePaymentsResult
    {
        public AdaptivePaymentsRequestEnvelope responseEnvelope { get; set; }

    }
}
