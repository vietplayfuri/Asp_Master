using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Platform.Utility;
using Platform.Models;
using GoPlay.Models;
using GoPlay.Models.Models;

namespace GoPlay.Web.Areas.Admin.Models
{
    public class AdminViewModel
    {
        public string timeZone { get; set; }
        public AdminViewModel()
        {
            timeZone = "Singapore Standard Time";
        }
    }

    public class UserQueryForm : AdminViewModel
    {
        public UserQueryForm()
            : base()
        {
        }
        public string username { get; set; }//Nick, username or email
        public string referrer { get; set; } //Referrer nick, username or email", validators=[])
        //public List<Dictionary<string,string>> source { get; set; }//= SelectField(u"Source", coerce=unicode, validators=[])
        public string source { get; set; }
        public string regStartTime { get; set; }//= DateTimeField(label="Registered from", validators=[])
        public string regEndTime { get; set; } //= DateTimeField(label="Until", validators=[])
        public string loginStartTime { get; set; }//= DateTimeField(label="Log in from", validators=[])
        public string loginEndTime { get; set; } //= DateTimeField(label="Until", validators=[])
        public string query { get; set; }
        public string export { get; set; }
    }
    public class TransactionQueryForm : AdminViewModel
    {
        public TransactionQueryForm()
            : base()
        {
        }

        public List<string> statusList = new List<string>() { "success", "pending", "failure"};
        public List<Game> games { get; set; }

        public string orderID { get; set; }
        public string username { get; set; }
        public string status { get; set; }
        public int gameID { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string query { get; set; }
        public string export { get; set; }

        public List<AdminGeneralTransaction> transactions { get; set; }

        public List<gcoin_transaction> gcoinTransaction { get; set; }


    }

    public class UserDetail
    {
        public CustomerAccount user { get; set; }
        public List<Transaction> transactions { get; set; }
    }

    public class CardQueryForm : AdminViewModel
    {
        public string username { get; set; }
        public string usageStartTime { get; set; }
        public string usageEndTime { get; set; }
        public string cardNumber { get; set; }
        public bool isFree { get; set; }
        public List<string> statusList = new List<string>() { "used", "unused" };
        public string status { get; set; }
        public string query { get; set; }
        public string export { get; set; }

        public List<topup_card> cards { get; set; }

    }
}