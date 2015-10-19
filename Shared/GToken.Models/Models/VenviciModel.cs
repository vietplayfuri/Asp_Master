using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class VenviciModel
    {
        public VenviciAction enumAction { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string md5Password { get; set; }
        public string introducerId { get; set; }
        public string country { get; set; }
        public decimal amount { get; set; }
        public string refNo { get; set; }
        public string remark { get; set; }
        public decimal bv { get; set; }
    }
}
