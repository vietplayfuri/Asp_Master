using System.Collections.Generic;
using System.Linq;
using Platform.Dal;
using Platform.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Configuration;

namespace Platform.Core
{
    public partial class Api
    {
        /// <summary>
        /// Get referral Game
        /// </summary>
        public Result<List<ReferralGame>> GetReferralGames()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["GOPLAY-API"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                HttpResponseMessage response = client.PostAsync("game/get-games", null).Result;
                if (response.IsSuccessStatusCode)
                {
                    var games = JsonConvert.DeserializeObject<List<ReferralGame>>(response.Content.ReadAsStringAsync().Result);
                    if (games != null && games.Any())
                    {
                        return Result<List<ReferralGame>>.Make(games);
                    }
                }
                return Result<List<ReferralGame>>.Null(ErrorCodes.NotFound);
            }
        }


    }
}
