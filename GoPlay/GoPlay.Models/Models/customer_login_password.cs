namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    
    public class customer_login_password
    {
        public customer_login_password()
        {
        }

        public int id { get; set; }

        public int customer_account_id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string unhashed_password { get; set; }

    }
}
