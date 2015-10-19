using System.Collections.Generic;
using Platform.Models;
using System.Data;
using GoPlay.Models;
using GoPlay.Dal;
using GoPlay.Models.Models;
using System.Linq;
using System;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        /// <summary>
        /// Get coin_transaction first, then, if null --> get free coin transaction
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Result<coinTransaction> GetCoinTransaction(string orderId = null, string status = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGeneralCoinTransaction(db, orderId, status);
            }
        }


        public Result<coin_transaction> GetCoinTransactionByOderId(string orderId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCoinTransaction(db, orderId);
            }
        }

        public Result<List<coin_transaction>> GetCoinTransactionsByCustomQuery(string queryString)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCoinTransactionsByCustomQuery(db, queryString);
            }
        }


        public Result<List<free_coin_transaction>> GetFreeCoinTransactionsByCustomQuery(string queryString)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetFreeCoinTransactionsByCustomQuery(db, queryString);
            }
        }

        public Result<List<gcoin_transaction>> GetGCoinTransactionsByCustomQuery(string queryString)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGCoinTransactionsByCustomQuery(db, queryString);
            }
        }

        public Result<coin_transaction> GetCoinTransactionByPaypalPaymentId(string paypal_payment_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCoinTransactionByPaypalPaymentId(db, paypal_payment_id);
            }
        }

        public Result<List<coin_transaction>> GetCoinTransaction(
            int gameId,
            int userId,
            int packageId,
            List<string> status = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (status == null || !status.Any())
                {
                    status = new List<string>() {
                       ConstantValues.S_SUCCESS,
                       ConstantValues.S_PENDING
                    };
                }

                return repo.GetCoinTransaction(db, gameId, userId, packageId, status);
            }
        }

        public Result<List<free_coin_transaction>> GetFreeCoinTransaction(
            int gameId,
            int userId,
            int packageId,
            List<string> status = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (status == null || !status.Any())
                {
                    status = new List<string>() {
                       ConstantValues.S_SUCCESS,
                       ConstantValues.S_PENDING
                    };
                }

                return repo.GetFreeCoinTransaction(db, gameId, userId, packageId, status);
            }
        }

        public Result<free_coin_transaction> GetFreeCoinTransactionByOderId(string orderId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetFreeCoinTransaction(db, orderId);
            }
        }

        public Result<free_coin_transaction> GetFreeCoinTransactionById(int transactionId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetFreeCoinTransaction(db, transactionId);
            }
        }
        public Result<List<coinTransaction>> GetCoinTransactions(int customerAccountId,
            int gameId, string status = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGeneralCoinTransactions(db, status, gameId, customerAccountId);
            }
        }

        public bool UpdateCoinTransactionStatus(int coin_transaction_id, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                bool result = repo.UpdateCoinTransaction(db, coin_transaction_id, status);
                if (result)
                    StormFlushedCoinTransaction(coin_transaction_id, status);
                return result;
            }
        }

        /// <summary>
        /// This method is called when coin transaction table is changed. (update/ insert/ delete)
        /// </summary>
        /// <param name="coinTransactionId"></param>
        /// <param name="status"></param>
        public void StormFlushedCoinTransaction(int coinTransactionId, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                customer_account customer = repo.GetCustomerByCoinTransactionId(db, coinTransactionId).Data;
                if (string.Compare(status, ConstantValues.S_SUCCESS, StringComparison.OrdinalIgnoreCase) == 0
                    && !string.IsNullOrEmpty(customer.inviter_username))
                {
                    coin_transaction transaction = repo.GetCoinTransactionById(db, coinTransactionId).Data;
                    if (!string.IsNullOrEmpty(transaction.payment_method))
                    {
                        AddActiveGamerScheme(customer, transaction);
                    }
                }
                RecalculatePlayToken(customer.id);
            }
        }

        /// <summary>
        /// This method is called when coin transaction table is changed. (update/ insert/ delete)
        /// </summary>
        /// <param name="coinTransactionId"></param>
        /// <param name="status"></param>
        public void StormFlushedFreeCoinTransaction(int freeCoinTransactionId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                customer_account customer = repo.GetCustomerByFreeCoinTransactionId(db, freeCoinTransactionId).Data;
                RecalculatePlayToken(customer.id);
            }
        }

        public void StormFlushedGCoinTransaction(int customerId, int gcoinTransactionId, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (string.Compare(status, ConstantValues.S_SUCCESS, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    paypal_preapproval pp = repo.GetPaypalPreApproval(db, true).Data;
                    if (pp != null)
                    {
                        gcoin_transaction transaction = repo.GetGcoinTransaction(db, gcoinTransactionId).Data;
                        pp.current_number_of_payments += 1;
                        pp.current_total_amount_of_all_payments = pp.current_total_amount_of_all_payments - transaction.amount ?? 0;
                        repo.UpdateGCoinTransaction(db, transaction);
                    }
                }
                RecalculateGCoin(customerId);
            }
        }

        public bool UpdateCoinTransactionStatus(int coin_transaction_id, string status, string description)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCoinTransactionPaypalInfor(db, coin_transaction_id, status, description);
            }
        }
        public bool UpdateCoinTransaction(int coin_transaction_id, string paypal_redirect_urls, string paypal_payment_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCoinTransactionPaypalInfor(db, coin_transaction_id, paypal_redirect_urls, paypal_payment_id);
            }
        }

        public bool UpdateFreeCoinTransactionStatus(int coin_transaction_id, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var result = repo.UpdateFreeCoinTransaction(db, coin_transaction_id, status);
                if (result)
                {
                    StormFlushedFreeCoinTransaction(coin_transaction_id);
                }
                return result;
            }
        }

        public bool UpdateCreditTransactionStatus(int credit_transaction_id, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCreditTransactionStatus(db, credit_transaction_id, status);
            }
        }

        //public Result<List<GeneralTransaction>> GetTransactions(int userId)
        //{
        //    {
        //        var repo = Repo.Instance;
        //        using (var db = repo.OpenConnectionFromPool())
        //        {
        //            return repo.GetTransactions(db, userId);
        //        }
        //    }
        //}

        public Result<List<GeneralTransaction>> GetTransactions(int userId, List<string> statuses = null,int? page =null, int? pageSize=null)
        {
            if (statuses == null)
            {
                statuses = new List<string>() { "'success'", "'pending'" };
            }
            {
                var repo = Repo.Instance;
                using (var db = repo.OpenConnectionFromPool())
                {
                    return repo.GetTransactions(db, userId, statuses, page,pageSize);
                }
            }
        }


        public Result<List<AdminGeneralTransaction>> GetTransactionsByCustomQuery(string sqlQuery)
        {
            {
                var repo = Repo.Instance;
                using (var db = repo.OpenConnectionFromPool())
                {
                    return repo.GetTransactionsByCustomQuery(db, sqlQuery);
                }
            }
        }

        public Result<coin_transaction> CreateCoinTransaction(coin_transaction coinTrans)
        {
            var repo = Repo.Instance;
            coinTrans.created_at = DateTime.UtcNow;
            using (var db = repo.OpenConnectionFromPool())
            {
                var newId = repo.CreateCoinTransaction(db, coinTrans);
                if (newId > 0)
                    StormFlushedCoinTransaction(newId, coinTrans.status);
                return repo.GetCoinTransactionById(db, newId);
            }
        }
        public Result<credit_transaction> CreateCreditTransaction(credit_transaction creditTrans)
        {

            var repo = Repo.Instance;
            creditTrans.created_at = DateTime.UtcNow;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCreditTransactionById(db, repo.CreateCreditTransaction(db, creditTrans));
            }
        }

        public int CreateFreeCoinTransaction(free_coin_transaction freeCoinTrans)
        {
            var repo = Repo.Instance;
            freeCoinTrans.created_at = DateTime.UtcNow;
            using (var db = repo.OpenConnectionFromPool())
            {
                var newId = repo.CreateFreeCoinTransaction(db, freeCoinTrans);
                if (newId > 0)
                {
                    StormFlushedFreeCoinTransaction(newId);
                }
                return newId;
            }
        }

        public Result<GtokenPackage> GetGTokenPackage(string sku)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGTokenPackage(db, sku);
            }
        }
        public Result<GtokenPackage> GetGTokenPackage(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGTokenPackage(db, id);
            }
        }

        public bool UpdateCoinTransaction(int id, string status, string description, string telkom_order_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                bool result = repo.UpdateCoinTransaction(db, id, status, description, telkom_order_id);
                if (result)
                    StormFlushedCoinTransaction(id, status);
                return result;
            }
        }

        public bool UpdateFreeCoinTransaction(int id, string status, string description)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                bool result = repo.UpdateFreeCoinTransaction(db, id, status, description);
                if (result)
                    StormFlushedFreeCoinTransaction(id);
                return result;
            }
        }

        public Result<topup_card> GetTopUpCard(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTopUpCard(db, id);
            }
        }

        public Result<List<topup_card>> getTopupCards(int customer_account_id, string status = "all")
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                string sql = @"SELECT * FROM topup_card WHERE customer_account_id=@customer_account_id ";
                if (status == "used")
                {
                    sql += " AND used_at is not NULL";
                }
                else if (status == "unused")
                {
                    sql += " AND used_at is NULL";
                }
                return repo.GetTopUpCards(db, sql, customer_account_id);
            }
        }

        public Result<GtokenPackage> GetGTokenPackage(decimal price, string currency = "USD")
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGTokenPackage(db, price, currency);
            }
        }

        public int CreateTransferCoinTransaction(coin_transaction coinTrans)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var newId = repo.CreateCoinTransaction(db, coinTrans.customer_account_id, coinTrans.receiver_account_id.Value,
                    coinTrans.amount.Value, coinTrans.description, coinTrans.order_id, coinTrans.use_gtoken, coinTrans.status);
                if (newId > 0)
                    StormFlushedCoinTransaction(newId, coinTrans.status);

                return newId;
            }
        }

        public Result<gcoin_transaction> GetGcoinTransaction(string orderId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGcoinTransaction(db, orderId);
            }
        }

        public Result<paypal_preapproval> GetPaypalPreApproval(bool isActive)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPaypalPreApproval(db, isActive);
            }
        }

        public Result<gcoin_transaction> CreateGCoinTransaction(gcoin_transaction gcoinTrans)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var newId = repo.CreateGCoinTransaction(db, gcoinTrans);
                if (newId > 0)
                    StormFlushedCoinTransaction(newId, gcoinTrans.status);
                return repo.GetGcoinTransaction(db, newId);
            }
        }

        public bool UpdateGCoinTransaction(gcoin_transaction gcoinTrans)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                gcoinTrans.updated_at = DateTime.UtcNow;
                var result = repo.UpdateGCoinTransaction(db, gcoinTrans);
                if (result)
                    StormFlushedGCoinTransaction(gcoinTrans.customer_account_id, gcoinTrans.id, gcoinTrans.status);
                return result;
            }
        }

        public Result<List<gcoin_transaction>> GetGcoinTransactionByCustomeQuery(string sqlQuery)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGcoinTransactionByCustomeQuery(db, sqlQuery);
            }
        }
    }
}
