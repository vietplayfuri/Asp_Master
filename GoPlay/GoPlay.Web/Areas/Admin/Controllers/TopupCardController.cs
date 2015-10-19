using GoPlay.Core;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Areas.Admin.Models;
using GoPlay.Web.Controllers;
using GoPlay.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Areas.Admin.Controllers
{
     [Authorize]
    [RouteArea("admin")]
    [RoutePrefix("topupcard")]
    [RequiredLogin]
    [RBAC(AccessAction = "access_admin_accountant_page")]
    public class TopupCardController : BaseController
    {
        [Route("")]
        public ActionResult Index()
        {
            var model = new CardQueryForm();
            return View(model);
        }

        [HttpPost]
        [Route("")]
        public ActionResult topupCardIndex(CardQueryForm model)
        {
            var api = GoPlayApi.Instance;
            if (model.query != null || model.export != null)
            {

                var conditions = new List<string>();

                if (model.usageStartTime != null)
                {
                    DateTime usageStartTime = Platform.Utility.Helper.timeFromString(model.usageStartTime, ConfigurationManager.AppSettings["DATETIMEFORMAT"], model.timeZone);
                    conditions.Add(String.Format("topup_card.used_at >= '{0}'", usageStartTime.ToString()));
                }

                if (model.usageEndTime != null)
                {
                    DateTime usageEndTime = Platform.Utility.Helper.timeFromString(model.usageEndTime, ConfigurationManager.AppSettings["DATETIMEFORMAT"], model.timeZone);
                    conditions.Add(String.Format("topup_card.used_at <= '{0}'", usageEndTime.ToString()));
                }

                if (!string.IsNullOrEmpty(model.cardNumber))
                {
                    string query = "(topup_card.card_number like '%" + model.cardNumber + "%'";
                    query += " OR topup_card.card_password like '%" + model.cardNumber + "%')";
                    conditions.Add(query);
                }


                if (!string.IsNullOrEmpty(model.status))
                {
                    if (model.status == "used")
                    {
                        conditions.Add("topup_card.used_at IS NOT NULL");
                    }
                    else if (model.status == "unused")
                    {
                        conditions.Add("topup_card.used_at IS NULL");
                    }
                }

                if (model.isFree)
                {
                    conditions.Add("topup_card.is_free = True");
                }

                if (!string.IsNullOrEmpty(model.username))
                {
                    string query = "(customer_account.email like '%" + model.username.ToLower() + "%'";
                    query += " OR customer_account.nickname like '%" + model.username.ToLower() + "%'";
                    query += " OR customer_account.username like '%" + model.username.ToLower() + "%')";
                    conditions.Add(query);
                }
                if (conditions.Count > 0)
                {

                    string sqlQuery = @"SELECT topup_card.*, customer_account.username FROM topup_card 
                                        LEFT JOIN customer_account on customer_account.id = topup_card.customer_account_id
                                        WHERE {0}";
                    sqlQuery = String.Format(sqlQuery, String.Join(" AND ", conditions.ToArray()));
                    model.cards = api.GetTopupCardsByCustomQuery(sqlQuery).Data;
                }

                if (!String.IsNullOrEmpty(model.export))
                {
                    StringWriter sw = new StringWriter();
                    sw = getStringForExport(model, sw);

                    Response.Write(sw.ToString());
                    Response.End();
                }
            }
            else
            {

            }

            return View("index", model);
        }
        private StringWriter getStringForExport(CardQueryForm model, StringWriter sw)
        {
            sw.WriteLine("Card Number, Card Password, Value, State, Account, Is Free, Validity Time, Used Time\n");
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=topupcards_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
            Response.ContentType = "text/csv";
            if (model.cards != null && model.cards.Count > 0)
            {
                var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                foreach (var line in model.cards)
                {
                    DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                    DateTime validity_date = TimeZoneInfo.ConvertTimeFromUtc(line.validity_date, est);
                    var used_at = line.used_at.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(line.used_at.Value, est).ToString(ConfigurationManager.AppSettings["DATETIMEFORMAT"]) : String.Empty;
                    sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                               line.card_number,
                                               line.card_password,
                                               line.amount,
                                               line.amount,
                                               line.used_at.HasValue ? "Used" : "Unused",
                                               line.customer_account_id.HasValue ? line.username : String.Empty,
                                               line.is_free,
                                               validity_date,
                                               used_at
                                               ));
                }
            }
            return sw;

        }
    }
}