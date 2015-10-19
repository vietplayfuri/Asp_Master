using Platform.Utility;
using System;
using System.Text;
namespace Platform.Models
{
    public class ImportReferralHistory : ModelBase
    {
        public ImportReferralHistory()
        {
            created_at = DateTime.UtcNow;
        }
        public int id { get; set; }
        public int game_id { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
        public string importer_username { get; set; }
        public string result { get; set; }
        public DateTime created_at { get; set; }
        public string game_name { get; set; }
        public string campaign_name { get; set; }

        public int referral_campaign_id { get; set; }

        #region External fields
        public ImportReferralResult ImportResult
        {
            get
            {
                if(!string.IsNullOrEmpty(result))
                    return JsonHelper.DeserializeObject<ImportReferralResult>(result);
                return null;
            }
        }
        #endregion
    }

    public class ImportReferralResult
    {
        public int total { get; set; }
        public int pass { get; set; }
        public int failed { get; set; }
        public string file_path { get; set; }
    }
}
