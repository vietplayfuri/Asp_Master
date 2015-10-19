using System;

namespace Platform.Models
{
    public class GameAccessToken : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public int game_id { get; set; }
        public string token { get; set; }
        public string data { get; set; }
        public string meta { get; set; }
        public DateTime saved_at { get; set; }
        public string stats { get; set; }
    }
}
