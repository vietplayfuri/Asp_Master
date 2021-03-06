﻿using System;
using System.Data;
using System.Linq;
using System.Net;
using Dapper;
using Platform.Models;
using Platform.Utility;
using GoPlay.Models;
namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<customer_account> Login(IDbConnection db, string userName, string pwd, IPAddress ip)
        {
            string userNameLC = userName.ToLower();

            var loginInfo = db.Query<customer_login_password>("SELECT * FROM customer_login_password WHERE lower(username)=@userName OR lower(email)=@userName ", new { userName = userNameLC }).FirstOrDefault();

            if (loginInfo == null || string.Compare(loginInfo.unhashed_password, pwd, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return Result<customer_account>.Null(ErrorCodes.InvalidUserNameOrPassword);
            }

            var user = GetCustomerById(db, loginInfo.customer_account_id);
            int id = user.Data.id;

            // Change country code only IF the database has not store this value before //
            if (string.IsNullOrEmpty(user.Data.country_code) || string.IsNullOrEmpty(user.Data.country_name))
            {
                ip.GetCountryCode(c => user.Data.country_code = c, n => user.Data.country_name = n);
            }
          

            // TODO: Update
            user.Data.last_login_at = DateTime.UtcNow;
            db.Execute("UPDATE customer_account SET last_login_at=@last_login_at, country_code=@country_code, country_name=@country_name", user.Data);

            return user;
        }

    }
}
