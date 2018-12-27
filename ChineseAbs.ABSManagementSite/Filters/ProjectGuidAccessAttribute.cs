using System;
using System.Linq;
using System.Web.Mvc;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagementSite.Filters
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
            var guid = filterInfo.Parameters.Values.Single().ToString();
            var projectId = DbUtils.GetIdByGuid(guid, "dbo.Project", "project_guid");

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