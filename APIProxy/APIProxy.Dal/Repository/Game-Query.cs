using APIProxy.Model;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace APIProxy.Dal
{
    public partial class Repo
    {
        public Result<Game> GetGame(IDbConnection db, int gameId)
        {
            string sqlQuery = "select * from game where id = @gameId";
            var game = db.Query<Game>(sqlQuery, new { gameId }).FirstOrDefault();
            return Result<Game>.Make(game);
        }
    }
}
