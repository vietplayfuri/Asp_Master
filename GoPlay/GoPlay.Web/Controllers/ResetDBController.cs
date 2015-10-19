using Microsoft.Owin.Security;
using System.Web;
using GoPlay.Web.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using GoPlay.Web.ActionFilter;
using System;
using GoPlay.Models;
using Platform.Models;
using Platform.Utility;
using GoPlay.Web.Helpers;
using GoPlay.Core;

namespace GoPlay.Web.Controllers
{
    public class ResetDBController : BaseController
    {
        //private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ResetDBController(IUserStore<ApplicationUser, int> userStore)
        {
            _userManager = new ApplicationUserManager(userStore);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// In python code, only create 2 tables with token is difference of input token and token of Current user
        /// </summary>
        /// <returns></returns>
        [Route("reset-balance")]
        [RequiredLogin]
        [RBAC(AccessAction = "modify_self_balance")]
        public ActionResult resetBalance()
        {
            int playToken = 0;
            Int32.TryParse(Request.Params["play_token"], out playToken);
            int freePlayToken = 0;
            Int32.TryParse(Request.Params["free_play_token"], out freePlayToken);

            var diffPlayToken = playToken - CurrentUser.play_token;

            coin_transaction transaction = new coin_transaction();
            transaction.order_id = Guid.NewGuid().ToString();
            transaction.customer_account_id = CurrentUser.Id;
            transaction.amount = diffPlayToken;
            transaction.description = GoPlayConstantValues.S_RESETBALANCE_DESCRIPTION;
            transaction.status = Helper.GetDescription(TransactionStatus.Success);

            var diffFreePlayToken = freePlayToken - CurrentUser.free_play_token;
            free_coin_transaction freeTransaction = new free_coin_transaction();
            freeTransaction.order_id = Guid.NewGuid().ToString();
            freeTransaction.customer_account_id = CurrentUser.Id;
            freeTransaction.amount = diffFreePlayToken;
            freeTransaction.description = GoPlayConstantValues.S_RESETBALANCE_DESCRIPTION;
            freeTransaction.status = Helper.GetDescription(TransactionStatus.Success);

            var api = GoPlayApi.Instance;
            if (api.CreateCoinTransaction(transaction).Data != null
                && api.CreateFreeCoinTransaction(freeTransaction) > 0)
                this.Flash(string.Format(GoPlayConstantValues.S_RESETBALANCE_SUCCESS, freePlayToken, playToken), FlashLevel.Success);
            else
                this.Flash("Account balance was reset fail", FlashLevel.Error);

            return RedirectToAction("profile", "account");
        }
    }
}