namespace GoPlay.Models
{
    using System;
    using System.Collections.Generic;
    using Platform.Models;
    public class customer_account
    {
        public customer_account()
        {
        }

        public int id { get; set; }

        public string nickname { get; set; }

        public string email { get; set; }

        public string gender { get; set; }

        public string avatar_filename { get; set; }

        public string vip { get; set; }

        public decimal? play_token { get; set; }

        //public int? inviter_id { get; set; }
        public string inviter_username { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public DateTime last_login_at { get; set; }

        public bool is_archived { get; set; }

        public string referral_code { get; set; }

        public int? password_credential_id { get; set; }

        public decimal? free_play_token { get; set; }

        public string bio { get; set; }

        public string locale { get; set; }

        public string country_code { get; set; }

        public string country_name { get; set; }

        public int? game_id { get; set; }

        public string cover_filename { get; set; }

        public string account_manager { get; set; }

        public string account_manager_note { get; set; }

        public decimal? gcoin { get; set; }

        public bool is_discount_permanent { get; set; }

        public DateTime? referred_at { get; set; }

        public string username { get; set; }

        public string unhashed_password { get; set; }

        public string GetValidAvatarUrl()
        {
            return string.IsNullOrEmpty(avatar_filename) ? ConstantCommon.DEFAULT_AVATAR_URL : avatar_filename;
        }

        public string GetValidCoverUrl()
        {
            return string.IsNullOrEmpty(cover_filename) ? ConstantCommon.DEFAULT_COVER_URL : cover_filename;

        }
        public string GetDisplayName()
        {
            return !string.IsNullOrEmpty(this.nickname) ? this.nickname : this.username;
        }

        public bool HasDiscount()
        {
            //12/10/2015 remove all discount for all topup packages
            return false;
            // return this.is_discount_permanent || !string.IsNullOrEmpty(this.inviter_username);
        }
        public object ToPublicDictationary()
        {
            return new
            {
                uid = this.id,
                account = this.username,
                email = string.IsNullOrEmpty(this.email) ? string.Empty : this.email,
                nickname = this.GetDisplayName(),
                vip = this.vip,
                avatar = this.GetValidAvatarUrl(),
                country_code = this.country_code,
                bio = this.bio
            };
        }
    }

    /// <summary>
    /// Used for search friend in goplay
    /// </summary>
    public class SimpleCustomerAccount
    {
        public int id { get; set; }

        /// <summary>
        /// Status in friend table 
        /// </summary>
        public string status { get; set; }
        public string avatar_filename { get; set; }
        public string nickname { get; set; }
        public string username { get; set; }
        public int full_count { get; set; }

        public string GetDisplayName()
        {
            return !string.IsNullOrEmpty(this.nickname) ? this.nickname : this.username;
        }
        public string GetValidAvatarUrl()
        {
            return string.IsNullOrEmpty(avatar_filename) ? ConstantCommon.DEFAULT_AVATAR_URL : avatar_filename;
        }
        public object ToPublicDictationary()
        {
            return new
            {
                uid = this.id,
                account = this.username,
                nickname = this.GetDisplayName(),
                avatar = this.GetValidAvatarUrl(),
                status = string.IsNullOrEmpty(this.status) ? string.Empty : this.status
            };
        }
    }

    /// <summary>
    /// Used for admin >> user
    /// </summary>
    public class CustomerAccountReport
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public decimal? play_token { get; set; }
        public decimal? free_play_token { get; set; }
        public string vip { get; set; }
        public string game_name { get; set; }
        public string country_name { get; set; }
        public string inviter_username { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime last_login_at { get; set; }
        public string account_manager { get; set; }
    }
}
