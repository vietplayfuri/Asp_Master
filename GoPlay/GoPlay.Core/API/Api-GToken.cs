using GoPlay.Models;
using GoPlay.Models.Models;
using Newtonsoft.Json;
using Platform.Models;
using Platform.Models.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public async Task<Result<GtokenAPILoginModel>> GTokenAPIAccount(GtokenModelAccountAction model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["GTOKEN_SERVICE_HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                List<KeyValuePair<string, string>> ValueCollection = new List<KeyValuePair<string, string>>();
                string action = string.Empty;
                switch (model.enumAction)
                {
                    #region Account
                    case EGtokenAction.Login:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PASSWORD, model.pwd));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_GAME_ID, model.game_id));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_DEVICE_ID, model.device_id));

                        action = Urls.ACTION_LOGIN;
                        break;
                    case EGtokenAction.Register:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PASSWORD, model.pwd));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_EMAIL, model.email));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_NICKNAME, model.nickname));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_GENDER, model.gender));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REFERRAL_CODE, model.referral_code));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_IP_ADDRESS, model.ip_address));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_COUNTRY_CODE, model.country_code));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_COUNTRY_NAME, model.country_name));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_GAME_ID, model.game_id));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_DEVICE_ID, model.device_id));
                        action = Urls.ACTION_REGISTER;
                        break;
                    case EGtokenAction.Profile:
                        model.partnerId = string.IsNullOrEmpty(model.partnerId) ? ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"] : model.partnerId;
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_USERNAME, model.username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        action = Urls.ACTION_PROFILE;
                        break;
                    case EGtokenAction.EditProfile:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_NICKNAME, model.nickname));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_EMAIL, model.email));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_GENDER, model.gender));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_BIO, model.bio));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_COUNTRY_CODE, model.country_code));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_COUNTRY_NAME, model.country_name));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_REFERRAL_CODE, model.referral_code));
                        action = Urls.ACTION_EDIT_PROFILE;
                        break;
                    case EGtokenAction.ConnectOauth:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_TOKEN, model.token));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SERVICE, model.service));
                        action = Urls.ACTION_CONNECT_OAUTH;
                        break;
                    case EGtokenAction.DisconnectOauth:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SERVICE, model.service));
                        action = Urls.ACTION_DISCONNECT_OAUTH;
                        break;
                    case EGtokenAction.QueryUserId:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_OAUTH_ID, model.oauth_id));
                        action = Urls.ACTION_QUERY_USER_ID;
                        break;
                    case EGtokenAction.GetNotifications:
                        action = Urls.ACTION_GET_NOTIFICATIONS;
                        break;
                    case EGtokenAction.LoginOauth:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_TOKEN, model.token));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SERVICE, model.service));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_GAME_ID, model.game_id));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_DEVICE_ID, model.device_id));
                        action = Urls.ACTION_LOGIN_OAUTH;
                        break;
                    case EGtokenAction.ChangePassword:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_NEW_PASSWORD, model.pwd));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_OLD_PASSWORD, model.old_pwd));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_CONFIRM_PASSWORD, model.confirm_pwd));
                        action = Urls.ACTION_CHANGE_PASSWORD;
                        break;
                    case EGtokenAction.CheckOauthConnection:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SERVICE, model.service));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_TOKEN, model.token));
                        action = Urls.ACTION_CHECK_OAUTH_CONNECTION;
                        break;
                    #endregion
                    default:
                        break;

                }
                var formContent = new FormUrlEncodedContent(ValueCollection);
                HttpResponseMessage response = client.PostAsync(action, formContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var customerLogin = JsonConvert.DeserializeObject<GtokenAPILoginModel>(await response.Content.ReadAsStringAsync());
                    if (customerLogin.success)
                    {
                        return Result<GtokenAPILoginModel>.Make(customerLogin);
                    }
                    ErrorCodes err;
                    Enum.TryParse(customerLogin.error_code, out err);
                    return Result<GtokenAPILoginModel>.Null(err);
                }
                return Result<GtokenAPILoginModel>.Null(ErrorCodes.ServerError);
            }
        }

        public Result<GtokenAPIFriend> GTokenAPIFriend(GtokenModelFriendAction model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["GTOKEN_SERVICE_HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                List<KeyValuePair<string, string>> ValueCollection = new List<KeyValuePair<string, string>>();
                string action = string.Empty;
                switch (model.enumAction)
                {
                    case EGtokenAction.GetFriendList:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_INCLUDE_PROFILE, model.include_profile));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_STATUS, model.status));
                        action = Urls.ACTION_FRIEND_FRIEND_LIST;
                        break;

                    case EGtokenAction.SearchUsers:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_KEYWORD, model.keyword));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_OFFSET, model.offset));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_COUNT, model.count));
                        action = Urls.ACTION_FRIEND_FRIEND_SEARCH;
                        break;

                    case EGtokenAction.AddFriend:
                    case EGtokenAction.SendRequest:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_FRIEND_USERNAME, model.friend_username));
                        action = Urls.ACTION_FRIEND_SEND_REQUEST;
                        break;

                    case EGtokenAction.RespondRequest:
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_SESSION, model.session));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_FRIEND_USERNAME, model.friend_username));
                        ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_STATUS, model.status));
                        action = Urls.ACTION_FRIEND_RESPONSE_REQUEST;
                        break;

                    default:
                        break;
                }
                var formContent = new FormUrlEncodedContent(ValueCollection);
                HttpResponseMessage response = client.PostAsync(action, formContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var customerLogin = JsonConvert.DeserializeObject<GtokenAPIFriend>(response.Content.ReadAsStringAsync().Result);
                    if (customerLogin.success)
                    {
                        return Result<GtokenAPIFriend>.Make(customerLogin);
                    }
                    ErrorCodes err = ErrorCodes.ServerError;
                    Enum.TryParse(customerLogin.error_code, out err);
                    return Result<GtokenAPIFriend>.Null(err);
                }
                return Result<GtokenAPIFriend>.Null(ErrorCodes.ServerError);
            }
        }

        public Result<GtokenAPITransaction> GTokenAPITransaction(GtokenModelTransactionAction model)
        {
            if (string.IsNullOrEmpty(model.hashed_token))
            {
                string input = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"] + ConfigurationManager.AppSettings["GTOKEN_PARTNER_SECRET_KEY"];
                model.hashed_token = Helper.OnewayHash(input);
            }
            if (string.IsNullOrEmpty(model.partnerId))
                model.partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["GTOKEN_SERVICE_HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                List<KeyValuePair<string, string>> ValueCollection = new List<KeyValuePair<string, string>>();
                string action = string.Empty;

                ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, model.hashed_token));
                ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, model.partnerId));
                ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_TOKEN_TRANSACTION, JsonConvert.SerializeObject(model.token_transaction)));
                ValueCollection.Add(new KeyValuePair<string, string>(ConstantValues.S_IP_ADDRESS, model.ip_address));
                action = Urls.ACTION_RECORD_TOKEN_TRANSACTION;

                var formContent = new FormUrlEncodedContent(ValueCollection);
                HttpResponseMessage response = client.PostAsync(action, formContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var customerLogin = JsonConvert.DeserializeObject<GtokenAPITransaction>(response.Content.ReadAsStringAsync().Result);
                    if (customerLogin.success)
                    {
                        return Result<GtokenAPITransaction>.Make(customerLogin);
                    }
                    ErrorCodes err = ErrorCodes.ServerError;
                    Enum.TryParse(customerLogin.error_code, out err);
                    return Result<GtokenAPITransaction>.Null(err);
                }
                return Result<GtokenAPITransaction>.Null(ErrorCodes.ServerError);
            }
        }

        /// <summary>
        /// Call Create new transaction of GToken API
        /// </summary>
        /// <param name="modal"></param>
        public Result<GTokenTransactionModel> CreateGTokenTransaction(GTokenTransaction modal)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["GTOKEN_SERVICE_HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    // TODO: FIx hard codes //
                    new KeyValuePair<string, string>( ConstantValues.S_PARTNER_ID,getPartnerId()), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, getHashedToken()),
                    new KeyValuePair<string, string>(ConstantValues.S_TRANSACTION, JsonConvert.SerializeObject(modal))
                });

                HttpResponseMessage response = client.PostAsync(Urls.ACTION_CREATE_TRANSACTION, formContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenTransactionModel>(response.Content.ReadAsStringAsync().Result);
                    if (result.success)
                    {
                        return Result<GTokenTransactionModel>.Make(result);
                    }
                    return Result<GTokenTransactionModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenTransactionModel>.Null(ErrorCodes.HttpRequestError);
            }
        }

        public Result<GTokenTransactionModel> RetrieveTransaction(string order_id = null, string gtoken_transaction_id = null)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["GTOKEN_SERVICE_HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    // TODO: FIx hard codes //
                    new KeyValuePair<string, string>( ConstantValues.S_PARTNER_ID, getPartnerId()), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN, getHashedToken()),
                    new KeyValuePair<string, string>("order_id", String.IsNullOrEmpty(order_id)? "": order_id),
                    new KeyValuePair<string, string>("gtoken_transaction_id", String.IsNullOrEmpty(gtoken_transaction_id)? "": gtoken_transaction_id)
                });

                HttpResponseMessage response = client.PostAsync(Urls.ACTION_RETRIEVE_TRANSACTION, formContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenTransactionModel>(response.Content.ReadAsStringAsync().Result);
                    if (result.success)
                    {
                        return Result<GTokenTransactionModel>.Make(result);
                    }
                    return Result<GTokenTransactionModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenTransactionModel>.Null(ErrorCodes.HttpRequestError);
            }
        }


        public async Task<Result<GTokenTransactionModel>> ExecuteGTokenTransaction(string gtoken_transaction_id, string status)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["GTOKEN_SERVICE_HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                // HTTP POST
                var formContent = new FormUrlEncodedContent(new[]
                {
                    // TODO: FIx hard codes //
                    new KeyValuePair<string, string>(ConstantValues.S_PARTNER_ID, getPartnerId()), 
                    new KeyValuePair<string, string>(ConstantValues.S_HASHED_TOKEN,getHashedToken()),
                    new KeyValuePair<string, string>(ConstantValues.S_GTOKEN_TRANSACTION_ID, gtoken_transaction_id),
                    new KeyValuePair<string, string>(ConstantValues.S_STATUS, status)
                });

                HttpResponseMessage response = await client.PostAsync(Urls.ACTION_EXECUTE_TRANSACTION, formContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<GTokenTransactionModel>(await response.Content.ReadAsStringAsync());
                    if (result.success)
                    {
                        return Result<GTokenTransactionModel>.Make(result);
                    }
                    return Result<GTokenTransactionModel>.Null(EnumEx.GetValueFromDescription<ErrorCodes>(result.error_code));
                }
                return Result<GTokenTransactionModel>.Null(ErrorCodes.HttpRequestError);
            }
        }

        public async Task<Result<GTokenTransactionModel>> updateGTokenTransactionStatus(string order_id, string status)
        {
            var result = RetrieveTransaction(order_id);
            if (result.HasData && result.Data.success)
            {
                var gtoken_transaction_id = result.Data.transaction.gtoken_transaction_id;
                if (!String.IsNullOrEmpty(gtoken_transaction_id))
                {
                    return await ExecuteGTokenTransaction(gtoken_transaction_id, status);
                }
            }
            return Result<GTokenTransactionModel>.Make(null, ErrorCodes.InvalidOrderId);
        }
        public string getPartnerId()
        {
            return ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"];
        }

        public string getHashedToken()
        {
            string input = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"] + ConfigurationManager.AppSettings["GTOKEN_PARTNER_SECRET_KEY"];
            return Helper.OnewayHash(input);
        }

    }
}

