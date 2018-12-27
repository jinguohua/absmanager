using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ProjectActivityController : BaseController
    {
        private class RecordInfo
        {
            public RecordInfo()
            {
                UidChain = new Dictionary<string,string>();
            }

            public Dictionary<string, string> UidChain { get; set; }
            
            public bool IsValidRecord { get; set; }
        }

        [HttpPost]
        public ActionResult GetProjectActivities(string projectGuid, int topRecordCount)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var logicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var records = logicModel.Activity.Get(topRecordCount);

                var userNames = records.Select(x => x.CreateUserName);
                Platform.UserProfile.Precache(userNames);

                var result = records.ConvertAll(x => new {
                    Guid = x.Guid,
                    CreatTime = Toolkit.DateTimeToString(x.CreateTime),
                    UidChainInfo = GetUidChainInfo(x.ActivityObjectType, x.ActivityObjectUniqueIdentifier),
                    Type = x.ActivityObjectType.ToString(),
                    UserInfo = new {
                        RealName = Platform.UserProfile.Get(x.CreateUserName).RealName,
                        UserName = Platform.UserProfile.Get(x.CreateUserName).UserName
                    },
                    LastModifyTime = Toolkit.DateTimeToString(x.LastModifyTime),
                    LastModifyUserName = x.LastModifyUserName,
                    Comment = x.Comment
                });

                return ActionUtils.Success(result);
            });
        }

        private RecordInfo GetUidChainInfo(ActivityObjectType activityObjectType, string objectUniqueIdentifier)
        {
            switch (activityObjectType)
            {
                case ActivityObjectType.Task:
                    return GetTaskUidChain(objectUniqueIdentifier);
                case ActivityObjectType.Agenda:
                    return GetAgendaUidChain(objectUniqueIdentifier);
                case ActivityObjectType.TaskGroup:
                    return GetTaskGroupUidChain(objectUniqueIdentifier);
                case ActivityObjectType.Contact:
                    return GetContactUidChain(objectUniqueIdentifier);
                case ActivityObjectType.TeamMember:
                    return GetTeamMemberUidChain(objectUniqueIdentifier);
                case ActivityObjectType.TeamAdmin:
                    return GetTeamMemberUidChain(objectUniqueIdentifier);
                default:
                    CommUtils.Assert(false, "NotImplemented GetUidChain(activityObjectType={0}, objectUniqueIdentifier={1})",
                        activityObjectType.ToString(), objectUniqueIdentifier);
                    break;
            }

            return null;
        }

        private RecordInfo GetTaskUidChain(string shortCode)
        {
            var info = new RecordInfo();
            info.IsValidRecord = m_dbAdapter.Task.Exists(shortCode);
            info.UidChain["shortCode"] = shortCode;

            //工作被删除后，仍查找工作的shortCode，并返回工作组guid，用于项目动态到工作组跳转
            var task = m_dbAdapter.Task.GetTask(shortCode, info.IsValidRecord);
            if (task.TaskGroupId.HasValue)
            {
                var taskGroups = m_dbAdapter.TaskGroup.GetByIds(new[] { task.TaskGroupId.Value });
                if (taskGroups.Count == 1)
                {
                    info.UidChain["taskGroupGuid"] = taskGroups.Single().Guid;
                }
            }

            var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);
            info.UidChain["projectGuid"] = project.ProjectGuid;

            return info;
        }

        private RecordInfo GetAgendaUidChain(string agendaGuid)
        {
            var info = new RecordInfo();
            info.IsValidRecord = m_dbAdapter.Agenda.Exists(agendaGuid);
            if (info.IsValidRecord)
            {
                var agenda = m_dbAdapter.Agenda.GetAgendaByGuid(agendaGuid);
                info.UidChain["agendaGuid"] = agendaGuid;
                var project = m_dbAdapter.Project.GetProjectById(agenda.ProjectId);
                info.UidChain["projectGuid"] = project.ProjectGuid;
            }

            return info;
        }

        private RecordInfo GetTaskGroupUidChain(string taskGroupGuid)
        {
            var info = new RecordInfo();
            info.IsValidRecord = m_dbAdapter.TaskGroup.Exists(taskGroupGuid);
            if (info.IsValidRecord)
            {
                var taskGroup = m_dbAdapter.TaskGroup.GetByGuid(taskGroupGuid);
                info.UidChain["taskGroupGuid"] = taskGroup.Guid;

                var project = m_dbAdapter.Project.GetProjectById(taskGroup.ProjectId);
                info.UidChain["projectGuid"] = project.ProjectGuid;
            }

            return info;
        }

        private RecordInfo GetContactUidChain(string contactGuid)
        {
            var info = new RecordInfo();
            info.IsValidRecord = m_dbAdapter.Contact.Exists(contactGuid);
            if (info.IsValidRecord)
            {
                var contact = m_dbAdapter.Contact.GetByGuid(contactGuid);
                info.UidChain["contactGuid"] = contactGuid;

                var project = m_dbAdapter.Project.GetProjectById(contact.ProjectId);
                info.UidChain["projectGuid"] = project.ProjectGuid;
            }

            return info;
        }

        private RecordInfo GetTeamMemberUidChain(string teamMemberGuid)
        {
            var info = new RecordInfo();
            info.IsValidRecord = m_dbAdapter.TeamMember.Exists(teamMemberGuid);
            if (info.IsValidRecord)
            {
                var teamMember = m_dbAdapter.TeamMember.GetByGuid(teamMemberGuid);
                info.UidChain["teamMemberGuid"] = teamMemberGuid;

                var project = m_dbAdapter.Project.GetProjectById(teamMember.ProjectId);
                info.UidChain["projectGuid"] = project.ProjectGuid;
            }

            return info;
        }
    }
}
