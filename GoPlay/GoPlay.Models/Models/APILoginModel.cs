using Platform.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Models
{
    public class GtokenAPILoginModel
    {
        public CustomerAccountProfile profile { get; set; }
        public bool success { get; set; }
        // public string message { get; set; } // not used //
        public string error_code { get; set; }
        public string session { get; set; }
        public string message { get; set; }
        public string username { get; set; }

        /// <summary>
        /// Used for check-oauth-connection
        /// </summary>
        public string bindonlyID { get; set; }
    }

    public class CustomerAccountProfile
    {
        public int uid { get; set; }
        public string account { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string vip { get; set; }
        public string bio { get; set; }
        public string inviter { get; set; }
        public string avatar { get; set; }
        public string cover { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public decimal? gtoken { get; set; }
        public bool is_venvici_member { get; set; }
        public decimal? free_gtoken { get; set; }

        public decimal? goplay_token { get; set; }

        public decimal? free_goplay_token { get; set; }

        public static CustomerAccountProfile GetForm(customer_account user)
        {
            if (user == null)
                return null;
            return new CustomerAccountProfile
            {
                uid = user.id,
                account = user.username,
                email = user.email,
                nickname = user.nickname,
                gender = user.gender,
                vip = user.vip,
                avatar = user.GetValidAvatarUrl(),
                country_code = user.country_code,
                bio = user.bio ?? string.Empty,
                gtoken = user.play_token,
                free_gtoken = user.free_play_token,
                goplay_token = user.play_token,
                free_goplay_token = user.free_play_token
            };
        }
    }

    public class GtokenAPIFriend
    {
        public GtokenAPIFriend()
        {

        }
        /// <summary>
        /// used for friend/search api
        /// </summary>
        public object users { get; set; }
        public int count { get; set; }
        public object friends { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
        /// <summary>
        /// Gtoken session
        /// </summary>
        public string session { get; set; }
    }

    public class GtokenModelAccountAction
    {
        public EGtokenAction enumAction { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }
        public string old_pwd { get; set; }
        public string confirm_pwd { get; set; }
        public string partnerId { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string referral_code { get; set; }
        public string ip_address { get; set; }
        public string session { get; set; }
        public string bio { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string token { get; set; }
        public string service { get; set; }
        public string oauth_id { get; set; }
        public string friend_username { get; set; }
        public string status { get; set; }
        /// <summary>
        /// If true, return multiple profile objects.
        /// If false, return an array of username. Default to false
        /// </summary>
        public string include_profile { get; set; }

        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public string device_id { get; set; }
        /// <summary>
        /// Added for referral_download_system
        /// </summary>
        public string game_id { get; set; }
    }

    public class GtokenModelFriendAction
    {
        public string session { get; set; }
        public string partnerId { get; set; }
        public EGtokenAction enumAction { get; set; }
        public string friend_username { get; set; }

        public string status { get; set; }
        /// <summary>
        /// If true, return multiple profile objects.
        /// If false, return an array of username. Default to false
        /// </summary>
        public string include_profile { get; set; }
        public string keyword { get; set; }
        public string offset { get; set; }
        public string count { get; set; }
    }

    public class GtokenModelTransactionAction
    {
        public string hashed_token { get; set; }
        public string partnerId { get; set; }
        public EGtokenAction enumAction { get; set; }
        public TokenTransactionJson token_transaction { get; set; }
        public string ip_address { get; set; }
    }

    public class GtokenAPITransaction
    {
        public object token_transaction { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }
    }
}
