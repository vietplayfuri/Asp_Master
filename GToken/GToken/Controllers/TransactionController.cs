using Platform.Models;
using Platform.Models.Models;
using GToken.Web.Models;
using System.Web.Mvc;

namespace GToken.Web.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {
        // GET: Transaction
        public ActionResult Index(int page = 1)
        {
            int pagesize = 10;
            var api = Platform.Core.Api.Instance;

            int fromIndex = (page - 1) * pagesize;
            int toIndex = page * pagesize;


            var trans = api.GetAllTransactions(fromIndex, toIndex, CurrentUser.UserName, "success","pending");

            TransactionsPaging transPaging = new TransactionsPaging();
            transPaging.count = api.CountAllTransactions(CurrentUser.UserName, "success");
            transPaging.transactions = trans.Data;
            transPaging.pagination = new GToken.Helpers.Extensions.Pagination(page, pagesize, transPaging.count);

            return View(transPaging);
        }

        [HttpGet]
        [Route("transaction/invoice")]
        public ActionResult Invoice(string order_id)
        {
            var api = Platform.Core.Api.Instance;
            Transaction trans = api.GetTransaction(order_id).Data;
            TokenTransaction token_trans = null;
            bool permission = false;
            if (trans == null)
            {
                token_trans = api.GetTokenTransaction(order_id).Data;
                if(token_trans!=null)
                {
                    permission = token_trans.customer_username == CurrentUser.UserName;
                }
            }
            else
            {
                permission = trans.customer_username == CurrentUser.UserName;
            }
            InvoiceModel model = new InvoiceModel();
            if (permission)
            {
                model.transaction = trans;
                model.token_transaction = token_trans;
            }
            return View(model);
        }
    }
}