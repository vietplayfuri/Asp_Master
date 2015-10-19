using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using Platform.Models;
using GoPlay.Models;
using Newtonsoft.Json.Linq;
namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<List<Package>> GetPackagesForAdminUser(IDbConnection db)
        {
            string sqlQuery = @"SELECT p.*, g.name as game_name FROM package
                p JOIN game g ON p.game_id = g.id
                ORDER BY game_id";

            var packages = db.Query<Package>(sqlQuery).AsList();
            return Result<List<Package>>.Make(packages, errorIfNull: ErrorCodes.NotFound);
        }

        public Result<List<Package>> GetPackagesForAdminUser(IDbConnection db, int userId,
            bool creditArchived = false, bool gameArchived = false)
        {
            string sqlQuery = @"SELECT p.*, g.name as game_name 
                FROM package p 
                JOIN game g ON p.game_id = g.id
                JOIN studio_admin_assignment sas ON g.studio_id = sas.studio_id
                WHERE 
                    p.is_archived = @creditArchived
                AND g.is_archived = @gameArchived
                AND sas.game_admin_id = @userId
                ORDER BY game_id";

            var packages = db.Query<Package>(sqlQuery, new
            {
                userId,
                creditArchived,
                gameArchived
            }).AsList();

            return Result<List<Package>>.Make(packages, errorIfNull: ErrorCodes.NotFound);
        }


        public Result<List<external_exchange>> GetExternalExchange(IDbConnection db, int customerId, int gameId, string identifier = null, string transactionId = null)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.Append(@"SELECT * FROM external_exchange
                WHERE customer_account_id = @customerId
                    AND game_id = @gameId");

            if (!string.IsNullOrEmpty(identifier))
                strBuilder.Append(" AND exchange_option_identifier = @identifier");

            if (!string.IsNullOrEmpty(transactionId))
                strBuilder.Append(" AND transaction_id = @transactionId");

            var externalExchange = db.Query<external_exchange>(strBuilder.ToString(), new { customerId, gameId, identifier, transactionId }).AsList();
            return Result<List<external_exchange>>.Make(externalExchange, errorIfNull: ErrorCodes.INVALID_EXCHANGE_OPTION);
        }

        public bool CreateExternalExchange(IDbConnection db, external_exchange external_exchange)
        {
            string sqlQuery = @"
            INSERT INTO external_exchange(
                customer_account_id, game_id, package_id, credit_type_id, 
                exchange_option_identifier, transaction_id)
            VALUES (
                @customer_account_id, @game_id, @package_id, @credit_type_id, 
                @exchange_option_identifier, @transaction_id)";

            return 1 == db.Execute(sqlQuery, external_exchange);
        }

        public Result<Package> GetPackageForAdminUser(IDbConnection db, int packageId, bool? isActive = null,
            bool? isArchived = null)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"SELECT * FROM package WHERE id = @packageId");

            if (isActive.HasValue)
                strBuilder.Append(" AND is_active = @isActive");
            if (isArchived.HasValue)
                strBuilder.Append(" AND is_archived = @isArchived");

            strBuilder.Append(" ORDER BY game_id");

            var packages = db.Query<Package>(strBuilder.ToString(), new
            {
                packageId,
                isActive,
                isArchived
            }).FirstOrDefault();
            return Result<Package>.Make(packages, errorIfNull: ErrorCodes.NotFound);
        }
    }
}
