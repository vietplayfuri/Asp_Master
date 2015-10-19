using GoEat.Core;
using GoEat.Dal.Common;
using GoEat.Web.ActionFilter;
using System;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace GoEat.Web.Controllers
{
    [Authorize]
    [RoutePrefix("admin")]
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }


        [Route("reconcile")]
        [RBAC(AccessAction = "do_transaction")]
        public ActionResult FinishPayment()
        {
            return null;
        }
    }
}