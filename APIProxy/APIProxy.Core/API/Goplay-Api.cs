using Platform.Utility;
using APIProxy.Model;
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

namespace APIProxy.Core
{
    public class GoplayService
    {
        #region Singleton
        private GoplayService() { }

        public static readonly GoplayService Instance;

        static GoplayService()
        {
            Instance = new GoplayService();
        }
        #endregion

        /// <summary>
        /// Call to api goplay
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">params are defined based on the action</param>
        /// <param name="enumAction"></param>
        /// <param name="isApi">if true: the link will be appended api/1/ and else will not</param>
        /// <returns></returns>
        public async Task<T> SendAPIRequest<T>(Dictionary<string, string> param, EGoPlayAction enumAction, bool isApi = false) where T : class, new()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(isApi ? ConfigurationManager.AppSettings["GOPLAY_SERVICE_HOST_API"] : ConfigurationManager.AppSettings["GOPLAY_SERVICE_HOST"]);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string action = string.Empty;
                    switch (enumAction)
                    {
                        case EGoPlayAction.PurchasePlayToken:
                            action = Urls.ACTION_PARTNER_PURCHASE_PLAY_TOKEN;
                            break;
                        case EGoPlayAction.InAppPurchase:
                            action = Urls.ACTION_GAME_IN_APP_PURCHASE;
                            break;
                        case EGoPlayAction.Login:
                            action = Urls.ACTION_LOGIN;
                            break;
                        case EGoPlayAction.Register:
                            action = Urls.ACTION_REGISTER;
                            break;
                        case EGoPlayAction.QueryUserId:
                            action = Urls.ACTION_QUERY_USER_ID;
                            break;
                        case EGoPlayAction.GetNotifications:
                            action = Urls.ACTION_GET_NOTIFICATIONS;
                            break;
                        case EGoPlayAction.LoginOauth:
                            action = Urls.ACTION_LOGIN_OAUTH;
                            break;
                        case EGoPlayAction.ConnectOauth:
                            action = Urls.ACTION_CONNECT_OAUTH;
                            break;
                        case EGoPlayAction.UpdateVIPStatus:
                            action = Urls.ACTION_PARTNER_UPDATE_VIP_STATUS;
                            break;
                        case EGoPlayAction.DisconnectOauth:
                            action = Urls.ACTION_DISCONNECT_OAUTH;
                            break;
                        case EGoPlayAction.CheckOauthConnection:
                            action = Urls.ACTION_CHECK_OAUTH_CONNECTION;
                            break;
                        default:
                            break;

                    }
                    var formContent = new FormUrlEncodedContent(param);
                    HttpResponseMessage response = await client.PostAsync(action, formContent);
                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
