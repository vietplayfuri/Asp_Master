using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay.Models;
using Platform.Models;
using Dapper;
using System.Data;

namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<topup_card> GetTopUpCard(IDbConnection db, string card_number, string card_password)
        {
            var topup_card = db.Query<topup_card>("SELECT * FROM topup_card WHERE card_number = @card_number AND card_password=@card_password", new { card_number, card_password }).SingleOrDefault();
            return Result<topup_card>.Make(topup_card);
        }
        public bool UpdateTopupCard(IDbConnection db, int id, int customer_account_id, string status, DateTime used_at)
        {
            string stringSql = @"UPDATE topup_card
                SET customer_account_id = @customer_account_id,
                status=@status,
                used_at =@used_at WHERE id = @id";
            return 1 == db.Execute(stringSql, new { id, customer_account_id, status, used_at });

        }

        public Result<List<topup_card>> GetTopupCardsByCustomQuery(IDbConnection db, string stringSql)
        {
            var topups = db.Query<topup_card>(stringSql).ToList();
            return Result<List<topup_card>>.Make(topups);
        }
    }
}
