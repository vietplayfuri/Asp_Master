using APIProxy.Dal;
using APIProxy.Model;
using Newtonsoft.Json;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Core
{
    public partial class ProxyApi
    {
        public Result<Game> GetGame(int gameId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGame(db, gameId);
            }
        }

        
    }
}
