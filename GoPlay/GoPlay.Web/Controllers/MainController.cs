using Platform.Models;
using GoPlay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay.Web.Helpers;
using System.Net;
using GoPlay.Models;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using GoPlay.Web.Helpers.Extensions;
using System.Configuration;
using System.Threading.Tasks;

namespace GoPlay.Web.Controllers
{
    public class MainController : BaseController
    {
        // GET: Main
        public ActionResult Index()
        {
            var api = GoPlay.Core.GoPlayApi.Instance;

            string limitComingGameConfig = ConfigurationManager.AppSettings["LIMIT_COMING_GAME"];
            int limitComingGame = 0;
            if (!int.TryParse(limitComingGameConfig, out limitComingGame))
                limitComingGame = 3;

            string limitFeatureGameConfig = ConfigurationManager.AppSettings["LIMIT_FEATURE_GAME"];
            int limitFeatureGame = 0;
            if (!int.TryParse(limitFeatureGameConfig, out limitFeatureGame))
                limitFeatureGame = 5;

            string popularGameConfig = ConfigurationManager.AppSettings["POPULAR_GAMES"];
            List<int> popularGames;
            if (string.IsNullOrEmpty(popularGameConfig))
                popularGames = new List<int> {
                    42, 33, 20, 31, 55, 49
                };
            else
                popularGames = popularGameConfig.Split(',').Select(i=> Convert.ToInt32(i)).ToList();

            var gameModel = new GamesViewModel();
            IPAddress ip = WebIpHelper.GetClientIp(Request);
            gameModel.games = Helpers.GameHelper.getCommingGames(CurrentUser, ip, 3);
            var featuredGames = api.getFeaturedGames();
            if (!featuredGames.HasData)
            {
                gameModel.featuredGames = Helpers.GameHelper.GetGamesForCurrentUser(CurrentUser).Take(5).ToList();
            }
            else
            {
                gameModel.featuredGames = featuredGames.Data;
            }
            gameModel.popularGames = api.GetGamesByIds(true, false, popularGames).Data;
            return View(gameModel);
        }

        [Route("about")]
        public ActionResult About()
        {
            // ViewBag.Message = "Your application description page.";

            return View();
        }
        [Route("Contact")]
        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }
        [Route("Club")]
        public ActionResult Club()
        {

            return View();
        }
        [HttpPost]
        [Route("Support")]
        public async Task<JsonResult> Support(SupportViewModel model)
        {

            if (ModelState.IsValid)
            {
                var games = Helpers.GameHelper.GetGamesForCurrentUser(CurrentUser);
                string errorMessage = null;
                string errorKey = "";
                if (model.platform == "game")
                {
                    if (model.gameID == null || model.gameID < 0)
                    {
                        errorMessage = Resources.Resources.This_field_is_required;
                        errorKey = "gameID";
                    }
                    else if (games.SingleOrDefault(x => x.id == model.gameID) == null)
                    {
                        errorMessage = "Game is not active or has been removed";
                        errorKey = "gameID";
                    }
                    else if (string.IsNullOrEmpty(model.gameOSName))
                    {
                        errorMessage = Resources.Resources.This_field_is_required;
                        errorKey = "gameOSName";
                    }
                }
                if (model.forValidate && string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { correct = true });
                }
                else
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        ModelState.AddModelError(errorKey, errorMessage);
                    }
                    else
                    {
                        Game game = null;
                        if (model.gameID != null && model.gameID >= 0)
                        {
                            game = games.SingleOrDefault(x => x.id == model.gameID);
                        }
                        model.game = game;
                        await EmailHelper.SendMailSupport(model);
                        return Json(new { success = true, message = "We will contact you soon" });
                    }
                }
                
            }
            return Json(new { errors = Errors(ModelState) });
        }
        [HttpGet]
        [Route("Support")]
        public ActionResult Support()
        {
            SupportViewModel model = new SupportViewModel();
            if (CurrentUser != null)
            {
                model.customerEmail = CurrentUser.Email;
                model.customerName = CurrentUser.display_name;
            }
            model.listgames = Helpers.GameHelper.GetGamesForCurrentUser(CurrentUser);
            return View(model);
        }
        [Route("Terms")]
        public ActionResult Terms()
        {
            return View();
        }
        [Route("News")]
        public ActionResult News()
        {

            return View();
        }


        [HttpPost]
        [Route("get-session/")]
        public JsonResult checkShowPopup(ShowPopupViewModel data)
        {
            bool show = false;
            string popupNumber = data.popupNumber;
            if (Session[popupNumber] == null)
            {
                show = true;
                Session[popupNumber] = true;
            }
            return Json(new { success = show });
        }

        [HttpGet]
        [Route("get-session/")]
        public JsonResult getCheckShowPopup(ShowPopupViewModel data)
        {
            bool show = false;
            string popupNumber = data.popupNumber;
            return Json(new { success = show }, JsonRequestBehavior.AllowGet);
            // TODO: Check: success always retrn false ? //
        }

        //TODO: this function is not implement View and we dont see it in product + dev server
        [Route("endgods-magic-battles")]
        public ActionResult MagicBattle()
        {

            return View();
        }


        [Route("gdc")]
        public ActionResult Gdc()
        {
            string url = Request.Url.ToString().Replace("gdc", "account/register/gdc");
            return Redirect(url);
        }
    }
}