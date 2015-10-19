using GoEat.Models;

namespace GoEat.Dal.Models
{
    public class CreditBalance : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public decimal token { get; set; }
    }
}
