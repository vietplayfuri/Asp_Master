using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.WebApi
{
    public class GetProgressResult : Result
    {
        public string data { get; set; }
        public string meta { get; set; }
        public string file { get; set; }

        public DateTime save_at { get; set; }


    }
}