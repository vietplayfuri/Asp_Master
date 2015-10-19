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
        /// Check RecordDownloadHistory exist or not
        /// </summary>
        public bool CheckRecordDownloadHistory(IDbConnection db, int referralCampaignId, int gameId, string deviceId)
        {
            var isExist = db.Query<bool>(@"SELECT EXISTS(
                SELECT id FROM record_download_history
                WHERE referral_campaign_id = @referralCampaignId
                  AND game_id = @gameId
                  AND device_id = @deviceId)", new { referralCampaignId, gameId, deviceId }).FirstOrDefault();
            return isExist;
        }


        /// <summary>
        /// Get RecordDownloadHistory
        /// </summary>
        public Result<RecordDownloadHistory> GetRecordDownloadHistory(IDbConnection db, int referralCampaignId, int gameId, int userId, string deviceId)
        {
            var recordDownloadHistory = db.Query<RecordDownloadHistory>(@"SELECT * FROM record_download_history
                WHERE referral_campaign_id = @referralCampaignId
                  AND game_id = @gameId
                  AND user_id = @userId
                  AND device_id = @deviceId", new { referralCampaignId, gameId, userId, deviceId }).FirstOrDefault();
            return Result<RecordDownloadHistory>.Make(recordDownloadHistory, errorIfNull: ErrorCodes.NotFound);
        }


        /// <summary>
        /// count RecordDownloadHistory by gameId
        /// </summary>
        /// <returns></returns>
        public int CountRecordDownloadHistory(IDbConnection db, int referralCampaignId)
        {
            return db.Query<int>(@"SELECT COUNT(id) FROM record_download_history
                WHERE referral_campaign_id = @referralCampaignId", new { referralCampaignId }).FirstOrDefault();
        }



        public int CreateRecordDownloadHistory(IDbConnection db, RecordDownloadHistory recordDownloadHistory)
        {
            string sql = @"INSERT INTO record_download_history
                   (game_id, user_id, device_id, referral_campaign_id, created_at, earned_username)
            VALUES (@game_id, @user_id, @device_id, @referral_campaign_id, @created_at, @earned_username)
            RETURNING id";

            return db.Query<int>(sql, recordDownloadHistory).FirstOrDefault();
        }



        /// <summary>
        /// Get RecordDownloadHistory for admin
        /// </summary>
        public Result<List<RecordDownloadHistory>> GetRecordDownloadHistory(IDbConnection db, int? campaign_id= null, int? gameId = null, string username = null, string referrerUsername = null,
            DateTime? from = null, DateTime? to = null, int? skip = null, int? take = null)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"SELECT rdh.*, rc.game_name, ca.username, ca.inviter_username, rc.gtoken_per_download, rc.title as referral_title
                FROM record_download_history rdh
                JOIN customer_account ca on rdh.user_id = ca.id
                JOIN referral_campaign rc on rc.id = rdh.referral_campaign_id
                WHERE 1 = 1");

            string termUsername = string.Empty;
            string termreferrerUsername = string.Empty;

            if (!string.IsNullOrEmpty(username))
            {
                termUsername = "%" + username + "%";
                sqlBuilder.AppendLine("AND ca.username like @termUsername");
            }
            if (!string.IsNullOrEmpty(referrerUsername))
            {
                termreferrerUsername = "%" + referrerUsername + "%";
                sqlBuilder.AppendLine("AND ca.inviter_username like @termreferrerUsername");
            }
            if (gameId.HasValue)
                sqlBuilder.AppendLine("AND rdh.game_id = @gameId");
            if (campaign_id.HasValue)
                sqlBuilder.AppendLine("AND rdh.referral_campaign_id = @campaign_id");
            if (to.HasValue && to.Value >= SqlDateTime.MinValue.Value)
                sqlBuilder.AppendLine("AND rdh.created_at <= @to");
            if (from.HasValue && from.Value >= SqlDateTime.MinValue.Value)
                sqlBuilder.AppendLine("AND rdh.created_at >= @from");

            sqlBuilder.AppendLine("ORDER BY rdh.id DESC");

            if (skip.HasValue && take.HasValue && skip.Value >= 0 && take.Value > 0)
                sqlBuilder.AppendLine("OFFSET @skip LIMIT @take");

            string sqlQuery = sqlBuilder.ToString();
            var recordDownloadHistories = db.Query<RecordDownloadHistory>(sqlQuery, new
            {
                termUsername,
                termreferrerUsername,
                campaign_id,
                gameId,
                to,
                from,
                skip,
                take
            }).AsList();

            return Result<List<RecordDownloadHistory>>.Make(recordDownloadHistories, ErrorCodes.NotFound);
        }


        /// <summary>
        /// Get all record download history for user
        /// </summary>
        public Result<List<RecordDownloadHistory>> GetRecordDownloadHistory(IDbConnection db, string earned_username, int? gameId = null, string username = null,
            DateTime? from = null, DateTime? to = null, int? skip = null, int? take = null)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"SELECT rdh.*, rc.game_name, rc.gtoken_per_download, ca.username
                FROM record_download_history rdh                
                JOIN referral_campaign rc on rc.id = rdh.referral_campaign_id                
                JOIN customer_account ca on rdh.user_id = ca.id
                WHERE rdh.earned_username=@earned_username");

            string termUsername = string.Empty;
            if (!string.IsNullOrEmpty(username))
            {
                termUsername = "%" + username + "%";
                sqlBuilder.AppendLine("AND (ca.username like @termUsername OR ca.inviter_username like @termUsername)");
            }

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
            var recordDownloadHistories = db.Query<RecordDownloadHistory>(sqlQuery, new
            {
                earned_username,
                termUsername,
                gameId,
                to,
                from,
                skip,
                take
            }).AsList();

            return Result<List<RecordDownloadHistory>>.Make(recordDownloadHistories, ErrorCodes.NotFound);
        }


        /// <summary>
        /// Count all record download history for user
        /// </summary>
        public int CountRecordDownloadHistory(IDbConnection db, string referrerUsername, int? gameId = null, string username = null,
            DateTime? from = null, DateTime? to = null)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"SELECT COUNT(1)
                FROM record_download_history rdh                
                JOIN referral_campaign rc on rc.id = rdh.referral_campaign_id
                JOIN customer_account ca on rdh.user_id = ca.id
                WHERE ca.inviter_username = @referrerUsername");

            string termUsername = string.Empty;
            if (!string.IsNullOrEmpty(username))
            {
                termUsername = "%" + username + "%";
                sqlBuilder.AppendLine("AND ca.username like @termUsername");
            }

            if (gameId.HasValue)
                sqlBuilder.AppendLine("AND rdh.game_id = @gameId");
            if (to.HasValue && to.Value >= SqlDateTime.MinValue.Value)
                sqlBuilder.AppendLine("AND rdh.created_at <= @to");
            if (from.HasValue && from.Value >= SqlDateTime.MinValue.Value)
                sqlBuilder.AppendLine("AND rdh.created_at >= @from");

            string sqlQuery = sqlBuilder.ToString();
            var count = db.Query<int>(sqlQuery, new
            {
                referrerUsername,
                termUsername,
                gameId,
                to,
                from
            }).FirstOrDefault();

            return count;
        }


        /// <summary>
        /// Get Total referral money for user
        /// </summary>
        public decimal GetTotalReferralMoney(IDbConnection db, string referrerUsername)
        {
            string sqlQuery =
              @"SELECT COALESCE(SUM(rc.gtoken_per_download), 0) 
                FROM   record_download_history rdh
                JOIN   referral_campaign rc on rc.id = rdh.referral_campaign_id
                JOIN   customer_account ca on rdh.user_id = ca.id
                WHERE  ca.inviter_username = @referrerUsername";

            var count = db.Query<decimal>(sqlQuery, new { referrerUsername }).FirstOrDefault();
            return count;
        }
    }
}
