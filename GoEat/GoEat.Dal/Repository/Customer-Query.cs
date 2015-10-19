using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using GoEat.Dal.Models;
using GoEat.Models;
using GoEat.Models;

namespace GoEat.Dal
{
    public partial class GoEatRepo
    {

        public Result<CustomerAccount_ToRemove> GetCustomerById(IDbConnection db, int id)
        {
            var customer = db.Query<CustomerAccount_ToRemove>("SELECT * FROM customer_account WHERE id=@id", new { id }).FirstOrDefault();
            return Result<CustomerAccount_ToRemove>.Make(customer, ErrorCodes.InvalidUserId);
        }

        public Result<CustomerAccount_ToRemove> GetCustomerBySession(IDbConnection db, string session)
        {
            var customer = db.Query<CustomerAccount_ToRemove>("SELECT * FROM customer_account WHERE session=@session", new { session }).FirstOrDefault();
            return Result<CustomerAccount_ToRemove>.Make(customer, ErrorCodes.InvalidSession);
        }

        public Result<CustomerAccount_ToRemove> GetCustomerByUserName(IDbConnection db, string username)
        {
            string sqlString = "SELECT ca.* FROM customer_account AS ca " +
                               "INNER JOIN customer_login_password AS cl ON cl.customer_account_id = ca.id " +
                               "WHERE cl.username=@username";
            var customer = db.Query<CustomerAccount_ToRemove>(sqlString, new { username }).FirstOrDefault();
            return Result<CustomerAccount_ToRemove>.Make(customer, ErrorCodes.InvalidUserName);
        }


        public bool CreateCustomerAccount(IDbConnection db, CustomerAccount_ToRemove customerAccount)
        {
            string sqlString = "INSERT INTO customer_account " +
                                       "(id" +
                                       ",username" +
                                       ",profile" +
                                       ",created_at " +
                                       ",session) " +
                                 "VALUES (@id, @username, @profile, @created_at, @session)";
            return 1 == db.Execute(sqlString, new
            {
                customerAccount.id,
                customerAccount.username,
                customerAccount.profile,
                created_at = customerAccount.created_at.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                customerAccount.session
            });
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
            string sqlString = @"SELECT auth_action.* 
                                       FROM auth_action 
                                       WHERE auth_action.id IN (
                                             SELECT DISTINCT(auth_role_action.action_id) 
                                             FROM auth_role_action 
                                            WHERE auth_role_action.role_id=@id)";
            var authAction = db.Query<AuthAction>(sqlString, new { id }).AsList();
            return Result<List<AuthAction>>.Make(authAction);
        }

        public Result<List<AuthRole>> GetAuthRolesByUsername(IDbConnection db, string username)
        {
            string sqlString = "SELECT auth_role.* " +
                               "FROM auth_role " +
                               "WHERE auth_role.id IN (" +
                                     "SELECT DISTINCT(auth_assignment.role_id) " +
                                     "FROM auth_assignment " +
                                     "JOIN customer_account ON customer_account.id=auth_assignment.customer_account_id " +
                                     "JOIN customer_login_password ON customer_login_password.customer_account_id = customer_account.id " +
                                     "WHERE customer_login_password.username=@username))";
            var authAction = db.Query<AuthRole>(sqlString, new { username }).AsList();
            return Result<List<AuthRole>>.Make(authAction);
        }

        public bool UpdateCustomeAccountProfile(IDbConnection db, int userid, string profile, string session)
        {
            return 1 == db.Execute("UPDATE customer_account SET profile=@profile, session=@session WHERE id=@id", new { profile, id = userid.ToString(), session });
        }

        public Result<Discount> GetCurrentDiscount(IDbConnection db, int user_id, int restaurant_id)
        {
            string sql = "SELECT discount.id, discount.code, discount.user_type, discount.start_date, discount.end_date, discount.rate,user_discount.is_activated  FROM discount" +
                        " INNER JOIN user_discount on user_discount.discount_id = discount.id" +
                        " INNER JOIN restaurant on restaurant.id = discount.restaurant_id" +
                        " WHERE (user_discount.customer_account_id = @id and restaurant.id = @restaurant_id) and DATEDIFF(day,GETDATE(),discount.end_date) >= 0";

            var discount = db.Query<Discount>(sql, new { id = user_id, restaurant_id }).FirstOrDefault();
            return Result<Discount>.Make(discount, ErrorCodes.NotFound);
        }

        public Result<SimpleDiscount> GetSimpleDiscount(IDbConnection db, int user_id, int restaurant_id)
        {
            string sql = "SELECT discount.id, discount.rate FROM discount" +
                        " INNER JOIN user_discount on user_discount.discount_id = discount.id" +
                        " INNER JOIN restaurant on restaurant.id = discount.restaurant_id" +
                        " WHERE (user_discount.customer_account_id = @id and restaurant.id = @restaurant_id) and DATEDIFF(day,GETDATE(),discount.end_date) >= 0";

            var discount = db.Query<SimpleDiscount>(sql, new { id = user_id, restaurant_id }).FirstOrDefault();
            return Result<SimpleDiscount>.Make(discount, ErrorCodes.NotFound);
        }

        /// <summary>
        /// Minus token of customer after using
        /// </summary>
        /// <param name="db">IDBConnection</param>
        /// <param name="userId">customer_id</param>
        /// <param name="minusToken">minus token</param>
        /// <returns>true: minus success / false: error</returns>
        public bool MinusTokenOfCustomer(IDbConnection db, int userId, decimal minusToken)
        {
            return 1 == db.Execute("UPDATE [credit_balance] SET token = token - @minusToken WHERE customer_id= @userId",
                new { minusToken, userId = userId.ToString() });
        }


        public bool AddTokenOfCustomer(IDbConnection db, int userId, decimal addedToken)
        {
            string sql = "IF NOT EXISTS(SELECT * FROM credit_balance WHERE customer_id = @userId) " +
                            "BEGIN " +
                                  "INSERT INTO [credit_balance] ([customer_id], [token]) VALUES (@userId, @addedToken) " +
                            "END " +
                        "ELSE  " +
                            "BEGIN " +
                                "UPDATE [credit_balance] SET token = token + @addedToken WHERE customer_id = @userId " +
                            "END ";
            return 1 == db.Execute(sql, new { addedToken, userId = userId.ToString() });
        }


        public Result<List<AuthRole>> GetAuthRolesByUserId(IDbConnection db, int userid)
        {
            string sqlString = "SELECT auth_role.* " +
                               "FROM auth_role " +
                               "WHERE auth_role.id IN (" +
                                     "SELECT DISTINCT(auth_assignment.role_id) " +
                                     "FROM auth_assignment " +
                                     "JOIN customer_account ON customer_account.id=auth_assignment.customer_account_id " +
                                     "WHERE customer_account.id=@id)";
            var authAction = db.Query<AuthRole>(sqlString, new { id = userid }).AsList();
            return Result<List<AuthRole>>.Make(authAction);
        }

    }
}
