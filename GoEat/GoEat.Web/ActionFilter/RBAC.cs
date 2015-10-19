using System.Collections.Generic;
using System.Linq;
using GoEat.Core;
using GoEat.Models;

namespace GoEat.Web.ActionFilter
{
    public class RBAC
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        private List<UserRole> Roles = new List<UserRole>();

        public RBAC(string _username)
        {
            Username = _username;
            GetUserRolesAndPermission();
        }

        public RBAC(int userid)
        {
            UserId = userid;
            GetUserRolesAndPermission();
        }

        private void GetUserRolesAndPermission()
        {
            var api = GoEatApi.Instance;
            var roles = api.GetRolesByUserId(UserId);
            foreach (var role in roles.Data)
            {
                var userRole = new UserRole { RoleId = role.id, RoleName = role.name };
                var permissions = api.GetActionsByRoleId(role.id);
                foreach (var p in permissions.Data)
                {
                    userRole.Permissions.Add(p);
                }
                Roles.Add(userRole);
            }

        }

        public bool HasPermission(string requiredPermission)
        {
            bool bFound = false;
            foreach (UserRole role in Roles)
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

        public bool IsCustomer()
        {
            return Roles.Count == 0;
        }
    }

    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<AuthAction> Permissions = new List<AuthAction>();
    }
}