using System.Web.Mvc;
using GoEat.Web.ActionFilter;
using GoEat.Web.Identity;

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
        catch
        {
        }
        return bFound;
    }

    public static bool IsCustomer(this ControllerBase controller)
    {
        bool bFound = false;
        try
        {
            //Check if the requesting user has the specified application permission...
            bFound = new RBAC(controller.ControllerContext.HttpContext.User.Identity.GetUserId<int>()).IsCustomer();
        }
        catch
        {
        }
        return bFound;
    }
}
