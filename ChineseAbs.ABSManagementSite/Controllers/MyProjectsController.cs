using ABS.Core.Services;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class MyProjectsController : BaseController
    {
        public TestServices TestServices { get; set; }

        public ActionResult Index(int? page, int? pageSize)
        {
            var authorizedIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(page ?? 1, pageSize ?? 10, true, authorizedIds);

            var viewModel = new ProjectManagerViewModel();
            viewModel.PageInfo = Toolkit.ConvertPageInfo(projects);

            var filterDate = DateTime.Now.AddYears(10);//截止时间设定为未来十年内，仅为触发日期筛选
            foreach (var project in projects.Items)
            {
                var projectView = Toolkit.ConvertProject(project);
                if (projectView != null)
                {
                    var tasks = m_dbAdapter.Task.GetTasks(1, 1, filterDate, project.ProjectId, 
                                new List<TaskStatus>(){ TaskStatus.Waitting,TaskStatus.Running, TaskStatus.Finished, 
                                                        TaskStatus.Skipped,TaskStatus.Overdue,TaskStatus.Error }, 
                                new List<int> { project.ProjectId });
                    if (tasks.Items != null && tasks.Items.Count > 0)
                    {
                        projectView.CurrentTask = Toolkit.ConvertTask(tasks.Items.First());
                    }

                    projectView.Message = m_dbAdapter.News.CountNewsByProjectAndStatus(projectView.Id, NewsConsts.NewsStatusAll)
                        - m_dbAdapter.News.CountNewsByProjectAndStatus(projectView.Id, NewsConsts.NewsStatusRead);
                    viewModel.Projects.Add(projectView);
                }
            }

            viewModel.HasCreateProjectAuthority = m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject);
            viewModel.HasEditModelAuthority = m_dbAdapter.Authority.IsAuthorized(AuthorityType.ModifyModel);
            viewModel.HasEditProjectAuthority = m_dbAdapter.Authority.IsAuthorized(AuthorityType.ModifyTask);
            viewModel.IsSuperUser = m_dbAdapter.SuperUser.IsSuperUser();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetProjects(string projectType)
        {
            return ActionUtils.Json(() =>
            {
                //TODO:
                CommUtils.AssertEquals(projectType, "存续期", "目前仅支持获取存续期产品列表");

                var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var projects = m_dbAdapter.Project.GetProjects(projectIds);
                var result = projects.ConvertAll(x => new
                {
                    name = x.Name,
                    guid = x.ProjectGuid
                });

                return ActionUtils.Success(result);
            });
        }
    }
}