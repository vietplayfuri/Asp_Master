using GoEat.Dal;

namespace GoEat.Core
{
    public partial class GoEatApi 
    {
        public bool UpdateUserDiscount(int customer_id, int discount_id, bool is_activated)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateUserDiscount(db, customer_id, discount_id, is_activated);
            }
        }

        public bool RegisterDiscount(int userId, int discountId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.RegisterDiscount(db, userId, discountId);
            }
        }
    }
}
