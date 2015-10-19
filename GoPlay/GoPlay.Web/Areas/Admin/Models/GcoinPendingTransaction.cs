using GoPlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Areas.Admin.Models
{
    public class GcoinPendingTransaction
    {
        public List<gcoin_transaction> transactions
        {
            get;
            set;
        }
    }
}