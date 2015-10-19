namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    using Platform.Utility;

    public class GtokenPackage
    {
        public GtokenPackage()
        {
        }

        public int id { get; set; }

        public string name { get; set; }

        public decimal? price { get; set; }

        public string icon_filename { get; set; }

        public decimal? play_token_amount { get; set; }

        public string sku { get; set; }

        public string currency { get; set; }

        public string icon_animation_html { get; set; }

        public bool is_archived { get; set; }

        public decimal getPrice(customer_account user = null)
        {
            if (user == null || !user.HasDiscount())
            {
                return this.price.Value;
            }
            return Math.Round(this.price.Value * 0.9m, 2);
        }

        public string getNameSlugified()
        {
            return this.name.GenerateSlug().ToLower();
        }

        public decimal? GetPlayToken(customer_account user = null)
        {
            if (user == null || !user.HasDiscount())
                return this.play_token_amount;
            if (!this.price.HasValue)
                return null;
            var price = this.price.Value * (decimal)1.1;
            return Math.Round(price, 2);
        }
    }
}
