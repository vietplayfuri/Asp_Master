using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Models.Models
{
    public class GTokenTransaction
    {
        public int id { get; set; }
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public decimal price { get; set; }
        public decimal final_amount { get; set; }
        public string currency { get; set; }
        public decimal original_price { get; set; }
        public decimal original_final_amount { get; set; }
        public string original_currency { get; set; }
        public decimal discount_percentage { get; set; }
        public string status { get; set; }
        public string payment_method { get; set; }
        public string description { get; set; }
        public bool? is_venvici_applicable { get; set; }
        public decimal original_tax { get; set; }
        public decimal original_service_charge { get; set; }
        public decimal revenue_percentage { get; set; }
    }

    public class GTokenTransactionModel
    {
        public GTokenTransaction transaction { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
        public string session { get; set; }
    }
}
