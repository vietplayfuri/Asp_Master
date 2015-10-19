using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class VenviciTransaction
    {
        public int id { get; set; }
        public string transaction_id { get; set; }
        public string customer_username { get; set; }
        public decimal gtoken_deduct_amount { get; set; }
        public decimal commission_credit_amount { get; set; }
        public decimal pushbv_amount { get; set; }
        public decimal gtoken_add_amount { get; set; }
        public string remark { get; set; }
        public DateTime created_at { get; set; }
    }
}
