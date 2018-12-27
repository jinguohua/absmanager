using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using SAFS.Utility.Web;

namespace ChineseAbs.ABSManagementSite.Filters
{
    public class ErrorHandlerFilterAttribute : FilterAttribute, IExceptionFilter
    {
        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (IsAjax(filterContext))
            {
                JsonResultDataEntity<string> result = new JsonResultDataEntity<string>();
                result.Message = filterContext.Exception.Message;
                result.Code = 1;
                filterContext.ExceptionHandled = true;
                filterContext.Result = new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                string controllerName = (String)filterContext.RouteData.Values["controller"];
                string actionName = (String)filterContext.RouteData.Values["action"];
                System.Web.Mvc.HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                filterContext.ExceptionHandled = true;
                filterContext.Result = new ViewResult() { ViewName = "Error", ViewData = new ViewDataDictionary(model), TempData = filterContext.Controller.TempData };
            }


            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            //filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            string errorMessages = "";
            var ex = filterContext.Exception;
            while (ex != null)
            {
                errorMessages += ex.Message + ex.StackTrace;
                ex = ex.InnerException;
            }
            log4net.LogManager.GetLogger("Error").Error(errorMessages);
        }
    }
}