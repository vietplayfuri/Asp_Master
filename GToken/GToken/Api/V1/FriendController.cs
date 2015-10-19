using GToken.Web.Api.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Platform.Core;
using Platform.Utility;
using Platform.Models;
using GToken.Web.Models;
using System.Text.RegularExpressions;
using GToken.WebApi;
using System.Web;
using System.Net.Http;
using System.ServiceModel.Channels;
using Newtonsoft.Json;
using GToken.Models;
using GToken.Web.Helpers.Extensions;

namespace GToken.WebApi.V1
{
    public class FriendController : ApiController
    {
        [HttpPost]
        [ActionName("search")]
        [ExecutionMeter]
        public object SearchUsers(SearchFriendAPI param)
        {
            var api = Api.Instance;
            string partnerUID = param.partner_id;
            Partner partner = api.GetPartnerById(partnerUID).Data;
            string session = param.session;
            CustomerAccount user = null;
            string keyword = param.keyword;
            int offset = param.offset ?? 0;
            int count = param.count ?? 10;
            List<Platform.Models.Profile> users = null;
            int totalcount = 0;
            ErrorCodes? error = null;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (string.IsNullOrEmpty(keyword))
                error = ErrorCodes.MISSING_FIELDS;
            else
            {
                user = api.LoadFromAccessToken(partner.identifier, session).Data;
                if (user == null)
                {
                    error = ErrorCodes.INVALID_SESSION;
                }
                else
                {
                    users = new List<Profile>();
                    var resultList = api.FindFriends(out totalcount, user.username, keyword.ToLower(), offset, count);
                    foreach (var item in resultList)
                    {
                        users.Add(api.toFriendDictionary(item, user));
                    }
                }
            }
            //log//
            api.LogApi("1", "/friend/search", error == null, 
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id,
                (user != null && user.id > 0) ? user.username : string.Empty,
                IpHelper.GetClientIp(Request), error.ToErrorCode(), JsonConvert.SerializeObject(param));

            //out put
            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                users = error == null ? users : null,
                count = totalcount
            };
        }

        [HttpPost]
        [ActionName("send-request")]
        [ExecutionMeter]
        public object SendFriendRequest(RequestAddFriendAPI param)
        {
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            string accessToken = param.session;
            CustomerAccount user = null;
            string friend_username = param.friend_username;
            ErrorCodes? error = null;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (string.IsNullOrEmpty(friend_username))
                error = ErrorCodes.MISSING_FIELDS;
            else
            {
                user = api.LoadFromAccessToken(partner.identifier, accessToken).Data;
                if (user == null)
                {
                    error = ErrorCodes.INVALID_SESSION;
                }
                else
                {
                    CustomerAccount friend = api.GetUserByUserName(friend_username).Data;
                    if (friend == null)
                    {
                        error = ErrorCodes.NON_EXISTING_USER;
                    }
                    else
                    {
                        if (!api.SendRequest(user.username, friend.username))
                        {
                            error = ErrorCodes.REQUEST_ALREADY_SENT;
                        }
                    }
                }
            }

            //log
            string errorCode = error.ToErrorCode();
            string username = user != null && user.id > 0 ? user.username : string.Empty;
            api.LogApi("1", "/friend/send-request", error == null, Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty, param.partner_id, username,
                GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request), errorCode, JsonConvert.SerializeObject(param));

            //out put
            return new
            {
                success = error == null,
                error_code = errorCode,
                message = error.ToErrorMessage()
            };
        }

        [HttpPost]
        [ActionName("friend-list")]
        [ExecutionMeter]
        public object GetFriendList([FromBody] RequestFriendListAPI param)
        {
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            CustomerAccount user = null;
            bool includeProfile = param.include_profile;
            string status = string.IsNullOrEmpty(param.status) ? Platform.Models.ConstantValues.S_ACCEPTED : param.status;
            ErrorCodes? error = null;
            dynamic friends = default(dynamic);

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else
            {
                user = api.LoadFromAccessToken(partner.identifier, param.session).Data;
                if (user == null)
                {
                    error = ErrorCodes.INVALID_SESSION;
                }
                else
                {
                    var stat = EnumConverter.EnumFromDescription<FriendStatus>(status);
                    if (includeProfile)
                    {
                        friends = api.GetFriendList(user.username, stat);
                    }
                    else
                    {
                        friends = api.GetListFriendName(user.username, stat).Data;
                    }
                }
            }


            string errorCode = error.ToErrorCode();
            string username = user != null && user.id > 0 ? user.username : string.Empty;
            api.LogApi("1", "/friend/friend-list", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, username,
                IpHelper.GetClientIp(Request),
                errorCode, JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                error_code = errorCode,
                message = error.ToErrorMessage(),
                friends = error == null ? friends : null
            };
        }

        [HttpPost]
        [ActionName("respond-request")]
        [ExecutionMeter]
        public object RespondFriendRequest([FromBody] RequestAddFriendAPI param)
        {
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            CustomerAccount user = null;
            CustomerAccount friend = null;
            string friendUsername = param.friend_username;
            string status = param.status;
            ErrorCodes? error = null;


            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (string.IsNullOrEmpty(friendUsername))
                error = ErrorCodes.MISSING_FIELDS;
            else if (!ConstantValues.ListOfValidFriendStatus.Contains(status))
                //not status in (u'accepted', u'rejected'):
                error = ErrorCodes.INVALID_FRIEND_REQUEST_STATUS;
            else
            {
                user = api.LoadFromAccessToken(partner.identifier, param.session).Data;
                if (user == null)
                {
                    error = ErrorCodes.INVALID_SESSION;
                }
                else
                {
                    friend = api.GetUserByUserName(friendUsername).Data;
                    if (friend == null)
                        error = ErrorCodes.NON_EXISTING_USER;

                    if (error == null)
                    {
                        if (!api.ResponseRequest(user.username, friend.username, status))
                        {
                            error = ErrorCodes.NON_EXISTING_FRIEND_REQUEST;
                        }
                    }
                }
            }

            //Handle result
            string errorCode = error.ToErrorCode();
            string username = user != null && user.id > 0 ? user.username : string.Empty;
            api.LogApi("1", "/friend/send-request", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, username,
                GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request),
                errorCode, JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                error_code = errorCode,
                message = error.ToErrorMessage(),
                friends = error == null
                    ? api.GetListFriendName(user.username, EnumConverter.EnumFromDescription<FriendStatus>(status)).Data
                    : null
            };
        }
    }
}