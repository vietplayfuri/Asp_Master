using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.WebApi
{
    public class SaveProgressResult : Result
    {
        public DateTime save_at { get; set; }
    }
}