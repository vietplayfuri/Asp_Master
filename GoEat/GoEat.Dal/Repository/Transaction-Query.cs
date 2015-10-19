using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using GoEat.Dal.Models;
using GoEat.Models;
using System;
using System.Text;
using GoEat.Dal;

namespace GoEat.Dal
{
    public partial class GoEatRepo
    {
        /// <summary>
        /// Get all transaction by restanrant id - feature is used by admin of restaurant
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="restaurant_id"></param>
        /// <returns></returns>
        public Result<List<HistoryTransaction>> GetTransactions(IDbConnection db, int startIndex, int endIndex,
            int restaurant_id, int status)
        {
            string sqlQuery = @"SELECT * FROM (
                SELECT ROW_NUMBER() OVER ( ORDER BY main.id DESC) AS RowNum, 
                res.name as restaurant_name,
                main.order_id,
                main.customer_id,
                account.username,
                main.amount, 
                CASE 
					WHEN main.drink_status != 4
					THEN 0
					ELSE main.token_amount
				END AS token_amount,
				main.original_price,
                main.created_date

                FROM [main_transaction] main
                JOIN [customer_account] account on main.customer_id = account.id
                JOIN [restaurant] res on main.restaurant_id = res.id

                WHERE
                    restaurant_id= @restaurant_id
                    AND (food_status = @status or drink_status = @status))
                AS result

                WHERE RowNum >= @index
                AND RowNum <= @to
                ORDER BY RowNum";

            var transactions = db.Query<HistoryTransaction>(sqlQuery,
                new
                {
                    status,
                    index = startIndex,
                    to = endIndex,
                    restaurant_id
                }).AsList();
            
            return Result<List<HistoryTransaction>>.Make(transactions);
        }

        /// <summary>
        /// Get all transactions of specific customer
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="customer_id"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public Result<List<HistoryTransaction>> GetTransactions(IDbConnection db, int startIndex, int endIndex, int customer_id, DateTime fromDate, DateTime toDate)
        {
            string sqlQuery = @"SELECT * FROM (
                SELECT ROW_NUMBER() OVER ( ORDER BY main.id DESC) AS RowNum, 
                res.name as restaurant_name,
                main.order_id,
                main.customer_id,
                account.username,
                main.amount, 
                CASE 
					WHEN main.drink_status != 4
					THEN 0
					ELSE main.token_amount
				END AS token_amount,
				main.original_price,
                main.method,
                main.created_date

                FROM [main_transaction] main
                JOIN [customer_account] account on main.customer_id = account.id
                JOIN [restaurant] res on main.restaurant_id = res.id

                WHERE
                    customer_id= @customer_id
                    AND (food_status = 4 or food_status = 5 or drink_status = 4))
                AS result

                WHERE RowNum >= @index
                AND RowNum <= @to
                AND (DATEDIFF(DAY,@from_date, created_date) >= 0)
                AND (DATEDIFF(DAY,created_date, @to_date) >=0)
                ORDER BY RowNum";

            var transactions = db.Query<HistoryTransaction>(sqlQuery,
                new
                {
                    index = startIndex,
                    to = endIndex,
                    customer_id = customer_id,
                    from_date = fromDate.ToString(ConstantValues.S_DATETIME_FORMAT),
                    to_date = toDate.ToString(ConstantValues.S_DATETIME_FORMAT)
                }).AsList();

            return Result<List<HistoryTransaction>>.Make(transactions);
        }

        /// <summary>
        /// Used for export csv
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="restaurant_id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Result<List<ExportTransaction>> GetTransactions(IDbConnection db, DateTime fromDate, DateTime toDate,
            int restaurant_id, string status)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"SELECT 
                    main.id,
                    res.name as restaurant_name,
                    account.username,
                    main.[order_id],
                    main.food_status,
                    main.amount,
                    main.method,
                    main.food_gtoken_transaction_id as gtoken_transaction_id,
                    main.currency,
                    main.original_currency,
                    main.original_price,
					main.discount_percentage,
					main.gst,
					main.service_charge,
					main.description,
                    main.[created_date]
                        FROM [main_transaction] main
                        JOIN [customer_account] account on main.customer_id = account.id
                        JOIN [restaurant] res on main.restaurant_id = res.id
                WHERE
                    restaurant_id= @restaurant_id");

            TransactionStatus parser = TransactionStatus.Cancelled;
            if (Enum.TryParse<TransactionStatus>(status, out parser))
            {
                query.Append(@" AND (food_status = @parser)");
            }

            query.Append(@"
                AND (DATEDIFF(DAY,@from_date, created_date) >= 0)
                AND (DATEDIFF(DAY,created_date, @to_date) >=0)
                ORDER BY created_date");

            var transactions = db.Query<ExportTransaction>(query.ToString(),
                new
                {
                    from_date = fromDate,
                    to_date = toDate,
                    parser,
                    restaurant_id
                }).AsList();

            return Result<List<ExportTransaction>>.Make(transactions);
        }

        public int CountAdminTransactions(IDbConnection db, int restaurant_id, int status)
        {
            string sql = @"SELECT TOP 1 count(*) OVER()
                FROM [main_transaction]
                WHERE
                    restaurant_id= @restaurant_id
                    AND 
                       (food_status = @status or drink_status = @status)";

            return db.Query<int>(sql, new { restaurant_id, status }).FirstOrDefault();
        }

        public int CountCustomerTransactions(IDbConnection db, int customer_id, DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT TOP 1 count(*) OVER()
                FROM [main_transaction]
                WHERE
                    customer_id = @customer_id
                    AND (food_status = 4 or food_status = 5 or drink_status = 4)
					AND (DATEDIFF(DAY,@from_date, created_date) >= 0)
                    AND (DATEDIFF(DAY,created_date, @to_date) >=0)";

            return db.Query<int>(sql, new
            {
                customer_id,
                from_date = fromDate.ToString(ConstantValues.S_DATETIME_FORMAT),
                to_date = toDate.ToString(ConstantValues.S_DATETIME_FORMAT)
            }).FirstOrDefault();
        }


        /// <summary>
        /// Update gtoken_transaction_id, description in cash table
        /// </summary>
        /// <param name="db">IDbConnection</param>
        /// <param name="cash_transaction_id">cash_transaction_id</param>
        /// <param name="status">status</param>
        /// <returns>true: update success and else</returns>
        public bool UpdateCashTransaction(IDbConnection db, int cash_transaction_id, string description)
        {
            string sql = @"
                UPDATE [dbo].[cash_transaction]
                SET [description] = @description
                WHERE id = @cash_transaction_id";

            return 1 == db.Execute(sql, new { cash_transaction_id, description });
        }

        public int CreateMainTransaction(IDbConnection db, MainTransaction mainTransaction)
        {
            string sql = @"INSERT INTO [dbo].[main_transaction]
                ([cashier_id],[created_date],[method],[restaurant_id],[customer_id])
                VALUES
                ( @cashier_id, @created_date, @method, @restaurant_id,@customer_id)";

            sql += " SELECT CAST(SCOPE_IDENTITY() as int);";
            return db.Query<int>(sql, mainTransaction).FirstOrDefault();
        }

        public int CreatePaypalMainTransaction(IDbConnection db, MainTransaction mainTransaction)
        {
            string sql = @"INSERT INTO [dbo].[main_transaction]
                ([token_transaction_id],[status],[created_date],[amount],[method], [customer_id])
                VALUES
                (@token_transaction_id, @status, @created_date, @amount, @method, @customer_id)";

            sql += " SELECT CAST(SCOPE_IDENTITY() as int);";
            return db.Query<int>(sql, mainTransaction).FirstOrDefault();
        }


        public bool UpdateMainTransactionStatusForFood(IDbConnection db, int main_transaction_id, int status)
        {
            string sql = @"UPDATE [dbo].[main_transaction]
                SET food_status = @status
                WHERE id = @main_transaction_id";

            return 1 == db.Execute(sql, new { main_transaction_id, status });
        }

        public bool UpdateMainTransactionStatusForDrink(IDbConnection db, int main_transaction_id, int status)
        {
            string sql = @"UPDATE [dbo].[main_transaction]
                SET drink_status = @status
                WHERE id = @main_transaction_id";

            return 1 == db.Execute(sql, new { main_transaction_id, status });
        }
        public bool UpdateMainTransactionStatusForFood(IDbConnection db, string order_id, int main_transaction_id, int status)
        {
            string sql = @"UPDATE [dbo].[main_transaction]
                SET food_status = @status, order_id = @order_id
                WHERE id = @main_transaction_id";

            return 1 == db.Execute(sql, new { main_transaction_id, status, order_id });
        }

        public bool UpdateMainTransactionStatusForDrink(IDbConnection db, string order_id, int main_transaction_id, int status)
        {
            string sql = @"UPDATE [dbo].[main_transaction]
                SET drink_status = @status, order_id = @order_id
                WHERE id = @main_transaction_id";

            return 1 == db.Execute(sql, new { main_transaction_id, status, order_id });
        }

        public bool UpdateTokenTransactionStatus(IDbConnection db, int token_transaction_id, string status)
        {
            string sql = @"UPDATE [dbo].[token_transaction]
                SET status = @status
                WHERE id = @token_transaction_id";

            return 1 == db.Execute(sql, new { token_transaction_id, status });
        }

        public Result<ConfirmTransaction> GetConfirmTransactionById(IDbConnection db, int id)
        {
            string sql = @"SELECT 
                main.id,
                main.cash_transaction_id,
                main.token_transaction_id,
                main.customer_id,
                cash.original_price,
                cash.discount_percentage,
                main.drink_status, main.food_status,
                main.restaurant_id,
                main.cashier_id,
                main.order_id,
                --token.amount token_amount,
                cash.amount cash_amount,
                main.method

                FROM [main_transaction] main
                --JOIN [token_transaction] token ON main.token_transaction_id = token.id
                JOIN [cash_transaction] cash ON main.cash_transaction_id = cash.id

                WHERE main.id = @main_transaction_id";

            var tran = db.Query<ConfirmTransaction>(sql, new { main_transaction_id = id }).FirstOrDefault();
            return Result<ConfirmTransaction>.Make(tran);
        }

        public Result<MainTransaction> GetMainTransactionById(IDbConnection db, int id)
        {
            var tran = db.Query<MainTransaction>("SELECT * FROM main_transaction WHERE [id] = @id", new { id }).FirstOrDefault();
            return Result<MainTransaction>.Make(tran);
        }


        public bool CheckExistOrderId(IDbConnection db, string orderId)
        {
            string sql = @"SELECT count(id) FROM main_transaction WHERE [order_id] = @orderId";
            return db.Query<int>(sql, new { orderId }).FirstOrDefault() == 0;
        }

        public Result<ModifyTransaction> GetModifyTransactionById(IDbConnection db, int id)
        {
            string sql = @"SELECT 
                main.id,
                main.customer_id,
                main.restaurant_id,
                main.drink_status,
                main.food_status,
                main.cashier_id,
                main.order_id,
                main.amount cash_amount,
                main.token_amount token_amount,
                main.drinks drinks

                FROM [main_transaction] main

                where main.id = @id";

            var tran = db.Query<ModifyTransaction>(sql, new { id }).FirstOrDefault();
            return Result<ModifyTransaction>.Make(tran);
        }

        public Result<CreditBalance> GetCustomerTotalToken(IDbConnection db, int customer_id)
        {
            string sql = @"SELECT * FROM [credit_balance] WHERE customer_id = @customer_id";

            var credit = db.Query<CreditBalance>(sql, new { customer_id }).FirstOrDefault();
            return Result<CreditBalance>.Make(credit);
        }

        public Result<TokenPackage> GetTokenPackageBySKU(IDbConnection db, string sku)
        {
            string sql = @"SELECT * FROM [token_package] WHERE sku = @sku";

            var package = db.Query<TokenPackage>(sql, new { sku }).FirstOrDefault();
            return Result<TokenPackage>.Make(package);
        }

        public Result<ConfirmTransaction> GetConfirmTokenTransactionById(IDbConnection db, int id)
        {
            string sql = @"SELECT 
                main.id,
                main.cash_transaction_id,
                main.token_transaction_id,
                main.customer_id,
                token.original_price,
                token.discount_percentage,
                main.drink_status, main.food_status,
                main.restaurant_id,
                main.cashier_id,
                main.order_id,
                token.amount token_amount,
                main.method,
                token.paypal_payment_id

                FROM [main_transaction] main
                JOIN [token_transaction] token ON main.token_transaction_id = token.id

                WHERE main.id = @main_transaction_id";

            var tran = db.Query<ConfirmTransaction>(sql, new { main_transaction_id = id }).FirstOrDefault();
            return Result<ConfirmTransaction>.Make(tran);
        }

        public bool UpdateTransOrderId(IDbConnection db, int mainTransactionId, string order_id)
        {
            string sql = @"UPDATE [dbo].[main_transaction]
                SET order_id = @order_id
                WHERE id = @mainTransactionId";

            return 1 == db.Execute(sql, new { mainTransactionId, order_id });
        }

        public bool UpdateTransGTokenTransactionId(IDbConnection db, string sql, int mainTransactionId, string gtoken_transaction_id)
        {
            return 1 == db.Execute(sql, new { mainTransactionId, gtoken_transaction_id });
        }

        public Result<List<ReconcileTransactionModel>> GetReconcileTransactionModel(IDbConnection db,
            int restaurant_id, string status, DateTime fromDate, DateTime toDate)
        {
            string sqlQuery = @"SELECT
                main.id,
                main.created_date
                FROM [main_transaction] main
                WHERE
                restaurant_id= @restaurant_id
                AND status = @status
                AND (DATEDIFF(DAY,@from_date, created_date) >= 0)
                AND (DATEDIFF(DAY,created_date, @to_date) >=0)
                ORDER BY id";

            var transactions = db.Query<ReconcileTransactionModel>(sqlQuery,
                new
                {
                    status,
                    restaurant_id,
                    from_date = fromDate,
                    to_date = toDate
                }).AsList();

            return Result<List<ReconcileTransactionModel>>.Make(transactions);
        }

        public Result<SimpleTransaction> GetSimpleTransactionById(IDbConnection db, int id)
        {
            string sql = @"SELECT 
                main.id,
                ca.username,
                main.amount, 
                main.token_amount,
                main.order_id,
                main.description

                FROM [main_transaction] main
                JOIN [customer_account] ca ON main.customer_id = ca.id

                WHERE main.id = @main_transaction_id";

            var tran = db.Query<SimpleTransaction>(sql, new { main_transaction_id = id }).FirstOrDefault();
            return Result<SimpleTransaction>.Make(tran);
        }

        public bool CreateCashTransaction(IDbConnection db, MainTransaction cashTransaction)
        {
            string sql = @"UPDATE [dbo].[main_transaction]
                SET [order_id]=@order_id, [food_status]=@food_status, [amount] = @amount,[original_currency] = @original_currency,[currency] = @currency,[original_price] = @original_price,[discount_percentage] = @discount_percentage,[gst] = @gst,[service_charge] =@service_charge,[description] = @description
                WHERE id=@id";

            return 1 == db.Query<int>(sql, cashTransaction).FirstOrDefault();
        }

        public bool CreateTokenTransaction(IDbConnection db, MainTransaction transaction)
        {
            string sql = @"UPDATE [dbo].[main_transaction]
                SET [order_id]=@order_id, [drink_status]=@drink_status, [token_amount] = @token_amount,[drinks] = @drinks,[gst] = @gst,[service_charge] =@service_charge,[description] = @description
                WHERE id=@id";

            return 1 == db.Query<int>(sql, transaction).FirstOrDefault();
        }
    }
}
