using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class PermissionLogicModel : BaseLogicModel
    {
        public PermissionLogicModel(ProjectLogicModel project)
            : base(project)
        {

        }

        public PermissionLogicModel(string userName, ProjectSeries projectSeries)
            : base(userName, null)
        {
            m_currentUserName = userName;
            m_projectSeries = projectSeries;
        }

        public void SetPermission(string userName, string uid, PermissionObjectType objectType, List<Permission> curPermissions, bool read, bool write, bool execute)
        {
            Action<bool, PermissionType> resetPermission = (permissionType, dbPermissionType) =>
            {
                var permission = curPermissions.SingleOrDefault(x => x.Type == dbPermissionType
                    && x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)
                    && x.ObjectUniqueIdentifier == uid);

                if (permissionType && permission == null)
                {
                    m_dbAdapter.Permission.NewPermission(userName, uid, objectType, dbPermissionType);
                }

                if (!permissionType && permission != null)
                {
                    m_dbAdapter.Permission.DeletePermission(permission);
                }
            };

            resetPermission(read, PermissionType.Read);
            resetPermission(write, PermissionType.Write);
            resetPermission(execute, PermissionType.Execute);
        }

        public void SetPermission(ProjectSeries projectSeries, Project project, string uid, PermissionObjectType objectType)
        {
            var adminUsernames = new List<string> { projectSeries.CreateUserName, projectSeries.PersonInCharge };
            adminUsernames = adminUsernames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var projectId = project.ProjectId;
            var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(projectId);
            var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(projectId);

            //给[负责人和创建者]添加所有权限。
            foreach (var adminUsername in adminUsernames)
            {
                SetPermission(adminUsername, uid, objectType,new List<Permission>(), true, true, true);
            }

            //给[项目管理员]添加所有权限。
            foreach (var teamAdmin in teamAdmins)
            {
                SetPermission(teamAdmin.UserName, uid, objectType, new List<Permission>(), true, true, true);
            }

            //给[项目成员]添加其拥有的权限权限。
            foreach (var teamMember in teamMembers)
            {
                SetPermission(teamMember.UserName, uid, objectType, new List<Permission>(), teamMember.Read, teamMember.Write, teamMember.Execute);
            }
        }

        public void RemoveUserPermissionByProjectSeries(List<string> userNameList)
        {
            var projectSeries = m_projectSeries;
            var project = CurrentProject.Instance;

            //移除projectSeries权限
            RemovePermissionByObjType(PermissionObjectType.ProjectSeries, new List<string>{ projectSeries.Guid }, userNameList);

            //移除project权限
            RemovePermissionByObjType(PermissionObjectType.Project, new List<string> { project.ProjectGuid }, userNameList);

            //移除taskGroup权限
            var taskGroupGuids = CurrentProject.TaskGroups.Select(x => x.Instance.Guid).ToList();
            RemovePermissionByObjType(PermissionObjectType.TaskGroup, taskGroupGuids, userNameList);

            //移除task权限
            var taskShortCodes = new List<string>();
            CurrentProject.TaskGroups.ForEach(x => taskShortCodes.AddRange(x.Tasks.Select(task => task.ShortCode)));
            RemovePermissionByObjType(PermissionObjectType.Task, taskShortCodes, userNameList);
        }

        //移除用户对uid的全部权限
        private void RemovePermissionByObjType(PermissionObjectType permissionObjectType, List<string> uids, List<string> userNames)
        {
            var permissions = m_dbAdapter.Permission.GetAllPermission(userNames, uids);
            foreach (var permission in permissions)
            {
                CommUtils.AssertEquals(permission.ObjectType, permissionObjectType,
                    "检测到权限类型错误：uid={0};objType={1}", permission.ObjectUniqueIdentifier, permission.ObjectType);
            }

            m_dbAdapter.Permission.DeletePermission(permissions);
        }

        public void AddUserPermissionByProjectSeries(List<TeamMember> teamMembers, List<TeamAdmin> teamAdmins, List<string> adminUserNames, List<TeamAdmin> asd = null)
        {
            var projectSeries = m_projectSeries;
            var project = CurrentProject.Instance;

            //为项目成员，管理员, 创建者和负责人配置ProjectSeries、Project、TaskGroup、Task的权限
            //重设projectSeries权限
            SetPermissionByObjType(PermissionObjectType.ProjectSeries,
                projectSeries.Guid, teamMembers, teamAdmins, adminUserNames);

            //重设project权限
            SetPermissionByObjType(PermissionObjectType.Project,
                project.ProjectGuid, teamMembers, teamAdmins, adminUserNames);

            //重设taskGroup权限
            foreach (var taskGroup in CurrentProject.TaskGroups)
            {
                SetPermissionByObjType(PermissionObjectType.TaskGroup,
                    taskGroup.Instance.Guid, teamMembers, teamAdmins, adminUserNames);

                //重设task权限
                foreach (var task in taskGroup.Tasks)
                {
                    SetPermissionByObjType(PermissionObjectType.Task,
                        task.ShortCode, teamMembers, teamAdmins, adminUserNames);
                }
            }
        }

        private void SetPermissionByObjType(PermissionObjectType permissionObjectType, string uid,
            List<TeamMember> teamMembers, List<TeamAdmin> teamAdmins, List<string> adminUserNames)
        {
            Func<Dictionary<string, List<Permission>>, string, List<Permission>> getPermission =
                (dict, userName) => dict.Keys.Contains(userName,StringComparer.OrdinalIgnoreCase) ? dict[userName.ToLower()] : new List<Permission>();

            var userNames = teamMembers.Select(x => x.UserName).ToList();
            var userPermissions = m_dbAdapter.Permission.GetAllPermission(userNames, uid);
            foreach (var teamMember in teamMembers)
            {
                //为项目成员操作ProjectSeries设置只读权限
                //操作其他内容（如TaskGroup、Task）设置TeamMember拥有的权限
                if (permissionObjectType == PermissionObjectType.ProjectSeries)
                {
                    SetPermission(teamMember.UserName, uid, permissionObjectType,
                        getPermission(userPermissions, teamMember.UserName), true, false, false);
                }
                else
                {
                    SetPermission(teamMember, uid, permissionObjectType,
                        getPermission(userPermissions, teamMember.UserName));
                }
            }

            var teamAdminUserNames = teamAdmins.Select(x => x.UserName).ToList();
            var teamAdminUserPermissions = m_dbAdapter.Permission.GetAllPermission(teamAdminUserNames, uid);
            foreach (var teamAdmin in teamAdmins)
            {
                //设置项目管理员权限
                SetPermission(teamAdmin.UserName, uid, permissionObjectType,
                    getPermission(teamAdminUserPermissions, teamAdmin.UserName), true, true, true);
            }

            var adminPermissions = m_dbAdapter.Permission.GetAllPermission(adminUserNames, uid);
            foreach (var adminUserName in adminUserNames)
            {
                SetPermission(adminUserName, uid, permissionObjectType,
                    getPermission(adminPermissions, adminUserName), true, true, true);
            }
        }

        //根据teamMember中的权限设置，重置该用户的所有权限
        private void SetPermission(TeamMember teamMember, string uid, PermissionObjectType objectType, List<Permission> curPermissions)
        {
            SetPermission(teamMember.UserName, uid, objectType, curPermissions, teamMember.Read, teamMember.Write, teamMember.Execute);
        }

        public ProjectLogicModel CurrentProject
        {
            get
            {
                if (m_currentProject == null)
                {
                    var projects = m_dbAdapter.Project.GetByProjectSeriesId(m_projectSeries.Id);
                    projects = projects.Where(x => x.TypeId.HasValue && x.TypeId == (int)m_projectSeries.Stage).ToList();
                    CommUtils.Assert(projects.Count < 2, "[{0}]中，处于[{1}]的项目数不唯一", m_projectSeries.Name, m_projectSeries.Stage);
                    //CommUtils.Assert(projects.Count > 0, "[{0}]中不包含处于[{1}]阶段的项目", m_projectSeries.Name, m_projectSeries.Stage);
                    if (projects.Count == 1)
                    {
                        var project = projects.Single();
                        m_currentProject = new ProjectLogicModel(m_currentUserName, project);
                    }
                }

                return m_currentProject;
            }
        }

        private ProjectLogicModel m_currentProject;

        private ProjectSeries m_projectSeries;

        private string m_currentUserName;
    }
}
