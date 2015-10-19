using GoPlay.Web.Identity;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoPlay.Models;
using GoPlay.Core;

namespace GoPlay.Web.ActionFilter
{
    public class RBAC
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        private List<UserRole> Roles = new List<UserRole>();

        public RBAC(int userId)
        {
            this.UserId = userId;
            GetUserRolesAndPermission();
        }

        private void GetUserRolesAndPermission()
        {
            var api = GoPlayApi.Instance;
            var roles = api.GetRolesByUserId(this.UserId);
            foreach (var role in roles.Data)
            {
                var userRole = new UserRole() { RoleId = role.id, RoleName = role.name };
                var permissions = api.GetActionsByRoleId(role.id);
                foreach (var p in permissions.Data)
                {
                    userRole.Permissions.Add(p);
                }
                this.Roles.Add(userRole);
            }

        }

        public bool HasPermission(string requiredPermission)
        {
            requiredPermission = !string.IsNullOrEmpty(requiredPermission) ? requiredPermission : string.Empty; 
            bool bFound = false;
            foreach (UserRole role in this.Roles)
            {
                bFound = (role.Permissions.Where(p => p.action == requiredPermission.ToLower()).ToList().Count > 0);
                if (bFound)
                    break;
            }
            return bFound;
        }

        public bool HasRole(string role)
        {
            return (Roles.Where(p => p.RoleName == role).ToList().Count > 0);
        }

        public static bool checkRolePermission(ApplicationUser currentUser, string requiredPermission)
        {
            if (currentUser != null)
            {
                var rbac = new RBAC(currentUser.Id);
                return rbac.HasPermission(requiredPermission);
            }
            return false;
        }
    }

    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<auth_action> Permissions = new List<auth_action>();
    }
}