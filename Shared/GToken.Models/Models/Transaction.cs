using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Platform.Utility;
namespace Platform.Models
{
    public class Transaction : ModelBase
    {
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
        public decimal revenue_percentage { get; set; }

        #region extend properties
        public string partner_name { get; set; }
        public decimal original_final_amount_after_tax
        {
            get
            {
                return (decimal)original_final_amount + original_tax + original_service_charge;
            }
        }

        public decimal revenue
        {
            get
            {
                return price * revenue_percentage;
            }
        }
        #endregion

        public TransactionJsonModel ToDictationary()
        {
            return new TransactionJsonModel()
            {
                username = this.customer_username,
                order_id = this.partner_order_id,
                gtoken_transaction_id = this.gtoken_transaction_id,
                price = this.price,
                final_amount = this.final_amount,
                tax = this.tax,
                service_charge = this.service_charge,
                currency = this.currency,
                original_price = this.original_price,
                original_final_amount = this.original_final_amount,
                original_tax = this.original_tax,
                original_service_charge = this.original_service_charge,
                original_currency = this.original_currency,
                discount_percentage = this.discount_percentage,
                status = this.status,
                payment_method = this.payment_method,
                description = this.description,
                is_venvici_applicable = this.is_venvici_applicable
            };
        }
    }

    public class TransactionJsonModel
    {
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public decimal price { get; set; }
        public decimal final_amount { get; set; }
        public decimal tax { get; set; }
        public decimal service_charge { get; set; }
        public string currency { get; set; }
        public decimal? original_price { get; set; }
        public decimal? original_final_amount { get; set; }
        public decimal original_tax { get; set; }
        public decimal original_service_charge { get; set; }
        public string original_currency { get; set; }
        public decimal? discount_percentage { get; set; }
        public string status { get; set; }
        public string payment_method { get; set; }
        public string description { get; set; }
        public bool? is_venvici_applicable { get; set; }
        public decimal revenue_percentage { get; set; }
    }

    public class CustomTransaction
    {
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
        public string currency { get; set; }
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

        public string partner_name { get; set; }
        public decimal original_final_amount_after_tax
        {
            get
            {
                decimal amount = this.original_final_amount ?? 0 + this.original_tax + this.original_service_charge;
                return CurrencyHelper.Round(amount, this.currency);
            }
        }
        public string username { get; set; }

        public decimal final_amount_after_tax
        {
            get
            {
                decimal amount = this.final_amount + this.tax + this.service_charge;
                return CurrencyHelper.Round(amount, this.currency);
            }
        }
    }

    /// <summary>
    /// Used for load list in the admin side
    /// </summary>
    public class DirectTransaction
    {
        public long id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public string description { get; set; }
        public string country_code { get; set; }
        public DateTime created_at { get; set; }

        public string customer_username { get; set; }
        public decimal amount { get; set; }
        public string partner_order_id { get; set; }
        public string partner_name { get; set; }
        public string username { get; set; }
    }


    /// <summary>
    /// Table direct_gtoken_transaction in database
    /// </summary>
    public class DirectGtokenTransaction : DirectTransaction
    {
        public DirectGtokenTransaction()
        {
            created_at = DateTime.UtcNow;
            updated_at = DateTime.UtcNow;
        }

        public string ip_address { get; set; }
        public string partner_identifier { get; set; }
        public DateTime updated_at { get; set; }

        public APIDirectGTokenTransactionResult ToDictionary()
        {
            return new APIDirectGTokenTransactionResult
            {
                gtoken_transaction_id = this.gtoken_transaction_id,
                order_id = this.partner_order_id,
                username = this.customer_username,
                amount = this.amount,
                description = this.description
            };
        }
    }


    /// <summary>
    /// Used for result of api direct charge
    /// </summary>
    public class APIDirectGTokenTransactionResult
    {
        public string username { get; set; }
        public string order_id { get; set; }
        public string gtoken_transaction_id { get; set; }
        public decimal? amount { get; set; }
        public string description { get; set; }
    }
}




