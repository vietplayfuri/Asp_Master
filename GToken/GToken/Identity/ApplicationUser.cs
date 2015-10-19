using Platform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GToken.Web.Const;

namespace GToken.Web.Identity
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
        public Nullable<decimal> play_token { get; set; }
        public string[] Roles { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }

        public ApplicationUser()
        {

        }

        public ApplicationUser(CustomerAccount user)
        {
            Id = user.id;
            UserName = user.username;
            nickname = user.nickname;
            Email = user.email;
            gender = user.gender;
            avatar_filename = String.IsNullOrEmpty(user.avatar_filename) ? Platform.Models.ConstantCommon.DEFAULT_AVATAR_URL : user.avatar_filename;
            vip = user.vip;
            play_token = user.gtoken;
            country_code = user.country_code;
            country_name = user.country_name;
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