using System.Data;
using Dapper;

namespace GoEat.Dal
{
    public partial class GoEatRepo
    {
        public bool UpdateUserDiscount(IDbConnection db, int customer_id, int discount_id, bool is_activated)
        {
            string sql = "UPDATE user_discount SET is_activated = @is_activated WHERE customer_account_id=@customer_id AND discount_id=@discount_id";
            return 1 == db.Execute(sql, new {customer_id, discount_id, is_activated });
        }

        public bool RegisterDiscount(IDbConnection db, int userId, int discountId)
        {
            string sql = "INSERT user_discount (customer_account_id, discount_id, is_activated) VALUES( " +
                " @userId, @discountId, @is_activated)";
            return 1 == db.Execute(sql, new {userId, discountId, is_activated = false });
        }
    }
}
