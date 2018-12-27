using System;
using System.Web.Mvc;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Models;
using System.Web.Routing;

namespace ChineseAbs.ABSManagementSite.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DesignAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var filterInfo = new FilterContextInfo(filterContext);
            var userInfo = new UserInfo(filterInfo.Username);
            var authority = new DBAdapter().Authority;

            bool isAuthorized = false;

            //有指定权限中的任何一种权限，即可通过该filter验证
            //AuthorityType.Undefined 表示任意权限
            var flags = CommUtils.GetEnumFlags(m_authorityType);
            foreach (var flag in flags)
            {
                if (authority.IsAuthorized((AuthorityType)flag))
                {
                    isAuthorized = true;
                    break;
                }
            }

            if (!isAuthorized)
            {
                string msg = "Check authority failed:" + (m_authorityType == AuthorityType.Undefined ?
                    "Design" : m_authorityType.ToString());

                var routeValue = new RouteValueDictionary(new
                {
                    controller = "Error",
                    action = "NoAccess",
                    message = msg
                });
                filterContext.Result = new RedirectToRouteResult(routeValue);
                base.OnActionExecuting(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        public AuthorityType AuthorityType { get { return m_authorityType; } set { m_authorityType = value; } }

        private AuthorityType m_authorityType = AuthorityType.Undefined;
    }
}