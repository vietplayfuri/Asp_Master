using Platform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GToken.Areas.Admin.Models
{
    public class ImportReferalViewModel
    {
        public ImportReferalViewModel()
        {
        }
        //[Required]
        //public int game_id { get; set; }

        [Required]
        public int campaign_id { get; set; }
        //public List<Platform.Models.ReferralGame> games { get; set; }
        public List<Platform.Models.ReferralCampaign> referral_campaigns { get; set; }
        public string time_zone { get; set; }

        public ImportReferralHistory ImportResult { get; set; }
        public List<string> errorMsg { get; set; }
    }

    public class SearchImportViewModel
    {
        public SearchImportViewModel()
        {
            time_zone = "Singapore Standard Time";
        }
        public int? game_id { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        //public int? page { get; set; }
        //
        public List<ReferralGame> games { get; set; }
        public string time_zone { get; set; }

    }
}