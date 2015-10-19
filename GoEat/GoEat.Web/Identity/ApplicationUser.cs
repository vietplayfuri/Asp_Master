using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GoEat.Dal.Models;
using Microsoft.AspNet.Identity;
using Constants = GoEat.Web.Const.Constants;

namespace GoEat.Web.Identity
{
    public class ApplicationUser : IUser<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string nickname { get; set; }
        public string Email { get; set; }
        public string gender { get; set; }
        public string avatar_filename { get; set; }
        public string vip { get; set; }
        public decimal? play_token { get; set; }
        public string[] Roles { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }

        public ApplicationUser()
        {

        }

        public ApplicationUser(CustomerAccountProfile user)
        {
            if (user != null)
            {
                Id = user.uid;
                UserName = user.account;//temporary for now
                Email = user.email;
                gender = user.gender;
                avatar_filename = String.IsNullOrEmpty(user.avatar) ? Constants.DEFAULT_AVATAR : user.avatar;
                vip = user.vip;
                play_token = user.gtoken;
                nickname = user.nickname;
                country_code = user.country_code;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}