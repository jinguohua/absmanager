//using System;
//using System.Web.Mvc;
//using ChineseAbs.ABSManagement;
//using ChineseAbs.ABSManagement.Models;
//using System.Web.Routing;
//using System.Configuration;
//using ChineseAbs.ABSManagementSite.Helpers;
//using System.Web;

//namespace ChineseAbs.ABSManagementSite.Filters
//{
//    public class SuperUserAccessAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var filterInfo = new FilterContextInfo(filterContext);
//            var userInfo = new UserInfo(filterInfo.Username);

//            string msg = "Check authority failed:The user[" + userInfo.UserName + "] do not have authorized to access";
//            var isAuthorized = IsAuthorized(userInfo);

//            if (!isAuthorized)
//            {
//                var routeValue = new RouteValueDictionary(new
//                {
//                    controller = "Error",
//                    action = "NoAccess",
//                    message = msg
//                });
//                filterContext.Result = new RedirectToRouteResult(routeValue);
//                base.OnActionExecuting(filterContext);

//                sendEmail(userInfo);

//                return;
//            }

//            base.OnActionExecuting(filterContext);
//        }

//        private bool IsAuthorized(UserInfo userInfo)
//        {
//            var isAuthorized = new DBAdapter().SuperUser.IsSuperUser();
//            return isAuthorized;
//        }

//        private void sendEmail(UserInfo userInfo)
//        {
//            string ip = HttpContext.Current.Request.UserHostAddress;
//            if (ip == "::1" || ip == "127.0.0.1")
//            {
//                return;
//            }
//            var accountInfo = new DBAdapter().Authority.GetAccountInfoByUserName(userInfo.UserName);

//            string exMessage = "Url: " + HttpContext.Current.Request.Url.AbsoluteUri
//                + "\nReferer: " + (HttpContext.Current.Request.UrlReferrer == null ? string.Empty : HttpContext.Current.Request.UrlReferrer.AbsoluteUri)
//                + "\nUserName: " + userInfo.UserName;

//            if (accountInfo != null)
//            {
//                exMessage += "\nName: " + accountInfo.name
//                    + "\nCompany: " + accountInfo.company
//                    + "\nDepartment: " + accountInfo.department;
//            }

//            exMessage += "\nIP: " + ip
//                + "\nTime: " + DateTime.Now.ToString()
//                + "\nError: 试图访问超级管理员配置权限的页面";

//            var config = ConfigurationManager.GetSection("emailSection") as EmailConfigHandler;
//            if (config != null)
//            {
//                var mailer = config.EmailSetting;
//                mailer.SendMail("ABS Manager错误日志", exMessage, mailer.To);
//            }
//        }
//    }
//}