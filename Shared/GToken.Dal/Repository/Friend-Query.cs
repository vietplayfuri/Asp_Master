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


namespace Platform.Dal
{
    public partial class Repo
    {
        public Result<List<string>> GetFriendNameList(IDbConnection db, string myUsername, string status)
        {
            StringBuilder strBuilder = new StringBuilder();
            List<string> lstFriend = null;
            strBuilder.Append(@"SELECT friend2_username FROM Friend 
                WHERE 
                friend1_username = @myUsername");

            if (!string.IsNullOrEmpty(status))
            {
                strBuilder.Append(@" AND status = @status");
                lstFriend = db.Query<string>(strBuilder.ToString(), new { myUsername, status }).AsList();
            }
            else
            {
                lstFriend = db.Query<string>(strBuilder.ToString(), new { myUsername }).AsList();
            }

            return Result<List<string>>.Make(lstFriend, errorIfNull: ErrorCodes.INVALID_USERNAME);
        }

        public Result<Friend> GetFriendByName(IDbConnection db, string friendName1, string friendName2)
        {
            string sql = @"SELECT * FROM Friend 
                WHERE 
                friend1_username = @friendName1
                AND 
                friend2_username = @friendName2";
            var friend = db.Query<Friend>(sql, new { friendName1, friendName2 }).FirstOrDefault();
            return Result<Friend>.Make(friend, errorIfNull: ErrorCodes.InvalidUserName);
        }


        public Result<List<Friend>> GetFriendByName(IDbConnection db, string friendName)
        {
            string sql = @"SELECT * FROM Friend 
                WHERE 
                friend1_username = @friendName";
            var friend = db.Query<Friend>(sql, new { friendName }).AsList();
            return Result<List<Friend>>.Make(friend, errorIfNull: ErrorCodes.InvalidUserName);
        }

        public Result<List<Friend>> GetFriendByNameAndStatus(IDbConnection db, string friendName, string status)
        {
            string sql = @"SELECT * FROM Friend 
                WHERE 
                friend1_username = @friendName
                AND 
                status = @status";
            var friend = db.Query<Friend>(sql, new { friendName, status }).AsList();
            return Result<List<Friend>>.Make(friend, errorIfNull: ErrorCodes.InvalidUserName);
        }

        /// <summary>
        /// Insert data into Friend table
        /// </summary>
        /// <param name="db"></param>
        /// <param name="customerAccount"></param>
        /// <returns></returns>
        public int CreateFriend(IDbConnection db, Friend friend)
        {
            string sql = @"INSERT INTO friend 
            (sent_at, status, friend1_username, friend2_username) 
            VALUES 
            (@sent_at, @status, @friend1_username, @friend2_username)";

            return db.Query<int>(sql, friend).FirstOrDefault();
        }

        /// <summary>
        /// Update friend status after get friend's response from my invitation
        /// </summary>
        /// <param name="db"></param>
        /// <param name="friend"></param>
        /// <returns></returns>
        public bool UpdateFriendStatus(IDbConnection db, string status, string friend1_username, string friend2_username)
        {
            string sql = @"UPDATE friend 
            SET status = @status
            WHERE 
                  friend1_username = @friend1_username
              AND friend2_username = @friend2_username";

            return 1 == db.Execute(sql, new { status, friend1_username, friend2_username });
        }



        public List<CustomerAccount> FindFriends(IDbConnection db, out int totalCount, string username, string keyword, int offset = 0, int count = 10)
        {
            string query = @"
            SELECT ca.id, f.status, count(1) OVER() AS full_count
            FROM customer_account ca
                LEFT JOIN friend f
                ON f.friend1_username = ca.username
                AND f.friend2_username = @username
                WHERE (lower(ca.email) LIKE '%"+ keyword + "%'"+
                    @"OR lower(ca.nickname) LIKE '%"+ keyword +"%'"+
                    @"OR lower(ca.username) LIKE '%"+ keyword +"%')"+
                    @"AND ca.username !=@username
                ORDER BY (CASE WHEN f.status = 'accepted' then 1
                    WHEN f.status = 'pending' then 2
                    WHEN f.status = 'waiting' then 3
                    WHEN f.status = 'rejected' then 5
                    ELSE 4
                    END)
                LIMIT @count
                OFFSET @offset;";
            var friendList = db.Query<FindFriendDto>(query, new { username,  keyword,  offset, count }).ToList();
            List<CustomerAccount> cusList = new List<CustomerAccount>();
            totalCount = 0;
            if (friendList.Count > 0)
            {
                totalCount = friendList[0].full_count;
                foreach (var item in friendList)
                {
                    cusList.Add(GetCustomerById(db, item.id).Data);
                }
            }
            return cusList;
        }
    }
}
