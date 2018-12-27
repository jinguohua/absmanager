using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ChineseAbs.ABSManagement.Utils;
using System;
using ChineseAbs.ABSManagement.LogicModels;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class TeamAdminController : BaseController
    {
        //批量增加管理员
        [HttpPost]
        public ActionResult AddTeamAdmins(string projectSeriesGuid, string userNames)
        {
            return ActionUtils.Json(() =>
            {
                //判断操作者权限
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                CommUtils.Assert(IsAdmin(projectSeriesLogicModel.Instance, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的创建者/负责人", Platform.UserProfile.GetDisplayRealNameAndUserName(CurrentUserName),
                    projectSeriesLogicModel.Instance.Name);

                //判断传入userName是否有效
                var userNameList = CommUtils.Split(userNames);
                foreach (var userName in userNameList)
                {
                    CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(userName), "用户[{0}]不存在", userName);
                    CommUtils.Assert(!IsAdmin(projectSeriesLogicModel.Instance, userName),
                        "用户[{0}]是产品创建者/负责人，无法增加至项目管理员", userName);
                }

                //判断项目管理员是否已经添加
                var project = projectSeriesLogicModel.CurrentProject.Instance;
                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);
                foreach (var teamAdmin in teamAdmins)
                {
                    CommUtils.Assert(!userNameList.Contains(teamAdmin.UserName),
                        "项目管理员中已经存在用户[{0}]", teamAdmin.UserName);
                }

                var projectActivityLogicModel = projectSeriesLogicModel.CurrentProject.Activity;

                //添加项目管理员
                var teamMemberList = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                var teamMemberDic = teamMemberList.ToDictionary(x => x.UserName);
                var teamMembeUserNames = teamMemberList.Select(x => x.UserName).ToList();
                foreach (var userName in userNameList)
                {
                    var teamAdmin = new TeamAdmin
                    {
                        UserName = userName,
                        ProjectId = project.ProjectId
                    };

                    m_dbAdapter.TeamAdmin.New(teamAdmin);
                    projectActivityLogicModel.Add(project.ProjectId, ActivityObjectType.TeamAdmin, teamAdmin.Guid, "增加项目管理员：" + Platform.UserProfile.Get(teamAdmin.UserName).RealName);

                    if (teamMembeUserNames.Contains(userName))
                    {
                        m_dbAdapter.TeamMember.Remove(teamMemberDic[userName]);
                    }
                }

                var teamMembers = new List<TeamMember>();
                teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);
                var newTeamAdmins = teamAdmins.Where(x => userNameList.Contains(x.UserName)).ToList();
                var adminUserNames = new List<string>();

                var permissionLogicModel = new PermissionLogicModel(CurrentUserName, projectSeriesLogicModel.Instance);
                permissionLogicModel.AddUserPermissionByProjectSeries(teamMembers, newTeamAdmins, adminUserNames);

                return ActionUtils.Success(1);
            });
        }

        //获取组管理员及统计信息
        [HttpPost]
        public ActionResult GetTeamAdminsOfUserGroup(string projectSeriesGuid, string userGroupGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var projectSeries = projectSeriesLogicModel.Instance;
                CommUtils.Assert(IsAdmin(projectSeriesLogicModel.Instance, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的创建者/负责人", Platform.UserProfile.GetDisplayRealNameAndUserName(CurrentUserName),
                    projectSeriesLogicModel.Instance.Name);

                var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
                CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

                var projectId = projectSeriesLogicModel.CurrentProject.Instance.ProjectId;
                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(projectId);
                var teamAdminUserNames = teamAdmins.Select(x => x.UserName).ToList();

                var userGroupMaps = m_dbAdapter.UserGroupMap.GetByUserGroupGuid(userGroupGuid);
                var userGroupMapUserNames = userGroupMaps.Select(x => x.UserName).ToList();

                var teamAdminsOfUserGroup = teamAdminUserNames.Intersect(userGroupMapUserNames).ToList();
                Platform.UserProfile.Precache(userGroupMapUserNames);
                var result = new
                {
                    UserGroupUsers = userGroupMapUserNames.ConvertAll(x => new
                    {
                        UserName = Platform.UserProfile.Get(x).UserName,
                        RealName = Platform.UserProfile.Get(x).RealName,
                        IsTeamAdminOfUserGroup = teamAdminsOfUserGroup.Contains(x) ? true : false,
                        IsCreator = projectSeries.CreateUserName.Equals(x, StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        IsPersonInCharge = projectSeries.PersonInCharge.Equals(x, StringComparison.CurrentCultureIgnoreCase) ? true : false
                    }).ToList(),
                    StatisticInfo = new
                    {
                        TotalUserGroupUser = userGroupMapUserNames.Count(),
                        TotalteamAdminOfUserGroup = teamAdminsOfUserGroup.Count()
                    }
                };

                return ActionUtils.Success(result);
            });
        }

        //批量移除项目成员
        //创建者、负责人暂不支持移除
        [HttpPost]
        public ActionResult RemoveTeamAdmins(string projectSeriesGuid, string userNames)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                CommUtils.Assert(IsAdmin(projectSeriesLogicModel.Instance, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的创建者/负责人", Platform.UserProfile.GetDisplayRealNameAndUserName(CurrentUserName),
                    projectSeriesLogicModel.Instance.Name);

                var project = projectSeriesLogicModel.CurrentProject.Instance;
                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);
                var userNameList = CommUtils.Split(userNames).ToList();
                foreach (var userName in userNameList)
                {
                    CommUtils.Assert(teamAdmins.Any(x => x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)),
                        "查找项目管理员[{0}]失败", userName);

                    CommUtils.Assert(!IsAdmin(projectSeriesLogicModel.Instance, userName),
                        "检测到[{0}]是产品创建者/负责人，移除项目管理员失败", userName);
                }

                var projectActivityLogicModel = projectSeriesLogicModel.CurrentProject.Activity;

                teamAdmins = teamAdmins.Where(x => userNameList.Contains(x.UserName)).ToList();
                foreach (var teamAdmin in teamAdmins)
                {
                    m_dbAdapter.TeamAdmin.Delete(teamAdmin);
                    projectActivityLogicModel.Add(project.ProjectId, ActivityObjectType.TeamAdmin, teamAdmin.Guid, "删除项目管理员：" + Platform.UserProfile.Get(teamAdmin.UserName).RealName);
                }

                var permissionLogicModel = new PermissionLogicModel(CurrentUserName, projectSeriesLogicModel.Instance);
                permissionLogicModel.RemoveUserPermissionByProjectSeries(userNameList);

                return ActionUtils.Success(1);
            });
        }

        ////判断当前用户是否拥有修改权限的权限
        private bool IsAdmin(ProjectSeries projectSeries)
        {
            return IsAdmin(projectSeries, CurrentUserName);
        }

        //判断某个用户是否拥有修改权限的权限
        private bool IsAdmin(ProjectSeries projectSeries, string userName)
        {
            return projectSeries.CreateUserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                || projectSeries.PersonInCharge.Equals(userName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}