using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Platform.Models;

namespace GToken.Models
{
    public class CreateReferralViewModel : IValidatableObject
    {

        [Required]
        public int? game_id { get; set; }
        public string game_name { get; set; }
        [Required]
        public DateTime? start_date { get; set; }
        [Required]
        public DateTime? end_date { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int? quantity { get; set; }
        [Required]
        [Range(0.001, Double.MaxValue)]
        public decimal? gtoken_per_download { get; set; }
        [Required]
        public string title { get; set; }
        public string description { get; set; }

        /// <summary>
        /// override the percent value of the referral campaigns
        /// </summary>
        public string is_override { get; set; }
        /// <summary>
        /// if is_override is true, this field will be used
        /// </summary>
        [Range(0, 100)]
        public int override_value { get; set; }

        /// <summary>
        /// if this is true - show on the list and do nothing
        /// </summary>
        public string is_display_only { get; set; }
        /// <summary>
        /// Order number to set position in the list
        /// </summary>
        public int order_number { get; set; }

        public List<ReferralGame> games { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            TimeSpan span = this.end_date.Value.Subtract(this.start_date.Value);
            if (span.TotalHours < 0)
            {
                yield return new ValidationResult("End date must greater than Start date", new[] { "end_date" });
            }
        }
    }

    public class EditReferralViewModel : IValidatableObject
    {
        public int id { get; set; }

        [Required]
        public int? game_id { get; set; }
        public string game_name { get; set; }
        [Required]
        public DateTime? start_date { get; set; }
        [Required]
        public DateTime? end_date { get; set; }
        [Required]
        [Range(1, Int32.MaxValue)]
        public int? quantity { get; set; }
        [Required]
        [Range(0.001, Double.MaxValue)]
        public decimal? gtoken_per_download { get; set; }
        [Required]
        public int? status { get; set; }
        [Required]
        public string title { get; set; }
        public string description { get; set; }

        /// <summary>
        /// override the percent value of the referral campaigns
        /// </summary>
        public string is_override { get; set; }
        /// <summary>
        /// if is_override is true, this field will be used
        /// </summary>
        [Range(0, 100)]
        public int override_value { get; set; }

        /// <summary>
        /// if this is true - show on the list and do nothing
        /// </summary>
        public string is_display_only { get; set; }
        /// <summary>
        /// Order number to set position in the list
        /// </summary>
        public int order_number { get; set; }

        public List<ReferralGame> games { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            TimeSpan span = this.end_date.Value.Subtract(this.start_date.Value);
            if (span.TotalHours < 0)
            {
                yield return new ValidationResult("End date must greater than Start date", new[] { "end_date" });
            }

        }
    }

    public class DownloadHistoryViewModel
    {
        public DownloadHistoryViewModel()
        {
            timeZone = "Singapore Standard Time";
        }
        public int? game_id { get; set; }
        public string username { get; set; }
        public string referrer { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
        public string timeZone { get; set; }
        public string query { get; set; }
        public string export { get; set; }
        public int? campaign_id { get; set; }
        public List<ReferralGame> games { get; set; }
        public List<ReferralCampaign> campaigns { get; set; }
    }

    public class SearchReferalViewModel
    {
        public SearchReferalViewModel()
        {
        }
        public int? game_id { get; set; }
        public string username { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int? page { get; set; }
        //
        public List<ReferralGame> games { get; set; }
        public double? time_zone { get; set; }


    }

    public class OrderReferralCampaign
    {
        public int source_id { get; set; }
        public int destination_id { get; set; }
    }

    public class ImportReferralModel
    {
        [Required]
        public int game_id { get; set; }
    }


}