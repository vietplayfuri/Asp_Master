using GoPlay.Dal;
using GoPlay.Models;
using Newtonsoft.Json.Linq;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public Result<List<StudioAdminAssignment>> GetStudioAdminAssignment(int userId, int studioId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetStudio(db, userId, studioId);
            }
        }

        public bool HasStudioPermission(int userId, int studio_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.HasStudioPermission(db, userId, studio_id);
            }
        }
        public Result<List<Studio>> GetAllStudios()
        {
            return Repo.Instance.GetAllStudios();
        }


        public Result<List<Studio>> GetStudios(int userId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetStudios(db, userId);
            }
        }

        public Result<Studio> GetStudio(int id)
        {
            return Repo.Instance.GetStudio(id);
        }
        public Result<List<StudioAdminAssignment>> GetStudiosAssignment(int studioId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetStudiosAssignment(db, studioId);
            }
        }

        public int CreateStudio(Studio studio)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateStudio(db, studio);
            }
        }
        public bool CreateStudioAdminAssignment(StudioAdminAssignment studioAssignment)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateStudioAdminAssignment(db, studioAssignment);
            }
        }

        public bool UpdateStudio(Studio studio)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateStudio(db, studio);
            }
        }


        public bool DeleteStudioAdminAssignment(int studioId, int userId)
        {

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.DeleteStudioAdminAssignment(db, studioId, userId);
            }
        }

        public bool AssignStudioAdmin(int studio_id, int userId, params string[] roles)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var myTrans = db.BeginTransaction();
                try
                {
                    if (repo.CreateAuthAssignmentByRoles(db, userId, roles))
                    {
                        var studioAssignment = new StudioAdminAssignment()
                        {
                            studio_id = studio_id,
                            game_admin_id = userId
                        };
                        repo.CreateStudioAdminAssignment(db, studioAssignment);
                        myTrans.Commit();
                        return true;
                    }
                    myTrans.Rollback();
                    return false;
                }
                catch
                {
                    myTrans.Rollback();
                    return false;
                }
            }
        }

        public bool DeleteAuthAssignment(int customerId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var authRoles = repo.GetAuthRolesByUserId(db, customerId).Data;
                if (authRoles != null && authRoles.Any())
                {
                    var idsDeletedRole = authRoles.Where(r => GoPlayConstantValues.LIST_OF_DELETED_ROLES.Contains(r.name)).Select(r => r.id);
                    return repo.DeleteAuthAssignment(db, customerId, idsDeletedRole.ToList());
                }
            }
            return true;
        }
    }
}
