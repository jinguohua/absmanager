using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using ChineseAbs.ABSManagement.Utils;
using System;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.ABSManagement;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class PermissionController : BaseController
    {
        [HttpPost]
        public ActionResult GetProjectSeriesTreeData(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(projectSeriesGuid),"项目名称发生错误，请刷新页面后重试");
                var projectSeries = m_dbAdapter.ProjectSeries.GetByGuid(projectSeriesGuid);
                CheckPermission(PermissionObjectType.ProjectSeries, projectSeriesGuid, PermissionType.Read);
                var projectSeriesTreeData = GetProjectSeriesTreeData(projectSeries);
                var result = ConvertProjectSeriesTreeData(projectSeriesTreeData);
                return ActionUtils.Success(result);
            });
        }

        private List<PermissionTree> ConvertProjectSeriesTreeData(List<PermissionTree> projectSeriesTreeData)
        {
            var newProjectSeriesTreeData = new List<PermissionTree>();
            foreach (var item in projectSeriesTreeData)
            {
                newProjectSeriesTreeData.Add(new PermissionTree
                {
                    title = item.title,
                    key = item.key,
                    type = item.type,
                    children = item.children[0].children
                });
            }
            return newProjectSeriesTreeData;
        }

        private List<PermissionTree> GetProjectSeriesTreeData(ProjectSeries projectSeries)
        {
            var result = new List<PermissionTree>();
            result.Add(new PermissionTree
            {
                title = projectSeries.Name,
                key = projectSeries.Guid,
                type = PermissionObjectType.ProjectSeries.ToString(),
                children = m_dbAdapter.Project.GetByProjectSeriesId(projectSeries.Id).ConvertAll(x => new PermissionTree
                {
                    title = x.Name,
                    key = x.ProjectGuid,
                    type = PermissionObjectType.Project.ToString(),
                    children = m_dbAdapter.TaskGroup.GetByProjectId(x.ProjectId).ConvertAll(y =>
                    new PermissionTree
                    {
                        title = y.Name,
                        key = y.Guid,
                        type = PermissionObjectType.TaskGroup.ToString(),
                        children = m_dbAdapter.Task.GetByTaskGroupId(y.Id).ConvertAll(task =>
                        new PermissionTree
                        {
                            title = task.Description,
                            key = task.ShortCode,
                            type = PermissionObjectType.Task.ToString(),
                            children = new List<PermissionTree>()
                        })
                    })
                })
            });
            return result;
        }

        [HttpPost]
        public ActionResult GetUserPermissionByUid(string uid,string objectType, string treeNodeName)
        {
            return ActionUtils.Json(() =>
            {
                var permissionObjectType = CommUtils.ParseEnum<PermissionObjectType>(objectType);
                AuthorityCheck(permissionObjectType, uid, treeNodeName);
                var allUserName = m_dbAdapter.Permission.GetAllUserNameByUid(uid);
                Platform.UserProfile.Precache(allUserName);

                var allPermission = new List<Permission>();
                allUserName.ForEach(x => allPermission.AddRange(m_dbAdapter.Permission.GetAllPermission(x,uid)));

                var dictAllPermission = allPermission.GroupBy(x => x.UserName).ToDictionary(x => x.Key, y => y.ToList());
                var result = new List<UserPermissionInfo>();

                foreach (var item in dictAllPermission.Keys)
                {
                    var userRealName = Platform.UserProfile.GetRealName(item);
                    var userPermission = dictAllPermission[item];
                    if (userPermission.Count != 0 && userPermission.Count > 1)
                    {
                        var permissionTypeList = userPermission.ConvertAll(x => x.Type.ToString());
                        var permissionInfo = ConvertUserPermissionInfo(userPermission[0], userRealName);
                        permissionInfo.Permission = CommUtils.Join(permissionTypeList);
                        result.Add(permissionInfo);
                        continue;
                    }

                    var userPermissionInfo = ConvertUserPermissionInfo(userPermission.First(), userRealName);
                    result.Add(userPermissionInfo);
                }

                return ActionUtils.Success(result);
            });
        }

        private UserPermissionInfo ConvertUserPermissionInfo(Permission permission, string userRealName)
        {
            var userPermissionInfo = new UserPermissionInfo();
            userPermissionInfo.UserName = permission.UserName;
            userPermissionInfo.RealName = userRealName;
            userPermissionInfo.UniqueIdentifier = permission.ObjectUniqueIdentifier;
            userPermissionInfo.Permission = permission.Type.ToString() ;
            return userPermissionInfo;
        }

        private void AuthorityCheck(PermissionObjectType objectType, string guid, string treeNodeName, string userName = null)
        {
            var projectSeries = new ProjectSeries();
            if (objectType == PermissionObjectType.Task)
            {
                var task = m_dbAdapter.Task.GetTask(guid);
                var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);
                projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
                CheckIsAdmin(projectSeries, userName);
            }
            if (objectType == PermissionObjectType.TaskGroup)
            {
                var taskGroup = m_dbAdapter.TaskGroup.GetByGuid(guid);
                var project = m_dbAdapter.Project.GetProjectById(taskGroup.ProjectId);
                projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
                CheckIsAdmin(projectSeries, userName);
            }
            if (objectType == PermissionObjectType.Project)
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(guid);
                projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
                CheckIsAdmin(projectSeries, userName);
            }
            if (objectType == PermissionObjectType.ProjectSeries)
            {
                projectSeries = m_dbAdapter.ProjectSeries.GetByGuid(guid);
                CheckIsAdmin(projectSeries,userName);
            }
            CommUtils.Assert(projectSeries.CreateUserName.Equals(CurrentUserName, StringComparison.CurrentCultureIgnoreCase)
                || projectSeries.PersonInCharge.Equals(CurrentUserName, StringComparison.CurrentCultureIgnoreCase),
                "当前用户[" + CurrentUserName + "]没有修改[" + treeNodeName + "]的权限");
        }

        private void CheckIsAdmin(ProjectSeries projectSeries, string userName)
        {
            if (userName != null)
            {
                CommUtils.Assert(projectSeries.CreateUserName != userName && projectSeries.PersonInCharge != userName,
                    "检测到[{0}]是产品创建者/负责人，无法对其权限进行任何操作", userName);
            }
        }

        [HttpPost]
        public ActionResult AddUserPermission(string uid, string objectType, string username, string permissionTypeText, string treeNodeName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(permissionTypeText),"权限类型不能为空");
                CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(username), "用户[{0}]不存在", username);

                var permissionObjectType = CommUtils.ParseEnum<PermissionObjectType>(objectType);

                AuthorityCheck(permissionObjectType, uid, treeNodeName, username);
                
                var permissionList = CommUtils.ParseEnumList<PermissionType>(permissionTypeText, true);
                CommUtils.Assert(permissionList.Any(x => x == PermissionType.Read), "增加用户权限时，只读权限必须勾选");
                var realName = m_dbAdapter.Authority.GetUserRealName(username);

                var toAddPermission = new List<Permission>();

                foreach (var type in permissionList)
                {
                    var permission = new Permission
                    {
                        Type = type,
                        UserName = username,
                        ObjectUniqueIdentifier = uid,
                        ObjectType = permissionObjectType
                    };
                    CommUtils.Assert(!m_dbAdapter.Permission.HasPermission(permission),
                        "用户[{0}({1})]已有[{2}]权限", realName, username, type.ToString());
                    toAddPermission.Add(permission);
                }

                toAddPermission.ForEach(x => m_dbAdapter.Permission.NewPermission(x));
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, null,
                    "用户["+realName+"(" + username + ")],ObjectType[" + permissionObjectType + "]中添加[" + permissionTypeText + "]权限", "");
                
                AddParentPermission(uid,username,permissionObjectType);
                return ActionUtils.Success(1);
            });
        }

        private void AddParentPermission(string uid, string username, PermissionObjectType objectTypePermission)
        {
            if (objectTypePermission == PermissionObjectType.Task)
            {
                var task = m_dbAdapter.Task.GetTask(uid);
                var taskGroup = m_dbAdapter.TaskGroup.GetById(task.TaskGroupId.Value);
                var project = m_dbAdapter.Project.GetProjectById(taskGroup.ProjectId);
                var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);

                m_dbAdapter.Permission.NewPermission(username, taskGroup.Guid, PermissionObjectType.TaskGroup, PermissionType.Read);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, project.ProjectId,
                    "用户[" + username + ")],ObjectType[" + PermissionObjectType.TaskGroup.ToString() +
                    "]中添加[Read]权限", "");

                m_dbAdapter.Permission.NewPermission(username, project.ProjectGuid, PermissionObjectType.Project, PermissionType.Read);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, project.ProjectId,
                    "用户[" + username + ")],ObjectType[" + PermissionObjectType.Project.ToString() +
                    "]中添加[Read]权限", "");

                m_dbAdapter.Permission.NewPermission(username, projectSeries.Guid, PermissionObjectType.ProjectSeries, PermissionType.Read);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, project.ProjectId,
                    "用户[" + username + ")],ObjectType[" + PermissionObjectType.ProjectSeries.ToString() +
                    "]中添加[Read]权限", "");
            }

            if (objectTypePermission == PermissionObjectType.TaskGroup)
            {
                var taskGroup = m_dbAdapter.TaskGroup.GetByGuid(uid);
                var project = m_dbAdapter.Project.GetProjectById(taskGroup.ProjectId);
                var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);

                m_dbAdapter.Permission.NewPermission(username, project.ProjectGuid, PermissionObjectType.Project, PermissionType.Read);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, project.ProjectId,
                    "用户[" + username + ")],ObjectType[" + PermissionObjectType.Project.ToString() +
                    "]中添加[Read]权限", "");

                m_dbAdapter.Permission.NewPermission(username, projectSeries.Guid, PermissionObjectType.ProjectSeries, PermissionType.Read);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, project.ProjectId,
                    "用户[" + username + ")],ObjectType[" + objectTypePermission.ToString() +
                    "]中添加[Read]权限", "");
            }
        }

        [HttpPost]
        public ActionResult DeleteUserPermission(string username, string uid, string treeNodeName, string objectType)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(username), "用户[{0}]不存在，请刷新页面后重试", username);

                var permissionObjectType = CommUtils.ParseEnum<PermissionObjectType>(objectType);
                AuthorityCheck(permissionObjectType, uid, treeNodeName, username);

                DeleteUserAllPermissionByUid(username, uid);

                return ActionUtils.Success(1);
            });
        }

        private void DeleteUserAllPermissionByUid(string username, string uid)
        {
            var allPermission = m_dbAdapter.Permission.GetAllPermission(username, uid);
            var permissionType = allPermission.ConvertAll(x => x.ObjectType.ToString());
            allPermission.ForEach(x => m_dbAdapter.Permission.DeletePermission(x));
            m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, null,
                "删除用户[" + username + ")]的[" + CommUtils.Join(permissionType) + "]权限", "");
        }

        [HttpPost]
        public ActionResult ModifyPermission(string uid, string objectType, string username, string permissionTypeText, string treeNodeName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(username), "用户[{0}]不存在，请刷新页面后重试", username);

                var permissionObjectType = CommUtils.ParseEnum<PermissionObjectType>(objectType);

                AuthorityCheck(permissionObjectType, uid, treeNodeName, username);

                var permissionTypeStr = CommUtils.Split(permissionTypeText);
                if (permissionTypeStr.Length == 1 && permissionTypeStr[0] == "Delete")
                {
                    DeleteUserAllPermissionByUid(username, uid);
                    return ActionUtils.Success(1);
                }

                var permissionType = permissionTypeStr.ToList().ConvertAll(x => CommUtils.ParseEnum<PermissionType>(x));
                CommUtils.Assert(permissionType.Any(x => x.ToString() == "Read"), "只读权限必须勾选");
                var realName = m_dbAdapter.Authority.GetUserRealName(username);
                var allOldUserPermission = m_dbAdapter.Permission.GetAllPermission(username, uid);
                var oldPermissionTypeList = allOldUserPermission.ConvertAll(x => x.Type);

                foreach (var type in permissionType)
                {
                    if (oldPermissionTypeList.IndexOf(type) < 0)
                    {
                        m_dbAdapter.Permission.NewPermission(username, uid, permissionObjectType, type);
                        m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, null,
                            "用户[" + username + ")],ObjectType[" + permissionObjectType.ToString() +
                            "]中添加[" + type + "]权限", "");
                    }
                }

                foreach (var item in allOldUserPermission)
                {
                    if (permissionType.IndexOf(item.Type) < 0)
                    {
                        var permission = new Permission();
                        permission.UserName = username;
                        permission.ObjectType = permissionObjectType;
                        permission.ObjectUniqueIdentifier = uid;
                        permission.Type = item.Type;

                        m_dbAdapter.Permission.DeletePermission(item);
                        m_dbAdapter.Project.NewEditProductLog(EditProductType.EditPermission, null,
                            "用户[" + username + ")],ObjectType[" + permissionObjectType.ToString() +
                            "]中删除[" + item.Type.ToString() + "]权限", "");
                    }
                }

                return ActionUtils.Success(1);
            });
        }
    }
}