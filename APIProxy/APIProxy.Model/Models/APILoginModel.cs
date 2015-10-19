using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Model
{
    public class GoPlayAPILoginModel
    {
        public CustomerAccountProfile profile { get; set; }
        public bool success { get; set; }
        // public string message { get; set; } // not used //
        public string error_code { get; set; }
        public string session { get; set; }
        public string message { get; set; }
        public string username { get; set; }
        public string user_id { get; set; }
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

        //public static CustomerAccountProfile GetForm(CustomerAccount user)
        //{
        //    if (user == null)
        //        return null;
        //    return new CustomerAccountProfile
        //    {
        //        uid = user.id,
        //        account = user.username,
        //        email = user.email,
        //        nickname = user.nickname,
        //        gender = user.gender,
        //        vip = user.vip,
        //        avatar = user.GetValidAvatarUrl(),
        //        country_code = user.country_code,
        //        bio = user.bio ?? string.Empty,
        //        gtoken = user.play_token,
        //        free_gtoken = user.free_play_token,
        //        goplay_token = user.play_token,
        //        free_goplay_token = user.free_play_token
        //    };
        //}
    }
    public class GoPlayModelAccountAction
    {
        public EGoPlayAction enumAction { get; set; }
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
        public string game_id { get; set; }
        /// <summary>
        /// If true, return multiple profile objects.
        /// If false, return an array of username. Default to false
        /// </summary>
        public string include_profile { get; set; }
    }
}
