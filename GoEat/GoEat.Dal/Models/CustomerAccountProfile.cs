namespace GoEat.Dal.Models
{
    public class CustomerAccountProfile
    {
        public int uid { get; set; }
        public string account { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string vip { get; set; }
        public string bio { get; set; }
        public string inviter { get; set; }
        public string avatar { get; set; }
        public string cover { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public decimal? gtoken { get; set; }

        //public decimal? goplay_token { get; set; }
        //public decimal? free_play_token { get; set; }
        //public decimal? free_goplay_token { get; set; }
        
        
        public bool is_venvici_member { get; set; }
    }
}
