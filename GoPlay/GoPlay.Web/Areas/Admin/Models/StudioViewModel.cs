using GoPlay.Models;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Areas.Admin.Models
{
    public class StudioViewModel
    {

    }
    public class StudioDetailViewModel
    {
        public Studio studio { get; set; }
        public List<StudioAdminAssignment> studiosAssignment { get; set; }
    }
    public class StudioAdminViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int? studio_id { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Required]
        public string name { get; set; }
        public string action { get; set; }
        public string previous_page { get; set; }
    }

    public class StudioAssignViewModel
    {
        public Studio studio { get; set; }
        public string username { get; set; }
        public List<customer_account> admins { get; set; }
    }
}