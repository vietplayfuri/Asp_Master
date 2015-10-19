using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Areas.Admin.Models;
using GoPlay.Web.Controllers;
using Platform.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay.Web.Helpers;
namespace GoPlay.Web.Areas.Admin.Controllers
{
    [Authorize]
    [RouteArea("admin")]
    [RoutePrefix("user")]
    [RequiredLogin]
    [RBAC(AccessAction = GoPlayConstantValues.S_PERMISSION_ACCESS_ADMIN_ACCOUNTANT_PAGE)]
    public class UserController : BaseController
    {
        [Route("")]
        public ActionResult Index()
        {
            var api = GoPlayApi.Instance;
            List<SimpleGame> shownGames = new List<SimpleGame>() {
                new SimpleGame{ name = GoPlayConstantValues.S_ALL, id = -1},
                new SimpleGame{ name = GoPlayConstantValues.S_WEBSITE, id = 0}
            };
            var games = api.GetGamesForDropdownlist(CurrentUser.Id, CurrentUser.GetRoles().Select(c => c.RoleName).ToList());
            shownGames.AddRange(games);

            List<string> shownManager = new List<string>() {
                GoPlayConstantValues.S_ALL
            };

            string accountManagerConfig = ConfigurationManager.AppSettings["ACCOUNT_MANAGERS"];
            if (!string.IsNullOrEmpty(accountManagerConfig))
                shownManager.AddRange(accountManagerConfig.Split(','));

            AdminUserModel model = new AdminUserModel()
            {
                games = shownGames,
                account_managers = shownManager
            };

            return View(model);
        }

        [HttpPost]
        [Route("")]
        public ActionResult Index(AdminUserModel model)
        {
            var api = GoPlayApi.Instance;
            List<SimpleGame> shownGames = new List<SimpleGame>() {
                new SimpleGame{ name = GoPlayConstantValues.S_ALL, id = -1},
                new SimpleGame{ name = GoPlayConstantValues.S_WEBSITE, id = 0}
            };
            var games = api.GetGamesForDropdownlist(CurrentUser.Id, CurrentUser.GetRoles().Select(c => c.RoleName).ToList());
            shownGames.AddRange(games);

            List<string> shownManager = new List<string>() {
                GoPlayConstantValues.S_ALL
            };

            string accountManagerConfig = ConfigurationManager.AppSettings["ACCOUNT_MANAGERS"];
            if (!string.IsNullOrEmpty(accountManagerConfig))
                shownManager.AddRange(accountManagerConfig.Split(','));

            model.games = shownGames;
            model.account_managers = shownManager;

            if (!string.IsNullOrEmpty(model.export) || !string.IsNullOrEmpty(model.query))
            {
                model.users = api.GetUserByConditions(model.timezone, model.regStartTime, model.regEndTime, model.loginStartTime, model.loginEndTime, model.username, model.referrer, model.game_id, model.account_manager).Data;

                if (!string.IsNullOrEmpty(model.export))
                {
                    StringWriter sw = new StringWriter();
                    sw.WriteLine("\"Account ID\",\"Nickname\",\"Username\",\"Email\",\"GoPlay Token\",\"Free Play Token\",\"VIP\",\"Source\",\"Registered at\",\"Last login at\",\"Recommender\"");
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", string.Format("attachment;filename=users_{0}.csv", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString()));
                    Response.ContentType = "text/csv";
                    if (model.users != null && model.users.Any())
                    {
                        foreach (var line in model.users)
                        {
                            line.created_at = Helper.ConvertTimeFromUtc(line.created_at.ToString(ConstantValues.S_DATETIME_FORMAT), model.timezone);
                            line.last_login_at = Helper.ConvertTimeFromUtc(line.last_login_at.ToString(ConstantValues.S_DATETIME_FORMAT), model.timezone);
                            sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",
                                                       line.id,
                                                       line.nickname,
                                                       line.username,
                                                       line.email,
                                                       line.play_token,
                                                       line.free_play_token,
                                                       line.vip,
                                                       !string.IsNullOrEmpty(line.game_name) ? line.game_name : GoPlayConstantValues.S_WEBSITE,
                                                       line.created_at.ToString(ConstantValues.S_DATETIME_FORMAT),
                                                       line.last_login_at.ToString(ConstantValues.S_DATETIME_FORMAT),
                                                       line.inviter_username));
                        }
                    }
                    Response.Write(sw.ToString());
                    Response.End();
                }
            }

            return View(model);
        }


        [Route("{id}")]
        public ActionResult UserDetail(int id)
        {
            List<string> shownManager = new List<string>();
            string accountManagerConfig = ConfigurationManager.AppSettings["ACCOUNT_MANAGERS"];
            if (!string.IsNullOrEmpty(accountManagerConfig))
                shownManager.AddRange(accountManagerConfig.Split(','));

            var api = GoPlayApi.Instance;
            var user = api.GetUserById(id).Data;

            if (user == null)
            {
                this.Flash("Account not found", FlashLevel.Alert);
                return Redirect("/admin/user");
            }
            var model = new AdminUserIndexModel
            {
                user = user,
                account_managers = shownManager,
            };
            model.transactions = api.GetTransactions(id).Data;
            model.accountManagerNote = !string.IsNullOrEmpty(model.accountManagerNote) ? model.accountManagerNote : "Account Manager Note";
            model.GetLogs(id);

            return View(model);
        }

        [Route("username/{username}")]
        public ActionResult UserName(string username)
        {
            List<string> shownManager = new List<string>();
            string accountManagerConfig = ConfigurationManager.AppSettings["ACCOUNT_MANAGERS"];
            if (!string.IsNullOrEmpty(accountManagerConfig))
                shownManager.AddRange(accountManagerConfig.Split(','));

            var api = GoPlayApi.Instance;
            var user = api.GetUserByUserName(username).Data;

            if (user == null)
            {
                this.Flash("Account not found", FlashLevel.Alert);
                return Redirect("/admin/user");
            }
            var model = new AdminUserIndexModel
            {
                user = user,
                account_managers = shownManager,
            };
            model.transactions = api.GetTransactions(user.id).Data;
            model.accountManagerNote = !string.IsNullOrEmpty(model.accountManagerNote) ? model.accountManagerNote : "Account Manager Note";
            model.GetLogs(user.id);

            return View("UserDetail", model);
        }


        [HttpPost]
        [Route("{id}")]
        public ActionResult UserDetail(AdminUserIndexModel model)
        {
            List<string> shownManager = new List<string>();
            string accountManagerConfig = ConfigurationManager.AppSettings["ACCOUNT_MANAGERS"];
            if (!string.IsNullOrEmpty(accountManagerConfig))
                shownManager.AddRange(accountManagerConfig.Split(','));

            int userId = 0;
            if (Request.Url != null && Request.Url.Segments.Count() > 3 && !string.IsNullOrEmpty(Request.Url.Segments[3]))
            {
                Int32.TryParse(Request.Url.Segments[3], out userId);
            }

            var api = GoPlayApi.Instance;
            model.user = api.GetUserById(userId).Data;
            if (model.user != null)
            {
                if (!string.IsNullOrEmpty(model.accountManagerNote) && CurrentUser.GetRoles().Any(r => r.RoleName == GoPlayConstantValues.S_ROLE_CUSTOMER_SUPPORT))
                {
                    api.UpdateCustomerAccount(model.user.id, model.accountManager, model.accountManagerNote);
                }
                model.transactions = api.GetTransactions(userId).Data;
                model.account_managers = shownManager;
                model.GetLogs(userId);

                return View(model);
            }

            this.Flash("Account not found", FlashLevel.Alert);
            return Redirect("/admin/user");
        }
    }
}