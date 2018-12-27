using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement
{
    public class TaskGroupManager : BaseManager
    {
        public TaskGroupManager()
        {
            m_defaultTableName = "dbo.TaskGroup";
            m_defaultPrimaryKey = "task_group_id";
            m_defaultOrderBy = " ORDER BY create_time desc, task_group_id desc";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public TaskGroup NewTaskGroup(int projectId, string name, string description)
        {
            var taskGroup = new ABSMgrConn.TableTaskGroup()
            {
                task_group_guid = Guid.NewGuid().ToString(),
                project_id = projectId,
                name = name,
                description = description,
                record_status_id = (int)RecordStatus.Valid,
                create_time = DateTime.Now,
                create_user_name = UserInfo.UserName
            };
            taskGroup.task_group_id = Insert(taskGroup);
            return new TaskGroup(taskGroup);
        }

        public int UpdateTaskGroup(TaskGroup taskGroup)
        {
            var taskGroupTable = taskGroup.GetTableObject();
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, taskGroupTable, taskGroup.Id);
        }

        public TaskGroup RemoveTaskGroup(TaskGroup taskGroup)
        {
            taskGroup.RecordStatus = RecordStatus.Deleted;
            var count = UpdateTaskGroup(taskGroup);
            CommUtils.AssertEquals(count, 1, "Remove taskGroup failed : id={0};recordsCount={1}", taskGroup.Id, count);
            return taskGroup;
        }

        public List<TaskGroup> GetByProjectId(int projectId)
        {
            var records = Select<ABSMgrConn.TableTaskGroup>("project_id", projectId);
            return records.ToList().ConvertAll(x => new TaskGroup(x));
        }

        public TaskGroup GetByGuid(string guid)
        {
            var record = SelectSingle<ABSMgrConn.TableTaskGroup>("task_group_guid", guid);
            return new TaskGroup(record);
        }

        public bool Exists(string guid)
        {
            return Exists<ABSMgrConn.TableTaskGroup>("task_group_guid", guid);
        }
        
        public List<TaskGroup> GetByGuids(IEnumerable<string> guids)
        {
            var records = Select<ABSMgrConn.TableTaskGroup, string>("task_group_guid", guids);
            return records.ToList().ConvertAll(x => new TaskGroup(x));
        }

        public TaskGroup GetById(int id)
        {
            var record = SelectSingle<ABSMgrConn.TableTaskGroup>(id);
            return new TaskGroup(record);
        }

        public List<TaskGroup> GetByIds(IEnumerable<int> ids)
        {
            var records = Select<ABSMgrConn.TableTaskGroup, int>(ids);
            return records.ToList().ConvertAll(x => new TaskGroup(x));
        }
    }
}
