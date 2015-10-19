using System.Web.Mvc;

namespace GoEat.Web.Controllers
{
    public class MainController : BaseController
    {
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Club()
        {

            return View();
        }

        public ActionResult Support()
        {

            return View();
        }

        public ActionResult Terms()
        {

            return View();
        }

        public ActionResult News()
        {

            return View();
        }

        public ActionResult Partners() {
            return View();
        }
    }
}