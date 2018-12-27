using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public enum IssueEmergencyLevel
    {
        Undefined = 1,
        Trivial = 2,
        Minor = 3,
        Major = 4,
        Critical = 5
    }

    public enum IssueStatus
    {
        Undefined = 1,
        Preparing = 2,
        Running = 3,
        Finished = 4,
        Skipped = 5,
        Ignore = 6
    }

    public enum IssueOperationType
    {
        CloseIssue = 1,
        AdditionalIssue = 2,
        CreateIssue = 3,
        DeleteIssueActivity = 4
    }

    public class Issue : BaseDataContainer<TableIssue>
    {
        public Issue()
        {

        }

        public Issue(TableIssue issue)
            : base(issue)
        {

        }

        public int Id { get; set; }

        public string IssueGuid { get; set; }

        public int ProjectId { get; set; }

        public string IssueName { get; set; }

        public string Description { get; set; }

        public IssueEmergencyLevel IssueEmergencyLevel { get; set; }

        public string RelatedTaskShortCode { get; set; }

        public IssueStatus IssueStatus { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public string AllotUser { get; set; }

        public DateTime? LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public DateTime? IssueAllotTime { get; set; }

        public override TableIssue GetTableObject()
        {
            var obj = new TableIssue();
            obj.issue_id = Id;
            obj.issue_guid = IssueGuid;
            obj.project_id = ProjectId;
            obj.issue_name = IssueName;
            obj.description = Description;
            obj.issue_emergency_level_id = (int)IssueEmergencyLevel;
            obj.related_task_short_code = RelatedTaskShortCode;
            obj.issue_status_id = (int)IssueStatus;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.record_status_id = (int)RecordStatus;
            obj.issue_allot_user = AllotUser;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.issue_allot_time = IssueAllotTime;

            return obj;
        }

        public override void FromTableObject(TableIssue obj)
        {
            Id = obj.issue_id;
            IssueGuid = obj.issue_guid;
            ProjectId = obj.project_id;
            IssueName = obj.issue_name;
            Description = obj.description;
            IssueEmergencyLevel = (IssueEmergencyLevel)obj.issue_emergency_level_id;
            RelatedTaskShortCode = obj.related_task_short_code;
            IssueStatus = (IssueStatus)obj.issue_status_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
            AllotUser = obj.issue_allot_user;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            IssueAllotTime = obj.issue_allot_time;
        }
    }
}
