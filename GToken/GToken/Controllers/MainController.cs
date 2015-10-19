using GToken.Web.Controllers;
using GToken.Web.Helpers;
using GToken.Web.Models;
using Newtonsoft.Json;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GToken.Web.Controllers
{
    public class MainController : BaseController
    {
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }
        [Route("about")]
        public ActionResult About()
        {
            return View();
        }
        [Route("clubs")]
        public ActionResult Clubs()
        {
            return View();
        }
        [Route("news")]
        public ActionResult News()
        {
            return View();
        }
        [Route("Partners")]
        public ActionResult Partners()
        {
            return View();
        }

        [Route("partner-detail/{partner}")]
        public ActionResult PartnerDetail(string partner)
        {
            return View();
        }

        [Route("Support")]
        public ActionResult Support()
        {
            return View();
        }
        [Route("Terms")]
        public ActionResult Terms()
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

        [HttpPost]
        [Route("index.aspx")]
        public async Task<object> ApiProxy()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["API-PROXY-HOST"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string action = @"api";
                var formContent = new FormUrlEncodedContent(ToDictionary<string, string>(Request.Params));
                HttpResponseMessage response = await client.PostAsync(action, formContent);
                return JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());
            }

        }

        [Route("campaigns")]
        public ActionResult Deals()
        {
            var gtokenApi = Platform.Core.Api.Instance;
            var currentCamps = gtokenApi.GetCurrentReferralCampaigns(new List<int> { (int)ReferralCampaignStatus.Running });
            var result = new DealsViewModel();
            if (currentCamps.HasData && currentCamps.Data.Any())
            {
                result = getGameLinks(currentCamps.Data, result);
                result.currentCamps = currentCamps.Data;
            }

            var inCommingCamp = gtokenApi.GetInCommingReferralCampaigns(new List<int> { (int)ReferralCampaignStatus.Active });
            if (inCommingCamp.HasData && inCommingCamp.Data.Any())
            {
                result = getGameLinks(inCommingCamp.Data, result);
                result.inCommingCamp = inCommingCamp.Data;
            }

            return View(result);
        }

        private DealsViewModel getGameLinks(List<ReferralCampaign> camps, DealsViewModel result)
        {
            var goplayApi = GoPlay.Core.GoPlayApi.Instance;
            foreach (var camp in camps)
            {
                var game = goplayApi.GetGame(camp.game_id);
                if (game.HasData)
                {
                    var goplayHost = ConfigurationManager.AppSettings["GOPLAY-HOST"];
                    if (!game.Data.thumb_filename.StartsWith("https://"))
                        result.image_covers.Add(new DealsGameModel { game_id = camp.game_id, links = goplayHost + (game.Data.thumb_filename.StartsWith("/") ? game.Data.thumb_filename.Substring(1) : game.Data.thumb_filename) });
                    else
                        result.image_covers.Add(new DealsGameModel { game_id = camp.game_id, links = game.Data.thumb_filename });

                    result.game_links.Add(new DealsGameModel { game_id = camp.game_id, links = GameHelper.GenerateLinkGame(game.Data, goplayHost) });
                    var icon = String.Empty;
                    if (!game.Data.icon_filename.StartsWith("https://"))
                        icon = goplayHost + (game.Data.icon_filename.StartsWith("/") ? game.Data.icon_filename.Substring(1) : game.Data.icon_filename);
                    else
                        icon = game.Data.icon_filename;
                    if (!result.icon_filenames.ContainsKey(camp.game_id))
                    {
                        result.icon_filenames.Add(camp.game_id, icon);
                    }
                }
            }
            return result;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(NameValueCollection col)
        {
            var dict = new Dictionary<TKey, TValue>();
            var keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
            var valueConverter = TypeDescriptor.GetConverter(typeof(TValue));

            foreach (string name in col)
            {
                TKey key = (TKey)keyConverter.ConvertFromString(name);
                TValue value = (TValue)valueConverter.ConvertFromString(col[name]);
                dict.Add(key, value);
            }

            return dict;
        }
    }
}