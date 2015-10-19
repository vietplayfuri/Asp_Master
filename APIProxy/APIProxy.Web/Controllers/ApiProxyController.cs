using Platform.Utility;
using APIProxy.Core;
using APIProxy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Platform.Models;
using System.Reflection;
using APIProxy.Web.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Threading.Tasks;
using System.Text;

namespace APIProxy.Web.Controllers
{
    public class ApiProxyController : ApiController
    {
        /// <summary>
        /// Original api --> will redirect to another apis
        /// </summary>
        /// <returns></returns>
        [Route("api")]
        public async Task<object> OriginalAPI()
        {
            var request = HttpContext.Current.Request;
            var api = ProxyApi.Instance;
            string token = !string.IsNullOrEmpty(request.Params["token"]) ? request.Params["token"] : string.Empty;
            string action = !string.IsNullOrEmpty(request.Params["action"]) ? request.Params["action"].ToLower() : string.Empty;
            Game game = null;
            ErrorCodes? error = null;
            ResultResponse response = null;
            int gameId = 0;
            int.TryParse(request.Params["gid"], out gameId);
            int venviciGid = Convert.ToInt16(ConfigurationManager.AppSettings["VENVICI_GID"]);

            if (gameId == venviciGid)
            {
                //so confuse
                game = new Game();
                game.id = venviciGid;
                game.guid = ConfigurationManager.AppSettings["VENVICI_GUID"];
            }
            else
                game = api.GetGame(gameId).Data;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (ConstantValues.LIST_PROXY_ACTION.ContainsKey(action))
            {
                MethodInfo theMethod = this.GetType().GetMethod(ConstantValues.LIST_PROXY_ACTION[action]);
                var apiResponse = await (Task<object>)theMethod.Invoke(this, new object[] { game, token, request });
                response = apiResponse.CastObject<ResultResponse>();
            }
            else
                error = ErrorCodes.NON_EXISTING_API;

            if (error != null || response == null)
            {
                response = new ResultResponse
                {
                    success = false,
                    message = (error != null) ? error.ToErrorMessage() : ErrorCodes.ServerError.ToErrorMessage()
                };
            }

            //Not implemented yet in python
            //api.LogApi();

            if (response.profile != null && !string.IsNullOrEmpty(response.session))
            {
                var userCredentialObj = await api.GetUserCredential(response.profile.uid, game.id);
                UserCredential userCredential = userCredentialObj.Data;
                if (userCredential == null)
                {
                    userCredential = new UserCredential
                    {
                        user_id = response.profile.uid,
                        game_id = game.id,
                        username = response.profile.account,
                        session = response.session
                    };
                    await api.CreateUserCredential(userCredential);
                }
                else
                {
                    userCredential.username = response.profile.account;
                    userCredential.session = response.session;
                    await api.UpdateUserCredential(userCredential);
                }
            }

            ResultResponse result = ResponseConverter(response);
            return result;
        }
        private ResultResponse ResponseConverter(ResultResponse response)
        {
            if (response.success)
            {
                response.status = "Success";
                response.Error = "0";

                if (response.profile != null)
                {
                    response.profile = Helper.FullFillEmptyFields<Profile>(response.profile);
                    var profileList = new List<Profile>();
                    profileList.Add(response.profile);
                    response.UserData = profileList;
                }

                if (response.notifications != null && response.notifications.Any())
                {
                    response.list = new List<ResultNotificationModel>();
                    foreach (var note in response.notifications)
                    {
                        ReponseGetNotificationModel noti = JsonHelper.DeserializeObject<ReponseGetNotificationModel>(note.ToString());
                        if (noti != null)
                            response.list.Add(new ResultNotificationModel
                            {
                                id = noti.id,
                                addTime = noti.created_at,
                                state = noti.is_archived ? 1 : 0,
                                serverID = 1,
                                goodType = noti.good_type,
                                goodID = noti.good_id,
                                goodAmount = noti.good_amount
                            });
                    }
                }
            }
            else
            {
                response.status = "fail";
                response.Error = !string.IsNullOrEmpty(response.message)
                    ? response.message
                    : ErrorCodes.SYSTEM_ERROR.ToErrorMessage();
            }
            return response;
        }

        public async Task<object> PurchasePlayToken(Game game, string token, HttpRequest request)
        {
            string username = request.Params["username"];
            string amount = request.Params["amount"];

            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append(request.Params["action"]);
            tokenBuilder.Append(amount);
            tokenBuilder.Append(username);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["tstamp"]);

            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());
            if (string.Compare(token, generatedToken, true) == 0)
            {
                string tokenRawString = username + amount + ConfigurationManager.AppSettings["PARTNER_CLIENT_ID"];
                string newToken = Helper.CalculateMD5Hash(tokenRawString);
                var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>() {
                    { ConstantValues.S_IP_ADDRESS, request.UserHostAddress},
                    { ConstantValues.S_USERNAME, username},
                    { ConstantValues.S_TOKEN, newToken},
                    { ConstantValues.S_AMOUNT, amount},
                }, EGoPlayAction.PurchasePlayToken);

                return result;
            }
            else
                return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorCode());
        }

        public async Task<object> GetUserNotification(Game game, string token, HttpRequest request)
        {
            string userID = !string.IsNullOrEmpty(request.Params["myuserid"])
                ? request.Params["myuserid"]
                : !string.IsNullOrEmpty(request.Params["MyUserID"])
                    ? request.Params["MyUserID"]
                    : string.Empty;
            string message = string.Empty;
            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append(request.Params["action"]);
            tokenBuilder.Append(userID);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["tstamp"]);
            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());

            if (string.Compare(token, generatedToken, true) != 0)
                message = ErrorCodes.INVALID_TOKEN.ToErrorCode();
            else
            {
                int userId = 0;
                int.TryParse(userID, out userId);
                var userCredentialObj = await ProxyApi.Instance.GetUserCredential(userId, game.id);
                UserCredential userCredential = userCredentialObj.Data;
                if (userCredential == null)
                    message = "User Session was not saved in previous steps";
                else
                {
                    var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>() {
                    { ConstantValues.S_IP_ADDRESS, request.UserHostAddress},
                    { ConstantValues.S_SESSION, userCredential.session },
                    { ConstantValues.S_GAME_ID, game.guid }},
                    EGoPlayAction.GetNotifications, true);

                    return result;
                }
            }
            return FailResult(message);
        }

        public async Task<object> InAppPurchase(Game game, string token, HttpRequest request)
        {
            string userID = request.Params["myuserid"];
            string amount = request.Params["amount"];
            string gameCurrencyID = request.Params["goodsid"];
            string orderID = request.Params["orderid"];
            string message = string.Empty;

            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append(userID);
            tokenBuilder.Append(amount);
            tokenBuilder.Append(gameCurrencyID);
            tokenBuilder.Append(orderID);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["action"]);
            tokenBuilder.Append(request.Params["tstamp"]);

            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());
            if (token != generatedToken)
                message = ErrorCodes.INVALID_TOKEN.ToErrorCode();
            else
            {
                int userId = 0;
                int.TryParse(userID, out userId);
                var userCredentialObj = await ProxyApi.Instance.GetUserCredential(userId, game.id);
                UserCredential userCredential = userCredentialObj.Data;
                if (userCredential == null)
                    message = "User Session was not saved in previous steps";
                else
                {
                    int gameCurrencyId = 0;
                    int.TryParse(gameCurrencyID, out gameCurrencyId);
                    var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>() {
                        { ConstantValues.S_IP_ADDRESS, request.UserHostAddress},
                        { ConstantValues.S_SESSION, userCredential.session},
                        { ConstantValues.S_GAME_ID, game.guid},
                        { ConstantValues.S_ORDER_ID, orderID},
                        { ConstantValues.S_AMOUNT, amount},
                        { ConstantValues.S_EXCHANGE_OPTION_TYPE, ConstantValues.S_PACKAGE},
                        { ConstantValues.S_EXCHANGE_OPTION_ID, gameCurrencyId.ToString()}},
                        EGoPlayAction.InAppPurchase);

                    return result;
                }
            }
            return FailResult(message);
        }

        public async Task<object> Login(Game game, string token, HttpRequest request)
        {
            string username = request.Params["username"];
            string password = request.Params["password"];
            string tokenRawString = username + password + "login" + game.guid + request.Params["tstamp"];
            string generatedToken = Helper.CalculateMD5Hash(tokenRawString);
            if (string.Compare(token, generatedToken, true) == 0)
            {
                var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>()
                {   {ConstantValues.S_USERNAME, username},
                    {ConstantValues.S_PASSWORD, password},
                    {ConstantValues.S_GAME_ID, game.guid},
                    {ConstantValues.S_IP_ADDRESS, request.UserHostAddress}
                }, EGoPlayAction.Login, true);
                return result;
            }
            return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorMessage());
        }

        public async Task<object> Signin(Game game, string token, HttpRequest request)
        {
            string username = request.Params["username"];
            string nickname = !string.IsNullOrEmpty(request.Params["nickname"])
                ? request.Params["nickname"] : request.Params["Nickname"];

            string password = request.Params["password"];
            Genders gender = Genders.Male;
            int sexindex;
            if (int.TryParse(request.Params["sex"], out sexindex) && Enum.IsDefined(typeof(Genders), sexindex))
                gender = (Genders)sexindex;

            string email = !string.IsNullOrEmpty(request.Params["email"])
                ? request.Params["email"] : request.Params["Email"];

            string referralCode = request.Params["recommend"];
            string tokenRawString = username + password + "signin" + game.guid + request.Params["tstamp"];
            string generatedToken = Helper.CalculateMD5Hash(tokenRawString);
            if (string.Compare(token, generatedToken, true) == 0)
            {
                var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>()
                {
                    {ConstantValues.S_USERNAME, username},
                    {ConstantValues.S_PASSWORD, password},
                    {ConstantValues.S_EMAIL, email},
                    {ConstantValues.S_NICKNAME, nickname},
                    {ConstantValues.S_GENDER, Helper.GetDescription(gender)},
                    {ConstantValues.S_REFERRAL_CODE, referralCode},
                    {ConstantValues.S_GAME_ID, game.guid},
                    {ConstantValues.S_IP_ADDRESS, request.UserHostAddress}
                }, EGoPlayAction.Register, true);
                return result;
            }
            return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorMessage());
        }

        public async Task<object> QueryUserID(Game game, string token, HttpRequest request)
        {
            string oauthIDList = request.Params["onlyIDlist"];
            string tokenRawString = game.id.ToString() + request.Params["action"] + oauthIDList + game.guid + request.Params["tstamp"];
            string generatedToken = Helper.CalculateMD5Hash(tokenRawString);
            if (string.Compare(token, generatedToken, true) != 0)
                return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorMessage());

            var param = new Dictionary<string, string>()
            {
                {ConstantValues.S_GAME_ID, game.guid},
                {ConstantValues.S_IP_ADDRESS, request.UserHostAddress},
                {ConstantValues.S_OAUTH_ID, string.Empty},
            };
            List<BlobModel> responseBlob = new List<BlobModel>();
            var api = GoplayService.Instance;
            foreach (var oauthID in oauthIDList.Split(','))
            {
                param[ConstantValues.S_OAUTH_ID] = oauthID;
                var result = await api.SendAPIRequest<ResultResponse>(param, EGoPlayAction.QueryUserId, true);
                if (result != null && result.success)
                {
                    BlobModel blob = new BlobModel()
                    {
                        uid = result.user_id,
                        otherID = oauthID
                    };
                    responseBlob.Add(blob);
                }
            }
            return new
            {
                success = true,
                list = responseBlob
            };
        }

        public async Task<object> ThirdPartyLogin(Game game, string token, HttpRequest request)
        {
            string accessToken = request.Params["access_token"];
            string service = GetService(request.Params["ttype"]);

            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append(request.Params["action"]);
            tokenBuilder.Append(accessToken);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["tstamp"]);

            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());
            if (string.Compare(token, generatedToken, true) == 0)
            {
                var api = ProxyApi.Instance;
                string oauthAccountID = api.IDRetriever(service, accessToken);
                var oauthAccessToken = (await api.GetOauthAccessToken(service, oauthAccountID, game.id)).Data;
                if (oauthAccessToken == null)
                {
                    oauthAccessToken = new OauthAccessToken()
                    {
                        service = service,
                        identity = oauthAccountID,
                        game_id = game.id,
                        access_token = accessToken
                    };
                    await api.CreateOauthAccessToken(oauthAccessToken);
                }
                else
                {
                    await api.UpdateOauthAccessToken(oauthAccessToken.id, accessToken);
                }
                var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>() {
                    { ConstantValues.S_IP_ADDRESS, request.UserHostAddress},
                    { ConstantValues.S_SERVICE, service},
                    { ConstantValues.S_TOKEN, accessToken},
                    { ConstantValues.S_GAME_ID, game.guid},
                }, EGoPlayAction.LoginOauth, true);

                if (result != null && result.error_code == ErrorCodes.NON_EXISTING_OAUTH.ToErrorCode())
                    result.onlyID = oauthAccountID;
                return result;
            }
            return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorCode());
        }

        public async Task<object> ThirdPartyRegister(Game game, string token, HttpRequest request)
        {
            string service = GetService(request.Params["ttype"]);

            string oauthAccountID = !string.IsNullOrEmpty(request.Params["onlyid"])
                ? request.Params["onlyid"] : request.Params["onlyID"];
            string username = request.Params["username"];
            string password = request.Params["password"];

            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append(request.Params["action"]);
            tokenBuilder.Append(username);
            tokenBuilder.Append(oauthAccountID);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["tstamp"]);

            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());
            if (string.Compare(token, generatedToken, true) != 0)
                return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorCode());

            ResultResponse result;
            var api = ProxyApi.Instance;
            if (!string.IsNullOrEmpty(password))
            {
                result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>()
                {   {ConstantValues.S_USERNAME, username},
                    {ConstantValues.S_PASSWORD, password},
                    {ConstantValues.S_GAME_ID, game.guid},
                    {ConstantValues.S_IP_ADDRESS, request.UserHostAddress}
                }, EGoPlayAction.Login, true);
            }
            else
            {
                password = Guid.NewGuid().ToString("d").ToUpper().Substring(0, 6);
                string nickname = !string.IsNullOrEmpty(request.Params["nickname"])
                    ? request.Params["nickname"] : request.Params["Nickname"];

                string email = request.Params["email"];
                string referralCode = request.Params["recommend"];

                result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>()
                {
                    {ConstantValues.S_USERNAME, username},
                    {ConstantValues.S_PASSWORD, password},
                    {ConstantValues.S_EMAIL, email},
                    {ConstantValues.S_NICKNAME, email},//don't know why?
                    {ConstantValues.S_REFERRAL_CODE, referralCode},
                    {ConstantValues.S_GAME_ID, game.guid},
                    {ConstantValues.S_IP_ADDRESS, request.UserHostAddress}
                }, EGoPlayAction.Register, true);

                if (result != null && result.profile != null && !string.IsNullOrEmpty(result.session))
                {
                    UserCredential userCredential = (await api.GetUserCredential(result.profile.uid, game.id)).Data;
                    if (userCredential == null)
                    {
                        userCredential = new UserCredential()
                        {
                            user_id = result.profile.uid,
                            game_id = game.id,
                            username = result.profile.account,
                            session = result.session,
                        };
                        await api.CreateUserCredential(userCredential);
                    }
                    else
                    {
                        userCredential.username = result.profile.account;
                        userCredential.session = result.session;
                        await api.UpdateUserCredential(userCredential);
                    }
                }
            }
            if (result == null || !result.success)
                return FailResult(ErrorCodes.INVALID_SESSION.ToErrorCode());

            string session = result.session;
            var oauthAccessToken = (await api.GetOauthAccessToken(service, oauthAccountID, game.id)).Data;
            if (oauthAccessToken == null)
                return FailResult("OAuth Account access token was not saved in previous steps");

            result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>()
                {
                    {ConstantValues.S_SESSION, session},
                    {ConstantValues.S_SERVICE, service},
                    {ConstantValues.S_TOKEN, oauthAccessToken.access_token},
                    {ConstantValues.S_GAME_ID, game.guid},
                    {ConstantValues.S_IP_ADDRESS, request.UserHostAddress}
                }, EGoPlayAction.ConnectOauth, true);
            return result;
        }
        public async Task<object> UpdateVIPStatus(Game game, string token, HttpRequest request)
        {
            string vipStatus = request.Params["ttype"];
            string username = request.Params["username"];
            string promotionRatio = !string.IsNullOrEmpty(request.Params["promotion_ratio"])
                ? request.Params["promotion_ratio"]
                : ConfigurationManager.AppSettings["DEFAULT_PROMOTION_RATIO"];
            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append(request.Params["action"]);
            tokenBuilder.Append(vipStatus);
            tokenBuilder.Append(username);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["tstamp"]);
            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());

            if (string.Compare(token, generatedToken, true) == 0)
            {
                string tokenRawString = username + vipStatus + ConfigurationManager.AppSettings["PARTNER_CLIENT_ID"];
                string newToken = Helper.CalculateMD5Hash(tokenRawString);
                var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>() {
                    { ConstantValues.S_IP_ADDRESS, request.UserHostAddress},
                    { ConstantValues.S_USERNAME, username},
                    { ConstantValues.S_STATUS, vipStatus },
                    { ConstantValues.S_TOKEN, newToken},                    
                    { ConstantValues.S_PROMOTION_RATIO, promotionRatio }},
                EGoPlayAction.UpdateVIPStatus);

                return result;
            }
            return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorCode());
        }

        public async Task<object> UnbindThirdPartyAccount(Game game, string token, HttpRequest request)
        {
            string service = GetService(request.Params["ttype"]);

            string oauthAccountID = !string.IsNullOrEmpty(request.Params["onlyid"])
                ? request.Params["onlyid"] : request.Params["onlyID"];
            int userID;
            int.TryParse(request.Params["myuserid"], out userID);
            string password = request.Params["password"];

            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append(request.Params["action"]);
            tokenBuilder.Append(userID);
            tokenBuilder.Append(oauthAccountID);
            tokenBuilder.Append(password);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["tstamp"]);

            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());
            if (string.Compare(token, generatedToken, true) != 0)
                return FailResult(ErrorCodes.INVALID_TOKEN.ToErrorCode());

            var api = ProxyApi.Instance;
            var userCredentialObj = await api.GetUserCredential(userID, game.id);
            UserCredential userCredential = userCredentialObj.Data;

            if (userCredential == null)
                return FailResult("User Session was not saved in previous steps");

            var oauthAccessToken = (await api.GetOauthAccessToken(service, oauthAccountID, game.id)).Data;
            if (oauthAccessToken == null)
                return FailResult("OAuth Account access token was not saved in previous steps");

            ResultResponse result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>()
                {   {ConstantValues.S_SESSION, userCredential.session},
                    {ConstantValues.S_SERVICE, service},
                    {ConstantValues.S_TOKEN, oauthAccessToken.access_token},
                                        {ConstantValues.S_GAME_ID, game.guid},
                    {ConstantValues.S_IP_ADDRESS, request.UserHostAddress}
                }, EGoPlayAction.DisconnectOauth, true);
            if (result != null && result.error_code == ErrorCodes.INVALID_SESSION.ToErrorCode())
            {
                result.message = ErrorCodes.OAUTH_DISCONNECT_INVALID_SESSION.ToErrorMessage();
                result.error_code = ErrorCodes.OAUTH_DISCONNECT_INVALID_SESSION.ToErrorCode();
            }
            return result;
        }


        public async Task<object> CheckOAuthConnection(Game game, string token, HttpRequest request)
        {
            string userID = !string.IsNullOrEmpty(request.Params["myuserid"]) ? request.Params["myuserid"] : string.Empty;
            string message = ErrorCodes.INVALID_TOKEN.ToErrorCode();
            string service = GetService(request.Params["ttype"]);
            string oauthAccountID = request.Params["onlyid"];

            StringBuilder tokenBuilder = new StringBuilder();
            tokenBuilder.Append(game.id.ToString());
            tokenBuilder.Append("searchonlyidbind");
            tokenBuilder.Append(userID);
            tokenBuilder.Append(oauthAccountID);
            tokenBuilder.Append(game.guid);
            tokenBuilder.Append(request.Params["tstamp"]);
            string generatedToken = Helper.CalculateMD5Hash(tokenBuilder.ToString());

            if (string.Compare(token, generatedToken, true) == 0)
            {
                var api = ProxyApi.Instance;
                int userId = 0;
                int.TryParse(userID, out userId);
                UserCredential userCredential = (await api.GetUserCredential(userId, game.id)).Data;
                if (userCredential == null)
                    message = "User Session was not saved in previous steps";
                else
                {
                    var oauthAccessToken = (await api.GetOauthAccessToken(service, oauthAccountID, game.id)).Data;
                    if (oauthAccessToken == null)
                        message = "OAuth Account access token was not saved in previous steps";
                    else
                    {
                        var result = await GoplayService.Instance.SendAPIRequest<ResultResponse>(new Dictionary<string, string>() {
                        { ConstantValues.S_IP_ADDRESS, request.UserHostAddress},
                        { ConstantValues.S_SESSION, userCredential.session},
                        { ConstantValues.S_GAME_ID, game.guid},
                        { ConstantValues.S_SERVICE, service },
                        { ConstantValues.S_TOKEN, oauthAccessToken.access_token}},
                        EGoPlayAction.CheckOauthConnection, true);

                        if (string.Compare(result.message, ErrorCodes.NON_EXISTING_OAUTH.ToErrorMessage(), true) == 0
                         || string.Compare(result.message, ErrorCodes.OAUTH_ALREADY_CONNECTED.ToErrorMessage(), true) == 0)
                        {
                            result.error_code = ErrorCodes.OAUTH_ALREADY_CONNECTED.ToErrorCode();
                            result.message = ErrorCodes.OAUTH_ALREADY_CONNECTED.ToErrorMessage();
                        }

                        return result;
                    }
                }
            }
            return FailResult(message);
        }

        private string GetService(string param)
        {
            EThirdPartyService eService = EThirdPartyService.Facebook;
            int serviceIndex;
            if (int.TryParse(param, out serviceIndex) && Enum.IsDefined(typeof(EThirdPartyService), serviceIndex))
                eService = (EThirdPartyService)serviceIndex;
            return Helper.GetDescription(eService);
        }

        private object FailResult(string message)
        {
            return new
            {
                status = false,
                success = false,
                message = message
            };
        }
    }
}
