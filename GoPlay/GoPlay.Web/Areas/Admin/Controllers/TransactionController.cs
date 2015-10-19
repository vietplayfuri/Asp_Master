using GoPlay.Web.ActionFilter;
using GoPlay.Web.Areas.Admin.Models;
using GoPlay.Web.Controllers;
using GoPlay.Web.Helpers;
using GoPlay.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using GoPlay.Models;

namespace GoPlay.Web.Areas.Admin.Controllers
{
    [Authorize]
    [RouteArea("admin")]
    [RoutePrefix("transaction")]
    [RequiredLogin]
    [RBAC(AccessAction = "access_game_accountant_page")]
    public class TransactionController : BaseController
    {
        [Route("")]
        public ActionResult transactionIndex()
        {
            var model = new TransactionQueryForm();
            model.games = GameHelper.GetGamesForAdminUser(CurrentUser);
            return View("index", model);
        }

        [HttpPost]
        [Route("")]
        public ActionResult QueryTransaction(TransactionQueryForm model)
        {
            var api = GoPlayApi.Instance;
            if (model.query != null || model.export != null)
            {
                var games = GameHelper.GetGamesForAdminUser(CurrentUser);
                model.games = games;
                var gameIDs = games.Select(x => x.id).ToArray();

                var gcoinConditions = new List<string>();
                var coinConditions = new List<string>();
                var freeConditions = new List<string>();

                if (model.startTime != null)
                {
                    DateTime startTime = Platform.Utility.Helper.timeFromString(model.startTime, ConfigurationManager.AppSettings["DATETIMEFORMAT"], model.timeZone);
                    gcoinConditions.Add(String.Format("gcoin_transaction.created_at >= '{0}'", startTime.ToString()));
                    coinConditions.Add(String.Format("coin_transaction.created_at >= '{0}'", startTime.ToString()));
                    freeConditions.Add(String.Format("free_coin_transaction.created_at >= '{0}'", startTime.ToString()));
                }

                if (model.endTime != null)
                {
                    DateTime endTime = Platform.Utility.Helper.timeFromString(model.endTime, ConfigurationManager.AppSettings["DATETIMEFORMAT"], model.timeZone);
                    gcoinConditions.Add(String.Format("gcoin_transaction.created_at <= '{0}'", endTime.ToString()));
                    coinConditions.Add(String.Format("coin_transaction.created_at <= '{0}'", endTime.ToString()));
                    freeConditions.Add(String.Format("free_coin_transaction.created_at <= '{0}'", endTime.ToString()));
                }

                if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "exchange")
                {
                    coinConditions.Add("coin_transaction.game_id is not NULL");
                    freeConditions.Add("free_coin_transaction.game_id is not NULL");

                    if (model.gameID != 0)
                    {
                        coinConditions.Add(String.Format("coin_transaction.game_id ={0}", model.gameID));
                        freeConditions.Add(String.Format("free_coin_transaction.game_id = {0}", model.gameID));
                    }
                    else
                    {
                        coinConditions.Add(String.Format("coin_transaction.game_id IN ({0})", String.Join(",", gameIDs)));
                        freeConditions.Add(String.Format("free_coin_transaction.game_id IN ({0})", String.Join(",", gameIDs)));
                    }
                }
                else if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "topup")
                {
                    coinConditions.Add("coin_transaction.amount > 0");
                    freeConditions.Add("free_coin_transaction.amount > 0");
                }
                else if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "gcoin-income")
                {
                    gcoinConditions.Add("gcoin_transaction.amount > 0");
                    if (model.gameID != 0)
                    {
                        gcoinConditions.Add(String.Format("gcoin_transaction.game_id ={0}", model.gameID));
                    }
                }
                else if (Request.Params["tab"] != null && Request.Params["tab"].ToString() == "gcoin-outcome")
                {
                    gcoinConditions.Add("gcoin_transaction.amount < 0");
                }

                if (!string.IsNullOrEmpty(model.status))
                {
                    gcoinConditions.Add(String.Format("gcoin_transaction.status ='{0}'", model.status));
                    coinConditions.Add(String.Format("coin_transaction.status ='{0}'", model.status));
                    freeConditions.Add(String.Format("free_coin_transaction.status ='{0}'", model.status));
                }

                if (!string.IsNullOrEmpty(model.orderID))
                {
                    gcoinConditions.Add(String.Format("gcoin_transaction.order_id ='{0}'", model.orderID));
                    coinConditions.Add(String.Format("coin_transaction.order_id ='{0}'", model.orderID));
                    freeConditions.Add(String.Format("free_coin_transaction.order_id ='{0}'", model.orderID));
                }

                if (!string.IsNullOrEmpty(model.username))
                {
                    string query = "(customer_account.email like '%" + model.username.ToLower() + "%'";
                    query += " OR customer_account.nickname like '%" + model.username.ToLower() + "%'";
                    query += " OR customer_account.username like '%" + model.username.ToLower() + "%')";
                    gcoinConditions.Add(query);
                    coinConditions.Add(query);
                    freeConditions.Add(query);
                }


                if (Request.Params["tab"] != null && (Request.Params["tab"].ToString() == "exchange" || Request.Params["tab"].ToString() == "topup"))
                {
                    var queryString = @"SELECT id, credit_type_id, table_name, order_id, result.created_at, description, amount, result.game_id, receiver_account_id, payment_method, partner_account_id, game_name, package_name, credit_type_name, customer_account_id, username, nickname, price, status, is_free, result.country_code, country_name
                           FROM
                            (
	                            SELECT coin_transaction.id, coin_transaction.credit_type_id, 'coin_transaction' AS table_name, order_id, coin_transaction.created_at, coin_transaction.description, coin_transaction.amount, coin_transaction.game_id, receiver_account_id, payment_method, partner_account_id,game.name as game_name, package.name AS package_name, credit_type.name AS credit_type_name, price, status, coin_transaction.customer_account_id, FALSE as is_free, coin_transaction.country_code, customer_account.username, customer_account.country_name,customer_account.nickname
	                            FROM coin_transaction
                                JOIN  customer_account on customer_account.id = customer_account_id
	                            LEFT JOIN game on game.id = coin_transaction.game_id
                                LEFT JOIN credit_type on credit_type.id = coin_transaction.credit_type_id
                                LEFT JOIN package on package.id = coin_transaction.package_id
	                            WHERE {0}
	                            UNION ALL
	                            SELECT free_coin_transaction.id, free_coin_transaction.credit_type_id, 'free_coin_transaction' AS table_name, order_id, free_coin_transaction.created_at, free_coin_transaction.description, free_coin_transaction.amount, free_coin_transaction.game_id, NULL as receiver_account_id, payment_method , NULL as partner_account_id,game.name as game_name, package.name AS package_name, credit_type.name AS credit_type_name, NULL as price, status, free_coin_transaction.customer_account_id, True as is_free, free_coin_transaction.country_code, customer_account.username, customer_account.country_name, customer_account.nickname
	                            FROM free_coin_transaction
                                JOIN  customer_account on customer_account.id = customer_account_id
	                            LEFT JOIN game on game.id = free_coin_transaction.game_id
                                LEFT JOIN credit_type on credit_type.id = free_coin_transaction.credit_type_id
                                LEFT JOIN package on package.id = free_coin_transaction.package_id
	                            WHERE {1}
                            )result
                           ORDER BY created_at DESC";

                    queryString = String.Format(queryString, String.Join(" AND ", coinConditions.ToArray()), String.Join(" AND ", freeConditions.ToArray()));

                    model.transactions = api.GetTransactionsByCustomQuery(queryString).Data;
                }
                if (Request.Params["tab"] != null && (Request.Params["tab"].ToString() == "gcoin-income" || Request.Params["tab"].ToString() == "gcoin-outcome"))
                {
                    var queryString = @"SELECT gcoin_transaction.*,customer_account.*, game.name as game_name  FROM
                                        gcoin_transaction
                                        JOIN customer_account on customer_account.id = gcoin_transaction.customer_account_id 
                                        LEFT JOIN game on game.id = gcoin_transaction.game_id
                                      WHERE {0}";
                    model.gcoinTransaction = api.GetGCoinTransactionsByCustomQuery(String.Format(queryString, String.Join(" AND ", gcoinConditions.ToArray()))).Data;
                }

                ViewBag.tab = Request.Params["tab"];

                if (!String.IsNullOrEmpty(model.export))
                {
                    StringWriter sw = new StringWriter();
                    if (Request.Params["tab"] != null && (Request.Params["tab"].ToString() == "exchange"))
                    {
                        sw = getStringForExchange(model, sw);
                    }
                    else if (Request.Params["tab"] != null && (Request.Params["tab"].ToString() == "topup"))
                    {
                        sw = getStringForTopup(model, sw);
                    }
                    else if (Request.Params["tab"] != null && (Request.Params["tab"].ToString() == "gcoin-income" || Request.Params["tab"].ToString() == "gcoin-outcome"))
                    {
                        sw = getStringForGCoin(model, sw);
                    }

                    Response.Write(sw.ToString());
                    Response.End();
                }
            }
            else
            {

            }

            return View("index", model);
        }

        private StringWriter getStringForGCoin(TransactionQueryForm model, StringWriter sw)
        {
            sw.WriteLine("Order No,Account,Account ID,Amount,Description,Game,Country Code,Created At,Status\n");
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
            Response.ContentType = "text/csv";
            if (model.gcoinTransaction != null && model.gcoinTransaction.Count > 0)
            {
                var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                foreach (var line in model.gcoinTransaction)
                {
                    DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"",
                                               line.order_id,
                                               line.username,
                                               line.customer_account_id,
                                               line.amount,
                                               line.description,
                                               line.game_name,
                                               line.country_name,
                                               targetTime.ToString(ConfigurationManager.AppSettings["DATETIMEFORMAT"]),
                                               line.status
                                               ));
                }
            }
            return sw;
        }


        private StringWriter getStringForExchange(TransactionQueryForm model, StringWriter sw)
        {
            sw.WriteLine("Order No,Free Transaction,Account,Account ID,Amount,Quantity,Game,Exchange Option,Country Code,Created At,Status\n");
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
            Response.ContentType = "text/csv";
            if (model.transactions != null && model.transactions.Count > 0)
            {
                var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                var api = GoPlayApi.Instance;
                string sqlQuery = @"SELECT * FROM credit_transaction WHERE coin_transaction_id={0} OR free_coin_transaction_id={0}";
                foreach (var line in model.transactions)
                {
                    sqlQuery = String.Format(sqlQuery, line.id);
                    var creditTransaction = api.GetCreditTransactionsByCustomQuery(sqlQuery).Data.SingleOrDefault();
                    DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",
                                               line.order_id,
                                               line.is_free,
                                               line.username,
                                               line.customer_account_id,
                                               line.amount,
                                               creditTransaction != null ? creditTransaction.amount : 0,
                                               line.game_name,
                                               line.credit_type_id.HasValue ? line.credit_type_name : line.package_name,
                                               line.country_name,
                                               targetTime.ToString(ConfigurationManager.AppSettings["DATETIMEFORMAT"]),
                                               line.status
                                               ));
                }
            }
            return sw;
        }

        private StringWriter getStringForTopup(TransactionQueryForm model, StringWriter sw)
        {
            sw.WriteLine("Order No,Free Transaction,Account,Account ID,Amount,Price,Source,Description,Country,Created At,Status\n");
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
            Response.ContentType = "text/csv";
            if (model.transactions != null && model.transactions.Count > 0)
            {
                var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                foreach (var line in model.transactions)
                {
                    DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",
                                               line.order_id,
                                               line.is_free,
                                               line.username,
                                               line.customer_account_id,
                                               line.amount,
                                               line.price ?? 0,
                                               line.source(),
                                               line.description,
                                               line.country_name,
                                               targetTime.ToString(ConfigurationManager.AppSettings["DATETIMEFORMAT"]),
                                               line.status
                                               ));
                }
            }
            return sw;
        }
    }
}