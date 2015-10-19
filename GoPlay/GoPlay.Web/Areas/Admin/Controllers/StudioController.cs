using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using GoPlay.Web.Areas.Admin.Models;
using GoPlay.Web.Controllers;
using GoPlay.Web.Helpers;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Areas.Admin.Controllers
{
    [Authorize]
    [RouteArea("admin")]
    [RoutePrefix("studio")]
    [RequiredLogin]
    public class StudioController : BaseController
    {
        [Route("")]
        [RBAC(AccessAction = GoPlayConstantValues.S_ROLE_ACCESS_ADMIN_GAME_PAGE)]
        public ActionResult Index()
        {
            var studios = GameHelper.GetStudiosForAdminUser(CurrentUser);
            return View(studios);
        }
        [HttpGet]
        [Route("add")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_ADMIN)]
        public ActionResult StudioAdd()
        {
            StudioAdminViewModel model = new StudioAdminViewModel()
            {
                action = "add"
            };
            return View(model);
        }
        [HttpPost]
        [Route("add")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_ADMIN)]
        public ActionResult StudioAdd(StudioAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var api = GoPlayApi.Instance;
                var studio = new Studio()
                {
                    name = model.name
                };
                studio.id = api.CreateStudio(studio);
                if (studio.id > 0)
                {
                    var studioAssignment = new StudioAdminAssignment()
                    {
                        studio_id = studio.id,
                        game_admin_id = CurrentUser.Id
                    };
                    api.CreateStudioAdminAssignment(studioAssignment);
                    this.Flash(string.Format("Successfully added studio {0}!", model.name), FlashLevel.Success);
                    return Redirect("/admin/studio/" + studio.id);
                }
                this.Flash(string.Format("Failure added studio {0}!", model.name), FlashLevel.Error);
            }
            return View(model);
        }

        [Route("{id}")]
        [RBAC(AccessAction = GoPlayConstantValues.S_ROLE_ACCESS_ADMIN_GAME_PAGE)]
        public ActionResult StudioDetail(int id)
        {
            var api = GoPlayApi.Instance;
            var studio = api.GetStudio(id).Data;
            if (studio == null)
            {
                this.Flash(string.Format("Studio with id {0} doesn't exist", id), FlashLevel.Warning);
                return Redirect("/admin/studio");
            }
            if (!PermissionHelper.HasViewStudio(CurrentUser.GetRoles(), CurrentUser.Id, id))
                return new HttpStatusCodeResult(403);
            var studioModel = new StudioDetailViewModel()
            {
                studio = studio,
                studiosAssignment = api.GetStudiosAssignment(studio.id).Data
            };
            return View(studioModel);
        }
        [HttpGet]
        [Route("{id}/edit")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
        public ActionResult StudioEdit(int id)
        {
            var api = GoPlayApi.Instance;
            var studio = api.GetStudio(id).Data;
            if (studio == null)
            {
                this.Flash(string.Format("Studio with id {0} doesn't exist", id), FlashLevel.Warning);
                return Redirect("/admin/studio");
            }
            if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, id))
                return new HttpStatusCodeResult(403);
            StudioAdminViewModel model = new StudioAdminViewModel()
            {
                action = "edit",
                name = studio.name,
                studio_id = studio.id
            };
            model.previous_page = Request.Params["previous_page"];
            return View("StudioAdd", model);
        }
        [HttpPost]
        [Route("{id}/edit")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_GAME_ADMIN)]
        public ActionResult StudioEdit(StudioAdminViewModel model)
        {
            if (!ModelState.IsValid)
                return View("StudioAdd", model);

            var api = GoPlayApi.Instance;
            if (!PermissionHelper.HasManageStudio(CurrentUser.GetRoles(), CurrentUser.Id, model.studio_id.Value))
                return new HttpStatusCodeResult(403);

            var studio = new Studio()
            {
                id = model.studio_id.Value,
                name = model.name
            };
            if (api.UpdateStudio(studio))
            {
                this.Flash(string.Format("Successfully updated studio {0}!", model.name), FlashLevel.Success);
                return Redirect("/admin/studio/" + studio.id);
            }
            this.Flash(string.Format("Failure updated studio {0}!", model.name), FlashLevel.Error);
            return View("StudioAdd", model);
        }

        [HttpGet]
        [Route("{id}/assign-game-admin-or-accountant")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_ADMIN)]
        public ActionResult StudioAssignAdminOrAccountant(int id)
        {
            var api = GoPlayApi.Instance;
            var studio = api.GetStudio(id).Data;
            if (studio == null)
            {
                this.Flash(string.Format("Studio with id {0} doesn't exist", id), FlashLevel.Warning);
                return Redirect("/admin/studio");
            }
            StudioAssignViewModel model = new StudioAssignViewModel() { studio = studio };
            return View(model);
        }
        [HttpPost]
        [Route("{id}/assign-game-admin-or-accountant")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_ADMIN)]
        public ActionResult StudioAssignAdminOrAccountant(StudioAssignViewModel model)
        {
            var api = GoPlayApi.Instance;
            var studio = api.GetStudio(model.studio.id).Data;
            if (studio == null)
            {
                this.Flash(string.Format("Studio with id {0} doesn't exist", model.studio.id), FlashLevel.Warning);
                return Redirect("/admin/studio");
            }
            if (!string.IsNullOrEmpty(model.username))
            {
                StringBuilder condition = new StringBuilder();
                string encodeForLike = model.username.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
                string termUsername = "'%" + encodeForLike + "%'";
                condition.AppendLine(string.Format(@"AND (lower(ca.email) LIKE {0} 
                   OR lower(ca.nickname) LIKE {0} 
                   OR lower(ca.username) LIKE {0})", termUsername));

                string subselectStudio = "SELECT game_admin_id FROM studio_admin_assignment WHERE studio_id = " + model.studio.id;
                string subselect = "SELECT distinct customer_account_id FROM auth_assignment WHERE customer_account_id IN (" + subselectStudio + ")";
                condition.AppendLine(@" AND ca.id NOT IN (" + subselect + ")");
                model.admins = api.GetCustomerAccountsByCondition(condition.ToString()).Data;
            }
            return View(model);
        }

        [HttpGet]
        [Route("{id}/assign/{userId}")]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_ADMIN)]
        public ActionResult Assign(int id, int userId)
        {
            var api = GoPlayApi.Instance;
            var studio = api.GetStudio(id).Data;
            var user = api.GetUserById(userId).Data;
            if (studio == null)
            {
                this.Flash(string.Format("Studio with id {0} doesn't exist", id), FlashLevel.Warning);
                return Redirect("/admin/studio");
            }
            if (user == null)
            {
                this.Flash(string.Format("CustomerAccount with id {0} doesn't exist", userId), FlashLevel.Warning);
                return Redirect("/admin/studio");
            }
            if (api.AssignStudioAdmin(id, userId, GoPlayConstantValues.S_ROLE_GAME_ADMIN, GoPlayConstantValues.S_ROLE_GAME_ACCOUNTANT))
            {
                this.Flash(string.Format("Successfully assigned game admin {0} to studio {1}!", user.GetDisplayName(), studio.name), FlashLevel.Success);
                return Redirect("/admin/studio/" + id);
            }
            return new HttpStatusCodeResult(403);
        }

        [HttpPost]
        [Route("{id}/unassign-game-admin-or-accountant")]
        [RequiredLogin]
        [RBAC(AccessRole = GoPlayConstantValues.S_ROLE_ADMIN)]
        public ActionResult StudioUnassignAdminOrAccountant(int id)
        {
            var api = GoPlayApi.Instance;
            var studio = api.GetStudio(id).Data;
            if (studio == null)
            {
                this.Flash(string.Format("Studio with id {0} doesn't exist", id), FlashLevel.Warning);
                return Redirect("/admin/studio");
            }

            int gameAdminId = 0;
            if (Request.Params["game_admin_id"] != null && !string.IsNullOrEmpty(Request.Params["game_admin_id"]))
            {
                Int32.TryParse(Request.Params["game_admin_id"], out gameAdminId);
            }
            var studioAdminAssignment = api.GetStudioAdminAssignment(gameAdminId, id).Data;
            if (studioAdminAssignment == null)
            {
                this.Flash(string.Format("Game admin/accountant {0} isn't assigned to studio {1}", gameAdminId, studio.name), FlashLevel.Warning);
                return Redirect(string.Format("/admin/studio/{0}", id));
            }

            if (api.DeleteStudioAdminAssignment(id, gameAdminId) && api.DeleteAuthAssignment(gameAdminId))
            {
                this.Flash(string.Format("Unassigned game admin/accountant {0} from studio {1}!", gameAdminId, studio.name), FlashLevel.Success);
                return Redirect(string.Format("/admin/studio/{0}", id));
            }

            return new HttpStatusCodeResult(403);
        }
    }
}