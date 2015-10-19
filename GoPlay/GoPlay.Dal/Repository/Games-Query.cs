using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using Platform.Models;
using GoPlay.Models;
using Newtonsoft.Json.Linq;
using System.Configuration;
namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<List<Game>> GetAllGames(IDbConnection db)
        {
            string sqlString = @"SELECT game.*, studio.name AS studio_name FROM game
                                INNER JOIN studio on studio.id = game.studio_id";
            var games = db.Query<Game>(sqlString).AsList();

            return Result<List<Game>>.Make(games);
        }


        public Result<List<Game>> GetGamesByCustomQuery(IDbConnection db, string sqlQuery)
        {
            var games = db.Query<Game>(sqlQuery).AsList();

            return Result<List<Game>>.Make(games);
        }

        ///// <summary>
        ///// Used for showing of dropdownlist
        ///// </summary>
        ///// <param name="db"></param>
        ///// <returns></returns>
        //public Result<List<SimpleGame>> GetGamesForDropdownlist(IDbConnection db)
        //{
        //    string sqlString = @"SELECT game.id, game.name FROM game";
        //    var games = db.Query<SimpleGame>(sqlString).AsList();
        //    return Result<List<SimpleGame>>.Make(games);
        //}

        public Result<List<SimpleGame>> GetGamesForDropdownlist(IDbConnection db, bool? is_archived = null, bool? is_active = null)
        {
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("SELECT id, name FROM game WHERE 1=1");

            if (is_archived.HasValue)
                sqlQuery.AppendLine("AND is_archived = @is_archived");
            if (is_active.HasValue)
                sqlQuery.AppendLine("AND is_active = @is_active");

            sqlQuery.AppendLine("ORDER BY name");
            var games = db.Query<SimpleGame>(sqlQuery.ToString(), new
            {
                is_archived,
                is_active
            }).AsList();

            return Result<List<SimpleGame>>.Make(games);
        }

        /// <summary>
        /// Used for showing of dropdownlist
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public Result<List<SimpleGame>> GetGamesForDropdownlist(IDbConnection db, int userId, bool is_archived)
        {
            string sqlQuery = @"SELECT game.id, game.name
                                FROM game 
                                JOIN studio_admin_assignment 
                                     ON game.studio_id = studio_admin_assignment.studio_id
                                WHERE game.is_archived = @is_archived 
                                      AND studio_admin_assignment.game_admin_id = @userId
                                ORDER BY game.name";

            var games = db.Query<SimpleGame>(sqlQuery, new { userId, is_archived }).AsList();
            return Result<List<SimpleGame>>.Make(games);
        }

        public Result<List<Game>> GetGames(IDbConnection db, SearchCondition searchConditions = null)
        {
            if (searchConditions == null)
            {
                return Result<List<Game>>.Null();
            }
            StringBuilder conditions = new StringBuilder();
            string keyword = searchConditions.keywork;
            string genre = searchConditions.genre;
            string platform = searchConditions.platform;
            string release = searchConditions.release;
            if (!string.IsNullOrEmpty(keyword))
            {
                conditions.AppendLine(string.Format("AND lower(name) like '%{0}%'", keyword));

            }
            if (!string.IsNullOrEmpty(genre) && genre != "all" && genre != "default")
            {
                conditions.AppendLine(string.Format("AND genre like '%{0}%'", genre));
            }
            if (!string.IsNullOrEmpty(platform))
            {
                switch (platform)
                {
                    case GoPlayConstantValues.PLATFORM_IOS:
                        conditions.AppendLine("AND download_links like '%apple%'");
                        break;
                    case GoPlayConstantValues.PLATFORM_ANDROID:
                        conditions.AppendLine("AND download_links like '%google%'");
                        break;
                    case GoPlayConstantValues.PLATFORM_PC:
                        conditions.AppendLine("AND download_links like '%pc%'");
                        break;
                    case GoPlayConstantValues.PLATFORM_APK:
                        conditions.AppendLine("AND download_links like '%apk%'");
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["APK_GAMES"]))
                        {
                            conditions.AppendLine("AND game.id IN (" + ConfigurationManager.AppSettings["APK_GAMES"] + ")");
                        }
                        break;
                }
            }
            //    '''Comment out in python             
            //    fromDate = _utcnow()
            //    if release != 'all' or genre != 'default':
            //        if release == '1week':
            //            fromDate =  fromDate + datetime.timedelta(days=-7)
            //        elif release == '1month':
            //            fromDate =  fromDate + datetime.timedelta(days=-30)
            //        elif release == '6month':
            //            fromDate =  fromDate + datetime.timedelta(days=-180)
            //        conditions = conditions + " AND (released_at is NULL OR released_at >= '"+ fromDate.strftime("%Y-%m-%d")+ "') "'''
            if (!string.IsNullOrEmpty(release) && release == GoPlayConstantValues.RELEASE_COMING)
            {
                conditions.AppendLine("AND download_links like '%{}%'");
            }
            string query = string.Format(@"SELECT* 
                                          FROM game 
                                          WHERE is_active = true 
                                           AND is_archived = false {0} 
                                           GROUP BY id
                                           ORDER BY id desc
                                        ", conditions.ToString());

            var games = db.Query<Game>(query).AsList();
            return Result<List<Game>>.Make(games);
        }


        public Result<Game> GetGame(IDbConnection db, string guid)
        {
            var game = db.Query<Game>("SELECT * FROM game WHERE guid=@guid", new { guid }).FirstOrDefault();
            return Result<Game>.Make(game, errorIfNull: ErrorCodes.InvalidGameId);
        }


        public Result<game_access_token> GetGameAccessToken(IDbConnection db, string token)
        {
            var gat = db.Query<game_access_token>("SELECT * FROM game_access_token WHERE token=@token", new { token }).FirstOrDefault();
            return Result<game_access_token>.Make(gat, errorIfNull: ErrorCodes.NotFound);
        }


        public bool SaveGameAccessToken(IDbConnection db, GameAccessToken gat)
        {
            gat.saved_at = DateTime.UtcNow;
            return 1 == db.Execute("UPDATE game_access_token SET data=@data, meta=@meta, saved_at=@saved_at WHERE token=@token", gat);

        }


        public Result<Game> GetGame(IDbConnection db, int id)
        {
            var game = db.Query<Game>(@"SELECT game.*, studio.name AS studio_name 
                FROM game
                LEFT JOIN studio ON studio.id = game.studio_id
                WHERE game.id = @id", new { id }).FirstOrDefault();

            return Result<Game>.Make(game, errorIfNull: ErrorCodes.InvalidGameId);
        }


        public Result<List<Game>> GetGamesByIds(IDbConnection db, bool? isActive = null, bool? isArchive = null, List<int> gameIds = null)
        {
            if (gameIds == null || !gameIds.Any())
                return Result<List<Game>>.Null(ErrorCodes.InvalidGameId);

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(string.Format(@"SELECT * FROM game WHERE id IN (" + string.Join(",", gameIds) + ")"));

            if (isActive.HasValue)
                sqlBuilder.Append(" AND is_active = @isActive");
            if (isArchive.HasValue)
                sqlBuilder.Append(" AND is_archived = @isArchive");

            var games = db.Query<Game>(sqlBuilder.ToString(), new { isActive, isArchive }).AsList();
            return Result<List<Game>>.Make(games, ErrorCodes.InvalidGameId);
        }


        public Result<List<Game>> GetGamesByIds(IDbConnection db, bool? isActive = null, bool? isArchive = null, bool? isPopular = null)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine(@"SELECT * FROM game WHERE 1 =1");

            if (isActive.HasValue)
                sqlBuilder.AppendLine("AND is_active = @isActive");
            if (isArchive.HasValue)
                sqlBuilder.AppendLine("AND is_archived = @isArchive");
            if (isPopular.HasValue)
                sqlBuilder.Append("AND is_popular = @isPopular");

            var games = db.Query<Game>(sqlBuilder.ToString(), new { isActive, isArchive, isPopular }).AsList();
            return Result<List<Game>>.Make(games, ErrorCodes.NotFound);
        }


        public Result<game_access_token> GetGameAccessToken(IDbConnection db, int gameId, int userId)
        {
            string sqlString = @"SELECT * 
                           FROM game_access_token 
                           WHERE game_id = @gameId 
                           AND customer_account_id = @userId";
            var obj = db.Query<game_access_token>(sqlString, new { gameId, userId }).FirstOrDefault();
            return Result<game_access_token>.Make(obj);
        }
        public bool SaveGameAccessToken(IDbConnection db, int id, int game_id, int customer_account_id, string token)
        {
            string sql = @"UPDATE game_access_token 
                SET game_id = @game_id, customer_account_id = @customer_account_id, token = @token
                WHERE id = @id";
            return 1 == db.Execute(sql, new { game_id, customer_account_id, token });
        }
        public int CreateGameAccessToken(IDbConnection db, int game_id, int customer_account_id, string token)
        {
            string sql = @"INSERT INTO game_access_token 
            (game_id, customer_account_id, token) 
            VALUES 
            (@game_id, @customer_account_id,@token)
             RETURNING id";

            return db.Query<int>(sql, new { game_id, customer_account_id, token }).FirstOrDefault();
        }

        public bool UpdateGameAccessToken(IDbConnection db, int id, string gtoken, string token)
        {
            string sql = @"UPDATE game_access_token 
                SET token = @token, gtoken_token = @gtoken
                WHERE 
                id=@id";

            return 1 == db.Execute(sql, new { id, token, gtoken });
        }

        public bool UpdateGameAccessToken(IDbConnection db, int id, string stats, DateTime saved_at)
        {
            string sql = @"UPDATE game_access_token 
                SET stats = @stats, saved_at = @saved_at
                WHERE 
                id=@id";

            return 1 == db.Execute(sql, new { id, stats, saved_at });
        }

        public bool UpdateGameAccessToken(IDbConnection db, int id, string data, string meta, DateTime now, string storage_file_name)
        {
            string sql = @"UPDATE game_access_token 
                SET data = @data, meta = @meta, saved_at = @now, storage_file_name = @storage_file_name
                WHERE 
                id=@id";

            return 1 == db.Execute(sql, new { id, data, meta, now, storage_file_name });
        }

        public bool UpdateGameAccessToken(IDbConnection db, int id, string data, string meta, DateTime now)
        {
            string sql = @"UPDATE game_access_token 
                SET data = @data, meta = @meta, saved_at = @now
                WHERE 
                id=@id";

            return 1 == db.Execute(sql, new { id, data, meta, now });
        }


        public Result<customer_account> LoadFromAccessToken(IDbConnection db, string token)
        {
            string sqlString = @"
                SELECT customer_account.* 
                FROM game_access_token  
                JOIN customer_account ON game_access_token.customer_account_id = customer_account.id 
                WHERE token = @token";
            var partner = db.Query<customer_account>(sqlString, new { token }).FirstOrDefault();
            return Result<customer_account>.Make(partner);
        }
        //        public Result<List<game>> GetAllGames(IDbConnection db, bool is_active, bool is_archived)
        //        {
        //            string sqlString = @"
        //                SELECT * 
        //                FROM game
        //                WHERE is_archived = @is_archived and is_active=@is_active";
        //            var games = db.Query<game>(sqlString, new { is_active, is_archived }).ToList();
        //            return Result<List<game>>.Make(games);
        //        }

        public Result<List<Game>> GetAllGames(IDbConnection db,
            bool is_active, bool is_archived, bool hasDownloadLink = false)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"SELECT * 
                FROM game
                WHERE 
                    is_archived = @is_archived 
                AND is_active=@is_active");
            if (hasDownloadLink)
                builder.Append(@" AND download_links != '{}'");

            var games = db.Query<Game>(builder.ToString(), new { is_active, is_archived }).ToList();
            return Result<List<Game>>.Make(games);
        }

        public Result<List<InvitableGameModel>> GetInvitableGames(IDbConnection db, int customer_account_id)
        {
            string sqlQuery = @"SELECT game_id, MAX(created_at)
                FROM api_log 
                WHERE customer_account_id = @customer_account_id
                  AND game_id IN (
                    SELECT id 
                    FROM game 
                    WHERE game.is_active = true
                      AND game.is_archived = false
                      AND game.download_links != '{}'
                  )
                GROUP BY game_id 
                ORDER BY MAX(created_at) DESC";

            var games = db.Query<InvitableGameModel>(sqlQuery, new { customer_account_id }).ToList();
            return Result<List<InvitableGameModel>>.Make(games);
        }

        public Result<List<Game>> FindGames(IDbConnection db, string name, bool is_active = true, bool is_archived = false)
        {
            var encodeForLike = string.IsNullOrEmpty(name)
                ? string.Empty
                : name.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");
            name = "%" + encodeForLike + "%";
            string sqlString = @"
                SELECT game.*, studio.name AS studio_name FROM
                game
                INNER JOIN studio ON studio.id = game.studio_id
                WHERE lower(game.name) like @name AND game.is_active=@is_active AND game.is_archived = @is_archived";
            var games = db.Query<Game>(sqlString, new { name, is_active, is_archived }).ToList();
            return Result<List<Game>>.Make(games);
        }

        public Result<List<Game>> GetGamesForAdmin(IDbConnection db, int userId, bool is_archived)
        {
            string sqlQuery = @"SELECT game.*,studio.name AS studio_name
                                FROM game 
                                JOIN studio_admin_assignment 
                                     ON game.studio_id = studio_admin_assignment.studio_id
                                JOIN studio on studio.id = game.studio_id 
                                WHERE game.is_archived = @is_archived 
                                      AND studio_admin_assignment.game_admin_id = @userId
                                ORDER BY game.name";

            var games = db.Query<Game>(sqlQuery, new { userId, is_archived }).AsList();
            return Result<List<Game>>.Make(games);
        }

        public int CreateGame(IDbConnection db, Game game)
        {
            string sqlQuery = @"INSERT INTO game(
                                    guid, name, description, is_archived, 
                                    studio_id, is_active, banner_filename, icon_filename, download_links, 
                                    thumb_filename, genre, short_description, endpoint, gtoken_client_id, 
                                    gtoken_client_secret, slider_images, current_version, file_size, 
                                    content_rating, current_changelog, released_at, warning, game_invite_protocol, 
                                    is_featured,youtube_links)
                                VALUES (@guid, @name, @description, @is_archived, 
                                    @studio_id, @is_active, @banner_filename, @icon_filename, @download_links, 
                                    @thumb_filename, @genre, @short_description, @endpoint, @gtoken_client_id, 
                                    @gtoken_client_secret, @slider_images, @current_version, @file_size, 
                                    @content_rating, @current_changelog, @released_at, @warning, @game_invite_protocol, 
                                    @is_featured,@youtube_links) 
                                RETURNING id;";
            return db.Query<int>(sqlQuery, game).FirstOrDefault();
        }
        public bool UpdateGame(IDbConnection db, Game game)
        {
            StringBuilder query = new StringBuilder();
            game.updated_at = DateTime.UtcNow;
            query.AppendLine(@"UPDATE game
                               SET name=@name, description=@description, updated_at = @updated_at, 
                                   is_archived=@is_archived, studio_id=@studio_id, is_active=@is_active, 
                                   download_links=@download_links, genre=@genre, short_description=@short_description, 
                                   endpoint=@endpoint, gtoken_client_id=@gtoken_client_id, gtoken_client_secret=@gtoken_client_secret, 
                                   current_version=@current_version, file_size=@file_size, content_rating=@content_rating, 
                                   current_changelog=@current_changelog, warning=@warning, game_invite_protocol=game_invite_protocol, 
                                   is_featured=@is_featured, youtube_links = @youtube_links, is_popular=@is_popular");
            if (game.released_at.HasValue)
            {
                query.AppendLine(", released_at=@released_at");
            }
            if (!string.IsNullOrEmpty(game.icon_filename))
            {
                query.AppendLine(", icon_filename=@icon_filename");
            }
            if (!string.IsNullOrEmpty(game.thumb_filename))
            {
                query.AppendLine(", thumb_filename=@thumb_filename");
            }
            if (!string.IsNullOrEmpty(game.banner_filename))
            {
                query.AppendLine(", banner_filename=@banner_filename");
            }
            if (!string.IsNullOrEmpty(game.slider_images))
            {
                query.AppendLine(", slider_images=@slider_images");
            }
            query.AppendLine(" WHERE id = @id;");
            return 1 == db.Execute(query.ToString(), game);
        }


        public Result<List<CustomApiLog>> GetApiLogByCustomQuery(IDbConnection db, string sqlQuery)
        {
            var logs = db.Query<CustomApiLog>(sqlQuery).ToList();
            return Result<List<CustomApiLog>>.Make(logs);
        }

        public Result<List<promotion>> GetOngoingPromotion(IDbConnection db, string game_ids, bool is_archived = false)
        {
            var utcNow = DateTime.UtcNow;
            string sqlQuery = string.Format(@"SELECT promotion.*,promotion_game.game_id
                                FROM promotion_game 
                                LEFT JOIN promotion ON promotion.id = promotion_game.promotion_id
                                WHERE promotion.start_at <= @utcNow AND  promotion.end_at>= @utcNow
                                      AND promotion.progress < promotion.threshold
                                      AND promotion.is_archived = @is_archived
                                      AND promotion_game.game_id IN ({0})", game_ids);

            var obj = db.Query<promotion>(sqlQuery, new { utcNow, is_archived }).AsList();
            return Result<List<promotion>>.Make(obj);
        }
    }
}
