using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ChineseAbs.ABSManagementSite.Common;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DashboardController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPermission(string userName, string uid, string objectType, string permissionType)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(userName), "用户[{0}]不存在", userName);

                var permissionObjectType = CommUtils.ParseEnum<PermissionObjectType>(objectType);
                var newPermissionType = CommUtils.ParseEnum<PermissionType>(permissionType);

                CheckPermission(permissionObjectType, uid, PermissionType.Write);

                var task = m_dbAdapter.Task.GetTask(uid);
                var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);
                var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);

                var permission = new Permission
                {
                    Type = newPermissionType,
                    UserName = userName,
                    ObjectUniqueIdentifier = uid,
                    ObjectType = permissionObjectType
                };

                var realName = m_dbAdapter.Authority.GetUserRealName(userName);
                CommUtils.Assert(!m_dbAdapter.Permission.HasPermission(permission),
                    "项目[{0}]中，用户[{1}({2})]已有[{3}]权限", 
                    projectSeries.Name,realName,userName,newPermissionType);

                m_dbAdapter.Permission.NewPermission(permission);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, project.ProjectId,
                    "项目[" + projectSeries.Name + "]中，工作[" + task.Description + "]，用户[" +
                    realName + "(" + userName + ")]添加[" + newPermissionType + "]权限", "");

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult DeletePermission(string userName, string uid, string objectType, string permissionType)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(userName), "用户[{0}]不存在", userName);

                var permissionObjectType = CommUtils.ParseEnum<PermissionObjectType>(objectType);
                var newPermissionType = CommUtils.ParseEnum<PermissionType>(permissionType);

                CheckPermission(permissionObjectType, uid, PermissionType.Write);

                var task = m_dbAdapter.Task.GetTask(uid);
                var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);
                var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);

                var permission = new Permission
                {
                    Type = newPermissionType,
                    UserName = userName,
                    ObjectUniqueIdentifier = uid,
                    ObjectType = permissionObjectType
                };

                var realName = m_dbAdapter.Authority.GetUserRealName(userName);
                CommUtils.Assert(m_dbAdapter.Permission.HasPermission(permission),
                    "项目[{0}]中，用户[{1}({2})]的[{3}]权限有误，请刷新后重试",
                    projectSeries.Name, realName, userName, newPermissionType);

                var result = m_dbAdapter.Permission.GetPermission(
                    permission.UserName,permission.ObjectUniqueIdentifier,permission.Type);

                CommUtils.Assert(result.Id != 0, "项目[" + projectSeries.Name + "]中，用户[" + realName +
                        "(" + userName + ")]的[" + newPermissionType + "]权限有误，请刷新后重试");

                m_dbAdapter.Permission.DeletePermission(result);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, project.ProjectId,
                    "删除项目[" + projectSeries.Name + "]中，工作[" + task.Description + "]，用户[" +
                    realName + "(" + userName + ")]的[" + newPermissionType + "]权限", "");

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult GetTasks(string projectGuid, string taskStatusFilter, int taskCountLimit)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到[{0}]", project.Name);
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var taskStatusList = CommUtils.Split(taskStatusFilter).ToList()
                    .ConvertAll(CommUtils.ParseEnum<TaskStatus>);

                var allTasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId);
                var dictTasks = allTasks.GroupBy(x => x.TaskStatus).ToDictionary(x => x.Key, y => y.ToList());
                var resultTask = new List<Task>();

                foreach (var status in taskStatusList)
                {
                    if (!dictTasks.ContainsKey(status))
                    {
                        continue;
                    }

                    var currentStatusTasks = dictTasks[status];
                    currentStatusTasks = currentStatusTasks.OrderBy(x => x.EndTime).ToList();

                    while (resultTask.Count < taskCountLimit && currentStatusTasks.Count > 0)
                    {
                        var task = currentStatusTasks.First();
                        resultTask.Add(task);
                        currentStatusTasks.Remove(task);
                    }
                }

                var allTasksCount = allTasks.Count;
                var finishedTaskCount = allTasks.Count(x => x.TaskStatus == TaskStatus.Finished);
                var runningTaskCount = allTasks.Count(x => x.TaskStatus == TaskStatus.Running);
                var finishedPercent = CommUtils.Percent(finishedTaskCount, allTasksCount);

                Platform.UserProfile.Precache(resultTask.Select(x => x.PersonInCharge));

                var now = DateTime.Now;
                var result = new {
                    tasks = resultTask.ConvertAll(x => new { 
                    ShortCode = x.ShortCode,
                    Description = x.Description,
                    StartTime = Toolkit.DateToString(x.StartTime),
                    EndTime = Toolkit.DateToString(x.EndTime),
                    PrevTaskShortCodeArray = x.PrevTaskShortCodeArray,
                    PrevTaskIdArray = x.PrevTaskIdArray,
                    TaskExtension = x.TaskExtension,
                    Status = x.TaskStatus.ToString(),
                    personInCharge = x.PersonInCharge,
                    personInChargeUserProfile = Platform.UserProfile.Get(x.PersonInCharge),
                    OverdueDaysCount = x.TaskStatus == TaskStatus.Overdue ? (now - x.EndTime.Value).Days : 0
                }),
                    statisticInfo = new
                    {
                        allTasksCount = allTasksCount,
                        finishedTaskCount = finishedTaskCount,
                        runningTaskCount = runningTaskCount,
                        finishedPercent = finishedPercent
                    }
                };
                return ActionUtils.Success(result);
            });
        }

        [HttpGet]
        public ActionResult GetUserAvatar(string userName)
        {
            var avatar = CommUtils.DefaultAvatarPath;

            //TODO: 跨机构是否显示头像？
            var queryUserEnterpriseId = m_dbAdapter.Authority.GetEnterpriseId(userName);
            var currentUserEnterpriseId = m_dbAdapter.Authority.GetEnterpriseId(CurrentUserName);
            if (queryUserEnterpriseId.HasValue && currentUserEnterpriseId.HasValue
                && queryUserEnterpriseId.Value == currentUserEnterpriseId.Value)
            {
                var userProfile = m_dbAdapter.Authority.GetUserProfile(userName);
                if (userProfile != null)
                {
                    avatar = ChineseAbs.Logic.Common.AvatarHelper.GetAvatar(userProfile.AvatarPath);
                }
            }

            return Content(avatar);
        }
    }
}