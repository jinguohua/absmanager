using ChineseAbs.ABSManagementSite.Filters;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    [DesignAccessAttribute]
    public class DesignController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.IsSuperUser = m_dbAdapter.SuperUser.IsSuperUser();
            return View();
        }
    }
}