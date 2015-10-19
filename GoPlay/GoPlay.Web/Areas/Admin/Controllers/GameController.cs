using GoPlay.Core;
using GoPlay.Web.Controllers;
using GoPlay.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay.Web.Helpers;
using GoPlay.Web.Areas.Admin.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;
using GoPlay.Models;
using Platform.Utility;
using GoPlay.Web.Models;

namespace GoPlay.Web.Areas.Admin.Controllers
{
    [Authorize]
    [RouteArea("admin")]
    [RoutePrefix("game")]
    [RequiredLogin]
    public class GameController : BaseController
    {
        [Route("")]
        [RBAC(AccessAction = GoPlayConstantValues.S_ROLE_ACCESS_ADMIN_GAME_PAGE)]
        public ActionResult Index()
        {
            var games = Helpers.GameHelper.GetGamesForAdminUser(CurrentUser);
            return View(games);
        }

        [Route("{id}")]
        [RBAC(AccessAction = GoPlayConstantValues.S_ROLE_ACCESS_ADMIN_GAME_PAGE)]
        public ActionResult GameDetail(int id)
        {
            var api = GoPlayApi.Instance;
            var game = Helpers.GameHelper.GetGameForAdminUser(CurrentUser, id);
            if (game == null)
            {
                this.Flash(string.Format("Game with id {0} doesn't exist", id), FlashLevel.Warning);
                return Redirect("/admin/game");
            }
            if (!game.studio_id.HasValue || !PermissionHelper.HasViewStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id.Value))
            {
                return new HttpStatusCodeResult(403);
            }
            GameIndexViewModel gameModel = new GameIndexViewModel()
            {
                game = game,
                Genre = new LocaleInfo() { Text = "Genre", Name = "Genre", Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.genre) },
                Description = new LocaleInfo() { Text = "Description", Name = "Description", Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.description) },
                Short_description = new LocaleInfo() { Text = "Short Description", Name = "Short_description", Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.short_description) },
                Current_changelog = new LocaleInfo() { Text = "Current Changelog", Name = "Current_changelog", Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.current_changelog) },
                Content_rating = new LocaleInfo() { Text = "Content Rating", Name = "Content_rating", Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.content_rating) },
                Warning = new LocaleInfo() { Text = "Warning", Name = "Warning", Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.warning) },
                Platforms = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.download_links),
            };
            return View(gameModel);
        }

        [HttpGet]
        [Route("add")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
        public ActionResult GameAdd()
        {
            var api = GoPlayApi.Instance;
            var studios = Helpers.GameHelper.GetStudiosForAdminUser(CurrentUser);
            Dictionary<string, string> lang_locale = new Dictionary<string, string>();
            foreach (var item in CultureHelper._languges)
            {
                lang_locale.Add(item.Key, "");
            }
            GameAdminViewModel gameModel = new GameAdminViewModel()
            {
                Genre = new LocaleInfo() { Name = "Genre", Text = "Genre", Locale = lang_locale },
                Description = new LocaleInfo() { Name = "Description", Text = "Description", Locale = lang_locale },
                Short_description = new LocaleInfo() { Name = "Short_description", Text = "Short Description", Locale = lang_locale },
                Current_changelog = new LocaleInfo() { Name = "Current_changelog", Text = "What's new in latest version?", Locale = lang_locale },
                Content_rating = new LocaleInfo() { Name = "Content_rating", Text = "Content Rating", Locale = lang_locale },
                Warning = new LocaleInfo() { Name = "Warning", Text = "Warning", Locale = lang_locale },
                studios = studios
            };
            gameModel.HasRoleAdmin = CurrentUser.GetRoles().Any(x => x.RoleName == GoPlayConstantValues.S_ROLE_ADMIN);
            gameModel.action = "add";
            return View(gameModel);
        }

        [HttpPost]
        [Route("add")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
        public ActionResult GameAdd(GameAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var game = GetGameFromViewModel(model);
                int game_id = GoPlayApi.Instance.CreateGame(game);
                if (game_id > 0)
                {
                    this.Flash("Successfully added game " + game.name, FlashLevel.Success);
                    return Redirect("/admin/game/" + game_id);
                }
                else
                {
                    this.Flash("Failure added game" + game.name, FlashLevel.Error);
                }
            }
            model.studios = Helpers.GameHelper.GetStudiosForAdminUser(CurrentUser);
            return View(model);
        }

        [HttpGet]
        [Route("{id}/edit")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
        public ActionResult GameEdit(int id)
        {
            var api = GoPlayApi.Instance;
            var game = GameHelper.GetGameForAdminUser(CurrentUser, id);
            if (game == null)
            {
                this.Flash(string.Format("Game with id {0} doesn't exist", id), FlashLevel.Warning);
                return Redirect("/admin/game");
            }
            if (!game.studio_id.HasValue || !PermissionHelper.HasViewStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id.Value))
            {
                return new HttpStatusCodeResult(403);
            }
            var studios = Helpers.GameHelper.GetStudiosForAdminUser(CurrentUser);

            GameAdminViewModel gameModel = new GameAdminViewModel()
            {
                game_id = game.id,
                name = game.name,
                studio_id = game.studio_id,
                is_active = game.is_active == true ? "on" : string.Empty,
                is_featured = game.is_featured == true ? "on" : string.Empty,
                is_popular = game.is_popular == true ? "on" : string.Empty,
                released_at = game.released_at,
                current_version = game.current_version,
                file_size = game.file_size,
                endpoint = game.endpoint,
                gtoken_client_id = game.gtoken_client_id,
                gtoken_client_secret = game.gtoken_client_secret,
                game_invite_protocol = game.game_invite_protocol,
                studios = studios
            };

            if (!string.IsNullOrEmpty(game.youtube_links))
            {
                var links = JsonHelper.DeserializeObject<List<YoutubeLink>>(game.youtube_links);
                if (links != null)
                    gameModel.youtubeLinks = string.Join(",", links.Select(l => l.link));
            }

            Dictionary<string, string> Platforms = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.download_links);
            if (Platforms.ContainsKey("apple"))
                gameModel.iosDownloadLink = Platforms["apple"];
            if (Platforms.ContainsKey("google"))
                gameModel.androidDownloadLink = Platforms["google"];
            if (Platforms.ContainsKey("apk"))
                gameModel.apkDownloadLink = Platforms["apk"];
            if (Platforms.ContainsKey("pc"))
                gameModel.pcDownloadLink = Platforms["pc"];

            gameModel.Genre = new LocaleInfo()
            {
                Name = "Genre",
                Text = "Genre",
                Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.genre)
            };
            gameModel.Description = new LocaleInfo()
            {
                Name = "Description",
                Text = "Description",
                Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.description)
            };
            gameModel.Short_description = new LocaleInfo()
            {
                Name = "Short_description",
                Text = "Short Description",
                Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.short_description)
            };
            gameModel.Current_changelog = new LocaleInfo()
            {
                Name = "Current_changelog",
                Text = "What's new in latest version?",
                Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.current_changelog)
            };
            gameModel.Content_rating = new LocaleInfo()
            {
                Name = "Content_rating",
                Text = "Content rating (Everyone, Teens, etc.)",
                Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.content_rating)
            };
            gameModel.Warning = new LocaleInfo()
            {
                Name = "Warning",
                Text = "Warning",
                Locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(game.warning)
            };
            gameModel.HasRoleAdmin = CurrentUser.GetRoles().Any(x => x.RoleName == GoPlayConstantValues.S_ROLE_ADMIN);
            gameModel.action = "edit";
            gameModel.previous_page = Request.Params["previous_page"];
            return View("GameAdd", gameModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("{id}/edit")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
        public ActionResult GameEdit(GameAdminViewModel model)
        {
            ModelState.Remove("icon");
            ModelState.Remove("thumb");
            ModelState.Remove("banner");
            if (ModelState.IsValid)
            {
                var game = GetGameFromViewModel(model);
                game.id = model.game_id.Value;
                game.is_active = true;
                if (GoPlayApi.Instance.UpdateGame(game))
                {
                    this.Flash("Successfully updated game " + game.name, FlashLevel.Success);
                    return Redirect("/admin/game/" + game.id);
                }
                else
                {
                    this.Flash("Failure updated game" + game.name, FlashLevel.Error);
                }
            }
            model.studios = Helpers.GameHelper.GetStudiosForAdminUser(CurrentUser);
            return View("GameAdd", model);
        }

        [HttpPost]
        [Route("delete")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
        public ActionResult GameDelete()
        {
            this.Flash("Delete not supported yet", FlashLevel.Warning);
            return Redirect("/admin/game");
        }
        private Game GetGameFromViewModel(GameAdminViewModel model)
        {
            var api = GoPlayApi.Instance;
            string path = HttpContext.Server.MapPath(ConfigurationManager.AppSettings["UPLOADS_DIR"]);
            Game game = new Game()
            {
                guid = Guid.NewGuid().ToString(),
                name = model.name,
                studio_id = model.studio_id,
                is_active = (!string.IsNullOrEmpty(model.is_active) && model.is_active == "on"),
                is_featured = (!string.IsNullOrEmpty(model.is_featured) && model.is_featured == "on"),
                is_popular = (!string.IsNullOrEmpty(model.is_popular) && model.is_popular == "on"),
                released_at = model.released_at,
                current_version = model.current_version,
                file_size = model.file_size,
                endpoint = model.endpoint,
                gtoken_client_id = model.gtoken_client_id,
                gtoken_client_secret = model.gtoken_client_secret,
                game_invite_protocol = model.game_invite_protocol
            };
            if (model.sliderImages != null)
            {
                int index = 0;
                List<Dictionary<string, string>> imageList = new List<Dictionary<string, string>>();
                foreach (var item in model.sliderImages)
                {
                    if (item == null || !item.FileName.Contains('.'))
                        break;
                    string imageFile = api.HandleFile(HttpContext.Server.MapPath("~"), item.InputStream, path, item.FileName);
                    Dictionary<string, string> img = new Dictionary<string, string>();
                    img.Add("filename", imageFile);
                    img.Add("index", (index++).ToString());
                    imageList.Add(img);
                }
                if (imageList.Count > 0)
                    game.slider_images = "{\"images\":" + JsonConvert.SerializeObject(imageList) + "}";
            }

            //TODO: Missing line 225 in new code 3/9, apply for update game only

            Dictionary<string, string> download_links = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(model.iosDownloadLink))
                download_links.Add("apple", model.iosDownloadLink);
            if (!string.IsNullOrEmpty(model.androidDownloadLink))
                download_links.Add("google", model.androidDownloadLink);
            if (!string.IsNullOrEmpty(model.apkDownloadLink))
                download_links.Add("apk", model.apkDownloadLink);
            if (!string.IsNullOrEmpty(model.pcDownloadLink))
                download_links.Add("pc", model.pcDownloadLink);
            game.download_links = JsonConvert.SerializeObject(download_links);

            game.youtube_links = GameHelper.SetYoutubeLinks(model.youtubeLinks);

            game.description = JsonConvert.SerializeObject(model.Description.Locale);
            game.short_description = JsonConvert.SerializeObject(model.Short_description.Locale);
            game.genre = JsonConvert.SerializeObject(model.Genre.Locale);
            game.current_changelog = JsonConvert.SerializeObject(model.Current_changelog.Locale);
            game.content_rating = JsonConvert.SerializeObject(model.Content_rating.Locale);
            game.warning = JsonConvert.SerializeObject(model.Warning.Locale);

            if (model.icon != null)
                game.icon_filename = api.HandleFile(HttpContext.Server.MapPath("~"), model.icon.InputStream, path, model.icon.FileName);
            if (model.thumb != null)
                game.thumb_filename = api.HandleFile(HttpContext.Server.MapPath("~"), model.thumb.InputStream, path, model.thumb.FileName);
            if (model.banner != null)
                game.banner_filename = api.HandleFile(HttpContext.Server.MapPath("~"), model.banner.InputStream, path, model.banner.FileName);
            return game;
        }
    }
}