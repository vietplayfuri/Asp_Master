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
        public Result<List<GtokenPackage>> GetBasicGtokenPackages()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetBasicGtokenPackages(db);
            }
        }

        public Result<List<GtokenPackage>> GetUpointGTokenPackages()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetBasicGtokenPackages(db);
            }
        }

        public Result<List<GtokenPackage>> GetCustomGtokenPackage()
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetBasicGtokenPackages(db);
            }
        }

        public Result<GtokenPackage> GetTokenPackageBySKU(string sku)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTokenPackageBySKU(db, sku);
            }
        }
    }
}
