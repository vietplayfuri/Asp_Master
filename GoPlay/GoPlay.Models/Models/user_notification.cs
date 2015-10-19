namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class user_notification
    {
        public int id { get; set; }

        public int customer_account_id { get; set; }

        public int? game_notification_id { get; set; }

        public DateTime created_at { get; set; }

        public bool is_archived { get; set; }

    }

    /// <summary>
    /// Used to get notification for user with specific game
    /// </summary>
    public class game_user_notification
    {
        /// <summary>
        /// notification_id
        /// </summary>
        public int id { get; set; }
        public DateTime created_at { get; set; }
        public bool is_archived { get; set; }
        public int? good_id { get; set; }
        public int? good_type { get; set; }
        public int? good_amount { get; set; }
    }
}
