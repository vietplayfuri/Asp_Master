using GoEat.Dal;
using GoEat.Dal.Models;
using GoEat.Models;
using System.Collections.Generic;

namespace GoEat.Core
{
    public partial class GoEatApi
    {

        public Result<CustomerAccount_ToRemove> GetUserById(int userId)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerById(db, userId);
            }
        }

        public Result<CustomerAccount_ToRemove> GetUserSession(string session)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCustomerBySession(db, session);
            }
        }

        public bool UpdateCustomerAccountProfile(int userid, string profile, string session)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCustomeAccountProfile(db, userid, profile, session);
            }
        }


        public bool CreateCustomerAccount(CustomerAccount_ToRemove user)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateCustomerAccount(db, user);
            }
        }

        public Result<Discount> GetCurrentDiscount(int user_id, int restaurant_id)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCurrentDiscount(db, user_id, restaurant_id);
            }
        }

        public Result<SimpleDiscount> GetSimpleDiscount(int user_id, int restaurant_id)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetSimpleDiscount(db, user_id, restaurant_id);
            }
        }

        /// <summary>
        /// Minus token of customer after using
        /// </summary>
        /// <param name="userid">customer_id</param>
        /// <param name="minusToken">minus token</param>
        /// <returns>true: minus success / false: error</returns>
        private bool MinusTokenOfCustomer(int userid, decimal minusToken)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.MinusTokenOfCustomer(db, userid, minusToken);
            }
        }

        /// <summary>
        /// Add token for user after they finished payment with paypal
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="addToken"></param>
        /// <returns></returns>
        private bool AddTokenOfCustomer(int userid, decimal addToken)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.AddTokenOfCustomer(db, userid, addToken);
            }
        }

        public Result<List<AuthRole>> GetRolesByUserId(int userid)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAuthRolesByUserId(db, userid);
            }
        }

        public Result<List<AuthAction>> GetActionsByRoleId(int id)
        {
            var repo = GoEatRepo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetAuthActionsByRoleId(db, id);
            }
        }

    }
}
