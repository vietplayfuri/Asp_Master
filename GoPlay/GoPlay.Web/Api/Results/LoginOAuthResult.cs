using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.WebApi
{
    public class LoginOAuthResult : Result
    {
        public LoginOAuthResult()
        {
            Profile = new Profile();
        }
        public string session { get; set; }

        public Profile Profile { get; set; }

    }
}