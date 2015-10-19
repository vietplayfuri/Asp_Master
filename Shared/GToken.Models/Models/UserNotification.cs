using System;

namespace Platform.Models
{
    public class UserNotification : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public int? game_notification_id { get; set; }
        public DateTime created_at { get; set; }
        public bool is_archived { get; set; }

    }

    /// <summary>
    /// Response model when calling get notification apis
    /// Used in 2 places: get-notification in goplay and proxy
    /// </summary>
    public class ReponseGetNotificationModel
    {
        public int id { get; set; }
        public string created_at { get; set; }
        public bool is_archived { get; set; }
        public int? good_type { get; set; }
        public int? good_id { get; set; }
        public int? good_amount { get; set; }
    }
}
