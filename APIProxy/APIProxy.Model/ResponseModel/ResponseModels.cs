using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Model
{
    /// <summary>
    /// Receive response from api Goplay
    /// </summary>
    public class ResultResponse
    {
        public bool success { get; set; }
        public string error_code { get; set; }
        public string message { get; set; }
        public string session { get; set; }
        public Profile profile { get; set; }
        public List<object> notifications { get; set; }
        public string Error { get; set; }
        public dynamic status { get; set; }
        public List<Profile> UserData { get; set; }

        //Two fields for in-app-purchase api
        public object exchange { get; set; }
        public string transaction_id { get; set; }

        //Two fields used for queryIds
        public dynamic list { get; set; }
        public string user_id { get; set; }

        //thirdPartyLogin
        public string onlyID { get; set; }
        /// <summary>
        /// Used for check-oauth-connection
        /// </summary>
        public string bindonlyID { get; set; }
    }

    public class ResultNotificationModel
    {
        public int id { get; set; }
        public string addTime { get; set; }
        public int state { get; set; }
        public int serverID { get; set; }
        public int? goodType { get; set; }
        public int? goodID { get; set; }
        public int? goodAmount { get; set; }
    }
    public class BlobModel
    {
        public string uid { get; set; }
        public string otherID { get; set; }
    }
}

