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
using System.Configuration;

namespace GoPlay.WebApi.V1
{
    public class FriendController : ApiController
    {
        [HttpPost]
        [ActionName("friend-list")]
        [ExecutionMeter]
        [ProxyPassToGtokenFriend]
        public object GetFriendList(APIFriendModel param)
        {
            //TODO: need to apply new code 3/9
            //bool isInclude;
            //if (bool.TryParse(HttpContext.Current.Request.Params["include_profile"], out isInclude))
            //{
            //    List<FriendDto> friends = JsonHelper.DeserializeObject<List<FriendDto>>(param.result.friends.ToString());
            //    if (friends != null && friends.Any())
            //    {
            //        var api = GoPlayApi.Instance;
            //        foreach (var item in friends)
            //        {
            //            var user = api.GetUserByUserName(item.username).Data;
            //            if (user != null)
            //            {
            //                item.profile.avatar = user.GetValidAvatarUrl();
            //                item.profile.cover = user.GetValidCoverUrl();
            //            }
            //        }
            //    }
            //    param.result.friends = friends;
            //}

            return param.result;
        }

        [HttpPost]
        [ActionName("search")]
        [ExecutionMeter]
        [ProxyPassToGtokenFriend]
        public object SearchUsers(APIFriendModel param)
        {
            return param.result;
        }

        [HttpPost]
        [ActionName("send-request")]
        [ExecutionMeter]
        [ProxyPassToGtokenFriend]
        public object SendRequest(APIFriendModel param)
        {
            //TODO: need to apply new code 3/9
            var friendUsername = HttpContext.Current.Request.Params["friend_username"];
            var api = GoPlayApi.Instance;
            var friend = api.GetUserByUserName(friendUsername).Data;
            if (friend == null)
            {
                //# Shouldn't happen, but I am paranoid
                var gtokenProfile = api.GTokenAPIAccount(new GtokenModelAccountAction
                {
                    enumAction = EGtokenAction.Profile,
                    username = friendUsername,
                    partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]
                }).Result.Data;

                if (gtokenProfile != null)
                    friend = api.UpdateProfile(gtokenProfile).Data;
            }

            if (friend != null)
                param.result.success = api.SendRequest(param.result.session, param.user.id, friend.id);

            return param.result;
        }


        [HttpPost]
        [ActionName("respond-request")]
        [ExecutionMeter]
        [ProxyPassToGtokenFriend]
        public object RejectRequest(APIFriendModel param)
        {
            //TODO: need to apply new code 3/9
            var friendUsername = HttpContext.Current.Request.Params["friend_username"];
            var status = HttpContext.Current.Request.Params["status"];

            var api = GoPlayApi.Instance;
            var friend = api.GetUserByUserName(friendUsername).Data;
            if (friend == null)
            {
                //# Shouldn't happen, but I am paranoid
                var gtokenProfile = api.GTokenAPIAccount(new GtokenModelAccountAction
                {
                    enumAction = EGtokenAction.Profile,
                    username = friendUsername,
                    partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]
                }).Result.Data;
                if (gtokenProfile != null)
                    friend = api.UpdateProfile(gtokenProfile).Data;
            }

            if (friend != null)
            {
                var response = api.RespondRequest(param.result.session, param.user.id, friend.id, status);
                if (response.HasData)
                    param.result = response.Data;
            }

            return param.result;
        }

    }
}