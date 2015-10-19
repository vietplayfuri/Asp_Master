namespace GoPlay.Models
{
    using Platform.Models;
    using System;
    using System.Collections.Generic;
    public class coin_transaction
    {
        public coin_transaction()
        {
        }

        public coin_transaction(int customerId, int receiverId, string description)
        {
            this.order_id = Guid.NewGuid().ToString();
            this.customer_account_id = customerId;
            this.receiver_account_id = receiverId;
            this.status = ConstantValues.S_SUCCESS;
            this.description = description;
        }


        public object ToDictionary(credit_transaction credit, string creditTypeString_identifier, string packageString_identifier)
        {
            return new
            {
                transaction_id = this.order_id,
                gtoken_value = this.amount,
                goplay_token_value = this.amount,
                quantity = credit.amount,
                is_free = false,
                exchange_option_type = credit.credit_type_id.HasValue 
                    ? "CreditType" : credit.package_id.HasValue ? "Package" : string.Empty,
                exchange_option_identifier = credit.credit_type_id.HasValue
                    ? creditTypeString_identifier : credit.package_id.HasValue ? packageString_identifier : string.Empty,
            };
        }


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

        public bool is_free { get; set; }
        public string table_name { get; set; }

    }

    /// <summary>
    /// This class contains the general fields of coin and free_coin tables
    /// </summary>
    public class coinTransaction
    {
        public string order_id { get; set; }
        /// <summary>
        /// Amount in coin_transaction or free_coin_transaction
        /// </summary>
        public decimal amount { get; set; }
        /// <summary>
        /// Amount in credit transaction
        /// </summary>
        public decimal quantity { get; set; }
        public bool is_free { get; set; }
        public string exchange_option_type { get; set; }
        public string exchange_option_identifier { get; set; }
        public int? coin_id { get; set; }
        public int? free_coin_id { get; set; }
        public int credit_transaction_id { get; set; }
        public string status { get; set; }
        public string username { get; set; }
        public string description { get; set; }
        public object ToDictionary()
        {
            return new
            {
                transaction_id = this.order_id,
                gtoken_value = this.amount,
                goplay_token_value = this.amount,
                quantity = this.quantity,
                is_free = this.free_coin_id.HasValue ? true : false,
                exchange_option_type = this.exchange_option_type,
                exchange_option_identifier = this.exchange_option_identifier
            };
        }
    }
}
