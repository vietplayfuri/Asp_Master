using GoPlay.Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Platform.Utility;
using GoPlay.Web.Const;
using Newtonsoft.Json.Linq;
using GoPlay.Web.Helpers.Extensions;
using GoPlay.Models;
using GoPlay.Core;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Models;
using Platform.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GoPlay.Web.Helpers
{
    public class GameHelper
    {
        public static List<string> LOCALIZED_FIELDS = new List<string>(){
        "description",
        "short_description",
        "genre",
        "current_changelog",
        "content_rating",
        "warning"
         };

        public static List<Game> getCommingGames(ApplicationUser currentUser, IPAddress ip, int limit = 0)
        {
            var api = GoPlayApi.Instance;
            var result = api.GetGames(true, false);
            var commingGames = result.Data.Where(x => isComingSoon(x, currentUser, ip)).ToList();
            if (limit == 0) { return commingGames; }
            return commingGames.Take(limit).ToList();

        }

        public static JObject getDownloadLinksForCurrentUser(Game game, ApplicationUser currentUser, IPAddress ip)
        {
            var countryCode = String.Empty;
            if (currentUser != null)
            {
                countryCode = currentUser.country_code;
            }
            else
            {
                string country_code = null;
                string country_name = null;
                if (ip.GetCountryCode(c => country_code = c, n => country_name = n))
                {
                    countryCode = country_code;
                }
            }

            var downloadLink = JObject.Parse(game.download_links);
            var jsonLink = JObject.Parse(game.download_links);
            if (Local.NO_GAME_STORES_COUNTRY_CODES.Contains(countryCode.ToLower()))
            {
                if (jsonLink["google"] != null)
                {
                    downloadLink.Remove("google");
                }
            }
            else
            {
                if (game.id != 21 && game.id != 24)
                {
                    if (jsonLink["apk"] != null)
                    {
                        downloadLink.Remove("apk");
                    }
                }
            }
            return downloadLink;
        }

        public static bool isComingSoon(Game game, ApplicationUser currentUser, IPAddress ip)
        {
            return game.is_active && (getDownloadLinksForCurrentUser(game, currentUser, ip).Count == 0);
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static string getLocalized(Game game, string attr)
        {
            if (!LOCALIZED_FIELDS.Contains(attr))
            {
                return "";
            }
            string localizedAttr = GetPropValue(game, attr).ToString();
            if (localizedAttr != null)
            {
                if (JObject.Parse(localizedAttr)[CultureHelper.GetCurrentNeutralCulture()] != null)
                {
                    string localizedValue = JObject.Parse(localizedAttr)[CultureHelper.GetCurrentNeutralCulture()].ToString();
                    if (!string.IsNullOrEmpty(localizedValue))
                        return localizedValue;
                }
            }
            localizedAttr = JObject.Parse(localizedAttr)["en"].ToString();
            return localizedAttr;
        }

        public static string getThumbFilename(Game game)
        {
            if (!String.IsNullOrEmpty(game.thumb_filename))
            {
                return game.thumb_filename;
            }
            return GoPlay.Web.Const.Common.DEFAULT_GAME_THUMBNAIL_URL;
        }

        public static string generateParam(Game game)
        {
            return String.Format("{0}-{1}", game.name.GenerateSlug(), game.id);
        }

        public static List<Game> GetGamesForCurrentUser(ApplicationUser currentUser, SearchCondition searchConditions = null)
        {
            var api = GoPlayApi.Instance;

            var games = api.GetGames(searchConditions);
            if (!games.HasData && searchConditions == null)
            {
                List<string> roles = new List<string>();
                if (currentUser != null)
                {
                    roles = currentUser.GetRoles().Select(r => r.RoleName).ToList();
                }
                if (!roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ADMIN) && !roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ACCOUNTANT))
                {
                    return api.GetAllGames().Data.Where(x => x.is_active && x.is_archived == false).OrderBy(x => x.id).ToList();
                }
                return api.GetAllGames().Data.Where(x => x.is_archived == false).OrderBy(x => x.id).ToList();
            }
            return games.Data;
        }

        public static List<Game> FilterGamesByCondition(List<Game> games, SearchCondition searchConditions, int page, int pagesize, out int totalRows)
        {
            int fromIndex = (page - 1) * pagesize;
            int toIndex = page * pagesize;
            // if (searchConditions == null)
            //  {
            totalRows = games.Count();
            return games.OrderBy(x => x.id).
                        Skip(fromIndex).
                        Take(toIndex - fromIndex).
                        ToList();
            // }

            //string genre = searchConditions.genre;
            //string keyword = searchConditions.keywork;
            //string platform = searchConditions.platform;
            //string release = searchConditions.release;

            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    games = games.Where(g => g.name.ToLower().Contains(keyword)).ToList();
            //}

            //if (genre != "all" && genre != "default")
            //{
            //    games = games.Where(g => g.genre.Contains(genre)).ToList();
            //}

            //if (platform == "ios")
            //{
            //    games = games.Where(g => g.download_links.Contains("apple")).ToList();
            //}
            //else if (platform == "android")
            //{
            //    games = games.Where(g => g.download_links.Contains("google")).ToList();
            //}
            //else if (platform == "pc")
            //{
            //    games = games.Where(g => g.download_links.Contains("pc")).ToList();
            //}
            //else if (platform == "apk")
            //{
            //    games = games.Where(g => g.download_links.Contains("apk")).ToList();
            //}

            //if (release == "coming")
            //{
            //    games = games.Where(g => g.download_links.Contains("{}")).ToList();
            //}

            //totalRows = games.Count();
            //return games.OrderBy(x => x.id).
            //                Skip(fromIndex).
            //                Take(toIndex - fromIndex).
            //                ToList();
        }

        public static List<Game> getGamesPerPageForCurrentUser(int page, int pagesize, ApplicationUser currentUser, List<Game> games = null, SearchCondition searchConditions = null)
        {
            int fromIndex = (page - 1) * pagesize;
            int toIndex = page * pagesize;

            var api = GoPlayApi.Instance;
            if (games == null)
            {
                games = api.GetGames(searchConditions).Data;
            }

            if (games == null && searchConditions == null)
            {
                if (currentUser != null)
                {
                    List<string> roles = currentUser.GetRoles().Select(r => r.RoleName).ToList();
                    if (!roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ADMIN) && !roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ACCOUNTANT))
                    {
                        return api.GetAllGames().Data.Where(x => x.is_active && x.is_archived == false)
                            .OrderBy(x => x.id)
                            .Skip(fromIndex)
                            .Take(toIndex - fromIndex)
                            .ToList();
                    }
                }
                return api.GetAllGames().Data.Where(x => x.is_archived == false).OrderBy(x => x.id)
                    .Skip(fromIndex)
                    .Take(toIndex - fromIndex)
                    .ToList();
            }
            return games.Skip(fromIndex).Take(toIndex - fromIndex).ToList();
        }

        public static List<string> getLocalizedField(List<Game> gamesForCurrentUser, string attr)
        {
            return gamesForCurrentUser.Where(m => getLocalized(m, attr) != "").Select(x => getLocalized(x, attr)).Distinct().ToList();
        }

        public static List<Game> GetGamesForAdminUser(ApplicationUser currentUser)
        {
            var api = GoPlayApi.Instance;
            List<string> roles = currentUser.GetRoles().Select(r => r.RoleName).ToList();
            if (roles.Contains(GoPlayConstantValues.S_ROLE_ADMIN))
                return api.GetAllGames().Data.OrderBy(x => x.name).ToList();
            if (roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ADMIN) || roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ACCOUNTANT))
                return api.GetGamesForAdmin(currentUser.Id, false).Data;

            return api.GetAllGames().Data.Where(x => x.is_archived == false && x.is_active == true).OrderBy(x => x.name).ToList();
        }
        public static Game GetGameForAdminUser(ApplicationUser currentUser, int game_id)
        {
            var api = GoPlayApi.Instance;
            List<string> roles = currentUser.GetRoles().Select(r => r.RoleName).ToList();
            if (roles.Contains(GoPlayConstantValues.S_ROLE_ADMIN))
                return api.GetGame(game_id).Data;

            return GetGameForCurrentUser(currentUser, game_id);
        }

        public static Game GetGameForCurrentUser(ApplicationUser currentUser, int game_id)
        {
            var api = GoPlayApi.Instance;
            List<string> roles = currentUser.GetRoles().Select(r => r.RoleName).ToList();
            if (!roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ADMIN) && !roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ACCOUNTANT))
            {
                return api.GetGames(true, false).Data.FirstOrDefault(x => x.id == game_id);
            }

            return api.GetAllGames().Data.FirstOrDefault(x => x.is_archived == false && x.id == game_id);
        }

        public static List<Studio> GetStudiosForAdminUser(ApplicationUser currentUser)
        {
            var api = GoPlayApi.Instance;
            List<string> roles = currentUser.GetRoles().Select(r => r.RoleName).ToList();
            if (roles.Contains(GoPlayConstantValues.S_ROLE_ADMIN))
                return api.GetAllStudios().Data;
            if (roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ADMIN))
                return api.GetStudios(currentUser.Id).Data;
            return null;
        }

        public static string GenerateLinkGame(Game game, string originalUrl = null)
        {
            string urlGameDetail = @"game/detail/{0}";
            string outputUrl = string.Format(
                (!string.IsNullOrEmpty(originalUrl) ? originalUrl : "/") + urlGameDetail,
                (game.name + "-" + game.id).GenerateSlug());
            return outputUrl;
        }

        public static string SetYoutubeLinks(string youtubeLinks)
        {
            if (string.IsNullOrEmpty(youtubeLinks)) return "[]";

            List<YoutubeLink> lstLinks = new List<YoutubeLink>();
            foreach (var link in youtubeLinks.Split(',').ToList())
            {
                if (!string.IsNullOrEmpty(link))
                {
                    var videoId = Regex.Match(link.Trim(), @"(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+").Groups[1].Value;
                    if (!string.IsNullOrEmpty(videoId))
                        lstLinks.Add(new YoutubeLink
                        {
                            id = videoId,
                            link = link.Trim(),
                            thumbnail = string.Format("http://i.ytimg.com/vi/{0}/0.jpg", videoId),
                            embedded_player = string.Format("https://www.youtube.com/embed/{0}", videoId)
                        });
                }
            }
            if (lstLinks.Any())
                return JsonConvert.SerializeObject(lstLinks);
            return "[]";
        }


        public static JObject CustomJObject(JObject jobject)
        {
            if (jobject["profile"] != null)
            {
                if (jobject["profile"]["goplay_token"] != null)
                    jobject["profile"]["goplay_token"].Parent.Remove();
                if (jobject["profile"]["free_goplay_token"] != null)
                    jobject["profile"]["free_goplay_token"].Parent.Remove();
                if (jobject["profile"]["goplay_token_value"] != null)
                    jobject["profile"]["goplay_token_value"].Parent.Remove();
            }

            if (jobject["exchanges"] != null)
            {
                foreach (var item in jobject["exchanges"].Children())
                {
                    if (item["goplay_token"] != null)
                        item["goplay_token"].Parent.Remove();
                    if (item["free_goplay_token"] != null)
                        item["free_goplay_token"].Parent.Remove();
                    if (item["goplay_token_value"] != null)
                        item["goplay_token_value"].Parent.Remove();
                }
            }

            return jobject;
        }
    }
}