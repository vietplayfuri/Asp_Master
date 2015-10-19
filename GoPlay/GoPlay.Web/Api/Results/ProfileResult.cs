using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.WebApi
{
    public class ProfileResult : Result
    {

        public ProfileResult()
        {
            Profile = new Profile();
        }
    

        public Profile Profile { get; set; }
    }
}