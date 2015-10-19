using GToken.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using GToken.Web.Identity;

public static class RBACExtension
{
    public static bool HasRole(this ControllerBase controller, string role)
    {
        bool bFound = false;
        try
        {
            //Check if the requesting user has the specified role...
            bFound = new RBAC(controller.ControllerContext.HttpContext.User.Identity.GetUserId<int>()).HasRole(role);
        }
        catch { }
        return bFound;
    }

    public static bool HasPermission(this ControllerBase controller, string permission)
    {
        bool bFound = false;
        try
        {
            //Check if the requesting user has the specified application permission...
            bFound = new RBAC(controller.ControllerContext.HttpContext.User.Identity.GetUserId<int>()).HasPermission(permission);
        }
        catch { }
        return bFound;
    }
}
