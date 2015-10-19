using AutoMapper;
using GToken.Areas.Admin.Models;
using GToken.Web.Identity;
using GToken.Models;
using GToken.Web.ActionFilter;
using Platform.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace GToken.Areas.Admin.Controllers
{
    [Authorize]
    [RBAC(AccessAction = "access_admin_page")]
    [RouteArea("admin")]
    [RoutePrefix("referral")]
    public class ReferralController : Controller
    {
        private IMappingEngine _mapper;
        public ReferralController(IMappingEngine mapper)
        {
            _mapper = mapper;
        }

        // GET: Admin/Referral
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("campaignlist")]
        public ActionResult CampaignList()
        {
            var api = Platform.Core.Api.Instance;

            ViewBag.ReferralCampaign = api.GetReferralCampaigns().Data;
            return View();
        }

        [Route("create")]
        public ActionResult Create()
        {
            var api = Platform.Core.Api.Instance;
            var model = new CreateReferralViewModel();
            var games = api.GetReferralGames();
            if (games.HasData) model.games = games.Data;
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [Route("create")]
        public ActionResult CreateReferral(CreateReferralViewModel model)
        {
            var api = Platform.Core.Api.Instance;
            if (!ModelState.IsValid)
            {
                model.games = getGameList();
                return View("Create", model);
            }
            var referrer = this._mapper.Map<ReferralCampaign>(model);
            var game = api.GetReferralGames().Data.Where(x => x.id == model.game_id).FirstOrDefault();
            if (game != null)
                referrer.game_name = game.name;
            else
                return RedirectToAction("CampaignList");
            referrer.status = (int)ReferralCampaignStatus.Active;

            if (!referrer.is_display_only)
            {
                string validate = ValidateListCampaigns(model.game_id.Value, model.start_date, model.end_date, 0);
                if (!string.IsNullOrEmpty(validate))
                {
                    model.games = getGameList();
                    ViewBag.Error = validate;
                    return View("Create", model);
                }
            }

            api.CreateReferralCampaign(referrer);

            return RedirectToAction("CampaignList");
        }


        private string ValidateListCampaigns(int gameId, DateTime? start_date, DateTime? end_date, int currentCampId)
        {
            var api = Platform.Core.Api.Instance;
            var lastCampaign = api.GetListValidCompaignOfGame(gameId);
            if (lastCampaign.HasData)
            {
                foreach (var camp in lastCampaign.Data)
                {
                    if (camp.id == currentCampId) continue;
                    TimeSpan span_start = start_date.Value.Subtract(camp.end_date);
                    TimeSpan span_end = end_date.Value.Subtract(camp.start_date);
                    if ((span_start.TotalHours <= 0 && span_end.TotalHours >= 0) ||
                        (span_start.TotalHours >= 0 && span_end.TotalHours <= 0))
                    {
                        return "There is existing campaign for this game.";
                    }
                }
            }
            return string.Empty;
        }


        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var api = Platform.Core.Api.Instance;
            var campaign = api.GetReferralCampaignById(id);
            if (!campaign.HasData)
            {
                return RedirectToAction("CampaignList");
            }

            var model = this._mapper.Map<EditReferralViewModel>(campaign.Data);
            var games = api.GetReferralGames();
            if (games.HasData) model.games = games.Data;

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [Route("edit/{id}")]
        public ActionResult EditReferral(EditReferralViewModel model)
        {
            var api = Platform.Core.Api.Instance;
            if (!ModelState.IsValid)
            {
                model.games = getGameList();
                return View("edit", model);
            }

            var currentCamp = api.GetReferralCampaignById(model.id);
            if (!currentCamp.HasData)
            {
                return RedirectToAction("CampaignList");
            }

            /*
                Edit status rules
                Active ==> InActive
                Running ==> InActive
                Finished ==> No
                InActive ==> Active
            */
            var rules = new Dictionary<int, List<int>>();
            rules.Add((int)ReferralCampaignStatus.Active, new List<int>() { (int)ReferralCampaignStatus.Inactive });
            rules.Add((int)ReferralCampaignStatus.Running, new List<int>() { (int)ReferralCampaignStatus.Inactive });
            rules.Add((int)ReferralCampaignStatus.Finished, new List<int>() { });
            rules.Add((int)ReferralCampaignStatus.Inactive, new List<int>() { (int)ReferralCampaignStatus.Active });

            if (currentCamp.Data.status == (int)ReferralCampaignStatus.Running)
            {
                if (model.status != currentCamp.Data.status)
                {
                    var status = rules[currentCamp.Data.status];
                    if (status.Contains(model.status.Value))
                    {
                        currentCamp.Data.status = model.status.Value;
                    }
                    else
                    {
                        model.games = getGameList();
                        ViewBag.Error = "Invalid status. This campaign is running.";
                        return View("edit", model);
                    }
                }

                //Admin change to display only when campaign running --> the campaign won't effect any more
                if (currentCamp.Data.status == (int)ReferralCampaignStatus.Running && string.Compare(model.is_display_only, "on", true) == 0)
                {
                    model.games = getGameList();
                    ViewBag.Error = "This campaign is running. Please stop it before change it to display only.";
                    return View("edit", model);
                }

                currentCamp.Data.title = model.title;
                currentCamp.Data.description = model.description;
                currentCamp.Data.is_display_only = string.Compare(model.is_display_only, "on", true) == 0 ? true : false;
                currentCamp.Data.is_override = string.Compare(model.is_override, "on", true) == 0 ? true : false;
                currentCamp.Data.override_value = model.override_value;
                api.UpdateReferralCampaigns(currentCamp.Data);
            }
            else
            {
                if (model.status != currentCamp.Data.status)
                {
                    var status = rules[currentCamp.Data.status];
                    if (!status.Contains(model.status.Value))
                    {
                        model.games = getGameList();
                        ViewBag.Error = "Invalid status";
                        return View("edit", model);
                    }
                }

                if (string.Compare(model.is_display_only, "on", true) != 0)
                {
                    string validate = ValidateListCampaigns(model.game_id.Value, model.start_date, model.end_date, currentCamp.Data.id);
                    if (!string.IsNullOrEmpty(validate))
                    {
                        model.games = getGameList();
                        ViewBag.Error = validate;
                        return View("edit", model);
                    }
                }

                var countReferralHistory = api.CountRecordDownloadHistory(model.id);
                if (model.quantity < countReferralHistory)
                {
                    model.games = getGameList();
                    ViewBag.Error = "Quantity is invalid. Current number users are " + countReferralHistory;
                    return View("edit", model);
                }
                currentCamp.Data.status = model.status.Value;
                currentCamp.Data.quantity = model.quantity.Value;
                currentCamp.Data.start_date = model.start_date.Value;
                currentCamp.Data.end_date = model.end_date.Value;
                var game = api.GetReferralGames().Data.Where(x => x.id == model.game_id).FirstOrDefault();
                if (game != null)
                {
                    currentCamp.Data.game_name = game.name;
                    currentCamp.Data.game_id = game.id;
                }
                currentCamp.Data.gtoken_per_download = model.gtoken_per_download.Value;
                currentCamp.Data.title = model.title;
                currentCamp.Data.description = model.description;

                currentCamp.Data.is_display_only = string.Compare(model.is_display_only, "on", true) == 0 ? true : false;
                currentCamp.Data.is_override = string.Compare(model.is_override, "on", true) == 0 ? true : false;
                currentCamp.Data.override_value = model.override_value;

                api.UpdateReferralCampaigns(currentCamp.Data);
            }

            return RedirectToAction("CampaignList");
        }

        [HttpPost]
        [Route("delete/{id}")]
        public JsonResult Delete(int id)
        {
            var api = Platform.Core.Api.Instance;
            var countReferralHistory = api.CountRecordDownloadHistory(id);
            var currentCamp = api.GetReferralCampaignById(id);
            if (countReferralHistory != 0 || (currentCamp.HasData && currentCamp.Data.status == (int)ReferralCampaignStatus.Running))
                return Json(new { status = false });
            if (api.DeleteReferralCampaign(id))
                return Json(new { status = true });
            return Json(new { status = false });

        }



        [HttpPost]
        [Route("order-referral-campaign")]
        public JsonResult OrderReferralCampaign(OrderReferralCampaign model)
        {
            if (model == null || !ModelState.IsValid) return Json(new { status = false });
            var api = Platform.Core.Api.Instance;
            return Json(new { status = api.OrderReferralCampaign(model.source_id, model.destination_id) });
        }

        [Route("downloadhistory")]
        public ActionResult DownloadHistory(DownloadHistoryViewModel model)
        {
            var api = Platform.Core.Api.Instance;
            List<RecordDownloadHistory> lstRecords = api.GetRecordDownloadHistory(model.timeZone, model.campaign_id, model.game_id, model.username, model.referrer, model.from_date, model.to_date).Data;
            ViewBag.RecordDownloadHistory = lstRecords;

            if (!string.IsNullOrEmpty(model.export))
            {
                StringWriter sw = new StringWriter();
                sw.WriteLine("Game,Username,Referrer,Device ID,Created At");
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=transactions_" + DateTime.Now.ToUniversalTime().ToString() + ".csv");
                Response.ContentType = "text/csv";
                if (lstRecords != null && lstRecords.Any())
                {
                    var est = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
                    foreach (var line in lstRecords)
                    {
                        DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(line.created_at, est);
                        sw.WriteLine(string.Format("{0},{1},{2},{3},{4}",
                                                   line.game_name,
                                                   line.username,
                                                   line.inviter_username,
                                                   line.device_id,
                                                   targetTime.ToString(ConstantValues.S_DATETIME_FORMAT)
                                                   ));
                    }
                }
                Response.Write(sw.ToString());
                Response.End();
            }
            model.games = getGameList();
            model.campaigns = api.GetReferralCampaigns().Data;
            return View(model);
        }

        [HttpGet]
        [Route("import-referral")]
        public ActionResult ImportReferral()
        {
            return View(new ImportReferalViewModel());
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


        [HttpPost]
        [Route("import-referral")]
        public ActionResult ImportReferral(ImportReferalViewModel model)
        {
            var api = Platform.Core.Api.Instance;
            var file = Request.Files["file"];
            var campaigns = api.GetCurrentReferralCampaigns(new List<int>() { (int)ReferralCampaignStatus.Running, (int)ReferralCampaignStatus.Finished }).Data;

            if (!ModelState.IsValid
                || file == null
                || file.InputStream.Length == 0
                || string.IsNullOrEmpty(file.FileName)
                || file.FileName.Length > 225
                || System.IO.Path.GetExtension(file.FileName) != ".csv")
            {
                if (campaigns != null) model.referral_campaigns = campaigns;
                model.errorMsg = new List<string> { "File is invalid" };
                return View(model);
            }

            int maximumMb = 0;
            if (!(int.TryParse(ConfigurationManager.AppSettings["MaxLengthImportReferralFile"], out maximumMb)))
                maximumMb = 5;
            int maxlength = maximumMb * 1024 * 1024;
            if (file.InputStream.Length > maxlength)
            {
                if (campaigns != null) model.referral_campaigns = campaigns;
                model.errorMsg = new List<string> { string.Format("Maximum file length exceeded: {0} MB", maximumMb) };
                return View(model);
            }

            string ip = Request.UserHostAddress == null || Request.UserHostAddress == "::1"
                    ? IPAdressExtension.GetDefaultIp()
                    : Request.UserHostAddress;
            string savedPath = HttpContext.Server.MapPath(Platform.Models.ConstantValues.S_REFERRAL_UPLOAD_DIR);
            int userId = HttpContext.User.Identity.GetUserId<int>();

            if (System.IO.File.Exists(savedPath + file.FileName) && api.CheckImportHistoryByFilename(file.FileName))
            {
                if (campaigns != null) model.referral_campaigns = campaigns;
                model.errorMsg = new List<string> { "Filename is existed" };
                return View(model);
            }

            ImportReferralHistory history = null;
            model.errorMsg = api.ImportReferral(HttpContext.Server.MapPath("~"), file.InputStream, savedPath, file.FileName, model.campaign_id, userId, ip, out history);

            if (campaigns != null) model.referral_campaigns = campaigns;
            model.ImportResult = history;
            return View(model);
        }

        [HttpPost]
        [Route("import-history-list")]
        public JsonResult GetImportHisotry(SearchImportViewModel model)
        {
            var api = Platform.Core.Api.Instance;
            List<ImportReferralHistory> listImportHistory = api.GetImportHistory(model.time_zone, model.game_id, model.start_date, model.end_date, null, null).Data;
            if (listImportHistory != null)
            {
                TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById(model.time_zone);
                return Json(new
                {
                    success = true,
                    importlist = listImportHistory.Select(x => new
                    {
                        x.game_name,
                        x.file_name,
                        file_path = "/admin/referral/download/import/" + x.id,
                        failed_path = (x.ImportResult != null && !String.IsNullOrEmpty(x.ImportResult.file_path))
                            ? "/admin/referral/download/fail/" + x.id
                            : string.Empty,
                        created_at = TimeZoneInfo.ConvertTimeFromUtc(x.created_at, est).ToString(),
                        x.campaign_name
                    }).ToList()
                });
            }
            return Json(new { success = false, importlist = new List<ImportReferralHistory>() });
        }

        [HttpGet]
        [Route("download/import/{id}")]
        public FileResult Download(int id)
        {
            var api = Platform.Core.Api.Instance;
            var result = api.GetImportHistoryById(id);
            if (result.HasData)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(ControllerContext.HttpContext.Server.MapPath("~" + result.Data.file_path));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, result.Data.file_name);
            }
            return null;
        }


        [HttpGet]
        [Route("download/fail/{id}")]
        public FileResult DownloadFailed(int id)
        {
            var api = Platform.Core.Api.Instance;
            var result = api.GetImportHistoryById(id).Data;
            if (result != null && result.ImportResult != null && !string.IsNullOrEmpty(result.ImportResult.file_path))
            {
                string pathFile = ControllerContext.HttpContext.Server.MapPath("~" + result.ImportResult.file_path);
                byte[] fileBytes = System.IO.File.ReadAllBytes(pathFile);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pathFile));
            }
            return null;
        }


        /// <summary>
        /// Change combobox campaign item
        /// </summary>
        [HttpPost]
        [Route("import-referral/{id}")]
        public ActionResult GetImportReferral(int id)
        {
            var api = Platform.Core.Api.Instance;
            var referral = api.GetReferralCampaignById(id).Data;
            if (referral == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    used = referral.gt_usage,
                    total = referral.quantity,
                    percent = referral.percent
                }
            });
        }

        /// <summary>
        /// Used for load combobox campaigns at the fist time
        /// </summary>
        [HttpPost]
        [Route("import-referral/campaigns/")]
        public ActionResult GetCampaigns()
        {
            var api = Platform.Core.Api.Instance;
            var campaigns = api.GetCurrentReferralCampaigns(new List<int>() { (int)ReferralCampaignStatus.Running, (int)ReferralCampaignStatus.Finished }).Data;

            if (campaigns == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                data = campaigns
            });
        }


        [HttpPost]
        [Route("pro-process-import-file")]
        public JsonResult PreprocessImportFile(int referralID, decimal totalRecord)
        {
            var api = Platform.Core.Api.Instance;
            var currentCamp = api.GetReferralCampaignById(referralID);
            if (!currentCamp.HasData)
            {
                return Json(new { success = false, message = "Invalid referral id" });
            }
            if (currentCamp.Data.status == (int)ReferralCampaignStatus.Finished)
            {
                string message = String.Format("This campaign is finished.\nDo you want to continue?");
                return Json(new { success = true, message = message });
            }

            var remainAllocatedQty = currentCamp.Data.quantity - currentCamp.Data.gt_usage;
            if (totalRecord > remainAllocatedQty)
            {
                string message = String.Format("Your records will be overflow the the allocated amount of current campaign.\n Your record is {0}, your remain allocated number is {1}. \n Do you want to continue?", totalRecord, remainAllocatedQty);
                return Json(new { success = true, message = message });
            }
            return Json(new { success = true });
        }

    }
}