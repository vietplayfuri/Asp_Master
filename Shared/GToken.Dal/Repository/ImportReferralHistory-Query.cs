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
using Npgsql;
using System.Data.SqlTypes;


namespace Platform.Dal
{
    public partial class Repo
    {

        /// <summary>
        /// Get RecordDownloadHistory for admin
        /// </summary>
        public Result<List<ImportReferralHistory>> GetImportHistory(IDbConnection db, int? gameId = null,
            DateTime? from = null, DateTime? to = null, int? skip = null, int? take = null)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"SELECT rdh.*, rc.game_name, rc.title as campaign_name
                FROM import_referral_history rdh
                JOIN referral_campaign rc on rc.id = rdh.referral_campaign_id
                WHERE 1 = 1");

            if (gameId.HasValue)
                sqlBuilder.AppendLine("AND rdh.game_id = @gameId");
            if (to.HasValue && to.Value >= SqlDateTime.MinValue.Value)
                sqlBuilder.AppendLine("AND rdh.created_at <= @to");
            if (from.HasValue && from.Value >= SqlDateTime.MinValue.Value)
                sqlBuilder.AppendLine("AND rdh.created_at >= @from");

            sqlBuilder.AppendLine("ORDER BY rdh.id DESC");

            if (skip.HasValue && take.HasValue && skip.Value >= 0 && take.Value > 0)
                sqlBuilder.AppendLine("OFFSET @skip LIMIT @take");

            string sqlQuery = sqlBuilder.ToString();
            var importHistory = db.Query<ImportReferralHistory>(sqlQuery, new
            {
                gameId,
                to,
                from,
                skip,
                take
            }).AsList();

            return Result<List<ImportReferralHistory>>.Make(importHistory, ErrorCodes.NotFound);
        }


        /// <summary>
        /// GetImportHistoryById, support for downloading
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<ImportReferralHistory> GetImportHistoryById(IDbConnection db, int id)
        {
            string sql = "SELECT * FROM import_referral_history WHERE id=@id";
            var result = db.Query<ImportReferralHistory>(sql, new { id }).FirstOrDefault();
            return Result<ImportReferralHistory>.Make(result, ErrorCodes.NotFound);
        }


        public bool CheckImportHistoryByFilename(IDbConnection db, string file_name)
        {
            string sql = "SELECT COUNT(*) FROM import_referral_history WHERE file_name = @file_name";
            var result = db.Query<int>(sql, new { file_name }).FirstOrDefault();
            return result > 0;
        }
    }
}
