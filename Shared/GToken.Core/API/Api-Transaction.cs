using System.Collections.Generic;
using Platform.Dal;
using Platform.Models;
using System.Net;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;
using Platform.Models.Models;
using Platform.Utility;
using System.Linq;
using System.Data;

namespace Platform.Core
{
    public partial class Api
    {

        //public Result<List<CreditTransaction>> GetUnFullfilledExchanges(string gameUuid, int userId)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var game = repo.GetGame(db, gameUuid);
        //        if (!game.Succeeded)
        //        {
        //            return Result<List<CreditTransaction>>.Null(game.Error);
        //        }
        //        return repo.GetUnFullfilledExchanges(db, game.Data.id, userId);
        //    }
        //}

        //public Result<List<CreditTransaction>> FullfillExchange(string gameUuid, int userId, string orderId) /* string instead of Guidfor more flexibility */
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var game = repo.GetGame(db, gameUuid);
        //        if (!game.Succeeded)
        //        {
        //            return Result<List<CreditTransaction>>.Null(game.Error);
        //        }

        //        var coinTran = repo.GetCoinTransaction(db, orderId);
        //        if (coinTran.Succeeded)
        //        {
        //            // TODO: Full up the list of packages IAP items //
        //            // return Result< List<CreditTransaction> >.Make(coinTran.Data);
        //        }
        //        // Else, not succeeded //

        //        var freeCoinTran = repo.GetFreeCoinTransaction(db, orderId);
        //        // return Result< List<CreditTransaction> >.Make(freeCoinTran.Data, freeCoinTran.Error);

        //        // TEST //
        //        return Result<List<CreditTransaction>>.Null();
        //    }
        //}

        //public Result<List<CreditTransaction>> GetExchange(string gameUuid, int userId, string orderId, TransactionStatus? statusFilter = null)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var game = repo.GetGame(db, gameUuid);
        //        if (!game.Succeeded)
        //        {
        //            return Result<List<CreditTransaction>>.Null(game.Error);
        //        }
        //        return repo.GetExchange(db, orderId, statusFilter);
        //    }
        //}

        //public Result<List<CreditTransaction>> RejectExchange(string gameUuid, int userId, string orderId)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var game = repo.GetGame(db, gameUuid);
        //        if (!game.Succeeded)
        //        {
        //            return Result<List<CreditTransaction>>.Null(game.Error);
        //        }

        //        var exchange = repo.GetExchange(db, orderId, TransactionStatus.Pending);
        //        if (exchange.HasData)
        //        {
        //            return repo.SetExchangeStatus(db, orderId, TransactionStatus.Failure);
        //        }
        //        return exchange.Nullify();
        //    }
        //}


        //public void UpdateExternalExchange()
        //{

        //}


        public Result<ExchangeRate> GetExchangeRate(string sourceCurrency, string destinationCurrency = Platform.Models.ConstantValues.S_CURRENCY_USD, DateTime? date = null)
        {
            //Find in DB
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                DateTime now = date ?? DateTime.Now;

                var exchangeRate = repo.GetExchangeRate(db, now.Month.ToString(), now.Year.ToString(), sourceCurrency, destinationCurrency);
                if (exchangeRate.Succeeded)
                {
                    return exchangeRate;
                }

                float rate = 0;
                if (date.HasValue)
                {
                    rate = GetExchangeRateByDateFromYahoo(sourceCurrency, destinationCurrency, now);
                    if (rate <= 0)
                        return Result<ExchangeRate>.Make(null, ErrorCodes.INVALID_CURRENCY_CODE);
                }
                else
                {
                    rate = GetExchangeRateFromYahoo(sourceCurrency, destinationCurrency);
                    if (rate <= 0)
                        return Result<ExchangeRate>.Make(null, ErrorCodes.INVALID_CURRENCY_CODE);
                }

                var newExchangeRate = new ExchangeRateData
                {
                    month = now.Month.ToString(),
                    year = now.Year.ToString(),
                    source_currency = sourceCurrency,
                    destination_currency = destinationCurrency,
                    exchange_rate = Convert.ToDecimal(rate)
                };

                repo.CreateExchangeRate(db, newExchangeRate);

                return Result<ExchangeRate>.Make(newExchangeRate);
            }
        }

        private float GetExchangeRateFromYahoo(string sourceCurrency, string destinationCurrency)
        {
            WebClient web = new WebClient();
            string ur = Platform.Models.ConstantValues.S_YAHOO_GET_EXCHANGE_RATE;
            string url = string.Format(ur, sourceCurrency.ToUpper(), destinationCurrency.ToUpper());

            string response = web.DownloadString(url);
            string[] values = Regex.Split(response, ",");

            float rate = -1f;
            bool isSuccess = float.TryParse(values[1], out rate);
            return rate;
        }


        /// <summary>
        /// GetExchangeRate history
        /// </summary>
        /// <param name="sourceCurrency"></param>
        /// <param name="destinationCurrency"></param>
        /// <param name="date">using format YYYYMMDD</param>
        /// <returns></returns>
        private float GetExchangeRateByDateFromYahoo(string sourceCurrency, string destinationCurrency, DateTime date)
        {
            try
            {
                string sourceUrl = Platform.Models.ConstantValues.S_YAHOO_GET_EXCHANGE_RATE_BY_DATE;
                string destinationUrl = string.Format(sourceUrl, date.ToString(Platform.Models.ConstantValues.S_YAHOO_DATE_FORMAT));
                string response = new WebClient().DownloadString(destinationUrl);

                //137 - the number of removed characters from yahoo on header
                //142 - remove 5 final characters
                //The removing to convert returned content from yahoo to parsable Json

                var r1 = response.Substring(137, response.Length - 142);
                var r2 = ("{" + r1).Replace("\n", string.Empty);

                var listHistoryExchange = JsonConvert.DeserializeObject<YahooExchange>(r2);

                float frate = 1;
                float trate = 1;

                if (sourceCurrency.ToUpper() != "USD")
                {
                    foreach (var item in listHistoryExchange.resources)
                    {
                        if (item.resource.fields.symbol == sourceCurrency.ToUpper() + "=X")
                        {
                            frate = (float)item.resource.fields.price;
                            break;
                        }
                    }
                }

                if (destinationCurrency.ToUpper() != "USD")
                {
                    foreach (var item in listHistoryExchange.resources)
                    {
                        if (item.resource.fields.symbol == destinationCurrency + "=X")
                        {
                            trate = (float)item.resource.fields.price;
                            break;

                        }
                    }
                }
                return trate / frate;
            }
            catch
            {
                return -1f;
            }
        }

        /// <summary>
        /// Get transaction from transaction Id or (orderId and partnerIdentifier)
        /// </summary>
        /// <param name="partnerIdentifier"></param>
        /// <param name="orderId"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public Result<Transaction> GetTransaction(string partnerIdentifier, string orderId = null, string transactionId = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (!string.IsNullOrEmpty(transactionId))
                {
                    return repo.GetTransaction(db, transactionId);
                }
                if (!string.IsNullOrEmpty(orderId))
                {
                    return repo.GetTransaction(db, partnerIdentifier, orderId);
                }
                return Result<Transaction>.Make(null, ErrorCodes.INVALID_TRANSACTION_ID);
            }
        }


        public Result<DirectGtokenTransaction> GetDirectGTokenTransaction(string partnerIdentifier, string orderId = null, string transactionId = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (!string.IsNullOrEmpty(transactionId))
                {
                    return repo.GetDirectGTokenTransaction(db, transactionId);
                }
                if (!string.IsNullOrEmpty(orderId))
                {
                    return repo.GetDirectGTokenTransaction(db, partnerIdentifier, orderId);
                }
                return Result<DirectGtokenTransaction>.Make(null, ErrorCodes.INVALID_TRANSACTION_ID);
            }
        }



        public Result<Transaction> GetTransactionById(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransactionById(db, id);
            }
        }

        public Result<TokenTransaction> GetTokenTransactionById(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTokenTransactionById(db, id);
            }
        }

        public bool UpdateTransactionStatus(long transactionId, TransactionStatus status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateTransactionStatus(db, transactionId, Platform.Utility.Helper.GetDescription(status));
            }
        }

        public Result<Transaction> GetTransaction(string gtoken_transaction_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransaction(db, gtoken_transaction_id);
            }
        }
        public Result<Transaction> CreateTransaction(string partner_identifier, string ipAddress, TransactionJsonModel transactionJSON)
        {
            Transaction transaction = new Transaction();
            while (true)
            {
                // Generate a unique ID to avoid duplicated ID in database //
                string gToken_trans_id = Guid.NewGuid().ToString();

                if (!GetTransaction(gToken_trans_id).Succeeded) // Not found in DB //
                {
                    transaction.gtoken_transaction_id = gToken_trans_id;
                    break;
                }
            }
            transaction.partner_identifier = partner_identifier;
            transaction.partner_order_id = transactionJSON.order_id;
            transaction.customer_username = transactionJSON.username;
            transaction.price = transactionJSON.original_price.HasValue ? transactionJSON.original_price.Value : 0;
            transaction.original_price = transactionJSON.original_price;
            transaction.original_final_amount = transactionJSON.original_final_amount;
            transaction.original_currency = transactionJSON.original_currency;
            transaction.original_tax = transactionJSON.original_tax;
            transaction.original_service_charge = transactionJSON.original_service_charge;
            transaction.discount_percentage = transactionJSON.discount_percentage;
            transaction.payment_method = transactionJSON.payment_method;
            transaction.description = transactionJSON.description;
            transaction.ip_address = ipAddress;
            transaction.revenue_percentage = transactionJSON.revenue_percentage;

            IPAddress ip;
            if (!IPAddress.TryParse(ipAddress, out ip))
            {
                transaction.ip_address = "127.0.0.1";
                ip = IPAddress.Parse(transaction.ip_address);

            }
            string country_name = string.Empty;
            ip.GetCountryCode(c => transaction.country_code = c, n => country_name = n);

            transaction.status = !String.IsNullOrEmpty(transactionJSON.status) ? transactionJSON.status : Platform.Models.ConstantValues.S_PENDING;

            transaction.is_venvici_applicable = transactionJSON.is_venvici_applicable.HasValue ? transactionJSON.is_venvici_applicable.Value : true;

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var myTrans = db.BeginTransaction();
                try
                {
                    int newTransId = repo.CreateTransaction(db, transaction);
                    myTrans.Commit();
                    Transaction newTrans = GetTransactionById(newTransId).Data;
                    return Result<Transaction>.Make(newTrans, null);
                }
                catch
                {
                    myTrans.Rollback();
                    return Result<Transaction>.Null(ErrorCodes.ServerError);
                }

            }
        }

        public Result<List<Transaction>> getTransactions(string username, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransactions(db, username, status);
            }
        }

        public Result<TokenTransaction> GetTokenTransaction(string gtoken_transaction_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTokenTransaction(db, gtoken_transaction_id);
            }
        }


        public Result<DirectGtokenTransaction> GetDirectGTokenTransaction(string gtoken_transaction_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetDirectGTokenTransaction(db, gtoken_transaction_id);
            }
        }

        /// <summary>
        /// Get row in direct_gtoken_transaction by its Id
        /// </summary>
        /// <param name="id">DirectGTokenTransaction Id</param>
        /// <returns></returns>
        public Result<DirectGtokenTransaction> GetDirectGTokenTransaction(int id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetDirectGTokenTransaction(db, id);
            }
        }

        public Result<List<MainTransaction>> GetTokenTransactionsByCustomQuery(string sqlQuery)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTokenTransactionsByCustomQuery(db, sqlQuery);
            }
        }


        public Result<List<MainTransaction>> GetAllTransactions(int fromIndex, int toIndex, string username, params string[] status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAllTransactions(db, fromIndex, toIndex, username, status);
            }
        }
        public int CountAllTransactions(string username, string status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CountAllTransactionRows(db, username, status);
            }
        }

        public Result<List<CustomTransaction>> GetTransactionsByQueryString(string startTime = null, string endTime = null, string partner_identifier = null, string status = null,
            string partner_order_id = null, string gtoken_transaction_id = null, string timeZone = null, string username = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransactionsByQueryString(db, startTime, endTime, partner_identifier, status, partner_order_id, gtoken_transaction_id, timeZone, username);
            }
        }



        public Result<List<DirectTransaction>> GetDirectGTokenTransactionByQueryString(string startTime = null, string endTime = null, string partner_identifier = null, string status = null,
            string partner_order_id = null, string gtoken_transaction_id = null, string timeZone = null, string username = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetDirectGTokenTransactionByQueryString(db, startTime, endTime, partner_identifier, status, partner_order_id, gtoken_transaction_id, timeZone, username);
            }
        }



        public Result<List<Transaction>> FindUserTransactions(string username, List<string> status = null)
        {
            if (status == null)
            {
                status = new List<string>() { 
                    Platform.Utility.Helper.GetDescription(TransactionStatus.Pending),
                    Platform.Utility.Helper.GetDescription(TransactionStatus.Success)
                };
            }

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransactions(db, username, status);
            }
        }

        public Result<TokenTransaction> CreateTokenTransaction(string partner_identifier, string ipAddress, TokenTransactionJson jsonTokenTransaction)
        {
            TokenTransaction tokenTransaction = new TokenTransaction();

            while (true)
            {
                // Generate a unique ID to avoid duplicated ID in database //
                string gToken_trans_id = Guid.NewGuid().ToString();

                if (!GetTokenTransaction(gToken_trans_id).Succeeded) // Not found in DB //
                {
                    tokenTransaction.gtoken_transaction_id = gToken_trans_id;
                    break;
                }
            }

            tokenTransaction.partner_identifier = partner_identifier;
            tokenTransaction.partner_order_id = jsonTokenTransaction.order_id;
            tokenTransaction.customer_username = jsonTokenTransaction.username;
            tokenTransaction.amount = jsonTokenTransaction.amount.HasValue
                ? jsonTokenTransaction.amount.Value
                : 0;
            tokenTransaction.tax = jsonTokenTransaction.tax.HasValue
                ? jsonTokenTransaction.tax.Value
                : 0;
            tokenTransaction.service_charge = jsonTokenTransaction.service_charge.HasValue
                ? jsonTokenTransaction.service_charge.Value
                : 0;
            tokenTransaction.token_type = jsonTokenTransaction.token_type;
            tokenTransaction.transaction_type = jsonTokenTransaction.transaction_type;
            tokenTransaction.description = jsonTokenTransaction.description;

            tokenTransaction.ip_address = ipAddress;
            IPAddress ip;
            if (!IPAddress.TryParse(ipAddress, out ip))
            {
                tokenTransaction.ip_address = "127.0.0.1";
                ip = IPAddress.Parse(tokenTransaction.ip_address);

            }
            string country_name = string.Empty;
            ip.GetCountryCode(c => tokenTransaction.country_code = c, n => country_name = n);

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                int newTransId = repo.CreateTokenTransaction(db, tokenTransaction);
                if (newTransId > 0)
                {
                    TokenTransaction newTrans = GetTokenTransactionById(newTransId).Data;
                    return Result<TokenTransaction>.Make(newTrans, null);
                }
                else
                {
                    return Result<TokenTransaction>.Make(null, ErrorCodes.ServerError);
                }

            }
        }


        public Result<DirectGtokenTransaction> CreateDirectGTokenTransaction(string partner_identifier, string username, string ipAddress, APIDirectGTokenTransactionResult jsonDirectGTokenTransaction)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                DirectGtokenTransaction tokenTransaction = new DirectGtokenTransaction();

                while (true)
                {
                    // Generate a unique ID to avoid duplicated ID in database //
                    string gToken_trans_id = Guid.NewGuid().ToString();

                    if (!repo.GetDirectGTokenTransaction(db, gToken_trans_id).Succeeded) // Not found in DB //
                    {
                        tokenTransaction.gtoken_transaction_id = gToken_trans_id;
                        break;
                    }
                }

                tokenTransaction.partner_identifier = partner_identifier;
                tokenTransaction.partner_order_id = jsonDirectGTokenTransaction.order_id;
                tokenTransaction.customer_username = jsonDirectGTokenTransaction.username;
                tokenTransaction.amount = jsonDirectGTokenTransaction.amount.HasValue
                    ? jsonDirectGTokenTransaction.amount.Value
                    : 0;
                tokenTransaction.description = jsonDirectGTokenTransaction.description;
                tokenTransaction.ip_address = ipAddress;
                IPAddress ip;
                if (!IPAddress.TryParse(ipAddress, out ip))
                {
                    tokenTransaction.ip_address = "127.0.0.1";
                    ip = IPAddress.Parse(tokenTransaction.ip_address);

                }
                string country_name = string.Empty;
                ip.GetCountryCode(c => tokenTransaction.country_code = c, n => country_name = n);

                int newTransId = repo.CreateDirectGTokenTransaction(db, tokenTransaction);
                if (newTransId > 0)
                {
                    tokenTransaction = repo.GetDirectGTokenTransaction(db, newTransId).Data;
                    return Result<DirectGtokenTransaction>.Make(tokenTransaction, null);
                }
                else
                {
                    return Result<DirectGtokenTransaction>.Make(null, ErrorCodes.ServerError);
                }
            }
        }
        public Result<List<Partner>> GetAllPartners()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAllPartners(db);
            }
        }


    }
}
