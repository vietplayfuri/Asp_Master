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
using Platform.Utility;

namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<active_gamer_scheme> GetActiveGamerScheme(IDbConnection db, int customerId)
        {
            string sqlQuery = @"SELECT * FROM active_gamer_scheme
                WHERE customer_account_id = @customerId";

            var activeGamer = db.Query<active_gamer_scheme>(sqlQuery, new { customerId }).FirstOrDefault();
            return Result<active_gamer_scheme>.Make(activeGamer, ErrorCodes.InvalidUserId);
        }


        public bool UpdateActiveGamerScheme(IDbConnection db, active_gamer_scheme activeGamerScheme)
        {
            string sqlQuery = @"UPDATE active_gamer_scheme
                SET 
                    balance = @balance,
                    is_archived = @is_archived,
                    reward = @reward
                WHERE 
                    customer_account_id = @customer_account_id
                AND inviter_id = @inviter_id";

            return 1 == db.Execute(sqlQuery, new
            {
                customer_account_id = activeGamerScheme.customer_account_id,
                inviter_id = activeGamerScheme.inviter_id,
                balance = activeGamerScheme.balance.Value,
                is_archived = activeGamerScheme.is_archived,
                reward = activeGamerScheme.reward,
            });
        }


        public bool CreateActiveGamerScheme(IDbConnection db, active_gamer_scheme activeGamerScheme)
        {
            string sqlQuery = @"INSERT INTO active_gamer_scheme
                    (customer_account_id, inviter_id, balance, is_archived, reward)
                VALUES
                    (@customer_account_id, @inviter_id, @balance, @is_archived, @reward)";

            return 1 == db.Execute(sqlQuery, new {
                customer_account_id = activeGamerScheme.customer_account_id,
                inviter_id = activeGamerScheme.inviter_id,
                balance = activeGamerScheme.balance.Value,
                is_archived = activeGamerScheme.is_archived,
                reward = activeGamerScheme.reward,
            });
        }
    }
}
