using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models.Email
{
    public class EmailContext : BaseModel<TableEmailContext>
    {
        public EmailContext()
        {

        }

        public EmailContext(TableEmailContext obj)
            : base(obj)
        {

        }
        public string Content { get; set; }
        public string Subject { get; set; }
        public int? IsDraft { get; set; }
        public DateTime? sentTime { get; set; }

        public override TableEmailContext GetTableObject()
        {
            var obj = new TableEmailContext();
            obj.email_context_id = Id;
            obj.email_context_guid = Guid;
            obj.content = Content;
            obj.subject = Subject;
            obj.isDraft = IsDraft;
            obj.sentTime = sentTime;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableEmailContext obj)
        {
            Id = obj.email_context_id;
            Guid = obj.email_context_guid;
            Content = obj.content;
            Subject = obj.subject;
            IsDraft = obj.isDraft;
            sentTime = obj.sentTime ;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
