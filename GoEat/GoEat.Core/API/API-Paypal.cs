using GoEat.Dal;
using GoEat.Dal.Models;
using GoEat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GoEat.Core
{
    public partial class GoEatApi
    {
        public int CreatePaypalPendingTransaction(TokenTransaction tokenTransaction, decimal finalAmount, int customerId, int restaurantId)
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
                            //TODO: this code will be applied after we use token
                            //token_transaction_id = repo.CreateTokenTransaction(db, tokenTransaction),
                            drink_status = (int)GoEat.Models.TransactionStatus.Pending,
                            created_date = DateTime.Now,
                            amount = finalAmount,
                            method = ConstantValues.S_PAYPAL_BUY_TOKEN,
                            customer_id = customerId,
                            restaurant_id = restaurantId
                        };

                        var mainTransactionId = repo.CreatePaypalMainTransaction(db, mainTransaction);
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


        /// <summary>
        /// After finshed with paypal, add token to user's balance and update transaction's status = success
        /// </summary>
        /// <param name="description"></param>
        /// <param name="mainTransactionId"></param>
        /// <param name="status"></param>
        /// <param name="customerId"></param>
        /// <param name="tokenAmount"></param>
        /// <returns></returns>
        public bool FinishPaypalFinalTransaction(string orderId, int mainTransactionId, int customerId, decimal tokenAmount)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    var repo = GoEatRepo.Instance;
                    using (var db = repo.OpenConnectionFromPool())
                    {
                        if (UpdateMainTransactionStatus(false, orderId, mainTransactionId, (int)GoEat.Models.TransactionStatus.Success))
                        {
                            AddTokenOfCustomer(customerId, tokenAmount);
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


        public Result<TokenPackage> GetTokenPackageBySKU(string sku)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTokenPackageBySKU(db, sku);
            }
        }
    }
}
