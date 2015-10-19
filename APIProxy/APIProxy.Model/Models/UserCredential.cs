using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Model
{
    public class UserCredential
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int game_id { get; set; }
        public string username { get; set; }
        public string session { get; set; }
    }
}
