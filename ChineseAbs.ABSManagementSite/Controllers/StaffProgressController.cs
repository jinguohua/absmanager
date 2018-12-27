using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class StaffProgressController : BaseController
    {
        private class SingleProjectSeriesProcessInfo
        {
            public SingleProjectSeriesProcessInfo()
            {
                
            }

            public string Name { get; set; }

            public string Guid { get; set; }

            public int TotalTaskCount { get; set; }

            public int FinishedTaskCount { get; set; }

            public string TaskPercentCompleted { get; set; }
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetRelatedUsers()
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesGuidsOfCurrentUserName = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.ProjectSeries, PermissionType.Read);
                var projectSeries = m_dbAdapter.ProjectSeries.GetByGuids(projectSeriesGuidsOfCurrentUserName);
                
                List<string> usernameList = new List<string>();
                foreach (var singleProjectSeries in projectSeries)
                {
                    var singleProjectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, singleProjectSeries);
                    var projectSeriesInstance = singleProjectSeriesLogicModel.Instance;
                    var project = singleProjectSeriesLogicModel.CurrentProject.Instance;

                    usernameList.Add(projectSeriesInstance.PersonInCharge);
                    usernameList.Add(projectSeriesInstance.CreateUserName);
                    var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                    var teamMemberUsernames = teamMembers.Select(x => x.UserName).ToList();
                    usernameList.AddRange(teamMemberUsernames);
                }

                usernameList = usernameList.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                Platform.UserProfile.Precache(usernameList);

                var result = usernameList.ConvertAll(x => new
                {
                    userName = Platform.UserProfile.Get(x).UserName,
                    realName = Platform.UserProfile.Get(x).RealName,
                });
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetProjectSeriesProcessInfo(string username)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesGuidsOfCurrentUserName = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.ProjectSeries, PermissionType.Read);
                var projectSeriesGuidsOfUsername = m_dbAdapter.ProjectSeries.GetByPersonInCharge(username).Select(x => x.Guid).ToList();
                var projectSeries = m_dbAdapter.ProjectSeries.GetByGuids(projectSeriesGuidsOfCurrentUserName.Intersect(projectSeriesGuidsOfUsername));

                List<SingleProjectSeriesProcessInfo> singleProjectSeriesProcessInfoList = new List<SingleProjectSeriesProcessInfo>();

                foreach (var singleProjectSeries in projectSeries)
                {
                    var singleProjectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, singleProjectSeries);
                    var projectSeriesInstance = singleProjectSeriesLogicModel.Instance;
                    var project = singleProjectSeriesLogicModel.CurrentProject.Instance;

                    var tasksOfSingleprojectSeries = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId);
                    var finishedTasksOfSingleprojectSeries = tasksOfSingleprojectSeries.Where(x => x.TaskStatus == TaskStatus.Finished).ToList();

                    singleProjectSeriesProcessInfoList.Add(new SingleProjectSeriesProcessInfo  
                    {
                        Name = projectSeriesInstance.Name,
                        Guid = projectSeriesInstance.Guid,
                        TotalTaskCount = tasksOfSingleprojectSeries.Count,
                        FinishedTaskCount = finishedTasksOfSingleprojectSeries.Count,
                        TaskPercentCompleted = CommUtils.Percent(finishedTasksOfSingleprojectSeries.Count, tasksOfSingleprojectSeries.Count)
                    });
                }

                var finishedProjectSeriesCount = singleProjectSeriesProcessInfoList.Count(x => 
                    double.Parse(x.TaskPercentCompleted.Replace("%","")) == 100);
                var result = new 
                {
                    SingleProjectSeriesProcessInfoList = singleProjectSeriesProcessInfoList,
                    ProjectSeriesStatisticInfo = new 
                    {
                        TotalProjectSeriesCount = singleProjectSeriesProcessInfoList.Count,
                        FinishedProjectSeriesCount = finishedProjectSeriesCount,
                        ProjectSeriesPercentCompleted = CommUtils.Percent(finishedProjectSeriesCount, singleProjectSeriesProcessInfoList.Count)
                    }
                };

                return ActionUtils.Success(result);
            });
        }


         [HttpPost]
        public ActionResult GetTasksProcessInfo(string username)
        {
            return ActionUtils.Json(() =>
            {
                var taskShortCodesOfCurrentUserName = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.Task, PermissionType.Read);
                var taskShortCodesOfUsername = m_dbAdapter.Task.GetTasksByPersonInCharge(username).Select(x => x.ShortCode).ToList();
                var taskShortCodes = taskShortCodesOfCurrentUserName.Intersect(taskShortCodesOfUsername).ToList();
                var shortCodes = GetShortCodesOfProjectSeriesExist(taskShortCodes);
                var tasks = m_dbAdapter.Task.GetTasks(shortCodes);

                var finishedTasksCount = tasks.Count(x => x.TaskStatus == TaskStatus.Finished);

                var projectIds = tasks.ConvertAll(x => x.ProjectId).Distinct().ToList();
                var projects = m_dbAdapter.Project.GetProjects(projectIds);

                var projectSeriesIds = projects.Where(x => x.ProjectSeriesId.HasValue).ToList().ConvertAll(x => x.ProjectSeriesId.Value);
                var projectSeriesList = m_dbAdapter.ProjectSeries.GetByIds(projectSeriesIds);
                var tempDictProjectSeries = projectSeriesList.ToDictionary(x => x.Id);

                Dictionary<int, ProjectSeries> dictProjectSeries = new Dictionary<int, ProjectSeries>();
                for (int i = 0; i < projects.Count; i++)
                {
                    var project = projects[i];
                    dictProjectSeries[project.ProjectId] = tempDictProjectSeries[project.ProjectSeriesId.Value];
                }

                var now = DateTime.Now;
                var result = new
                {
                    SingleTaskInfoList = tasks.ConvertAll(x => new
                    {
                        ShortCode = x.ShortCode,
                        Name = x.Description,
                        ProjectSeriesName = dictProjectSeries[x.ProjectId].Name,
                        ProjectSeriesGuid = dictProjectSeries[x.ProjectId].Guid,
                        TaskStatus = x.TaskStatus.ToString(),
                        OverdueDaysCount = x.TaskStatus == TaskStatus.Overdue ? (now - x.EndTime.Value).Days : 0
                    }).ToList(),
                    TasksStatisticInfo = new
                    {
                        TotalTasksCount = tasks.Count,
                        WaittingTasksCount = tasks.Count(x => x.TaskStatus == TaskStatus.Waitting),
                        RunningTasksCount = tasks.Count(x => x.TaskStatus == TaskStatus.Running),
                        FinishedTasksCount = finishedTasksCount,
                        OverdueTasksCount = tasks.Count(x => x.TaskStatus == TaskStatus.Overdue),
                        ErrorTasksCount = tasks.Count(x => x.TaskStatus == TaskStatus.Error),
                        TasksPercentCompleted = CommUtils.Percent(finishedTasksCount, tasks.Count)
                    }
                };

                return ActionUtils.Success(result);
            });
        }
        private List<string> GetShortCodesOfProjectSeriesExist(List<string> shortCodes)
        {
            List<string> taskShortCodes = new List<string>();
            var tasks = m_dbAdapter.Task.GetTasks(shortCodes);

            var projectIds = tasks.Select(x => x.ProjectId).Distinct().ToList();
            var projects = m_dbAdapter.Project.GetProjects(projectIds);
            var dicProjects = projects.ToDictionary(x => x.ProjectId);

            foreach (var task in tasks) 
            {
                if (!dicProjects[task.ProjectId].ProjectSeriesId.HasValue)
                {
                    continue;
                }
                var exist = m_dbAdapter.ProjectSeries.Exists(dicProjects[task.ProjectId].ProjectSeriesId.Value);
                if (exist)
                {
                    taskShortCodes.Add(task.ShortCode);
                }
            }

            return taskShortCodes;
        }

    }
}