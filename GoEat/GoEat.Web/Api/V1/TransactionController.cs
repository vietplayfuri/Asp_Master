using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using AutoMapper;
using GoEat.Core;
using GoEat.Dal.Models;
using GoEat.Web.ActionFilter;
using GoEat.Web.Hubs;
using GoEat.Web.Models;
using GoEat.Dal;
using GoEat.Models;
using GoEat.Utility.Crytography;
using System.Web.Http;
using System.Threading.Tasks;
using GoEat.Dal.Common;
using System.Data.SqlTypes;

namespace GoEat.Web.Api.V1
{
    public class TransactionController : BaseApiController
    {

        [HttpPost]
        [ActionName("check-barcode")]
        public object CheckBarcode([FromBody] BarcodeToken param)
        {
            try
            {
                if (param == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                var tokenDecode = Cryptogahpy.Base64Decode(param.token);
                if (string.IsNullOrEmpty(tokenDecode) || tokenDecode.Split('-').Count() != 3)
                {
                    return Json(new
                    {
                        success = false,
                        error_code = ErrorCodes.InvalidBarCode.ToString()
                    });
                }

                var api = GoEatApi.Instance;
                string[] values = tokenDecode.Split('-');
                int userId = Int32.Parse(values[1]);
                int restaurantId = Int32.Parse(values.First());

                var discount = api.GetCurrentDiscount(userId, restaurantId).Data;
                if (discount != null
                    && !discount.is_activated
                    && discount.id == Int32.Parse(values.Last()))
                {
                    var currentUser = api.GetUserById(userId);
                    var model = new TransactionViewModel
                    {
                        customer_id = userId,
                        nickname = currentUser.Data.Profile.nickname,
                        restaurant_id = restaurantId,
                        discount_percentage = discount.rate,
                        rateTokenWithSGD = ConstantValues.D_RATE_TOKEN_SGD,
                        service_charge = ConstantValues.D_SERVICE_CHARGE,
                        gst = ConstantValues.D_GST,
                        //TODO: consider if we need to call service to get exactly gtoken in this time or we get from DB simply
                        token_balance = currentUser.Data.Profile.gtoken == null
                            ? 0
                            : currentUser.Data.Profile.gtoken.Value,
                        is_venvici_member = currentUser.Data.Profile.is_venvici_member,
                        username = currentUser.Data.Profile.account
                    };
                    return Json(new
                    {
                        success = true,
                        order = model
                    });
                }

                return Json(new
                {
                    success = false,
                    error_code = ErrorCodes.InvalidBarCode.ToString()
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    error_code = ErrorCodes.InvalidBarCode.ToString()
                });
            }
        }

        public void NotifyFinalAmountToUser(int userId, TransactionInformation shownTransaction)
        {
            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connectionId in NotifyHub._connections.GetConnections(userId.ToString()))
            {
                hubContext.Clients.Client(connectionId).showTransactionConfirmation(shownTransaction);
            }
        }

        [HttpPost]
        [RBAC(AccessAction = "do_transaction")]
        [ActionName("create-transaction")]
        public object CreatePendingTransaction([FromBody] TransactionModel model)
        {
            try
            {
                if (model == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    var goEatApi = GoEatApi.Instance;
                    var discount = goEatApi.GetSimpleDiscount(model.customer_id, model.restaurant_id).Data;
                    if (discount != null)
                    {
                        var customer = goEatApi.GetUserById(model.customer_id);
                        if (!customer.Data.Profile.is_venvici_member || (model.cash_amount == 0 && model.drinks_quantity == 0))
                        {
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        if (model.drinks_quantity > 0)
                        {
                            var gtoken_amount = model.drinks_quantity * ConstantValues.D_TRANSACTION_DRINK_PRICE;
                            var result = goEatApi.CheckGTokenBalance(customer.Data.username, gtoken_amount).Result;
                            if (!result.Succeeded || (result.HasData && result.Data.success == false))
                            {
                                return new
                                {
                                    success = false,
                                    error_code = ErrorCodes.INSUFFICIENT_AMOUNT.ToString()
                                };
                            }
                        }
                        decimal finalAmount = goEatApi.CalculateSubtotal(model.original_price, discount.rate, ConstantValues.D_GST, ConstantValues.D_SERVICE_CHARGE);

                        if (model.cash_amount > 0 && finalAmount != (model.cash_amount))
                        {
                            return new
                            {
                                success = false,
                                error_code = ErrorCodes.SubtotalIsWrong.ToString()
                            };
                        }

                        return CreatePendingTransaction(model, finalAmount, discount.rate);
                    }
                    return new
                    {
                        success = false,
                        error_code = ErrorCodes.NonExistingUser.ToString()
                    };
                }
                return new
                {
                    success = false,
                    error_code = ErrorCodes.MissingFields.ToString()
                };
            }
            catch
            {
                return new
                {
                    success = false,
                    error_code = ErrorCodes.ServerError.ToString()
                };
            }
        }


        [HttpPost]
        [RBAC(AccessAction = "do_transaction")]
        [ActionName("resend")]
        public object ReShowConfirmedPopup([FromBody] UserConfirmTransaction model)
        {
            try
            {
                if (model == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                var goEatApi = GoEatApi.Instance;
                var result = goEatApi.GetModifyTransaction(model.id).Data;
                bool isSuccess = false;

                if (result != null)
                {
                    if (result.drink_status == (int)GoEat.Models.TransactionStatus.Pending)
                    {
                        TransactionInformation transaction = new TransactionInformation
                        {
                            original_price = result.original_price,
                            amount = result.token_amount,
                            order_id = result.order_id,
                            main_transaction_id = result.id,
                            quantity = result.drinks,
                        };
                        NotifyFinalAmountToUser(result.customer_id, transaction);
                        isSuccess = true;
                        return new
                        {
                            success = isSuccess,
                            mainTransactionId = model.id,
                            status = isSuccess ? result.drink_status : 0,
                            error_code = isSuccess ? string.Empty : ErrorCodes.TransactionNotFound.ToString()
                        };
                    }
                    else
                    {
                        return new
                        {
                            success = false,
                            mainTransactionId = model.id,
                            status = result.drink_status,
                            error_code = string.Empty
                        };
                    }
                }

                return new
                {
                    success = isSuccess,
                    mainTransactionId = model.id,
                    status = isSuccess ? result.drink_status : 0,
                    error_code = isSuccess ? string.Empty : ErrorCodes.TransactionNotFound.ToString()
                };
            }
            catch
            {
                return new
                {
                    success = false,
                    error_code = ErrorCodes.ServerError.ToString()
                };
            }
        }

        private object CreatePendingTransaction(TransactionModel model,
            decimal finalAmount, decimal discountRate)
        {
            GoEatApi goEatApi = GoEatApi.Instance;
            var user = goEatApi.GetUserById(model.customer_id);
            int mainTransactionId = mainTransactionId = goEatApi.CreatePendingTransaction(CurrentUser.Id, model.restaurant_id, model.customer_id);
            bool isSuccess = false;

            if (mainTransactionId > 0)
            {
                model.order_id = String.Format(ConstantValues.S_TRANSACTION_ORDER_ID_FORMAT,
                    Urls.GetRestaurantId(), mainTransactionId);
                var trans = goEatApi.GetMainTransaction(mainTransactionId);
                trans.Data.order_id = model.order_id;
                if (model.cash_amount > 0)
                {
                    MainTransaction updatedTrans = CreateCashTransactionEntity(trans.Data, model);

                    goEatApi.UpdateCashTransaction(updatedTrans);

                    string gTokenTransactionId = CreateGTokenTransaction(discountRate, model.order_id + "_Food",
                        model.original_price, user.Data.username,
                        ConstantValues.S_DESCRIPTION_AMOUNT_WITH_CASH, ConstantValues.D_GST,
                        ConstantValues.D_SERVICE_CHARGE);

                    if (string.IsNullOrEmpty(gTokenTransactionId))
                    {
                        goEatApi.UpdateMainTransactionStatus(true, mainTransactionId, (int)GoEat.Models.TransactionStatus.Failure);

                        return new
                        {
                            success = isSuccess,
                            error_code = ErrorCodes.ServerError.ToString()
                        };
                    }

                    goEatApi.UpdateTransGTokenTransactionId(false, mainTransactionId, gTokenTransactionId);
                }
                if (model.drinks_quantity > 0)
                {
                    return CreatePendingTokenTransaction(mainTransactionId, model);
                }

                isSuccess = true;
            }
            return new
            {
                success = isSuccess,
                mainTransactionId = mainTransactionId,
                error_code = isSuccess ? string.Empty : ErrorCodes.ServerError.ToErrorMessage()
            };
        }

        private object CreatePendingTokenTransaction(int mainTransactionId, TransactionModel model)
        {
            GoEatApi goEatApi = GoEatApi.Instance;

            var trans = goEatApi.GetMainTransaction(mainTransactionId);

            MainTransaction updatedTrans = CreateTokenTransactionEntity(trans.Data, model);

            goEatApi.UpdateTokenTransaction(updatedTrans);

            bool isSuccess = false;

            if (mainTransactionId > 0)
            {

                TransactionInformation transaction = new TransactionInformation
                {
                    quantity = updatedTrans.drinks,
                    main_transaction_id = mainTransactionId,
                    amount = updatedTrans.token_amount
                };
                NotifyFinalAmountToUser(model.customer_id, transaction);
                isSuccess = true;
            }
            return new
            {
                success = isSuccess,
                mainTransactionId = mainTransactionId,
                error_code = isSuccess ? string.Empty : ErrorCodes.ServerError.ToErrorMessage()
            };
        }

        private string CreateGTokenTransaction(decimal discountRate, string orderId,
decimal originalPrice, string username, string description, decimal original_tax, decimal original_service_charge)
        {
            decimal amountAfterDiscount = originalPrice * (1 - discountRate);
            decimal amountServiceCharge = amountAfterDiscount * original_service_charge;
            decimal amountGst = amountAfterDiscount * original_tax;

            var gTokenTransaction = GoEatApi.Instance.CreateGTokenTransaction(new GTokenTransaction
            {
                currency = ConstantValues.S_CURRENCY_SGD,
                discount_percentage = discountRate,
                order_id = orderId,
                original_currency = ConstantValues.S_CURRENCY_SGD,
                original_price = originalPrice,
                original_final_amount = originalPrice * (1 - discountRate),
                username = username,
                status = ConstantValues.S_PENDING,
                payment_method = ConstantValues.S_PAYMENT_METHOD,
                description = description,
                is_venvici_applicable = true,
                original_tax = amountGst,
                original_service_charge = amountServiceCharge,
                revenue_percentage = ConstantValues.D_REVENUE_PERCENTAGE
            });

            if (!gTokenTransaction.Succeeded)
            {
                return string.Empty;
            }

            return gTokenTransaction.Data.transaction.gtoken_transaction_id;
        }



        private MainTransaction CreateTokenTransactionEntity(MainTransaction tokenTransaction, TransactionModel model)
        {
            // tokenTransaction.currency = ConstantValues.S_CURRENCY_SGD;
            // tokenTransaction.gst = ConstantValues.D_GST;
            // tokenTransaction.service_charge = ConstantValues.D_SERVICE_CHARGE;
            tokenTransaction.order_id = model.order_id;
            tokenTransaction.description = ConstantValues.S_TRANSACTION_TYPE_DRINK;
            tokenTransaction.drink_status = (int)GoEat.Models.TransactionStatus.Pending;
            tokenTransaction.drinks = model.drinks_quantity;
            tokenTransaction.token_amount = model.drinks_quantity * ConstantValues.D_TRANSACTION_DRINK_PRICE;
            return tokenTransaction;
        }

        private MainTransaction CreateCashTransactionEntity(MainTransaction cashTransaction, TransactionModel model)
        {
            cashTransaction.order_id = model.order_id;
            cashTransaction.original_price = model.original_price;
            cashTransaction.discount_percentage = model.discount_percentage;
            cashTransaction.original_currency = ConstantValues.S_CURRENCY_SGD;
            cashTransaction.currency = ConstantValues.S_CURRENCY_SGD;
            cashTransaction.gst = ConstantValues.D_GST;
            cashTransaction.service_charge = ConstantValues.D_SERVICE_CHARGE;
            cashTransaction.amount = model.cash_amount;
            cashTransaction.description = ConstantValues.S_TRANSACTION_TYPE_FOOD;
            cashTransaction.food_status = (int)GoEat.Models.TransactionStatus.Pending_reconcile;
            return cashTransaction;
        }

        #region After cashier click create transaction, we will redirect to new page
        /// <summary>
        /// Check transaction status
        /// if approved --> redirect to receipt
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("check-transaction-status")]
        public object CheckTransactionStatus([FromBody] MainTransactionInfo info)
        {
            try
            {
                var mainTransaction = GoEatApi.Instance.GetMainTransaction(info.MainTransactionId).Data;
                return new
                {
                    success = true,
                    status = mainTransaction.drink_status,
                    mainTransactionId = mainTransaction.id
                };
            }
            catch
            {
                return new
                {
                    success = false,
                    error_code = ErrorCodes.ServerError.ToString()
                };
            }
        }

        /// <summary>
        /// cashier can click cancel transaction to cancel a transaction by id
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [RBAC(AccessAction = "do_transaction")]
        [ActionName("cancel-transaction")]
        public object CancelTransaction([FromBody] MainTransactionInfo info)
        {
            try
            {
                var goEatApi = GoEatApi.Instance;

                if (goEatApi.UpdateMainTransactionStatus(false, info.MainTransactionId, (int)GoEat.Models.TransactionStatus.Cancelled))
                {
                    CloseUserConfirmPopup(goEatApi.GetMainTransaction(info.MainTransactionId).Data.customer_id);
                    return new
                    {
                        success = true
                    };
                }
                return new
                {
                    success = false,
                    error_code = ErrorCodes.ServerError.ToString()
                };
            }
            catch
            {
                return new
                {
                    success = false,
                    error_code = ErrorCodes.ServerError.ToString()
                };
            }
        }
        public void CloseUserConfirmPopup(int userId)
        {
            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connectionId in NotifyHub._connections.GetConnections(userId.ToString()))
            {
                hubContext.Clients.Client(connectionId).closePopupConfirm();
            }
        }

        /// <summary>
        /// cashier click edit on the new page, the current page will redirect to payment page
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [RBAC(AccessAction = "do_transaction")]
        [ActionName("get-transaction")]
        public object GetTransactionById([FromBody] MainTransactionInfo info)
        {
            try
            {
                return new
                {
                    success = true,
                    transaction = GoEatApi.Instance.GetModifyTransaction(info.MainTransactionId).Data
                };
            }
            catch
            {
                return new
                {
                    success = false,
                    error_code = ErrorCodes.ServerError.ToString()
                };
            }
        }
        #endregion

        [HttpPost]
        [RBAC(AccessAction = "do_transaction")]
        [ActionName("reconcile")]
        public async Task<object> ReconcileTransaction([FromBody] ReconcileTransaction listTransaction)
        {
            var api = GoEatApi.Instance;

            if (listTransaction == null)
            {
                return Json(new
                {
                    success = false,
                    error_code = ErrorCodes.MissingFields.ToString()
                });
            }

            var result = await api.ReconcileTransaction(listTransaction);

            return Json(new
            {
                success = result.fail.Any() ? false : true,
                item_success = result.success.Any() ? result.success : null,
                item_fail = result.fail.Any() ? result.fail : null,
            });
        }

        [RBAC(AccessAction = "do_transaction")]
        [ActionName("get-transactions")]
        public object GetTransactionsByStatus([FromUri] APIGetTransactionsModel param)
        {
            if (param == null)
            {
                param = new APIGetTransactionsModel
                {
                    from_date = SqlDateTime.MinValue.Value,
                    to_date = SqlDateTime.MaxValue.Value,
                    status = GoEat.Utility.Helper.GetDescription(TransactionStatus.Pending)
                };
            }
            else
            {
                TransactionStatus tranStatus = TransactionStatus.Pending;
                bool isStatus = Enum.TryParse<TransactionStatus>(param.status, out tranStatus);
                param.status = GoEat.Utility.Helper.GetDescription(tranStatus);
                param.to_date = param.to_date == SqlDateTime.MinValue.Value
                    ? param.to_date = SqlDateTime.MaxValue.Value
                    : param.to_date;
            }

            var api = GoEatApi.Instance;
            var transactions = api.GetReconcileTransactionModel(Urls.GetRestaurantId(), param.status, param.from_date, param.to_date);

            return new
            {
                success = transactions.HasData,
                transactions = transactions.HasData ? transactions.Data : null,
                error_code = transactions.HasData ? string.Empty : transactions.Error.Value.ToString()
            };
        }
    }
}
