using GToken.Models;
using GToken.Web.Controllers;
using GToken.Web.Models;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GToken.Controllers
{
    [Authorize]
    public class ReferralController : BaseController
    {
        // GET: Referral
        [Route("referral/index")]
        public ActionResult Index(SearchReferalViewModel model)
        {
            int pagesize = 10;
            model.page = model.page ?? 1;
            var api = Platform.Core.Api.Instance;

            int fromIndex = (model.page.Value - 1) * pagesize;

            var rewards = api.GetRecordDownloadHistory(null, CurrentUser.UserName, model.game_id, model.username, model.start_date.HasValue ? model.start_date.Value.ToString() : null,
                model.end_date.HasValue ? model.end_date.Value.ToString() : null, fromIndex, pagesize);

            ReferalsPaging transPaging = new ReferalsPaging();
            transPaging.totalMoney = api.GetTotalReferralMoney(CurrentUser.UserName);
            transPaging.transactions = rewards.Data;
            //if (model.start_date.HasValue)
            //{
            //    model.start_date = model.start_date.Value.AddMinutes(model.time_zone.Value);
            //}
            //if (model.end_date.HasValue)
            //{
            //    model.end_date = model.end_date.Value.AddMinutes(model.time_zone.Value);
            //}
            transPaging.count = api.CountRecordDownloadHistory(null, CurrentUser.UserName, model.game_id, model.username, model.start_date.HasValue ? model.start_date.Value.ToString() : "",
                model.end_date.HasValue ? model.end_date.Value.ToString() : "", fromIndex, pagesize);
            transPaging.pagination = new GToken.Helpers.Extensions.Pagination(model.page.Value, pagesize, transPaging.count);

            transPaging.model = model;

            model.games = getGameList();

            return View(transPaging);
        }

        private List<ReferralGame> getGameList()
        {
            if (Session["referalGames"] != null)
            {
                return (List<ReferralGame>)Session["referalGames"];
            }
            else
            {
                var api = Platform.Core.Api.Instance;
                var games = api.GetReferralGames();
                if (games.HasData)
                {
                    Session["referalGames"] = games.Data;
                    return games.Data;
                }
                return new List<ReferralGame>();
            }
        }
    }
}