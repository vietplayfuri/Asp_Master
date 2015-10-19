using GoPlay.Models;
using GoPlay.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.Web.Models
{
    public class UPointBalanceDeductionViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string phoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string GtokenPackageSKU { get; set; }
    }

    public class UPointSpeedyViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string phoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string speedyNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string GtokenPackageSKU { get; set; }
    }

    public class UPointTMoneyModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string phoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string GtokenPackageSKU { get; set; }
    }
    public class UPointTelkomselVoucherViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string hrn { get; set; }
        public string submit { get; set; }
    }
    public class UPointStandardVoucherViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string hrn { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        public string vsn { get; set; }
        public string phoneNumber { get; set; }
        public string submit { get; set; }
    }

    public class InvoiceViewModel
    {
        public coin_transaction transaction { get; set; }
        public customer_account payer { get; set; }
        public GtokenPackage package { get; set; }
        public topup_card topupCard { get; set; }
        public GeneralTransaction generalTrans { get; set; }
        public string mainUrl { get; set; }
        public InvoiceViewModel()
        {
            var Request = HttpContext.Current.Request;
            mainUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
        }
    }

    
}