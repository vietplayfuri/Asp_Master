using GoPlay.Models;
using Newtonsoft.Json;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public async Task<Result<APIUpointModel>> UpointAPI(APIUpointParamModel model)
        {
            using (var client = new HttpClient())
            {
                List<KeyValuePair<string, string>> ValueCollection = new List<KeyValuePair<string, string>>();
                string action = string.Empty;
                if (model.enumAction == EUpointAction.GetTicketTelkomsel)
                {
                    ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SECRET_TOKEN, model.secret_token));
                    action = ConfigurationManager.AppSettings["UPOINT_TELKOMSEL_VOUCHER_TICKET_ENDPOINT"];
                }
                else
                {
                    ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SECRET_TOKEN, model.secret_token));
                    ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_TRX_ID, model.trx_id));
                    ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_ITEM, model.item));
                    if (model.amount > -1)
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_AMOUNT, model.amount.ToString()));

                    switch (model.enumAction)
                    {
                        case EUpointAction.BalanceDeduction:
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PHONE_NUMBER, model.phone_number));
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_CALLBACK_URL, model.callback_url));
                            action = ConfigurationManager.AppSettings["UPOINT_BALANCE_DEDUCTION_ENDPOINT"];
                            break;
                        case EUpointAction.Speedy:
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PHONE_NUMBER, model.phone_number));
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SPEEDY_NUMBER, model.speedy_number));
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_IP, model.ip));
                            action = ConfigurationManager.AppSettings["UPOINT_SPEEDY_ENDPOINT"];
                            break;
                        case EUpointAction.TMoney:
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PHONE_NUMBER, model.phone_number));
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_CALLBACK_URL, model.callback_url));
                            action = ConfigurationManager.AppSettings["UPOINT_TMONEY_ENDPOINT"];
                            break;
                        case EUpointAction.TelkomselVoucher:
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_HRN, model.hrn));
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_TICKET, model.ticket));
                            action = ConfigurationManager.AppSettings["UPOINT_TELKOMSEL_VOUCHER_SUBMIT_ENDPOINT"];
                            break;
                        case EUpointAction.StandardVoucher:
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_HRN, model.hrn));
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_VSN, model.vsn));
                            ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PHONE_NUMBER, model.phone_number));
                            action = ConfigurationManager.AppSettings["UPOINT_STANDARD_VOUCHER_ENDPOINT"];
                            break;
                        default:
                            break;
                    }
                }
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var formContent = new FormUrlEncodedContent(ValueCollection);
                HttpResponseMessage response = client.PostAsync(action, formContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<APIUpointModel>(await response.Content.ReadAsStringAsync());
                    return Result<APIUpointModel>.Make(result);
                }
                return Result<APIUpointModel>.Null(ErrorCodes.ServerError);
            }
        }
    }
}
