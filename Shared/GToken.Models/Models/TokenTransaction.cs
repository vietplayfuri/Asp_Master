using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models.Models
{
    public class TokenTransaction : ModelBase
    {
        public TokenTransaction()
        {
            created_at = DateTime.UtcNow;
            updated_at = DateTime.UtcNow;
        }

        public long id { get; set; }
        public string customer_username { get; set; }
        public string partner_identifier { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string partner_order_id { get; set; }
        public string token_type { get; set; }
        public string transaction_type { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }
        public string ip_address { get; set; }
        public string country_code { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public decimal tax { get; set; }
        public decimal service_charge { get; set; }

        public bool is_cash
        {
            get
            {
                string currencyName = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                              .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == transaction_type)
                              .Select(c => new RegionInfo(c.LCID).CurrencyEnglishName)
                              .FirstOrDefault();
                return !string.IsNullOrEmpty(currencyName);
            }
        }
        public decimal amount_after_tax
        {
            get{
                return amount + tax + service_charge;
            }
        }

        public TokenTransactionJson ToDictationary()
        {
            return new TokenTransactionJson()
            {
                username = this.customer_username,
                order_id = this.partner_order_id,
                gtoken_transaction_id = this.gtoken_transaction_id,
                token_type = this.token_type,
                transaction_type = this.transaction_type,
                amount = this.amount,
                service_charge = this.service_charge,
                tax = this.tax,
                description = this.description
            };
        }
    }

    public class MainTransaction
    {
        public string partner_name { get; set; }
        string _currency = "USD";
        public long id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public decimal price { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string payment_method { get; set; }
        public string ip_address { get; set; }
        public string country_code { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string partner_identifier { get; set; }
        public string customer_username { get; set; }
        public decimal? discount_percentage { get; set; }
        public decimal final_amount { get; set; }
        public string currency
        {
            get { return _currency; }
            set { _currency = value; }
        }
        public decimal? original_price { get; set; }
        public decimal? original_final_amount { get; set; }
        public string original_currency { get; set; }
        public string partner_order_id { get; set; }
        public decimal exchange_rate { get; set; }
        public bool is_venvici_applicable { get; set; }
        public decimal original_tax { get; set; }
        public decimal original_service_charge { get; set; }
        public decimal tax { get; set; }
        public decimal service_charge { get; set; }
        public string token_type { get; set; }
        public string transaction_type { get; set; }
        public decimal amount { get; set; }

        public decimal original_final_amount_after_tax
        {
            get
            {
                return (decimal)original_final_amount + original_tax + original_service_charge;
            }
        }

        public bool is_cash
        {
            get
            {
                string currencyName = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                              .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == transaction_type)
                              .Select(c => new RegionInfo(c.LCID).CurrencyEnglishName)
                              .FirstOrDefault();
                return !string.IsNullOrEmpty(currencyName);
            }
        }
        public decimal amount_after_tax
        {
            get
            {
                return amount + tax + service_charge;
            }
        }
    }

    public class TokenTransactionJson
    {
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string token_type { get; set; }
        public string transaction_type { get; set; }
        public decimal? amount { get; set; }
        public decimal? tax { get; set; }
        public decimal? service_charge { get; set; }
        public string description { get; set; }

        public TokenTransactionJson()
        { }

        public TokenTransactionJson(string username, string orderId, string description, decimal amount, string token_type)
        {
            this.username = username;
            this.order_id = orderId;
            this.description = description;
            this.amount = amount;
            this.token_type = token_type;
            this.transaction_type = ConstantValues.S_TRANSACTION_TYPE_TRANSFER;
        }
    }
}
