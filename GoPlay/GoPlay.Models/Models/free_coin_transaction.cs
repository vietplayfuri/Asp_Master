namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;

    public class free_coin_transaction
    {
        public free_coin_transaction()
        {
        }

        public int id { get; set; }

        public int customer_account_id { get; set; }

        public decimal? amount { get; set; }

        public int? game_id { get; set; }

        public int? credit_type_id { get; set; }

        public int? package_id { get; set; }

        public DateTime created_at { get; set; }

        public string description { get; set; }

        public string status { get; set; }

        public string order_id { get; set; }

        public int? topup_card_id { get; set; }

        public string payment_method { get; set; }

        public decimal? price { get; set; }

        public string ip_address { get; set; }

        public string country_code { get; set; }

        #region extends
        public bool is_free { get; set; }
        public string table_name { get; set; }
        #endregion


        public object ToDictionary(credit_transaction credit, string creditTypeString_identifier, string packageString_identifier)
        {
            return new
            {
                transaction_id = this.order_id,
                gtoken_value = this.amount,
                goplay_token_value = this.amount,
                quantity = credit.amount,
                is_free = true,
                exchange_option_type = credit.credit_type_id.HasValue
                    ? "CreditType" : credit.package_id.HasValue ? "Package" : string.Empty,
                exchange_option_identifier = credit.credit_type_id.HasValue
                    ? creditTypeString_identifier : credit.package_id.HasValue ? packageString_identifier : string.Empty,
            };
        }
    }
}
