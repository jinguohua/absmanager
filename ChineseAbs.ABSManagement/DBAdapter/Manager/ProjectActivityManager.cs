using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class ProjectActivityManager : BaseManager
    {
        public ProjectActivityManager()
        {
            m_defaultTableName = "dbo.ProjectActivity";
            m_defaultPrimaryKey = "project_activity_id";
            m_defaultOrderBy = " ORDER BY create_time DESC";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }


        public ProjectActivity NewProjectActivity(int projectId, ActivityObjectType activityObjectType, string activityObjectUniqueIdentifier, string comment)
        {
            var projectActivity = new ProjectActivity()
            {
                Guid = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                ActivityObjectType = activityObjectType,
                ActivityObjectUniqueIdentifier = activityObjectUniqueIdentifier,
                Comment = comment,
                CreateTime = DateTime.Now,
                CreateUserName = UserInfo.UserName,
                LastModifyTime = DateTime.Now,
                LastModifyUserName = UserInfo.UserName,
                RecordStatus = RecordStatus.Valid

            };

            return NewProjectActivity(projectActivity);
        }


        public ProjectActivity NewProjectActivity(ProjectActivity projectActivity)
        {
            var records = m_db.Fetch<ABSMgrConn.TableProjectActivity>(
                "SELECT * FROM " + m_defaultTableName
                + " where project_id = @0 and project_activity_object_unique_identifier = @1 and project_activity_object_type_id = @2 and create_time = @3",
                projectActivity.ProjectId, projectActivity.ActivityObjectUniqueIdentifier, (int)projectActivity.ActivityObjectType, projectActivity.CreateTime);
            
            if (records.Count > 0)
            {
                return new ProjectActivity(records.First());
            }

            var newId = Insert(projectActivity.GetTableObject());
            projectActivity.Id = newId;
            return projectActivity;
        }


        public List<ProjectActivity> GetProjectActivities(int projectId, int topActivityCount)
        {
            var records = m_db.Fetch<ABSMgrConn.TableProjectActivity>(
                 "SELECT * FROM " + m_defaultTableName
                 + " where project_id = @0" + " AND record_status_id <> @1" + m_defaultOrderBy, 
                 projectId, (int)RecordStatus.Deleted);

            return records.Take(topActivityCount).ToList().ConvertAll(x => new ProjectActivity(x));
        }
    }
}
