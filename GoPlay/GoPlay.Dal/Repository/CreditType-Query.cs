using Dapper;
using GoPlay.Models;
using Platform.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GoPlay.Dal
{
    public partial class Repo
    {
        /// <summary>
        /// # Get package
        /// # if admin --> should let isActive param is null
        /// # To allow admin account to test exchange option without showing it to everyone
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <param name="isActive">if admin --> should let isActive param is null</param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        public Result<CreditType> GetCreditType(IDbConnection db, int id, bool? isActive = null, bool? isArchived = null)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT * FROM credit_type WHERE id = @id");

            if (isArchived.HasValue)
                strBuilder.Append(" AND is_archived = @isArchived");
            if (isActive.HasValue)
                strBuilder.Append(" AND is_active = @isActive");

            var customer = db.Query<CreditType>(strBuilder.ToString(), new { id, isActive, isArchived }).FirstOrDefault();
            return Result<CreditType>.Make(customer, errorIfNull: ErrorCodes.NotFound);
        }

        public Result<CreditType> GetCreditType(IDbConnection db, string identifier, bool? isActive = null, bool? isArchived = null)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"SELECT * FROM credit_type 
                WHERE string_identifier = @identifier");

            if (isArchived.HasValue)
                strBuilder.Append(" AND is_archived = @isArchived");
            if (isActive.HasValue)
                strBuilder.Append(" AND is_active = @isActive");

            var creditType = db.Query<CreditType>(strBuilder.ToString(), new { identifier, isActive, isArchived }).FirstOrDefault();
            return Result<CreditType>.Make(creditType, errorIfNull: ErrorCodes.NotFound);
        }

        /// <summary>
        /// Get credit types
        /// </summary>
        /// <param name="db"></param>
        /// <param name="gameId"></param>
        /// <param name="isActive">if admin => isActive=NULL</param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        public Result<List<CreditType>> GetCreditTypes(IDbConnection db, int? gameId, bool? isActive = true, bool isArchived = false)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"SELECT * FROM credit_type WHERE is_archived = @isArchived");
            if (isActive.HasValue)
            {
                strBuilder.Append(@" AND is_active = @isActive");
            }
            if (gameId.HasValue)
            {
                strBuilder.Append(@" AND game_id = @gameId");
            }
            strBuilder.Append(@" ORDER BY id");

            var creditTypes = db.Query<CreditType>(strBuilder.ToString(), new
            {
                gameId = gameId.Value,
                isActive = isActive.Value,
                isArchived
            }).AsList();
            return Result<List<CreditType>>.Make(creditTypes, errorIfNull: ErrorCodes.NotFound);
        }


        public Result<List<CreditType>> GetCreditTypesForAdminUser(IDbConnection db)
        {
            string sqlQuery = @"SELECT ct.*, g.name as game_name
                FROM credit_type ct 
                JOIN game g ON ct.game_id = g.id
                ORDER BY game_id";

            var creditTypes = db.Query<CreditType>(sqlQuery).AsList();
            return Result<List<CreditType>>.Make(creditTypes, errorIfNull: ErrorCodes.NotFound);
        }


        public Result<List<CreditType>> GetCreditTypesForAdminUser(IDbConnection db, int userId,
            bool creditArchived = false, bool gameArchived = false)
        {
            string sqlQuery = @"SELECT ct.*, g.name as game_name 
                FROM credit_type ct 
                JOIN game g ON ct.game_id = g.id
                JOIN studio_admin_assignment sas ON g.studio_id = sas.studio_id
                WHERE 
                    ct.is_archived = @creditArchived
                AND g.is_archived = @gameArchived
                AND sas.game_admin_id = @userId
                ORDER BY game_id";

            var creditTypes = db.Query<CreditType>(sqlQuery, new
            {
                userId,
                creditArchived,
                gameArchived
            }).AsList();

            return Result<List<CreditType>>.Make(creditTypes, errorIfNull: ErrorCodes.NotFound);
        }


        public Result<CreditType> GetCreditTypeById(IDbConnection db, int credit_type_id)
        {
            var credit = db.Query<CreditType>("SELECT * FROM credit_type WHERE id = @credit_type_id",
                new { credit_type_id }).FirstOrDefault();
            return Result<CreditType>.Make(credit, errorIfNull: ErrorCodes.INVALID_CREDIT_TYPE_ID);
        }


        public bool UpdateCreditType(IDbConnection db, CreditType creditType)
        {
            string stringSql = @"UPDATE credit_type
				SET 
                    game_id = @game_id,
                    name = @name,
                    exchange_rate = @exchange_rate,
                    icon_filename = @icon_filename,
                    created_at = @created_at,
                    updated_at = @updated_at,
                    is_archived = @is_archived,
                    old_db_id = @old_db_id,
                    free_exchange_rate = @free_exchange_rate,
                    is_active = @is_active,
                    string_identifier = @string_identifier

				WHERE id = @id";
            return 1 == db.Execute(stringSql, creditType);
        }


        public int CreateCreditType(IDbConnection db, CreditType creditType)
        {
            string stringSql = @"INSERT INTO credit_type
                   (game_id, name, exchange_rate, icon_filename, created_at, 
                    updated_at, is_archived, old_db_id, free_exchange_rate, is_active, 
                    string_identifier)
            VALUES (@game_id, @name, @exchange_rate, @icon_filename, @created_at, 
                    @updated_at, @is_archived, @old_db_id, @free_exchange_rate, @is_active, 
                    @string_identifier)
            RETURNING id";

            return db.Query<int>(stringSql, creditType).FirstOrDefault();
        }
    }
}
