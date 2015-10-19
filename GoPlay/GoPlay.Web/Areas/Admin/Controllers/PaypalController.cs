using GoPlay.Web.Controllers;
using GoPlay.Web.ActionFilter;
using GoPlay.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay.Web.Models;
using PayPal.Api;
using GoPlay.Web.Helpers;
using GoPlay.Models;
using AutoMapper;
using PayPal.AdaptivePayments.Model;
using System.Configuration;
using PayPal.AdaptivePayments;
using System.Text;
using log4net;
using GoPlay.Web.Areas.Admin.Models;

namespace GoPlay.Web.Areas.Admin.Controllers
{
     [Authorize]
    [RouteArea("admin")]
    [RequiredLogin]
    public class PaypalController : BaseController
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(PaypalController));

        [Route("paypal/")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult Index()
        {
            var api = GoPlayApi.Instance;
            var paypalPreapproval = api.FindPaypalPreapproval(true).Data.FirstOrDefault();
            return View(paypalPreapproval);
        }

        [Route("paypal/search-paypal-payment")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult GetSearchPaypalTransaction()
        {
            var model = new PaypalPaymentSearchForm();
            return View("Search", model);
        }

        [HttpPost]
        [Route("paypal/search-paypal-payment")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult searchPaypalTransaction(PaypalPaymentSearchForm form)
        {
            if (ModelState.IsValid)
            {
                var txn_id = form.transaction_id;
                try
                {
                    var payment = FindPayment(txn_id);
                    if (payment != null)
                    {
                        form.payment = payment;
                        return View("Search", form);
                    }
                    else
                    {
                        form.error = "Your transaction could not be found.";
                    }
                }
                catch (Exception)
                {
                    form.error = "Your transaction could not be found.";
                }

            }
            return View("Search", form);
        }

        [Route("paypal/confirm/{flag}")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult paypalConfirm(string flag)
        {
            var api = GoPlayApi.Instance;
            var resultActive = api.FindPaypalPreapproval(true).Data.FirstOrDefault();
            var resultNew = api.FindPaypalPreapproval(false, flag).Data.FirstOrDefault();

            if (resultActive != null)
            {
                resultActive.is_active = false;
                api.UpdatePaypalPreapproval(resultActive.id, false);
            }
            if (resultNew != null)
            {
                resultNew.is_active = true;
                api.UpdatePaypalPreapproval(resultNew.id, true);
            }

            return RedirectToAction("index", "paypal");

        }


        [Route("paypal/PreapprovalSubmit")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult GetPaypalPreapprovalSubmit()
        {
            var model = new PaypalPreapprovalAdminForm();
            return View("submit", model);
        }

        [HttpPost]
        [Route("paypal/PreapprovalSubmit")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult paypalPreapprovalSubmit(PaypalPreapprovalAdminForm form)
        {
            if (ModelState.IsValid)
            {
                //var timezone = TimeZoneInfo.FindSystemTimeZoneById("Antarctica/Troll");
                var timezone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");//it's the same with Antarctica/Troll
                if (form.starting_date != null)
                {
                    form.starting_date = TimeZoneInfo.ConvertTimeFromUtc(form.starting_date, timezone);
                }

                if (form.ending_date != null)
                {
                    form.ending_date = TimeZoneInfo.ConvertTimeFromUtc(form.ending_date, timezone);
                }

                Mapper.CreateMap<PaypalPreapprovalAdminForm, paypal_preapproval>();

                var paypalPreapproval = Mapper.Map<paypal_preapproval>(form);
                paypalPreapproval.flag = Guid.NewGuid().ToString();

                var result = PaypalPreapprovalRequest(paypalPreapproval);
                if (result.responseEnvelope.ack.HasValue && result.responseEnvelope.ack.Value.ToString().ToLower() == "failure")
                {
                    form.error = String.Format("Fail from field: {0}, message: {1}", result.error[0].parameter[0].name, result.error[0].message);
                    return View("submit", form);
                }
                else
                {
                    var api = GoPlayApi.Instance;

                    var key = result.preapprovalKey;
                    paypalPreapproval.preapproval_key = key;

                    this.Flash("Submitting...", FlashLevel.Success);

                    paypalPreapproval.created_at = DateTime.UtcNow;
                    api.CreatePaypalPreapproval(paypalPreapproval);

                    return Redirect(ConfigurationManager.AppSettings["PAYPAL_CONFIRM_KEY_URL"] + key);

                }

            }
            return View("submit", form);
        }


        [Route("paypal/PreapprovalCancel")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult GetPaypalPreapprovalCancel()
        {
            var api = GoPlayApi.Instance;

            var paypalPreapproval = api.FindPaypalPreapproval(true).Data.SingleOrDefault();
            if (paypalPreapproval != null)
            {
                var headers = PaypalHelper.GetHeader();

                RequestEnvelope requestEnvelope = new RequestEnvelope("en_US");

                CancelPreapprovalRequest cancelPreapprovalRequest = new CancelPreapprovalRequest
                {
                    requestEnvelope = requestEnvelope,
                    preapprovalKey = paypalPreapproval.preapproval_key
                };


                AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(headers);
                CancelPreapprovalResponse cancelPreapprovalResponse = adaptivePaymentsService.CancelPreapproval(cancelPreapprovalRequest);
                if (cancelPreapprovalResponse.responseEnvelope.ack.HasValue && cancelPreapprovalResponse.responseEnvelope.ack.Value.ToString().ToLower() == "success")
                {
                    paypalPreapproval.is_active = false;
                    api.UpdatePaypalPreapproval(paypalPreapproval.id, false);
                    this.Flash("Transaction is successful.", FlashLevel.Success);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { error = cancelPreapprovalResponse.error[0].message });
                }
            }
            return RedirectToAction("index", "paypal");
        }

        [Route("paypal/payment-execute/{id}")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult paypalPaymentExecute(string id)
        {
            var api = GoPlayApi.Instance;
            var transaction = api.GetGcoinTransaction(id);
            if (transaction.HasData)
            {
                var headers = PaypalHelper.GetHeader();

                RequestEnvelope requestEnvelope = new RequestEnvelope("en_US");

                ExecutePaymentRequest executePayment = new ExecutePaymentRequest
                {
                    requestEnvelope = requestEnvelope,
                    payKey = transaction.Data.pay_key
                };

                AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(headers);
                ExecutePaymentResponse executePaymentResponse = adaptivePaymentsService.ExecutePayment(executePayment);

                if (executePaymentResponse.responseEnvelope.ack.Value.ToString().ToLower() == "success" && executePaymentResponse.paymentExecStatus == "COMPLETED" && executePaymentResponse.error[0].errorId.Value.ToString() == "580022")
                {
                    transaction.Data.status = "success";
                    //update trans

                    api.UpdateGCoinTransaction(transaction.Data);

                    logger.Fatal("update payment success");

                    this.Flash("Transaction is successful.", FlashLevel.Success);
                    return Json(new { success = true });
                }
                else
                {
                    var error_msg = executePaymentResponse.error[0].message;
                    logger.Fatal(error_msg);

                    this.Flash(error_msg, FlashLevel.Error);
                }
                return RedirectToAction("gcoinTransactionIndex", "paypal");
            }
            return Json(new { success = false });
        }

        [Route("paypal/gcoin-transaction")]
        [RBAC(AccessRole = "accountant")]
        public ActionResult gcoinTransactionIndex()
        {
            var api = GoPlayApi.Instance;
            string sqlQuery = @"SELECT gcoin_transaction.*, ca.nickname  
                                FROM gcoin_transaction
                                JOIN customer_account ca on ca.id = gcoin_transaction.customer_account_id
                                WHERE status='pending' AND pay_key is not NULL AND receiver_email is not NULL";
            var transaction = api.GetGcoinTransactionByCustomeQuery(sqlQuery);
            return View("gcoin_index", new GcoinPendingTransaction() { transactions = transaction.Data });
        }

        #region Helper
        private Payment FindPayment(string txn_id)
        {
            var paypalHelper = new PaypalHelper();
            var sale = Sale.Get(paypalHelper.GetGetAPIContext(), txn_id);
            var payment = Payment.Get(paypalHelper.GetGetAPIContext(), sale.parent_payment);
            return payment;
        }

        public PreapprovalResponse PaypalPreapprovalRequest(paypal_preapproval obj)
        {
            var headers = PaypalHelper.GetHeader();

            RequestEnvelope requestEnvelope = new RequestEnvelope("en_US");

            PreapprovalRequest payRequest = new PreapprovalRequest
            {
                requestEnvelope = requestEnvelope,
                returnUrl = ConfigurationManager.AppSettings["PAYPAL_CONFIRM_RETURN_URL"] + obj.flag,
                cancelUrl = ConfigurationManager.AppSettings["PAYPAL_CONFIRM_CANCEL_URL"],
                pinType = "NOT_REQUIRED",
                currencyCode = ConfigurationManager.AppSettings["DEFAULT_PAYPAL_CURRENCY"],
                maxAmountPerPayment = obj.max_amount_per_payment,
                maxNumberOfPayments = obj.max_number_of_payments,
                maxTotalAmountOfAllPayments = obj.max_total_amount_of_all_payments,
                senderEmail = obj.sender_email,
                startingDate = obj.starting_date.ToString("yyyy-MM-dd'T'HH:mm:ss.000zzz"),
                endingDate = obj.ending_date.ToString("yyyy-MM-dd'T'HH:mm:ss.000zzz")
            };


            AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(headers);
            PreapprovalResponse payResponse = adaptivePaymentsService.Preapproval(payRequest);
            return payResponse;
        }
        #endregion
    }
}