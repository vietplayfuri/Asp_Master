using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Net;
using System.Data;
using System.Text.RegularExpressions;
using GoPlay.Models;
using Platform.Models;
using Npgsql;
using Platform.Utility;

namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<customer_account> GetCustomerById(IDbConnection db, int id)
        {
            string sqlString = @"SELECT ca.* FROM customer_account AS ca 
                              WHERE ca.id=@id";
            var customer = db.Query<customer_account>(sqlString, new { id }).FirstOrDefault();
            return Result<customer_account>.Make(customer, errorIfNull: ErrorCodes.InvalidUserNameOrPassword);
        }

        public Result<customer_account> GetCustomerByUserName(IDbConnection db, string username)
        {
            string sqlString = @"SELECT 
                ca.*
                FROM customer_account AS ca 
                WHERE ca.username=@username";

            var customer = db.Query<customer_account>(sqlString, new { username }).FirstOrDefault();
            return Result<customer_account>.Make(customer, errorIfNull: ErrorCodes.InvalidUserNameOrPassword);
        }


        /// <summary>
        /// Get user who has email/nickname/username like term but except myseft
        /// </summary>
        /// <param name="db"></param>
        /// <param name="term">term like email/nickname/username</param>
        /// <returns>customer_account</returns>
        public Result<List<customer_account>> GetCustomerByTerm(IDbConnection db, int userId, string term)
        {
            var encodeForLike = string.IsNullOrEmpty(term)
                ? string.Empty
                : term.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
            term = "%" + encodeForLike + "%";
            string query = @"
            SELECT id, avatar_filename, nickname, username
            FROM customer_account
            WHERE 
                (lower(email) LIKE @term 
                   OR lower(nickname) LIKE @term 
                   OR lower(username) LIKE @term) 
                AND id != @userId
            ORDER BY username";

            var customers = db.Query<customer_account>(query, new { userId, term }).AsList();

            return Result<List<customer_account>>.Make(customers, errorIfNull: ErrorCodes.NotFound);
        }

//        public int CreateCustomerAccount(IDbConnection db, customer_account customerAccount)
//        {
//            string sql = @"INSERT INTO customer_account(
//            username, nickname, email, gender, avatar_filename, vip, play_token, 
//            inviter_id, created_at, updated_at, last_login_at, is_archived, 
//            referral_code, password_credential_id, free_play_token, bio, 
//            locale, country_code, country_name, game_id, cover_filename, 
//            account_manager, account_manager_note, gcoin, is_discount_permanent, 
//            referred_at)
//            VALUES (
//            @username, @nickname, @email, @gender, @avatar_filename, @vip, @play_token, 
//            @inviter_id, @created_at, @updated_at, @last_login_at, @is_archived, 
//            @referral_code, @password_credential_id, @free_play_token, @bio, 
//            @locale, @country_code, @country_name, @game_id, @cover_filename, 
//            @account_manager, @account_manager_note, @gcoin, @is_discount_permanent, 
//            @referred_at)
//             RETURNING id";
//            return db.Query<int>(sql, customerAccount).FirstOrDefault();
//        }

        public int CreateCustomerAccount(IDbConnection db,
            string email,
            string username,
            string country_code,
            string country_name,
            string nickname,
            string gender,
            string bio,
            int? game_id = null)
        {
            string sql = @"INSERT INTO customer_account(
            username, nickname, email, gender, bio, 
            country_code, country_name, game_id)
            VALUES (
            @username, @nickname, @email, @gender, @bio, 
            @country_code, @country_name, @game_id)
             RETURNING id";
            return db.Query<int>(sql, new
            {
                username,
                email,
                country_code,
                country_name,
                nickname,
                gender,
                bio,
                game_id
            }).FirstOrDefault();
        }

        public int CreateCustomerAccount(IDbConnection db, string email,
            string username,
            string country_code, string country_name)
        {
            string sql = @"INSERT INTO customer_account(
            username, email, country_code, country_name)
            VALUES (
            @username, @email, @country_code, @country_name)
             RETURNING id";
            return db.Query<int>(sql, new { username, email, country_code, country_name }).FirstOrDefault();
        }

        public bool CreateCustomerLoginPassword(IDbConnection db, CustomerLoginPassword customerLoginPassword)
        {
            //string sqlString = "";
            //return 1 == db.Execute(sqlString);
            return true;//for testing
        }

        public Result<List<auth_action>> GetAuthActionsByCustomerId(IDbConnection db, int id)
        {
            string sqlString = "SELECT auth_action.* " +
                               "FROM auth_action " +
                               "WHERE auth_action.id IN (" +
                                     "SELECT DISTINCT(auth_role_action.action_id) " +
                                            "FROM auth_role_action " +
                                            "WHERE auth_role_action.role_id IN (" +
                                                  "SELECT DISTINCT(auth_assignment.role_id) " +
                                                         "FROM auth_assignment " +
                                                         "JOIN customer_account ON customer_account.id=auth_assignment.customer_account_id " +
                                                         "WHERE customer_account.id=@id))";
            var authAction = db.Query<auth_action>(sqlString, new { id }).AsList();
            return Result<List<auth_action>>.Make(authAction);
        }

        public Result<List<auth_action>> GetAuthActionsByRoleId(IDbConnection db, int id)
        {
            string sqlString = @"SELECT aa.* 
                FROM auth_action aa
                JOIN auth_role_action ara ON aa.id = ara.action_id
                WHERE ara.role_id=@id";
            var authAction = db.Query<auth_action>(sqlString, new { id }).AsList();
            return Result<List<auth_action>>.Make(authAction);
        }

        public Result<List<auth_action>> GetAllAuthActions(IDbConnection db)
        {
            string sqlString = @"SELECT * 
                FROM auth_action";
            var authAction = db.Query<auth_action>(sqlString).AsList();
            return Result<List<auth_action>>.Make(authAction);
        }

        public Result<List<auth_role>> GetAuthRolesByUserId(IDbConnection db, int userid)
        {
            string sqlString = @"SELECT ar.* 
                FROM auth_role ar
                JOIN auth_assignment aa ON ar.id = aa.role_id
                WHERE aa.customer_account_id = @userid";
            var authAction = db.Query<auth_role>(sqlString, new { userid }).AsList();
            return Result<List<auth_role>>.Make(authAction);
        }

        public Result<List<auth_role>> GetAllAuthRoles(IDbConnection db)
        {
            string sqlString = @"SELECT * 
                FROM auth_role";
            var authAction = db.Query<auth_role>(sqlString).AsList();
            return Result<List<auth_role>>.Make(authAction);
        }

        public Result<customer_login_password> GetCustomerLoginPassword(IDbConnection db, int id)
        {
            var customer = db.Query<customer_login_password>(@"SELECT * FROM customer_login_password
                WHERE customer_account_id = @id", new { id }).FirstOrDefault();
            return Result<customer_login_password>.Make(customer, errorIfNull: ErrorCodes.InvalidUserId);
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string email, string nickname, string gender, string vip)
        {
            string sql = @"UPDATE customer_account 
                SET email =@email, nickname = @nickname, gender = @gender, vip= @vip 
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, email, nickname, gender, vip });
        }

        public bool UpdateCustomerAccount(IDbConnection db,
            int customerId, string email,
            string country_code, string country_name,
            string nickname, string gender, string bio)
        {
            string sql = @"UPDATE customer_account 
                SET email = @email, country_code = @country_code,
                    country_name = @country_name, 
                    nickname = @nickname, gender = @gender, bio = @bio 
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new
            {
                customerId,
                email,
                country_code,
                country_name,
                nickname,
                gender,
                bio,
            });
        }

        public Result<List<user_notification>> GetUnreadNotifications(IDbConnection db, int userId, int gameId)
        {
            string sql = @"SELECT un.* 
                FROM user_notification un
                JOIN game_notification gn ON un.game_notification_id = gn.id
                WHERE
                    gn.game_id= @gameId 
                AND un.customer_account_id = @userId 
                AND un.is_archived = false";
            var obj = db.Query<user_notification>(sql, new { userId, gameId }).AsList();
            return Result<List<user_notification>>.Make(obj);
        }

        public Result<List<game_user_notification>> GetUnreadNotificationsForGame(IDbConnection db, int userId, int gameId)
        {
            string sql = @"SELECT
                un.id,
                un.created_at,
                un.is_archived,
                gn.good_type,
                gn.good_id,
                gn.good_amount

                FROM user_notification un
                JOIN game_notification gn ON un.game_notification_id = gn.id
                WHERE
                      gn.game_id= @gameId 
                  AND un.customer_account_id = @userId 
                  AND un.is_archived = false";
            var obj = db.Query<game_user_notification>(sql, new { userId, gameId }).AsList();

            return Result<List<game_user_notification>>.Make(obj, errorIfNull: ErrorCodes.INVALID_GAME_ID);
        }

        public bool UpdateUnreadNotifications(IDbConnection db,
            int notificationId, bool isArchived)
        {
            string sql = @"UPDATE user_notification
                SET is_archived = @isArchived
                WHERE
                id = @notificationId";

            return 1 == db.Execute(sql, new { notificationId, isArchived });
        }

        public bool UpdateUserNotification(IDbConnection db, string Ids)
        {
            if (!string.IsNullOrEmpty(Ids))
            {
                string sql = String.Format("UPDATE user_notification SET is_archived = true WHERE id in({0})", Ids);
                return Ids.Split(',').Length == db.Execute(sql);
            }
            return false;
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string inviter_username, DateTime referred_at)
        {
            string sql = @"UPDATE customer_account 
                SET inviter_username = @inviter_username, referred_at = @referred_at
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, inviter_username, referred_at });
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId,
            string country_code, string country_name, DateTime last_login_at,
            string avatar_filename = null, string cover_filename = null)
        {
            string ava = string.IsNullOrEmpty(avatar_filename) ? "" : ", avatar_filename = @avatar_filename ";
            string cover = string.IsNullOrEmpty(cover_filename) ? "" : ", cover_filename = @cover_filename ";
            string sql = @"UPDATE customer_account 
                SET country_code = @country_code, country_name = @country_name,
                    last_login_at = @last_login_at " + ava + cover +
                " WHERE id = @customerId";
            return 1 == db.Execute(sql, new
            {
                customerId,
                country_code,
                country_name,
                last_login_at,
                avatar_filename,
                cover_filename
            });
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, DateTime last_login_at)
        {
            string sql = @"UPDATE customer_account 
                SET last_login_at = @last_login_at
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new
            {
                customerId,
                last_login_at
            });
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId,
            string country_code, string country_name, string bio, string nickname, string inviter_username)
        {
            string sql = string.Format(@"UPDATE customer_account 
                                        SET nickname = @nickname, country_code = @country_code, country_name = @country_name,
                                        bio = @bio, inviter_username ={0}
                                        WHERE id = @customerId", !string.IsNullOrEmpty(inviter_username) ? "@inviter_username" : "inviter_username");

            return 1 == db.Execute(sql, new
            {
                nickname,
                customerId,
                country_code,
                country_name,
                bio,
                inviter_username
            });
        }
        public bool UpdateCustomerAccount(IDbConnection db, int customerId, bool is_discount_permanent)
        {
            string sql = @"UPDATE customer_account 
                SET is_discount_permanent = @is_discount_permanent
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new
            {
                customerId,
                is_discount_permanent
            });
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, decimal? play_token, decimal? free_play_token)
        {
            string sql = @"UPDATE customer_account 
                SET {0}
                WHERE id = @customerId";

            var param = new List<string>();
            if (play_token.HasValue)
            {
                param.Add("play_token=@play_token");
            }
            if (free_play_token.HasValue)
            {
                param.Add("free_play_token=@free_play_token");
            }

            sql = String.Format(sql, String.Join(" ,", param.ToArray()));
            return 1 == db.Execute(sql, new
                        {
                            customerId,
                            play_token,
                            free_play_token
                        });
        }

        public bool SetUserLocale(IDbConnection db, int userId, string locale)
        {
            string sql = @"UPDATE customer_account 
                SET locale = @locale
                WHERE 
                id = @id";

            return 1 == db.Query<int>(sql, new { id = userId, locale = locale }).FirstOrDefault();
        }

        public Result<List<verification_token>> GetValidVerificationToken(IDbConnection db, int customer_account_id)
        {
            string sqlString = @"SELECT *
                FROM verification_token
                WHERE 
                    customer_account_id = @customer_account_id
                AND 
                    is_valid = 't'";
            var verificationToken = db.Query<verification_token>(sqlString, new { customer_account_id }).AsList();
            return Result<List<verification_token>>.Make(verificationToken);
        }
        public Result<verification_token> GetValidVerificationTokenByCode(IDbConnection db, string code)
        {
            string sqlString = @"SELECT *
                FROM verification_token
                WHERE 
                    code = @code";
            var verificationToken = db.Query<verification_token>(sqlString, new { code }).FirstOrDefault();
            return Result<verification_token>.Make(verificationToken);
        }

        public bool UpdateVerificationStatus(IDbConnection db, int customer_account_id, bool is_valid)
        {
            string sqlString = @"UPDATE verification_token
                SET is_valid = @is_valid                
                WHERE 
                    customer_account_id = @customer_account_id";

            return 1 == db.Execute(sqlString, new { customer_account_id, is_valid });
        }
        public bool CreateVerificationToken(IDbConnection db, verification_token token)
        {
            string sqlString = @"INSERT INTO verification_token
                (customer_account_id, code, is_valid)
                VALUES 
                (@customer_account_id, @code, @is_valid)";
            return 1 == db.Execute(sqlString, new { token.customer_account_id, token.code, token.is_valid });
        }

        public bool UpdateUserPassword(IDbConnection db, int userId, string password, string unhashed_password)
        {
            string sql = @"UPDATE customer_login_password
                SET password = @password, unhashed_password = @unhashed_password
                WHERE 
                customer_account_id = @userId";

            return 1 == db.Execute(sql, new { userId, password, unhashed_password });
        }
        public int CreateCustomerLoginOAuth(IDbConnection db, customer_login_oauth customerLoginOauth)
        {
            string sql = @"INSERT INTO customer_login_oauth
                (service, identity, customer_account_id)
                VALUES
                (@service, @identity, @customer_account_id)
                 RETURNING id";

            return db.Query<int>(sql, customerLoginOauth).FirstOrDefault();
        }

        public int CreateUserNotification(IDbConnection db, user_notification user_noti)
        {
            string sql = @"INSERT INTO user_notification(
                          customer_account_id, game_notification_id)
                          VALUES (@customer_account_id,@game_notification_id)
                           RETURNING id";
            return db.Query<int>(sql, user_noti).FirstOrDefault();
        }
        public Result<partner_account> GetPartner(IDbConnection db, string client_id)
        {
            string sql = @"SELECT * FROM partner_account
                                    WHERE client_id = @client_id";
            var obj = db.Query<partner_account>(sql, new { client_id }).FirstOrDefault();
            return Result<partner_account>.Make(obj, ErrorCodes.INVALID_PARTNER_ID);
        }

        public Result<partner_account> GetPartner(IDbConnection db, int partner_id)
        {
            string sql = @"SELECT * FROM partner_account
                                    WHERE id = @partner_id";
            var obj = db.Query<partner_account>(sql, new { partner_id }).FirstOrDefault();
            return Result<partner_account>.Make(obj, ErrorCodes.INVALID_PARTNER_ID);
        }

        public Result<List<customer_account>> GetCustomerAccountsByCondition(IDbConnection db, string condition)
        {
            string sql = @"SELECT * FROM customer_account ca
                                    WHERE 1=1 " + condition;
            var obj = db.Query<customer_account>(sql).AsList();
            return Result<List<customer_account>>.Make(obj, ErrorCodes.NotFound);
        }

        public bool CreateAuthAssignmentByRoles(IDbConnection db, int userId, params string[] roles)
        {
            string sql = @"INSERT INTO auth_assignment
                            SELECT AR.id AS role_id,
	                                @userId AS customer_account_id
                            FROM auth_role ar
                            LEFT JOIN auth_assignment aa ON ar.id =aa.role_id AND customer_account_id = @userId
                            WHERE aa.role_id IS NULL AND name IN ('" + string.Join("','", roles) + "')";

            return roles.Count() == db.Execute(sql, new { userId });
        }
        public Result<List<CustomerAccountReport>> GetUserByCondition(IDbConnection db,
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
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.AppendLine(@"SELECT 
                ca.id,
                ca.nickname,
                ca.email,
                ca.username,
                ca.play_token,
                ca.free_play_token,
                ca.vip,
                game.name as game_name,
                ca.country_name,
                ca.inviter_username,
                ca.created_at,
                ca.last_login_at,
                ca.account_manager

                FROM customer_account ca
                LEFT JOIN game ON game.id = ca.game_id
                WHERE 1 = 1 ");

            DateTime dateRegFrom = DateTime.UtcNow;
            DateTime dateRegTo = DateTime.UtcNow;
            DateTime dateLoginFrom = DateTime.UtcNow;
            DateTime dateLoginTo = DateTime.UtcNow;

            string termUsername = username;
            string termReferral = referrer;
            string encodeForLike = string.Empty;

            if (!string.IsNullOrEmpty(regStartTime))
            {
                dateRegFrom = Helper.timeFromString(regStartTime, timezone);
                queryBuilder.AppendLine("AND ca.created_at >= @dateRegFrom");
            }

            if (!string.IsNullOrEmpty(regEndTime))
            {
                dateRegTo = Helper.timeFromString(regEndTime, timezone);
                queryBuilder.AppendLine("AND ca.created_at <= @dateRegTo");
            }

            if (!string.IsNullOrEmpty(loginStartTime))
            {
                dateLoginFrom = Helper.timeFromString(loginStartTime, timezone);
                queryBuilder.AppendLine("AND ca.last_login_at >= @dateLoginFrom");
            }

            if (!string.IsNullOrEmpty(loginEndTime))
            {
                dateLoginTo = Helper.timeFromString(loginEndTime, timezone);
                queryBuilder.AppendLine("AND ca.last_login_at <= @dateLoginTo");
            }

            if (!string.IsNullOrEmpty(username))
            {
                encodeForLike = string.IsNullOrEmpty(username)
                ? string.Empty
                : username.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
                termUsername = "%" + encodeForLike + "%";

                queryBuilder.AppendLine(@"AND (lower(ca.email) LIKE @termUsername 
                   OR lower(ca.nickname) LIKE @termUsername 
                   OR lower(ca.username) LIKE @termUsername)");
            }

            if (!string.IsNullOrEmpty(referrer))
            {
                encodeForLike = string.IsNullOrEmpty(referrer)
                ? string.Empty
                : referrer.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
                termReferral = "%" + encodeForLike + "%";

                queryBuilder.AppendLine(@"AND ca.inviter_username IN (
                    SELECT username FROM customer_account ca 
                    WHERE 
                        lower(ca.email) LIKE @termReferral 
                        OR lower(ca.nickname) LIKE @termReferral 
                        OR lower(ca.username) LIKE @termReferral)");
            }

            if (!string.IsNullOrEmpty(accountManager) && accountManager != GoPlayConstantValues.S_ALL)
            {
                queryBuilder.AppendLine("AND ca.account_manager = @accountManager");
            }

            if (game_id.HasValue && game_id == 0)
            {
                queryBuilder.AppendLine("AND ca.game_id is null");
            }
            else if (game_id.HasValue && game_id != -1)
            {
                queryBuilder.AppendLine("AND ca.game_id = @game_id");
            }

            string sqlString = queryBuilder.ToString();
            var customers = db.Query<CustomerAccountReport>(sqlString, new
            {
                dateRegFrom,
                dateRegTo,
                dateLoginFrom,
                dateLoginTo,
                termUsername,
                termReferral,
                game_id,
                accountManager
            }).AsList();

            return Result<List<CustomerAccountReport>>.Make(customers);
        }


        public int GetIdByUsername(IDbConnection db, string username)
        {
            string sqlString = @"SELECT id FROM customer_account WHERE username=@username";
            return db.Query<int>(sqlString, new { username }).FirstOrDefault();
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string account_manager, string account_manager_note)
        {
            string sql = @"UPDATE customer_account 
                SET account_manager =@account_manager, account_manager_note = @account_manager_note
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, account_manager, account_manager_note });
        }

        public Result<customer_account> GetCustomerByCoinTransactionId(IDbConnection db, int id)
        {
            string sqlString = @"SELECT ca.* 
                FROM customer_account ca
                JOIN coin_transaction ct ON ca.id = ct.customer_account_id
                WHERE ct.id = @id";
            var customer = db.Query<customer_account>(sqlString, new { id }).FirstOrDefault();
            return Result<customer_account>.Make(customer, errorIfNull: ErrorCodes.InvalidTransactionId);
        }

        public Result<customer_account> GetCustomerByFreeCoinTransactionId(IDbConnection db, int id)
        {
            string sqlString = @"SELECT ca.* 
                FROM customer_account ca
                JOIN free_coin_transaction ct ON ca.id = ct.customer_account_id
                WHERE ct.id = @id";
            var customer = db.Query<customer_account>(sqlString, new { id }).FirstOrDefault();
            return Result<customer_account>.Make(customer, errorIfNull: ErrorCodes.InvalidTransactionId);
        }

        public Result<customer_account> GetCustomerByGCoinTransactionId(IDbConnection db, int id)
        {
            string sqlString = @"SELECT ca.* 
                FROM customer_account ca
                JOIN gcoin_transaction gt ON ca.id = gt.customer_account_id
                WHERE gt.id = @id";
            var customer = db.Query<customer_account>(sqlString, new { id }).FirstOrDefault();
            return Result<customer_account>.Make(customer, errorIfNull: ErrorCodes.InvalidTransactionId);
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, decimal gcoin)
        {
            string sql = @"UPDATE customer_account 
                SET gcoin = @gcoin
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, gcoin });
        }

        public Result<customer_login_oauth> GetCustomerLoginOauth(IDbConnection db,string service, string identity)
        {
            string sqlString = @"SELECT * 
                FROM customer_login_oauth 
                WHERE lower(service) = @service AND identity= @identity";
            var obj = db.Query<customer_login_oauth>(sqlString, new { service, identity }).FirstOrDefault();
            return Result<customer_login_oauth>.Make(obj, errorIfNull: ErrorCodes.NotFound);
        }
    }
}
