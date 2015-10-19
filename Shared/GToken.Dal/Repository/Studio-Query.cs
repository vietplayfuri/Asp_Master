using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Net;
using System.Data;
using System.Text.RegularExpressions;
using Platform.Models;


namespace Platform.Dal
{
    public partial class Repo
    {
        public Result< List<Studio> > GetAllStudios()
        {
            using (var db = OpenConnectionFromPool())
            {
                var studios = db.Query<Studio>("SELECT * FROM studio ORDER BY id").AsList();
                return Result<List<Studio>>.Make(studios);
            }
        }


        public Result<Studio> GetStudio(int id)
        {
            using (var db = OpenConnectionFromPool())
            {
                var studio = db.Query<Studio>("SELECT * FROM studio WHERE id=@id", new { id }).FirstOrDefault();
                return Result<Studio>.Make(studio, errorIfNull: ErrorCodes.InvalidGameId);
            }
        }

    }
}
