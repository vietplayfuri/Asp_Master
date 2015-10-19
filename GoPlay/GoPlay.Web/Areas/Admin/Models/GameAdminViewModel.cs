using GoPlay.Models;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace GoPlay.Web.Areas.Admin.Models
{

    public class GameAdminViewModel
    {
        public GameAdminViewModel()
        {
        }
        public int? game_id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [StringLength(30)]
        public string name { get; set; }
        public int? studio_id { get; set; }
        public string iosDownloadLink { get; set; }
        public string androidDownloadLink { get; set; }
        public string apkDownloadLink { get; set; }
        public string pcDownloadLink { get; set; }
        public string youtubeLinks { get; set; }

        public string is_active
        {
            get;
            set;
        }
        public string is_featured { get; set; }
        public string is_popular { get; set; }

        public DateTime? released_at { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [ValidateImage(ErrorMessage = "JPG or PNG images only!")]
        public HttpPostedFileBase icon { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [ValidateImage(ErrorMessage = "JPG or PNG images only!")]

        public HttpPostedFileBase thumb { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [ValidateImage(ErrorMessage = "JPG or PNG images only!")]

        public HttpPostedFileBase banner { get; set; }
        [StringLength(6)]
        public string current_version { get; set; }
        public string file_size { get; set; }
        public string endpoint { get; set; }
        public string gtoken_client_id { get; set; }
        public string gtoken_client_secret { get; set; }
        public string game_invite_protocol { get; set; }
        public List<HttpPostedFileBase> sliderImages { get; set; }
        public string submit { get; set; }

        public List<Studio> studios { get; set; }
        [AllowHtml]
        public LocaleInfo Genre { get; set; }
        [AllowHtml]
        public LocaleInfo Description { get; set; }
        [AllowHtml]
        public LocaleInfo Short_description { get; set; }
        [AllowHtml]
        public LocaleInfo Current_changelog { get; set; }
        [AllowHtml]
        public LocaleInfo Content_rating { get; set; }
        [AllowHtml]
        public LocaleInfo Warning { get; set; }
        public bool HasRoleAdmin { get; set; }
        public string action { get; set; }
        public string previous_page { get; set; }
    }

    public class ValidateImageAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return false;
            }
            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return img.RawFormat.Equals(ImageFormat.Png) ||
                           img.RawFormat.Equals(ImageFormat.Jpeg);
                }
            }
            catch { }
            return false;
        }
    }
    public class ValidateUrlAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var url = value as string;
            if (string.IsNullOrEmpty(url))
                return true;
            Regex regex = new Regex(ConfigurationManager.AppSettings["PATTERN_URL"]);
            return regex.Match(value.ToString()).Success;
        }
    }


    public class SliderImages
    {
        public List<SliderInfo> images { get; set; }
    }
    public class SliderInfo
    {
        public int index { get; set; }
        public string filename { get; set; }
    }

    public class GameIndexViewModel
    {
        public Game game { get; set; }
        public LocaleInfo Genre { get; set; }
        public LocaleInfo Description { get; set; }
        public Dictionary<string, string> Platforms { get; set; }
        public LocaleInfo Short_description { get; set; }
        public LocaleInfo Current_changelog { get; set; }
        public LocaleInfo Content_rating { get; set; }
        public LocaleInfo Warning { get; set; }
    }
    public class LocaleInfo
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Locale { get; set; }
        public List<string> Errors { get; set; }

    }
}