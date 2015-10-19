using System;
using GoEat.Dal.Common;
using GoEat.Models;

namespace GoEat.Dal.Models
{
    public class Discount : ModelBase
    {
        public int id { get; set; }
        public string code { get; set; }
        public UserTypes user_type { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int restaurant_id { get; set; }
        public decimal rate { get; set; }
        public bool is_activated { get; set; }

    }

    /// <summary>
    /// Contain id and rate only
    /// </summary>
    public class SimpleDiscount : ModelBase
    {
        public int id { get; set; }
        public decimal rate { get; set; }
    }
}
