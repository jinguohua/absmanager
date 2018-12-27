using System.Collections.Generic;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Filters
{
    public class FilterContextInfo
    {
        public FilterContextInfo(ActionExecutingContext filterContext)
        {
            DomainName = filterContext.HttpContext.Request.Url.Authority;
            ControllerName = filterContext.RouteData.Values["controller"].ToString();
            ActionName = filterContext.RouteData.Values["action"].ToString();
            Parameters = filterContext.ActionParameters;
            Username = filterContext.HttpContext.User.Identity.Name;
        }

        public string DomainName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public IDictionary<string,object> Parameters { get; set; }

        public string Username { get; set; }
    }
}