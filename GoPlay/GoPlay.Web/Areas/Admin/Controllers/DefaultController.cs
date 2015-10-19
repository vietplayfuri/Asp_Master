using GoPlay.Web.Controllers;
using GoPlay.Web.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoPlay.Web.Areas.Admin.Controllers
{
     [Authorize]
    [RouteArea("admin")]
    [RoutePrefix("")]
    public class DefaultController : BaseController
    {
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
    }
}