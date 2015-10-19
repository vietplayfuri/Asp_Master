using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Model
{
    public class PartnerAccount
    {
        public int id { get; set; }
        public string name { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string client_type { get; set; }
        public string redirect_uris { get; set; }
        public string default_scopes { get; set; }
    }
}
