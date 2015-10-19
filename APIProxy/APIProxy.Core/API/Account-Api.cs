using APIProxy.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform.Models;
using APIProxy.Model;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web;
using Facebook;
using Platform.Utility;
namespace APIProxy.Core
{
    public partial class ProxyApi
    {
        public void GetAccounts()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {

            }
        }
        public async Task<Result<UserCredential>> GetUserCredential(int userId, int gameId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return await repo.GetUserCredential(db, userId, gameId);
            }
        }

        public async Task<int> CreateUserCredential(UserCredential userCredential)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return await repo.CreateUserCredential(db, userCredential);
            }
        }

        /// <summary>
        /// Update username and session only
        /// </summary>
        /// <param name="userCredential"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserCredential(UserCredential userCredential)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return await repo.UpdateUserCredential(db, userCredential);
            }
        }

        public string IDRetriever(string service, string token)
        {
            if (string.Compare(service, Helper.GetDescription(EThirdPartyService.Facebook), StringComparison.OrdinalIgnoreCase) == 0)
            {
                var fbObj = new FacebookGraphAPI(token).GetObject("/me", null);
                if(fbObj==null)
                    throw new Exception(Helper.GetDescription(ErrorCodes.FACEBOOK_ACCESS_ERROR));
                return JsonHelper.DeserializeObject<FacebookProfile>(fbObj.ToString()).id;
            }
            else if (string.Compare(service, Helper.GetDescription(EThirdPartyService.Apple), StringComparison.OrdinalIgnoreCase) == 0)
                return token;
            throw new Exception(Helper.GetDescription(ErrorCodes.NOT_SUPPORTED_OAUTH_PROVIDER));
        }

        public async Task<Result<OauthAccessToken>> GetOauthAccessToken(string service, string identity, int game_id)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return await repo.GetOauthAccessToken(db, service, identity, game_id);
            }
        }
        public async Task<int> CreateOauthAccessToken(OauthAccessToken oauthAccessToken)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return await repo.CreateOauthAccessToken(db, oauthAccessToken);
            }
        }
        /// <summary>
        /// Update accessToken only
        /// </summary>
        /// <param name="db"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOauthAccessToken(int id,string access_token)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return await repo.UpdateOauthAccessToken(db, id,access_token);
            }
        }

    }
}
