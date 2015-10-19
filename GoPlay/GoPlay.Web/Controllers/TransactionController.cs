using GoPlay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay.Core;
using GoPlay.Web.Helpers;
using Platform.Models;
using GoPlay.Models;
using GoPlay.Models.Models;
using AutoMapper;
using PayPal.Api;
using Platform.Utility;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Configuration;
using GoPlay.Web.ActionFilter;
using System.Net.Mail;
using PayPal.AdaptivePayments;
using PayPal.AdaptivePayments.Model;
using System.Net;
using System.Text;
using System.IO;
using System.Dynamic;
using GoPlay.Web.Identity;
using log4net;
using GoPlay.Web.Helpers.Extensions;
using System.Configuration;

namespace GoPlay.Web.Controllers
{
    public class TransactionController : BaseController
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Transaction
        [Route("transaction/")]
        [Route("transaction/{panelName}/")]
        [Route("transaction/{panelName}/{topupMethodPanelName}")]
        [RequiredLogin]
        public ActionResult Index(string panelName = null, string topupMethodPanelName = null)
        {
            var api = GoPlayApi.Instance;
            var model = new TransactionViewModel();
            model.basicGtokenPackages = api.GetBasicGtokenPackages().Data;
            model.upointGTokenPackages = api.GetUpointGTokenPackages().Data;
            model.customGtokenPackage = api.GetCustomGtokenPackage().Data;

            model.user = api.GetUserById(CurrentUser.Id).Data;
            var games = GameHelper.GetGamesForCurrentUser(CurrentUser);
            foreach (var game in games)
            {
                var creditTypes = api.GetCreditTypes(game.id);
                var packages = api.GetPackages(game.id);
                if ((creditTypes.HasData && creditTypes.Data.Count > 0) || (packages.HasData && packages.Data.Count > 0))
                {
                    model.games.Add(game);
                }
            }
            //model.transactions = api.GetTransactions(CurrentUser.Id).Data;
            //model.transactions_count = api.GetTransactions(CurrentUser.Id).Data.Count();

            model.pageParams.Add("showTopupPanel", "false");
            model.pageParams.Add("showExchangePanel", "false");
            model.pageParams.Add("showGcoinPanel", "false");
            model.pageParams.Add("topUpMethod", "paypal");

            //Pushstate for top up and exchange panels
            if (panelName == "topup")
            {
                model.pageParams["showTopupPanel"] = "true";
                if (topupMethodPanelName == "topup-card")
                {
                    model.pageParams["topUpMethod"] = "topUpCard";
                }
                else if (topupMethodPanelName == "paypal")
                {
                    model.pageParams["topUpMethod"] = "paypal";
                }
                else if (topupMethodPanelName == "upoint")
                {
                    model.pageParams["topUpMethod"] = "upoint";
                }
            }
            else if (panelName == "exchange")
            {
                model.pageParams["showExchangePanel"] = "true";
            }
            else if (panelName == "gcoin")
            {
                model.pageParams["showGcoinPanel"] = "true";
            }
            else if (panelName != null)
            {
                return HttpNotFound();
            }
            model.gCoinConvert = new GCoinConvertViewModel();
            return View(model);
        }

        //TODO: need to apply new code 3/9, change input and some logic inside this function
        [Route("transaction/c/{pageIndex?}")]
        [Route("transaction/transaction-list/{pageIndex}/")]
        [RequiredLogin]
        public JsonResult TransactionList(int? pageIndex)
        {
            int totalRow = 0;
            int pageSize;
            if (!int.TryParse(ConfigurationManager.AppSettings["PAGESIZE_TRANSACTION_HISTORY"], out pageSize))
                pageSize = 10;
            pageIndex = pageIndex.HasValue ? pageIndex : 0;
            var transactions = GoPlayApi.Instance.GetTransactions(CurrentUser.Id, null, pageIndex, pageSize).Data;
            if (transactions.Count > 0)
            {
                totalRow = transactions[0].totalrow;
            }
            var data = transactions.Select(x => new
            {
                order_id = x.order_id,
                icon = x.getIcon(),
                source = x.source(),
                localized_created_at = TimeHelper.localizeDatetime(x.created_at),
                description = TransactionHelper.getDescription(x),
                amount = Helper.displayDecimal(x.amount),
                class_name = x.table_name,
                payment_method = x.payment_method,
                url = Url.Action("invoice", "transaction", new { order_id = x.order_id }, Request.Url.Scheme)
            }).ToList();
            return Json(new { data = data, total = totalRow }, JsonRequestBehavior.AllowGet);
        }

        [Route("transaction/transaction-list/{id}/{pageIndex}/")]
        [RequiredLogin]
        public JsonResult TransactionList(int id, int? pageIndex)
        {
            int totalRow = 0;
            int pageSize;
            if (!int.TryParse(ConfigurationManager.AppSettings["PAGESIZE_TRANSACTION_HISTORY"], out pageSize))
                pageSize = 10;
            pageIndex = pageIndex.HasValue ? pageIndex : 0;

            if (id == 0) id = CurrentUser.Id;
            var transactions = GoPlayApi.Instance.GetTransactions(id, null, pageIndex, pageSize).Data;
            if (transactions.Count > 0)
            {
                totalRow = transactions[0].totalrow;
            }
            var data = transactions.Select(x => new
            {
                order_id = x.order_id,
                icon = x.getIcon(),
                source = x.source(),
                localized_created_at = TimeHelper.localizeDatetime(x.created_at),
                description = TransactionHelper.getDescription(x),
                amount = Helper.displayDecimal(x.amount),
                class_name = x.table_name,
                payment_method = x.payment_method,
                url = Url.Action("invoice", "transaction", new { order_id = x.order_id }, Request.Url.Scheme)
            }).ToList();
            return Json(new { data = data, total = totalRow }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("transaction/invoice")]
        [RequiredLogin]
        public ActionResult Invoice(string order_id)
        {
            var api = GoPlayApi.Instance;
            InvoiceViewModel model = new InvoiceViewModel();
            if (!string.IsNullOrEmpty(order_id))
            {
                GeneralTransaction trans = GetGeneralTransaction(order_id);
                if (trans != null && trans.customer_account_id == CurrentUser.Id)
                {
                    model.generalTrans = trans;
                    model.package = trans.gtoken_package_id.HasValue ? api.GetGTokenPackage(trans.gtoken_package_id.Value).Data : null;
                    model.topupCard = trans.topup_card_id.HasValue ? api.GetTopUpCard(trans.topup_card_id.Value).Data : null;
                }
            }
            return View(model);
        }

        [Authorize]
        [Route("paypal")]
        [RequiredLogin]
        public ActionResult Paypal(TransactionPayPal modal)
        {

            var api = GoPlayApi.Instance;
            var package = api.GetTokenPackageBySKU(modal.sku);
            var user = api.GetUserById(CurrentUser.Id);

            string skuItem = package.Data.sku;
            string nameItem = package.Data.name;
            decimal priceItem = package.Data.getPrice(user.Data);
            string currency = ConstantCommon.DEFAULT_PAYPAL_CURRENCY;
            int quantityItem = 1;

            decimal totalAmount = priceItem * quantityItem;
            decimal playTokenAmount = quantityItem * (decimal)package.Data.play_token_amount;

            var coinTransaction = CreateCoinTransactionEntity(package.Data, playTokenAmount, totalAmount);

            coinTransaction = api.CreateCoinTransaction(coinTransaction).Data;

            //For PAYPAL!!!

            var helper = new PaypalHelper();
            var guid = Guid.NewGuid();
            var basePaypalReturn = GetPaypalReturn(guid.ToString());
            var returnUrl = basePaypalReturn + "/paypal_execute?order_id=" + coinTransaction.order_id;
            var cancelUrl = basePaypalReturn + "/cancel?order_id=" + coinTransaction.order_id;

            var itemList = new List<Item>();
            var item = helper.CreateItem(nameItem, currency, priceItem, quantityItem, skuItem);
            itemList.Add(item);

            var amount = helper.CreateAmount(currency, totalAmount);
            var redirectUrls = helper.CreateRedirectUrls(returnUrl, cancelUrl);
            var transactionList = new List<PayPal.Api.Transaction>();
            var transaction = helper.CreateTransaction(coinTransaction.description, amount, itemList);
            transactionList.Add(transaction);
            try
            {
                var createdPayment = helper.CreatePayment(helper.GetGetAPIContext(), transactionList, redirectUrls);
                var links = createdPayment.links.GetEnumerator();
                var paypalRedirectUrl = new PaypalRedirectUrl();

                while (links.MoveNext())
                {
                    Links lnk = links.Current;

                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment
                        paypalRedirectUrl.approval_url = lnk.href;
                    }
                    if (lnk.rel.ToLower().Trim().Equals("execute"))
                    {
                        paypalRedirectUrl.execute = lnk.href;
                    }
                }
                //Save information of transaction into Database
                //State of transaction is pending
                //---------------------------------------------
                coinTransaction.paypal_payment_id = createdPayment.id;
                coinTransaction.paypal_redirect_urls = JsonConvert.SerializeObject(paypalRedirectUrl);

                api.UpdateCoinTransaction(coinTransaction.id, coinTransaction.paypal_redirect_urls, coinTransaction.paypal_payment_id);

                //submit transaction to GTOKEN
                var gtoken_transaction_id = CreateGTokenTransaction(coinTransaction, package.Data, user.Data, totalAmount);

                return Redirect(paypalRedirectUrl.approval_url);
            }
            catch
            {
            }
            //update description
            api.UpdateCoinTransactionStatus(coinTransaction.id, "failure", coinTransaction.description);
            return RedirectToAction("index", "transaction");
        }

        [Authorize]
        [Route("transaction/paypal_execute")]
        [RequiredLogin]
        public async Task<ActionResult> PaypalExcecute()
        {
            string payerId = Request.Params["PayerID"];
            var order_id = Request.Params["order_id"];
            var api = GoPlayApi.Instance;
            var transaction = api.GetCoinTransactionByOderId(order_id.ToLower());

            var helper = new PaypalHelper();
            var payment = helper.Find(helper.GetGetAPIContext(), transaction.Data.paypal_payment_id);

            // Rechecks the total amount and currency of PayPal payment
            decimal paypalTotalAmount = Decimal.Parse(payment.transactions[0].amount.total);
            string paypalCurrency = payment.transactions[0].amount.currency;
            if (paypalTotalAmount != transaction.Data.price || paypalCurrency != ConstantCommon.DEFAULT_PAYPAL_CURRENCY)
            {
                transaction.Data.status = "failure";
                transaction.Data.description = "PayPal Error: Invalid currency or total amount.";
                api.UpdateCoinTransactionStatus(transaction.Data.id, transaction.Data.status, transaction.Data.description);

                await api.updateGTokenTransactionStatus(transaction.Data.order_id, "failure");

                return RedirectToAction("index", "transaction");
            }
            var executedPayment = payment.Execute(helper.GetGetAPIContext(), new PaymentExecution() { payer_id = payerId });
            if (executedPayment.state.ToLower() != "approved")
            {
                //when executedPayment is not approved
                //---------------------------------------------
                api.UpdateCoinTransactionStatus(transaction.Data.id, "failure", executedPayment.failed_transactions[0].message);
                await api.updateGTokenTransactionStatus(transaction.Data.order_id, "failure");
                try
                {
                    if (Int32.Parse(executedPayment.failed_transactions[0].code) == 400)
                    {
                        api.UpdateCoinTransactionStatus(transaction.Data.id, "pending");
                    }
                    try
                    {
                        string customerEmail = ConfigurationManager.AppSettings["ADMINS"];//should change
                        string subject = "PlayToken - Topup transaction notification";
                        string from = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];
                        string to = customerEmail;
                        string body = String.Format("Thank you for your business. Your payment {0} was under review by Paypal service and could take up to 24 hours to release. Once the transaction is completed, the system will update your Play Token automatically.", transaction.Data.paypal_payment_id);

                        var message = new MailMessage(from, to, subject, body);
                        EmailHelper.SendMail(message);
                    }
                    catch (Exception)
                    {
                        string adminEmail = ConfigurationManager.AppSettings["ADMINS"];
                        string subject = "PlayToken wrong email";
                        string from = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];
                        string to = adminEmail;
                        string body = String.Format("This guy purchased Play Token with a proper email {0} {1} but the payment was under review by Paypal.", CurrentUser.Id, CurrentUser.Email);

                        var message = new MailMessage(from, to, subject, body);
                        EmailHelper.SendMail(message);
                    }

                }
                catch (Exception e)
                {
                    logger.Fatal(e.StackTrace.ToString());

                }

            }
            else
            {
                try
                {
                    //State of transaction is approved
                    //---------------------------------------------
                    transaction.Data.status = "pending";
                    api.UpdateCoinTransactionStatus(transaction.Data.id, transaction.Data.status);
                    //Not use since March 1st 2015
                    //Send a request to Venvici to update their database with the payment
                    //venvici.pushBv(transaction.customerAccount, transaction.amount / 2)
                    try
                    {
                        var invoiceTemplate = new InvoiceViewModel()
                        {
                            transaction = transaction.Data,
                            payer = api.GetUserById(transaction.Data.customer_account_id).Data,
                            package = api.GetGTokenPackage(transaction.Data.gtoken_package_id.Value).Data,
                            topupCard = transaction.Data.topup_card_id.HasValue ? api.GetTopUpCard(transaction.Data.topup_card_id.Value).Data : null
                        };
                        await EmailHelper.SendMailInvoice(invoiceTemplate);

                    }
                    catch (Exception)
                    {
                        string adminEmail = ConfigurationManager.AppSettings["ADMINS"];
                        string subject = "PlayToken wrong email";
                        string from = ConfigurationManager.AppSettings["CUSTOMER_SUPPORT_EMAIL_SENDER"];
                        string to = adminEmail;
                        string body = String.Format("This guy purchased Play Token with a proper email {0} {1}", CurrentUser.Id, CurrentUser.Email);

                        var message = new MailMessage(from, to, subject, body);
                        EmailHelper.SendMail(message);
                    }

                    return RedirectToAction("invoice", "transaction", new { order_id = transaction.Data.order_id });
                }
                catch (Exception ex)
                {
                    api.UpdateCoinTransactionStatus(transaction.Data.id, ex.Message.ToString());
                }
            }
            return RedirectToAction("index", "transaction");
        }

        [Authorize]
        [Route("transaction/cancel")]
        public async Task<ActionResult> Cancle()
        {
            var token = Request.Params["token"];
            bool tokenAvailable = false;
            if (!String.IsNullOrEmpty(token) && token.StartsWith("EC-") && token.Count() > 0)
            {
                tokenAvailable = true;
            }
            var order_id = Request.Params["order_id"];
            var api = GoPlayApi.Instance;
            var transaction = api.GetCoinTransactionByOderId(order_id);
            var list = new List<string>() { "cancelled", "success" };
            if (tokenAvailable && transaction.HasData && list.Where(x => x.Contains(transaction.Data.status)).Count() == 0)
            {
                this.Flash("Your PayPal payment has been cancelled!", FlashLevel.Info);
                transaction.Data.status = "cancelled";
                api.UpdateCoinTransactionStatus(transaction.Data.id, transaction.Data.status);
                //update status GTOKEN transaction
                await api.updateGTokenTransactionStatus(transaction.Data.order_id, "cancelled");
            }
            else
            {
                this.Flash("Don't try to be sneaky", FlashLevel.Warning);
            }
            return RedirectToAction("index", "transaction");
        }

        [Route("transaction/ipn-handler")]
        public async Task<ActionResult> IpnHandler()
        {
            try
            {
                // extract ipn data into a string
                byte[] param = Request.BinaryRead(Request.ContentLength);
                string strRequest = Encoding.ASCII.GetString(param);

                // append PayPal verification code to end of string
                strRequest += "&cmd=_notify-validate";

                logger.Debug(strRequest);

                // create an HttpRequest channel to perform handshake with PayPal
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["PAYPAL_URL"]);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = strRequest.Length;

                // send data back to PayPal to request verification
                StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(strRequest);
                streamOut.Close();

                // receive response from PayPal
                StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
                string strResponse = streamIn.ReadToEnd();
                streamIn.Close();

                // if PayPal response is successful / verified
                logger.Debug(strResponse);
                if (strResponse.Equals("VERIFIED"))
                {
                    // paypal has verified the data, it is safe for us to perform processing now

                    // extract the form fields expected: buyer and seller email, payment status, amount
                    // string payerEmail = Request.Form["payer_email"];
                    string paymentStatus = Request.Form["payment_status"];
                    //string receiverEmail = Request.Form["receiver_email"];
                    string amount = Request.Form["mc_gross"];
                    string txn_id = Request.Form["txn_id"];
                    string txn_type = Request.Form["txn_type"];
                    string payment_type = Request.Form["payment_type"];

                    logger.Debug(txn_id);
                    logger.Debug(txn_type);

                    if (txn_type == "cart" && payment_type == "instant")
                    {
                        if (paymentStatus.Equals("Completed"))
                        {
                            var api = GoPlayApi.Instance;
                            var paypalHelper = new PaypalHelper();
                            var sale = Sale.Get(paypalHelper.GetGetAPIContext(), txn_id);
                            if (sale != null)
                            {
                                var payment_id = sale.parent_payment;
                                var transaction = api.GetCoinTransactionByPaypalPaymentId(payment_id);
                                if (!transaction.HasData)
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                                }


                                var user = api.GetUserById(transaction.Data.customer_account_id).Data;
                                //if transaction.use_gtoken:
                                //# Charge 25% in GToken
                                //gtoken = transaction.amount / 4
                                //sufficientBalance = venvici.checkGToken(user, gtoken)
                                //if not sufficientBalance:
                                //    transaction.status = u'failure'
                                //    transaction.description += u'. Paypal approved but insufficient Play Token'
                                //    store.commit()
                                //    return '', 200
                                //venvici.deductGToken(user, transaction)

                                if (transaction.Data.status == Helper.GetDescription(TransactionStatus.Pending))
                                {
                                    if (user.HasDiscount())
                                    {
                                        user.is_discount_permanent = true;
                                        api.UpdateCustomerAccount(user.id, user.is_discount_permanent);
                                    }
                                    transaction.Data.status = Helper.GetDescription(TransactionStatus.Success);
                                    api.UpdateCoinTransactionStatus(transaction.Data.id, transaction.Data.status);
                                    //update status GTOKEN transaction
                                    await api.updateGTokenTransactionStatus(transaction.Data.order_id, Helper.GetDescription(TransactionStatus.Success));

                                }

                                if (!string.IsNullOrEmpty(user.inviter_username))
                                {
                                    var inviter = api.GetUserByUserName(user.inviter_username);
                                    if (inviter.Data.username == "gdc")
                                    {
                                        string sqlString = @"SELECT * FROM coin_transaction
                                WHERE customer_account_id={0} AND status={1} AND amount >= 5 AND sender_account_id is NULL ";
                                        var transactions = api.GetCoinTransactionsByCustomQuery(String.Format(sqlString, user.id, "success"));
                                        if (transactions.HasData && transactions.Data.Count == 1)
                                        {
                                            var freeCoin = new free_coin_transaction()
                                            {
                                                order_id = Guid.NewGuid().ToString(),
                                                customer_account_id = user.id,
                                                status = Helper.GetDescription(TransactionStatus.Success),
                                                description = "GDC Promotion",
                                                amount = 3
                                            };
                                            api.CreateFreeCoinTransaction(freeCoin);
                                        }
                                    }
                                }
                                //Venici - from 15/3/2015    
                                //venvici.updateVenviciBalance(user,transaction)

                            }
                        }
                    }
                    else
                    {
                        string adminEmail = ConfigurationManager.AppSettings["ADMINS"];
                        string subject = "Another payment type";
                        string body = String.Format("Transaction id: {0}, payment type: {1}, txn type: {2}", txn_id, payment_type, txn_type);
                        var from = new MailAddress("admin@goplay.la", "GoPlay Admin");
                        var to = new MailAddress(adminEmail);
                        var message = new MailMessage(from, to)
                        {
                            Subject = subject,
                            Body = body
                        };
                        EmailHelper.SendMail(message);

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("API Handler: " + ex.StackTrace);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        #region adaptive-payments
        //TODO : tested already but we dont see the place to use this code in python code now
        // Document: https://developer.paypal.com/docs/classic/api/adaptive-payments/Pay_API_Operation/
        // Example: https://devtools-paypal.com/guide/ap_parallel_payment/dotnet?interactive=ON&env=sandbox
        public PayResponse PaypalPayment(decimal amount, string receiverEmail,
            bool autoMode, string orderID)
        {
            var api = GoPlayApi.Instance;
            var headers = PaypalHelper.GetHeader();
            ReceiverList receiverList = new ReceiverList
            {
                receiver = new List<Receiver> {
                    new Receiver(amount)
                    {
                        email = receiverEmail
                    }
                }
            };

            RequestEnvelope requestEnvelope = new RequestEnvelope("en_US")
            {
                detailLevel = DetailLevelCode.RETURNALL
            };

            PayRequest payRequest = new PayRequest
            {
                requestEnvelope = requestEnvelope,
                reverseAllParallelPaymentsOnError = true,
                returnUrl = ConfigurationManager.AppSettings["PAYPAL_PAYMENT_RETURN_URL"] + orderID,
                cancelUrl = ConfigurationManager.AppSettings["PAYPAL_PAYMENT_CANCEL_URL"] + orderID,
                currencyCode = ConfigurationManager.AppSettings["DEFAULT_PAYPAL_CURRENCY"],
                feesPayer = "EACHRECEIVER",
                receiverList = receiverList
            };

            //TODO: When the payment is complete, PayPal sends an IPN message to the URL specified in the ipnNotificationUrl field of the Pay request. 
            //May be we dont need do it, but should consider
            //payRequest.ipnNotificationUrl = "http://replaceIpnUrl.com";

            if (autoMode)
            {
                var pp = api.GetPaypalPreApproval(true).Data;
                if (pp != null)
                    payRequest.preapprovalKey = pp.preapproval_key;
                payRequest.actionType = "PAY";
            }
            else
            {
                payRequest.senderEmail = ConfigurationManager.AppSettings["PAYPAL_SENDER_EMAIL"];
                payRequest.actionType = "CREATE";
                payRequest.payKeyDuration = ConfigurationManager.AppSettings["PAYPAL_PAY_KEY_DURATION"];
            }

            AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(headers);
            PayResponse payResponse = adaptivePaymentsService.Pay(payRequest);
            return payResponse;
        }

        private PaymentDetailsResponse PaypalPaymentDetail(string payKey)
        {
            var api = GoPlayApi.Instance;
            var headers = PaypalHelper.GetHeader();
            PaymentDetailsRequest request = new PaymentDetailsRequest
            {
                payKey = payKey,
                requestEnvelope = new RequestEnvelope("en_US")
                {
                    detailLevel = DetailLevelCode.RETURNALL
                }
            };

            AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(headers);
            PaymentDetailsResponse payResponse = adaptivePaymentsService.PaymentDetails(request);
            return payResponse;
        }
        #endregion


        [HttpPost]
        [RequiredLogin]
        [Route("transaction/exchange")]
        public async Task<ActionResult> Exchange(ExchangeViewModel model)
        {
            var api = GoPlayApi.Instance;
            Game game = api.GetGame(model.gameID).Data;
            var user = api.GetUserById(CurrentUser.Id).Data;

            dynamic exchangeOption = BaseExchangeHandler.retrieveExchangeOption(model.exchangeOption, model.exchangeOptionID);

            BaseExchangeHandlerInterface handler = BaseExchangeHandler.retrieveExchangeHandler(user, game, exchangeOption, model.exchangeAmount, Request.GetClientIp());
            if (!handler.validate())
            {
                return Json(new { errors = handler.getErrors(), timespan = TimeHelper.epoch() });
            }

            if (!String.IsNullOrEmpty(model.executeExchange))
            {
                if (!handler.exchange().Result)
                {
                    return Json(new { errors = handler.getErrors(), timespan = TimeHelper.epoch() });
                }
                else
                {
                    var message = Resources.Resources.Exchanged_successfully;
                    this.Flash(message, FlashLevel.Success);
                    return Json(new { success = true, message = message });
                }
            }
            return Json(new { success = true, timespan = TimeHelper.epoch() });
        }

        [HttpPost]
        [RequiredLogin]
        [Route("transaction/exchange-calculate")]
        public async Task<ActionResult> ExchangeCalculate(ExchangeViewModel model)
        {
            var api = GoPlayApi.Instance;
            Game game = api.GetGame(model.gameID).Data;
            var user = api.GetUserById(CurrentUser.Id).Data;

            dynamic exchangeOption = BaseExchangeHandler.retrieveExchangeOption(model.exchangeOption, model.exchangeOptionID);

            BaseExchangeHandlerInterface handler = BaseExchangeHandler.retrieveExchangeHandler(user, game, exchangeOption, model.exchangeAmount, Request.GetClientIp());

            Tuple<decimal, decimal> tuple = await handler.calculate();
            if (!(tuple.Item1 > 0 || tuple.Item2 > 0))
                return null;

            var result = String.Empty;

            if (tuple.Item1 > 0 && tuple.Item2 > 0)
            {
                result = String.Format(Resources.Resources.This_exchange_will_cost_you_Free_Play_Token_and_Play_Token, tuple.Item1, tuple.Item2);
            }
            else if (tuple.Item1 > 0)
            {
                result = String.Format(Resources.Resources.This_exchange_will_cost_you_Free_Play_Token, tuple.Item1);
            }
            else
            {
                result = String.Format(Resources.Resources.This_exchange_will_cost_you_Play_Token, tuple.Item2);
            }

            return Json(new { success = true, msg = result, timespan = TimeHelper.epoch() });
        }


        [HttpPost]
        [Authorize]
        [Route("transaction/convert-gcoin")]
        public JsonResult convertGCoin(GCoinConvertViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { errors = Errors(ModelState) });
            }
            if (!model.IsValidPassWord(CurrentUser.UserName))
                return FailResult("password", Resources.Resources.Password_is_not_correct);
            if (!PaypalHelper.isReceiverEmailVerified(model.paypalEmail))
                return FailResult("paypalEmail", Resources.Resources.Invalid_paypal_account);

            decimal gcoin = model.gcoin;
            string receiverEmail = model.paypalEmail;
            if (gcoin > CurrentUser.gcoin)
                return FailResult("gcoin", Resources.Resources.Invalid_GCoin_Convert);

            var api = GoPlayApi.Instance;
            gcoin_transaction transaction = new gcoin_transaction()
            {
                order_id = Guid.NewGuid().ToString(),
                customer_account_id = CurrentUser.Id,
                amount = -gcoin, //# Keeps it negative so sum(amount) works
                description = "Convert GCoin to USD, send to Paypal account: " + receiverEmail,
                status = Helper.GetDescription(TransactionStatus.Pending),
                payment_method = Helper.GetDescription(PaymentMethod.Convert_Gcoin),
                sender_email = ConfigurationManager.AppSettings["PAYPAL_SENDER_EMAIL"],
                receiver_email = receiverEmail
            };
            transaction = api.CreateGCoinTransaction(transaction).Data;
            if (transaction == null)
                return FailResult("gcoin", ErrorCodes.ServerError.ToErrorMessage());

            var autoMode = false;
            var pp = api.GetPaypalPreApproval(true).Data;
            if (pp != null)
                autoMode = true;
            var paymentResult = PaypalPayment(gcoin, receiverEmail, autoMode, transaction.order_id);
            string err = null;
            if (paymentResult.responseEnvelope.ack == AckCode.FAILURE)
            {
                transaction.status = Helper.GetDescription(TransactionStatus.Failure);
                err = "paypal: " + paymentResult.error[0].message;
            }
            else if (paymentResult.paymentExecStatus == "COMPLETED")
            {
                transaction.description = string.Format(GoPlayConstantValues.S_GCOIN_CONVERT_DESCRIPTION, Helper.displayDecimal(gcoin), receiverEmail);
                transaction.pay_key = paymentResult.payKey;
                transaction.status = Helper.GetDescription(TransactionStatus.Success);
                this.Flash(Resources.Resources.GCoin_transaction_is_successful, FlashLevel.Success);
            }
            else
            {
                transaction.pay_key = paymentResult.payKey;
                transaction.description = string.Format(GoPlayConstantValues.S_GCOIN_CONVERT_DESCRIPTION, Helper.displayDecimal(gcoin), receiverEmail);
                var paymentdetail = PaypalPaymentDetail(paymentResult.payKey);
                if (paymentdetail.responseEnvelope.ack == AckCode.SUCCESS)
                    transaction.pay_key_expiration_date = DateTime.Parse(paymentdetail.payKeyExpirationDate);
                else
                    transaction.pay_key_expiration_date = DateTime.UtcNow;
                transaction.status = Helper.GetDescription(TransactionStatus.Pending);
                this.Flash(Resources.Resources.GCoin_transaction_is_successful, FlashLevel.Success);
            }
            api.UpdateGCoinTransaction(transaction);
            if (err == null)
                return Json(new { success = true });
            else
                return FailResult("gcoin", err);
        }

        [RequiredLogin]
        [Route("transaction/topup-card")]
        public async Task<JsonResult> topupCard(TopupCardForm form)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { errors = Errors(ModelState) });
            }
            var api = GoPlayApi.Instance;
            var card = api.GetTopUpCard(form.cardNumber, form.cardPassword);
            dynamic transaction = new ExpandoObject();
            if (card.Data.is_free.HasValue && card.Data.is_free.Value)
            {
                transaction = CreateFreeCoinTrans(CurrentUser, card.Data);
            }
            else
            {
                transaction = CreateCoinTrans(CurrentUser, card.Data);
            }

            api.UpdateTopupCard(card.Data.id, CurrentUser.Id, "used", DateTime.UtcNow);

            var user = api.GetUserById(CurrentUser.Id);
            if (user.Data.HasDiscount())
            {
                user.Data.is_discount_permanent = true;
            }
            api.UpdateCustomerAccount(user.Data.id, user.Data.is_discount_permanent);

            //    Not use since March 1st 2015
            //if not card.is_free and card.is_bv:
            //    # Send a request to Venvici to update their database with the payment
            //    # So there are 3 types of topup cards now
            //    # - standard card, not free and has bv value
            //    # - free card, free and has no bv value
            //    # - no-bv card, not free and has no bv value
            //    venvici.pushBv(transaction.customerAccount, transaction.amount / 2)

            //    #Venici - from 15/3/2015    
            //#if not card.is_free and card.is_bv:
            //    #venvici.updateVenviciBalance(user,transaction)


            if (!card.Data.is_free.HasValue || !card.Data.is_free.Value)
            {
                var gTokenTransaction = GoPlayApi.Instance.CreateGTokenTransaction(new GTokenTransaction
                {
                    username = CurrentUser.UserName,
                    order_id = transaction.order_id,
                    original_price = card.Data.amount,
                    original_final_amount = card.Data.amount,
                    original_currency = ConstantCommon.DEFAULT_PAYPAL_CURRENCY,
                    discount_percentage = 0,
                    payment_method = Helper.GetDescription(PaymentMethod.TopUpCard),
                    description = transaction.description,
                    status = ConstantValues.S_SUCCESS,
                    is_venvici_applicable = card.Data.is_bv.HasValue ? card.Data.is_bv.HasValue : false,
                    revenue_percentage = decimal.Parse(ConfigurationManager.AppSettings["REVENUE_PERCENTAGE"].ToString())
                });
            }
            var payer = api.GetUserById(user.Data.id).Data;
            if (!string.IsNullOrEmpty(payer.email))
            {
                if (transaction is free_coin_transaction)
                {
                    Mapper.CreateMap<free_coin_transaction, coin_transaction>();
                    transaction = Mapper.Map<coin_transaction>(transaction);
                }
                var invoiceTemplate = new InvoiceViewModel()
                           {
                               transaction = transaction,
                               payer = payer,
                               package = null,
                               topupCard = card.Data
                           };
               await EmailHelper.SendMailInvoice(invoiceTemplate);
            }

            var message = String.Empty;
            if (card.Data.is_free.HasValue && card.Data.is_free.Value)
            {
                message = String.Format(Resources.Resources.You_topped_up_Free_Play_Token, Helper.displayDecimal(card.Data.amount));
            }
            else
            {
                message = String.Format(Resources.Resources.You_topped_up_Play_Token, Helper.displayDecimal(card.Data.amount));
            }
            return Json(new { success = true, message = message, is_free = (card.Data.is_free.Value && card.Data.is_free.Value), amount = card.Data.amount });
        }



        #region Helper

        private GeneralTransaction GetGeneralTransaction(string order_id)
        {
            var api = GoPlayApi.Instance;
            var Gtrans = api.GetGcoinTransaction(order_id).Data;
            GeneralTransaction trans = null;
            if (Gtrans != null)
            {
                Mapper.CreateMap<gcoin_transaction, GeneralTransaction>();
                trans = Mapper.Map<gcoin_transaction, GeneralTransaction>(Gtrans);
                trans.table_name = GoPlayConstantValues.GCOIN_TRANSACTION;
            }
            else
            {
                var CoinTrans = api.GetCoinTransactionByOderId(order_id).Data;
                if (CoinTrans != null)
                {
                    Mapper.CreateMap<coin_transaction, GeneralTransaction>();
                    trans = Mapper.Map<coin_transaction, GeneralTransaction>(CoinTrans);
                    trans.table_name = GoPlayConstantValues.COIN_TRANSACTION;
                }
                else
                {
                    var FreeTrans = api.GetFreeCoinTransactionByOderId(order_id).Data;
                    if (FreeTrans != null)
                    {
                        Mapper.CreateMap<free_coin_transaction, GeneralTransaction>()
                            .ForMember(d => d.gtoken_package_id, s => s.MapFrom(src => src.package_id));
                        trans = Mapper.Map<free_coin_transaction, GeneralTransaction>(FreeTrans);
                        trans.table_name = GoPlayConstantValues.FREE_COIN_TRANSACTION;
                    }
                }
            }
            return trans;
        }

        private string GetPaypalReturn(string guid)
        {
            string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/transaction";
            var returnUrl = string.Format("{0}", baseURI);
            return returnUrl;
        }

        private coin_transaction CreateCoinTransactionEntity(GtokenPackage package, decimal playTokenAmount, decimal totalAmount)
        {
            var transaction = new coin_transaction();

            transaction.order_id = Guid.NewGuid().ToString();
            transaction.customer_account_id = CurrentUser.Id;
            transaction.amount = playTokenAmount;
            transaction.price = totalAmount;
            transaction.payment_method = "PayPal";
            transaction.gtoken_package_id = package.id;
            transaction.description = String.Format("You topped up {0} Play Token", Helper.displayDecimal(playTokenAmount));
            transaction.status = "payment_pending";
            transaction.use_gtoken = false;

            IPAddress ip = WebIpHelper.GetClientIp(Request);
            var country_name = String.Empty;
            ip.GetCountryCode(c => transaction.country_code = c, n => country_name = n);
            transaction.ip_address = ip.ToString();

            return transaction;
        }

        private string CreateGTokenTransaction(coin_transaction coinTransaction, GtokenPackage package, customer_account user, decimal totalAmount)
        {
            var gTokenTransaction = GoPlayApi.Instance.CreateGTokenTransaction(new GTokenTransaction
            {
                username = CurrentUser.UserName,
                order_id = coinTransaction.order_id,
                original_price = package.getPrice(user),
                original_final_amount = totalAmount,
                original_currency = ConstantCommon.DEFAULT_PAYPAL_CURRENCY,
                currency = ConstantValues.S_CURRENCY_SGD,
                discount_percentage = user.HasDiscount() ? 0.1m : 0,
                payment_method = "PayPal",
                description = coinTransaction.description,
                status = ConstantValues.S_PENDING,
                revenue_percentage = decimal.Parse(ConfigurationManager.AppSettings["REVENUE_PERCENTAGE"].ToString())
            });

            if (!gTokenTransaction.Succeeded)
            {
                return string.Empty;
            }

            return gTokenTransaction.Data.transaction.gtoken_transaction_id;
        }
        private JsonResult FailResult(string key, string err)
        {
            ModelState.AddModelError(key, err);
            return Json(new { errors = Errors(ModelState) });
        }

        public free_coin_transaction CreateFreeCoinTrans(ApplicationUser user, topup_card card)
        {
            var api = GoPlayApi.Instance;
            var transaction = new free_coin_transaction();
            transaction.order_id = Guid.NewGuid().ToString();
            transaction.customer_account_id = user.Id;
            transaction.amount = card.amount;
            transaction.price = card.amount;
            transaction.payment_method = "Top Up Card";
            transaction.status = "success";
            transaction.description = String.Format("You topped up {0} Play Token", Helper.displayDecimal(card.amount));
            transaction.topup_card_id = card.id;
            transaction.created_at = DateTime.UtcNow;

            IPAddress ip = WebIpHelper.GetClientIp(Request);
            var country_name = String.Empty;
            ip.GetCountryCode(c => transaction.country_code = c, n => country_name = n);
            transaction.ip_address = ip.ToString();

            api.CreateFreeCoinTransaction(transaction);
            return transaction;
        }

        public coin_transaction CreateCoinTrans(ApplicationUser user, topup_card card)
        {
            var api = GoPlayApi.Instance;
            var transaction = new coin_transaction();
            transaction.order_id = Guid.NewGuid().ToString();
            transaction.customer_account_id = user.Id;
            transaction.amount = card.amount;
            transaction.price = card.amount;
            transaction.payment_method = "Top Up Card";
            transaction.status = "success";
            transaction.description = String.Format("You topped up {0} Play Token", Helper.displayDecimal(card.amount));
            transaction.topup_card_id = card.id;
            transaction.created_at = DateTime.UtcNow;

            IPAddress ip = WebIpHelper.GetClientIp(Request);
            var country_name = String.Empty;
            ip.GetCountryCode(c => transaction.country_code = c, n => country_name = n);
            transaction.ip_address = ip.ToString();

            api.CreateCoinTransaction(transaction);
            return transaction;
        }
        #endregion
    }
}