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


namespace Platform.Dal
{
    public partial class Repo
    {
        /// <summary>
        /// Get ReferralCampaign 
        /// </summary>
        /// <returns></returns>
        public ReferralCampaign GetReferralCampaign(IDbConnection db, int gameId, DateTime date, int status)
        {
            var referralCampaign = db.Query<ReferralCampaign>(@"SELECT * FROM referral_campaign
                WHERE 
                      status = @status
                  AND game_id = @gameId
                  AND is_display_only = false
                  AND start_date <= @date
                  AND end_date >= @date", new { gameId, date, status }).FirstOrDefault();
            return referralCampaign;
        }

        public ReferralCampaign GetReferralCampaign(IDbConnection db, List<int> status, int id)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"SELECT * FROM referral_campaign WHERE id = @id AND (is_display_only=True OR status in (" + string.Join(",", status) + @"))");
            var referralCampaign = db.Query<ReferralCampaign>(sqlBuilder.ToString(), new { id, status }).FirstOrDefault();
            return referralCampaign;
        }

        public int CreateReferralCampaign(IDbConnection db, ReferralCampaign referralCampaign)
        {
            string sql = @"INSERT INTO referral_campaign
                    (game_id, start_date, end_date, quantity, status, gtoken_per_download, created_at, game_name, title, description, is_override, override_value, is_display_only, order_number)
            VALUES  (@game_id, @start_date, @end_date, @quantity, @status, @gtoken_per_download, @created_at, @game_name, @title, @description, @is_override, @override_value, @is_display_only, @order_number)
            RETURNING id";

            return db.Query<int>(sql, referralCampaign).FirstOrDefault();
        }

        /// <summary>
        /// Get all referral campaigns
        /// </summary>
        /// <returns></returns>
        public Result<List<ReferralCampaign>> GetReferralCampaigns(IDbConnection db, int? status = null)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            DateTime now = DateTime.Now;
            sqlBuilder.AppendLine(@"SELECT *, (SELECT COUNT(*) FROM record_download_history WHERE record_download_history.referral_campaign_id = referral_campaign.id) AS gt_usage
                      FROM referral_campaign WHERE 1 = 1 ");

            if (status.HasValue)
            {
                sqlBuilder.AppendLine("AND ((status = @status)");
                sqlBuilder.AppendLine("OR (start_date <= @now");
                sqlBuilder.AppendLine("AND end_date >= @now");
                sqlBuilder.AppendLine("AND status = " + (int)ReferralCampaignStatus.Inactive);
                sqlBuilder.AppendLine("AND is_display_only = true))");
            }

            sqlBuilder.AppendLine(" ORDER BY referral_campaign.order_number, referral_campaign.start_date DESC");

            string sqlQuery = sqlBuilder.ToString();
            var referralCampaign = db.Query<ReferralCampaign>(sqlQuery, new { status, now }).AsList();

            return Result<List<ReferralCampaign>>.Make(referralCampaign, ErrorCodes.NotFound);
        }


        /// <summary>
        /// Get all referral campaigns that have time is match to start to finish
        /// </summary>
        /// <paparam name="status">Status that will be gotten</paparam>
        public Result<List<ReferralCampaign>> GetReferralCampaigns(IDbConnection db, List<int> status)
        {
            string sqlQuery = @"SELECT id, start_date, end_date, status, is_display_only
                FROM referral_campaign WHERE status in (" + string.Join(",", status) + ")";
            var referralCampaign = db.Query<ReferralCampaign>(sqlQuery).AsList();
            return Result<List<ReferralCampaign>>.Make(referralCampaign, ErrorCodes.NotFound);
        }

        public Result<List<ReferralCampaign>> GetCurrentReferralCampaigns(IDbConnection db, List<int> status)
        {
            string sqlQuery = @"SELECT *, (SELECT COUNT(*) FROM record_download_history WHERE record_download_history.referral_campaign_id = referral_campaign.id) AS gt_usage
                FROM referral_campaign WHERE is_display_only=True OR status in (" + string.Join(",", status) + @")
                ORDER BY order_number";
            var referralCampaign = db.Query<ReferralCampaign>(sqlQuery).AsList();
            return Result<List<ReferralCampaign>>.Make(referralCampaign, ErrorCodes.NotFound);
        }


        public Result<List<ReferralCampaign>> GetInCommingReferralCampaigns(IDbConnection db, List<int> status)
        {
            string sqlQuery = @"SELECT *, (SELECT COUNT(*) FROM record_download_history WHERE record_download_history.referral_campaign_id = referral_campaign.id) AS gt_usage
                FROM referral_campaign WHERE start_date > now() AND status in (" + string.Join(",", status) + @")
                ORDER BY order_number";
            var referralCampaign = db.Query<ReferralCampaign>(sqlQuery).AsList();
            return Result<List<ReferralCampaign>>.Make(referralCampaign, ErrorCodes.NotFound);
        }

        public bool UpdateReferralCampaigns(IDbConnection db, List<int> campaignId, int status)
        {
            string sql = @"UPDATE referral_campaign
                SET status=@status
                WHERE id in (" + string.Join(",", campaignId) + ")";

            return db.Execute(sql, new { status }) > 0;
        }

        public bool UpdateReferralCampaign(IDbConnection db, ReferralCampaign referralCampaign)
        {
            string sql = @"UPDATE referral_campaign
                SET game_id=@game_id
                    , start_date=@start_date
                    , end_date=@end_date
                    , quantity=@quantity
                    , status=@status
                    , gtoken_per_download=@gtoken_per_download
                    , game_name=@game_name
                    , title=@title
                    , description=@description
                    , is_override = @is_override
                    , override_value=@override_value
                    , is_display_only=@is_display_only
                    , order_number=@order_number
    
                WHERE id = @id";

            return 1 == db.Execute(sql, referralCampaign);
        }


        public Result<ReferralCampaign> GetReferralCampaignById(IDbConnection db, int referralCampaignId)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"SELECT *, (SELECT COUNT(*) FROM record_download_history WHERE record_download_history.referral_campaign_id = referral_campaign.id) AS gt_usage
                                    FROM referral_campaign WHERE id =@referralCampaignId");
            string sqlQuery = sqlBuilder.ToString();
            var referralCampaign = db.Query<ReferralCampaign>(sqlQuery, new { referralCampaignId }).FirstOrDefault();

            return Result<ReferralCampaign>.Make(referralCampaign, ErrorCodes.NotFound);
        }


        public bool DeleteReferralCampaign(IDbConnection db, int referralCampaignId)
        {
            var sqlString = @"DELETE FROM referral_campaign WHERE id=@referralCampaignId";
            return 1 == db.Execute(sqlString, new { referralCampaignId });
        }


        public Result<List<ReferralCampaign>> GetListValidCompaignOfGame(IDbConnection db, int game_id)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"select * from referral_campaign WHERE game_id=@game_id AND status NOT IN (3,4)
                                   order by created_at DESC");

            string sqlQuery = sqlBuilder.ToString();
            var referralCampaign = db.Query<ReferralCampaign>(sqlQuery, new { game_id }).ToList();

            return Result<List<ReferralCampaign>>.Make(referralCampaign, ErrorCodes.NotFound);
        }


        public int CreateImportReferralHistory(IDbConnection db, ImportReferralHistory importReferral)
        {
            string sql = @"INSERT INTO import_referral_history(
                    created_at, game_id, file_name, file_path, importer_username, referral_campaign_id,result)
            VALUES (@created_at, @game_id, @file_name, @file_path, @importer_username,@referral_campaign_id,@result)
            RETURNING id";

            return db.Query<int>(sql, importReferral).FirstOrDefault();
        }
    }
}
