using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Platform.Utility;
using Platform.Models;

namespace GToken.Web.Models
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

        public List<string> statusList = new List<string>() { 
            Helper.GetDescription(TransactionStatus.Success),
            Helper.GetDescription(TransactionStatus.Pending),
            Helper.GetDescription(TransactionStatus.Failure),
            Helper.GetDescription(TransactionStatus.Cancelled)};
        public List<string> partners;

        public string gtoken_transaction_id { get; set; }
        public string username { get; set; }
        public string status { get; set; }
        public string partner_identifier { get; set; }
        public string partner_order_id { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string query { get; set; }
        public string export { get; set; }
    }

    public class UserDetail
    {
        public CustomerAccount user { get; set; }
        public List<Transaction> transactions { get; set; }
    }
}