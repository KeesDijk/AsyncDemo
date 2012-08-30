namespace AsynchronousMVCWebApplication.Controllers
{
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            this.ViewBag.Message = "Start page";

            return this.View();
        }
    }
}