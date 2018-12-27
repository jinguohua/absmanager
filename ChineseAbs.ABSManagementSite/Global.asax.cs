using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
//using ChineseAbs.Authorize.Mvc;
using SAFS.Autofac;
using SAFS.Core.Initialize;
using SAFS.Core.Reflection;
using SAFS.SiteBase.Initialize;
using ABS.ABSManagementSite.AutoMapper;

namespace ChineseAbs.ABSManagementSite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected static ConcurrentQueue<UserVisitLog> m_logs = new ConcurrentQueue<UserVisitLog>();
        public static string SystemVersion = string.Empty;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();


            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.CreateMappings();
            //设置系统版本
            SystemVersion = SysUtils.GetDllVersion();
            Initialize();
            Aspose.Words.License license = new Aspose.Words.License();
            license.SetLicense("Aspose.Words.lic");
        }

        protected void Initialize()
        {
            Func<Assembly, bool> assemblyPredicate = new Func<Assembly, bool>(o =>
            {
                return (
                o.FullName.StartsWith("ABS")
                || o.FullName.StartsWith("SAFS")
                );
            });
            var finder = new CurrentDomainAssemblyFinder(assemblyPredicate);
            IFrameworkInitializer initializer = new FrameworkInitializer(finder)
            {
                MvcIocInitializer = new AutofacMvcIocInitializer(),
                WebApiIocInitializer = new AutofacWebApiIocInitializer(),
                DatabaseInitializer = new DatabaseInitializer(),
                DataHandlerInitializer = new DataHandlerInitializer(),
            };
            initializer.Initialize();

            SAFS.Core.Caching.CacheManager.AddProvider(new SAFS.Core.Caching.RuntimeMemoryCacheProvider() { Enabled = true });

            Func<Assembly, bool> enumFinder = new Func<Assembly, bool>(o =>
            {
                return (
                o.FullName.StartsWith("ABS")
                );
            });
            ABS.Core.EnumHelper.AssemberFinder = new CurrentDomainAssemblyFinder(enumFinder);
            int count = ABS.Core.EnumHelper.Enums.Count;
        }

        protected void Application_BeginRequest(Object source, EventArgs e)
        {
            //if (!Context.Request.IsSecureConnection)
            //{
            //    if (!Request.IsLocal && !CommUtils.IsLocalDeployed()
            //        && !CommUtils.ForceUseHttp())
            //    {
            //        //Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"), false);
            //        //Context.ApplicationInstance.CompleteRequest();
            //    }
            //}
        }

        //protected void Application_AcquireRequestState()
        //{
        //    if (User.Identity.IsAuthenticated)// && Session != null
        //    {
        //        var op = SAFS.Core.Context.ApplicationContext.Current.Operator;
        //        {
        //            op.UserID = User.Identity.GetUserId();
        //            op.Name = User.Identity.Name;
        //            op.NickName = 
        //            op.Ip = Request.UserHostAddress;
        //        }
        //    }
        //}

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //try
            //{
            //    var requestRawUrl = HttpContext.Current.Request.RawUrl.ToLower();

            //    var filterLinks = new[] { "/__browserlink/", "/images/", "/scripts/",
            //        "/content/", "/Task/GetTaskStatus", "/Task/GetTaskStatusHistory", 
            //        "/bundles/", "/Base/GetMenuList", "/Schedule/GetNewestTaskStatus"};
            //    if (filterLinks.All(x => !requestRawUrl.StartsWith(x, true, System.Globalization.CultureInfo.CurrentCulture)))
            //    {
            //        UserVisitLog log = new UserVisitLog();
            //        if (HttpContext.Current.User != null
            //            && HttpContext.Current.User.Identity.IsAuthenticated)
            //        {
            //            log.Username = HttpContext.Current.User.Identity.Name;
            //        }
            //        else
            //        {
            //            log.Username = "Anonymous";
            //            return;
            //        }
            //        if (log.Username != "safs_robot")
            //        {
            //            log.RequestUrl = HttpContext.Current.Request.RawUrl;
            //            log.UserAgent = HttpContext.Current.Request.UserAgent;
            //            log.Ip = HttpContext.Current.Request.UserHostAddress;
            //            log.TimeStamp = DateTime.Now;
            //            m_logs.Enqueue(log);

            //            if (m_logs.Count() >= 30)
            //            {
            //                List<UserVisitLog> logs = new List<UserVisitLog>();
            //                UserVisitLog log2;
            //                while (m_logs.TryDequeue(out log2))
            //                {
            //                    logs.Add(log2);
            //                }

            //                new DBAdapter().UserVisitLog.AddUserVisitLogs(logs);
            //            }
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //}
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Exception objError = Server.GetLastError().GetBaseException();
            //var msg = string.Empty;
            //msg += "sender=" + sender.ToString() + Environment.NewLine;
            //msg += "EventArgs=" + e.ToString() + Environment.NewLine;
            //msg += "Now=" + System.DateTime.Now.ToString();
            //msg += "HttpContext.Current.Request.Url=" + System.Web.HttpContext.Current.Request.Url.ToString();
            //msg += "Message=" + objError.Message;
            //msg += "Source=" + objError.Source;
            //msg += "StackTrace=" + objError.StackTrace;
            //msg += "TargetSite.DeclaringType.FullName=" + objError.TargetSite.DeclaringType.FullName;
            //msg += "TargetSite.Name=" + objError.TargetSite.Name;
            //Framework.Logger.LogHelper.WriteLog(msg);
        }
    }
}