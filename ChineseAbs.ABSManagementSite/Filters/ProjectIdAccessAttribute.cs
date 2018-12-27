using System;
using System.Linq;
using System.Web.Mvc;
using ChineseAbs.ABSManagement;


namespace ChineseAbs.ABSManagementSite.Filters
{
    public class ProjectIdAccessAttribute
    {
        [AttributeUsage(AttributeTargets.Class
       | AttributeTargets.Method, AllowMultiple = false)]
        public class ProjectGuidAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var filterInfo = new FilterContextInfo(filterContext);
                var authority = new DBAdapter().Authority;
                if (filterInfo.Parameters.Count != 1)
                {
                    throw new ApplicationException("Project guid access attribute detected wrong action parameters.");
                }
                var projectId = int.Parse(filterInfo.Parameters.Values.Single().ToString());
                if (!authority.IsAuthorized(projectId))
                {
                    /// todo 跳到 No Access
                }
                else
                {
                    base.OnActionExecuting(filterContext);
                }
            }
        }
    }
}