namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    public class Package
    {
        public Package()
        {
            created_at = DateTime.UtcNow;
            updated_at = DateTime.UtcNow;
        }

        public int id { get; set; }

        public int game_id { get; set; }

        public string name { get; set; }

        public decimal play_token_value { get; set; }

        public string icon_filename { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public bool is_archived { get; set; }

        public int? old_db_id { get; set; }

        public decimal? free_play_token_value { get; set; }

        public bool? is_active { get; set; }

        public string string_identifier { get; set; }

        public int? limited_time_offer { get; set; }

        public string GetIcon()
        {
            return !string.IsNullOrEmpty(this.icon_filename)
                ? this.icon_filename
                : ConfigurationManager.AppSettings["DEFAULT_PACKAGE_ICON_URL"];
        }

        public PackageDictationary ToDictationary()
        {
            return new PackageDictationary
            {
                id = this.id,
                name = this.name,
                icon_filename = this.GetIcon(),
                gtoken_value = this.play_token_value,
                free_gtoken_value = this.free_play_token_value.HasValue
                    ? this.free_play_token_value.Value
                    : 0,
                play_token_value = this.play_token_value,
                free_play_token_value = this.free_play_token_value.HasValue
                    ? this.free_play_token_value.Value
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

            if (this.free_play_token_value.HasValue && this.free_play_token_value.Value > 0)
            {
                var maxCapacity = this.calculateInGameAmount(user.free_play_token.Value, "free_play_token");
                if (maxCapacity >= purchaseAmount)
                {
                    return Tuple.Create(purchaseAmount * free_play_token_value.Value, 0m);
                }
            }

            if (this.play_token_value > 0)
            {
                var maxCapacity = calculateInGameAmount(user.play_token.Value, "play_token");
                maxCapacity = Math.Round(maxCapacity, 2);
                if (maxCapacity >= purchaseAmount)
                {
                    return Tuple.Create(0m, purchaseAmount * play_token_value);
                }

            }
            return Tuple.Create(0m, 0m);
        }

        public decimal calculateInGameAmount(decimal playTokenAmount, string balanceType)
        {
            if (balanceType == "free_play_token")
                return playTokenAmount / free_play_token_value.Value;
            if (balanceType == "play_token")
                return playTokenAmount / play_token_value;
            return 0;
        }

        #region External fields

        /// <summary>
        /// get game_name from game_id in package
        /// </summary>
        public string game_name { get; set; }

        #endregion
    }

    public class PackageDictationary
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon_filename { get; set; }
        public decimal gtoken_value { get; set; }
        public decimal free_gtoken_value { get; set; }
        public decimal play_token_value { get; set; }
        public decimal free_play_token_value { get; set; }
    }
}
