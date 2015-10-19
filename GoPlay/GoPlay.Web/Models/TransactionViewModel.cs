using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoPlay.Models;
using GoPlay.Models.Models;
using Platform.Models;
using Platform.Utility;
using Resources;
using GoPlay.Core;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
namespace GoPlay.Web.Models
{
    public class TransactionViewModel
    {
        public TransactionViewModel()
        {
            pageParams = new Dictionary<string, string>();
            games = new List<Game>();
        }
        public List<GtokenPackage> basicGtokenPackages { get; set; }
        public List<GtokenPackage> upointGTokenPackages { get; set; }
        public List<GtokenPackage> customGtokenPackage { get; set; }

        public List<Game> games { get; set; }

        public Dictionary<string, string> pageParams { get; set; }

        public List<GeneralTransaction> transactions { get; set; }

        public customer_account user { get; set; }
        public GCoinConvertViewModel gCoinConvert { get; set; }
        public int transactions_count { get; set; }
    }

    public class TransactionPayPal
    {
        public string sku { get; set; }
    }
    public class PaypalRedirectUrl
    {
        public string approval_url { get; set; }
        public string execute { get; set; }
    }

    public class GCoinConvertViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "This_field_is_required")]
        [Email(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Invalid_email_address")]
        public string paypalEmail { get; set; }
        public decimal gcoin { get; set; }
        public string submit { get; set; }
        public string password { get; set; }

        public bool IsValidPassWord(string _username)
        {
            var result = GoPlayApi.Instance.GTokenAPIAccount(new GtokenModelAccountAction()
            {
                enumAction = EGtokenAction.Login,
                username = _username,
                pwd = password,
                partnerId = ConfigurationManager.AppSettings["GTOKEN_PARTNER_UID"]

            }).Result;

            if (!result.HasData || !result.Data.success)
            {
                return false;
            }
            return true;
        }
    }

    public class ExchangeViewModel
    {
        public int gameID { get; set; }
        public int exchangeAmount { get; set; }
        public int exchangeOptionID { get; set; }
        public string exchangeOption { get; set; }
        public string executeExchange { get; set; }
    }

    public class UnfulfilledExchangeModel
    {
        public string session { get; set; }
        public string game_id { get; set; }
    }


    public class FulfilledExchangeModel
    {
        public string session { get; set; }
        public string game_id { get; set; }
        public  string transaction_id { get; set; }
    }
}