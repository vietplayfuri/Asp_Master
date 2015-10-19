using Dapper;
using APIProxy.Model;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProxy.Dal
{
    public partial class Repo
    {
        public async Task<Result<UserCredential>> GetUserCredential(IDbConnection db, int userId, int gameId)
        {
            string sqlQuery = "select * from user_credential where user_id = @userId and game_id = @gameId";
            var userCredential =await db.QueryAsync<UserCredential>(sqlQuery, new { userId, gameId });
            return Result<UserCredential>.Make(userCredential.FirstOrDefault(), ErrorCodes.NotFound);
        }


        public async Task<int> CreateUserCredential(IDbConnection db, UserCredential userCredential)
        {
            string sqlQuery = @"INSERT INTO user_credential
                (user_id, game_id, username, session)
                VALUES
                (@user_id, @game_id, @username, @session)
                RETURNING id";
            var obj = await db.QueryAsync<int>(sqlQuery, userCredential);
            return obj.FirstOrDefault();
        }


        /// <summary>
        /// Update username and session only
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userCredential"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserCredential(IDbConnection db, UserCredential userCredential)
        {
            string sqlQuery = @"UPDATE user_credential 
                SET
                    username = @username,
                    session = @session
                WHERE
                    user_id = @user_id
                AND game_id = @game_id";
            return 1 == await db.ExecuteAsync(sqlQuery, userCredential);
        }

        public async Task<Result<OauthAccessToken>> GetOauthAccessToken(IDbConnection db, string service, string identity, int game_id)
        {
            service = service.ToLower();
            string sqlString = @"SELECT *  
                                 FROM oauth_access_token 
                                 WHERE lower(service) =@service and identity=@identity and game_id = @game_id";
            var OAuth = await db.QueryAsync<OauthAccessToken>(sqlString, new { service, identity, game_id });
            return Result<OauthAccessToken>.Make(OAuth.FirstOrDefault());
        }
        public async Task<int> CreateOauthAccessToken(IDbConnection db, OauthAccessToken oauthAccessToken)
        {
            string sqlQuery = @"INSERT INTO oauth_access_token(service, identity, access_token, game_id)
                                VALUES (@service, @identity, @access_token, @game_id)
                                     RETURNING id";
            var obj = await db.QueryAsync<int>(sqlQuery, oauthAccessToken);
            return obj.FirstOrDefault();
        }
        /// <summary>
        /// Update accessToken only
        /// </summary>
        /// <param name="db"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOauthAccessToken(IDbConnection db, int id,string access_token)
        {
            string sqlQuery = @"UPDATE oauth_access_token 
                SET
                    access_token = @access_token
                WHERE
                    id = @id";
            return 1 == await db.ExecuteAsync(sqlQuery, new { id, access_token });
        }

    }
}
