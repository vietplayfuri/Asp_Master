using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using GoPlay.Models;
using GoPlay.Dal;
using Platform.Models;
using Platform.Utility;
using System.Configuration;
using System.Data;
using Facebook;
using System.Linq;
using DevOne.Security.Cryptography.BCrypt;
namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        //public Result<CustomerAccount> Register(string userName, string pwd, string email = null, string nickName = null, Genders gender = Genders.Male, string referralID = null, IPAddress ip = null)
        //{
        //    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
        //    {
        //        return Result<CustomerAccount>.Null(ErrorCodes.MissingFields);
        //    }

        //    int len = userName.Length;
        //    if (len < 3 || len > 20)
        //    {
        //        return Result<CustomerAccount>.Null(ErrorCodes.InvalidUsernameLength);
        //    }
        //    if (pwd.Length < 3)
        //    {
        //        return Result<CustomerAccount>.Null(ErrorCodes.PasswordLengthMustAtLeast3);
        //    }

        //    // Check valid user name //
        //    if (!Regex.IsMatch(userName, ConstantValues.REG_EX_USERNAME))
        //    {
        //        return Result<CustomerAccount>.Null(ErrorCodes.InvalidUserName);
        //    }

        //    var repo = Repo.Instance;
        //    // Check duplicate  //
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        //using transaction
        //        // Exist ? //
        //        var user = repo.GetCustomerByUserName(db, userName);
        //        if (user.Succeeded) // already exist //
        //        {
        //            return Result<CustomerAccount>.Null(ErrorCodes.UserNameOrEmailExist); // This has error code //   
        //        }

        //        // Valid Game ? //
        //        //var game = repo.GetGame(db, gameGuid);
        //        //if (game.Data == null)
        //        //{
        //        //    return user.Nullify(game.Error); // quick , null it //
        //        //}

        //        // Not exist, create it //
        //        CustomerAccount newUser = new CustomerAccount();
        //        newUser.nickname = nickName;
        //        newUser.email = email;
        //        newUser.gender = gender.ToString();
        //        //newUser.game_id = game.Data.id;

        //        if (ip == null || IPAddress.IsLoopback(ip)) /* 127.0.0.1 || game.Data.id == 1 || game.Data.id == 4 || game.Data.id == 7)*/ // YN: TODO: REFACTOR THIS //
        //        {
        //            newUser.country_code = ip.GetDefaultCountryCode();
        //            newUser.country_name = ip.GetDefaultCountryName();
        //        }
        //        else
        //        {
        //            ip.GetCountryCode(c => newUser.country_code = c, n => newUser.country_name = n);
        //        }

        //        // Referal code //
        //        // TODO : In a class nicely //
        //        if (!String.IsNullOrEmpty(referralID))
        //        {
        //            var referralUser = repo.GetCustomerByUserName(db, referralID);
        //            if (!referralUser.Succeeded)
        //            {
        //                return Result<CustomerAccount>.Null(ErrorCodes.InvalidReferrerId);
        //            }
        //            newUser.inviter_id = referralUser.Data.id;
        //            newUser.referred_at = DateTime.UtcNow;

        //            // add friend
        //            //friend newFriend = new friend();
        //            //newFriend.friend1_id = newUser.id;
        //            //newFriend.friend2_id = referralUser.id;
        //            //newFriend.status = ConstantValues.ADD_FRIEND_STATUS_ACCEPTED; /*accepted*/
        //        }
        //        //create new user
        //        return repo.CreateCustomerAccount(db, newUser);

        //        //// more todo .... //
        //        //
        //        //
        //        //ef.customer_account.Add(newUser);
        //    }

        //}

        public game_access_token GetAccessToken(int userId, int gameId, string gameUid, string gtokenSession)
        {
            string SECRET_KEY = ConfigurationManager.AppSettings["SECRET_KEY"];
            string token = Helper.CalculateSHA1(gameUid + userId.ToString() + SECRET_KEY);
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var gameAccessToken = repo.GetGameAccessToken(db, gameId, userId).Data;
                if (gameAccessToken == null)
                {
                    gameAccessToken = new game_access_token
                    {
                        game_id = gameId,
                        customer_account_id = userId,
                        token = token
                    };
                    gameAccessToken.id = repo.CreateGameAccessToken(db, gameId, userId, token);
                }

                gameAccessToken.token = token;
                if (!string.IsNullOrEmpty(gtokenSession))
                {
                    gameAccessToken.gtoken_token = gtokenSession;
                    repo.UpdateGameAccessToken(db, gameAccessToken.id, gtokenSession, token);
                }

                return gameAccessToken;
            }
        }

        //public Result<customer_account> CreateCustomerAccount(customer_account user)
        //{
        //    try
        //    {
        //        var repo = Repo.Instance;
        //        using (var db = repo.OpenConnectionFromPool())
        //        {
        //            return repo.GetCustomerById(db, repo.CreateCustomerAccount(db, user));
        //        }
        //    }
        //    catch { return Result<customer_account>.Null(ErrorCodes.ServerError); }
        //}
        //public Result<customer_account> Login(string userName, string pwd, IPAddress ip)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        return repo.Login(db, userName, pwd, ip);
        //    }
        //}


        // TODO: NOT_IMPLEMENTED //
        //public Result<CustomerAccount> Login(LoginTypes platform, string token)
        //{
        //    return Result<CustomerAccount>.Null(); // TEMP
        //}


        //public void GetProfile()
        //{

        //}

        //public void EditProfile(string email, string nickName, Genders? gender = null)
        //{

        //}

        //// TODO: NOT_IMPLEMENTED //
        //private void BindOauth(string xxxx)
        //{

        //}

        //private void LoginOauth(string xxxx)
        //{

        //}
        //private void UnBindOAuth(string token)
        //{

        //}

        //private bool IsOAuthBinded(string xxx)
        //{
        //    return false;
        //}


        public Result<customer_account> GetUserById(int userId)
        {
            var repo = Repo.Instance;
            // Check duplicate  //
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerById(db, userId);
            }
        }

        public Result<customer_account> GetUserByUserName(string username)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerByUserName(db, username);
            }
        }

        /// <summary>
        /// Get user who has email/nickname/username like term but except myseft
        /// </summary>
        /// <param name="term">term like email/nickname/username</param>
        /// <returns>customer account</returns>
        public Result<List<customer_account>> GetUserByTerm(int userId, string term)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerByTerm(db, userId, term);
            }
        }

        public bool IsUserExist(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            var res = GetUserByUserName(username);
            return res.Succeeded;
        }

        public Result<List<auth_action>> GetActionsByRoleId(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAuthActionsByRoleId(db, id);
            }
        }

        public Result<List<auth_role>> GetRolesByUserId(int userid)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAuthRolesByUserId(db, userid);
            }
        }


        public Result<List<auth_role>> GetAllRoles()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAllAuthRoles(db);
            }
        }

        public Result<customer_account> LoadFromAccessToken(string token)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.LoadFromAccessToken(db, token);
            }
        }

        public Result<customer_login_password> GetCustomerLoginPassword(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerLoginPassword(db, id);
            }
        }

        public bool UpdateCustomerAccount(int customerId, string email, string nickname, string gender, string vip)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, email, nickname, gender, vip);
            }
        }

        public bool UpdateCustomerAccount(int customerId, string country_code,
            string country_name, DateTime last_login, string avatar_filename = null, string cover_filename = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, country_code, country_name, last_login, avatar_filename, cover_filename);
            }
        }

        public bool UpdateCustomerAccount(int customerId, DateTime last_login)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, last_login);
            }
        }

        public bool UpdateCustomerAccount(int customerId, bool is_discount_permanent)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, is_discount_permanent);
            }
        }

        public bool UpdateCustomerAccount(int customerId,
           string country_code, string country_name, string bio, string nickname, string inviter_username)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, country_code, country_name, bio, nickname, inviter_username);
            }
        }


        public Result<List<user_notification>> GetUnreadNotifications(int userId, int gameId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetUnreadNotifications(db, userId, gameId);
            }
        }

        public Result<List<game_user_notification>> GetUnreadNotificationsForGame(int userId, int gameId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetUnreadNotificationsForGame(db, userId, gameId);
            }
        }

        public bool UpdateUnreadNotifications(int notificationId, bool isArchived)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateUnreadNotifications(db, notificationId, isArchived);
            }
        }


        public bool UpdateUserNotification(string Ids)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateUserNotification(db, Ids);
            }
        }
        public int CreateUserNotification(user_notification user_noti)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateUserNotification(db, user_noti);
            }
        }
        public Result<customer_account> UpdateProfile(GtokenAPILoginModel gtokenProfile, int? game_id = null)
        {
            try
            {
                var repo = Repo.Instance;
                var profile = gtokenProfile.profile;
                var inviterUsername = profile.inviter;
                customer_account user = null;
                int inviterId = 0;
                int userId = -1;
                using (var db = repo.OpenConnectionFromPool())
                {
                    var inviter = repo.GetCustomerByUserName(db, inviterUsername).Data;

                    if (!string.IsNullOrEmpty(inviterUsername) && inviter == null)
                    {
                        var gtokenProfileData = GTokenAPIAccount(new GtokenModelAccountAction
                        {
                            enumAction = EGtokenAction.Profile,
                            username = inviterUsername,
                            session = gtokenProfile.session
                        }).Result;

                        if (!gtokenProfileData.HasData)
                        {
                            return Result<customer_account>.Make(null, ErrorCodes.InvalidSession);
                        }

                        var profileData = gtokenProfileData.Data;
                        var newId = repo.CreateCustomerAccount(db,
                            profileData.profile.email,
                            profileData.profile.account,
                            profileData.profile.country_code,
                            profileData.profile.country_name);

                        if (newId > 0)
                        {
                            inviterId = newId;
                            CreateFreeCoinTransaction(new free_coin_transaction
                            {
                                order_id = Guid.NewGuid().ToString(),
                                customer_account_id = newId,
                                status = ConstantValues.S_SUCCESS,
                                description = ConstantValues.S_DESCRIPTION_CREATE_FREE_COIN_TRANSACTION,
                                amount = 1
                            });
                            userId = newId;
                        }
                        else
                        {
                            return Result<customer_account>.Make(null, ErrorCodes.InvalidUserName);
                        }
                    }

                    if (inviter != null)
                        inviterId = inviter.id;

                    user = repo.GetCustomerByUserName(db, profile.account).Data;
                    if (user == null)
                    {
                        var newId = repo.CreateCustomerAccount(db,
                            profile.email,
                            profile.account,
                            profile.country_code,
                            profile.country_name,
                            profile.nickname,
                            profile.gender,
                            profile.bio,
                            game_id);

                        if (newId > 0)
                        {
                            var transacton = new free_coin_transaction
                            {
                                order_id = Guid.NewGuid().ToString(),
                                customer_account_id = newId,
                                status = ConstantValues.S_SUCCESS,
                                description = ConstantValues.S_DESCRIPTION_CREATE_FREE_COIN_TRANSACTION,
                                amount = 1
                            };
                            CreateFreeCoinTransaction(transacton);
                            userId = newId;
                        }
                        else
                        {
                            return Result<customer_account>.Make(null, ErrorCodes.InvalidUserName);
                        }
                    }
                    else
                    {
                        repo.UpdateCustomerAccount(db, user.id,
                            profile.email,
                            profile.country_code,
                            profile.country_name,
                            profile.nickname,
                            profile.gender,
                            profile.bio);
                        userId = user.id;
                    }

                    user = repo.GetCustomerById(db, userId).Data;
                    if (user != null && !string.IsNullOrEmpty(inviterUsername) && inviterId > 0)
                    {
                        AddInviter(gtokenProfile.session, user, inviterId, inviterUsername);
                    }

                    var i = 0;
                    if (string.IsNullOrEmpty(user.cover_filename))
                    {
                        i = user.id % 11;
                        user.cover_filename = string.Format(ConstantCommon.COVER_URL, i);
                    }
                    if (string.IsNullOrEmpty(user.avatar_filename))
                    {
                        i = user.id % 15;
                        user.avatar_filename = string.Format(ConstantCommon.AVATAR_URL, i);
                    }
                    repo.UpdateCustomerAccount(db, user.id, user.country_code, user.country_name, DateTime.UtcNow, user.avatar_filename, user.cover_filename);

                    return Result<customer_account>.Make(repo.GetCustomerById(db, userId).Data, null);
                }
            }
            catch
            {
                return Result<customer_account>.Make(null, ErrorCodes.ServerError);
            }
        }

        public void AddInviter(string gtokenSession, customer_account self, int inviter_id, string inviter_username)
        {
            self.inviter_username = inviter_username;
            self.referred_at = DateTime.UtcNow;
            SendRequest(gtokenSession, self.id, inviter_id, ConstantValues.S_ACCEPTED);

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                Repo.Instance.UpdateCustomerAccount(db, self.id, inviter_username, self.referred_at.Value);
            }

        }

        private bool UpdateFriendStatus(int myId, int myFriendId, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateFriendStatus(db, myId, myFriendId, status);
            }
        }

        private int SaveFriend(int myId, int myFriendId, string status = null)
        {
            var friend = new friend
            {
                friend1_id = myId,
                friend2_id = myFriendId,
                status = status
            };

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateFriend(db, friend);
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

        #region working with facebook
        public string IDRetriever(string service, string accessToken)
        {
            if (string.Compare(service, "facebook", StringComparison.OrdinalIgnoreCase) == 0)
            {
                //var facebook = new FacebookGraphAPI(accessToken);
                //var profile = facebook.GetObject("me", null);
                var profile = GetObjectFromFaceBook(accessToken);
                return string.Empty;
                //return profile["id"];
            }
            else if (string.Compare(service, "apple", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return accessToken;
            }
            throw new Exception(Platform.Utility.Helper.GetDescription(ErrorCodes.NOT_SUPPORTED_OAUTH_PROVIDER));
        }

        public dynamic GetObjectFromFaceBook(string token)
        {
            var facebook = new FacebookGraphAPI(token);
            return facebook.GetObject("me", null);
        }
        #endregion

        public string GenPasswordResetCode(customer_account customer)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var verificationTokens = repo.GetValidVerificationToken(db, customer.id).Data;
                if (verificationTokens != null && verificationTokens.Any())
                {
                    foreach (var item in verificationTokens)
                    {
                        repo.UpdateVerificationStatus(db, item.customer_account_id, false);
                    }
                }

                var verificationToken = new verification_token()
                {
                    code = Guid.NewGuid().ToString(),
                    customer_account_id = customer.id,
                    is_valid = true
                };

                if (repo.CreateVerificationToken(db, verificationToken))
                    return verificationToken.code;
            }
            return string.Empty;
        }

        public Result<verification_token> GetValidVerificationTokenByCode(string code)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetValidVerificationTokenByCode(db, code);
            }
        }
        public bool SetPassword(customer_account user, string password)
        {
            string salt = BCryptHelper.GenerateSalt();
            string hashpwd = BCryptHelper.HashPassword(password, salt);
            byte[] bytes = System.Text.Encoding.Default.GetBytes(hashpwd);

            string pwd = System.Text.Encoding.UTF8.GetString(bytes);
            string unhashed_pwd = password;

            var repo = Repo.Instance;
            using (var db = repo.OpenTransactionFromPool())
            {
                return repo.UpdateUserPassword(db, user.id, pwd, unhashed_pwd);
            }
        }
        public bool CreateLoginOAuth(customer_login_oauth customerLoginOauth)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var result = repo.GetCustomerLoginOauth(db, customerLoginOauth.service.ToLower(), customerLoginOauth.identity).Data;
                if (result != null)
                    return false;
                return repo.CreateCustomerLoginOAuth(db, customerLoginOauth) > 0;
            }
        }

        /// <summary>
        /// Send request to add friend
        /// </summary>
        /// <param name="gtokenSession">Session of user in Gtoken system 
        /// - we can get by using game_access_token table</param>
        /// <param name="myId">My Id</param>
        /// <param name="myFriendId">friend Id</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SendRequest(string gtokenSession, int myId, int myFriendId, string status = null)
        {
            friend mySelf = null;
            friend myFriend = null;

            CheckFriendTransaction(myId, myFriendId, c => mySelf = c, m => myFriend = m);

            if (mySelf != null)
                return false;
            SaveFriend(myId, myFriendId, string.IsNullOrEmpty(status) ? ConstantValues.S_WAITING : status);

            SaveFriend(myFriendId, myId, string.IsNullOrEmpty(status) ? ConstantValues.S_PENDING : status);

            //dont care response status
            var response = GTokenAPIFriend(new GtokenModelFriendAction
            {
                enumAction = EGtokenAction.AddFriend,
                session = gtokenSession,
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"],
                friend_username = GetUserById(myFriendId).Data.username
            });

            return true;
        }

        public Result<GtokenAPIFriend> RespondRequest(string gtokenSession, int myId, int myFriendId, string status = null)
        {
            if (!ConstantValues.ListOfValidFriendStatus.Contains(status))
                return Result<GtokenAPIFriend>.Null(ErrorCodes.INVALID_FRIEND_REQUEST_STATUS);

            friend mySelf = null;
            friend myFriend = null;

            CheckFriendTransaction(myId, myFriendId, c => mySelf = c, m => myFriend = m);

            if (mySelf == null)
                return Result<GtokenAPIFriend>.Null(ErrorCodes.REQUEST_ALREADY_SENT);

            var response = GTokenAPIFriend(new GtokenModelFriendAction
            {
                enumAction = EGtokenAction.RespondRequest,
                session = gtokenSession,
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"],
                friend_username = GetUserById(myFriendId).Data.username,
                status = status
            });

            if (response.Succeeded)
            {
                UpdateFriendStatus(myId, myFriendId, status);
                UpdateFriendStatus(myFriendId, myId, status);
            }
            return response;
        }

        public Result<partner_account> GetPartner(string client_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPartner(db, client_id);
            }
        }

        public Result<partner_account> GetPartner(int partner_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPartner(db, partner_id);
            }
        }

        public Result<List<customer_account>> GetCustomerAccountsByCondition(string condition)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerAccountsByCondition(db, condition);
            }
        }

        public Result<List<CustomerAccountReport>> GetUserByConditions(
            string timezone,
            string regStartTime = null,
            string regEndTime = null,
            string loginStartTime = null,
            string loginEndTime = null,
            string username = null,
            string referrer = null,
            int? game_id = null,
            string accountManager = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetUserByCondition(db, timezone, regStartTime, regEndTime, loginStartTime, loginEndTime, username, referrer, game_id, accountManager);
            }
        }


        public int GetIdByUsername(string username)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetIdByUsername(db, username);
            }
        }

        public bool UpdateCustomerAccount(int customerId, string account_manager, string account_manager_note)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, account_manager, account_manager_note);
            }
        }

        public bool RecalculatePlayToken(int customerId)
        {
            decimal play_token = 0;
            var sqlQuery = @"SELECT * FROM  coin_transaction
                            WHERE customer_account_id={0} AND status IN('success', 'pending')";
            var successTransactions = GetCoinTransactionsByCustomQuery(String.Format(sqlQuery, customerId));
            if (successTransactions.HasData && successTransactions.Data.Count > 0)
            {
                play_token = successTransactions.Data.Where(x => x.amount.HasValue).Sum(x => x.amount.Value);
            }

            decimal free_play_token = 0;
            sqlQuery = @"SELECT * FROM  free_coin_transaction
                            WHERE customer_account_id={0} AND status IN('success', 'pending')";

            var successFreeTransactions = GetFreeCoinTransactionsByCustomQuery(String.Format(sqlQuery, customerId));
            if (successFreeTransactions.HasData && successFreeTransactions.Data.Count > 0)
            {
                free_play_token = successFreeTransactions.Data.Where(x => x.amount.HasValue).Sum(x => x.amount.Value);
            }
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, play_token, free_play_token);
            }
        }


        public void AddActiveGamerScheme(customer_account customer, coin_transaction coinTransaction)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var reward = repo.GetActiveGamerScheme(db, customer.id).Data;
                if (reward == null)
                {
                    customer_account inviter = repo.GetCustomerByUserName(db, customer.inviter_username).Data;
                    reward = new active_gamer_scheme
                    {
                        customer_account_id = customer.id,
                        inviter_id = inviter.id,
                        balance = 0
                    };
                    repo.CreateActiveGamerScheme(db, reward);
                }

                if (reward.is_archived)
                    return;

                reward.balance += coinTransaction.amount;
                repo.UpdateActiveGamerScheme(db, reward);
            }
        }

        public bool RecalculateGCoin(int customerId)
        {
            decimal gcoin = 0;
            var sqlQuery = @"SELECT * FROM  gcoin_transaction
                            WHERE customer_account_id={0} AND status IN('success', 'pending')";
            var successTransactions = GetCoinTransactionsByCustomQuery(String.Format(sqlQuery, customerId));
            if (successTransactions.HasData && successTransactions.Data.Count > 0)
            {
                gcoin = successTransactions.Data.Where(x => x.amount.HasValue).Sum(x => x.amount.Value);
            }

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomerAccount(db, customerId, gcoin);
            }
        }

        public Result<customer_login_oauth> GetCustomerLoginOauth(string service, string identity)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerLoginOauth(db, service.ToLower(), identity);
            }
        }
    }
}
