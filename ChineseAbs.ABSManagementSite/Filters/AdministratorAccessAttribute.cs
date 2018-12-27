using ChineseAbs.ABSManagement;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ChineseAbs.ABSManagementSite.Filters
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple=false)]
    public class AdministratorAccessAttribute :ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var filterInfo = new FilterContextInfo(filterContext);
            if (ChineseAbs.ABSManagement.Utils.CommUtils.IsLocalDeployed())
            {
                if (!System.Web.Security.Roles.IsUserInRole(filterInfo.Username, 
                    ChineseAbs.ABSManagementSite.Controllers.ManageController.RoleAdmin))
                {
                    var routeValue = new RouteValueDictionary(new
                    {
                        controller = "Error",
                        action = "NoAccess",
                    });
                    filterContext.Result = new RedirectToRouteResult(routeValue);
                }

                base.OnActionExecuting(filterContext);
            }
            else
            {
                var authority = new DBAdapter().Authority;
                var isAdmin = authority.IsEnterpriseAdministrator(filterInfo.Username);
                if (isAdmin)
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
                    base.OnActionExecuting(filterContext);
                }
            }
        }
    }
}