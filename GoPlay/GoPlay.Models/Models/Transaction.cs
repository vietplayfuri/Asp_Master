using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Models.Models
{
    public class GeneralTransaction
    {
        public int id { get; set; }

        public int customer_account_id { get; set; }

        public int? receiver_account_id { get; set; }

        public decimal? amount { get; set; }

        public int? partner_account_id { get; set; }

        public int? game_id { get; set; }

        public int? credit_type_id { get; set; }

        public int? package_id { get; set; }

        public DateTime created_at { get; set; }

        public string description { get; set; }

        public string status { get; set; }

        public int? topup_card_id { get; set; }

        public string order_id { get; set; }

        public string payment_method { get; set; }

        public int? sender_account_id { get; set; }

        public int? gtoken_package_id { get; set; }

        public string paypal_redirect_urls { get; set; }

        public string paypal_payment_id { get; set; }

        public decimal? price { get; set; }

        public string ip_address { get; set; }

        public string country_code { get; set; }

        public string telkom_order_id { get; set; }

        public bool use_gtoken { get; set; }

        public string table_name { get; set; }
        public string getIcon()
        {
            switch (table_name)
            {
                case GoPlayConstantValues.COIN_TRANSACTION:
                    if (game_id.HasValue)
                    {
                        return "/static/images/trans-exchange-icon.png";
                    }
                    if (receiver_account_id.HasValue || sender_account_id.HasValue)
                    {
                        return "/static/images/trans-transfer-icon.png";
                    }
                    if (!String.IsNullOrEmpty(payment_method))
                    {
                        return "/static/images/trans-topup-icon.png";
                    }
                    if (partner_account_id.HasValue)
                    {
                        return "/static/images/trans-ven-icon.png";
                    }
                    return "/static/images/small-gtoken.png";

                case GoPlayConstantValues.FREE_COIN_TRANSACTION:
                    if (game_id.HasValue)
                    {
                        return "/static/images/trans-exchange-icon.png";
                    }
                    if (!String.IsNullOrEmpty(payment_method))
                    {
                        return "/static/images/trans-topup-icon.png";
                    }

                    return "/static/images/small-gtoken.png";
            }
            return "/static/images/small-gcoin.png";
        }

        public string source()
        {
            switch (table_name)
            {
                case GoPlayConstantValues.COIN_TRANSACTION:
                    if (game_id.HasValue)
                    {
                        return "Exchange";
                    }
                    if (receiver_account_id.HasValue || sender_account_id.HasValue)
                    {
                        return "Transfer";
                    }
                    if (!String.IsNullOrEmpty(payment_method))
                    {
                        return payment_method;
                    }
                    if (partner_account_id.HasValue)
                    {
                        return "Venvici";
                    }
                    return "Unknown";

                case GoPlayConstantValues.FREE_COIN_TRANSACTION:
                    if (game_id.HasValue)
                    {
                        return "Exchange";
                    }
                    if (!String.IsNullOrEmpty(payment_method))
                    {
                        return payment_method;
                    }
                    return "Free Play Token";
            }
            return String.Empty;
        }

        public int totalrow { get; set; }
    }

    public class AdminGeneralTransaction : GeneralTransaction
    {
        public int id { get; set; }
        public string nickname { get; set; }

        public string username { get; set; }

        public string country_name { get; set; }

        public string game_name { get; set; }

        public bool is_free { get; set; }

        public string credit_type_name { get; set; }
        public int? credit_type_id { get; set; }

        public string package_name { get; set; }
    }
}
