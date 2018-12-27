using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class DMSProjectLog : BaseModel<TableDMSProjectLog>
    {
        public DMSProjectLog()
        {

        }

        public DMSProjectLog(TableDMSProjectLog obj)
            : base(obj)
        {

        }
        public int DmsProjectLogId { get; set; }
        public string ProjectGuid { get; set; }
        public string FileSerisGuid { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string TimeStampUserName { get; set; }
        public string Comment { get; set; }

        public override TableDMSProjectLog GetTableObject()
        {
            var obj = new TableDMSProjectLog();
            obj.dms_project_log_id = DmsProjectLogId;
            obj.project_guid = ProjectGuid;
            obj.fileseries_guid = FileSerisGuid;
            obj.time_stamp = TimeStamp;
            obj.time_stamp_user_name = TimeStampUserName;
            obj.comment = Comment;
            return obj;
        }

        public override void FromTableObject(TableDMSProjectLog obj)
        {
            DmsProjectLogId = obj.dms_project_log_id;
            ProjectGuid = obj.project_guid;
            FileSerisGuid = obj.fileseries_guid;
            TimeStamp = obj.time_stamp;
            TimeStampUserName = obj.time_stamp_user_name;
            Comment = obj.comment;
        }
    }
}
