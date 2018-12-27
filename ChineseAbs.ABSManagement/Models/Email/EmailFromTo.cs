using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models.Email
{
    public class EmailFromTo : BaseModel<TableEmailFromTo>
    {
        public EmailFromTo()
        {

        }

        public EmailFromTo(TableEmailFromTo obj)
            : base(obj)
        {

        }
        public int EmailFromtoId { get; set; }
        public string EmailFromtoGuid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int EmailcontextId { get; set; }
        public int SendOk { get; set; }
        public int EmailRead { get; set; }

        public override TableEmailFromTo GetTableObject()
        {
            var obj = new TableEmailFromTo();
            obj.email_fromto_id = Id;
            obj.email_fromto_guid = Guid;
            obj.email_fromto_id = EmailFromtoId;
            obj.email_fromto_guid = EmailFromtoGuid;
            obj.from = From;
            obj.to = To;
            obj.emailcontextId = EmailcontextId;
            obj.sendOk = SendOk;
            obj.email_read = EmailRead;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableEmailFromTo obj)
        {
            Id = obj.email_fromto_id;
            Guid = obj.email_fromto_guid;
            EmailFromtoId = obj.email_fromto_id;
            EmailFromtoGuid = obj.email_fromto_guid;
            From = obj.from;
            To = obj.to;
            EmailcontextId = obj.emailcontextId;
            SendOk = obj.sendOk;
            EmailRead = obj.email_read;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
