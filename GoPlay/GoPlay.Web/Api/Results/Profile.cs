using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GoPlay.Models;
using Platform.Utility;
namespace GoPlay.WebApi
{
    public class Profile
    {
       public int uid { get; set; }  
       public string account { get; set; } 
       public string nickname { get; set; }

       public string email { get; set; }
       public string gender { get; set; }
       public string vip { get; set; }
       public string bio { get; set; }
       public string avatar { get; set; }
       public string country_code { get; set; }

       public decimal? gtoken { get; set; }
       public decimal? free_gtoken { get; set; }
        
       public decimal? goplay_token { get; set; }

       public decimal? free_goplay_token { get; set; }

       public static Profile GetForm(customer_account user)
       {
           if (user == null)
               return null;

           Profile profile = new Profile
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

           profile = Helper.FullFillEmptyFields<Profile>(profile);

           return profile;
       }
    }
}