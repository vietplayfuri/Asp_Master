using System;
using System.Collections.Generic;
using System.Net;
using GoEat.Web.Const;
using GoEat.Web.Helpers.Extensions;
using GoEat.Web.Identity;
using GoEat.Models;
using GoEat.Utility;
using Newtonsoft.Json.Linq;

namespace GoEat.Web.Helpers
{
    public class GameHelper
    {
        public List<string> LOCALIZED_FIELDS = new List<string>
        {
        "description",
        "short_description",
        "genre",
        "current_changelog",
        "content_rating"
     };
        //public JObject getDownloadLinksForCurrentUser(Game game, ApplicationUser currentUser, IPAddress ip)
        //{
        //    var countryCode = String.Empty;
        //    if (currentUser != null)
        //    {
        //        countryCode = currentUser.country_code;
        //    }
        //    else
        //    {
        //        string country_code = null;
        //        string country_name = null;
        //        ip = IPAddress.Parse("118.200.154.246");//For testing in localhost
        //        if (ip.GetCountryCode(c => country_code = c, n => country_name = n))
        //        {
        //            countryCode = country_code;
        //        }
        //    }

        //    var downloadLink = JObject.Parse(game.download_links);
        //    var jsonLink = JObject.Parse(game.download_links);
        //    if (Local.NO_GAME_STORES_COUNTRY_CODES.Contains(countryCode.ToLower()))
        //    {
        //        if (jsonLink["google"] != null)
        //        {
        //            downloadLink.Remove("google");
        //        }
        //    }
        //    else
        //    {
        //        if (game.id == 21 || game.id == 24)
        //        {
        //            if (jsonLink["apk"] != null)
        //            {
        //                downloadLink.Remove("apk");
        //            }
        //        }
        //    }
        //    return downloadLink;
        //}

        //public bool isComingSoon(Game game, ApplicationUser currentUser, IPAddress ip)
        //{
        //    return game.is_active && (getDownloadLinksForCurrentUser(game, currentUser, ip).Count == 0);
        //}

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        //public string getLocalized(Game game, string attr)
        //{
        //    if (!LOCALIZED_FIELDS.Contains(attr))
        //    {
        //        return "";
        //    }
        //    string localizedAttr = GetPropValue(game, attr).ToString();
        //    if (localizedAttr != null)
        //    {
        //        if (JObject.Parse(localizedAttr)[CultureHelper.GetCurrentNeutralCulture()] != null)
        //        {
        //            localizedAttr = JObject.Parse(localizedAttr)[CultureHelper.GetCurrentNeutralCulture()].ToString();
        //            return localizedAttr;
        //        }
        //    }
        //    localizedAttr = JObject.Parse(localizedAttr)["en"].ToString();
        //    return localizedAttr;
        //}

        //public string getThumbFilename(Game game)
        //{
        //    if (!String.IsNullOrEmpty(game.thumb_filename))
        //    {
        //        return game.thumb_filename;
        //    }
        //    return Common.DEFAULT_GAME_THUMBNAIL_URL;
        //}

        //public string generateParam(Game game)
        //{
        //    return String.Format("{0}-{1}", game.name.GenerateSlug(), game.id);
        //}
    }
}