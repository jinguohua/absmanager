using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    [AllowAnonymous]
    public class ErrorController:Controller
    {
        public ActionResult Index()
        {
            Response.StatusCode = 200;
            return View();
        }

        public ActionResult NoAccess()
        {
            Response.StatusCode = 200;
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 200;
            return View();
        }

    }
}