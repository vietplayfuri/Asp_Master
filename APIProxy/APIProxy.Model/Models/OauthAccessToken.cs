using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Model
{
    public class OauthAccessToken
    {
        public int id { get; set; }
        public string service { get; set; }
        public string identity { get; set; }
        public string access_token { get; set; }
        public int game_id { get; set; }
    }
}
