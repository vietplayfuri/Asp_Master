using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIProxy.Web.Models
{
    public class GoPlayApiModel
    {
    }

    /// <summary>
    /// Used for /partner/0/purchase-play-token
    /// Input: username / status / token / amount
    /// Output: success / message / error_code
    /// </summary>
    public class APIPurchasePlayTokenModel
    {
        public string username { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public string amount { get; set; }
    }
}