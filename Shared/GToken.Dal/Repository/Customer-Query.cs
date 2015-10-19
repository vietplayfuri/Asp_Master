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
using Platform.Models;
using Npgsql;


namespace Platform.Dal
{
    public partial class Repo
    {

        public Result<CustomerAccount> GetCustomerById(IDbConnection db, int id)
        {
            var customer = db.Query<CustomerAccount>("SELECT * FROM customer_account WHERE id=@id", new { id }).FirstOrDefault();
            return Result<CustomerAccount>.Make(customer, errorIfNull: ErrorCodes.InvalidUserNameOrPassword);
        }
        public Result<CustomerAccount> GetCustomerByUserName(IDbConnection db, string username)
        {
            string sqlString = "SELECT * FROM customer_account WHERE username = @username";
            var customer = db.Query<CustomerAccount>(sqlString, new { username }).FirstOrDefault();
            return Result<CustomerAccount>.Make(customer, errorIfNull: ErrorCodes.InvalidUserNameOrPassword);
        }

        public Result<CustomerAccount> GetInviterByCustomerUserName(IDbConnection db, string username)
        {
            string sqlString = "SELECT * FROM customer_account WHERE username = (SELECT inviter_username FROM customer_account WHERE username = @username)";
            var customer = db.Query<CustomerAccount>(sqlString, new { username }).FirstOrDefault();
            return Result<CustomerAccount>.Make(customer, errorIfNull: ErrorCodes.InvalidUserNameOrPassword);
        }

        public Result<CustomerAccount> GetCustomerByEmail(IDbConnection db, string email)
        {
            string sqlString = "SELECT * FROM customer_account WHERE email = @email";
            var customer = db.Query<CustomerAccount>(sqlString, new { email }).FirstOrDefault();
            return Result<CustomerAccount>.Make(customer, errorIfNull: ErrorCodes.INVALID_EMAIL);
        }

        public Result<List<CustomerAccount>> GetUserByConditions(IDbConnection db, string conditions)
        {
            string sqlString = string.Format(@"SELECT customer_account.*,partner.Name as partner_name
                                FROM customer_account 
                                LEFT JOIN partner ON partner.identifier = customer_account.partner_identifier
                                WHERE {0};", conditions);
            var customers = db.Query<CustomerAccount>(sqlString).AsList();
            return Result<List<CustomerAccount>>.Make(customers);
        }

        public bool UpdateVenviciMember(IDbConnection db, int customerId)
        {
            string sql = @"UPDATE customer_account SET is_venvici_member = 'true' WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId });
        }

        /// <summary>
        /// Update inviter_username and referred_at only
        /// </summary>
        /// <param name="db"></param>
        /// <param name="customerId"></param>
        /// <param name="inviter_username"></param>
        /// <param name="referred_at"></param>
        /// <returns></returns>
        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string inviter_username, DateTime referred_at, bool isVenviciMemmber)
        {
            string sql = @"UPDATE customer_account 
                SET inviter_username = @inviter_username, referred_at = @referred_at, is_venvici_member = @isVenviciMemmber
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, inviter_username, referred_at, isVenviciMemmber });
        }

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="db"></param>
        /// <param name="customerId"></param>
        /// <param name="inviter_username"></param>
        /// <param name="referred_at"></param>
        /// <returns></returns>
        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string password, string unhashed_password)
        {
            string sql = @"UPDATE customer_account 
                SET password = @password, unhashed_password = @unhashed_password
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, password, unhashed_password });
        }

        /// <summary>
        /// Update string country_code, string country_name, DateTime last_login_at only
        /// </summary>
        /// <param name="db"></param>
        /// <param name="customerId"></param>
        /// <param name="inviter_username"></param>
        /// <param name="referred_at"></param>
        /// <returns></returns>
        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string country_code, string country_name, DateTime last_login_at)
        {
            string sql = @"UPDATE customer_account 
                SET country_code = @country_code, country_name = @country_name, last_login_at = @last_login_at
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, country_code, country_name, last_login_at });
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string email, string nickname, string gender, string bio, string country_code, string country_name, DateTime dob)
        {
            string sql = @"UPDATE customer_account 
                SET email =@email, nickname = @nickname, gender = @gender, bio= @bio,country_code = @country_code, country_name = @country_name, dob=@dob
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, email, nickname, gender, bio, country_code, country_name, dob = dob.Date });
        }

        /// <summary>
        /// insert data into customer_account table --> dob field is not inserted
        /// </summary>
        /// <param name="db"></param>
        /// <param name="customerAccount"></param>
        /// <returns>id of new user</returns>
        //        public int CreateCustomerAccount(NpgsqlConnection db, CustomerAccount customerAccount)
        //        {
        //            string sql = @"INSERT INTO customer_account 
        //            (nickname, email, username, password, gender, vip, country_code, country_name,partner_identifier, unhashed_password) 
        //            VALUES 
        //            (@nickname, @email, @username, @password, @gender, @vip, @country_code, @country_name,@partner_identifier, @unhashed_password)
        //             RETURNING id";

        //            return db.Query<int>(sql, customerAccount).FirstOrDefault();
        //        }

        public int CreateCustomerAccount(IDbConnection db, CustomerAccount customerAccount)
        {
            string sql = @"INSERT INTO customer_account 
            (nickname, email, username, password, gender, vip, country_code, country_name, unhashed_password, partner_identifier) 
            VALUES 
            (@nickname, @email, @username, @password, @gender, @vip, @country_code, @country_name, @unhashed_password, @partner_identifier)
             RETURNING id";

            return db.Query<int>(sql, customerAccount).FirstOrDefault();
        }

        public bool CreateCustomerLoginPassword(IDbConnection db, CustomerLoginPassword customerLoginPassword)
        {
            //string sqlString = "";
            //return 1 == db.Execute(sqlString);
            return true;//for testing
        }

        public bool CheckPassword(IDbConnection db, string username, string old_password)
        {
            string sqlString = @"SELECT COUNT(*) FROM customer_account 
                WHERE 
                    username = @username
                AND
                    unhashed_password = @old_password";

            return db.Query<int>(sqlString, new { username, old_password }).FirstOrDefault() > 0;
        }

        public Result<List<AuthAction>> GetAuthActionsByCustomerId(IDbConnection db, int id)
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
            var authAction = db.Query<AuthAction>(sqlString, new { id }).AsList();
            return Result<List<AuthAction>>.Make(authAction);
        }

        public Result<List<AuthAction>> GetAuthActionsByRoleId(IDbConnection db, int id)
        {
            string sqlString = "SELECT auth_action.* " +
                                       "FROM auth_action " +
                                       "WHERE auth_action.id IN (" +
                                             "SELECT DISTINCT(auth_role_action.action_id) " +
                                             "FROM auth_role_action " +
                                             "WHERE auth_role_action.role_id=@id)";
            var authAction = db.Query<AuthAction>(sqlString, new { id = id }).AsList();
            return Result<List<AuthAction>>.Make(authAction);
        }

        public Result<List<AuthRole>> GetAuthRolesByUserId(IDbConnection db, int userid)
        {
            string sqlString = "SELECT auth_role.* " +
                               "FROM auth_role " +
                               "WHERE auth_role.id IN (" +
                                     "SELECT DISTINCT(auth_assignment.role_id) " +
                                     "FROM auth_assignment " +
                                     "JOIN customer_account ON customer_account.username=auth_assignment.customer_username " +
                                     "WHERE customer_account.id=@id)";
            var authAction = db.Query<AuthRole>(sqlString, new { id = userid }).AsList();
            return Result<List<AuthRole>>.Make(authAction);
        }


        public Result<CustomerAccount> LoadFromAccessToken(IDbConnection db, string partner_identifier,
            string token, string username = null)
        {
            StringBuilder conditions = new StringBuilder();
            if (!string.IsNullOrEmpty(partner_identifier))
            {
                conditions.Append("access_token.partner_identifier = @partner_identifier");
            }
            else
            {
                return Result<CustomerAccount>.Null();
            }
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(token))
            {
                return Result<CustomerAccount>.Null();
            }

            if (!string.IsNullOrEmpty(username))
            {
                if (conditions.ToString().Length > 0)
                {
                    conditions.Append(" AND ");
                }
                conditions.Append("access_token.customer_username = @username");
            }
            
            if (!string.IsNullOrEmpty(token))
            {
                if (conditions.ToString().Length > 0)
                {
                    conditions.Append(" AND ");
                }
                conditions.Append("access_token.token = @token");
            }

            if (conditions.ToString().Length > 0)
            {
                string sqlString = @"SELECT customer_account.* FROM access_token  
                                              INNER JOIN customer_account ON access_token.customer_username = customer_account.username 
                                                WHERE " + conditions.ToString();
                var partner = db.Query<CustomerAccount>(sqlString, new { username, partner_identifier, token }).FirstOrDefault();
                return Result<CustomerAccount>.Make(partner);
            }
            return Result<CustomerAccount>.Null();
        }

        public Result<Partner> GetPartnerById(IDbConnection db, string uid)
        {
            string sqlString = @"SELECT * FROM partner WHERE uid = @uid";
            var partner = db.Query<Partner>(sqlString, new { uid }).FirstOrDefault();
            return Result<Partner>.Make(partner);
        }

        public Result<Partner> GetPartnerByIdentifier(IDbConnection db, string identifier)
        {
            string sqlString = @"SELECT * FROM partner WHERE identifier = @identifier";
            var partner = db.Query<Partner>(sqlString, new { identifier }).FirstOrDefault();
            return Result<Partner>.Make(partner);
        }

        public Result<CustomerLoginOAuth> GetCustomerLoginOAuthByUsername(IDbConnection db, string service, string username)
        {
            string sqlString = @"SELECT *  
                                 FROM customer_login_oauth 
                                 WHERE lower(service) =@service and customer_username=@username";
            var CusLoginOAuth = db.Query<CustomerLoginOAuth>(sqlString, new { service, username }).FirstOrDefault();
            return Result<CustomerLoginOAuth>.Make(CusLoginOAuth);
        }


        public Result<CustomerLoginOAuth> GetCustomerLoginOAuthByIdentity(IDbConnection db, string service, string identity)
        {
            string sqlString = @"SELECT *  
                                 FROM customer_login_oauth 
                                 WHERE lower(service) =@service and identity=@identity";
            var CusLoginOAuth = db.Query<CustomerLoginOAuth>(sqlString, new { service, identity }).FirstOrDefault();
            return Result<CustomerLoginOAuth>.Make(CusLoginOAuth);
        }

        public Result<CustomerLoginOAuth> GetCustomerLoginOAuth(IDbConnection db, string identity)
        {
            string sqlString = @"SELECT *  
                                 FROM customer_login_oauth 
                                 WHERE identity = @identity";
            var CusLoginOAuth = db.Query<CustomerLoginOAuth>(sqlString, new { identity }).FirstOrDefault();
            return Result<CustomerLoginOAuth>.Make(CusLoginOAuth);
        }

        public int DeleteGetCustomerLoginOAuthById(IDbConnection db, int id)
        {
            string sqlString = @"DELETE FROM customer_login_oauth 
                                 WHERE id =@id";
            return db.Query<int>(sqlString, new { id }).FirstOrDefault();
        }

        public Result<AccessToken> GetAccessToken(IDbConnection db, string identifier, string customerUsername)
        {
            string sqlString = @"SELECT * FROM access_token
                WHERE 
                partner_identifier=@identifier
                AND
                customer_username = @customerUsername";
            var accessToken = db.Query<AccessToken>(sqlString, new { identifier, customerUsername }).FirstOrDefault();
            return Result<AccessToken>.Make(accessToken);
        }

        public int CreateAccessToken(IDbConnection db, AccessToken accessToken)
        {
            string sql = @"INSERT INTO access_token 
            (customer_username, partner_identifier, token, saved_at) 
            VALUES 
            (@customer_username, @partner_identifier, @token, @saved_at)";

            return db.Query<int>(sql, accessToken).FirstOrDefault();
        }

        public int UpdateAccessToken(IDbConnection db, string identifier, string username, string token)
        {
            string sql = @"UPDATE access_token 
                SET token = @token
                WHERE 
                partner_identifier=@identifier
                AND
                customer_username = @username";

            return db.Query<int>(sql, new { identifier, username, token }).FirstOrDefault();
        }

        public bool SetUserLocale(IDbConnection db, int userId, string locale)
        {
            string sql = @"UPDATE customer_account 
                SET locale = @locale
                WHERE 
                id = @id";

            return 1 == db.Query<int>(sql, new { id = userId, locale = locale }).FirstOrDefault();
        }

        public int CreateCustomerLoginOAuth(IDbConnection db, CustomerLoginOAuth customerLoginOauth)
        {
            string sql = @"INSERT INTO customer_login_oauth
                (service, identity, customer_username)
                VALUES
                (@service, @identity, @customer_username)
                 RETURNING id";

            return db.Query<int>(sql, customerLoginOauth).FirstOrDefault();
        }

        public Result<List<VerificationToken>> GetValidVerificationToken(IDbConnection db, string customerUsername)
        {
            string sqlString = @"SELECT *
                FROM verification_token
                WHERE 
                    customer_username = @customerUsername
                AND 
                    is_valid = 't'";
            var verificationToken = db.Query<VerificationToken>(sqlString, new { customerUsername }).AsList();
            return Result<List<VerificationToken>>.Make(verificationToken);
        }
        public Result<VerificationToken> GetValidVerificationTokenByCode(IDbConnection db, string code)
        {
            string sqlString = @"SELECT *
                FROM verification_token
                WHERE 
                    code = @code";
            var verificationToken = db.Query<VerificationToken>(sqlString, new { code }).FirstOrDefault();
            return Result<VerificationToken>.Make(verificationToken);
        }

        public bool UpdateVerificationStatus(IDbConnection db, string customerUsername, bool is_valid)
        {
            string sqlString = @"UPDATE verification_token
                SET is_valid = @is_valid                
                WHERE 
                    customer_username = @customerUsername";

            return 1 == db.Execute(sqlString, new { customerUsername, is_valid });
        }

        /// <summary>
        /// Create VerificationToken with default validation_time
        /// </summary>
        /// <param name="db"></param>
        /// <param name="customerUsername"></param>
        /// <param name="is_valid"></param>
        /// <returns></returns>
        public bool CreateVerificationToken(IDbConnection db, VerificationToken token)
        {
            string sqlString = @"INSERT INTO verification_token
                (customer_username, code, is_valid)
                VALUES 
                (@customer_username, @code, @is_valid)";
            return 1 == db.Execute(sqlString, new { token.customer_username, token.code, token.is_valid });
        }

        public bool UpdateUserPassword(IDbConnection db, int userId, string password, string unhashed_password)
        {
            string sql = @"UPDATE customer_account 
                SET password = @password, unhashed_password = @unhashed_password
                WHERE 
                id = @userId";

            return 1 == db.Execute(sql, new { userId, password, unhashed_password });
        }

        public bool UpdateCustomerAccount(IDbConnection db, int customerId, string avatar_filename)
        {
            DateTime last_login_at = DateTime.UtcNow;
            string sql = @"UPDATE customer_account 
                SET avatar_filename = @avatar_filename, last_login_at = @last_login_at
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, avatar_filename, last_login_at });
        }


        public bool UpdateCustomerAccount(IDbConnection db, int customerId, decimal gtoken)
        {
            string sql = @"UPDATE customer_account 
                SET gtoken = @gtoken
                WHERE id = @customerId";

            return 1 == db.Execute(sql, new { customerId, gtoken });
        }

        public Result<List<AuthRole>> GetAllAuthRoles(IDbConnection db)
        {
            string sqlString = @"SELECT * 
                FROM auth_role";
            var authAction = db.Query<AuthRole>(sqlString).AsList();
            return Result<List<AuthRole>>.Make(authAction);
        }


        public int GetIdByUsername(IDbConnection db, string username)
        {
            var id = db.Query<int>("SELECT id FROM customer_account WHERE username = @username", new { username }).FirstOrDefault();
            return id;
        }
    }
}
