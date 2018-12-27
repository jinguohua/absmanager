using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ChineseAbs.ABSManagementSite.Filters
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple=false)]
    public class DemoDeployedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ChineseAbs.ABSManagement.Utils.CommUtils.IsDemoDeployed())
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                var routeValue = new RouteValueDictionary(new
                {
                    controller = "Error",
                    action = "NoAccess",
                });
                filterContext.Result = new RedirectToRouteResult(routeValue);
            }
        }
    }
}