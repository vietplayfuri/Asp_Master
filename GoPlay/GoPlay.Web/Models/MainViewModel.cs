using GoPlay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Models
{
    public class MainViewModel
    {
    }
    public class MessageViewModel
    {
        public MessageViewModel()
        {
            message = new List<Tuple<string, string>>();
        }
        public List<Tuple<string, string>> message { get; set; }
    }
    public class SupportViewModel
    {
        //GAME_UNAVAILABLE = _("Game is not active or has been removed")
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string customerName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Email(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Invalid_email_address")]
        public string customerEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string subject { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string message { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string platform { get; set; }

        public int? gameID { get; set; }
        public string gameVersion { get; set; }
        public string gameDevice { get; set; }
        public string gameOSName { get; set; }
        public string gameOSVersion { get; set; }
        public bool forValidate { get; set; }
        public List<Game> listgames { get; set; }
        public Game game { get; set; }
        public SupportViewModel()
        {
            customerName = "";
            customerName = "";
        }
    }

}