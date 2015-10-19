using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoPlay.WebApi
{
    public class Result
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string error_code { get; set; }

    }
}