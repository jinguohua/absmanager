using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class IssueConnectionTasks : BaseModel<TableIssueConnectionTasks>
    {
        public IssueConnectionTasks()
        {

        }

        public IssueConnectionTasks(TableIssueConnectionTasks obj)
            : base(obj)
        {

        }
        public string TaskShortCode { get; set; }
        public int IssueId { get; set; }

        public override TableIssueConnectionTasks GetTableObject()
        {
            var obj = new TableIssueConnectionTasks();
            obj.issue_connection_tasks_id = Id;
            obj.issue_connection_tasks_guid = Guid;
            obj.task_short_code = TaskShortCode;
            obj.issue_id = IssueId;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableIssueConnectionTasks obj)
        {
            Id = obj.issue_connection_tasks_id;
            Guid = obj.issue_connection_tasks_guid;
            TaskShortCode = obj.task_short_code;
            IssueId = obj.issue_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
