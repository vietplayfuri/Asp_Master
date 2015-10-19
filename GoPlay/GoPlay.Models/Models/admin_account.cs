namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    public class admin_account
    {
        public int id { get; set; }
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }
        public string role { get; set; }

        public int? studio_id { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public DateTime last_login_at { get; set; }

        public bool is_active { get; set; }

        public bool is_archived { get; set; }

    }
}
