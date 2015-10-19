using GoPlay.Dal;
using GoPlay.Models;
using Newtonsoft.Json.Linq;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        //public Result<GameAccessToken> GetProgress(string gameUuid, string token)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var game = repo.GetGame(db, gameUuid);
        //        if (!game.Succeeded)
        //        {
        //            return Result<GameAccessToken>.Null(game.Error);
        //        }
        //        return repo.GetGameAccessToken(db, token);
        //    }
        //}

        //public bool SaveProgress(string gameUuid, string token, string data, string meta)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var game = repo.GetGame(db, gameUuid);
        //        if (!game.Succeeded)
        //        {
        //            return false;
        //        }

        //        var gat = new GameAccessToken
        //        {
        //            game_id = game.Data.id,
        //            token = token,
        //            data = data,
        //            meta = meta
        //        };

        //        repo.SaveGameAccessToken(db, gat);
        //    }
        //    return true;
        //}

        //public void UpdateGameStats()
        //{

        //}

        public List<Game> GetGamesForCurrentUser(bool is_archived = false, bool is_active = true)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var games = repo.GetAllGames(db);
                return games.Data.Where(x => x.is_active = is_active).ToList();
            }
        }

        //public Result<game> GetGame(int gameId)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        return repo.GetGame(db, gameId);
        //    }
        //}

        public Result<game_access_token> GetProgress(string gameUuid, string token)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var game = repo.GetGame(db, gameUuid);
                if (!game.Succeeded)
                {
                    return Result<game_access_token>.Null(game.Error);
                }
                return repo.GetGameAccessToken(db, token);
            }
        }

        public bool SaveProgress(string gameUuid, string token, string data, string meta)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var game = repo.GetGame(db, gameUuid);
                if (!game.Succeeded)
                {
                    return false;
                }

                var gat = new GameAccessToken
                {
                    game_id = game.Data.id,
                    token = token,
                    data = data,
                    meta = meta
                };

                repo.SaveGameAccessToken(db, gat);
            }
            return true;
        }

        public void UpdateGameStats()
        {

        }

        //public List<game> GetGamesForCurrentUser(bool is_archived = false)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var games = repo.GetAllGames(is_archived);
        //        return games.Data;
        //    }
        //}

        //public List<game> GetGamesForCurrentUser(bool is_archived = false, bool is_active = true)
        //{
        //    var repo = Repo.Instance;
        //    using (var db = repo.OpenConnectionFromPool())
        //    {
        //        var games = repo.GetAllGames(is_archived);
        //        games.Data.RemoveAll(x => x.is_active != is_active);

        //        return games.Data;
        //    }
        //}

        public Result<Game> GetGame(int gameId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGame(db, gameId);
            }
        }

        public Result<List<Game>> GetGamesByIds(bool? isActive = null, bool? isArchive = null, List<int> gameIds= null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGamesByIds(db, isActive, isArchive, gameIds);
            }
        }

        public Result<List<Game>> GetGamesByIds(bool? isActive = null, bool? isArchive = null, bool? isPopular = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGamesByIds(db, isActive, isArchive, isPopular);
            }
        }

        public Result<Game> GetGame(string gameUID)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGame(db, gameUID);
            }
        }

        public Result<List<Game>> GetAllGames()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAllGames(db);
            }
        }

        public Result<List<Game>> GetGames(bool is_active, bool is_archived)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAllGames(db, is_active, is_archived);
            }
        }

        public Result<List<Game>> GetGamesByCustomQuery(string sqlQuery)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGamesByCustomQuery(db, sqlQuery);
            }
        }

        public Result<List<Game>> GetInvitableGames(int userId,
            bool is_active, bool is_archived, bool hasDownloadLink)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var allGames = repo.GetAllGames(db, is_active, is_archived, hasDownloadLink).Data;
                var invitableGames = repo.GetInvitableGames(db, userId).Data;
                List<Game> games = new List<Game>();
                List<int> gameIds = invitableGames.Select(g => g.game_id).ToList();
                foreach (var game in allGames)
                {
                    if (!gameIds.Contains(game.id))
                    {
                        games.Add(game);
                    }
                }
                return Result<List<Game>>.Make(games, ErrorCodes.ServerError);
            }
        }

        public Result<List<Game>> GetGames(SearchCondition searchConditions)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGames(db, searchConditions);
            }
        }
        public Result<List<Game>> getFeaturedGames()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var result = repo.GetAllGames(db);
                var gameList = result.Data.Where(x => x.is_active
                                          && x.is_archived == false
                                              && x.is_featured).ToList();
                return Result<List<Game>>.Make(gameList);
            }
        }

        public Result<game_access_token> GetGameAccessToken(string token)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGameAccessToken(db, token);
            }
        }

        public bool UpdateGameAccessToken(int id, string latestStats, DateTime now)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateGameAccessToken(db, id, latestStats, now);
            }
        }

        public bool UpdateGameAccessToken(int id, string data, string meta, DateTime now)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateGameAccessToken(db, id, data, meta, now);
            }
        }

        public bool UpdateGameAccessToken(int id, string data, string meta, DateTime now, string storage_file_name)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateGameAccessToken(db, id, data, meta, now, storage_file_name);
            }
        }

        public Result<List<Game>> findGames(string name)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.FindGames(db, name);
            }
        }

        public Result<credit_transaction> GetCreditTransaction(int coin_transaction_id)
        {

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCreditTransactionByCoinTransactionId(db, coin_transaction_id);
            }
        }

        public Result<credit_transaction> GetCreditTransactionByFreeCoinTransactionId(int coin_transaction_id)
        {

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCreditTransactionByFreeCoinTransactionId(db, coin_transaction_id);
            }
        }


        public Result<List<credit_transaction>> GetCreditTransactionsByCustomQuery(string sqlQuery)
        {

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCreditTransactionsByCustomQuery(db, sqlQuery);
            }
        }

        //       if self.game: 
        //    creditTransaction = store.find(CreditTransaction,
        //                                   CreditTransaction.free_coin_transaction_id == self.id).one()
        //    if creditTransaction:
        //        return _("Exchange for %(item)s in %(game)s",
        //             amount = int(creditTransaction.amount) if int(creditTransaction.amount) != 1 else '',
        //             item = self.creditType.name if self.creditType else self.package.name if self.package else 'Unknown',
        //             game = self.game.name,
        //             play_token_amount = displayDecimal(self.amount * -1))
        //if self.payment_method:
        //    return _("Top-up", amount = displayDecimal(self.amount))
        //return _("Receive Free Play Token", amount = displayDecimal(self.amount))

        public Result<List<Game>> GetGamesForAdmin(int userId, bool is_archived)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetGamesForAdmin(db, userId, is_archived);
            }
        }
        public int CreateGame(Game game)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateGame(db, game);
            }
        }
        public List<SimpleGame> GetGamesForDropdownlist(int userId, List<string> roles)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (roles.Contains(GoPlayConstantValues.S_ROLE_ADMIN))
                    return repo.GetGamesForDropdownlist(db).Data;
                if (roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ADMIN) || roles.Contains(GoPlayConstantValues.S_ROLE_GAME_ACCOUNTANT))
                    return repo.GetGamesForDropdownlist(db, userId, false).Data;

                return repo.GetGamesForDropdownlist(db, false, true).Data;
            }
        }
        public bool UpdateGame(Game game)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateGame(db, game);
            }
        }

        public Result<List<CustomApiLog>> GetApiLogByCustomQuery(string sqlQuery)
        {

            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetApiLogByCustomQuery(db, sqlQuery);
            }
        }

        public Result<List<promotion>> GetOngoingPromotion(string game_ids, bool is_archived=false)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetOngoingPromotion(db,game_ids, is_archived);
            }
        }
    }
}
