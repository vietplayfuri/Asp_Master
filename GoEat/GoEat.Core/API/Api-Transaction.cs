using System.Collections.Generic;
using GoEat.Dal;
using GoEat.Dal.Models;
using GoEat.Models;
using System;
using System.Transactions;
using GoEat.Utility.Crytography;
using System.Linq;
using System.Threading.Tasks;
using GoEat.Utility;

namespace GoEat.Core
{
    public partial class GoEatApi
    {
        public Result<List<HistoryTransaction>> GetTransactions(int startIndex, int endIndex, int restaurantId,
            GoEat.Models.TransactionStatus status)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransactions(db, startIndex, endIndex, restaurantId, (int)status);
            }
        }

        public Result<List<ExportTransaction>> GetTransactions(int restaurantId, string status,
            DateTime start, DateTime end)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransactions(db, start, end, restaurantId, status);
            }
        }

        public Result<List<HistoryTransaction>> GetTransactions(int startIndex, int endIndex, int restaurant_id, DateTime fromDate, DateTime toDate)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTransactions(db, startIndex, endIndex, restaurant_id, fromDate, toDate);
            }
        }

        //public Result<List<HistoryTransaction>> GetTransactions(int startIndex, int endIndex, int customer_id, int restaurant_id, DateTime fromDate, DateTime toDate)
        //{
        //    var repo = GoEatRepo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        return repo.GetTransactions(db, startIndex, endIndex, customer_id, restaurant_id, fromDate, toDate);
        //    }
        //}

        //public Result<List<HistoryTransaction>> GetTransactions(int startIndex, int endIndex, int userId, int restaurantId)
        //{
        //    var repo = GoEatRepo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        return repo.GetTransactions(db, startIndex, endIndex, userId, restaurantId);
        //    }
        //}

        /// <summary>
        /// get total row for admin
        /// </summary>
        /// <returns></returns>
        public int CountAdminTransactions(int restaurantId, GoEat.Models.TransactionStatus status)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CountAdminTransactions(db, restaurantId, (int)status);
            }
        }

        /// <summary>
        /// get total row for customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="restaurantId"></param>
        /// <returns></returns>

        public int CountCustomerTransactions(int customerId, DateTime fDate, DateTime tDate)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CountCustomerTransactions(db, customerId, fDate, tDate);
            }
        }

        /// <summary>
        /// update gtokentransactionId, description for cash_transaction table
        /// </summary>
        /// <param name="cashTransactionId">cash_transaction_id</param>
        /// <param name="gtokenTransactionId">gtoken transaction Id</param>
        /// <param name="description">description</param>
        /// <returns>true: success / false: failure</returns>
        private bool UpdateCashTransaction(int cashTransactionId, string description)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCashTransaction(db, cashTransactionId, description);
            }
        }

        /// <summary>
        /// update status for main_transaction table
        /// </summary>
        /// <param name="mainTransactionId">id of main_transaction table</param>
        /// <param name="status">new status</param>
        /// <returns>true: success / false: failure</returns>
        public bool UpdateMainTransactionStatus(bool isFood, int mainTransactionId, int status)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (isFood)
                {
                    if (repo.GetMainTransactionById(db, mainTransactionId).Data.food_status != (int)GoEat.Models.TransactionStatus.Success)
                    {
                        return repo.UpdateMainTransactionStatusForFood(db, mainTransactionId, status);
                    }
                }
                else
                {
                    if (repo.GetMainTransactionById(db, mainTransactionId).Data.drink_status != (int)GoEat.Models.TransactionStatus.Success)
                    {
                        return repo.UpdateMainTransactionStatusForDrink(db, mainTransactionId, status);
                    }
                }
                return false;
            }
        }

        public bool UpdateMainTransactionStatus(bool isFood, string orderId, int mainTransactionId, int status)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (isFood)
                {
                    if (repo.GetMainTransactionById(db, mainTransactionId).Data.food_status != (int)GoEat.Models.TransactionStatus.Success)
                    {
                        return repo.UpdateMainTransactionStatusForFood(db, orderId, mainTransactionId, status);
                    }
                }
                else
                {
                    if (repo.GetMainTransactionById(db, mainTransactionId).Data.drink_status != (int)GoEat.Models.TransactionStatus.Success)
                    {
                        return repo.UpdateMainTransactionStatusForDrink(db, orderId, mainTransactionId, status);
                    }
                }
                return false;
            }
        }

        public decimal CalculateSubtotal(decimal originalPrice, decimal discountRate, decimal gst, decimal serviceCharge)
        {
            decimal amountAfterDiscount = originalPrice * (1 - discountRate);
            decimal amountAfterServiceCharge = Math.Round(amountAfterDiscount * serviceCharge, 2);
            decimal amountAfterGst = Math.Round((amountAfterDiscount + amountAfterServiceCharge) * gst, 2);

            return Math.Round((amountAfterDiscount + amountAfterServiceCharge + amountAfterGst), 2);
        }

        public Result<MainTransaction> GetMainTransaction(int mainTransactionId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetMainTransactionById(db, mainTransactionId);
            }
        }

        public Result<ModifyTransaction> GetModifyTransaction(int mainTransactionId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetModifyTransactionById(db, mainTransactionId);
            }
        }

        public Result<ConfirmTransaction> GetConfirmTransactionById(int mainTransactionId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetConfirmTransactionById(db, mainTransactionId);
            }
        }

        public Result<SimpleTransaction> GetSimpleTransactionById(int mainTransactionId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetSimpleTransactionById(db, mainTransactionId);
            }
        }
        public Result<ConfirmTransaction> GetConfirmTokenTransactionById(int mainTransactionId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetConfirmTokenTransactionById(db, mainTransactionId);
            }
        }

        public Result<CreditBalance> GetCustomerTotalToken(int customer_id)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerTotalToken(db, customer_id);
            }
        }

        public int CreatePendingTransaction(int cashierId, int restaurantId, int customerId)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    var repo = GoEatRepo.Instance;
                    using (var db = repo.OpenConnectionFromPool())
                    {
                        MainTransaction mainTransaction = new MainTransaction
                        {
                            cashier_id = cashierId,
                            created_date = DateTime.UtcNow,
                            method = ConstantValues.S_MAKE_PAYMENT,
                            restaurant_id = restaurantId,
                            customer_id = customerId
                        };

                        var mainTransactionId = repo.CreateMainTransaction(db, mainTransaction);
                        transactionScope.Complete();

                        return mainTransactionId;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }


        public bool UpdateFinalTransaction(string gtokenTransactionId, string description,
            int mainTransactionId, string status, int customerId, decimal tokenAmount)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    var repo = GoEatRepo.Instance;
                    using (var db = repo.OpenConnectionFromPool())
                    {
                        bool isGTokenTransactionSuccess = true;
                        if (!string.IsNullOrEmpty(gtokenTransactionId) && !string.IsNullOrEmpty(description))
                        {
                            // isGTokenTransactionSuccess = UpdateCashTransaction(cashTransactionId, description);
                        }

                        if (isGTokenTransactionSuccess)
                        {
                            //if (UpdateMainTransactionStatus(mainTransactionId, ConstantValues.S_SUCCESS))
                            //{
                            //    if (tokenAmount > 0)
                            //    {
                            //        MinusTokenOfCustomer(customerId, tokenAmount);
                            //    }
                            //}
                        }

                        transactionScope.Complete();

                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool CheckExistOrderId(string orderId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CheckExistOrderId(db, orderId);
            }
        }

        public async Task<ReconcileTransactionResult> ReconcileTransaction(ReconcileTransaction listTransaction)
        {
            var repo = GoEatRepo.Instance;
            ReconcileTransactionResult result = new ReconcileTransactionResult();
            string error_message = string.Empty;
            ReconcileTransactionItem itemError = new ReconcileTransactionItem();
            int tranId = 0;

            using (var db = repo.OpenConnectionFromPool())
            {
                foreach (var item in listTransaction.success_ids)
                {
                    Int32.TryParse(item, out tranId);
                    var transaction = repo.GetMainTransactionById(db, tranId).Data;

                    if (transaction == null)
                    {
                        error_message = ErrorCodes.InvalidTransactionId.ToString();
                        itemError = new ReconcileTransactionItem { id = item, reason = error_message };
                        result.fail.Add(itemError);
                        continue;
                    }

                    var resultExecuteGtoken = await ExecuteGTokenTransaction(new GTokenTransaction
                    {
                        status = ConstantValues.S_SUCCESS,
                        gtoken_transaction_id = transaction.food_gtoken_transaction_id
                    });

                    if (!resultExecuteGtoken.Succeeded)
                    {
                        error_message = ConstantValues.S_ERROR_FROM_GTOKEN_API
                            + resultExecuteGtoken.Error.ToString();
                        itemError = new ReconcileTransactionItem { id = item, reason = error_message };
                        result.fail.Add(itemError);

                        //Update to reconcile fail
                        repo.UpdateMainTransactionStatusForFood(db, tranId, (int)GoEat.Models.TransactionStatus.Reconcile_fail);
                        continue;
                    }

                    if (repo.UpdateMainTransactionStatusForFood(db, tranId,
                        (int)GoEat.Models.TransactionStatus.Success))
                    {
                        result.success.Add(item);
                    }
                    else
                    {
                        error_message = ErrorCodes.ServerError.ToString();
                        itemError = new ReconcileTransactionItem { id = item, reason = error_message };
                        result.fail.Add(itemError);
                    }
                }

                tranId = 0;
                foreach (var item in listTransaction.fail_ids)
                {
                    Int32.TryParse(item, out tranId);
                    if (tranId == 0)
                    {
                        error_message = ErrorCodes.InvalidTransactionId.ToString();
                        itemError = new ReconcileTransactionItem { id = item, reason = error_message };
                        result.fail.Add(itemError);
                        continue;
                    }

                    if (repo.UpdateMainTransactionStatusForFood(db, tranId, (int)GoEat.Models.TransactionStatus.Failure))
                    {
                        result.success.Add(item);
                    }
                    else
                    {
                        error_message = ErrorCodes.ServerError.ToString();
                        itemError = new ReconcileTransactionItem { id = item, reason = error_message };
                        result.fail.Add(itemError);
                    }
                }
            }

            return result;
        }

        public bool UpdateTransOrderId(int mainTransactionId, string order_id)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateTransOrderId(db, mainTransactionId, order_id);
            }
        }

        public bool UpdateTransGTokenTransactionId(bool isDrink, int mainTransactionId, string gtoken_transaction_id)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                string sql = @"UPDATE [dbo].[main_transaction]
                SET food_gtoken_transaction_id = @gtoken_transaction_id
                WHERE id = @mainTransactionId";
                if (isDrink)
                {
                    sql = @"UPDATE [dbo].[main_transaction]
                SET drink_gtoken_transaction_id = @gtoken_transaction_id
                WHERE id = @mainTransactionId";
                }
                return repo.UpdateTransGTokenTransactionId(db, sql, mainTransactionId, gtoken_transaction_id);
            }
        }

        public Result<List<ReconcileTransactionModel>> GetReconcileTransactionModel(int restaurantId, string status,
            DateTime fromDate, DateTime toDate)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetReconcileTransactionModel(db, restaurantId, status, fromDate, toDate);
            }
        }


        public bool UpdateCashTransaction(MainTransaction transaction)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateCashTransaction(db, transaction);
            }
        }

        public bool UpdateTokenTransaction(MainTransaction transaction)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateTokenTransaction(db, transaction);
            }
        }
    }
}
