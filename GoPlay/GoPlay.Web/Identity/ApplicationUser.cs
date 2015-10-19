using Platform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GoPlay.Web.Const;
using GoPlay.Models;
using GoPlay.Core;
using System.Collections.Generic;
using GoPlay.Web.ActionFilter;
using System.Web;
using GoPlay.Web.Helpers;
namespace GoPlay.Web.Identity
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
        public decimal play_token { get; set; }
        public decimal free_play_token { get; set; }
        public string[] Roles { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string display_name { get; set; }

        public string locale { get; set; }
        public string bio { get; set; }        
        public DateTime create_at { get; set; }
        public string inviterAvatarUrl { get; set; }
        public string inviter_username { get; set; }
        public string referralID { get; set; }
        public decimal? gcoin { get; set; }
        public string GetDisplayName()
        {
            return !string.IsNullOrEmpty(this.nickname) ? this.nickname : this.UserName;
        }
        public ApplicationUser()
        {

        }

        public ApplicationUser(customer_account user)
        {
            Id = user.id;
            UserName = user.username;
            Email = user.email;
            gender = user.gender;
            avatar_filename = user.GetValidAvatarUrl();
            vip = user.vip;
            display_name = user.GetDisplayName();
            country_code = user.country_code;
            country_name = user.country_name;
            locale = user.locale;
            bio = user.bio;
            create_at = user.created_at;
            inviter_username = user.inviter_username;
            nickname = user.nickname;
            play_token = user.play_token.HasValue ? user.play_token.Value : 0;
            free_play_token = user.free_play_token.HasValue ? user.free_play_token.Value : 0;
            gcoin = user.gcoin;
            if (!string.IsNullOrEmpty(user.inviter_username))
            {
                customer_account inviter = GoPlayApi.Instance.GetUserByUserName(user.inviter_username).Data;
                if (inviter != null)
                    inviterAvatarUrl = inviter.GetValidAvatarUrl();
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


        public List<UserRole> GetRoles()
        {
            return UserHelper.GetRoles(HttpContext.Current.User.Identity.GetUserId<int>());
        }


    }
}