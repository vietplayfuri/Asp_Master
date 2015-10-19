using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Areas.Admin.Models;
using GoPlay.Web.Controllers;
using GoPlay.Web.Helpers;
using GoPlay.Web.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Platform.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Areas.Admin.Controllers
{
     [Authorize]
    [RouteArea("admin")]
    [RequiredLogin]
    [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_CUSTOMER_SUPPORT)]
    public class ReportController : BaseController
    {
        [Route("report")]
        public ActionResult Index()
        {
            var api = GoPlayApi.Instance;
            List<SimpleGame> shownGames = new List<SimpleGame>() {
                new SimpleGame{ name = GoPlayConstantValues.S_ALL_GAMES, id = 0}
            };
            var games = api.GetGamesForDropdownlist(CurrentUser.Id, CurrentUser.GetRoles().Select(c => c.RoleName).ToList());
            shownGames.AddRange(games);
            return View(new AdminReportModel()
            {
                games = shownGames
            });
        }


        [HttpPost]
        [Route("report")]
        public ActionResult Index(AdminReportModel model)
        {
            var api = GoPlayApi.Instance;
            List<SimpleGame> shownGames = new List<SimpleGame>() {
                new SimpleGame{ name = GoPlayConstantValues.S_ALL_GAMES, id = 0}
            };

            var games = api.GetGamesForDropdownlist(CurrentUser.Id, CurrentUser.GetRoles().Select(c => c.RoleName).ToList());
            shownGames.AddRange(games);

            if (!string.IsNullOrEmpty(model.export) || !string.IsNullOrEmpty(model.query))
            {
                List<string> condition = new List<string>();

                DateTime dateFrom = DateTime.UtcNow;
                DateTime dateTo = DateTime.UtcNow;
                string strFrom = dateFrom.ToString(ConstantValues.S_DATETIME_FORMAT);
                string strTo = dateTo.ToString(ConstantValues.S_DATETIME_FORMAT);

                if (!string.IsNullOrEmpty(model.fromTime))
                {
                    dateFrom = Helper.timeFromString(model.fromTime, model.timezone);
                    strFrom = dateFrom.ToString(ConstantValues.S_DATETIME_FORMAT);
                    condition.Add(string.Format("created_at >= '{0}'", strFrom));
                }

                if (!string.IsNullOrEmpty(model.toTime))
                {
                    dateTo = Helper.timeFromString(model.toTime, model.timezone);
                    strTo = dateTo.ToString(ConstantValues.S_DATETIME_FORMAT);
                    condition.Add(string.Format("created_at <= '{0}'", strTo));
                }

                bool isDAU = (!string.IsNullOrEmpty(model.isDaily) && model.isDaily == "on")
                    ? true
                    : false;

                if (model.game_id == 0)
                {
                    if (!isDAU)
                    {
                        model.source = GenerateReportData(strFrom, strTo, condition, games, model.timezone);
                    }
                    else
                    {
                        model.source = GenerateReportDailyData(dateFrom, dateTo, games, model.timezone);
                    }
                }
                else if (model.game_id > 0)
                {
                    SimpleGame game = games.First(g => g.id == model.game_id);
                    if (!isDAU)
                    {
                        condition.Add(string.Format("game_id = {0}", model.game_id));
                        model.source = GenerateReportData(strFrom, strTo, condition, games, model.timezone, game.name);
                    }
                    else
                    {
                        model.source = GenerateReportDailyData(dateFrom, dateTo, games, model.timezone, game.name, model.game_id);
                    }
                }

                if (!string.IsNullOrEmpty(model.export))
                {
                    StringWriter sw = new StringWriter();
                    sw.WriteLine("\"From\",\"To\",\"Game\",\"Number of Active Users\"");
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", string.Format("attachment;filename=report_{0}.csv", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString()));
                    Response.ContentType = "text/csv";

                    if (model.source != null && model.source.Any())
                    {
                        bool lastRow = false;
                        foreach (var line in model.source)
                        {
                            if (lastRow && line.name == GoPlayConstantValues.S_ALL_GAMES)
                                sw.WriteLine(string.Empty);

                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"",
                                                       line.fromDate,
                                                       line.toDate,
                                                       line.name,
                                                       line.count));
                            lastRow = true;
                        }
                    }
                    Response.Write(sw.ToString());
                    Response.End();
                }
            }

            model.games = shownGames;
            return View(model);
        }


        private List<AdminReportDataModel> GenerateReportDailyData(DateTime dateFrom, DateTime dateTo, List<SimpleGame> games, string timezone, string gameName = null, int? game_id = null)
        {
            List<AdminReportDataModel> sources = new List<AdminReportDataModel>();
            List<string> condition;

            while (dateFrom < dateTo)
            {
                condition = new List<string>();

                string strFrom = dateFrom.ToString(ConstantValues.S_DATETIME_FORMAT);
                string strTo = dateFrom.AddSeconds((Helper.TimeDelta(days: 1) + Helper.TimeDelta(seconds: -1)).TotalSeconds).ToString(ConstantValues.S_DATETIME_FORMAT);

                condition.Add(string.Format("created_at >= '{0}'", strFrom));
                condition.Add(string.Format("created_at < '{0}'", strTo));

                sources.AddRange(GenerateReportData(strFrom, strTo, condition, games, timezone, gameName, game_id));

                dateFrom = dateFrom.AddSeconds(Helper.TimeDelta(days: 1).TotalSeconds);
            }

            return sources;
        }

        private List<AdminReportDataModel> GenerateReportData(string strFrom, string strTo,
            List<string> condition, List<SimpleGame> games, string timezone, string gameName = null, int? game_id = null)
        {
            List<AdminReportDataModel> sources = new List<AdminReportDataModel>();
            var api = GoPlayApi.Instance;
            int count = api.CountCustomerAccountId(string.Join(" AND ", condition));

            strFrom = Helper.ConvertTimeFromUtc(strFrom, timezone).ToString(ConstantValues.S_DATETIME_FORMAT);
            strTo = Helper.ConvertTimeFromUtc(strTo, timezone).ToString(ConstantValues.S_DATETIME_FORMAT);

            sources.Add(new AdminReportDataModel
            {
                fromDate = strFrom,
                toDate = strTo,
                name = !string.IsNullOrEmpty(gameName) ? gameName : GoPlayConstantValues.S_ALL_GAMES,
                count = count
            });

            if (game_id.HasValue)
                condition.Add(string.Format("game_id = {0}", game_id));

            if (string.IsNullOrEmpty(gameName))
                foreach (var game in games)
                {
                    condition.Add(string.Format("game_id = {0}", game.id));
                    count = api.CountCustomerAccountId(string.Join(" AND ", condition));
                    if (count > 0)
                        sources.Add(new AdminReportDataModel
                        {
                            fromDate = strFrom,
                            toDate = strTo,
                            name = game.name,
                            count = count
                        });

                    condition.RemoveAt(2); //keep the list is always have 2 items
                }
            return sources;
        }


        [Route("report-active-user")]
        public ActionResult ReportActiveUser()
        {
            return View(new AdminReportActiveUserModel());
        }


        [HttpPost]
        [Route("report-active-user")]
        public ActionResult ReportActiveUser(AdminReportActiveUserModel model)
        {
            var api = GoPlayApi.Instance;
            var games = api.GetGamesForDropdownlist(CurrentUser.Id, CurrentUser.GetRoles().Select(c => c.RoleName).ToList());

            if (!string.IsNullOrEmpty(model.query))
            {
                List<string> condition = new List<string>();

                DateTime dateFrom = DateTime.UtcNow;
                DateTime dateTo = DateTime.UtcNow;
                string strFrom = dateFrom.ToString(ConstantValues.S_DATETIME_FORMAT);
                string strTo = dateTo.ToString(ConstantValues.S_DATETIME_FORMAT);

                if (!string.IsNullOrEmpty(model.fromTime))
                {
                    dateFrom = Helper.timeFromString(model.fromTime, model.timezone);
                    strFrom = dateFrom.ToString(ConstantValues.S_DATETIME_FORMAT);
                    condition.Add(string.Format("created_at >= '{0}'", strFrom));
                }

                if (!string.IsNullOrEmpty(model.toTime))
                {
                    dateTo = Helper.timeFromString(model.toTime, model.timezone);
                    strTo = dateTo.ToString(ConstantValues.S_DATETIME_FORMAT);
                    condition.Add(string.Format("created_at <= '{0}'", strTo));
                }

                foreach (var game in games)
                {
                    condition.Add(string.Format("game_id = {0}", game.id));
                    int count = api.CountCustomerAccountId(string.Join(" AND ", condition));
                    if (count > 0)
                        model.source.Add(new AdminReportDataModel
                        {
                            fromDate = model.fromTime,
                            toDate = model.toTime,
                            name = game.name,
                            count = count
                        });
                    else
                        model.empty_games.Add(game.id);

                    condition.RemoveAt(2); //keep the list is always have 2 items
                }

                //TODO: handle chart later
            }

            return View(model);
        }
    }
}