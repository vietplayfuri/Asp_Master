namespace Platform.Models
{

    public class CustomerLoginPassword : ModelBase
    {
        public int id { get; set; }
        public int customer_account_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string unhashed_password { get; set; }
    }

}
