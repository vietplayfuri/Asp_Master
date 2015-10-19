using GoPlay.Core;
using GoPlay.Models;
using GoPlay.Models.Models;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Areas.Admin.Models
{
    /// <summary>
    /// Used for shown list in index page of admin >> user
    /// </summary>
    public class AdminUserModel
    {
        public AdminUserModel()
        {
            games = new List<SimpleGame>();
            users = new List<CustomerAccountReport>();
            account_managers = new List<string>();
            timezone = "Singapore Standard Time";
        }

        public string timezone { get; set; }
        public int game_id { get; set; }
        public List<SimpleGame> games { get; set; }
        public string account_manager { get; set; }
        public List<string> account_managers { get; set; }

        public string username { get; set; }//Nick, username or email
        public string referrer { get; set; } //Referrer nick, username or email", validators=[])
        public List<CustomerAccountReport> users { get; set; }
        public string regStartTime { get; set; }//= DateTimeField(label="Registered from", validators=[])
        public string regEndTime { get; set; } //= DateTimeField(label="Until", validators=[])
        public string loginStartTime { get; set; }//= DateTimeField(label="Log in from", validators=[])
        public string loginEndTime { get; set; } //= DateTimeField(label="Until", validators=[])
        public string query { get; set; }
        public string export { get; set; }
    }


    public class AdminUserIndexModel
    {
        public AdminUserIndexModel()
        {
            transactions = new List<GeneralTransaction>();
            logs = new List<SimpleApiLog>();
            account_managers = new List<string>();
        }

        public string updateAccountManager { get; set; }
        public string accountManagerNote { get; set; }
        public string accountManager { get; set; }
        public List<string> account_managers { get; set; }
        public customer_account user { get; set; }
        public List<GeneralTransaction> transactions { get; set; }
        public List<SimpleApiLog> logs { get; set; }

        public void GetLogs(int userId)
        {
            var api = GoPlayApi.Instance;
            this.logs = api.GetLogs(userId).Data;
        }
    }
}