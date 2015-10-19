namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    public class CreditType
    {
        public CreditType()
        {
            created_at = DateTime.UtcNow;
            updated_at = DateTime.UtcNow;
        }

        public int id { get; set; }

        public int game_id { get; set; }

        public string name { get; set; }

        public int? exchange_rate { get; set; }

        public string icon_filename { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public bool is_archived { get; set; }

        public int? old_db_id { get; set; }

        public int? free_exchange_rate { get; set; }

        public bool? is_active { get; set; }

        public string string_identifier { get; set; }

        public string GetIcon()
        {
            if (!string.IsNullOrEmpty(this.icon_filename))
                return this.icon_filename;

            if (!string.IsNullOrEmpty(this.name) && this.name.ToLower().Contains("gem"))
            {
                return ConfigurationManager.AppSettings["DEFAULT_CREDIT_TYPE_GEM_ICON_URL"];
            }
            return ConfigurationManager.AppSettings["DEFAULT_CREDIT_TYPE_GOLD_ICON_URL"];
        }

        public decimal free_play_token_value
        {
            get
            {
                return this.free_exchange_rate.HasValue ? (decimal)this.free_exchange_rate.Value : 0;
            }
        }

        public decimal play_token_value
        {
            get
            {
                return this.exchange_rate.HasValue ? (decimal)this.exchange_rate.Value : 0;
            }
        }
        public CreditTypeDictationary ToDictationary()
        {
            return new CreditTypeDictationary
            {
                id = this.id,
                name = this.name,
                icon_filename = this.GetIcon(),
                exchange_rate = this.exchange_rate.HasValue
                    ? this.exchange_rate.Value
                    : 0,
                free_exchange_rate = this.free_exchange_rate.HasValue
                    ? this.free_exchange_rate.Value
                    : 0,
            };
        }
        public Tuple<decimal, decimal> calculatePlayToken(customer_account user, decimal purchaseAmount)
        {
            /*  If the exchange option allows free play Token:
                Calculate the maximum purchase capacity with free Play Token
                If capacity is sufficient, purchase all with free Play Token
                Else purchase with free Play Token as much as possible, push the rest to Play Token balance

              Given the purchase amount, be it the whole if free Play Token isn't allowed or is zero, or the remaining amount after free Play Token purchase
              Calculate the maximum purchase capacity with Play Token
              if capacity is sufficient, purchase with Play Token
              Else the balance is insufficient, return (0, 0)

              :return: Return a tuple of (freePlayToken, playToken) which indicates how much the transaction will cost the use
              if (0, 0) is return, the balance is insufficient */

            if (this.free_exchange_rate.HasValue && this.free_exchange_rate.Value > 0)
            {
                var maxCapacity = this.calculateInGameAmount(user.free_play_token.Value, "free_play_token");
                maxCapacity = Math.Round(maxCapacity, 2);
                if (maxCapacity >= purchaseAmount)
                {
                    var freePlayToken = purchaseAmount / this.free_play_token_value;
                    freePlayToken = Math.Round(freePlayToken, 3);
                    return Tuple.Create(freePlayToken, 0m);
                }
            }

            if (this.exchange_rate.HasValue && this.exchange_rate.Value > 0)
            {
                var maxCapacity = calculateInGameAmount(user.play_token.Value, "play_token");
                maxCapacity = Math.Round(maxCapacity, 2);
                if (maxCapacity >= purchaseAmount)
                {
                    var playToken = purchaseAmount / play_token_value;
                    playToken = Math.Round(playToken, 3);
                    return Tuple.Create(0m, playToken);
                }

            }
            return Tuple.Create(0m, 0m);
        }

        public decimal calculateInGameAmount(decimal playTokenAmount, string balanceType)
        {
            if (balanceType == "free_play_token")
                return this.free_play_token_value * playTokenAmount;
            if (balanceType == "play_token")
                return this.play_token_value * playTokenAmount;
            return 0;
        }

        #region External fields
        /// <summary>
        /// get game_name from game_id in credit_type
        /// </summary>
        public string game_name { get; set; }

        #endregion
    }

    public class CreditTypeDictationary
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon_filename { get; set; }
        public int exchange_rate { get; set; }
        public int free_exchange_rate { get; set; }
    }
}
