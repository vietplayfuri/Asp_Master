using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Helpers
{
    public static class PermissionHelper
    {
        public static bool HasViewStudio(List<UserRole> userRole,int userId, int studioId)
        {
            if (userRole != null)
            {
                if (userRole.Any(x => x.RoleName == GoPlayConstantValues.S_ROLE_ADMIN))
                    return true;
                else if (userRole.Any(x => x.RoleName == GoPlayConstantValues.S_ROLE_GAME_ADMIN || x.RoleName == GoPlayConstantValues.S_ROLE_GAME_ACCOUNTANT))
                {
                    return GoPlayApi.Instance.HasStudioPermission(userId, studioId);
                }
            }
            return false;
        }
        public static bool HasManageStudio(List<UserRole> userRole, int userId, int studioId)
        {
            if (userRole != null)
            {
                if (userRole.Any(x => x.RoleName == GoPlayConstantValues.S_ROLE_ADMIN))
                    return true;
                else if (userRole.Any(x => x.RoleName == GoPlayConstantValues.S_ROLE_GAME_ADMIN))
                {
                    return GoPlayApi.Instance.HasStudioPermission(userId, studioId);
                }
            }
            return false;
        }
    }
}