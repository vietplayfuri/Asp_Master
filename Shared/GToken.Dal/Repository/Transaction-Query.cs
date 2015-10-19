using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Platform.Models;
using Platform.Models.Models;
using Npgsql;
using System;
using System.Text;
using Platform.Utility;
namespace Platform.Dal
{
    public partial class Repo
    {
        public Result<List<CreditTransaction>> GetUnFullfilledExchanges(IDbConnection db, int gameId, int userId)
        {
            var pending = ConstantValues.S_PENDING;
            var trans = db.Query<CreditTransaction>("SELECT * FROM credit_transaction WHERE game_id=@gameId AND customer_account_id=@userId AND status=@pending", new { gameId, userId, pending }).AsList();
            return Result<List<CreditTransaction>>.Make(trans);
        }

        public Result<CoinTransaction> GetCoinTransaction(IDbConnection db, string orderId, TransactionStatus? statusFilter = null)
        {
            var trans = db.Query<CoinTransaction>("SELECT * FROM coin_transaction WHERE order_id=@orderId", new { orderId }).FirstOrDefault();
            if (trans == null)
                return Result<CoinTransaction>.Null(ErrorCodes.InvalidTransactionId);

            if (statusFilter.HasValue)
            {
                if (statusFilter.Value.ToString() != trans.status)
                {
                    bool accepted = (trans.status == ConstantValues.S_ACCEPTED);
                    return Result<CoinTransaction>.Null(accepted ? ErrorCodes.TransactionAlreadyProcessed : ErrorCodes.TransactionNotFound);
                }
            }

            // TODO: Need a collection data structure in CoinTransaction to hold packages //
            return Result<CoinTransaction>.Make(trans);
        }

        public Result<FreeCoinTransaction> GetFreeCoinTransaction(IDbConnection db, string orderId, TransactionStatus? statusFilter = null)
        {
            var trans = db.Query<FreeCoinTransaction>("SELECT * FROM free_coin_transaction WHERE order_id=@orderId", new { orderId }).FirstOrDefault();
            if (trans == null)
                return Result<FreeCoinTransaction>.Null(ErrorCodes.InvalidTransactionId);

            if (statusFilter.HasValue)
            {
                if (statusFilter.Value.ToString() != trans.status)
                {
                    bool accepted = (trans.status == ConstantValues.S_ACCEPTED);
                    return Result<FreeCoinTransaction>.Null(accepted ? ErrorCodes.TransactionAlreadyProcessed : ErrorCodes.TransactionNotFound);
                }
            }

            // TODO: Need a collection data structure in FreeCoinTransaction to hold packages //
            return Result<FreeCoinTransaction>.Make(trans);
        }

        // TODO: Incomplete & Not Tested //
        public Result<List<CreditTransaction>> GetExchange(IDbConnection db, string orderId, TransactionStatus? statusFilter = null)
        {
            var coinTran = GetCoinTransaction(db, orderId, statusFilter);
            if (coinTran.HasData)
            {
                // TODO: Full up the list of packages IAP items //
                //Result< List<CreditTransaction> >.Make(coinTran.Data, coinTran.Error);
            }
            // Else, not succeeded //

            var freeCoinTran = GetFreeCoinTransaction(db, orderId, statusFilter);
            if (freeCoinTran.HasData)
            {
                // TODO: Full up the list of packages IAP items //
                // return Result< List<CreditTransaction> >.Make(freeCoinTran.Data, freeCoinTran.Error);
            }

            // TEST //
            return Result<List<CreditTransaction>>.Null();
        }

        public Result<List<CreditTransaction>> SetExchangeStatus(IDbConnection db, string orderId, TransactionStatus newStatus)
        {
            // TODO
            // Step 1: Find and Update CoinTransaction 's status to "failure"
            // Step 2: If not found, try on FreeCoinTransaction. Status to "failure"

            // Step 3: CreditTransaction status also mark as "failure"




            // TEST //
            return Result<List<CreditTransaction>>.Null();
        }

        public Result<Transaction> GetTransaction(IDbConnection db, string gtoken_transaction_id)
        {
            string sqlString = @"SELECT * FROM transaction WHERE gtoken_transaction_id = @gtoken_transaction_id";
            var transaction = db.Query<Transaction>(sqlString, new { gtoken_transaction_id }).FirstOrDefault();
            return Result<Transaction>.Make(transaction, errorIfNull: ErrorCodes.InvalidTransactionId);
        }

        public Result<Transaction> GetTransaction(IDbConnection db, string partner_identifier, string orderId = null)
        {
            string sqlString = @"SELECT * FROM transaction 
                WHERE 
                partner_order_id = @orderId
                AND
                partner_identifier = @partner_identifier";
            var transaction = db.Query<Transaction>(sqlString, new { orderId, partner_identifier }).FirstOrDefault();
            return Result<Transaction>.Make(transaction, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        public bool UpdateTransactionStatus(IDbConnection db, long transactionId, string status)
        {
            string sql = @"UPDATE transaction SET status = @status WHERE id = @transactionId";

            return 1 == db.Execute(sql, new { transactionId, status });
        }

        public int CreateTransaction(IDbConnection db, Transaction trans)
        {
            string sql = @"INSERT INTO transaction(
                                    gtoken_transaction_id, price, status, description, payment_method, 
                                    ip_address, country_code, partner_identifier, 
                                    customer_username, discount_percentage, final_amount, currency, 
                                    original_price, original_final_amount, original_currency, partner_order_id, 
                                    exchange_rate, is_venvici_applicable, original_tax, original_service_charge, 
                                    tax, service_charge, revenue_percentage)
                            VALUES (@gtoken_transaction_id, @price, @status, @description, @payment_method, 
                                    @ip_address, @country_code, @partner_identifier, 
                                    @customer_username, @discount_percentage, @final_amount, @currency, 
                                    @original_price, @original_final_amount, @original_currency, @partner_order_id, 
                                    @exchange_rate, @is_venvici_applicable, @original_tax, @original_service_charge, 
                                    @tax, @service_charge, @revenue_percentage)
                                     RETURNING id";

            return db.Query<int>(sql, trans).FirstOrDefault();
        }

        public int CreateTokenTransaction(IDbConnection db, TokenTransaction tokenTran)
        {
            string sql = @"INSERT INTO token_transaction(
            customer_username, partner_identifier, gtoken_transaction_id, 
            partner_order_id, token_type, transaction_type, amount, description, 
            ip_address, country_code, created_at, updated_at, tax, service_charge)
            VALUES (
            @customer_username, @partner_identifier, @gtoken_transaction_id, 
            @partner_order_id, @token_type, @transaction_type, @amount, @description, 
            @ip_address, @country_code, @created_at, @updated_at, @tax, @service_charge)
            RETURNING id";

            return db.Query<int>(sql, tokenTran).FirstOrDefault();
        }

        public int CreateDirectGTokenTransaction(IDbConnection db, DirectGtokenTransaction tokenTran)
        {
            string sql = @"INSERT INTO direct_gtoken_transaction
                (gtoken_transaction_id, partner_order_id, customer_username, 
                description, ip_address, country_code, created_at, updated_at, 
                amount, partner_identifier)
            VALUES
                (@gtoken_transaction_id, @partner_order_id, @customer_username, 
                @description, @ip_address, @country_code, @created_at, @updated_at, 
                @amount, @partner_identifier)
            RETURNING id";

            return db.Query<int>(sql, tokenTran).FirstOrDefault();
        }

        public Result<DirectGtokenTransaction> GetDirectGTokenTransaction(IDbConnection db, string partner_identifier, string orderId = null)
        {
            string sqlString = @"SELECT * 
                FROM direct_gtoken_transaction 
                WHERE 
                    partner_order_id = @orderId
                AND partner_identifier = @partner_identifier";
            var transaction = db.Query<DirectGtokenTransaction>(sqlString, new { orderId, partner_identifier }).FirstOrDefault();
            return Result<DirectGtokenTransaction>.Make(transaction, errorIfNull: ErrorCodes.NotFound);
        }

        public Result<Transaction> GetTransactionById(IDbConnection db, int id)
        {
            string sqlString = @"SELECT * FROM transaction 
                WHERE 
                id = @id;";
            var transaction = db.Query<Transaction>(sqlString, new { id }).FirstOrDefault();
            return Result<Transaction>.Make(transaction);
        }

        public Result<TokenTransaction> GetTokenTransactionById(IDbConnection db, int id)
        {
            string sqlString = @"SELECT * FROM token_transaction 
                WHERE 
                id = @id;";
            var transaction = db.Query<TokenTransaction>(sqlString, new { id }).FirstOrDefault();
            return Result<TokenTransaction>.Make(transaction);
        }

        public Result<List<Transaction>> GetTransactions(IDbConnection db, string username, string status)
        {
            string sqlString = @"SELECT * FROM transaction 
                WHERE 
                    customer_username = @username AND status=@status order by created_at desc;";
            var transaction = db.Query<Transaction>(sqlString, new { username, status }).ToList();
            return Result<List<Transaction>>.Make(transaction);
        }

        public Result<List<TokenTransaction>> GetTokenTransactions(IDbConnection db, string username, string status)
        {
            string sqlString = @"SELECT * FROM token_transaction 
                WHERE 
                customer_username = @username AND status=@status;";
            var transaction = db.Query<TokenTransaction>(sqlString, new { username, status }).ToList<TokenTransaction>();
            return Result<List<TokenTransaction>>.Make(transaction);
        }
        public Result<TokenTransaction> GetTokenTransaction(IDbConnection db, string gtoken_transaction_id)
        {
            string sqlString = @"SELECT * FROM token_transaction WHERE gtoken_transaction_id = @gtoken_transaction_id";
            var transaction = db.Query<TokenTransaction>(sqlString, new { gtoken_transaction_id }).FirstOrDefault();
            return Result<TokenTransaction>.Make(transaction, errorIfNull: ErrorCodes.InvalidTransactionId);
        }


        public Result<DirectGtokenTransaction> GetDirectGTokenTransaction(IDbConnection db, string gtoken_transaction_id)
        {
            string sqlString = @"SELECT * FROM direct_gtoken_transaction WHERE gtoken_transaction_id = @gtoken_transaction_id";
            var transaction = db.Query<DirectGtokenTransaction>(sqlString, new { gtoken_transaction_id }).FirstOrDefault();
            return Result<DirectGtokenTransaction>.Make(transaction, errorIfNull: ErrorCodes.TransactionNotFound);
        }

        public Result<DirectGtokenTransaction> GetDirectGTokenTransaction(IDbConnection db, int id)
        {
            string sqlString = @"SELECT * FROM direct_gtoken_transaction WHERE id = @id";
            var transaction = db.Query<DirectGtokenTransaction>(sqlString, new { id }).FirstOrDefault();
            return Result<DirectGtokenTransaction>.Make(transaction, errorIfNull: ErrorCodes.InvalidTransactionId);
        }


        public Result<List<MainTransaction>> GetTokenTransactionsByCustomQuery(IDbConnection db, string sqlQuery)
        {
            var transaction = db.Query<MainTransaction>(sqlQuery).ToList();
            return Result<List<MainTransaction>>.Make(transaction, errorIfNull: ErrorCodes.InvalidTransactionId);
        }

        public Result<List<MainTransaction>> GetAllTransactions(IDbConnection db, int fromIndex, int toIndex, string username, params string[] status)
        {
            string sttCondition = string.Join("','", status);
            string sqlString = @"SELECT RowNum, p.name as partner_name, created_at, customer_username, gtoken_transaction_id, status, description, original_final_amount, original_currency, amount, token_type, transaction_type
                                FROM (
	                                SELECT ROW_NUMBER() OVER (ORDER BY created_at DESC) AS  RowNum,
		                                id, partner_identifier, created_at, customer_username, gtoken_transaction_id, status, description, original_final_amount, original_currency, amount, token_type, transaction_type
	                                FROM(
		                                SELECT id, partner_identifier, created_at, customer_username, gtoken_transaction_id, status, description, original_final_amount, original_currency, NULL as amount, NULL as token_type, NULL as transaction_type 
		                                from transaction 
		                                WHERE customer_username = @username AND status IN ('" + sttCondition + "') " +
                                        @"UNION ALL 
		                                SELECT id, partner_identifier, created_at, customer_username, gtoken_transaction_id,NULL as status ,description,NULL as original_final_amount, NULL as original_currency, amount, token_type, transaction_type
		                                 from token_transaction
		                                WHERE customer_username = @username
	                                )results
                                )AS results2
                                INNER JOIN partner as p on p.identifier = partner_identifier
                                WHERE RowNum BETWEEN @fromIndex AND @toIndex 
                                ORDER BY created_at DESC";
            var transaction = db.Query<MainTransaction>(sqlString, new { fromIndex, toIndex, username }).ToList();
            return Result<List<MainTransaction>>.Make(transaction);
        }

        public int CountAllTransactionRows(IDbConnection db, string username, string status)
        {
            string sqlString = @"SELECT count(id)
	                            FROM(
		                            SELECT id, created_at
		                            from transaction 
		                            WHERE customer_username = @username AND status=@status
		                            UNION ALL 
		                            SELECT id, created_at
		                             from token_transaction
		                            WHERE customer_username = @username
	                            )results";
            return db.Query<int>(sqlString, new { username, status }).SingleOrDefault();

        }

        public Result<List<CustomTransaction>> GetTransactionsByQueryString(IDbConnection db,
            string startTime = null, string endTime = null, string partner_identifier = null, string status = null,
            string partner_order_id = null, string gtoken_transaction_id = null, string timeZone = null, string username = null)
        {
            var conditions = new List<string>();
            DateTime dtStartTime = DateTime.UtcNow;
            DateTime dtEndTime = DateTime.UtcNow;
            if (startTime != null)
            {
                dtStartTime = Helper.timeFromString(startTime, timeZone);
                conditions.Add("transaction.created_at >= @dtStartTime");
            }

            if (endTime != null)
            {
                dtEndTime = Helper.timeFromString(endTime, timeZone);
                conditions.Add("transaction.created_at <= @dtEndTime");
            }

            if (!string.IsNullOrEmpty(partner_identifier))
            {
                conditions.Add("transaction.partner_identifier = @partner_identifier");
            }

            if (!string.IsNullOrEmpty(status))
            {
                conditions.Add("transaction.status =@status");
            }

            if (!string.IsNullOrEmpty(partner_order_id))
            {
                conditions.Add("transaction.partner_order_id = @partner_order_id");
            }

            if (!string.IsNullOrEmpty(gtoken_transaction_id))
            {
                conditions.Add("transaction.gtoken_transaction_id = @gtoken_transaction_id");
            }

            StringBuilder condition = new StringBuilder();
            StringBuilder query = new StringBuilder();

            query.AppendLine(@"SELECT transaction.*, partner.name as partner_name 
                            FROM transaction
                            JOIN partner on partner.identifier = transaction.partner_identifier ");

            string termUsername = string.Empty;
            if (!string.IsNullOrEmpty(username))
            {
                string encodeForLike = string.IsNullOrEmpty(username)
                ? string.Empty
                : username.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
                termUsername = "%" + encodeForLike + "%";

                condition.AppendLine(@"  (lower(customer_account.email) like @termUsername");
                condition.AppendLine(@"OR lower(customer_account.nickname) like @termUsername");
                condition.AppendLine(@"OR lower(customer_account.username) like @termUsername)");
                conditions.Add(condition.ToString());

                query.AppendLine("JOIN customer_account on customer_account.username = transaction.customer_username");
            }

            if (conditions.Count > 0)
            {
                query.AppendLine("WHERE ");
                query.AppendLine(string.Join(" AND ", conditions.ToArray()));
            }

            string customeQuery = query.ToString();

            var transaction = db.Query<CustomTransaction>(customeQuery, new
            {
                dtStartTime,
                dtEndTime,
                partner_identifier,
                status,
                partner_order_id,
                gtoken_transaction_id,
                termUsername
            }).ToList();
            return Result<List<CustomTransaction>>.Make(transaction);
        }

        public Result<List<Transaction>> GetTransactions(IDbConnection db, string username, List<string> lstStatus)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"SELECT * FROM transaction 
                WHERE 
                    customer_username = @username ");
            if (lstStatus != null && lstStatus.Any())
            {
                strBuilder.Append(@"AND (");
                for (int i = 0; i < lstStatus.Count(); i++)
                {
                    strBuilder.Append(@"status = '");
                    strBuilder.Append(lstStatus[i]);
                    strBuilder.Append(@"' ");

                    if (i != lstStatus.Count() - 1)
                    {
                        strBuilder.Append(@"OR ");
                    }
                    else
                    {
                        strBuilder.Append(@") ");
                    }
                }
            }
            strBuilder.Append(@"ORDER BY created_at DESC");

            var transaction = db.Query<Transaction>(strBuilder.ToString(), new { username }).ToList();
            return Result<List<Transaction>>.Make(transaction);
        }

        public Result<List<Partner>> GetAllPartners(IDbConnection db)
        {
            string queryString = @"SELECT * FROM partner";

            var partner = db.Query<Partner>(queryString).ToList();
            return Result<List<Partner>>.Make(partner);
        }

        public int CreateVenviciTransaction(IDbConnection db, VenviciTransaction venTrans)
        {
            string sql = @"INSERT INTO venvici_transaction(
                            transaction_id, customer_username, gtoken_deduct_amount, 
                        commission_credit_amount, pushbv_amount, gtoken_add_amount, remark)
                        VALUES (@transaction_id, @customer_username, @gtoken_deduct_amount, 
                        @commission_credit_amount, @pushbv_amount,@gtoken_add_amount, @remark)
                        RETURNING id;";
            return db.Query<int>(sql, venTrans).FirstOrDefault();
        }



        public Result<List<DirectTransaction>> GetDirectGTokenTransactionByQueryString(IDbConnection db,
            string startTime = null, string endTime = null, string partner_identifier = null, string status = null,
            string partner_order_id = null, string gtoken_transaction_id = null, string timeZone = null, string username = null)
        {
            var conditions = new List<string>();
            DateTime dtStartTime = DateTime.UtcNow;
            DateTime dtEndTime = DateTime.UtcNow;
            if (startTime != null)
            {
                dtStartTime = Helper.timeFromString(startTime, timeZone);
                conditions.Add("direct_gtoken_transaction.created_at >= @dtStartTime");
            }

            if (endTime != null)
            {
                dtEndTime = Helper.timeFromString(endTime, timeZone);
                conditions.Add("direct_gtoken_transaction.created_at <= @dtEndTime");
            }

            if (!string.IsNullOrEmpty(partner_identifier))
            {
                conditions.Add("direct_gtoken_transaction.partner_identifier = @partner_identifier");
            }

            if (!string.IsNullOrEmpty(partner_order_id))
            {
                conditions.Add("direct_gtoken_transaction.partner_order_id = @partner_order_id");
            }

            if (!string.IsNullOrEmpty(gtoken_transaction_id))
            {
                conditions.Add("direct_gtoken_transaction.gtoken_transaction_id = @gtoken_transaction_id");
            }

            StringBuilder condition = new StringBuilder();
            StringBuilder query = new StringBuilder();

            query.AppendLine(@"SELECT direct_gtoken_transaction.*, partner.name as partner_name 
                            FROM direct_gtoken_transaction
                            JOIN partner on partner.identifier = direct_gtoken_transaction.partner_identifier ");

            string termUsername = string.Empty;
            if (!string.IsNullOrEmpty(username))
            {
                string encodeForLike = string.IsNullOrEmpty(username)
                ? string.Empty
                : username.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
                termUsername = "%" + encodeForLike + "%";

                condition.AppendLine(@"  (lower(customer_account.email) like @termUsername");
                condition.AppendLine(@"OR lower(customer_account.nickname) like @termUsername");
                condition.AppendLine(@"OR lower(customer_account.username) like @termUsername)");
                conditions.Add(condition.ToString());

                query.AppendLine("JOIN customer_account on customer_account.username = direct_gtoken_transaction.customer_username");
            }

            if (conditions.Count > 0)
            {
                query.AppendLine("WHERE ");
                query.AppendLine(string.Join(" AND ", conditions.ToArray()));
            }

            string customeQuery = query.ToString();

            var transaction = db.Query<DirectTransaction>(customeQuery, new
            {
                dtStartTime,
                dtEndTime,
                partner_identifier,
                status,
                partner_order_id,
                gtoken_transaction_id,
                termUsername
            }).ToList();
            return Result<List<DirectTransaction>>.Make(transaction);
        }
    }
}
