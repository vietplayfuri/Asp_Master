using GoPlay.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Platform.Utility.Helpers.Extensions;
using GoPlay.Core;
using GoPlay.Web.ActionFilter;
using Newtonsoft.Json.Linq;
using GoPlay.Web.Helpers.Extensions;
using Platform.Utility;
using GoPlay.Web.Helpers;
using GoPlay.Models;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using System.Dynamic;

namespace GoPlay.Web.Controllers
{
    public class GamesController : BaseController
    {
        // GET: Games
        [Route("game")]
        public ActionResult Index()
        {
            var api = GoPlayApi.Instance;
            var gameModel = new GamesViewModel();
            gameModel.featuredGames = api.getFeaturedGames().Data;
            gameModel.popularGames = api.GetGamesByIds(true, false, true).Data;
            return View(gameModel);
        }

        [HttpPost]
        [Route("game/getgames/{page}")]
        public ActionResult GetGames(int page = 1)
        {
            var api = GoPlayApi.Instance;
            var gameModel = new GamesViewModel();
            int per_page;
            if (!int.TryParse(ConfigurationManager.AppSettings["GAME_PER_PAGE"], out per_page))
            {
                per_page = 12;
            }
            int count = 0;

            var gamesTotal = Helpers.GameHelper.GetGamesForCurrentUser(CurrentUser);
            count = gamesTotal.Count;
            gameModel.games = Helpers.GameHelper.getGamesPerPageForCurrentUser(page, per_page, CurrentUser, gamesTotal);
            gameModel.pagination = new Pagination(page, per_page, count);
            gameModel.genres = Helpers.GameHelper.getLocalizedField(gamesTotal, "genre");

            IPAddress clientIp = Request.GetClientIp();
            gameModel.games.ForEach(g =>
                {
                    g.genre = GameHelper.getLocalized(g, "genre");
                    g.isComingSoon = GameHelper.isComingSoon(g, CurrentUser, clientIp);
                    JObject platforms = GameHelper.getDownloadLinksForCurrentUser(g, CurrentUser, clientIp);
                    if (platforms != null && platforms.Properties().Any())
                    {
                        Dictionary<string, string> links = new Dictionary<string, string>();
                        foreach (var item in platforms.Properties())
                        {
                            links[item.Name] = item.Value.ToString();
                        }
                        g.platforms = links;
                    }
                });

            return Json(new
            {
                data = gameModel
            });
        }


        [HttpPost]
        [Route("game/getgenres")]
        public ActionResult GetGenres()
        {
            var gamesTotal = Helpers.GameHelper.GetGamesForCurrentUser(CurrentUser);
            var genres = Helpers.GameHelper.getLocalizedField(gamesTotal, "genre");
            return Json(new
            {
                data = genres
            });
        }


        // GET: Games/Details/param
        [Route("game/detail")]
        [Route("game/detail/{param}")]
        public ActionResult Details(string param)
        {
            var gameId = param.Split('-').Last();
            var api = GoPlay.Core.GoPlayApi.Instance;
            var game = api.GetGame(Int32.Parse(gameId));
            if (!game.HasData)
            {
                throw new HttpException(404, game.Error.ToString());
            }
            if (!(game.Data.is_active || RBAC.checkRolePermission(CurrentUser, "game_admin")))
            {
                return RedirectToAction("index", "game");
            }
            var detailGameModel = new DetailGameViewModel();
            detailGameModel.slider_len = 0;
            detailGameModel.game = game.Data;
            detailGameModel.games = RandomHelper.Sample(GameHelper.GetGamesForCurrentUser(CurrentUser), 3).ToList();
            if (game.Data.slider_images.Count() > 0)
            {
                var lstSlider = JsonHelper.DeserializeObject<Slider>(game.Data.slider_images);
                if (lstSlider != null && lstSlider.images != null)
                    detailGameModel.slider_len = lstSlider.images.Count();
            }

            return View(detailGameModel);
        }

        [Route("game/find-game")]
        public JsonResult FindGame(string term)
        {
            var api = GoPlayApi.Instance;
            var result = api.findGames(term.ToLower().Trim());
            if (result.HasData)
            {
                var data = result.Data.Select(x => new
                {
                    id = x.id,
                    icon_filename = x.icon_filename,
                    name = x.name,
                    @params = x.detail_name,
                    studio = x.studio_name
                });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new { });
        }


        [HttpPost]
        [Route("game/search/{keyword}/{genre}/{platform}/{release}/{page}")]
        public ActionResult SeachGame(int page = 1, string keyword = "default", string genre = "default", string platform = "default", string release = "default")
        {
            var api = GoPlayApi.Instance;
            if (keyword == "all-keyword")
            {
                keyword = "";
            }
            int per_page;
            if (!int.TryParse(ConfigurationManager.AppSettings["GAME_PER_PAGE"], out per_page))
            {
                per_page = 12;
            }
            int count = 0;
            var searchConditions = new GoPlay.Models.SearchCondition()
            {
                keywork = keyword,
                genre = genre,
                platform = platform,
                release = release
            };

            var allGames = GameHelper.GetGamesForCurrentUser(CurrentUser, searchConditions);
            var allGamesByCondition = GameHelper.FilterGamesByCondition(allGames, searchConditions, page, per_page, out count);
            List<GameSearch> gameList = new List<GameSearch>();

            IPAddress clientIp = Request.GetClientIp();
            var host = string.Format("{0}://{1}/", Request.Url.Scheme, Request.Url.Authority);
            allGamesByCondition.ForEach(game =>
            {
                GameSearch gameSearch = new GameSearch
                {
                    id = game.id,
                    name = game.name,
                    web_link = GameHelper.GenerateLinkGame(game, host),
                    game_invite_protocol = game.game_invite_protocol,
                    icon_filename = game.icon_filename,
                    @params = string.Format("{0}-{1}", game.name, game.id),
                    studio = game.studio_name,
                    isComingSoon = GameHelper.isComingSoon(game, CurrentUser, clientIp),
                    banner_filename = game.banner_filename,
                    genre = GameHelper.getLocalized(game, "genre"),
                    detail_name = (game.name + "-" + game.id).GenerateSlug()
                };
                JObject platforms = GameHelper.getDownloadLinksForCurrentUser(game, CurrentUser, clientIp);
                if (platforms != null && platforms.Properties().Any())
                {
                    Dictionary<string, string> links = new Dictionary<string, string>();
                    foreach (var item in platforms.Properties())
                    {
                        links[item.Name] = item.Value.ToString();
                    }
                    gameSearch.platforms = links;
                }

                List<promotion> promotion = api.GetOngoingPromotion(game.id.ToString()).Data;
                if (promotion != null && promotion.Any())
                    gameSearch.promotion = promotion;
                gameList.Add(gameSearch);
            });

            return Json(new
            {
                gameList = gameList.OrderByDescending(x => x.id),//allGamesByCondition,
                totalGames = count
            });
        }



        [Route("game/search/")]
        [Route("game/search/page/{page}")]
        [Route("game/search/{keyword}/{genre}/{platform}/{release}/")]
        public ActionResult SeachGameGet(int page = 1, string keyword = "default", string genre = "default", string platform = "default", string release = "default")
        {
            var api = GoPlayApi.Instance;
            if (keyword == "all-keyword")
            {
                keyword = "";
            }
            int per_page;
            if (!int.TryParse(ConfigurationManager.AppSettings["GAME_PER_PAGE"], out per_page))
            {
                per_page = 12;
            }
            int count = 0;
            var searchConditions = new GoPlay.Models.SearchCondition()
            {
                keywork = keyword,
                genre = genre,
                platform = platform,
                release = release
            };

            var allGames = GameHelper.GetGamesForCurrentUser(CurrentUser, searchConditions);
            var allGamesByCondition = GameHelper.FilterGamesByCondition(allGames, searchConditions, page, per_page, out count);

            List<GameSearch> gameList = new List<GameSearch>();

            IPAddress clientIp = Request.GetClientIp();
            var host = string.Format("{0}://{1}/", Request.Url.Scheme, Request.Url.Authority);
            allGamesByCondition.ForEach(game =>
            {
                GameSearch gameSearch = new GameSearch
                {
                    id = game.id,
                    name = game.name,
                    web_link = GameHelper.GenerateLinkGame(game, host),
                    game_invite_protocol = game.game_invite_protocol,
                    icon_filename = game.icon_filename,
                    @params = string.Format("{0}-{1}", game.name, game.id),
                    studio = game.studio_name,
                    isComingSoon = GameHelper.isComingSoon(game, CurrentUser, clientIp),
                    banner_filename = game.banner_filename,
                    genre = GameHelper.getLocalized(game, "genre")
                };
                JObject platforms = GameHelper.getDownloadLinksForCurrentUser(game, CurrentUser, clientIp);
                if (platforms != null && platforms.Properties().Any())
                {
                    Dictionary<string, string> links = new Dictionary<string, string>();
                    foreach (var item in platforms.Properties())
                    {
                        links[item.Name] = item.Value.ToString();
                    }
                    gameSearch.platforms = links;
                }

                List<promotion> promotion = api.GetOngoingPromotion(game.id.ToString()).Data;
                if (promotion != null && promotion.Any())
                    gameSearch.promotion = promotion;
                gameList.Add(gameSearch);
            });

            return Json(new
            {
                gameList = gameList,
                totalGames = count
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Route("game/game-list")]
        public JsonResult getGameList()
        {
            var api = GoPlayApi.Instance;
            var result = api.GetInvitableGames(CurrentUser.Id, true, false, true).Data;
            if (result != null)
            {
                string host = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~/"));
                string rootUrl = string.Format("{0}{1}/{2}/", host, "game", "detail");
                var data = result.Select(x => new
                {
                    id = x.id,
                    name = x.name,
                    web_link = string.Format("{0}{1}", rootUrl, GameHelper.generateParam(x)),
                    game_invite_protocol = x.game_invite_protocol,
                    icon_filename = x.icon_filename
                });

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new { });
        }

        [RequiredLogin]
        [Route("game/{id}/exchange-items")]
        public string GetExchangeItems(int id)
        {
            var api = GoPlayApi.Instance;
            Game game = api.GetGame(id).Data;
            if (game == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            List<CreditType> creditTypes = api.GetCreditTypes(game.id, true, false).Data;
            List<Package> packages = api.GetPackages(game.id, true, false).Data;

            var result = new GetExchangeItemsModel();
            creditTypes.ForEach(c => result.Credit.Add(c.ToDictationary()));

            foreach (var package in packages)
            {
                if (package.limited_time_offer.HasValue && package.limited_time_offer.Value > 0)
                {
                    int exteralExchange = 0;
                    int coinTransaction = 0;
                    int freeCoinTransaction = 0;

                    var externalExchanges = api.GetExternalExchanges(CurrentUser.Id, game.id, package.string_identifier);
                    if (externalExchanges != null)
                        exteralExchange = externalExchanges.Count();

                    var coinTransactions = api.GetCoinTransaction(game.id, CurrentUser.Id, package.id).Data;
                    if (coinTransactions != null)
                        coinTransaction = coinTransactions.Count();

                    var freeCoinTransactions = api.GetFreeCoinTransaction(game.id, CurrentUser.Id, package.id).Data;
                    if (freeCoinTransactions != null)
                        freeCoinTransaction = freeCoinTransactions.Count();

                    if ((exteralExchange + coinTransaction + freeCoinTransaction) >= package.limited_time_offer)
                        continue;
                }
                result.Package.Add(package.ToDictationary());
            }

            return JsonConvert.SerializeObject(result);
        }

        [HttpGet]
        [RequiredLogin]
        [Route("game/game-listing")]
        public JsonResult gameListing(GameListingViewModel model)
        {
            List<Game> games = null;
            var api = GoPlayApi.Instance;
            string sqlQuery = string.Empty;
            if (this.HasRole("admin"))
            {
                sqlQuery = @"SELECT game.*, studio.name as studio_name  FROM game 
                                JOIN studio on studio.id = game.studio_id
                                WHERE lower(game.name) LIKE '%{0}%'
                                ORDER BY game.name";
                sqlQuery = String.Format(sqlQuery, model.term);
            }
            else if (this.HasRole("game_admin") || this.HasRole("game_accountant"))
            {
                sqlQuery = @"SELECT game.*, studio.name as studio_name FROM game 
                                JOIN studio on studio.id = game.studio_id
                                JOIN studio_admin_assignment saa ON saa.studio_id = game.studio_id 
                                WHERE saa.game_admin_id = {0} AND game.is_archived = False AND lower(game.name) LIKE '%{1}%'          
                                ORDER BY game.name";
                sqlQuery = String.Format(sqlQuery, CurrentUser.Id, model.term);
            }
            else
            {
                sqlQuery = @"SELECT game.*, studio.name as studio_name FROM game 
                                JOIN studio on studio.id = game.studio_id
                                WHERE game.is_active=True AND game.is_archived = False AND lower(game.name) LIKE '%{0}%'          
                                ORDER BY game.name";
                sqlQuery = String.Format(sqlQuery, model.term);
            }
            games = api.GetGamesByCustomQuery(sqlQuery).Data;
            if (CurrentUser != null)
            {
                string query = @"SELECT game_id, MAX(created_at)
                                FROM api_log 
                                WHERE customer_account_id = {0}
                                  AND game_id IN (
                                    SELECT id 
                                    FROM game 
                                    WHERE game.is_active = true
                                      AND game.is_archived = false
                                      AND game.name like '%{1}%'
                                  )
                                GROUP BY game_id 
                                ORDER BY MAX(created_at) DESC";
                query = String.Format(query, CurrentUser.Id, model.term);
                var logs = api.GetApiLogByCustomQuery(query).Data;
                if (logs != null)
                {
                    foreach (var item in logs)
                    {
                        var g = api.GetGame(item.game_id);
                        if (g.HasData)
                        {
                            games.Add(g.Data);
                        }
                    }
                }
            }

            if (games == null || !games.Any())
            {
                return Json(new List<Game>(), JsonRequestBehavior.AllowGet);
            }
            string game_ids = string.Join(",", games.Select(x => x.id));
            var OngoingPromotion = api.GetOngoingPromotion(game_ids).Data;
            string host = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~/"));
            string rootUrl = string.Format("{0}{1}/{2}/", host, "game", "detail");
            var obj = Json(games.Select(x => x.ToDict(GameHelper.generateParam(x),
                                                        rootUrl,
                                                        OngoingPromotion == null ? null : OngoingPromotion.FirstOrDefault(c => c.game_id == x.id)
                                                        )).ToList(), JsonRequestBehavior.AllowGet);
            return obj;
        }
    }
}
