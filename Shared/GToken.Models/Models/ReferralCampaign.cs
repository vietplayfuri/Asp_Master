using System;
using System.Text;
namespace Platform.Models
{
    public class ReferralCampaign : ModelBase
    {
        public ReferralCampaign()
        {
            created_at = DateTime.UtcNow;
        }
        public int id { get; set; }
        public int game_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int quantity { get; set; }
        public int status { get; set; }
        public DateTime created_at { get; set; }
        public decimal gtoken_per_download { get; set; }
        public string game_name { get; set; }
        public string title { get; set; }
        public string description { get; set; }

        //extends
        public decimal gt_usage { get; set; }

        /// <summary>
        /// override the percent value of the referral campaigns
        /// </summary>
        public bool is_override { get; set; }
        /// <summary>
        /// if is_override is true, this field will be used
        /// </summary>
        public int override_value { get; set; }

        /// <summary>
        /// if this is true - show on the list and do nothing
        /// </summary>
        public bool is_display_only { get; set; }
        /// <summary>
        /// Order number to set position in the list
        /// </summary>
        public int order_number { get; set; }

        #region external fields

        /// <summary>
        /// Get real percent
        /// </summary>
        public float percent {
            get
            {
                return (float)(Math.Round(this.gt_usage / this.quantity, 2) * 100);
            }
        }
        #endregion
    }

    /// <summary>
    /// Used for dropdownlist in admin when creating referral campaign
    /// </summary>
    public class ReferralGame
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
