using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GoEat.Core;
using GoEat.Dal.Common;
using GoEat.Dal.Models;
using GoEat.Web.Helpers;
using GoEat.Web.Hubs;
using GoEat.Web.Models;
using GoEat.Dal;
using GoEat.Models;
using GoEat.Utility.Crytography;
using PayPal.Api;
using GoEat.Web.ActionFilter;
using System.Web.Mvc;
using System.IO;
using GoEat.Utility;
using System.Data.SqlTypes;

namespace GoEat.Web.Controllers
{
    [Authorize]
    [RoutePrefix("transaction")]
    public class TransactionController : BaseController
    {
        #region history transaction
        private int getLastPageIndex(int totalRow, int pageSize)
        {
            int lastPageIndex = totalRow / pageSize;
            if (totalRow % pageSize > 0)
            {
                lastPageIndex += 1;
            }
            return lastPageIndex;
        }

        // GET: Transaction, for admin, cashier --> they can see all customer 's transaction in their restaurants
        [RBAC(AccessAction = "access_transaction")]
        public ActionResult Index(int? page)
        {
            //paging
            var api = GoEat.Core.GoEatApi.Instance;
            page = (page ?? 1);
            const int pageSize = 10;

            var totalRow = api.CountAdminTransactions(Urls.GetRestaurantId(), TransactionStatus.Success);
            var lastPageIndex = getLastPageIndex(totalRow, pageSize);

            page = page <= lastPageIndex ? page : 1;

            int startIndex = page == 1 ? 1 : (((page.Value - 1) * pageSize) + 1);
            int endIndex = startIndex + pageSize - 1;

            ViewBag.currentPageIndex = page;
            ViewBag.lastPageIndex = lastPageIndex;

            var result = api.GetTransactions(startIndex, endIndex, Urls.GetRestaurantId(), TransactionStatus.Success);
            var transactions = new TransactionsViewModel();
            if (result.Succeeded)
            {
                transactions.transactions = result.Data;
            }

            return View(transactions);
        }


        [HttpPost]
        [ActionName("exportcsv")]
        [RBAC(AccessAction = "access_transaction")]
        public ActionResult ExportCsv(int? page, string status, DateTime? start, DateTime? end, string timeZone)
        {
            var api = GoEat.Core.GoEatApi.Instance;
            start = start == null ? SqlDateTime.MinValue.Value : Helper.timeFromString(start.ToString(), timeZone);
            end = end == null ? SqlDateTime.MaxValue.Value : Helper.timeFromString(end.ToString(), timeZone);
            var dataTransaction = api.GetTransactions(Urls.GetRestaurantId(), status, start.Value, end.Value).Data;

            StringWriter strWriter = new StringWriter();
            strWriter.WriteLine("TranID, Restaurant name, Username, Order Id, Status, Final Amount, Method, Gtoken_transaction_id, Currency, Original_currency, Original_price, Discount_percentage, Gst, Service_charge, Description, Create_at\n");

            var est = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            foreach (var line in dataTransaction)
            {
                DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_date, est);
                strWriter.WriteLine(
                    string.Format("=\"{0}\",{1},{2},{3},{4},=\"{5}\",{6},=\"{7}\",{8},{9},{10},{11},{12},{13},{14},=\"{15}\"",
                    line.id,
                    line.restaurant_name,
                    line.username,
                    line.order_id,
                    Helper.GetDescription((GoEat.Models.TransactionStatus)line.food_status),
                    line.amount,
                    line.method,
                    line.gtoken_transaction_id,
                    line.currency,
                    line.original_currency,
                    line.original_price,
                    line.discount_percentage,
                    line.gst,
                    line.service_charge,
                    line.description,
                    targetTime
                ));
            }

            Response.ClearContent();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename=\"{0}\"", "transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv"));
            Response.Write(strWriter.ToString());
            Response.End();
            return new HttpStatusCodeResult(200);
        }

        //for normal user
        [Route("transactions")]
        //public ActionResult transactions(int? page)
        public ActionResult transactions()
        {
            if (this.HasPermission("access_transaction"))
            {
                return RedirectToAction("index", "transaction");
            }
            return View();
        }

        // Shown all login 's transaction
        [HttpPost]
        [ActionName("get-transaction")]
        public ActionResult GetTransactions(int? page, int fromDay, int fromMonth, int fromYear, int toDay, int toMonth, int toYear)
        {
            if (this.HasPermission("access_transaction"))
            {
                return RedirectToAction("index", "transaction");
            }

            DateTime fromDate = new DateTime(fromYear, fromMonth, fromDay);
            DateTime toDate = new DateTime(toYear, toMonth, toDay);
            toDate = toDate.AddMinutes(1439);
            var api = GoEat.Core.GoEatApi.Instance;

            //paging
            page = (page ?? 1);
            const int pageSize = 10;

            var totalRow = api.CountCustomerTransactions(CurrentUser.Id, fromDate, toDate);
            var lastPageIndex = getLastPageIndex(totalRow, pageSize);

            page = page <= lastPageIndex ? page : 1;

            int startIndex = page == 1 ? 1 : (((page.Value - 1) * pageSize) + 1);
            int endIndex = startIndex + pageSize - 1;

            ViewBag.currentPageIndex = page;
            ViewBag.lastPageIndex = lastPageIndex;

            var result = api.GetTransactions(startIndex, endIndex, CurrentUser.Id, fromDate, toDate);
            var transactions = new TransactionsViewModel();
            if (result.Succeeded)
            {
                transactions.transactions = result.Data;
            }
            return PartialView("_transactionHistory", transactions);
        }
        #endregion

        [Route("order")]
        [RBAC(AccessAction = "do_transaction")]
        public ActionResult Order()
        {
            var token = Request.Params["token"];
            var tokenDecode = Cryptogahpy.Base64Decode(token);
            if (!String.IsNullOrEmpty(tokenDecode))
            {
                if (tokenDecode.Split('-').Count() != 3)
                {
                    ModelState.AddModelError("CustomeError", "This code is invalid!");
                    return View();
                }
                var api = GoEatApi.Instance;
                var userId = Int32.Parse(tokenDecode.Split('-')[1]);
                var restaurantId = Int32.Parse(tokenDecode.Split('-').First());
                var discount = api.GetCurrentDiscount(userId, restaurantId);
                if (discount.Succeeded)
                {
                    if (!discount.Data.is_activated)
                    {
                        if (discount.Data.id == Int32.Parse(tokenDecode.Split('-').Last()))
                        {
                            // code is valid and can create new transaction
                            var model = new TransactionViewModel
                            {
                                customer_id = userId,
                                nickname = api.GetUserById(userId).Data.Profile.nickname,
                                restaurant_id = restaurantId,
                                discount_percentage = discount.Data.rate
                            };

                            return View(model);
                        }
                        ViewBag.CustomeError = "This code is invalid!";
                    }
                    else
                    {
                        ViewBag.CustomeError = "This code was activated!";
                    }
                }
                else
                {
                    ViewBag.CustomeError = "This code is invalid!";
                }
            }
            return View();
        }


        //create the transaction
        [HttpPost]
        [Route("order")]
        [RBAC(AccessAction = "do_transaction")]
        [ValidateAntiForgeryToken]
        public ActionResult Order(TransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var api = GoEat.Core.GoEatApi.Instance;
                //var discount = api.GetCurrentDiscount(model.customer_id, model.restaurant_id);
                //if (discount.Succeeded && !discount.Data.is_activated)
                //{
                //    model.cashier_id = CurrentUser.Id;
                //    model.final_amount = model.price - (model.price * model.discount_percentage);

                //    var transaction = new Transaction()
                //    {
                //        customer_id = model.customer_id,
                //        restaurant_id = model.restaurant_id,
                //        order_id = model.order_id,
                //        price = model.price,
                //        created_at = DateTime.Now,
                //        discount_percentage = model.discount_percentage,
                //        final_amount = model.final_amount,
                //        cashier_id = model.cashier_id
                //    };
                //    //create transaction
                //    var rs = api.CreateTransaction(transaction);
                //    if (rs.Succeeded)
                //    {
                //        //set status of user bardcode
                //        api.UpdateUserDiscount(transaction.customer_id, discount.Data.id, false);//set false now for testing
                //    }

                //    //notifiy to user
                //    NotifyUser(model.customer_id, "Your code have been activated successfully!");
                //    return RedirectToAction("Index");
                //}
            }
            return View(model);
        }

        [Authorize]
        [Route("buytoken")]
        public ActionResult BuyToken()
        {
            var model = new BuyTokenViewModel();
            var goEatAPI = GoEatApi.Instance;

            var creditBalance = goEatAPI.GetCustomerTotalToken(CurrentUser.Id);

            if (creditBalance.HasData)
            {
                model.TotalToken = creditBalance.Data.token;
            }
            return View(model);
        }

        #region paypal
        [Authorize]
        [Route("paypal")]
        public ActionResult Paypal(TransactionPayPal modal)
        {
            var transactionDesc = "Transaction description.";

            //var invoiceNumber = "007"; //This is option. The invoiceNumber is unique which generated from our server
            var api = GoEat.Core.GoEatApi.Instance;
            var package = api.GetTokenPackageBySKU(modal.sku);
            if (!package.Succeeded)
            {
                return View("buytoken");
            }

            string skuItem = package.Data.sku;
            string nameItem = package.Data.name;
            decimal priceItem = package.Data.price;
            string currency = package.Data.currency.Trim();
            int quantityItem = 1;

            decimal totalAmout = priceItem * quantityItem;

            var helper = new PaypalHelper();
            var guid = Guid.NewGuid();
            var basePaypalReturn = GetPaypalReturn(guid.ToString());
            var returnUrl = basePaypalReturn + "&Success=True";
            var cancelUrl = basePaypalReturn + "&Success=False";

            var itemList = new List<Item>();
            var item = helper.CreateItem(nameItem, currency, priceItem, quantityItem, skuItem);
            itemList.Add(item);

            var amount = helper.CreateAmount(currency, totalAmout);
            var redirectUrls = helper.CreateRedirectUrls(returnUrl, cancelUrl);
            var transactionList = new List<PayPal.Api.Transaction>();
            var transaction = helper.CreateTransaction(transactionDesc, amount, itemList);
            transactionList.Add(transaction);
            try
            {
                var createdPayment = helper.CreatePayment(helper.GetGetAPIContext(), transactionList, redirectUrls);
                var links = createdPayment.links.GetEnumerator();
                string paypalRedirectUrl = null;

                while (links.MoveNext())
                {
                    Links lnk = links.Current;

                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment
                        paypalRedirectUrl = lnk.href;

                        //Save information of transaction into Database
                        //State of transaction is pending
                        //---------------------------------------------
                        var transactionId = api.CreatePaypalPendingTransaction(CreatePaypalTransactionEntity(package.Data, createdPayment), totalAmout, CurrentUser.Id, Urls.GetRestaurantId());
                        Session.Add("transactionId", transactionId);
                    }
                }

                // saving the paymentID in the key guid
                Session.Add(guid.ToString(), createdPayment.id);

                return Redirect(paypalRedirectUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private TokenTransaction CreatePaypalTransactionEntity(TokenPackage package, Payment payment)
        {
            var transaction = new TokenTransaction();

            transaction.amount = package.token_amount;
            transaction.currency = package.currency;
            transaction.original_currency = ConstantValues.S_CURRENCY_SGD;
            transaction.original_price = package.price;
            transaction.token_package_id = package.id;
            transaction.paypal_payment_id = payment.id;

            return transaction;
        }


        [Route("paypalreturn")]
        public async Task<ActionResult> PaypalReturn()
        {
            string payerId = Request.Params["PayerID"];
            var guid = Request.Params["guid"];
            var success = Request.Params["Success"];

            if (guid != null && success != null)
            {
                var isSuccess = Convert.ToBoolean(Request.QueryString["Success"]);
                var transactionID = Session[guid] as string;

                //If client buy succesful, process here
                if (isSuccess)
                {
                    var helper = new PaypalHelper();
                    APIContext apiContext = helper.GetGetAPIContext();

                    var paymentExecution = new PaymentExecution() { payer_id = payerId };
                    var payment = new Payment() { id = transactionID };

                    //We need to execute to submit paypal about client's transaction
                    //If we don't execute paypal, the transaction will be cancel, the buyer don't charge money
                    //--------------------------------------------------------------
                    try
                    {
                        var executedPayment = payment.Execute(apiContext, paymentExecution);
                        if (executedPayment.state.ToLower() != "approved")
                        {
                            //Save information of transaction into Database
                            //when executedPayment is not approved
                            //---------------------------------------------
                            var transactionId = Session["transactionId"];
                            var goEatApi = GoEatApi.Instance;
                            // goEatApi.UpdateMainTransactionStatus(Int32.Parse(transactionId.ToString()), ConstantValues.S_CANCELLED);
                            return View("PaypalFailure");
                        }
                        else
                        {
                            //Save information of transaction into Database
                            //State of transaction is approved
                            //---------------------------------------------
                            var goEatApi = GoEatApi.Instance;
                            var transactionId = Session["transactionId"];

                            var confirmTransactionResult = goEatApi.GetConfirmTokenTransactionById(Int32.Parse(transactionId.ToString()));
                            if (confirmTransactionResult.Succeeded)
                            {
                                if (confirmTransactionResult.Succeeded)
                                {
                                    goEatApi.FinishPaypalFinalTransaction(
                                        confirmTransactionResult.Data.paypal_payment_id,
                                        confirmTransactionResult.Data.id,
                                        confirmTransactionResult.Data.customer_id,
                                        confirmTransactionResult.Data.token_amount);

                                    var model = new PaypalSuccess() { amount = confirmTransactionResult.Data.token_amount };

                                    var customer = goEatApi.GetUserById(confirmTransactionResult.Data.customer_id);

                                    //create api recording
                                    await goEatApi.RecordTokenTransaction(new GRecordTokenTransaction
                                    {
                                        username = customer.Data.Profile.account,
                                        order_id = confirmTransactionResult.Data.paypal_payment_id,
                                        gtoken_transaction_id = string.Empty,
                                        token_type = ConstantValues.S_SUGAR_TOKEN,
                                        transaction_type = ConstantValues.S_TRANSACTION_TYPE_CONSUMPTION,
                                        amount = confirmTransactionResult.Data.token_amount,
                                        description = ConstantValues.S_DESCRIPTION_TOPUP_TOKEN
                                    });

                                    return View("PaypalSuccess", model);
                                }
                            }
                        }
                    }
                    catch
                    {
                        return View("PaypalFailure");
                    }

                }
                //If client cancel, process here
                else
                {
                    //Save information of transaction into Database
                    //When client cancel, Use transactionID to trace
                    //---------------------------------------------
                    var transactionId = Session["transactionId"];
                    var goEatApi = GoEatApi.Instance;
                    //goEatApi.UpdateMainTransactionStatus(Int32.Parse(transactionId.ToString()), ConstantValues.S_CANCELLED);

                    return RedirectToAction("buytoken");
                }
            }
            //All params is not valid, return valid view here
            return View("PaypalFailure");
        }

        private string GetPaypalReturn(string guid)
        {
            string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/transaction/paypalreturn?";
            var returnUrl = string.Format("{0}guid={1}", baseURI, guid);
            return returnUrl;
        }
        #endregion

        #region Helper
        public void NotifyFinalAmountToUser(int userId, string message)
        {
            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connectionId in NotifyHub._connections.GetConnections(userId.ToString()))
            {
                hubContext.Clients.Client(connectionId).SendMessage(CurrentUser.Id, message);
            }
        }


        #endregion

        #region Customer confirm the final transaction
        [HttpPost]
        [Authorize]
        [Route("cancel-transaction")]
        public JsonResult CancelTransaction(MainTransactionInfo model)
        {
            try
            {
                var goEatApi = GoEatApi.Instance;
                if (goEatApi.UpdateMainTransactionStatus(false, model.MainTransactionId, (int)GoEat.Models.TransactionStatus.Cancelled))
                {
                    return Json(new
                    {
                        success = true,
                        error_code = string.Empty
                    });
                }
                return Json(new
                {
                    success = false,
                    error_code = ErrorCodes.ServerError.ToString()
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    error_code = ErrorCodes.InvalidTransactionId.ToString()
                });
            }
        }

        [Authorize]
        [Route("create-transaction")]
        public JsonResult MakeFinalTransaction(MainTransactionInfo model)
        {
            try
            {
                var goEatApi = GoEatApi.Instance;
                var isSuccess = false;
                var transaction = goEatApi.GetSimpleTransactionById(model.MainTransactionId).Data;

                if (transaction != null)
                {
                    var gTokenTransaction = CreateDirectChargeGtoken(transaction.token_amount,
                        transaction.order_id + "_Drink", transaction.username, transaction.description);
                    if (gTokenTransaction.Succeeded && gTokenTransaction.HasData)
                    {
                        isSuccess = goEatApi.UpdateTransGTokenTransactionId(true, transaction.id, gTokenTransaction.Data.transaction.gtoken_transaction_id);
                        goEatApi.UpdateMainTransactionStatus(false, transaction.id, (int)GoEat.Models.TransactionStatus.Success);
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            error_code = gTokenTransaction.Error.ToErrorMessage()
                        });
                    }
                }
                return Json(new
                {
                    success = isSuccess,
                    error_code = isSuccess ? string.Empty : ErrorCodes.NonExistingUser.ToString()
                });
            }
            catch
            {
                return Json(new
                {
                    success = false,
                    error_code = ErrorCodes.InvalidTransactionId.ToString()
                });
            }
        }

        private async Task<JsonResult> UpdateFinalTransaction(GoEatApi goEatApi, ConfirmTransaction model, decimal discountRate)
        {
            Result<MainTransaction> mainTransaction = goEatApi.GetMainTransaction(model.id);

            var customer = goEatApi.GetUserById(model.customer_id);
            string gtoken_transaction_id = string.Empty;
            string description = string.Empty;// GToken.Dal.ConstantValues.S_DESCRIPTION;
            string encodeOrderId = Cryptogahpy.Base64Encode(string.Format("{0}-{1}-{2}", model.restaurant_id, model.order_id, model.customer_id));

            //if user is venvici member and their bill using cash only

            if (customer.Data.Profile.is_venvici_member
                && model.token_amount == 0
                && model.cash_amount > 0)
            {
                //Call creating transaction of GToken
                //var gTokenTransaction = await CreateGToken(goEatApi, discountRate, encodeOrderId, model.original_price,
                //    customer.Data.Profile.account, GToken.Dal.ConstantValues.S_DESCRIPTION);

                var gTokenTransaction = await CreateGTokenTransaction(goEatApi, discountRate, encodeOrderId, model.original_price,
                    customer.Data.Profile.account, ConstantValues.S_DESCRIPTION_AMOUNT_WITH_CASH, ConstantValues.D_GST, ConstantValues.D_SERVICE_CHARGE);

                if (!gTokenTransaction.Succeeded)
                {
                    //TODO: need update status fail for transaction and notify to user ???,
                    return Json(new
                    {
                        success = false,
                        error_code = "CreateGTokenTransaction:" + gTokenTransaction.Error.ToString()
                    });
                }

                var exeGTokenTransaction = await ExecuteGTokenTransaction(gTokenTransaction, goEatApi);

                if (!exeGTokenTransaction.Succeeded)
                {
                    //TODO: need update status fail for transaction and notify to user ???
                    return Json(new
                    {
                        success = false,
                        error_code = "ExecuteGTokenTransaction:" + exeGTokenTransaction.Error.ToString()
                    });
                }

                gtoken_transaction_id = gTokenTransaction.Data.transaction.gtoken_transaction_id;
                description = gTokenTransaction.Data.transaction.description;
            }
            else
            {
                decimal amountAfterDiscount = model.original_price * (1 - discountRate);
                decimal amountServiceCharge = amountAfterDiscount * ConstantValues.D_SERVICE_CHARGE;
                decimal amountGst = (amountAfterDiscount + amountServiceCharge) * ConstantValues.D_GST;

                if (model.token_amount == 0)
                {
                    await goEatApi.RecordTokenTransaction(new GRecordTokenTransaction
                    {
                        username = customer.Data.Profile.account,
                        order_id = encodeOrderId,
                        gtoken_transaction_id = gtoken_transaction_id,
                        token_type = ConstantValues.S_SGD,
                        transaction_type = ConstantValues.S_TRANSACTION_TYPE_CONSUMPTION,
                        amount = model.cash_amount,
                        description = ConstantValues.S_DESCRIPTION_AMOUNT_WITH_CASH,
                        tax = amountGst,
                        service_charge = amountServiceCharge
                    });
                }
                else if (model.cash_amount == 0)
                {
                    await goEatApi.RecordTokenTransaction(new GRecordTokenTransaction
                    {
                        username = customer.Data.Profile.account,
                        order_id = encodeOrderId,
                        gtoken_transaction_id = gtoken_transaction_id,
                        token_type = ConstantValues.S_SUGAR_TOKEN,
                        transaction_type = ConstantValues.S_TRANSACTION_TYPE_CONSUMPTION,
                        amount = model.token_amount,
                        description = ConstantValues.S_DESCRIPTION_AMOUNT_WITH_TOKEN,
                        tax = amountGst,
                        service_charge = amountServiceCharge
                    });
                }
                else
                {
                    await goEatApi.RecordTokenTransaction(new GRecordTokenTransaction
                    {
                        username = customer.Data.Profile.account,
                        order_id = encodeOrderId,
                        gtoken_transaction_id = gtoken_transaction_id,
                        token_type = ConstantValues.S_SUGAR_TOKEN,
                        transaction_type = ConstantValues.S_TRANSACTION_TYPE_CONSUMPTION,
                        amount = model.token_amount,
                        description = ConstantValues.S_DESCRIPTION_AMOUNT_WITH_TOKEN,
                        tax = amountGst,
                        service_charge = amountServiceCharge
                    });


                    await goEatApi.RecordTokenTransaction(new GRecordTokenTransaction
                    {
                        username = customer.Data.Profile.account,
                        order_id = encodeOrderId + "_extra",
                        gtoken_transaction_id = gtoken_transaction_id,
                        token_type = ConstantValues.S_SGD,
                        transaction_type = ConstantValues.S_TRANSACTION_TYPE_CONSUMPTION,
                        amount = model.cash_amount,
                        description = ConstantValues.S_DESCRIPTION_AMOUNT_WITH_CASH,
                        tax = amountGst,
                        service_charge = amountServiceCharge
                    });
                }
            }

            bool result = goEatApi.UpdateFinalTransaction(
                gtoken_transaction_id, description,
                mainTransaction.Data.id, ConstantValues.S_SUCCESS,
                model.customer_id, model.token_amount);

            return Json(new
            {
                success = result,
                error_code = string.Empty
            });
        }

        // Call CreateGtoken to Gtoken api in cases we use cash in transaction
        private async Task<Result<GTokenTransactionModel>> CreateGToken(GoEatApi goEatApi, decimal discountRate, string orderId,
            decimal originalPrice, decimal cash, string username, string description, decimal orginal_tax, decimal original_service_charge)
        {
            var gTokenTransaction = goEatApi.CreateGTokenTransaction(new GTokenTransaction
            {
                currency = ConstantValues.S_CURRENCY_SGD,
                discount_percentage = discountRate,
                order_id = orderId,
                original_currency = ConstantValues.S_CURRENCY_SGD,
                original_price = originalPrice,
                original_final_amount = cash,
                username = username,
                status = ConstantValues.S_PENDING,
                payment_method = ConstantValues.S_PAYMENT_METHOD,
                description = description,
                is_venvici_applicable = true,
                original_tax = orginal_tax,
                original_service_charge = original_service_charge
            });

            if (!gTokenTransaction.Succeeded)
            {
                return gTokenTransaction;
            }

            //execute transaction
            gTokenTransaction.Data.transaction.status = ConstantValues.S_SUCCESS;
            gTokenTransaction = await goEatApi.ExecuteGTokenTransaction(gTokenTransaction.Data.transaction);

            return gTokenTransaction;
        }


        private async Task<Result<GTokenTransactionModel>> CreateGTokenTransaction(GoEatApi goEatApi, decimal discountRate, string orderId,
            decimal originalPrice, string username, string description, decimal original_tax, decimal original_service_charge)
        {
            decimal amountAfterDiscount = originalPrice * (1 - discountRate);
            decimal amountServiceCharge = amountAfterDiscount * original_service_charge;
            decimal amountGst = (amountAfterDiscount + amountServiceCharge) * original_tax;

            var gTokenTransaction = goEatApi.CreateGTokenTransaction(new GTokenTransaction
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
                original_service_charge = amountServiceCharge
            });

            if (!gTokenTransaction.Succeeded)
            {
                return gTokenTransaction;
            }

            return gTokenTransaction;
        }

        private async Task<Result<GTokenTransactionModel>> ExecuteGTokenTransaction(Result<GTokenTransactionModel> gTokenTransaction,
            GoEatApi goEatApi)
        {
            //execute transaction
            gTokenTransaction.Data.transaction.status = ConstantValues.S_SUCCESS;
            gTokenTransaction = await goEatApi.ExecuteGTokenTransaction(gTokenTransaction.Data.transaction);

            return gTokenTransaction;
        }

        private Result<ResultDirectChargeGtoken> CreateDirectChargeGtoken(decimal originalPrice, string orderId, string username, string description)
        {
            var gTokenTransaction = GoEatApi.Instance.DirectChargeGtoken(new DirectChargeGtokenModel
            {
                order_id = orderId,
                username = username,
                description = description,
                amount = originalPrice
            });
            return gTokenTransaction;
        }
        #endregion
    }
}