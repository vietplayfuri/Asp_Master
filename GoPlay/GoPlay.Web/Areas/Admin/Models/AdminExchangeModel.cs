using GoPlay.Core;
using GoPlay.Models;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Areas.Admin.Models
{
    /// <summary>
    /// Used for shown list in index page of exchange
    /// </summary>
    public class AdminExchangeModel
    {
        public AdminExchangeModel()
        {
            CreditTypes = new List<CreditType>();
            Packages = new List<Package>();
        }
        public List<CreditType> CreditTypes { get; set; }
        public List<Package> Packages { get; set; }
    }

    /// <summary>
    /// Used for editing mode of exchange
    /// </summary>
    public class AdminEditExchangeModel
    {
        public AdminEditExchangeModel()
        {
            games = new List<Game>();
        }
        public int id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [MaxLength(225, ErrorMessage = "This field cannot longer than 225 characters.")]
        public string string_identifier { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [MaxLength(225, ErrorMessage = "This field cannot longer than 225 characters.")]
        public string name { get; set; }
        public HttpPostedFileBase icon { get; set; }
        public string is_archived { get; set; }
        public string is_active { get; set; }
        /// <summary>
        /// get list option for games
        /// </summary>
        public List<Game> games { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Please choose Game")]
        public int game_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Range(GoPlayConstantValues.I_MINIMUM_EXCHANGE_RATE, GoPlayConstantValues.I_MAXIMUM_EXCHANGE_RATE, ErrorMessage = "Exchange rate is between 0 and 100000")]
        public int exchange_rate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Range(GoPlayConstantValues.I_MINIMUM_EXCHANGE_RATE, GoPlayConstantValues.I_MAXIMUM_EXCHANGE_RATE, ErrorMessage = "Free exchange rate is between 0 and 100000")]
        public int free_exchange_rate { get; set; }
        public bool is_package { get; set; }

        public int limited_time_offer { get; set; }
        public int play_token_value { get; set; }
        public int free_play_token_value { get; set; }

        /// <summary>
        /// Edit page can go to by admin/exchange and admin/exchange/detail
        /// </summary>
        public string previous_page { get; set; }
        public string game_name { get; set; }
        public string icon_filename { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}