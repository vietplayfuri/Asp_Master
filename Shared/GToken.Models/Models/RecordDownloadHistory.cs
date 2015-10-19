using System;
using System.Text;
namespace Platform.Models
{
    public class RecordDownloadHistory : ModelBase
    {
        public RecordDownloadHistory()
        {
            created_at = DateTime.UtcNow;
        }
        public int id { get; set; }
        public int game_id { get; set; }
        public int user_id { get; set; }
        public string device_id { get; set; }
        public int referral_campaign_id { get; set; }
        public string referral_title { get; set; }
        public DateTime created_at { get; set; }
        public string earned_username { get; set; }
        #region External Fields
        public string game_name { get; set; }
        public string username { get; set; }
        public string inviter_username { get; set; }
        public decimal gtoken_per_download { get; set; }
        #endregion
    }
}
