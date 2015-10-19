using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoPlay.Models;
using GoPlay.Core;
using GoPlay.Web.ActionFilter;

namespace GoPlay.Web.Helpers
{
    public class UserHelper
    {
        public static List<topup_card> getTopupCards(int userId, string status = "all")
        {
            var api = GoPlayApi.Instance;
            return api.getTopupCards(userId, status).Data;
        }

        /// <summary>
        /// Service the purpose get inviter username
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetIdByUsername(string username)
        {
            var api = GoPlayApi.Instance;
            return api.GetIdByUsername(username);
        }


        public static List<UserRole> GetRoles(int userId)
        {
            if (!HttpContext.Current.Items.Contains(GoPlay.Web.Const.Constants.CurrentUserRoleHttpContextKey))
            {
                var userRoles = GoPlayApi.Instance.GetRolesByUserId(userId).Data;
                if (userRoles == null)
                    return new List<UserRole>();
                if (userRoles.Any(r => r.name == "admin"))
                {
                    userRoles = GoPlayApi.Instance.GetAllRoles().Data;
                }

                var roles = userRoles.Select(r => new UserRole() { RoleId = r.id, RoleName = r.name }).ToList();
                foreach (var role in roles)
                {
                    var permissions = GoPlayApi.Instance.GetActionsByRoleId(role.RoleId);
                    foreach (var p in permissions.Data)
                    {
                        role.Permissions.Add(p);
                    }
                }
                HttpContext.Current.Items.Add(GoPlay.Web.Const.Constants.CurrentUserRoleHttpContextKey, roles);
            }
            return HttpContext.Current.Items[GoPlay.Web.Const.Constants.CurrentUserRoleHttpContextKey] as List<UserRole>;
        }
    }
}