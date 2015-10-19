using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Platform.Dal;
using Platform.Models;
using Platform.Utility;
using System.Transactions;
using System.Configuration;
using System.Data;
using System.Web.Configuration;
using DevOne.Security.Cryptography.BCrypt;
using Facebook;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Platform.Core
{
    public partial class Api
    {
        /// <summary>
        /// Used for lock code
        /// </summary>
        static readonly object _object = new object();

        public bool SendRequest(IDbConnection db, string myUsername, string myFriendname, string status = null)
        {
            Friend mySelf = null;
            Friend myFriend = null;
            Platform.Core.Api.Instance.CheckFriendTransaction(db, myUsername, myFriendname, c => mySelf = c, m => myFriend = m);

            if (mySelf != null)
            {
                return false;
            }

            SaveFriend(db, myUsername, myFriendname, string.IsNullOrEmpty(status) ? ConstantValues.S_WAITING : status);
            SaveFriend(db, myFriendname, myUsername, string.IsNullOrEmpty(status) ? ConstantValues.S_PENDING : status);

            return true;
        }

        public bool SendRequest(string myUsername, string myFriendname, string status = null)
        {
            Friend mySelf = null;
            Friend myFriend = null;
            Platform.Core.Api.Instance.CheckFriend(myUsername, myFriendname, c => mySelf = c, m => myFriend = m);

            if (mySelf != null)
            {
                return false;
            }

            SaveFriend(myUsername, myFriendname, string.IsNullOrEmpty(status) ? ConstantValues.S_WAITING : status);
            SaveFriend(myFriendname, myUsername, string.IsNullOrEmpty(status) ? ConstantValues.S_PENDING : status);

            return true;
        }

        public bool ResponseRequest(string myUsername, string myFriendname, string status = null)
        {
            if (!ConstantValues.ListOfValidFriendStatus.Contains(status))
                return false;

            Friend mySelf = null;
            Friend myFriend = null;
            Api.Instance.CheckFriend(myUsername, myFriendname, c => mySelf = c, m => myFriend = m);

            if (mySelf == null)
            {
                return false;
            }

            SaveFriend(myUsername, myFriendname, string.IsNullOrEmpty(status) ? ConstantValues.S_WAITING : status);
            SaveFriend(myFriendname, myUsername, string.IsNullOrEmpty(status) ? ConstantValues.S_PENDING : status);

            return true;
        }

        private int SaveFriend(string myUsername, string myFriendname, string status = null)
        {
            var friend = new Friend
            {
                friend1_username = myUsername,
                friend2_username = myFriendname,
                status = status
            };

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                int result = 0;
                var myTrans = db.BeginTransaction();// db.BeginTransaction();
                try
                {
                    if (repo.GetFriendByName(db, myUsername, myFriendname).Data != null)
                    {
                        if (!repo.UpdateFriendStatus(db, status, myUsername, myFriendname))
                        {
                            myTrans.Rollback();
                            return 0;
                        }
                    }
                    else
                    {
                        result = repo.CreateFriend(db, friend);
                    }

                    myTrans.Commit();
                }
                catch
                {
                    myTrans.Rollback();
                }
                return result;
            }
        }

        private int SaveFriend(IDbConnection db, string myUsername, string myFriendname, string status = null)
        {
            var friend = new Friend
            {
                friend1_username = myUsername,
                friend2_username = myFriendname,
                status = status,
                sent_at = DateTime.UtcNow
            };

            var repo = Repo.Instance;
            return repo.CreateFriend(db, friend);
        }

        public Result<CustomerAccount> Login(string userName, string pwd, IPAddress ip)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.Login(db, userName, pwd, ip);
            }
        }

        public Profile toFriendDictionary(CustomerAccount Self, CustomerAccount targetUser)
        {
            Profile result;
            if (targetUser != null)
            {
                Friend mySelf = null;
                Friend friendship = null;
                Platform.Core.Api.Instance.CheckFriend(Self.username, targetUser.username, c => mySelf = c, m => friendship = m);

                if (friendship != null)
                {
                    if (friendship.status == "accepted")
                    {
                        result = Self.toPublicDictionary();
                        result.status = friendship.status;
                        return result;
                    }
                }
            }
            result = Self.ToMinimalProfile();
            result.status = "";
            return result;
        }


        public Result<CustomerAccount> GetUserById(int userId)
        {
            var repo = Repo.Instance;
            // Check duplicate  //
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerById(db, userId);
            }
        }

        public Result<CustomerAccount> GetUserByUserName(string username)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerByUserName(db, !String.IsNullOrEmpty(username) ? username.Trim().ToLower() : String.Empty);
            }
        }

        public Result<CustomerAccount> GetInviterByCustomerUserName(string username)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetInviterByCustomerUserName(db, username.ToLower());
            }
        }

        public Result<CustomerAccount> GetUserByEmail(string email)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerByEmail(db, email);
            }
        }
        public Result<List<CustomerAccount>> GetUserByConditions(string cons)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetUserByConditions(db, cons);
            }
        }

        public bool IsUserExist(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            var res = GetUserByUserName(username);
            return res.Succeeded;
        }

        public Result<List<AuthAction>> GetActionsByRoleId(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAuthActionsByRoleId(db, id);
            }
        }

        public Result<List<AuthRole>> GetRolesByUserId(int userid)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAuthRolesByUserId(db, userid);
            }
        }

        public bool CheckPassword(string username, string oldPassword)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CheckPassword(db, username, oldPassword);
            }
        }


        public Result<CustomerAccount> LoadFromAccessToken(string partner_identifier, string token, string username = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.LoadFromAccessToken(db, partner_identifier, token, username);
            }
        }

        public Result<CustomerAccount> CreateCustomerAccount(CustomerAccount user)
        {
            try
            {
                var repo = Repo.Instance;
                using (var db = repo.OpenConnectionFromPool())
                {
                    return repo.GetCustomerById(db, repo.CreateCustomerAccount(db, user));
                }
            }
            catch { return Result<CustomerAccount>.Null(ErrorCodes.ServerError); }
        }

        public void AddInviter(IDbConnection db, CustomerAccount self, CustomerAccount inviter)
        {
            self.inviter_username = inviter.username;
            self.referred_at = DateTime.UtcNow;
            SendRequest(db, self.username, inviter.username, Platform.Models.ConstantValues.S_ACCEPTED);

            //Check to know if new user has inviter or not
            var repo = Repo.Instance;

            var hasInviter = false;
            var venviciMember = false;
            hasInviter = repo.GetFriendByName(db, self.username, inviter.username).Data != null;
            if (hasInviter)
            {
                venviciMember = VenviciAPI.Instance.AddMember(self);
            }
            repo.UpdateCustomerAccount(db, self.id, self.inviter_username, self.referred_at, venviciMember);
        }

        public void AddInviter(CustomerAccount self, CustomerAccount inviter)
        {
            self.inviter_username = inviter.username;
            self.referred_at = DateTime.UtcNow;
            SendRequest(self.username, inviter.username, Platform.Models.ConstantValues.S_ACCEPTED);

            //Check to know if new user has inviter or not
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var hasInviter = false;
                var venviciMember = false;
                hasInviter = repo.GetFriendByName(db, self.username, inviter.username).Data != null;
                if (hasInviter)
                {
                    venviciMember = VenviciAPI.Instance.AddMember(self);
                }
                repo.UpdateCustomerAccount(db, self.id, self.inviter_username, self.referred_at, venviciMember);
            }
        }

        public Result<Partner> GetPartnerById(string partnerId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPartnerById(db, partnerId);
            }
        }

        public Result<Partner> GetPartnerByIdentifier(string identifier)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPartnerByIdentifier(db, identifier);
            }
        }

        public Result<CustomerLoginOAuth> GetCustomerLoginOAuthByUsername(string service, string username)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerLoginOAuthByUsername(db, service, username);
            }
        }

        public Result<CustomerLoginOAuth> GetCustomerLoginOAuthByIdentity(string service, string identity)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerLoginOAuthByIdentity(db, service, identity);
            }
        }

        public Result<CustomerLoginOAuth> GetCustomerLoginOAuth(string identity)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerLoginOAuth(db, identity);
            }
        }

        public int DeleteGetCustomerLoginOAuthById(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.DeleteGetCustomerLoginOAuthById(db, id);
            }
        }




        public string GetAccessToken(int userId, string username, Partner partner)
        {
            string SECRET_KEY = ConfigurationManager.AppSettings["SECRET_KEY"];
            string token = Helper.CalculateSHA1(partner.uid + userId.ToString() + SECRET_KEY);

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var accessToken = repo.GetAccessToken(db, partner.identifier, username).Data;
                if (accessToken == null)
                {
                    accessToken = new AccessToken
                    {
                        partner_identifier = partner.identifier,
                        customer_username = username,
                        token = token,
                        saved_at = DateTime.UtcNow
                    };
                    repo.CreateAccessToken(db, accessToken);
                }
                else
                {
                    accessToken.token = token;
                    repo.UpdateAccessToken(db, partner.identifier, username, token);
                }
            }

            return token;
        }

        public bool UpdateVenviciMember(int customerId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateVenviciMember(db, customerId);
            }
        }

        public bool SetUserLocale(int customerId, string locale)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.SetUserLocale(db, customerId, locale);
            }
        }

        /// <summary>
        /// Update string country_code, string country_name, DateTime last_login_at only
        /// </summary>
        public bool UpdateCustomerAccount(int customerId, string country_code, string country_name, DateTime last_login_at)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, country_code, country_name, last_login_at);
            }
        }

        /// <summary>
        /// Update password of customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="password"></param>
        /// <param name="unhashed_password"></param>
        /// <returns></returns>
        public bool UpdateCustomerAccount(int customerId, string password, string unhashed_password)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, password, unhashed_password);
            }
        }

        public bool UpdateCustomerAccount(int customerId, string email, string nickname, string gender, string bio, string country_code, string country_name, DateTime dob)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, email, nickname, gender, bio, country_code, country_name, dob);
            }
        }
        /// <summary>
        /// Update customer_account and if referralCode is not empty AddInviter
        /// </summary>
        /// <param name="referralCode">inviter's username</param>
        /// <param name="user">customer_account object want to update</param>
        /// <returns>Result CustomerAccount</returns>
        public Result<CustomerAccount> UpdateProfile(string referralCode, CustomerAccount user)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var myTrans = db.BeginTransaction();
                try
                {
                    bool result = repo.UpdateCustomerAccount(db, user.id, user.email, user.nickname, user.gender, user.bio, user.country_code, user.country_name, user.dob);
                    if (result)
                    {
                        if (!string.IsNullOrEmpty(referralCode) && string.IsNullOrEmpty(user.inviter_username))
                        {
                            CustomerAccount inviter = GetUserByUserName(referralCode).Data;
                            if (inviter != null)
                            {
                                if (inviter.inviter_username == user.username)
                                {
                                    myTrans.Commit();
                                    return Result<CustomerAccount>.Null(ErrorCodes.TWO_WAY_REFERRING);
                                }
                                else
                                {
                                    AddInviter(db, user, inviter);
                                }
                            }
                            else
                            {
                                myTrans.Commit();
                                return Result<CustomerAccount>.Null(ErrorCodes.NON_EXISTING_REFERRAL_CODE);
                            }
                        }
                        myTrans.Commit();
                        return Result<CustomerAccount>.Make(user);
                    }
                    else
                    {
                        myTrans.Rollback();
                        return Result<CustomerAccount>.Null(ErrorCodes.ServerError);
                    }
                }
                catch
                {
                    myTrans.Rollback();
                    return Result<CustomerAccount>.Null(ErrorCodes.ServerError);
                }

            }

        }

        public bool SetPassword(CustomerAccount user, string password)
        {
            string salt = BCryptHelper.GenerateSalt();
            string hashpwd = BCryptHelper.HashPassword(password, salt);
            byte[] bytes = System.Text.Encoding.Default.GetBytes(hashpwd);

            user.password = System.Text.Encoding.UTF8.GetString(bytes);
            user.unhashed_password = password;

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateUserPassword(db, user.id, user.password, user.unhashed_password);
            }
        }


        public Result<CustomerAccount> Register(string username, string password,
            Partner partner,
            string nickName = null,
            string email = null,
            Genders gender = Genders.Male,
            string referralCode = null,
            string ip_address = "127.0.0.1",
            string country_code = null,
            string country_name = null,
            int? game_id = null,
            string device_id = null)
        {
            if (string.IsNullOrEmpty(ip_address))
            {
                ip_address = "127.0.0.1";
            }

            CustomerAccount newUser = new CustomerAccount();
            newUser.username = username;
            SetPassword(newUser, password);
            newUser.nickname = nickName == null ? username : nickName;
            newUser.email = email;
            newUser.gender = gender.ToString().ToLower();
            IPAddress ip = IPAddress.Parse(ip_address);

            if (partner != null)
            {
                newUser.partner_identifier = partner.identifier;
            }
            if (!string.IsNullOrEmpty(country_code) && !string.IsNullOrEmpty(country_name))
            {
                newUser.country_code = country_code;
                newUser.country_name = country_name;
            }
            else if (ip_address == "127.0.0.1" || (partner != null && new List<int>() { 1, 4, 7 }.Contains(partner.id)))
            {
                newUser.country_code = "ZW";
                newUser.country_name = "Zimbabwe";
            }
            else
            {
                ip.GetCountryCode(c => newUser.country_code = c, n => newUser.country_name = n);
            }

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var myTrans = db.BeginTransaction();
                try
                {
                    //create new user
                    int newUserId = repo.CreateCustomerAccount(db, newUser);
                    if (newUserId == 0)
                    {
                        return Result<CustomerAccount>.Null(ErrorCodes.ServerError);
                    }

                    newUser = repo.GetCustomerById(db, newUserId).Data;
                    if (!string.IsNullOrEmpty(referralCode))
                    {
                        var inviter = repo.GetCustomerByUserName(db, referralCode.ToLower()).Data;
                        if (inviter == null)
                        {
                            myTrans.Commit();
                            return Result<CustomerAccount>.Make(newUser, null);
                        }

                        AddInviter(db, newUser, inviter);
                        newUser = repo.GetCustomerById(db, newUserId).Data;
                    }

                    myTrans.Commit();
                    return Result<CustomerAccount>.Make(newUser, null);
                }
                catch
                {
                    myTrans.Rollback();
                    return Result<CustomerAccount>.Null(ErrorCodes.ServerError);
                }
            }
        }


        /// <summary>
        /// This function is called from login
        /// </summary>
        public ErrorCodes? HandleDataReferralCampaign(int userId, string username, int? game_id, string device_id, string inviterUsername, string ip_address, string partnerIdentifier)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var myTrans = db.BeginTransaction();
                ErrorCodes? error = HandleDataReferralCampaign(repo, db, userId, username, game_id, device_id, inviterUsername, ip_address, partnerIdentifier);
                if (error != null)
                {
                    myTrans.Rollback();
                    return error;
                }

                myTrans.Commit();
            }
            return null;
        }

        private ErrorCodes? HandleDataReferralCampaign(Repo repo, IDbConnection db, int userId, string username, int? game_id,
            string device_id, string inviterUsername, string ip_address, string partnerIdentifier)
        {
            lock (_object)
                //Handle data with referral download system
                if (game_id.HasValue && !string.IsNullOrEmpty(device_id))
                {
                    ReferralCampaign referralCampaign = repo.GetReferralCampaign(db, game_id.Value, DateTime.UtcNow, (int)ReferralCampaignStatus.Running);
                    if (referralCampaign != null                                                                            //Game_id is in a valid campaign
                        && repo.CountRecordDownloadHistory(db, referralCampaign.id) < referralCampaign.quantity             //Quantity is available
                        && !(repo.CheckRecordDownloadHistory(db, referralCampaign.id, game_id.Value, device_id)))           //Check EXISTED DATA in record_download_history table
                    {
                        var referralHistory = new RecordDownloadHistory
                        {
                            device_id = device_id,
                            game_id = game_id.Value,
                            user_id = userId,
                            referral_campaign_id = referralCampaign.id,
                            game_name = referralCampaign.game_name
                        };

                        IPAddress ip;
                        string countryCode = "SG";
                        string countryName = ConstantValues.S_Singapore;

                        if (IPAddress.TryParse(ip_address, out ip))
                        {
                            ip.GetCountryCode(c => countryCode = c, n => countryName = n);
                        }
                        string guidId = Guid.NewGuid().ToString();
                        var transaction = new Platform.Models.Models.TokenTransaction
                        {
                            gtoken_transaction_id = guidId,
                            partner_order_id = guidId,
                            description = string.Format(ConstantValues.S_DESCRIPTION_CHARGED_TOKEN_CASH_BACK, referralCampaign.game_name, username),
                            ip_address = ip_address,
                            country_code = countryCode,
                            created_at = DateTime.UtcNow,
                            updated_at = DateTime.UtcNow,
                            partner_identifier = partnerIdentifier,
                            customer_username = username,
                            amount = referralCampaign.gtoken_per_download,
                            tax = 0,
                            transaction_type = ConstantValues.ListOfTransactionType.First(),
                            service_charge = 0,
                            token_type = "gtoken"
                        };
                        if (!UpdateCashBack(repo, db, transaction, username, inviterUsername, referralCampaign.gtoken_per_download, referralHistory, referralCampaign.title))
                            return ErrorCodes.UPDATE_CASH_BACK_ERROR;
                    }
                }
            return null;
        }

        /// <summary>
        /// 1. If the downloader has GT (ie a VV member), then we deduct and CC to this fellow.
        /// 2. If downloader is not VV member or has no GT, the we check the referral. If referral has GT, we deduct the referral and CC referral
        /// 3. If 1,2 not true, then we add the BV value. (BV= business value).
        /// </summary>
        private bool UpdateCashBack(Repo repo, IDbConnection db, Platform.Models.Models.TokenTransaction trans, string downloaderUsername, string inviterUsername, decimal gtokenPerDownload,
            RecordDownloadHistory referralHistory, string campTitle)
        {
            bool success = true;
            var venviciApi = VenviciAPI.Instance;
            bool isDownloaderVV = venviciApi.CheckValidVenviciUser(downloaderUsername);
            if (isDownloaderVV && venviciApi.CheckGToken(downloaderUsername, gtokenPerDownload))
            {
                referralHistory.earned_username = downloaderUsername;
                repo.CreateRecordDownloadHistory(db, referralHistory);

                repo.CreateTokenTransaction(db, trans);
                success = venviciApi.UpdateCashBack(downloaderUsername, trans, campTitle);
            }
            else
            {
                if (isDownloaderVV)
                {
                    string remark = string.Format("Add GToken for downloading game: {0} with referral: {1}", referralHistory.game_name, inviterUsername);
                    success = venviciApi.AddGToken(downloaderUsername, remark, gtokenPerDownload, inviterUsername);
                }
                if (!success) return success;

                if (venviciApi.CheckGToken(inviterUsername, gtokenPerDownload))
                {
                    referralHistory.earned_username = inviterUsername;
                    repo.CreateRecordDownloadHistory(db, referralHistory);

                    trans.customer_username = inviterUsername;
                    repo.CreateTokenTransaction(db, trans);
                    success = venviciApi.UpdateCashBack(inviterUsername, trans, campTitle);
                }
                else
                    success = venviciApi.PushBv(downloaderUsername, trans);
            }
            return success;
        }


        /// <summary>
        /// Import list referral from admin
        /// </summary>
        private string ImportUserRerralCampaigns(Repo repo, IDbConnection db, int userId, int? game_id, string username, string inviterUsername, string ip_address,
            ReferralCampaign referralCampaign)
        {
            //ReferralCampaign referralCampaign = repo.GetReferralCampaign(db, game_id.Value, DateTime.UtcNow, (int)ReferralCampaignStatus.Running);
            //if (referralCampaign == null)
            //    return ErrorCodes.INVALID_REFERRAL_CAMPAIGN.ToErrorMessage();

            //if (repo.CountRecordDownloadHistory(db, referralCampaign.id) >= referralCampaign.quantity)
            //    return ErrorCodes.REFERRAL_CAMPAIGN_QUANTITY_IS_OVER.ToErrorMessage();

            var referralHistory = new RecordDownloadHistory
            {
                game_id = game_id.Value,
                user_id = userId,
                device_id = string.Empty,
                referral_campaign_id = referralCampaign.id,
                game_name = referralCampaign.game_name
            };

            IPAddress ip;
            string countryCode = "SG";
            string countryName = ConstantValues.S_Singapore;

            if (IPAddress.TryParse(ip_address, out ip))
            {
                ip.GetCountryCode(c => countryCode = c, n => countryName = n);
            }
            string guidId = Guid.NewGuid().ToString();
            var transaction = new Platform.Models.Models.TokenTransaction
            {
                gtoken_transaction_id = guidId,
                partner_order_id = guidId,
                description = string.Format(ConstantValues.S_DESCRIPTION_CHARGED_TOKEN_CASH_BACK, referralCampaign.game_name, username),
                ip_address = ip_address,
                country_code = countryCode,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                partner_identifier = "goplay", //TODO: considering
                customer_username = username,
                amount = referralCampaign.gtoken_per_download,
                tax = 0,
                transaction_type = ConstantValues.ListOfTransactionType.First(),
                service_charge = 0,
                token_type = "gtoken"
            };
            if (!UpdateCashBack(repo, db, transaction, username, inviterUsername, referralCampaign.gtoken_per_download, referralHistory, referralCampaign.title))
                return ErrorCodes.UPDATE_CASH_BACK_ERROR.ToErrorMessage();

            return string.Empty;
        }


        public int CreateLoginOAuth(CustomerLoginOAuth customerLoginOauth)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateCustomerLoginOAuth(db, customerLoginOauth);
            }
        }


        #region working with facebook
        public FacebookProfile IDRetriever(string service, string token)
        {
            if (string.Compare(service, "facebook", StringComparison.OrdinalIgnoreCase) == 0)
                return JsonHelper.DeserializeObject<FacebookProfile>(new FacebookGraphAPI(token).GetObject("/me", null).ToString());
            else if (string.Compare(service, "apple", StringComparison.OrdinalIgnoreCase) == 0)
                return null;

            throw new Exception(Helper.GetDescription(ErrorCodes.NOT_SUPPORTED_OAUTH_PROVIDER));
        }
        #endregion

        public string GenPasswordResetCode(CustomerAccount customer)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var verificationTokens = repo.GetValidVerificationToken(db, customer.username).Data;
                if (verificationTokens != null && verificationTokens.Any())
                {
                    foreach (var item in verificationTokens)
                    {
                        repo.UpdateVerificationStatus(db, item.customer_username, false);
                    }
                }

                var verificationToken = new VerificationToken()
                {
                    code = Guid.NewGuid().ToString(),
                    customer_username = customer.username
                };

                if (repo.CreateVerificationToken(db, verificationToken))
                    return verificationToken.code;
            }
            return string.Empty;
        }

        public Result<VerificationToken> GetValidVerificationTokenByCode(string code)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetValidVerificationTokenByCode(db, code);
            }
        }
        public bool UpdateCustomerAccount(int customerId, string avatar_filname)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, avatar_filname);
            }
        }

        public Result<List<AuthRole>> GetAllRoles()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAllAuthRoles(db);
            }
        }


        public int GetIdByUsername(string username)
        {
            var repo = Repo.Instance;
            // Check duplicate  //
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetIdByUsername(db, username);
            }
        }
    }
}
