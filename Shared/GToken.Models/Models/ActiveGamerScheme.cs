namespace Platform.Models
{
    public class ActiveGamerScheme : ModelBase
    {
        public int customer_account_id { get; set; }
        public int inviter_id { get; set; }
        public decimal? balance { get; set; }
        public bool is_archived { get; set; }
        public string reward { get; set; }

    }

  
}
