using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Platform.Models;
using GoPlay.Models;
using System.Text;
using GoPlay.Models.Models;

namespace GoPlay.Dal
{

    public partial class Repo
    {

        public Result<List<credit_transaction>> GetUnFullfilledExchanges(IDbConnection db, int gameId, int userId)
        {
            var pending = ConstantValues.S_PENDING;
            var trans = db.Query<credit_transaction>("SELECT * FROM credit_transaction WHERE game_id=@gameId AND customer_account_id=@userId AND status=@pending", new { gameId, userId, pending }).AsList();
            return Result<List<credit_transaction>>.Make(trans);
        }

        public Result<coin_transaction> GetCoinTransaction(IDbConnection db, string orderId, TransactionStatus? statusFilter = null)
        {
            var trans = db.Query<coin_transaction>("SELECT * FROM coin_transaction WHERE order_id=@orderId", new { orderId }).FirstOrDefault();
            if (trans == null)
                return Result<coin_transaction>.Null(ErrorCodes.InvalidTransactionId);

            if (statusFilter.HasValue)
            {
                if (statusFilter.Value.ToString() != trans.status)
                {
                    bool accepted = (trans.status == ConstantValues.S_ACCEPTED);
                    return Result<coin_transaction>.Null(accepted ? ErrorCodes.TransactionAlreadyProcessed : ErrorCodes.TransactionNotFound);
                }
            }

            // TODO: Need a collection data structure in CoinTransaction to hold packages //
            return Result<coin_transaction>.Make(trans);
        }

        public Result<free_coin_transaction> GetFreeCoinTransaction(IDbConnection db, string orderId, TransactionStatus? statusFilter = null)
        {
            var trans = db.Query<free_coin_transaction>("SELECT * FROM free_coin_transaction WHERE order_id=@orderId", new { orderId }).FirstOrDefault();
            if (trans == null)
                return Result<free_coin_transaction>.Null(ErrorCodes.InvalidTransactionId);

            if (statusFilter.HasValue)
            {
                if (statusFilter.Value.ToString() != trans.status)
                {
                    bool accepted = (trans.status == ConstantValues.S_ACCEPTED);
                    return Result<free_coin_transaction>.Null(accepted ? ErrorCodes.TransactionAlreadyProcessed : ErrorCodes.TransactionNotFound);
                }
            }

            // TODO: Need a collection data structure in FreeCoinTransaction to hold packages //
            return Result<free_coin_transaction>.Make(trans);
        }

        // TODO: Incomplete & Not Tested //
        public Result<List<credit_transaction>> GetExchange(IDbConnection db, string orderId, TransactionStatus? statusFilter = null)
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
            return Result<List<credit_transaction>>.Null();
        }

        public Result<List<credit_transaction>> SetExchangeStatus(IDbConnection db, string orderId, TransactionStatus newStatus)
        {
            // TODO
            // Step 1: Find and Update CoinTransaction 's status to "failure"
            // Step 2: If not found, try on FreeCoinTransaction. Status to "failure"

            // Step 3: CreditTransaction status also mark as "failure"




            // TEST //
            return Result<List<credit_transaction>>.Null();
        }

        public int CreateFreeCoinTransaction(IDbConnection db, free_coin_transaction freeCoin)
        {
            string sql = @"INSERT INTO free_coin_transaction(
            customer_account_id, amount, game_id, credit_type_id, package_id, 
            created_at, description, status, order_id, topup_card_id, payment_method, 
            price, ip_address, country_code)
            VALUES (
            @customer_account_id, @amount, @game_id, @credit_type_id, @package_id, 
            @created_at, @description, @status, @order_id, @topup_card_id, @payment_method, 
            @price, @ip_address, @country_code)
             RETURNING id";
            return db.Query<int>(sql, freeCoin).FirstOrDefault();
        }

        public Result<coin_transaction> GetCoinTransaction(IDbConnection db, string order_id)
        {
            string stringSql = @"SELECT * FROM coin_transaction WHERE order_id = @order_id";
            var coin = db.Query<coin_transaction>(stringSql, new { order_id }).FirstOrDefault();
            return Result<coin_transaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        public Result<List<coin_transaction>> GetCoinTransactionsByCustomQuery(IDbConnection db, string queryString)
        {
            var coin = db.Query<coin_transaction>(queryString).ToList();
            return Result<List<coin_transaction>>.Make(coin, errorIfNull: ErrorCodes.ServerError);
        }

        public Result<List<gcoin_transaction>> GetGCoinTransactionsByCustomQuery(IDbConnection db, string queryString)
        {
            var coin = db.Query<gcoin_transaction>(queryString).ToList();
            return Result<List<gcoin_transaction>>.Make(coin, errorIfNull: ErrorCodes.ServerError);
        }

        public Result<List<free_coin_transaction>> GetFreeCoinTransactionsByCustomQuery(IDbConnection db, string queryString)
        {
            var coin = db.Query<free_coin_transaction>(queryString).ToList();
            return Result<List<free_coin_transaction>>.Make(coin, errorIfNull: ErrorCodes.ServerError);
        }

        public Result<coin_transaction> GetCoinTransactionByPaypalPaymentId(IDbConnection db, string paypal_payment_id)
        {
            string stringSql = @"SELECT * FROM coin_transaction WHERE paypal_payment_id = @paypal_payment_id";
            var coin = db.Query<coin_transaction>(stringSql, new { paypal_payment_id }).FirstOrDefault();
            return Result<coin_transaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        public Result<free_coin_transaction> GetFreeCoinTransaction(IDbConnection db, string orderId)
        {
            string stringSql = @"SELECT * FROM free_coin_transaction WHERE order_id = @orderId";
            var coin = db.Query<free_coin_transaction>(stringSql, new { orderId }).FirstOrDefault();
            return Result<free_coin_transaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }


        public Result<free_coin_transaction> GetFreeCoinTransaction(IDbConnection db, int transactionId)
        {
            string stringSql = @"SELECT * FROM free_coin_transaction WHERE id = @transactionId";
            var coin = db.Query<free_coin_transaction>(stringSql, new { transactionId }).FirstOrDefault();
            return Result<free_coin_transaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        public Result<gcoin_transaction> GetGCoinTransaction(IDbConnection db, string orderId)
        {
            string stringSql = @"SELECT * FROM gcoin_transaction WHERE order_id = @orderId";
            var coin = db.Query<gcoin_transaction>(stringSql, new { orderId }).FirstOrDefault();
            return Result<gcoin_transaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }


        /// <summary>
        /// Return fist or default
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Result<coinTransaction> GetGeneralCoinTransaction(IDbConnection db, string orderId = null, string status = null)
        {
            var coin = db.Query<coinTransaction>(GenerateGetGeneralCoinTransaction(orderId, status),
                new { orderId, status }).FirstOrDefault();
            return Result<coinTransaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        /// <summary>
        /// Return a list
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Result<List<coinTransaction>> GetGeneralCoinTransactions(IDbConnection db, string status = null, int? gameId = null, int? customerId = null)
        {
            var coins = db.Query<coinTransaction>(GenerateGetGeneralCoinTransaction(null, status, customerId, gameId),
                new { status, customerId, gameId }).AsList();
            return Result<List<coinTransaction>>.Make(coins, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        private string GenerateGetGeneralCoinTransaction(string orderId = null,
            string status = null,
            int? customerId = null,
            int? gameId = null)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append(@"SELECT 
                CASE WHEN coin.order_id IS NULL THEN free.order_id
                ELSE coin.order_id
                END AS order_id,
                CASE WHEN coin.amount IS NULL THEN free.amount
                ELSE coin.amount
                END AS amount,
                credit.amount AS quantity,
                CASE WHEN creditType.id IS NULL THEN 'Package'
                ELSE 'CreditType'
                END AS exchange_option_type,
                CASE WHEN creditType.id is null then pack.string_identifier
                ELSE creditType.string_identifier
                END AS exchange_option_identifier,
                credit.id AS credit_transaction_id,
                coin.id AS coin_id,
                free.id AS free_coin_id,     
                CASE WHEN coin.status IS NULL THEN free.status
                ELSE coin.status
                END AS status,
                ca.username,
                CASE WHEN coin.description IS NULL THEN free.description
                ELSE coin.description
                END AS description

                FROM credit_transaction credit
                LEFT JOIN coin_transaction coin ON credit.coin_transaction_id = coin.id
                LEFT JOIN free_coin_transaction free ON credit.free_coin_transaction_id = free.id
                LEFT JOIN credit_type creditType ON credit.credit_type_id = creditType.id
                LEFT JOIN package pack ON credit.package_id = pack.id
                LEFT JOIN customer_account ca ON credit.customer_account_id = ca.id
                
                WHERE 1 = 1 ");

            if (!string.IsNullOrEmpty(orderId))
                sqlBuilder.Append(@" AND (coin.order_id = @orderId OR  free.order_id = @orderId)");
            if (!string.IsNullOrEmpty(status))
                sqlBuilder.Append(@" AND credit.status = @status");
            if (customerId.HasValue)
                sqlBuilder.Append(@" AND credit.customer_account_id = @customerId");
            if (gameId.HasValue)
                sqlBuilder.Append(@" AND credit.game_id = @gameId");

            return sqlBuilder.ToString();
        }

        public bool UpdateCoinTransaction(IDbConnection db, int id, string status)
        {
            string stringSql = @"UPDATE coin_transaction
                SET status = @status WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, status });
        }

        public bool UpdateCoinTransaction(IDbConnection db, int id, string status, string description)
        {
            string stringSql = @"UPDATE coin_transaction
                SET status = @status, description=@description WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, status, description });
        }

        public bool UpdateCoinTransactionPaypalInfor(IDbConnection db, int id, string paypal_redirect_urls, string paypal_payment_id)
        {
            string stringSql = @"UPDATE coin_transaction
                SET paypal_redirect_urls = @paypal_redirect_urls,
                paypal_payment_id=@paypal_payment_id WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, paypal_redirect_urls, paypal_payment_id });
        }

        public bool UpdateFreeCoinTransaction(IDbConnection db, int id, string status)
        {
            string stringSql = @"UPDATE free_coin_transaction
                SET status = @status WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, status });
        }

        public bool UpdateCreditTransactionStatus(IDbConnection db, int id, string status)
        {
            string stringSql = @"UPDATE credit_transaction
                SET status = @status WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, status });
        }

        public Result<List<GeneralTransaction>> GetTransactions(IDbConnection db, int userId)
        {
            string sql = @"SELECT table_name, order_id, created_at, description, amount, game_id, receiver_account_id, payment_method, partner_account_id
                           FROM
                            (
	                            SELECT 'coin_transaction' AS table_name, order_id, coin.created_at, coin.description, coin.amount, coin.game_id, receiver_account_id, payment_method, partner_account_id
	                            FROM coin_transaction as coin
	                            LEFT JOIN game on game.id = game_id
	                            WHERE coin.customer_account_id=@userId
	                            UNION ALL
	                            SELECT 'gcoin_transaction' AS table_name, order_id, gcoin.created_at, gcoin.description, gcoin.amount, gcoin.game_id,NULL as receiver_account_id, payment_method, NULL as partner_account_id
	                            FROM gcoin_transaction as gcoin
	                            LEFT JOIN game on game.id = game_id
	                            WHERE customer_account_id=@userId
	                            UNION ALL
	                            SELECT 'free_coin_transaction' AS table_name, order_id, fcoin.created_at, fcoin.description, fcoin.amount, fcoin.game_id, NULL as receiver_account_id, payment_method , NULL as partner_account_id
	                             FROM free_coin_transaction as fcoin
	                            LEFT JOIN game on game.id = game_id
	                            WHERE customer_account_id=@userId
                            )result
                           ORDER BY created_at DESC";

            var trans = db.Query<GeneralTransaction>(sql, new { userId }).ToList();
            return Result<List<GeneralTransaction>>.Make(trans, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        public Result<List<GeneralTransaction>> GetTransactions(IDbConnection db, int userId, List<string> statuses,int? page =null, int? pageSize=null)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT id, table_name, order_id, created_at, description, amount, game_id, receiver_account_id, payment_method, partner_account_id, credit_type_id, package_id, count(*) OVER() as totalrow 
                           FROM
                            (
	                            SELECT coin.id, 'coin_transaction' AS table_name, order_id, coin.created_at, coin.description, coin.amount, coin.game_id, receiver_account_id, payment_method, partner_account_id, coin.credit_type_id, coin.package_id
	                            FROM coin_transaction as coin
	                            LEFT JOIN game on game.id = game_id
	                            WHERE coin.customer_account_id=@userId AND coin.status IN ({0})
	                            UNION ALL
	                            SELECT gcoin.id,'gcoin_transaction' AS table_name, order_id, gcoin.created_at, gcoin.description, gcoin.amount, gcoin.game_id,NULL as receiver_account_id, payment_method, NULL as partner_account_id, NULL AS credit_type_id, NULL AS package_id
	                            FROM gcoin_transaction as gcoin
	                            LEFT JOIN game on game.id = game_id
	                            WHERE customer_account_id=@userId AND  gcoin.status = 'success'
	                            UNION ALL
	                            SELECT fcoin.id, 'free_coin_transaction' AS table_name, order_id, fcoin.created_at, fcoin.description, fcoin.amount, fcoin.game_id, NULL as receiver_account_id, payment_method , NULL as partner_account_id, fcoin.credit_type_id, fcoin.package_id
	                             FROM free_coin_transaction as fcoin
	                            LEFT JOIN game on game.id = game_id
	                            WHERE customer_account_id=@userId AND fcoin.status IN ({0})
                            )result
                           ORDER BY created_at DESC");
            if(page.HasValue && pageSize.HasValue)
            {
                int offset = (page.Value - 1) * pageSize.Value;
                sql.AppendLine(string.Format("OFFSET {0}",offset));
                sql.AppendLine(string.Format("LIMIT {0}",pageSize.Value));
            }

            string query = string.Format(sql.ToString(), string.Join(" ,", statuses.ToArray()));

            var trans = db.Query<GeneralTransaction>(query, new { userId }).ToList();
            return Result<List<GeneralTransaction>>.Make(trans, errorIfNull: ErrorCodes.InvalidOrderId);
        }


        public Result<List<AdminGeneralTransaction>> GetTransactionsByCustomQuery(IDbConnection db, string sqlQuery)
        {
            var trans = db.Query<AdminGeneralTransaction>(sqlQuery).ToList();
            return Result<List<AdminGeneralTransaction>>.Make(trans, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        /// <summary>
        /// Create full fields
        /// </summary>
        /// <param name="db"></param>
        /// <param name="coinTrans">entity</param>
        /// <returns></returns>
        public int CreateCoinTransaction(IDbConnection db, coin_transaction coinTrans)
        {
            string sql = @"INSERT INTO coin_transaction(
            customer_account_id, receiver_account_id, amount, partner_account_id, 
            game_id, credit_type_id, package_id, description, 
            status, topup_card_id, order_id, payment_method, sender_account_id, 
            gtoken_package_id, paypal_redirect_urls, paypal_payment_id, price, 
            ip_address, country_code, telkom_order_id, use_gtoken)
            VALUES (
            @customer_account_id, @receiver_account_id, @amount, @partner_account_id, 
            @game_id, @credit_type_id, @package_id, @description, 
            @status, @topup_card_id, @order_id, @payment_method, @sender_account_id, 
            @gtoken_package_id, @paypal_redirect_urls, @paypal_payment_id, @price, 
            @ip_address, @country_code, @telkom_order_id, @use_gtoken)
             RETURNING id";
            return db.Query<int>(sql, coinTrans).FirstOrDefault();
        }
        public Result<coin_transaction> GetCoinTransactionById(IDbConnection db, int id)
        {
            string stringSql = @"SELECT * FROM coin_transaction WHERE id = @id";
            var coin = db.Query<coin_transaction>(stringSql, new { id }).FirstOrDefault();
            return Result<coin_transaction>.Make(coin);
        }


        public Result<credit_transaction> GetCreditTransactionByCoinTransactionId(IDbConnection db, int coin_transaction_id)
        {
            string stringSQL = @"SELECT * FROM credit_transaction WHERE coin_transaction_id=@coin_transaction_id";
            var coin = db.Query<credit_transaction>(stringSQL, new { coin_transaction_id }).FirstOrDefault();
            return Result<credit_transaction>.Make(coin);
        }

        public Result<credit_transaction> GetCreditTransactionByFreeCoinTransactionId(IDbConnection db, int free_coin_transaction_id)
        {
            string stringSQL = @"SELECT * FROM credit_transaction WHERE free_coin_transaction_id=@free_coin_transaction_id";
            var coin = db.Query<credit_transaction>(stringSQL, new { free_coin_transaction_id }).FirstOrDefault();
            return Result<credit_transaction>.Make(coin);
        }

        public Result<credit_transaction> GetCreditTransactionById(IDbConnection db, int id)
        {
            string stringSQL = @"SELECT * FROM credit_transaction WHERE id=@id";
            var coin = db.Query<credit_transaction>(stringSQL, new { id }).FirstOrDefault();
            return Result<credit_transaction>.Make(coin);
        }


        public Result<List<credit_transaction>> GetCreditTransactionsByCustomQuery(IDbConnection db, string sqlQuery)
        {
            var coin = db.Query<credit_transaction>(sqlQuery).ToList();
            return Result<List<credit_transaction>>.Make(coin);
        }


        public Result<GtokenPackage> GetGTokenPackage(IDbConnection db, string sku)
        {
            string stringSql = @"SELECT * FROM gtoken_package WHERE sku = @sku";
            var obj = db.Query<GtokenPackage>(stringSql, new { sku }).FirstOrDefault();
            return Result<GtokenPackage>.Make(obj);
        }
        public Result<GtokenPackage> GetGTokenPackage(IDbConnection db, int id)
        {
            string stringSql = @"SELECT * FROM gtoken_package WHERE id = @id";
            var obj = db.Query<GtokenPackage>(stringSql, new { id }).FirstOrDefault();
            return Result<GtokenPackage>.Make(obj);
        }
        public Result<GtokenPackage> GetGTokenPackage(IDbConnection db, decimal price, string currency = "USD")
        {
            string stringSql = @"SELECT * FROM gtoken_package WHERE price = @price 
                                            AND currency = @currency";
            var obj = db.Query<GtokenPackage>(stringSql, new { price, currency }).FirstOrDefault();
            return Result<GtokenPackage>.Make(obj);
        }
        public bool UpdateCoinTransaction(IDbConnection db, int id, string status, string description, string telkom_order_id)
        {
            string stringSql = @"UPDATE coin_transaction
                SET status = @status, description = @description, telkom_order_id = @telkom_order_id
                WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, status, description, telkom_order_id });
        }

        public bool UpdateFreeCoinTransaction(IDbConnection db, int id, string status, string description)
        {
            string stringSql = @"UPDATE coin_transaction
                SET status = @status, description = @description
                WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, status, description });
        }
        public Result<topup_card> GetTopUpCard(IDbConnection db, int id)
        {
            string stringSql = @"SELECT * FROM topup_card WHERE id = @id";
            var obj = db.Query<topup_card>(stringSql, new { id }).FirstOrDefault();
            return Result<topup_card>.Make(obj);
        }

        public Result<List<topup_card>> GetTopUpCards(IDbConnection db, string stringSql, int customer_account_id)
        {
            var obj = db.Query<topup_card>(stringSql, new { customer_account_id }).ToList();
            return Result<List<topup_card>>.Make(obj);
        }

        /// <summary>
        /// Create full fields
        /// </summary>
        /// <param name="db"></param>
        /// <param name="coinTrans">entity</param>
        /// <returns></returns>
        public int CreateCoinTransaction(IDbConnection db, int customer_account_id, int receiver_account_id, decimal amount, string description, string order_id, bool use_gtoken, string status)
        {
            string sql = @"INSERT INTO coin_transaction
            (customer_account_id, receiver_account_id, amount, description, order_id, use_gtoken, status)
            VALUES 
            (@customer_account_id, @receiver_account_id, @amount, @description, @order_id, @use_gtoken, @status)
             RETURNING id";

            return db.Query<int>(sql, new
            {
                customer_account_id,
                receiver_account_id,
                amount,
                description,
                order_id,
                use_gtoken,
                status
            }).FirstOrDefault();
        }

        public Result<gcoin_transaction> GetGcoinTransaction(IDbConnection db, string orderId)
        {
            string stringSql = @"SELECT * FROM gcoin_transaction WHERE order_id = @orderId";
            var coin = db.Query<gcoin_transaction>(stringSql, new { orderId }).FirstOrDefault();
            return Result<gcoin_transaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        /// <summary>
        /// Get first paypal_preapproval using is_active
        /// </summary>
        /// <param name="db"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public Result<paypal_preapproval> GetPaypalPreApproval(IDbConnection db, bool isActive)
        {
            string stringSql = @"select * from paypal_preapproval where is_active = @isActive";
            var paypal = db.Query<paypal_preapproval>(stringSql, new { isActive }).FirstOrDefault();
            return Result<paypal_preapproval>.Make(paypal, errorIfNull: ErrorCodes.NotFound);
        }


        public Result<List<coin_transaction>> GetCoinTransaction(IDbConnection db, int gameId,
            int userId,
            int packageId,
            List<string> status = null)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(@"SELECT * FROM coin_transaction
                WHERE 
                        game_id = @gameId
                    AND customer_account_id = @userId
                    AND package_id = @packageId");

            if (status != null)
            {
                foreach (var item in status)
                {
                    queryBuilder.Append(" AND status = '");
                    queryBuilder.Append(item);
                    queryBuilder.Append("'");
                }
            }

            var coins = db.Query<coin_transaction>(queryBuilder.ToString(), new { gameId, userId, packageId, status }).AsList();
            return Result<List<coin_transaction>>.Make(coins, errorIfNull: ErrorCodes.NotFound);
        }

        public Result<List<free_coin_transaction>> GetFreeCoinTransaction(IDbConnection db, int gameId,
            int userId,
            int packageId,
            List<string> status = null)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(@"SELECT * FROM free_coin_transaction
                WHERE 
                        game_id = @gameId
                    AND customer_account_id = @userId
                    AND package_id = @packageId");

            if (status != null)
            {
                foreach (var item in status)
                {
                    queryBuilder.Append(" AND status = '");
                    queryBuilder.Append(item);
                    queryBuilder.Append("'");
                }
            }

            var freeCoins = db.Query<free_coin_transaction>(queryBuilder.ToString(), new { gameId, userId, packageId, status }).AsList();
            return Result<List<free_coin_transaction>>.Make(freeCoins, errorIfNull: ErrorCodes.NotFound);
        }

        public int CreateGCoinTransaction(IDbConnection db, gcoin_transaction coinTrans)
        {
            string sql = @"INSERT INTO gcoin_transaction(
						customer_account_id, amount, game_id, description, 
			order_id, status, ip_address, country_code, payment_method, sender_email, 
			receiver_email, pay_key, pay_key_expiration_date, updated_at)
			VALUES (
			@customer_account_id, @amount, @game_id, @description, 
			@order_id, @status, @ip_address, @country_code, @payment_method, @sender_email, 
			@receiver_email, @pay_key, @pay_key_expiration_date, @updated_at)
			 RETURNING id";
            return db.Query<int>(sql, coinTrans).FirstOrDefault();
        }
        public Result<gcoin_transaction> GetGcoinTransaction(IDbConnection db, int id)
        {
            string stringSql = @"SELECT * FROM gcoin_transaction WHERE id = @id";
            var coin = db.Query<gcoin_transaction>(stringSql, new { id }).FirstOrDefault();
            return Result<gcoin_transaction>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }
        public bool UpdateGCoinTransaction(IDbConnection db, gcoin_transaction gcoinTrans)
        {
            string stringSql = @"UPDATE gcoin_transaction
				SET amount=@amount, game_id=@game_id, description=@description, 
                  status=@status, ip_address=@ip_address, country_code=@country_code, 
                   payment_method=@payment_method, sender_email=@sender_email, receiver_email=@receiver_email, pay_key=@pay_key, 
                   pay_key_expiration_date=@pay_key_expiration_date, updated_at=@updated_at
				WHERE id = @id";
            return 1 == db.Execute(stringSql, gcoinTrans);
        }

        public Result<List<gcoin_transaction>> GetGcoinTransactionByCustomeQuery(IDbConnection db, string sqlQuery)
        {
            var coin = db.Query<gcoin_transaction>(sqlQuery).ToList();
            return Result<List<gcoin_transaction>>.Make(coin, errorIfNull: ErrorCodes.InvalidOrderId);
        }

        public int CreateCreditTransaction(IDbConnection db, credit_transaction creditTrans)
        {
            string sql = @"INSERT INTO credit_transaction (
                          customer_account_id,
                          coin_transaction_id,
                          amount,
                          game_id,
                          credit_type_id,
                          package_id,
                          created_at,
                          description,
                          status,
                          free_coin_transaction_id) 
                        VALUES(
                          @customer_account_id,
                          @coin_transaction_id,
                          @amount,
                          @game_id,
                          @credit_type_id,
                          @package_id,
                          @created_at,
                          @description,
                          @status,
                          @free_coin_transaction_id) 
             RETURNING id";
            return db.Query<int>(sql, creditTrans).FirstOrDefault();
        }
    }
}
