using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ChineseAbs.ABSManagement.Utils;
using System;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.ABSManagement.LogicModels;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class TeamMemberController : BaseController
    {
        //获取Team内所有成员，包括创建者、负责人、管理员、项目成员
        //创建者和负责人可以获取到每个人员的权限信息
        [HttpPost]
        public ActionResult GetTeamMembers(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var projectSeries = projectSeriesLogicModel.Instance;
                var project = projectSeriesLogicModel.CurrentProject.Instance;

                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var logicModel = projectSeriesLogicModel.CurrentProject;
                var adminUserNames = logicModel.Team.Chiefs.Select(x => x.UserName).ToList();

                var projectId = project.ProjectId;
                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(projectId);
                var teamAdminUserNames = teamAdmins.Select(x => x.UserName).ToList();

                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(projectId);
                var teamMemberUserNames = teamMembers.Select(x => x.UserName).ToList();

                var allUserNames = new List<string>();
                allUserNames.AddRange(adminUserNames);
                allUserNames.AddRange(teamAdminUserNames);
                allUserNames.AddRange(teamMemberUserNames);
                allUserNames = allUserNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                Platform.UserProfile.Precache(allUserNames);

                //管理员的权限从数据库中取得
                var dictPermissions = m_dbAdapter.Permission.GetAllPermission(adminUserNames, projectSeriesGuid);
                var dictTeamAdminPermissions = m_dbAdapter.Permission.GetAllPermission(teamAdminUserNames, projectSeriesGuid);

                var result = new TeamMemberListViewModel();

                var personInCharge = Platform.UserProfile.Get(projectSeries.PersonInCharge);
                if (personInCharge != null)
                {
                    result.PersonInCharge = new TeamMemberViewModel(personInCharge);
                    if (IsAdmin(projectSeries))
                    {
                        result.PersonInCharge.Permission.Set(dictPermissions[result.PersonInCharge.UserName.ToLower()]);
                    }
                }

                var creator = Platform.UserProfile.Get(projectSeries.CreateUserName);
                if (creator != null)
                {
                    result.Creator = new TeamMemberViewModel(creator);
                    if (IsAdmin(projectSeries))
                    {
                        result.Creator.Permission.Set(dictPermissions[result.Creator.UserName.ToLower()]);
                    }
                }

                result.TeamMembers = teamMembers.ConvertAll(x => {
                    var teamMember = new TeamMemberViewModel(Platform.UserProfile.Get(x.UserName));
                    teamMember.Permission.Set(x.Read, x.Write, x.Execute);
                    return teamMember;
                }).OrderBy(x => x.RealName, CommUtils.StringComparerCN).ThenBy(x => x.UserName).ToList();

                result.TeamAdmins = teamAdmins.ConvertAll(x =>
                {
                    var teamMember = new TeamMemberViewModel(Platform.UserProfile.Get(x.UserName));
                    teamMember.Permission.Set(dictTeamAdminPermissions[teamMember.UserName.ToLower()]);
                    return teamMember;
                }).OrderBy(x => x.RealName, CommUtils.StringComparerCN).ThenBy(x => x.UserName).ToList();

                return ActionUtils.Success(result);
            });
        }

        //批量增加项目成员
        [HttpPost]
        public ActionResult AddTeamMembers(string projectSeriesGuid, string userNames, string permissions)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(permissions), "权限类型不能为空");
                var permissionList = CommUtils.ParseEnumList<PermissionType>(permissions, true);
                CommUtils.Assert(permissionList.Any(x => x == PermissionType.Read), "增加项目成员时，只读权限必须勾选");
                //判断操作者权限
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeriesLogicModel.CurrentProject.Instance;
                CommUtils.Assert(IsAdmin(projectSeriesLogicModel.Instance, CurrentUserName)
                    ||m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的创建者/负责人/管理员", Platform.UserProfile.GetDisplayRealNameAndUserName(CurrentUserName),
                    projectSeriesLogicModel.Instance.Name);

                //判断传入userName是否有效
                var userNameList = CommUtils.Split(userNames);
                foreach (var userName in userNameList)
                {
                    CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(userName), "用户[{0}]不存在", userName);
                    CommUtils.Assert((!IsAdmin(projectSeriesLogicModel.Instance, userName)) &&
                        (!m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, userName)),
                        "用户[{0}]是产品创建者/负责人/管理员，无法增加至项目成员", userName);
                }

                //判断项目成员是否已经添加
                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                foreach (var teamMember in teamMembers)
                {
                    CommUtils.Assert(!userNameList.Contains(teamMember.UserName),
                        "项目成员中已经存在用户[{0}]", teamMember.UserName);
                }

                var projectActivityLogicModel = projectSeriesLogicModel.CurrentProject.Activity;

                //添加项目成员
                foreach (var userName in userNameList)
                {
                    var teamMember = new TeamMember
                    {
                        UserName = userName,
                        ProjectId = project.ProjectId,
                        Read = permissionList.Any(x => x == PermissionType.Read),
                        Write = permissionList.Any(x => x == PermissionType.Write),
                        Execute = permissionList.Any(x => x == PermissionType.Execute)
                    };

                    m_dbAdapter.TeamMember.Add(teamMember);
                    projectActivityLogicModel.Add(project.ProjectId, ActivityObjectType.TeamMember, teamMember.Guid, "增加项目成员：" + Platform.UserProfile.Get(teamMember.UserName).RealName);

                }

                teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                var newTeamMembers = teamMembers.Where(x => userNameList.Contains(x.UserName)).ToList();
                var adminUserNames = new List<string>();
                var teamAdmins = new List<TeamAdmin>();

                var permissionLogicModel = new PermissionLogicModel(CurrentUserName, projectSeriesLogicModel.Instance);
                permissionLogicModel.AddUserPermissionByProjectSeries(newTeamMembers, teamAdmins, adminUserNames);

                return ActionUtils.Success(1);
            });
        }

        //获取组项目人员及统计信息
        [HttpPost]
        public ActionResult GetTeamMembersOfUserGroup(string projectSeriesGuid, string userGroupGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var projectSeries = projectSeriesLogicModel.Instance;
                var projectId = projectSeriesLogicModel.CurrentProject.Instance.ProjectId;
                CommUtils.Assert(IsAdmin(projectSeriesLogicModel.Instance, CurrentUserName)
                    || m_dbAdapter.TeamAdmin.IsTeamAdmin(projectId, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的创建者/负责人/管理员", Platform.UserProfile.GetDisplayRealNameAndUserName(CurrentUserName),
                    projectSeriesLogicModel.Instance.Name);


                var userGroup = m_dbAdapter.UserGroup.GetByGuid(userGroupGuid);
                CommUtils.Assert(IsCurrentUser(userGroup.Owner), "当前用户[{0}]不是[{1}]的创建者", CurrentUserName, userGroup.Name);

                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(projectId);
                var teamAdminUserNames = teamAdmins.Select(x => x.UserName).ToList();

                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(projectId);
                var teamMemberUserNames = teamMembers.Select(x => x.UserName).ToList();

                var userGroupMaps = m_dbAdapter.UserGroupMap.GetByUserGroupGuid(userGroupGuid);
                var userGroupMapUserNames = userGroupMaps.Select(x => x.UserName).ToList();

                var teamMembersOfUserGroup = teamMemberUserNames.Intersect(userGroupMapUserNames).ToList();
                Platform.UserProfile.Precache(userGroupMapUserNames);
                var result = new 
                {
                    UserGroupUsers = userGroupMapUserNames.ConvertAll(x => new
                    {
                        UserName = Platform.UserProfile.Get(x).UserName,
                        RealName = Platform.UserProfile.Get(x).RealName,
                        IsTeamMemberOfUserGroup = teamMembersOfUserGroup.Contains(x) ? true : false,
                        IsCreator = projectSeries.CreateUserName.Equals(x, StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        IsPersonInCharge = projectSeries.PersonInCharge.Equals(x, StringComparison.CurrentCultureIgnoreCase) ? true : false,
                        IsTeamAdmin = teamAdminUserNames.Contains(x) ? true : false,
                    }).ToList(),
                    StatisticInfo = new
                    {
                        TotalUserGroupUser = userGroupMapUserNames.Count(),
                        TotalteamMemberOfUserGroup = teamMembersOfUserGroup.Count()
                    }
                };

                return ActionUtils.Success(result);
            });
        }

        //批量移除项目成员
        //创建者、负责人暂不支持移除
        [HttpPost]
        public ActionResult RemoveTeamMembers(string projectSeriesGuid, string userNames)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeriesLogicModel.CurrentProject.Instance;
                CommUtils.Assert(IsAdmin(projectSeriesLogicModel.Instance, CurrentUserName)
                    || m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的创建者/负责人/管理员", Platform.UserProfile.GetDisplayRealNameAndUserName(CurrentUserName),
                    projectSeriesLogicModel.Instance.Name);

                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                var userNameList = CommUtils.Split(userNames).ToList();
                foreach (var userName in userNameList)
                {
                    CommUtils.Assert(teamMembers.Any(x => x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)),
                        "查找项目成员[{0}]失败", userName);

                    CommUtils.Assert((!IsAdmin(projectSeriesLogicModel.Instance, userName)) &&
                        (!m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, userName)),
                        "检测到[{0}]是产品创建者/负责人，移除项目成员失败", userName);
                }

                var projectActivityLogicModel = projectSeriesLogicModel.CurrentProject.Activity;

                teamMembers = teamMembers.Where(x => userNameList.Contains(x.UserName)).ToList();
                foreach (var teamMember in teamMembers)
                {
                    m_dbAdapter.TeamMember.Remove(teamMember);
                    projectActivityLogicModel.Add(project.ProjectId, ActivityObjectType.TeamMember, teamMember.Guid, "删除项目成员：" + Platform.UserProfile.Get(teamMember.UserName).RealName);
                }

                var permissionLogicModel = new PermissionLogicModel(CurrentUserName, projectSeriesLogicModel.Instance);
                permissionLogicModel.RemoveUserPermissionByProjectSeries(userNameList);

                return ActionUtils.Success(1);
            });
        }

        //批量修改项目成员权限
        [HttpPost]
        public ActionResult ModifyTeamMembersPermission(string projectSeriesGuid, string userNames, string permissions)
        {
            return ActionUtils.Json(() =>
            {
                var permissionList = CommUtils.ParseEnumList<PermissionType>(permissions, true);
                CommUtils.Assert(permissionList.Any(x => x == PermissionType.Read), "编辑项目成员权限时，只读权限必须勾选");

                RemoveTeamMembers(projectSeriesGuid, userNames);
                AddTeamMembers(projectSeriesGuid, userNames, permissions);
                return ActionUtils.Success(1);
            });
        }

        //重设所有项目成员权限
        [HttpPost]
        public ActionResult ResetAllTeamMemberPermission(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                //获取所有有读取权限的用户
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeriesLogicModel.CurrentProject.Instance;
                CommUtils.Assert(IsAdmin(projectSeriesLogicModel.Instance, CurrentUserName)
                    || m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, CurrentUserName),
                    "当前用户[{0}]不是项目[{1}]的创建者/负责人/管理员", Platform.UserProfile.GetDisplayRealNameAndUserName(CurrentUserName),
                    projectSeriesLogicModel.Instance.Name);

                //只保留项目成员
                var projectSeries = projectSeriesLogicModel.Instance;
                var permissionList = m_dbAdapter.Permission.GetByObjectUid(projectSeriesGuid, PermissionObjectType.ProjectSeries, PermissionType.Read);
                var userNames = permissionList.Select(x => x.UserName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                userNames.RemoveAll(x => IsAdmin(projectSeries, x) || m_dbAdapter.TeamAdmin.IsTeamAdmin(project.ProjectId, x));

                //从TeamMember表中移除没有读取权限的成员
                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                foreach (var teamMember in teamMembers)
                {
                    if (!userNames.Contains(teamMember.UserName))
                    {
                        m_dbAdapter.TeamMember.Remove(teamMember);
                    }
                }

                //增加有读取权限的成员到TeamMember中
                foreach (var userName in userNames)
                {
                    if (!teamMembers.Any(x => x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var teamMember = new TeamMember { 
                            UserName = userName,
                            ProjectId = project.ProjectId,
                            Read = true,
                            Write = false,
                            Execute = false
                        };

                        m_dbAdapter.TeamMember.Add(teamMember);
                    }
                }

                //获取管理员（负责人 + 创建者）
                var adminUserNames = new List<string> { projectSeries.CreateUserName, projectSeries.PersonInCharge };
                adminUserNames = adminUserNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                //获取所有项目成员
                teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);

                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);

                var permissionLogicModel = new PermissionLogicModel(CurrentUserName, projectSeriesLogicModel.Instance);
                permissionLogicModel.AddUserPermissionByProjectSeries(teamMembers, teamAdmins, adminUserNames);

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