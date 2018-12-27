using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Filters;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new SAFS.Web.MVC.Logging.OperateLogFilterAttribute());
            filters.Add(new ErrorHandlerFilterAttribute());
        }
    }
}