using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GToken.Web.ActionFilter
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
            var api = Platform.Core.Api.Instance;
            var roles = api.GetRolesByUserId(this.UserId);
            if (roles.Data.Any(r => r.name == "admin"))
            {
                roles = Platform.Core.Api.Instance.GetAllRoles();
            }
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
    }

    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<AuthAction> Permissions = new List<AuthAction>();
    }
}