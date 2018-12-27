//using ChineseAbs.ABSManagement;
//using ChineseAbs.ABSManagement.Models;
//using ChineseAbs.ABSManagement.Utils;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Security;


//namespace ChineseAbs.ABSManagementSite.Filters
//{
//    public class GlobalAuthorizeAttribute : AuthorizeAttribute
//    {
//        /// <summary>
//        /// Override authorization
//        /// </summary>
//        /// <param name="httpContext">http Context</param>
//        /// <returns>Whether authorized</returns>
//        protected override bool AuthorizeCore(HttpContextBase httpContext)
//        {
//            if (!httpContext.User.Identity.IsAuthenticated)
//            {
//                httpContext.Response.StatusCode = 401;
//                return false;
//            } 
           

//            if (ChineseAbs.ABSManagement.Utils.CommUtils.IsLocalDeployed())
//            {
//                if (Membership.FindUsersByName(httpContext.User.Identity.Name).Count == 0)
//                {
//                    httpContext.Response.StatusCode = 401;
//                    return false;
//                }

//                return base.AuthorizeCore(httpContext);
//            }

//            var userInfo = new UserInfo(httpContext.User.Identity.Name);

//            var dbAdapter = new DBAdapter();
//            bool authed = dbAdapter.Authority.IsAuthorizedUser(httpContext.User.Identity.Name);
//            if (!authed)
//            {
//                httpContext.Response.StatusCode = 403;
//                return false;
//            }
//            else
//            {
//                var strFunctionId = IsCooperationPlatform(httpContext.Request.RequestContext.RouteData)
//                    ? WebConfigUtils.CooperationPlatformFunctionId : WebConfigUtils.ABSManagementFunctionId;

//                bool opened = dbAdapter.Authority.IsOpenFunction(httpContext.User.Identity.Name, strFunctionId);
//                if (!opened)
//                {
//                    httpContext.Response.StatusCode = 403;
//                    return false;
//                }
//            }

//            return base.AuthorizeCore(httpContext);
//        }

//        //当前访问是否为发行协作平台
//        private bool IsCooperationPlatform(System.Web.Routing.RouteData routeData)
//        {
//            var cooperationPlatformControllers = new string[] { "Dashboard", "ProjectSeries" };
//            return routeData.Values.ContainsKey("controller")
//                && cooperationPlatformControllers.Contains(routeData.Values["controller"]);
//        }

//        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
//        {
//            base.HandleUnauthorizedRequest(filterContext);

//            switch (filterContext.HttpContext.Response.StatusCode)
//            {
//                //Reference: https://en.wikipedia.org/wiki/List_of_HTTP_status_codes
//                case 401:
//                    if (filterContext.HttpContext.Request.IsAjaxRequest())
//                    {
//                        filterContext.Result = new Http401Result();
//                    }
//                    else
//                    {
//                        filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
//                    }
//                    break;
//                case 403:
//                    FormsAuthentication.SignOut();
//                    filterContext.Result = new RedirectResult("/Error/NoAccess");
//                    break;
//                default:
//                    break;
//            }
//        }

//        //Reference: https://stackoverflow.com/questions/2580596/how-do-you-handle-ajax-requests-when-user-is-not-authenticated?answertab=active#tab-top
//        private class Http401Result : ActionResult
//        {
//            public override void ExecuteResult(ControllerContext context)
//            {
//                context.HttpContext.Response.StatusCode = 401;
//                context.HttpContext.Response.End();
//            }
//        }
//    }
//}