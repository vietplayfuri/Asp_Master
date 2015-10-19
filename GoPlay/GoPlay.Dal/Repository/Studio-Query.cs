using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Net;
using System.Data;
using System.Text.RegularExpressions;
using Platform.Models;


namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<List<Studio>> GetAllStudios()
        {
            using (var db = OpenConnectionFromPool())
            {
                var studios = db.Query<Studio>("SELECT * FROM studio ORDER BY id").AsList();
                return Result<List<Studio>>.Make(studios);
            }
        }


        public Result<Studio> GetStudio(int id)
        {
            using (var db = OpenConnectionFromPool())
            {
                var studio = db.Query<Studio>("SELECT * FROM studio WHERE id=@id", new { id }).FirstOrDefault();
                return Result<Studio>.Make(studio, errorIfNull: ErrorCodes.InvalidGameId);
            }
        }


        public Result<List<StudioAdminAssignment>> GetStudio(IDbConnection db, int userId, int studioId)
        {
            string sqlQuery = @"
                SELECT * 
                FROM studio_admin_assignment
                WHERE 
                    studio_id = @studioId
                AND game_admin_id = @userId";

            var studio = db.Query<StudioAdminAssignment>(sqlQuery, new { userId, studioId }).AsList();
            return Result<List<StudioAdminAssignment>>.Make(studio, errorIfNull: ErrorCodes.NotFound);

        }

        public bool HasStudioPermission(IDbConnection db, int userId, int studio_id)
        {
            string sqlQuery = @"SELECT EXISTS (SELECT 1
                                                FROM  studio_admin_assignment 
                                                WHERE studio_admin_assignment.game_admin_id = @userId
					                            AND studio_admin_assignment.studio_id = @studio_id)";
            return db.Query<bool>(sqlQuery, new { userId, studio_id }).FirstOrDefault();
        }

        public Result<List<Studio>> GetStudios(IDbConnection db, int userId)
        {
            string sqlQuery = @"SELECT studio.*
                                FROM studio
                                JOIN studio_admin_assignment ON studio_admin_assignment.studio_id = studio.id
                                WHERE studio_admin_assignment.game_admin_id = @userId";
            var studios = db.Query<Studio>(sqlQuery, new { userId }).AsList();
            return Result<List<Studio>>.Make(studios);
        }

        public Result<List<StudioAdminAssignment>> GetStudiosAssignment(IDbConnection db, int studioId)
        {
            string sqlQuery = @"
                SELECT sa.studio_id, sa.game_admin_id, ca.username  AS game_admin_username
                FROM studio_admin_assignment sa
                LEFT JOIN customer_account ca ON sa.game_admin_id = ca.id
                WHERE 
                    studio_id = @studioId;";
            var studio = db.Query<StudioAdminAssignment>(sqlQuery, new { studioId }).AsList();
            return Result<List<StudioAdminAssignment>>.Make(studio, errorIfNull: ErrorCodes.NotFound);
        }
        public int CreateStudio(IDbConnection db, Studio studio)
        {
                return db.Query<int>(@"INSERT INTO studio(name)
                                                VALUES (@name)
                                                RETURNING id;", studio).FirstOrDefault();
        }
        public bool CreateStudioAdminAssignment(IDbConnection db, StudioAdminAssignment studioAssignment)
        {
            return 1 == db.Execute(@"INSERT INTO studio_admin_assignment(
                                            studio_id, game_admin_id)
                                 VALUES (@studio_id, @game_admin_id);", studioAssignment);
        }

        public bool UpdateStudio(IDbConnection db, Studio studio)
        {
            studio.updated_at = DateTime.UtcNow;
            return 1 == db.Execute(@"UPDATE studio
                                        SET name = @name, updated_at = @updated_at
                                        WHERE id = @id", studio);
        }

        public bool DeleteStudioAdminAssignment(IDbConnection db, int studioId, int userId)
        {
            return 1 == db.Execute("DELETE FROM studio_admin_assignment WHERE studio_id=@studioId AND game_admin_id = @userId", new { studioId, userId });
        }

        public bool DeleteAuthAssignment(IDbConnection db, int id, List<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
                return false;

            string sqlQuery = string.Format(@"DELETE FROM auth_assignment 
                WHERE customer_account_id = @id
                AND role_id in ({0})", string.Join(",", roleIds));
            return roleIds.Count == db.Execute(sqlQuery, new { id });
        }
    }
}
