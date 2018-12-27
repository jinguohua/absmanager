using ChineseAbs.ABSManagementSite.Models;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Filters
{
    public class SiteMapPostConfigAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.Result == null)
            {
                return;
            }

            var result = filterContext.Result as ViewResult;
            if (result == null || result.ViewBag == null || result.ViewBag.SiteMapHtml == null || result.Model == null)
            {
                return;
            }

            var taskViewModel = result.Model as TaskViewModel;
            if (taskViewModel == null)
            {
                return;
            }

            string siteMapHtml = result.ViewBag.SiteMapHtml.ToString();
            if (taskViewModel.ProjectSeriesStage == ABSManagement.Models.ProjectSeriesStage.发行)
            {
                siteMapHtml = siteMapHtml.Replace("存续期管理平台", "发行协作平台")
                    .Replace("/MyProjects", "/ProjectSeries")
                    .Replace("工作列表", "项目面板");
            }
            else if (taskViewModel.ProjectSeriesStage == ABSManagement.Models.ProjectSeriesStage.存续期)
            {
                siteMapHtml = siteMapHtml.Replace("发行协作平台", "存续期管理平台")
                    .Replace("/ProjectSeries", "/MyProjects")
                    .Replace("项目面板", "工作列表");
            }

            result.ViewBag.SiteMapHtml = siteMapHtml;
        }
    }
}
