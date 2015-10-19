using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GToken.Web.Models
{
    public class MainViewModel
    {
    }
    public class ShowPopupViewModel
    {
        public string popupNumber { get; set; }
    }

    public class MessageViewModel
    {
        public MessageViewModel()
        {
            message = new List<Tuple<string, string>>();
        }
        public List<Tuple<string, string>> message { get; set; }
    }

    public class DealsViewModel
    {
        public DealsViewModel() {
            //image_covers = new Dictionary<int, string>();
            //game_links = new Dictionary<int, string>();
            image_covers = new List<DealsGameModel>();
            game_links = new List<DealsGameModel>();
            icon_filenames = new Dictionary<int, string>();
        }
        //public Dictionary<int, string> image_covers { get; set; }
        //public Dictionary<int, string> game_links { get; set; }

        public Dictionary<int, string> icon_filenames { get; set; }
        public List<DealsGameModel> image_covers { get; set; }
        public List<DealsGameModel> game_links { get; set; }

        public List<ReferralCampaign> currentCamps { get; set; }
        public List<ReferralCampaign> inCommingCamp { get; set; }
    }

    public class DealsGameModel
    {
        public int game_id { get; set; }
        public string links { get; set; }
    }
}