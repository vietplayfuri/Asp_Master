using System;
using System.Text;

namespace Platform.Models
{
    public class CustomerAccount : ModelBase
    {
        public int id { get; set; }
        public string nickname { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public string vip { get; set; }
        public decimal gtoken { get; set; }
        public string locale { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime last_login_at { get; set; }
        public DateTime referred_at { get; set; }
        public bool is_archived { get; set; }
        public string bio { get; set; }
        public string avatar_filename { get; set; }
        public string cover_filename { get; set; }
        public string inviter_username { get; set; }
        public string partner_identifier { get; set; }
        public string unhashed_password { get; set; }
        public DateTime date { get; set; }
        public bool is_venvici_member { get; set; }

        public DateTime dob { get; set; }

        #region extend properties
        public int dob_epoch
        {
            get
            {
                TimeSpan t = dob - new DateTime(1970, 1, 1);
                return (int)t.TotalSeconds;
            }
        }
        public string partner_name { get; set; }
        #endregion

        public string GetValidAvatarUrl()
        {
            return string.IsNullOrEmpty(avatar_filename) ? ConstantCommon.DEFAULT_AVATAR_URL : avatar_filename;
        }

        public string GetValidCoverUrl()
        {
            return string.IsNullOrEmpty(cover_filename) ? ConstantCommon.DEFAULT_COVER_URL : cover_filename;

        }

        public Profile toPublicDictionary()
        {
            Profile dict = Profile.GetFrom(this);
            dict.gtoken = null;
            return dict;
        }
        public Profile ToMinimalProfile()
        {
            return new Profile
            {
                account = username,
                avatar = GetValidAvatarUrl(),
                cover = GetValidCoverUrl()
            };
        }

        /// <summary>
        /// Check password login with unhashed_password
        /// </summary>
        /// <param name="loginPassword"></param>
        /// <returns>True: correct and ELSE</returns>
        public bool CheckPassword(string loginPassword)
        {
             return String.CompareOrdinal(unhashed_password.ToLower(), loginPassword.ToLower()) == 0;
        }

        public string GetDisplayName()
        {
            return !string.IsNullOrEmpty(this.nickname) ? this.nickname : this.username;
        }
    }
}


