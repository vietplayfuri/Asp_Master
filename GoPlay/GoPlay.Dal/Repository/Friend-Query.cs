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

namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<List<int>> GetFriendNameList(IDbConnection db, int friend1_id, string status)
        {
            StringBuilder strBuilder = new StringBuilder();
            List<int> lstFriend = null;
            strBuilder.Append(@"SELECT friend2_id FROM friend 
                WHERE 
                friend1_id = @friend1_id");

            if (!string.IsNullOrEmpty(status))
                strBuilder.Append(@" AND status = @status");

            lstFriend = db.Query<int>(strBuilder.ToString(), new { friend1_id, status }).AsList();

            return Result<List<int>>.Make(lstFriend, errorIfNull: ErrorCodes.NON_EXISTING_USER);
        }

        public Result<friend> GetFriendById(IDbConnection db, int myId, int friendId)
        {
            string sql = @"SELECT * FROM friend 
                WHERE 
                friend1_id = @myId
                AND 
                friend2_id = @friendId";
            var friend = db.Query<friend>(sql, new { myId, friendId }).FirstOrDefault();
            return Result<friend>.Make(friend, errorIfNull: ErrorCodes.NON_EXISTING_USER);
        }


        public Result<List<friend>> GetFriendByFriend1Id(IDbConnection db, int friend1_id)
        {
            string sql = @"SELECT * FROM friend 
                WHERE 
                friend1_id = @friend1_id";
            var friend = db.Query<friend>(sql, new { friend1_id }).AsList();
            return Result<List<friend>>.Make(friend, errorIfNull: ErrorCodes.NON_EXISTING_USER);
        }

        public Result<List<friend>> GetFriendByIdAndStatus(IDbConnection db, int friend1_id, string status)
        {
            string sql = @"SELECT * FROM friend 
                WHERE 
                friend1_id = @friend1_id
                AND 
                status = @status";
            var friend = db.Query<friend>(sql, new { friend1_id, status }).AsList();
            return Result<List<friend>>.Make(friend, errorIfNull: ErrorCodes.NON_EXISTING_USER);
        }

        /// <summary>
        /// Insert data into friend table
        /// </summary>
        /// <param name="db"></param>
        /// <param name="customerAccount"></param>
        /// <returns></returns>
        public int CreateFriend(IDbConnection db, friend friend)
        {
            string sql = @"INSERT INTO friend 
            (sent_at, status, friend1_id, friend2_id) 
            VALUES 
            (@sent_at, @status, @friend1_id, @friend2_id)";

            return db.Query<int>(sql, friend).FirstOrDefault();
        }

        public bool UpdateFriendStatus(IDbConnection db, int friend1_id,
            int friend2_id, string status)
        {
            string sql = @"UPDATE friend 
            SET status = @status 
            WHERE 
                  friend1_id = @friend1_id
              AND friend2_id = @friend2_id";

            return 1 == db.Execute(sql, new { friend1_id, friend2_id, status });
        }

        public List<SimpleCustomerAccount> FindFriends(IDbConnection db,
            out int totalCount, int userId, string keyword, int offset = 0, int count = 10)
        {
            var encodeForLike = string.IsNullOrEmpty(keyword) 
                ? string.Empty
                : keyword.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
            string term = "%" + encodeForLike + "%";
            string query = @"
            SELECT ca.id, ca.nickname, ca.username, ca.avatar_filename, f.status, count(1) OVER() AS full_count
            FROM customer_account ca
            LEFT JOIN friend f ON 
                f.friend1_id = ca.id
                AND f.friend2_id = @userId
            WHERE 
                (lower(ca.email) LIKE @term 
                   OR lower(ca.nickname) LIKE @term 
                   OR lower(ca.username) LIKE @term) 
                AND ca.id != @userId
            ORDER BY (CASE WHEN f.status = 'accepted' then 1
                      WHEN f.status = 'pending' then 2
                      WHEN f.status = 'waiting' then 3
                      WHEN f.status = 'rejected' then 5
                      ELSE 4
                      END)
            LIMIT @count
            OFFSET @offset;";

            var friendList = db.Query<SimpleCustomerAccount>(query, new { userId, term, offset, count }).ToList();
            totalCount = (friendList != null && friendList.Any()) 
                ? friendList[0].full_count
                : 0;
            return friendList;
        }

        public Result<List<customer_account>> GetFriendList(IDbConnection db, int friend1_id, string status = null)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"SELECT * FROM customer_account
                WHERE id in (
                    SELECT friend2_id FROM friend
                    WHERE friend1_id = @friend1_id");
            if (!string.IsNullOrEmpty(status))
                strBuilder.Append(@" AND status = @status");
            strBuilder.Append(@")");

            var lstFriend = db.Query<customer_account>(strBuilder.ToString(), new { friend1_id, status }).AsList();
            return Result<List<customer_account>>.Make(lstFriend, errorIfNull: ErrorCodes.NON_EXISTING_USER);
        }


        public Result<List<SimpleCustomerAccount>> GetSimpleFriendList(IDbConnection db, int friend1_id, string status = null)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine(@"SELECT id, username, nickname, avatar_filename FROM customer_account ca
                JOIN friend f ON ca.id = f.friend2_id
                WHERE friend1_id = @friend1_id");
            if (!string.IsNullOrEmpty(status))
                strBuilder.Append(@" AND status = @status");

            var lstFriend = db.Query<SimpleCustomerAccount>(strBuilder.ToString(), new { friend1_id, status }).AsList();
            return Result<List<SimpleCustomerAccount>>.Make(lstFriend, errorIfNull: ErrorCodes.NON_EXISTING_USER);
        }
    }
}
