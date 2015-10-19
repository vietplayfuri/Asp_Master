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
using GoPlay.Models;
using Platform.Models;
using Npgsql;

namespace GoPlay.Dal
{
    public partial class Repo
    {
        /// <summary>
        /// # Get package
        /// # if admin --> should let isActive param is false
        /// # To allow admin account to test exchange option without showing it to everyone
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <param name="isActive">if admin --> should let isActive param is false</param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        public Result<Package> GetPackage(IDbConnection db, int id, bool? isActive = null, bool? isArchived = null)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"SELECT * FROM package 
                WHERE id = @id");

            if (isArchived.HasValue)
                strBuilder.Append(" AND is_archived = @isArchived");
            if (isActive.HasValue)
                strBuilder.Append(" AND is_active = @isActive");

            var customer = db.Query<GoPlay.Models.Package>(strBuilder.ToString(), new { id, isActive, isArchived }).FirstOrDefault();
            return Result<GoPlay.Models.Package>.Make(customer, errorIfNull: ErrorCodes.INVALID_PACKAGE_ID);
        }


        public Result<Package> GetPackage(IDbConnection db, string identifier, bool? isActive = null, bool? isArchived = null)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT * FROM package WHERE string_identifier = @identifier");

            if (isArchived.HasValue)
                strBuilder.Append(" AND is_archived = @isArchived");
            if (isActive.HasValue)
                strBuilder.Append(" AND is_active = @isActive");

            var customer = db.Query<GoPlay.Models.Package>(strBuilder.ToString(), new { identifier, isActive, isArchived }).FirstOrDefault();
            return Result<GoPlay.Models.Package>.Make(customer, errorIfNull: ErrorCodes.INVALID_PACKAGE_ID);
        }


        public Result<List<Package>> GetPackages(IDbConnection db,
    int? gameId, bool? isActive = true, bool isArchived = false)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"SELECT * FROM package 
                WHERE is_archived = @isArchived");
            if (isActive.HasValue)
            {
                strBuilder.Append(@" AND is_active = @isActive");
            }
            if (gameId.HasValue)
            {
                strBuilder.Append(@" AND game_id=@gameId");
            }
            strBuilder.Append(@" ORDER BY id");

            var customer = db.Query<GoPlay.Models.Package>(strBuilder.ToString(), new { gameId = gameId.Value, isActive = isActive.Value, isArchived }).ToList();
            return Result<List<Package>>.Make(customer, errorIfNull: ErrorCodes.INVALID_PACKAGE_ID);
        }

        public Result<List<GtokenPackage>> GetBasicGtokenPackages(IDbConnection db)
        {
            string sqlString = @"SELECT * FROM gtoken_package
                                WHERE name != 'GToken' AND currency != 'IDR' AND is_archived = False
                                ORDER BY price";

            var result = db.Query<GtokenPackage>(sqlString).ToList();
            return Result<List<GtokenPackage>>.Make(result);
        }

        public Result<List<GtokenPackage>> GetUpointGTokenPackages(IDbConnection db)
        {
            string sqlString = @"SELECT * FROM gtoken_package
                                WHERE name != 'GToken' AND currency = 'IDR' AND name not like '%UPoint Voucher%' AND is_archived = False
                                ORDER BY price";

            var result = db.Query<GtokenPackage>(sqlString).ToList();
            return Result<List<GtokenPackage>>.Make(result);
        }

        public Result<GtokenPackage> GetCustomGtokenPackage(IDbConnection db)
        {
            string sqlString = @"SELECT * FROM gtoken_package
                                WHERE name = 'GToken' AND is_archived = False";

            var result = db.Query<GtokenPackage>(sqlString).SingleOrDefault();
            return Result<GtokenPackage>.Make(result, errorIfNull: ErrorCodes.NotFound);
        }

        public Result<GtokenPackage> GetTokenPackageBySKU(IDbConnection db, string sku)
        {
            string sql = @"SELECT * FROM gtoken_package WHERE sku = @sku";

            var package = db.Query<GtokenPackage>(sql, new { sku }).FirstOrDefault();
            return Result<GtokenPackage>.Make(package);
        }

        public int CreatePackage(IDbConnection db, Package package)
        {
            string stringSql = @"INSERT INTO package(
                game_id, name, play_token_value, icon_filename, created_at, 
                updated_at, is_archived, old_db_id, free_play_token_value, is_active, 
                string_identifier, limited_time_offer)
            VALUES(
                @game_id, @name, @play_token_value, @icon_filename, @created_at, 
                @updated_at, @is_archived, @old_db_id, @free_play_token_value, @is_active, 
                @string_identifier, @limited_time_offer)
            RETURNING id";

            return db.Query<int>(stringSql, package).FirstOrDefault();
        }

        public bool UpdatePackage(IDbConnection db, Package package)
        {
            string stringSql = @"UPDATE package
				SET 
                    game_id = @game_id,
                    name = @name,
                    play_token_value = @play_token_value,
                    icon_filename = @icon_filename,
                    created_at = @created_at,
                    updated_at = @updated_at,
                    is_archived = @is_archived,
                    old_db_id = @old_db_id,
                    free_play_token_value = @free_play_token_value,
                    is_active = @is_active,
                    string_identifier = @string_identifier,
                    limited_time_offer = @limited_time_offer

				WHERE id = @id";
            return 1 == db.Execute(stringSql, package);
        }
    }
}
