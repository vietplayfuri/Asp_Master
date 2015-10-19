using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Net;
using System.Web.Http.Controllers;
using GoPlay.Core;
using Newtonsoft.Json;
using System.Configuration;
using Platform.Models;
using GoPlay.Web.Models;
using GoPlay.Models;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GoPlay.Web.Helpers;

namespace GoPlay.Web.Api.Filters
{
    public class ProxyPassToGtokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var goPlayapi = GoPlayApi.Instance;
            var request = HttpContext.Current.Request;

            var game = goPlayapi.GetGame(request["game_id"]).Data;

            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Form["session"];

            var user = goPlayapi.LoadFromAccessToken(session).Data;
            var gtoken_session = string.Empty;
            string service = request.Params["service"];
            string token = request.Params["token"];
            var ip_address = !string.IsNullOrEmpty(request["ip_address"])
                ? request["ip_address"]
                : request.UserHostAddress;

            string username = request["username"];
            if(!String.IsNullOrEmpty(username))
                username = Regex.Replace(username, @"\n", "");
            string password = request["password"];
            string email = request["email"];
            string nickname = request["nickname"];
            string gender = request["gender"];
            string referral_code = request["referral_code"];
            string partnerId = request["partner_id"];
            string country_code = request["country_code"];
            string country_name = request["country_name"];

            string device_id = request["device_id"];


            ErrorCodes? error = null;
            game_access_token gtoken = null;

            if (game == null)
            {
                error = ErrorCodes.INVALID_GAME_ID;
            }
            else
            {
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"];
                if (user != null)
                {
                    gtoken = goPlayapi.GetAccessToken(user.id, game.id, game.guid, null);
                    gtoken_session = gtoken.gtoken_token;
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session["gtoken_section"] = gtoken_session;
                }

                string url = request.Url.LocalPath;
                Result<GtokenAPILoginModel> result = null;
                switch (url)
                {
                    #region account
                    case "/api/1/account/login-password":
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                        {
                            enumAction = EGtokenAction.Login,
                            username = username,
                            pwd = password,
                            partnerId = partnerId,
                            game_id = game.id.ToString(),
                            device_id = device_id
                        }).Result;
                        break;

                    case "/api/1/account/register":
                        if (String.IsNullOrEmpty(nickname)) nickname = username;
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                            {
                                enumAction = EGtokenAction.Register,
                                username = username,
                                pwd = password,
                                partnerId = partnerId,
                                email = email,
                                nickname = nickname,
                                gender = gender,
                                referral_code = referral_code,
                                ip_address = ip_address,
                                country_code = country_code,
                                country_name = country_name,
                                game_id = game.id.ToString(),
                                device_id = device_id
                            }).Result;
                        break;
                    case "/api/1/account/profile":
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                            {
                                enumAction = EGtokenAction.Profile,
                                username = username,
                                partnerId = partnerId,
                                session = gtoken_session
                            }).Result;
                        break;
                    case "/api/1/account/edit-profile":
                    case "/api/1/account/profile-edit":
                        string bio = request.Params["bio"];
                        country_code = request.Params["country_code"];
                        country_name = request.Params["country_name"];
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                            {
                                enumAction = EGtokenAction.EditProfile,
                                partnerId = partnerId,
                                session = gtoken_session,
                                email = email,
                                nickname = nickname,
                                gender = gender,
                                bio = bio,
                                country_code = country_code,
                                country_name = country_name
                            }).Result;
                        break;
                    case "/api/1/account/login-oauth":
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                            {
                                enumAction = EGtokenAction.LoginOauth,
                                partnerId = partnerId,
                                token = token,
                                service = service,
                                game_id = game.id.ToString(),
                                device_id = device_id
                            }).Result;
                        break;
                    case "/api/1/account/connect-oauth":
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                            {
                                enumAction = EGtokenAction.ConnectOauth,
                                partnerId = partnerId,
                                session = gtoken_session,
                                token = token,
                                service = service
                            }).Result;
                        break;
                    case "/api/1/account/disconnect-oauth":
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                            {
                                enumAction = EGtokenAction.DisconnectOauth,
                                partnerId = partnerId,
                                session = gtoken_session,
                                service = service
                            }).Result;
                        break;
                    case "/api/1/account/query-user-id":
                        string oauth_id = request["oauth_id"];
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                            {
                                enumAction = EGtokenAction.QueryUserId,
                                partnerId = partnerId,
                                oauth_id = oauth_id
                            }).Result;
                        break;
                    case "/api/1/account/check-oauth-connection":
                        result = goPlayapi.GTokenAPIAccount(new GtokenModelAccountAction
                        {
                            enumAction = EGtokenAction.CheckOauthConnection,
                            session = gtoken_session,
                            partnerId = partnerId,
                            service = service,
                            token = token
                        }).Result;
                        break;

                    #endregion
                    default:
                        break;
                }
                int? userId = null;

                //We dont have api get notification in Gtoken
                if (result == null)
                    error = ErrorCodes.ServerError;
                else if (!result.HasData)
                    error = result.Error;

                if (error == null)
                {
                    actionContext.ActionArguments["param"] = new APILoginModel
                    {
                        profile = result.Data,
                        game = game,
                        user = user
                    };
                }

                int? gameId = null;
                if (game != null)
                    gameId = game.id;

                if (user != null)
                    userId = user.id;

                goPlayapi.LogApi("1", url, error == null,
                    request.UserAgent != null ? request.UserAgent.ToString() : string.Empty,
                    game == null ? 0 : game.id, user == null ? 0 : user.id,
                    ip_address, error.ToErrorCode(),
                    request.Params.ToString());
            }

            if (error != null)
            {
                var response = new
                {
                    success = false,
                    message = error.ToErrorMessage(),
                    error_code = error.ToErrorCode()
                };

                var resp = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json")
                };
                actionContext.Response = resp;
            }
        }
    }

    public class ProxyPassToGtokenFriendAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var goPlayapi = GoPlay.Core.GoPlayApi.Instance;

            var request = HttpContext.Current.Request;
            var game_id = request["game_id"];
            var game = goPlayapi.GetGame(game_id).Data;
            string session = !string.IsNullOrEmpty(request.Form["session"])
                ? UrlHelperExtensions.GetSession(request.Form["session"])
                : request.Form["session"];
            var goplay_session = session;
            var gtoken_session = session;
            var user = goPlayapi.LoadFromAccessToken(goplay_session).Data;
            string service = request.Params["service"];
            string token = request.Params["token"];

            var ip_address = !string.IsNullOrEmpty(request["ip_address"])
                ? request["ip_address"]
                : request.UserHostAddress;

            string username = request["username"];
            string password = request["password"];
            string email = request["email"];
            string nickname = request["nickname"];
            string gender = request["gender"];
            string referral_code = request["referral_code"];
            string partnerId = request["partner_id"];
            ErrorCodes? error = null;

            string status = request["status"];
            status = !string.IsNullOrEmpty(status) ? status.ToString() : string.Empty;

            string is_include_profile = request["include_profile"];

            string friend_username = request["friend_username"];
            friend_username = friend_username != null ? friend_username.ToString() : string.Empty;

            string keyword = request["keyword"];

            if (game == null)
            {
                error = ErrorCodes.INVALID_GAME_ID;
            }
            else
            {
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"];

                if (user != null)
                {
                    gtoken_session = goPlayapi.GetAccessToken(user.id, game.id, game.guid, null).gtoken_token;
                    if (HttpContext.Current.Session != null)
                        HttpContext.Current.Session["gtoken_section"] = gtoken_session;
                }

                string url = request.Url.LocalPath;
                Result<GtokenAPIFriend> result = null;
                switch (url)
                {
                    #region friend
                    case "/api/1/friend/friend-list":
                        is_include_profile = is_include_profile != null
                            ? is_include_profile.ToString()
                            : "false";

                        if (!ConstantValues.ListOfFriendRequestType.Contains(status))
                        {
                            status = ConstantValues.S_ACCEPTED;
                        }
                        result = goPlayapi.GTokenAPIFriend(new GtokenModelFriendAction
                        {
                            enumAction = EGtokenAction.GetFriendList,
                            partnerId = partnerId,
                            session = gtoken_session,
                            include_profile = is_include_profile,
                            status = status
                        });
                        break;

                    case "/api/1/friend/search":
                        keyword = keyword != null
                            ? keyword.ToString()
                            : string.Empty;

                        if (string.IsNullOrEmpty(keyword))
                        {
                            error = ErrorCodes.MISSING_FIELDS;
                        }
                        else
                        {
                            result = goPlayapi.GTokenAPIFriend(new GtokenModelFriendAction
                            {
                                enumAction = EGtokenAction.SearchUsers,
                                partnerId = partnerId,
                                session = gtoken_session,
                                keyword = keyword
                            });
                        }
                        break;

                    case "/api/1/friend/send-request":
                    case "/api/1/friend/respond-request":
                        if (string.IsNullOrEmpty(friend_username))
                        {
                            error = ErrorCodes.MISSING_FIELDS;
                        }
                        else if (!ConstantValues.ListOfFriendRequestType.Contains(status))
                        {
                            status = ConstantValues.S_ACCEPTED;
                        }

                        result = Result<GtokenAPIFriend>.Make(new GtokenAPIFriend { }, null);

                        break;
                    #endregion
                    default:
                        break;
                }

                if (error == null)
                {
                    if (result == null)
                        error = ErrorCodes.ServerError;
                    else if (!result.HasData)
                        error = result.Error;
                }

                if (error == null)
                {
                    result.Data.session = gtoken_session;
                    actionContext.ActionArguments["param"] = new APIFriendModel
                    {
                        result = result.Data,
                        game = game,
                        user = user
                    };

                }

                int? gameId = null;
                if (game != null)
                    gameId = game.id;

                int? userId = null;
                if (user != null)
                    userId = user.id;

                goPlayapi.LogApi("1", url, error == null,
                    request.UserAgent.ToString(),
                    game == null ? 0 : game.id, user == null ? 0 : user.id,
                    ip_address,
                    error.ToErrorCode(), request.Params.ToString());
            }

            if (error != null)
            {
                var response = new
                {
                    success = false,
                    message = error.ToErrorMessage(),
                    error_code = error.ToErrorCode()
                };

                var resp = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonConvert.SerializeObject(response), System.Text.Encoding.UTF8, "application/json")
                };
                actionContext.Response = resp;
            }
        }
    }

    public class ExecutionMeterAttribute : ActionFilterAttribute
    {
        private Stopwatch stopWatch = new Stopwatch();
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            stopWatch.Reset();
            stopWatch.Start();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            stopWatch.Stop();
            var executionTime = stopWatch.ElapsedMilliseconds;

            if (actionExecutedContext.Response != null)
            {
                var objectContent = actionExecutedContext.Response.Content as ObjectContent;
                JObject jobject = new JObject();
                if (objectContent == null)
                {
                    jobject = actionExecutedContext.Response.Content.ReadAsAsync<JObject>().Result;
                }
                else
                {
                    jobject = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectContent.Value));
                }

                jobject["execution_time"] = executionTime;
                var gameUid = HttpContext.Current.Request.Params["game_id"];
                if (!string.IsNullOrEmpty(gameUid) && ConstantValues.GAMES_IDS_FOR_REMOVE_KEY.Contains(gameUid))
                {
                    jobject = GameHelper.CustomJObject(jobject);
                }

                if (objectContent != null)
                {
                    objectContent.Value = jobject;
                    actionExecutedContext.ActionContext.Response.Content = objectContent;
                }
                else
                {
                    actionExecutedContext.ActionContext.Response.StatusCode = HttpStatusCode.OK;
                    actionExecutedContext.ActionContext.Response.Content = new StringContent(JsonConvert.SerializeObject(jobject), Encoding.UTF8, "application/json");
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}