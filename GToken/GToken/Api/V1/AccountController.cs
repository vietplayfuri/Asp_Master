using System;
using System.Net;
using System.Web.Http;
using Platform.Core;
using Platform.Utility;
using Platform.Models;
using GToken.Web.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using GToken.Web.Helpers.Extensions;
using GToken.Web.Api.Filters;


namespace GToken.WebApi.V1
{
    [RoutePrefix("account")]
    public class AccountController : ApiController
    {
        [HttpPost]
        [ActionName("register")]
        [ExecutionMeter]
        public object Register([FromBody] RegisterModel param)
        {
            var api = Api.Instance;
            string username = !String.IsNullOrEmpty(param.username) ? param.username.Trim().ToLower() : String.Empty;
            string password = param.password;
            var partner = api.GetPartnerById(param.partner_id).Data; // TODO: Optimization. Should be delay called //
            CustomerAccount user = null;
            string referralCode = param.referral_code;
            ErrorCodes? error = null;
            Regex regex = new Regex(@"^[a-zA-Z0-9]+[\w\-\.]*$");

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                error = ErrorCodes.MISSING_FIELDS;
            else if (username.Length < 3 || username.Length > 20)
                error = ErrorCodes.USERNAME_LENGTH;
            else if (password.Length < 3)
                error = ErrorCodes.PASSWORD_LENGTH;
            else if (!regex.Match(username).Success)
                error = ErrorCodes.INVALID_USERNAME;
            else if (!string.IsNullOrEmpty(referralCode) && api.GetUserByUserName(referralCode.Trim().ToLower()).Data == null)
                error = ErrorCodes.NON_EXISTING_REFERRAL_CODE;
            else if (!string.IsNullOrEmpty(param.email) && !Helper.IsEmailValid(param.email))
                error = ErrorCodes.INVALID_EMAIL;
            else if (!string.IsNullOrEmpty(param.country_code)
                && !string.IsNullOrEmpty(param.country_name)
                && Helper.FindCountryName(param.country_code) != param.country_name)
                error = ErrorCodes.INVALID_COUNTRY;
            else if ((!string.IsNullOrEmpty(param.country_code) && string.IsNullOrEmpty(param.country_name))
                || (string.IsNullOrEmpty(param.country_code) && !string.IsNullOrEmpty(param.country_name)))
                error = ErrorCodes.MISSING_FIELDS;

            if (error == null)
            {
                user = api.GetUserByUserName(username).Data;
                if (user == null)
                {
                    Genders? defaultGender = EnumConverter.EnumFromDescription<Genders>(param.gender);
                    var result = api.Register(username, password, partner, param.nickname, param.email,
                        defaultGender ?? Genders.Male,
                        string.IsNullOrEmpty(referralCode) ? string.Empty : referralCode.ToLower(), param.ip_address, param.country_code, param.country_name,
                        param.game_id, param.device_id);

                    if (result.Succeeded)
                    {
                        user = result.Data;

                        ErrorCodes? errorInReferralCampaign = api.HandleDataReferralCampaign(user.id, user.username, param.game_id, param.device_id, user.inviter_username, param.ip_address, partner.identifier);
                        if (errorInReferralCampaign != null)
                            api.LogApi("1", "/account/register", true,
                                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                                param.partner_id, user == null ? string.Empty : user.username,
                                IpHelper.GetClientIp(Request), errorInReferralCampaign.ToErrorCode(), errorInReferralCampaign.ToErrorMessage());
                    }
                    else
                        error = result.Error;
                }
                else
                {
                    error = ErrorCodes.EXISTING_USERNAME_EMAIL;
                }
            }

            string errorCode = error.ToErrorCode();
            api.LogApi("1", "/account/register", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, username,
                GToken.Web.Helpers.Extensions.IpHelper.GetClientIp(Request),
                errorCode, JsonConvert.SerializeObject(param));

            //result
            return new
            {
                success = error == null,
                error_code = errorCode,
                message = error.ToErrorMessage(),
                session = (error == null && user != null) ? api.GetAccessToken(user.id, user.username, partner) : string.Empty,
                profile = (error == null) ? Platform.Models.Profile.GetFrom(user) : null
            };
        }

        [HttpPost]
        [ActionName("login-oauth")]
        [ExecutionMeter]
        public object LoginOAuth([FromBody] OauthModel param)
        {
            //prepare data
            var api = Api.Instance;
            string partnerUID = param.partner_id;
            Partner partner = api.GetPartnerById(partnerUID).Data;
            string service = !string.IsNullOrEmpty(param.service) ? param.service.ToLower() : string.Empty;
            //This token is returned from login facebook
            string token = param.token;
            string oauthAccountID = string.Empty;
            FacebookProfile fbProfile = api.IDRetriever(service, token);
            oauthAccountID = fbProfile == null
                ? token
                : fbProfile.id;
            string ip_address = !string.IsNullOrEmpty(param.ip_address)
                ? param.ip_address
                : IpHelper.GetClientIp(Request);

            ErrorCodes? message = null;
            CustomerAccount user = null;
            CustomerLoginOAuth userOAuth = null;

            if (partner == null)
                message = ErrorCodes.INVALID_PARTNER_ID;
            else if (string.IsNullOrEmpty(service) && string.IsNullOrEmpty(token))
                message = ErrorCodes.MISSING_FIELDS;
            else
            {
                userOAuth = api.GetCustomerLoginOAuthByIdentity(service, oauthAccountID).Data;
                if (userOAuth == null)
                {
                    if (fbProfile != null && !string.IsNullOrEmpty(fbProfile.email))
                    {
                        var customerAccount = api.GetUserByEmail(fbProfile.email).Data;
                        if (customerAccount != null)
                        {
                            int newId = api.CreateLoginOAuth(new CustomerLoginOAuth
                            {
                                customer_username = customerAccount.username,
                                service = service,
                                identity = fbProfile.id
                            });
                            userOAuth = new CustomerLoginOAuth()
                            {
                                customer_username = customerAccount.username
                            };
                        }
                        else
                        {
                            message = ErrorCodes.NON_EXISTING_OAUTH;
                        }
                    }
                    else
                    {
                        message = ErrorCodes.NON_EXISTING_OAUTH;
                    }
                }
                else
                {
                    user = api.GetUserByUserName(userOAuth.customer_username).Data;
                    if (user != null && (string.IsNullOrEmpty(user.country_code)))
                    {
                        IPAddress ip = IPAddress.Parse(ip_address);
                        ip.GetCountryCode(c => user.country_code = c, n => user.country_name = n);
                        api.UpdateCustomerAccount(user.id, user.country_code, user.country_name, DateTime.UtcNow);
                    }
                }
            }

            //Return result
            api.LogApi("1", "/account/login-oauth", message == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, user == null ? string.Empty : user.username,
                IpHelper.GetClientIp(Request), message.ToErrorCode(), JsonConvert.SerializeObject(param));

            if (message == null)
            {
                user = api.GetUserByUserName(userOAuth.customer_username).Data;
                api.UpdateCustomerAccount(user.id, user.country_code, user.country_name, DateTime.UtcNow);

                ErrorCodes? errorInReferralCampaign = api.HandleDataReferralCampaign(user.id, user.username, param.game_id, param.device_id, user.inviter_username, ip_address, partner.identifier);
                if (errorInReferralCampaign != null)
                    api.LogApi("1", "/account/login-oauth", true,
                        Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                        param.partner_id, user == null ? string.Empty : user.username,
                        IpHelper.GetClientIp(Request), errorInReferralCampaign.ToErrorCode(), errorInReferralCampaign.ToErrorMessage());
            }

            return new
            {
                success = message == null,
                message = message.ToErrorMessage(),
                error_code = message.ToErrorCode(),
                session = message == null ? api.GetAccessToken(user.id, user.username, partner) : string.Empty,
                profile = message == null ? Profile.GetFrom(user) : null
            };
        }

        [HttpPost]
        [ActionName("profile")]
        [ExecutionMeter]
        public object RetrieveProfile([FromBody] RetrieveProfileModel param)
        {
            var api = Api.Instance;
            CustomerAccount user = null;
            ErrorCodes? error = null;
            CustomerAccount guest = null;
            Profile profile = null;

            Partner partner = api.GetPartnerById(param.partner_id).Data;
            string accessToken = param.session;
            string username = param.username;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    user = api.LoadFromAccessToken(partner.identifier, accessToken).Data;
                }

                if (string.IsNullOrEmpty(username) && user == null)
                {
                    error = ErrorCodes.MISSING_FIELDS;
                }
                if (!string.IsNullOrEmpty(username))
                {
                    var result = api.GetUserByUserName(username);
                    if (result.HasData)
                    {
                        guest = result.Data;
                    }
                    else
                    {
                        error = ErrorCodes.NON_EXISTING_USER;
                    }
                }

                if (guest != null)
                {
                    profile = api.toFriendDictionary(guest, user);
                    if (user != null)
                    {
                        if (user.username == guest.username)
                        {
                            profile = Profile.GetFrom(user);
                        }
                    }
                }
                else if (user != null)
                    profile = Profile.GetFrom(user);
                else
                    error = ErrorCodes.NON_EXISTING_USER;
            }

            // Log //
            string errorCode = error.ToErrorCode();
            api.LogApi("1", "/account/profile", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id, param.username, IpHelper.GetClientIp(Request), errorCode, JsonConvert.SerializeObject(param));

            //output
            return new
            {
                success = error == null,
                profile = error == null ? profile : null,
                message = error.ToErrorMessage(),
                error_code = errorCode
            };
        }

        [HttpPost]
        [ActionName("edit-profile")]
        [ExecutionMeter]
        public object UpdateProfile([FromBody] ProfileModel param)
        {
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            string accessToken = param.session;
            ErrorCodes? error = null;
            CustomerAccount user = null;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else
            {
                user = api.LoadFromAccessToken(partner.identifier, accessToken).Data;
                if (user == null)
                    error = ErrorCodes.INVALID_SESSION;
                else
                {
                    string email = !string.IsNullOrEmpty(param.email) ? param.email : user.email;
                    string nickname = !string.IsNullOrEmpty(param.nickname) ? param.nickname : user.nickname;
                    string gender = !string.IsNullOrEmpty(param.gender) ? param.gender : user.gender;
                    string bio = !string.IsNullOrEmpty(param.bio) ? param.bio : user.bio;
                    string countryCode = !string.IsNullOrEmpty(param.country_code) ? param.country_code : user.country_code;
                    string countryName = !string.IsNullOrEmpty(param.country_name) ? param.country_name : user.country_name;

                    if (string.IsNullOrEmpty(email))
                        error = ErrorCodes.MISSING_FIELDS;
                    else if (!Helper.IsEmailValid(email))
                        error = ErrorCodes.INVALID_EMAIL;
                    else
                    {
                        user.email = email;
                        user.nickname = nickname;
                        user.gender = gender;
                        user.bio = bio;
                        user.country_code = countryCode;
                        user.country_name = countryName;
                        var userUpdated = api.UpdateProfile(param.referral_code, user);
                        if (!userUpdated.HasData)
                            error = userUpdated.Error;
                        else
                            user = userUpdated.Data;
                    }
                }
            }

            // Log //
            api.LogApi("1", "/account/edit-profile", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id,
                user != null && user.id > 0 ? user.username : string.Empty,
                IpHelper.GetClientIp(Request), error.ToErrorCode(), JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                profile = error == null ? Profile.GetFrom(user) : null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
            };
        }

        [HttpPost]
        [ActionName("login-password")]
        [ExecutionMeter]
        public object Login([FromBody] LoginViewModel param)
        {
            //Prepare data
            var api = Api.Instance;
            string username = !String.IsNullOrEmpty(param.username) ? param.username.Trim().ToLower() : String.Empty;
            string password = param.password;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            CustomerAccount user = null;

            ErrorCodes? error = null;
            string address = IpHelper.GetClientIp(Request);

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                error = ErrorCodes.MISSING_FIELDS;
            else
            {
                user = api.GetUserByUserName(username).Data;
                if (user == null)
                    error = ErrorCodes.INVALID_USN_PWD;
                else if (!user.CheckPassword(password))
                    error = ErrorCodes.INVALID_USN_PWD;
                else
                {
                    if (string.IsNullOrEmpty(user.country_code))
                    {
                        IPAddress ip;
                        if (IPAddress.TryParse(address, out ip))
                        {
                            ip.GetCountryCode(c => user.country_code = c, n => user.country_name = n);
                        }
                    }
                    if (!api.UpdateCustomerAccount(user.id, user.country_code, user.country_name, DateTime.UtcNow))
                    {
                        error = ErrorCodes.ServerError;
                    }
                    else
                    {
                        ErrorCodes? errorInReferralCampaign = api.HandleDataReferralCampaign(user.id, user.username, param.game_id, param.device_id, user.inviter_username, address, partner.identifier);
                        if (errorInReferralCampaign != null)
                            api.LogApi("1", "/account/login-password", true,
                                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                                param.partner_id, param.username, IpHelper.GetClientIp(Request), errorInReferralCampaign.ToErrorCode(), errorInReferralCampaign.ToErrorMessage());
                    }
                }
            }

            // Log //
            api.LogApi("1", "/account/login-password", error == null,
                    Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                    param.partner_id, param.username, IpHelper.GetClientIp(Request), error.ToErrorCode(), JsonConvert.SerializeObject(param));

            if (error == null)
                user = api.GetUserById(user.id).Data;

            return new
            {
                success = error == null,
                error_code = error.ToErrorCode(),
                message = error.ToErrorMessage(),
                session = (error == null && user != null) ? api.GetAccessToken(user.id, user.username, partner) : string.Empty,
                profile = error == null ? Profile.GetFrom(user) : null
            };
        }


        [HttpPost]
        [ActionName("change-password")]
        [ExecutionMeter]
        public object ChangePassword([FromBody] APIChangePasswordViewModel param)
        {
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            CustomerAccount user = null;
            ErrorCodes? error = null;
            string newPassword = param.new_password;
            string confirmPassword = param.confirm_password;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else
            {
                user = api.LoadFromAccessToken(partner.identifier, param.session).Data;

                if (user == null)
                    error = ErrorCodes.INVALID_SESSION;
                else if (string.Compare(user.unhashed_password, param.old_password, StringComparison.OrdinalIgnoreCase) != 0)
                    error = ErrorCodes.INVALID_USN_PWD;
                else if ((string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword)) || newPassword != confirmPassword)
                    error = ErrorCodes.UNIDENTICAL_PASSWORDS;
                else if (!api.SetPassword(user, newPassword))
                    error = ErrorCodes.ServerError;
            }

            api.LogApi("1", "/account/change-password", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id,
                (user != null && user.id > 0) ? user.username : string.Empty,
                IpHelper.GetClientIp(Request), error.ToErrorCode(), JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                profile = error == null ? Profile.GetFrom(user) : null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode()
            };
        }

        [HttpPost]
        [ActionName("query-user-id")]
        [ExecutionMeter]
        public object QueryUserID([FromBody] QueryUserIDRequest param)
        {
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            ErrorCodes? error = null;
            CustomerLoginOAuth userOAuth = null;

            // Error Checking //
            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else
            {
                userOAuth = api.GetCustomerLoginOAuth(param.oauth_id).Data;
                if (userOAuth == null)
                {
                    error = ErrorCodes.NON_EXISTING_OAUTH;
                }
            }

            api.LogApi("1", "/account/register", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id,
                (userOAuth != null && userOAuth.id > 0) ? userOAuth.customer_username : string.Empty,
                IpHelper.GetClientIp(Request), error.ToErrorCode(), JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                username = (error == null && userOAuth != null) ? userOAuth.customer_username : string.Empty,
                error_code = error.ToErrorCode(),
                message = error.ToErrorMessage()
            };
        }

        [HttpPost]
        [ActionName("disconnect-oauth")]
        [ExecutionMeter]
        public object DisconnectOAuth([FromBody] OauthModel param)
        {
            var api = Api.Instance;
            string partnerUID = param.partner_id;
            Partner partner = api.GetPartnerById(partnerUID).Data;
            string session = param.session;
            CustomerAccount user = null;
            string service = string.IsNullOrEmpty(param.service)
                ? param.service
                : param.service.ToLower();
            ErrorCodes? error = null;

            if (partner == null)
                error = ErrorCodes.INVALID_PARTNER_ID;
            else
            {
                user = api.LoadFromAccessToken(partner.identifier, session).Data;
                if (string.IsNullOrEmpty(service))
                    error = ErrorCodes.MISSING_FIELDS;
                else if (user == null)
                    error = ErrorCodes.INVALID_SESSION;
                else
                {
                    var userOAuth = api.GetCustomerLoginOAuthByUsername(service, user.username).Data;
                    if (userOAuth == null)
                        error = ErrorCodes.NON_EXISTING_OAUTH;
                    else
                        api.DeleteGetCustomerLoginOAuthById(userOAuth.id);
                }
            }

            api.LogApi("1", "/account/disconnect-oauth", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty,
                param.partner_id,
                user != null ? user.username : string.Empty, IpHelper.GetClientIp(Request),
                error.ToErrorCode(), JsonConvert.SerializeObject(param));
            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode()
            };
        }

        [HttpPost]
        [ActionName("connect-oauth")]
        [ExecutionMeter]
        public object ConnectOAuth([FromBody] APIConnectOauthModel param)
        {
            //prepare data
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            ErrorCodes? error = null;
            CustomerAccount user = null;
            CustomerLoginOAuth userOAuth = null;

            //Handle data
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
                    string service = string.IsNullOrEmpty(param.service)
                        ? string.Empty
                        : param.service.ToLower();

                    // Access token returned by third party (facebook)
                    string token = param.token;

                    if (string.IsNullOrEmpty(service) && string.IsNullOrEmpty(token))
                        error = ErrorCodes.MISSING_FIELDS;
                    else
                    {
                        //string oauthAccountID = api.IDRetriever(service, token);
                        string oauthAccountID = string.Empty;
                        FacebookProfile fbProfile = api.IDRetriever(service, token);
                        oauthAccountID = fbProfile == null
                            ? token
                            : fbProfile.id;
                        userOAuth = Api.Instance.GetCustomerLoginOAuthByIdentity(service, oauthAccountID).Data;

                        if (userOAuth != null)
                            error = ErrorCodes.EXISTING_OAUTH;
                        else if (api.CreateLoginOAuth(new CustomerLoginOAuth
                            {
                                customer_username = user.username,
                                identity = oauthAccountID,
                                service = param.service
                            }) == 0)
                            error = ErrorCodes.ServerError;
                    }
                }
            }

            //Return result
            api.LogApi("1", "/account/login-oauth", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty, param.partner_id,
                user == null ? string.Empty : user.username,
                IpHelper.GetClientIp(Request), error.ToErrorCode(), JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                profile = error == null ? Profile.GetFrom(user) : null
            };
        }

        [HttpPost]
        [ActionName("check-oauth-connection")]
        [ExecutionMeter]
        public object CheckOAuthConnection([FromBody] APIcheckOauthConnectionModel param)
        {
            //prepare data
            var api = Api.Instance;
            Partner partner = api.GetPartnerById(param.partner_id).Data;
            ErrorCodes? error = null;
            CustomerAccount user = null;
            CustomerLoginOAuth userOAuth = null;
            int? userOAuthID = null;

            //Handle data
            if (partner == null)
            {
                error = ErrorCodes.INVALID_PARTNER_ID;
            }
            else
            {
                // Access token returned by previous login
                user = api.LoadFromAccessToken(partner.identifier, param.session).Data;
            }
            if (user == null)
            {
                error = ErrorCodes.INVALID_SESSION;
            }
            else
            {
                string service = string.IsNullOrEmpty(param.service)
                    ? string.Empty
                    : param.service.ToLower();
                // Access token returned by third party
                string token = param.token;

                if (!string.IsNullOrEmpty(service) && !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(param.session))
                {
                    //oauthAccountID = api.IDRetriever(service, token);
                    string oauthAccountID = string.Empty;
                    FacebookProfile fbProfile = api.IDRetriever(service, token);
                    oauthAccountID = fbProfile == null
                        ? token
                        : fbProfile.id;

                    userOAuth = Api.Instance.GetCustomerLoginOAuthByIdentity(service, oauthAccountID).Data;
                    if (userOAuth != null)
                    {
                        userOAuthID = userOAuth.id;
                        if (userOAuth.customer_username == user.username)
                        {
                            error = ErrorCodes.OAUTH_ALREADY_CONNECTED;
                        }
                    }
                    else
                    {
                        error = ErrorCodes.NON_EXISTING_OAUTH;
                    }
                }
                else
                {
                    error = ErrorCodes.MISSING_FIELDS;
                }
            }

            //Return result
            api.LogApi("1", "/account/login-oauth", error == null,
                Request.Headers.UserAgent != null ? Request.Headers.UserAgent.ToString() : string.Empty, param.partner_id,
                user == null ? string.Empty : user.username,
                IpHelper.GetClientIp(Request),
                error.ToErrorMessage(), JsonConvert.SerializeObject(param));

            return new
            {
                success = error == null,
                message = error.ToErrorMessage(),
                error_code = error.ToErrorCode(),
                bindonlyID = userOAuthID
            };
        }
    }
}