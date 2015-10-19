using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Platform.Utility;
using Platform.Models;
using GoPlay.Web.Models;
using GoPlay.Models;
using Newtonsoft.Json;
using GoPlay.Web.Api.Filters;
using GoPlay.Core;
using System.Web;
using GoPlay.Web.Helpers.Extensions;
using GoPlay.Web.Helpers;
using System.Configuration;
using System.Threading.Tasks;

namespace GoPlay.WebApi.V1
{
    public class AccountController : ApiController
    {
        [HttpPost]
        [ActionName("register")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public async Task<object> Register(APILoginModel param)
        {
            var api = GoPlayApi.Instance;
            ErrorCodes? error = null;
            string rootUrl = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty)).AbsoluteUri;

            GtokenAPILoginModel profile = param.profile;
            // Game game = param.game;
            customer_account user = param.user;
            string gtokenSession = profile != null ? profile.session : null;

            var result = api.UpdateProfile(param.profile, param.game.id);
            if (!result.HasData)
            {
                error = result.Error;
            }
            else
            {
                user = result.Data;
                if (!string.IsNullOrEmpty(user.email))
                {
                    var template = new WelcomeMessageTemplate
                    {
                        main_index = rootUrl,
                        logoImg_src = rootUrl + ConstantValues.S_IMAGE_IN_EMAIL_LOGO_GOPLAY,
                        username = user.username,
                        login_link = Url.Link("Default", new { controller = "account", action = "login" }),
                        lastestGame_link = Url.Link("Default", new { controller = "game" }),
                        facebook_link = ConfigurationManager.AppSettings["LINK_TO_FACEBOOK_GO_PLAY"],
                        twitter_link = ConfigurationManager.AppSettings["LINK_TO_TWITTER_GO_PLAY"],
                        KennethImg_src = rootUrl + ConstantValues.S_IMAGE_IN_EMAIL_KENNETH,
                        home_link = rootUrl
                    };
                    await EmailHelper.SendMailWelcome(user.email, template);
                }
            }
            if (param.profile != null && param.game != null)
            {
                param.profile.session = api.GetAccessToken(user.id, param.game.id, param.game.guid, gtokenSession).token;
            }

            return Json(new
            {
                success = error == null,
                session = (error == null && param.profile != null) ? param.profile.session : "",
                profile = (error == null && param.profile != null) ? Profile.GetForm(user) : null
            });
        }

        [HttpPost]
        [ActionName("login-password")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object LoginPassword(APILoginModel param)
        {
            var api = GoPlayApi.Instance;
            var userData = api.UpdateProfile(param.profile);

            if (userData.HasData)
                api.UpdateCustomerAccount(userData.Data.id, DateTime.UtcNow);

            return Json(new
            {
                success = userData.HasData,
                session = !userData.HasData
                    ? ""
                    : api.GetAccessToken(userData.Data.id, param.game.id, param.game.guid, param.profile.session).token,
                profile = !userData.HasData
                    ? null
                    : Profile.GetForm(userData.Data)
            });
        }

        [HttpPost]
        [ActionName("login-oauth")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object LoginOAuth(APILoginModel param)
        {
            var api = GoPlayApi.Instance;
            var userData = api.UpdateProfile(param.profile);

            if (userData.HasData)
            {
                api.UpdateCustomerAccount(userData.Data.id,
                    param.profile.profile.country_code,
                    param.profile.profile.country_name,
                    DateTime.UtcNow);
            }

            return Json(new
            {
                success = userData.HasData,
                session = !userData.HasData
                    ? ""
                    : api.GetAccessToken(userData.Data.id, param.game.id, param.game.guid, param.profile.session).token,
                profile = !userData.HasData
                    ? null
                    : Profile.GetForm(userData.Data)
            });
        }

        [HttpPost]
        [ActionName("connect-oauth")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object ConnectOAuth(APILoginModel param)
        {
            return new
            {
                success = true,
                profile = Profile.GetForm(param.user)
            };
        }

        [HttpPost]
        [ActionName("disconnect-oauth")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object DisconnectOAuth(APILoginModel param)
        {
            return new { success = true };
        }

        [HttpPost]
        [ActionName("profile")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object RetrieveProfile(APILoginModel param)
        {
            return Json(new
            {
                success = true,
                profile = Profile.GetForm(param.user)
            });
        }

        [HttpPost]
        [ActionName("edit-profile")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object UpdateProfile(APILoginModel param)
        {
            return EditProfile(param);
        }

        private object EditProfile(APILoginModel param)
        {
            var api = GoPlayApi.Instance;
            var profile = param.profile.profile;
            if (profile != null)
            {
                api.UpdateCustomerAccount(profile.uid, profile.email, profile.nickname, profile.gender, profile.vip);

                //In python, when commit, the object will be refresh
                //Update user here instead getting it from DB                
                param.user.email = profile.email;
                param.user.nickname = profile.nickname;
                param.user.gender = profile.gender;
                param.user.vip = profile.vip;
            }

            return Json(new
            {
                success = true,
                profile = Profile.GetForm(param.user)
            });
        }

        [HttpPost]
        [ActionName("profile-edit")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object ProfileUpdate(APILoginModel param)
        {
            return EditProfile(param);
        }

        [HttpPost]
        [ActionName("query-user-id")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object QueryUserID(APILoginModel param)
        {
            string userId = string.Empty;
            if (!string.IsNullOrEmpty(param.profile.username))
            {
                var result = GoPlayApi.Instance.GetUserByUserName(param.profile.username).Data;
                userId = (result == null) ? userId : result.id.ToString();
            }
            return Json(new
            {
                success = !string.IsNullOrEmpty(userId) ? true : false,
                user_id = userId
            });
        }

        [HttpPost]
        [ActionName("get-notifications")]
        [ExecutionMeter]
        public object GetNotifications(APILoginModel param)
        {
            var api = GoPlayApi.Instance;
            List<ReponseGetNotificationModel> result = new List<ReponseGetNotificationModel>();

            var request = HttpContext.Current.Request;
            Game game = api.GetGame(request.Params["game_id"]).Data;
            customer_account user = api.LoadFromAccessToken(request.Params["session"]).Data;
            ErrorCodes? error = null;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else
            {
                var notifications = api.GetUnreadNotificationsForGame(user.id, game.id).Data;
                foreach (var noti in notifications)
                {
                    try
                    {
                        result.Add(new ReponseGetNotificationModel
                        {
                            id = noti.id,
                            created_at = noti.created_at.ToString(ConstantValues.S_DATETIME_FORMAT),
                            is_archived = noti.is_archived,
                            good_type = noti.good_type,
                            good_id = noti.good_id,
                            good_amount = noti.good_amount
                        });
                        api.UpdateUnreadNotifications(noti.id, true);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return Json(new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                notifications = result
            });
        }

        [HttpPost]
        [ActionName("check-oauth-connection")]
        [ExecutionMeter]
        [ProxyPassToGtoken]
        public object CheckOAuthConnection(APILoginModel param)
        {
            return param.profile;
        }

        //TODO: we will handle chat module with SignalR later
        [HttpPost]
        [ActionName("grant-chat-token")]
        [ExecutionMeter]
        public object GrantChatToken(APIGrantChatTokenModel param)
        {
            var api = GoPlayApi.Instance;
            var game = api.GetGame(param.game_id).Data;
            var user = api.LoadFromAccessToken(param.session).Data;
            ErrorCodes? error = null;
            string token = string.Empty;

            if (game == null)
                error = ErrorCodes.INVALID_GAME_ID;
            else if (user == null)
                error = ErrorCodes.INVALID_SESSION;
            else
                //TODO: token = chat.loginChatSystem(user.id, user.username) this is new code in 3/9
                token = api.LoginChatSystem(user.id, user.username);

            api.LogApi("1", "/account/grant-chat-token",
                error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                game == null ? 0 : game.id, user == null ? 0 : user.id,
                WebApiIpHelper.GetClientIp(Request),
                error.ToErrorCode(), JsonConvert.SerializeObject(param));

            return Json(new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                token = token
            });
        }
    }
}