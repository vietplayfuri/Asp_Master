using GoPlay.Models;
using System.Data;
using Dapper;
using System.Linq;
using System.Text;
using Platform.Models;
using System.Collections.Generic;

namespace GoPlay.Dal
{
    public partial class Repo
    {
        public int CreateLogApi(IDbConnection db, api_log log)
        {
            string sql = @"INSERT INTO api_log(
            action, version, ip_address, game_id, data, message, created_at, 
            status, user_agent, country_code, customer_account_id)
            VALUES (
            @action, @version, @ip_address, @game_id, @data, @message, @created_at, 
            @status, @user_agent, @country_code, @customer_account_id)";

            return db.Query<int>(sql, log).FirstOrDefault();
        }


        public int CountCustomerAccountId(IDbConnection db, string condition)
        {
            StringBuilder strQuery = new StringBuilder();
            strQuery.Append(@"SELECT COUNT(DISTINCT customer_account_id)
                FROM api_log
                WHERE 
                game_id is not null");

            if (!string.IsNullOrEmpty(condition))
            {
                strQuery.AppendLine(" AND " + condition);
            }
            return db.Query<int>(strQuery.ToString()).FirstOrDefault();
        }

        public Result<List<SimpleApiLog>> GetSimpleApiLogs(IDbConnection db, int userId)
        {
            string sql = @"SELECT log.*, game.name AS game_name
                FROM api_log log
                LEFT JOIN game ON log.game_id = game.id
                WHERE log.customer_account_id = @userId";

            var logs = db.Query<SimpleApiLog>(sql, new { userId }).AsList();
            return Result<List<SimpleApiLog>>.Make(logs, ErrorCodes.InvalidUserId);
        }
    }
}
