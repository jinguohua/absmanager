using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public enum IssueActivityType
    {
        Undefined = 1,
        Original = 2,
        Additional = 3,
        SystemGenerate = 4
    }

    public class IssueActivity : BaseDataContainer<TableIssueActivity>
    {
        public IssueActivity()
        {

        }

        public IssueActivity(TableIssueActivity issueActivity)
            : base(issueActivity)
        {

        }

        public int IssueActivityId { get; set; }

        public string IssueActivityGuid { get; set; }

        public int IssueId { get; set; }

        public IssueActivityType IssueActivityTypeId { get; set; }

        public string Comment { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableIssueActivity GetTableObject()
        {
            var obj = new TableIssueActivity();
            obj.issue_activity_id = IssueActivityId;
            obj.issue_activity_guid = IssueActivityGuid;
            obj.issue_id = IssueId;
            obj.issue_activity_type_id = (int)IssueActivityTypeId;
            obj.comment = Comment;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableIssueActivity obj)
        {
            IssueActivityId = obj.issue_activity_id;
            IssueActivityGuid = obj.issue_activity_guid;
            IssueId = obj.issue_id;
            IssueActivityTypeId = (IssueActivityType)obj.issue_activity_type_id;
            Comment = obj.comment;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
