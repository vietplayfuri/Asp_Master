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
        public int CreateLogApi(IDbConnection db, ApiLog log)
        {
            string sql = @"INSERT INTO api_log 
            (action, version, user_agent, status, message, ip_address, country_code, data, customer_username, partner_identifier) 
            VALUES 
            (@action, @version, @user_agent, @status, @message, @ip_address, @country_code, @data, @customer_username, @partner_identifier)";

            return db.Query<int>(sql, log).FirstOrDefault();
        }
    }
}
