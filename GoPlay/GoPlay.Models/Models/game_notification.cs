namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class game_notification
    {
        public game_notification()
        {
        }

        public int id { get; set; }

        public int game_id { get; set; }

        public int? good_id { get; set; }

        public int? good_amount { get; set; }

        public int? good_type { get; set; }

        public string description { get; set; }

    }
}
