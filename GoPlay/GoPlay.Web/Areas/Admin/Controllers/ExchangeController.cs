using GoPlay.Core;
using GoPlay.Web.Areas.Admin.Models;
using GoPlay.Web.Controllers;
using GoPlay.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoPlay.Web.Helpers;
using GoPlay.Models;
using AutoMapper;
using System.Configuration;
using Platform.Models;

namespace GoPlay.Web.Areas.Admin.Controllers
{
    [Authorize]
    [RouteArea("admin")]
    [RoutePrefix("exchange")]
    [RequiredLogin]
    [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
    public class ExchangeController : BaseController
    {
        #region CreditType
        [Route("")]
        public ActionResult Index()
        {
            var api = GoPlayApi.Instance;

            var creditTypes = api.GetCreditTypesForAdminUser(GoPlayConstantValues.S_ROLE_GAME_ADMIN, CurrentUser.Id, false, false);

            var packages = api.GetPackagesForAdminUser(GoPlayConstantValues.S_ROLE_GAME_ADMIN, CurrentUser.Id);

            return View(new AdminExchangeModel { CreditTypes = creditTypes.Data, Packages = packages.Data });
        }

        [Route("creditType/{id}")]
        public ActionResult CreditTypeDetail(int id)
        {
            var api = GoPlayApi.Instance;

            var creditType = api.GetCreditTypeForAdminUser(GoPlayConstantValues.S_ROLE_GAME_ADMIN, id).Data;

            if (creditType == null)
            {
                this.Flash(string.Format(Resources.Resources.CREDIT_TYPE_DOES_NOT_EXIST, id), FlashLevel.Warning);
                return RedirectToAction("Index", "exchange");
            }

            Game game = api.GetGame(creditType.game_id).Data;
            if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id ?? 0))
            {
                return new HttpStatusCodeResult(403);
            }

            Mapper.CreateMap<CreditType, AdminEditExchangeModel>()
                .ForMember(d => d.icon, s => s.Ignore())
                .ForMember(d => d.is_active, s => s.MapFrom(src => (src.is_active.HasValue && src.is_active.Value) ? "on" : string.Empty))
                .ForMember(d => d.is_archived, s => s.MapFrom(src => src.is_archived ? "on" : string.Empty));
            var model = Mapper.Map<AdminEditExchangeModel>(creditType);
            model.is_package = false;
            model.game_name = game.name;
            return View(model);
        }

        [Route("creditType/{id}/edit")]
        public ActionResult CreditTypeEdit(int id)
        {
            var api = GoPlayApi.Instance;

            var creditType = api.GetCreditTypeForAdminUser(GoPlayConstantValues.S_ROLE_GAME_ADMIN, id).Data;

            if (creditType == null)
            {
                this.Flash(string.Format(Resources.Resources.CREDIT_TYPE_DOES_NOT_EXIST, id), FlashLevel.Warning);
                return RedirectToAction("Index", "exchange");
            }

            Game game = api.GetGame(creditType.game_id).Data;
            if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id ?? 0))
            {
                return new HttpStatusCodeResult(403);
            }

            Mapper.CreateMap<CreditType, AdminEditExchangeModel>()
                .ForMember(d => d.icon, s => s.Ignore())
                .ForMember(d => d.is_active, s => s.MapFrom(src => (src.is_active.HasValue && src.is_active.Value) ? "on" : string.Empty))
                .ForMember(d => d.is_archived, s => s.MapFrom(src => src.is_archived ? "on" : string.Empty));

            var model = Mapper.Map<CreditType, AdminEditExchangeModel>(creditType);
            model.is_package = false;
            model.games = GameHelper.GetGamesForAdminUser(CurrentUser);
            model.previous_page = Request.Params["previous_page"];
            return View("Edit", model);
        }

        [HttpPost]
        [Route("creditType/{id}/edit")]
        public ActionResult CreditTypeEdit(AdminEditExchangeModel model)
        {
            if (ModelState.IsValid)
            {
                var api = GoPlayApi.Instance;
                var existingCredit = api.GetCreditType(model.string_identifier).Data;
                if (existingCredit != null && existingCredit.id != model.id)
                {
                    ModelState.AddModelError("string_identifier", Resources.Resources.STRING_IDENTIFIER_DUPLICATE);
                    return View("Edit", model);
                }

                var creditType = api.GetCreditType(model.id).Data;
                creditType = EditCreditType(creditType, model);
                if (api.UpdateCreditType(creditType))
                {
                    this.Flash(string.Format("Successfully updated credit type {0}!", model.name), FlashLevel.Success);
                    return RedirectToAction("CreditTypeDetail", model.id);
                }
                this.Flash(ErrorCodes.ServerError.ToErrorMessage(), FlashLevel.Error);
            }
            model.games = GameHelper.GetGamesForAdminUser(CurrentUser);
            return View("Edit", model);
        }

        private CreditType EditCreditType(CreditType creditType, AdminEditExchangeModel model)
        {
            if (model.icon != null && model.icon.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath(ConfigurationManager.AppSettings["UPLOADS_DIR"]);
                string filename = GoPlayApi.Instance.HandleFile(HttpContext.Server.MapPath("~"), model.icon.InputStream, path, model.icon.FileName);
                creditType.icon_filename = filename;
            }

            creditType.is_active = (!String.IsNullOrEmpty(model.is_active) && model.is_active == "on")
                ? true
                : false;
            creditType.is_archived = (!string.IsNullOrEmpty(model.is_archived) && model.is_archived == "on")
                ? true
                : false;
            creditType.updated_at = DateTime.UtcNow;
            creditType.exchange_rate = model.exchange_rate;
            creditType.free_exchange_rate = model.free_exchange_rate;
            creditType.game_id = model.game_id;
            creditType.name = model.name;
            creditType.string_identifier = model.string_identifier;

            return creditType;
        }

        [Route("creditType/add")]
        public ActionResult CreditTypeAdd()
        {
            return View("Edit", new AdminEditExchangeModel()
            {
                games = GameHelper.GetGamesForAdminUser(CurrentUser)//GameHelper.GetGamesForCurrentUser(CurrentUser)
            });
        }

        [HttpPost]
        [Route("creditType/add")]
        public ActionResult CreditTypeAdd(AdminEditExchangeModel model)
        {
            if (ModelState.IsValid)
            {
                var game = GameHelper.GetGameForCurrentUser(CurrentUser, model.game_id);
                var api = GoPlayApi.Instance;

                if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id ?? 0))
                {
                    return new HttpStatusCodeResult(403);
                }

                if (api.GetCreditType(model.string_identifier).Data != null)
                {
                    ModelState.AddModelError("string_identifier", Resources.Resources.STRING_IDENTIFIER_DUPLICATE);
                    return View("Edit", model);
                }

                CreditType creditType = new CreditType();
                creditType = EditCreditType(creditType, model);
                int newId = api.CreateCreditType(creditType);
                if (newId > 0)
                {
                    this.Flash(string.Format("Successfully added credit type {0}!", model.name),
                        FlashLevel.Success);
                    return RedirectToAction("CreditTypeDetail", "exchange", new { id = newId });
                }

                this.Flash(string.Format(ErrorCodes.ServerError.ToErrorMessage(), model.name), FlashLevel.Error);
                return View("Edit", model);
            }
            return View("Edit", model);
        }
        #endregion

        #region Package
        [Route("package/add")]
        public ActionResult PackageAdd()
        {
            return View("Edit", new AdminEditExchangeModel()
            {
                is_package = true,
                games = GameHelper.GetGamesForAdminUser(CurrentUser)
            });
        }

        [HttpPost]
        [Route("package/add")]
        public ActionResult PackageAdd(AdminEditExchangeModel model)
        {
            if (ModelState.IsValid)
            {
                var game = GameHelper.GetGameForCurrentUser(CurrentUser, model.game_id);
                var api = GoPlayApi.Instance;

                if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id ?? 0))
                {
                    return new HttpStatusCodeResult(403);
                }

                if (api.GetCreditType(model.string_identifier).Data != null)
                {
                    ModelState.AddModelError("string_identifier", Resources.Resources.STRING_IDENTIFIER_DUPLICATE);
                    return View("Edit", model);
                }

                Package package = new Package();
                package = EditPackage(package, model);
                int newId = api.CreatePackage(package);
                if (newId > 0)
                {
                    this.Flash(string.Format("Successfully added Package {0}!", model.name), FlashLevel.Success);
                    return RedirectToAction("PackageDetail", "exchange", new { id = newId });
                }

                this.Flash(string.Format(ErrorCodes.ServerError.ToErrorMessage(), model.name), FlashLevel.Error);
                return View("Edit", model);
            }
            return View("Edit", model);
        }


        private Package EditPackage(Package package, AdminEditExchangeModel model)
        {
            if (model.icon != null && model.icon.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath(ConfigurationManager.AppSettings["UPLOADS_DIR"]);
                string filename = GoPlayApi.Instance.HandleFile(HttpContext.Server.MapPath("~"), model.icon.InputStream, path, model.icon.FileName);
                package.icon_filename = filename;
            }

            package.is_active = (!string.IsNullOrEmpty(model.is_active) && model.is_active == "on")
                ? true
                : false;
            package.is_archived = (!string.IsNullOrEmpty(model.is_archived) && model.is_archived == "on")
                ? true
                : false;
            package.updated_at = DateTime.UtcNow;
            package.play_token_value = model.play_token_value;
            package.free_play_token_value = model.free_play_token_value;
            package.game_id = model.game_id;
            package.name = model.name;
            package.string_identifier = model.string_identifier;
            package.limited_time_offer = model.limited_time_offer;

            return package;
        }

        [Route("package/{id}")]
        public ActionResult PackageDetail(int id)
        {
            var api = GoPlayApi.Instance;
            var package = api.GetPackageForAdminUser(GoPlayConstantValues.S_ROLE_GAME_ADMIN, id, null, false).Data;
            if (package == null)
            {
                this.Flash(string.Format(Resources.Resources.PACKAGE_DOES_NOT_EXIST, id), FlashLevel.Warning);
                return RedirectToAction("Index", "exchange");
            }

            Game game = api.GetGame(package.game_id).Data;
            if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id ?? 0))
            {
                return new HttpStatusCodeResult(403);
            }

            Mapper.CreateMap<Package, AdminEditExchangeModel>()
                .ForMember(d => d.icon, s => s.Ignore())
                .ForMember(d => d.is_active, s => s.MapFrom(src => (src.is_active.HasValue && src.is_active.Value) ? "on" : string.Empty))
                .ForMember(d => d.is_archived, s => s.MapFrom(src => src.is_archived ? "on" : string.Empty));
            var model = Mapper.Map<AdminEditExchangeModel>(package);
            model.is_package = true;
            model.game_name = game.name;
            return View(model);
        }

        [Route("package/{id}/edit")]
        public ActionResult PackageEdit(int id)
        {
            var api = GoPlayApi.Instance;
            var package = api.GetPackageForAdminUser(GoPlayConstantValues.S_ROLE_GAME_ADMIN, id, null, false).Data;
            if (package == null)
            {
                this.Flash(string.Format(Resources.Resources.PACKAGE_DOES_NOT_EXIST, id), FlashLevel.Warning);
                return RedirectToAction("Index", "exchange");
            }

            Game game = api.GetGame(package.game_id).Data;
            if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, game.studio_id ?? 0))
            {
                return new HttpStatusCodeResult(403);
            }

            Mapper.CreateMap<Package, AdminEditExchangeModel>()
                .ForMember(d => d.icon, s => s.Ignore())
                .ForMember(d => d.is_active, s => s.MapFrom(src => (src.is_active.HasValue && src.is_active.Value) ? "on" : string.Empty))
                .ForMember(d => d.is_archived, s => s.MapFrom(src => src.is_archived ? "on" : string.Empty));

            var model = Mapper.Map<Package, AdminEditExchangeModel>(package);
            model.is_package = true;
            model.games = GameHelper.GetGamesForAdminUser(CurrentUser);
            model.previous_page = Request.Params["previous_page"];
            return View("Edit", model);
        }

        [HttpPost]
        [Route("package/{id}/edit")]
        public ActionResult PackageEdit(AdminEditExchangeModel model)
        {
            if (ModelState.IsValid)
            {
                var api = GoPlayApi.Instance;
                var existing = api.GetPackage(model.string_identifier).Data;
                if (existing != null && existing.id != model.id)
                {
                    ModelState.AddModelError("string_identifier", Resources.Resources.STRING_IDENTIFIER_DUPLICATE);
                    return View("Edit", model);
                }

                var package = api.GetPackage(model.id).Data;
                package = EditPackage(package, model);
                if (api.UpdatePackage(package))
                {
                    this.Flash(string.Format("Successfully updated package {0}!", model.name), FlashLevel.Success);
                    return RedirectToAction("PackageDetail", model.id);
                }
                this.Flash(ErrorCodes.ServerError.ToErrorMessage(), FlashLevel.Error);
            }
            model.games = GameHelper.GetGamesForAdminUser(CurrentUser);
            return View("Edit", model);
        }
        #endregion
    }
}