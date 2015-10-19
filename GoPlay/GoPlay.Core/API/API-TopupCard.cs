using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay.Models;
using GoPlay.Dal;
using Platform.Models;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public Result<topup_card> GetTopUpCard(string cardNumber, string cardPassword)
        {
            try
            {
                var repo = Repo.Instance;
                using (var db = repo.OpenConnectionFromPool())
                {
                    return repo.GetTopUpCard(db, cardNumber, cardPassword);
                }
            }
            catch { return Result<topup_card>.Null(ErrorCodes.ServerError); }
        }

        public bool UpdateTopupCard(int id, int customer_account_id, string status, DateTime used_at)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateTopupCard(db, id, customer_account_id, status, used_at);
            }
        }

        public Result<List<topup_card>> GetTopupCardsByCustomQuery(string sqlQuery)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetTopupCardsByCustomQuery(db, sqlQuery);
            }
        }
    }
}
