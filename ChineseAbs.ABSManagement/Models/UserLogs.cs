using System;
using System.Collections.Generic;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class UserLogs : List<UserLog>
    {
        public List<TableUserLogs> ToTableList()
        {
            var tableList = new List<TableUserLogs>();
            foreach (var item in this)
            {
                tableList.Add(item.GetTableObject());
            }
            return tableList;
        } 
    }

    public class UserLog: BaseDataContainer<TableUserLogs>
    {
        public UserLog(TableUserLogs log) : base(log) { }

        public UserLog() { }

        public int UserLogId { get; set; }

        public int? ProjectId { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        public ELogType LogType { get; set; }

        public int LogTypeId 
        {
            get 
            {
                return (int)LogType;
            }
            set 
            {
                LogType = (ELogType)Enum.Parse(typeof(ELogType), value.ToString());
            }
        }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        public override TableUserLogs GetTableObject()
        {
            TableUserLogs t = new TableUserLogs();
            t.user_log_id = this.UserLogId;
            t.project_id = this.ProjectId;
            t.description = this.Description;
            t.comment = this.Comment;
            t.log_type_id = this.LogTypeId;
            t.time_stamp = this.TimeStamp;
            t.time_stamp_user_name = this.TimeStampUserName;
            return t;
        }

        public override void FromTableObject(TableUserLogs t)
        {
            this.UserLogId = t.user_log_id;
            this.ProjectId = t.project_id;
            this.Description = t.description;
            this.Comment = t.comment;
            this.LogTypeId = t.log_type_id;
            this.TimeStamp = t.time_stamp;
            this.TimeStampUserName = t.time_stamp_user_name;
        }
    }

    public enum ELogType
    { 
        系统日志 = 1,
        用户操作 = 2,
        任务状态 = 3,
        文档管理 = 4,
        账户管理 = 5
    }
}
