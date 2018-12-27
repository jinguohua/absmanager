using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ProjectSeriesController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProjectSeriesNameAndGuid()
        {
            return ActionUtils.Json(() =>
            {
                var uids = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.ProjectSeries, PermissionType.Read);
                var projectSeriesList = m_dbAdapter.ProjectSeries.GetNameAndGuidByGuids(uids);

                var projectSeriesNameAndGuid = projectSeriesList.ConvertAll(x => new {
                    Name = x.ProjectSeriesName,
                    Guid = x.ProjectSeriesGuid,
                    CurrentProjectGuid = x.CurrentProjectGuid
                });

                return ActionUtils.Success(projectSeriesNameAndGuid);
            });
        }


        /// <summary>
        /// 获取ProjectSeries列表
        /// </summary>
        /// <param name="filterByCreatedDays">按立项时长筛选，单位：天</param>
        /// <param name="filterByStage">按项目状态筛选</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetProjectSeriesList(int filterByCreatedDays = -1, string filterByStage = "")
        {
            return ActionUtils.Json(() =>
            {
                //Check param filterByStage
                var stages = new List<ProjectSeriesStage>();
                if (!string.IsNullOrEmpty(filterByStage))
                {
                    stages = filterByStage.Split('|').ToList().ConvertAll(x => CommUtils.ParseEnum<ProjectSeriesStage>(x));
                }

                var uids = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.ProjectSeries, PermissionType.Read);
                var projectSeriesList = m_dbAdapter.ProjectSeries.GetByGuids(uids);
                projectSeriesList = projectSeriesList.OrderBy(x => x.EstimatedFinishTime).ThenBy(x => x.CreateTime).ToList();
                var projectSeriesLogicModelList = projectSeriesList.Where(x =>{
                    if (filterByCreatedDays >= 0
                        && (DateTime.Now - x.CreateTime).TotalDays > filterByCreatedDays) {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(filterByStage)) {
                        return stages.Contains(x.Stage);
                    }
                    
                    return true;
                }).ToList().ConvertAll(x => new ProjectSeriesLogicModel(CurrentUserName, x));

                var dict = new Dictionary<string, string>();
                var dictStatus = new Dictionary<string, string>();
                //已完成工作数，未完成工作数，成员数（包含创建者、负责人）
                var dictTaskCount = new Dictionary<string, Tuple<int, int>>();
                var dictPersonCount = new Dictionary<string, int>();

                foreach (var projectSeriesLogicModel in projectSeriesLogicModelList)
                {
                    var projectSeries = projectSeriesLogicModel.Instance;
                    dictStatus[projectSeries.Guid] = "准备";
                    dict[projectSeries.Guid] = "0.00%";
                    dictTaskCount[projectSeries.Guid] = new Tuple<int, int>(0, 0);
                    

                    var allMembers = new List<string>();

                    if (projectSeriesLogicModel.CurrentProject != null)
                        projectSeriesLogicModel.CurrentProject.Team.Chiefs.Select(x => x.UserName).ToList();

                    var logicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeries);
                    if (logicModel.CurrentProject != null)
                    {
                        var project = logicModel.CurrentProject.Instance;
                        var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                        var teamMemberUserNames = teamMembers.Select(x => x.UserName).ToList();
                        var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);
                        var teamAdminUserNames = teamAdmins.Select(x => x.UserName).ToList();
                        allMembers.AddRange(teamMemberUserNames);
                        allMembers.AddRange(teamAdminUserNames);

                        var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId);
                        var finishedTaskCount = tasks.Count(x => x.TaskStatus == TaskStatus.Finished);
                        if (tasks.Count != 0)
                        {
                            dict[projectSeries.Guid] = CommUtils.Percent(finishedTaskCount, tasks.Count);
                            dictStatus[projectSeries.Guid] = finishedTaskCount == 0 ? "准备" :
                                (finishedTaskCount == tasks.Count ? "完成" : "进行中");

                            dictTaskCount[projectSeries.Guid] = new Tuple<int, int>(finishedTaskCount, tasks.Count);
                        }
                    }
                    allMembers = allMembers.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    dictPersonCount[projectSeries.Guid] = allMembers.Count;
                }

                var allUserNames = projectSeriesLogicModelList.Select(x => x.Instance.PersonInCharge)
                    .Concat(projectSeriesLogicModelList.Select(x => x.Instance.CreateUserName)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

                var nicknames = UserService.GetNicknames(allUserNames);
                var result = projectSeriesLogicModelList.ConvertAll(logicModel => {
                    var x = logicModel.Instance;
                    return new { 
                        name = x.Name,
                        guid = x.Guid,
                        type = x.Type.ToString(),
                        personInCharge = new {
                                userName = x.PersonInCharge,
                                realName = nicknames.ContainsKey(x.Name) ? nicknames[x.Name] : x.Name,
                        },
                        createUser = new
                        {
                            userName = x.CreateUserName,
                            realName = nicknames.ContainsKey(x.Name) ? nicknames[x.Name] : x.Name,
                        },
                        isCreator= IsCurrentUser(x.CreateUserName),
                        createTimeStamp = x.CreateTime.ToString("yyyy-MM-dd"),
                        estimatedFinishTime = Toolkit.DateToString(x.EstimatedFinishTime),
                        remainingDayCount = x.EstimatedFinishTime.HasValue ? ((int)(x.EstimatedFinishTime.Value - DateTime.Today).TotalDays).ToString() : "-",
                        percentCompleted = dict[x.Guid],
                        finishedTaskCount = dictTaskCount[x.Guid].Item1,
                        taskCount = dictTaskCount[x.Guid].Item2,
                        status = dictStatus[x.Guid],
                        stage = x.Stage.ToString(),
                        currentProjectGuid = (logicModel.CurrentProject == null ? "" : logicModel.CurrentProject.Instance.ProjectGuid),
                        permission = new {
                            write = m_dbAdapter.Permission.HasPermission(CurrentUserName, x.Guid, PermissionType.Write)
                        },
                        personCount = dictPersonCount[x.Guid],
                    };
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetProjectSeriesProcessInfo(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                CheckPermission(PermissionObjectType.ProjectSeries, projectSeriesGuid, PermissionType.Read);
                var tasks = m_dbAdapter.Task.GetTasksByProjectId(projectSeriesLogicModel.CurrentProject.Instance.ProjectId);
                var finishedTaskCount = tasks.Count(x => x.TaskStatus == TaskStatus.Finished);
                var percentCompleted = CommUtils.Percent(finishedTaskCount, tasks.Count);

                var projectSeries = projectSeriesLogicModel.Instance;
                

                var personInChargeInfo = Platform.UserProfile.Get(projectSeries.PersonInCharge);
                var createUserInfo = Platform.UserProfile.Get(projectSeries.CreateUserName);

                var result = new
                {
                    name = projectSeries.Name,
                    personInCharge = new {
                        userName = personInChargeInfo.UserName,
                        realName = personInChargeInfo.RealName
                    },
                    createUser = new
                    {
                        userName = createUserInfo.UserName,
                        realName = createUserInfo.RealName
                    },
                    createTimeStamp = projectSeries.CreateTime.ToString("yyyy-MM-dd"),
                    estimatedFinishTime = Toolkit.DateToString(projectSeries.EstimatedFinishTime),
                    remainingDayCount = projectSeries.EstimatedFinishTime.HasValue ? ((int)(projectSeries.EstimatedFinishTime.Value - DateTime.Today).TotalDays).ToString() : "-",

                    stage = projectSeries.Stage.ToString(),

                    taskCount = tasks.Count,
                    finishedTaskCount = finishedTaskCount,
                    percentCompleted = percentCompleted,
                    projectType = projectSeries.Type.ToString(),
                    email = string.IsNullOrWhiteSpace(projectSeries.Email)? null:projectSeries.Email,
                };

                return ActionUtils.Success(result);
            });
        }


        [HttpPost]
        public ActionResult GetProjectSeriesByUsername()
        {
            return ActionUtils.Json(() =>
            {
                var currentUsername = CurrentUserName;
                var uids = m_dbAdapter.Permission.GetObjectUids(currentUsername, PermissionObjectType.ProjectSeries, PermissionType.Read);
                var projectSeriesList = m_dbAdapter.ProjectSeries.GetByGuids(uids).ToList();

                var result = new
                {
                    ChiefsProjectSeries = projectSeriesList.Where(x =>
                        x.RecordStatus == RecordStatus.Valid
                        && (IsCurrentUser(x.CreateUserName) || IsCurrentUser(x.PersonInCharge))
                    ).ToList().ConvertAll(x => new
                    {
                        Name = x.Name,
                        Guid = x.Guid,
                    }),
                    TeamAdminProjectSeries = projectSeriesList.Where(x =>
                        x.RecordStatus == RecordStatus.Valid
                        && IsTeamAdmin(x.Guid, CurrentUserName)
                    ).ToList().ConvertAll(x => new
                    {
                        Name = x.Name,
                        Guid = x.Guid,
                    }),
                    CreatorProjectSeries = projectSeriesList.Where(x =>
                       x.RecordStatus == RecordStatus.Valid
                       && IsCurrentUser(x.CreateUserName)
                    ).ToList().ConvertAll(x => new
                    {
                        Name = x.Name,
                        Guid = x.Guid,
                    })
                };

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CreateProjectSeries(string name, string projectSeriesType, string personInCharge, string createTime, string estimatedFinishTime, string email)
        {
            return ActionUtils.Json(() =>
            {
                var logicModel = new ProjectSeriesLogicModel(CurrentUserName);
                logicModel.NewProjectSeries(name, projectSeriesType, personInCharge, createTime, estimatedFinishTime, email);
                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult ModifyProjectSeriesInfo(string projectSeriesGuid, string name, string projectSeriesType, string personInCharge, string createTime, string estimatedFinishTime, string email)
        {
            return ActionUtils.Json(() =>
            {
                ValidateUtils.Name(name, "项目名称");
                CommUtils.Assert(email.Length <= 38, "名称不能超过38个字符数");
                CommUtils.AssertHasContent(personInCharge, "[项目负责人]不能为空");
                CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(personInCharge), "[用户]不存在");

                CommUtils.AssertHasContent(createTime, "[立项日期]不能为空");
                CommUtils.AssertHasContent(estimatedFinishTime, "[计划完成日期]不能为空");

                var valStartTime = DateTime.Parse(createTime);
                var valEstimatedFinishTime = DateTime.Parse(estimatedFinishTime);
                CommUtils.Assert(valEstimatedFinishTime >= valStartTime, "计划完成日期[{0}]必须大于等于立项日期[{1}]", valEstimatedFinishTime, valStartTime);

                var type = CommUtils.ParseEnum<ProjectSeriesType>(projectSeriesType);
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var projectSeries = projectSeriesLogicModel.Instance;
                var project = projectSeriesLogicModel.CurrentProject.Instance;

                CommUtils.Assert(IsCurrentUser(projectSeries.CreateUserName)
                    || IsCurrentUser(projectSeries.PersonInCharge)
                    || m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的管理员/创建者/负责人，无法进行修改", CurrentUserName, projectSeries.Name);

                CommUtils.Assert(!m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, CurrentUserName)
                    || personInCharge == projectSeries.PersonInCharge,
                    "当前用户[{0}]是项目[{1}]的管理员，无法修改负责人", CurrentUserName, projectSeries.Name);

                var isAdmin = projectSeries.CreateUserName.Equals(personInCharge, StringComparison.CurrentCultureIgnoreCase)
                || projectSeries.PersonInCharge.Equals(personInCharge, StringComparison.CurrentCultureIgnoreCase); //////////////

                var permissionLogicModel = new PermissionLogicModel(CurrentUserName, projectSeries);
                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);
                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                var teamMember = new TeamMember
                {
                    UserName = projectSeries.PersonInCharge,
                    ProjectId = project.ProjectId,
                    Read = true,
                    Write = false,
                    Execute = false
                };



                if (!isAdmin)
                {
                    if (!projectSeries.CreateUserName.Equals(projectSeries.PersonInCharge,StringComparison.CurrentCultureIgnoreCase))
                    {
                        //删除原有负责人的所有权限，将其添加为项目成员
                        permissionLogicModel.RemoveUserPermissionByProjectSeries(new List<string> { projectSeries.PersonInCharge });
                        m_dbAdapter.TeamMember.Add(teamMember);
                        permissionLogicModel.AddUserPermissionByProjectSeries(new List<TeamMember> { teamMember }, new List<TeamAdmin>(), new List<string>());
                    }

                    foreach (var item in teamMembers)
                    {
                        //如果新的负责人为原有的项目成员，则在项目成员里移除
                        if (item.UserName.Equals(personInCharge, StringComparison.CurrentCultureIgnoreCase))
                        {
                            m_dbAdapter.TeamMember.Remove(item);
                            permissionLogicModel.RemoveUserPermissionByProjectSeries(new List<string> { personInCharge });
                        }
                    }

                    foreach (var item in teamAdmins)
                    {
                        //如果新的负责人为原有的项目管理员，则在项目管理员里移除
                        if (item.UserName.Equals(personInCharge, StringComparison.CurrentCultureIgnoreCase))
                        {
                            m_dbAdapter.TeamAdmin.Delete(item);
                            permissionLogicModel.RemoveUserPermissionByProjectSeries(new List<string> { personInCharge });
                        }
                    }

                    permissionLogicModel.AddUserPermissionByProjectSeries(new List<TeamMember>(), new List<TeamAdmin>(), new List<string> { personInCharge });

                }
                else
                {
                    if (!projectSeries.PersonInCharge.Equals(personInCharge,StringComparison.CurrentCultureIgnoreCase))
                    {
                        //删除原有负责人的所有权限，将其添加为项目成员
                        permissionLogicModel.RemoveUserPermissionByProjectSeries(new List<string> { projectSeries.PersonInCharge });
                        m_dbAdapter.TeamMember.Add(teamMember);
                        permissionLogicModel.AddUserPermissionByProjectSeries(new List<TeamMember> { teamMember }, new List<TeamAdmin>(), new List<string>());
                    }
                }

                //同步产品系列名字与产品名字一致
                if (projectSeries.Name != name)
                {
                    project.Name = name;
                    m_dbAdapter.Project.Update(project);
                }

                projectSeries.Name = name;
                projectSeries.Type = type;
                projectSeries.CreateTime = valStartTime;
                projectSeries.EstimatedFinishTime = valEstimatedFinishTime;
                projectSeries.PersonInCharge = personInCharge;
                projectSeries.Email = email;

                m_dbAdapter.ProjectSeries.UpdateProjectSeries(projectSeries);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult RemoveProjectSeries(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeries = m_dbAdapter.ProjectSeries.GetByGuid(projectSeriesGuid);

                CommUtils.Assert(IsCurrentUser(projectSeries.CreateUserName),
                "当前用户[{0}]不是项目[{1}]的创建者", CurrentUserName, projectSeries.Name);

                var result = m_dbAdapter.ProjectSeries.RemoveByGuid(projectSeriesGuid);
                return ActionUtils.Success(result);
            });
        }


        [HttpPost]
        public ActionResult ExitProjectSeries(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.ProjectSeries, projectSeriesGuid, PermissionType.Read);
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var projectSeries = projectSeriesLogicModel.Instance;

                CommUtils.Assert(!IsCurrentUser(projectSeries.CreateUserName),
                "当前用户[{0}]是项目[{1}]的创建者,不能退出项目", CurrentUserName, projectSeries.Name);

                var projectLogicModel = projectSeriesLogicModel.CurrentProject;
                var project = projectLogicModel.Instance;
                var permissionLogicModel = new PermissionLogicModel(CurrentUserName, projectSeries);

                if (IsCurrentUser(projectSeries.PersonInCharge))
                {
                    projectSeries.PersonInCharge = projectSeries.CreateUserName;
                    permissionLogicModel.RemoveUserPermissionByProjectSeries(new List<string>() { CurrentUserName });
                }
                else if (m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, CurrentUserName))
                {
                    var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);
                    var teamAdmin = teamAdmins.FirstOrDefault(x => x.UserName.Equals(CurrentUserName, StringComparison.CurrentCultureIgnoreCase));

                    m_dbAdapter.TeamAdmin.Delete(teamAdmin);
                    permissionLogicModel.RemoveUserPermissionByProjectSeries(new List<string>() { CurrentUserName });
                }
                else if(m_dbAdapter.TeamMember.IsTeamMember(project.ProjectId, CurrentUserName))
                {
                    var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                    var teamMember = teamMembers.FirstOrDefault(x => x.UserName.Equals(CurrentUserName, StringComparison.CurrentCultureIgnoreCase));

                    m_dbAdapter.TeamMember.Remove(teamMember);
                    permissionLogicModel.RemoveUserPermissionByProjectSeries(new List<string>() { CurrentUserName });
                }
                
                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult GetOneProjectSeriesInfo(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var projectSeries = projectSeriesLogicModel.Instance;
                var project = projectSeriesLogicModel.CurrentProject.Instance;

                CommUtils.Assert(IsCurrentUser(projectSeries.CreateUserName)
                    || IsCurrentUser(projectSeries.PersonInCharge)
                    || m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的管理员/创建者/负责人", CurrentUserName, projectSeries.Name);

                CheckPermission(PermissionObjectType.ProjectSeries, projectSeriesGuid, PermissionType.Write);

                var result = new
                {
                    Name = projectSeries.Name,
                    Type = projectSeries.Type.ToString(),
                    StartTime = projectSeries.CreateTime.ToString("yyyy-MM-dd"),
                    EndTime = projectSeries.EstimatedFinishTime.HasValue?projectSeries.EstimatedFinishTime.Value.ToString("yyyy-MM-dd"):"",
                    PersonInCharge = Platform.UserProfile.Get(projectSeries.PersonInCharge).UserName,
                    Email = projectSeries.Email,
                };
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetUserInfo(string userName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(userName, "用户名不能为空");

                var queryUserEnterpriseId = m_dbAdapter.Authority.GetEnterpriseId(userName);
                var currentUserEnterpriseId = m_dbAdapter.Authority.GetEnterpriseId(CurrentUserName);
                CommUtils.Assert(queryUserEnterpriseId.HasValue && currentUserEnterpriseId.HasValue
                    && queryUserEnterpriseId.Value == currentUserEnterpriseId.Value, "查询用户和当前登录用户不在同一机构");
                
                var accountInfo = UserService.GetUserByName(userName);
                var result = new
                {
                    realName = accountInfo != null? accountInfo.Name : userName,
                    cellphone = accountInfo != null ? accountInfo.PhoneNumber : "",
                    email = accountInfo != null ? accountInfo.Email : ""
                };
                return ActionUtils.Success(result);


            });
        }


        private bool IsTeamAdmin(string projectSeriesGuid, string username) 
        {
            var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
            var project = projectSeriesLogicModel.CurrentProject.Instance;
            var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);

            return teamAdmins.Any(x => x.UserName == username);
        }
    }
}