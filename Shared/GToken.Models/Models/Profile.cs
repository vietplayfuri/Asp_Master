using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class Profile
    {
        public int uid { get; set; }

        public string account { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string vip { get; set; }
        public string avatar { get; set; }
        public string cover { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string bio { get; set; }
        public string inviter { get; set; }
        public decimal? gtoken { get; set; }
        public string locale { get; set; }
        public bool is_venvici_member { get; set; }
        public string status { get; set; }

        public static Profile GetFrom(CustomerAccount user)
        {
            if (user == null)
                return null;


            return new Profile
            {
                uid = user.id,
                account = user.username,
                email = user.email,
                nickname = user.nickname,
                gender = user.gender,
                vip = user.vip,
                avatar = user.GetValidAvatarUrl(),
                cover = user.GetValidCoverUrl(),
                country_code = user.country_code,
                country_name = user.country_name,
                bio = user.bio ?? string.Empty, // string is reference type, default is null //
                inviter = user.inviter_username,
                gtoken = user.gtoken,
                locale = string.IsNullOrEmpty(user.locale) ? ConstantCommon.BABEL_DEFAULT_LOCALE : user.locale,
                is_venvici_member = user.is_venvici_member

            };
        }
    }

    public class FacebookProfile
    {
        public string id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string gender { get; set; }
        public string last_name { get; set; }
        public string link { get; set; }
        public string locale { get; set; }
        public string name { get; set; }
        public string timezone { get; set; }
        public string updated_time { get; set; }
        public bool verified { get; set; }
    }
}
