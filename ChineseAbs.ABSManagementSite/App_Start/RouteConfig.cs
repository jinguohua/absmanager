using System.Web.Mvc;
using System.Web.Routing;

namespace ChineseAbs.ABSManagementSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "api",
                url: "api/{controller}/{action}/{id}",
                defaults: new {  id = UrlParameter.Optional},
                namespaces:new string[] { "ABS.ABSManagementSite.Controllers.API" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Schedule", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}