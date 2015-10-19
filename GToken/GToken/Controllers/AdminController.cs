using Platform.Models;
using GToken.Web.ActionFilter;
using GToken.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Platform.Utility;

namespace GToken.Web.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        // GET: Admin
        [RBAC(AccessAction = "access_admin_page")]
        [Route("admin/")]
        public ActionResult Index()
        {
            return View();
        }

        [RBAC(AccessAction = "access_admin_accountant_page")]
        [Route("admin/transaction")]
        public ActionResult transactionIndex()
        {
            var api = Platform.Core.Api.Instance;
            var result = api.GetAllPartners();
            var model = new TransactionQueryForm();
            model.partners = result.Data.Select(x => x.identifier).ToList();
            return View(model);
        }

        [HttpPost]
        [RBAC(AccessAction = "access_admin_accountant_page")]
        [Route("admin/transaction")]
        public ActionResult QueryTransaction(TransactionQueryForm model)
        {
            var api = Platform.Core.Api.Instance;
            var transactions = api.GetTransactionsByQueryString(model.startTime, model.endTime, model.partner_identifier, model.status, model.partner_order_id, model.gtoken_transaction_id, model.timeZone, model.username).Data;

            if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "transaction")
            {
                if (transactions != null)
                {
                    ViewBag.transactions = transactions;
                }
            }

            if (!String.IsNullOrEmpty(model.export))
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("GToken Transaction ID,Partner,Partner Order ID,Account,Price,After Discount,Tax,Service Charge,After Tax,Currency,Original Price,Original After Discount,Original Tax,Original Service Charge,Original After Tax,Original Currency,Exchange Rate,Discount Percentage,Payment Method,Country Code,Created At,Status");
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
                Response.ContentType = "text/csv";
                if (transactions != null)
                {
                    var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                    foreach (var line in transactions)
                    {
                        DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                        sw.WriteLine(string.Format("{0},{1},{2},\"{3}\",{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
                                                   line.gtoken_transaction_id,
                                                   line.partner_name,
                                                   line.partner_order_id,
                                                   line.customer_username,
                                                   line.price,
                                                   line.final_amount,
                                                   line.tax,
                                                   line.service_charge,
                                                   line.final_amount_after_tax,
                                                   line.currency,
                                                   line.original_price,
                                                   line.original_final_amount,
                                                   line.original_tax,
                                                   line.original_service_charge,
                                                   line.original_final_amount_after_tax,
                                                   line.original_currency,
                                                   line.exchange_rate,
                                                   line.discount_percentage,
                                                   line.payment_method,
                                                   line.country_code,
                                                   targetTime,
                                                   line.status
                                                   ));
                    }
                }
                Response.Write(sw.ToString());
                Response.End();
            }

            var result = api.GetAllPartners();
            model.partners = result.Data.Select(x => x.identifier).ToList();

            return View("transactionIndex", model);
        }


        [RBAC(AccessAction = "access_admin_accountant_page")]
        [Route("admin/token-transaction")]
        public ActionResult tokenTransactionIndex(TransactionQueryForm model)
        {
            var conditions = new List<string>();
            if (model.startTime != null)
            {
                DateTime startTime = Platform.Utility.Helper.timeFromString(model.startTime, model.timeZone);
                conditions.Add(String.Format("token_transaction.created_at >= '{0}'", model.startTime.ToString()));
            }

            if (model.endTime != null)
            {
                DateTime endTime = Platform.Utility.Helper.timeFromString(model.startTime, model.timeZone);
                conditions.Add(String.Format("token_transaction.created_at <= '{0}'", model.endTime.ToString()));
            }

            if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "transaction" && !string.IsNullOrEmpty(model.partner_identifier))
            {
                conditions.Add(String.Format("token_transaction.partner_identifier ='{0}'", model.partner_identifier));
            }

            if (!string.IsNullOrEmpty(model.partner_order_id))
            {
                conditions.Add(String.Format("token_transaction.partner_order_id ='{0}'", model.partner_order_id));
            }

            if (!string.IsNullOrEmpty(model.gtoken_transaction_id))
            {
                conditions.Add(String.Format("token_transaction.gtoken_transaction_id ='{0}'", model.gtoken_transaction_id));
            }

            if (!string.IsNullOrEmpty(model.username))
            {
                string query = "(customer_account.email like '%" + model.username.ToLower() + "%'";
                query += " OR customer_account.nickname like '%" + model.username.ToLower() + "%'";
                query += " OR customer_account.username like '%" + model.username.ToLower() + "%')";
                conditions.Add(query);
            }

            string queryString = @"SELECT token_transaction.*, partner.name as partner_name  FROM
                                        token_transaction
                                        INNER JOIN customer_account on customer_account.username = token_transaction.customer_username
                                        INNER JOIN partner on partner.identifier = customer_account.partner_identifier ";
            if (conditions.Count > 0)
            {
                queryString += "WHERE " + String.Join(" AND ", conditions.ToArray());
            }

            var api = Platform.Core.Api.Instance;
            var transactions = api.GetTokenTransactionsByCustomQuery(queryString);

            if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "transaction")
            {
                if (transactions.HasData)
                {
                    ViewBag.transactions = transactions.Data;
                }
            }

            if (!String.IsNullOrEmpty(model.export))
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("GToken Transaction ID,Partner,Partner Order ID,Account,Amount,Tax,Service Charge,Final Amount,Token Type,Transaction Type,Description,Country Code,Created At\n");
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
                Response.ContentType = "text/csv";
                if (transactions.HasData)
                {
                    var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                    foreach (var line in transactions.Data)
                    {
                        DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\"",
                                                   line.gtoken_transaction_id,
                                                   line.partner_name,
                                                   line.partner_order_id,
                                                   line.customer_username,
                                                   line.amount,
                                                   line.tax,
                                                   line.service_charge,
                                                   line.amount_after_tax,
                                                   line.token_type,
                                                   line.transaction_type,
                                                   line.description,
                                                   line.country_code,
                                                   targetTime.ToString()
                                                   ));
                    }
                }
                Response.Write(sw.ToString());
                Response.End();
            }

            var result = api.GetAllPartners();
            model.partners = result.Data.Select(x => x.identifier).ToList();

            return View(model);
        }

        [Route("admin/user/{username}/")]
        [RBAC(AccessAction = "access_admin_accountant_page")]
        public ActionResult UserDetail(string username)
        {
            var api = Platform.Core.Api.Instance;
            CustomerAccount user = api.GetUserByUserName(username).Data;

            if (user == null)
            {
                return RedirectToAction("Index", "admin");
            }

            if (Request.Params["pushAccount"] != null && this.HasPermission(Platform.Models.ConstantValues.S_VENVICI))
            {
                //#venvici.addMember(user=user)
                //#flash("Push Account API was called", "info")
                //pass
            }

            if (Request.Params["pushTransaction"] != null && this.HasPermission(Platform.Models.ConstantValues.S_VENVICI))
            {
                Transaction transaction = api.GetTransactionById(Convert.ToInt32(Request.Params["transaction_id"])).Data;
                if (transaction != null)
                {
                    //api.UpdateVenviciBalance()
                    //venvici.updateVenviciBalance(user, transaction)
                    //#flash("Push BV API was called", "info")
                }
                else
                {
                    //#flash("Man, that transaction ID doesn't exist", "alert")
                    //pass
                }
            }

            return View(new UserDetail()
            {
                user = user,
                transactions = api.FindUserTransactions(user.username, null).Data
            });
        }

        [HttpPost]
        [RBAC(AccessAction = "access_admin_accountant_page")]
        [Route("admin/user")]
        public ActionResult UserIndex(UserQueryForm model)
        {
            List<CustomerAccount> users = null;
            if (!string.IsNullOrEmpty(model.query) || !string.IsNullOrEmpty(model.export))
            {
                string formatDate = "";
                //StringBuilder conditions = new StringBuilder();
                var conditions = new List<string>();
                if (!string.IsNullOrEmpty(model.regStartTime))
                {
                    conditions.Add(string.Format("customer_account.created_at >='{0}'", Helper.timeFromString(model.regStartTime, model.timeZone).ToString(formatDate)));
                }
                if (!string.IsNullOrEmpty(model.regEndTime))
                {
                    conditions.Add(string.Format("customer_account.created_at <='{0}'", Helper.timeFromString(model.regEndTime, model.timeZone).ToString(formatDate)));
                }
                if (!string.IsNullOrEmpty(model.loginStartTime))
                {
                    conditions.Add(string.Format("customer_account.last_login_at >='{0}'", Helper.timeFromString(model.loginStartTime, model.timeZone).ToString(formatDate)));
                }
                if (!string.IsNullOrEmpty(model.loginEndTime))
                {
                    conditions.Add(string.Format("customer_account.last_login_at <='{0}'", Helper.timeFromString(model.loginEndTime, model.timeZone).ToString(formatDate)));
                }
                if (!string.IsNullOrEmpty(model.username))
                {
                    string subCons = string.Format("(customer_account.email like '%{0}%'", model.username) +
                                     string.Format(" OR customer_account.nickname like '%{0}%'", model.username) +
                                     string.Format(" OR customer_account.username like '%{0}%')", model.username);
                    conditions.Add(subCons);
                }

                if (!string.IsNullOrEmpty(model.referrer))
                {
                    string subCons = "customer_account.inviter_username in (select username from customer_account ca where " +
                                      string.Format("ca.email like '%{0}%'", model.referrer) +
                                      string.Format(" OR ca.username like '%{0}%')", model.referrer);
                    conditions.Add(subCons);
                }
                if (!string.IsNullOrEmpty(model.source))
                {
                    conditions.Add(model.source == "website" ? "customer_account.partner_identifier is null" : string.Format("customer_account.partner_identifier = '{0}'", model.source));
                }
                if (conditions.Count > 0)
                {
                    var cons = String.Join(" AND ", conditions.ToArray());
                    users = Platform.Core.Api.Instance.GetUserByConditions(cons).Data;
                }

                if (!string.IsNullOrEmpty(model.export))
                {
                    StringWriter sw = new StringWriter();
                    sw.WriteLine("\"Account ID\",\"Nickname\",\"Username\",\"Email\",\"GToken\",\"VIP\",\"Source\",\"Registered at\",\"Last login at\",\"Recommender\"");
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment;filename=Users.csv");
                    Response.ContentType = "text/csv";
                    if (users != null && users.Count > 0)
                    {
                        string formatDateExport = "MM-dd-yyyy hh:mm";
                        var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                        foreach (var line in users)
                        {
                            line.created_at = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                            line.last_login_at = TimeZoneInfo.ConvertTimeFromUtc(line.last_login_at, est);
                            sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                                       line.id,
                                                       line.nickname,
                                                       line.username,
                                                       line.email,
                                                       line.gtoken,
                                                       line.vip,
                                                       !string.IsNullOrEmpty(line.partner_name) ? line.partner_name : "Website",
                                                        line.created_at.ToString(formatDateExport),
                                                      line.last_login_at.ToString(formatDateExport),
                                                       line.inviter_username));
                        }
                    }
                    Response.Write(sw.ToString());
                    Response.End();
                }
            }
            ViewData["Users"] = users;
            return View(model);
        }

        [HttpGet]
        [RBAC(AccessAction = "access_admin_accountant_page")]
        [Route("admin/user")]
        public ActionResult UserIndex()
        {
            return View(new UserQueryForm());
        }






        #region direct gtoken transaction
        [RBAC(AccessAction = "access_admin_accountant_page")]
        [Route("admin/direct_gtoken_transaction")]
        public ActionResult DirectGTokenTransactionIndex()
        {
            var api = Platform.Core.Api.Instance;
            var result = api.GetAllPartners();
            var model = new TransactionQueryForm();
            model.partners = result.Data.Select(x => x.identifier).ToList();
            return View(model);
        }

        [HttpPost]
        [RBAC(AccessAction = "access_admin_accountant_page")]
        [Route("admin/direct_gtoken_transaction")]
        public ActionResult DirectGTokenTransactionIndex(TransactionQueryForm model)
        {
            var api = Platform.Core.Api.Instance;
            List<DirectTransaction> transactions = api.GetDirectGTokenTransactionByQueryString(model.startTime, model.endTime, model.partner_identifier, model.status, model.partner_order_id, model.gtoken_transaction_id, model.timeZone, model.username).Data;

            if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "transaction")
            {
                if (transactions != null)
                {
                    ViewBag.transactions = transactions;
                }
            }

            if (!String.IsNullOrEmpty(model.export))
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("GToken Transaction ID,Partner,Partner Order ID,Account,Amount,Description,Country Code,Created At");
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
                Response.ContentType = "text/csv";
                if (transactions != null)
                {
                    var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                    foreach (var line in transactions)
                    {
                        DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                        sw.WriteLine(string.Format("{0},{1},{2},\"{3}\",{4},{5},{6},{7}",
                                                   line.gtoken_transaction_id,
                                                   line.partner_name,
                                                   line.partner_order_id,
                                                   line.customer_username,
                                                   line.amount,
                                                   line.description,
                                                   line.country_code,
                                                   targetTime
                                                   ));
                    }
                }
                Response.Write(sw.ToString());
                Response.End();
            }

            var result = api.GetAllPartners();
            model.partners = result.Data.Select(x => x.identifier).ToList();

            return View(model);
        }
        #endregion
    }
}