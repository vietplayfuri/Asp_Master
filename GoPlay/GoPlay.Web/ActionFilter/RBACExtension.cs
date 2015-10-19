using System.Linq;
using GoPlay.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using GoPlay.Web.Identity;
using GoPlay.Web.Helpers;

namespace GoPlay.Web
{
    public static class RBACExtension
    {
        public static bool HasRole(this ControllerBase controller, string role)
        {
            bool bFound = false;
            try
            {
                //Check if the requesting user has the specified role...
                //bFound = new RBAC(controller.ControllerContext.HttpContext.User.Identity.GetUserId<int>()).HasRole(role);
                var userRoles = UserHelper.GetRoles(controller.ControllerContext.HttpContext.User.Identity.GetUserId<int>());
                if (userRoles.Any(r => r.RoleName == "admin"))
                    return true;
                else
                    bFound = userRoles.Any(r => r.RoleName == role);
            }
            catch { }
            return bFound;
        }

        public static bool HasPermission(this ControllerBase controller, string permission)
        {
            //With admin in testing header, we will show all header.
            //return true;
            bool bFound = false;
            try
            {
                //Check if the requesting user has the specified application permission...
                //bFound = new RBAC(controller.ControllerContext.HttpContext.User.Identity.GetUserId<int>()).HasPermission(permission);
                bFound = UserHelper.GetRoles(controller.ControllerContext.HttpContext.User.Identity.GetUserId<int>()).Any(r => r.Permissions.Any(a => a.action == permission));
            }
            catch { }
            return bFound;
        }
    }
}