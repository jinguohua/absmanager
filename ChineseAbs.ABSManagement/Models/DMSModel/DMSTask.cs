using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class DMSTask : BaseModel<TableDMSTask>
    {
        public DMSTask()
        {

        }

        public DMSTask(TableDMSTask obj)
            : base(obj)
        {

        }
        public int DmsTaskId { get; set; }
        public string DmsTaskGuid { get; set; }
        public int DmsId { get; set; }
        public string ShortCode { get; set; }

        public override TableDMSTask GetTableObject()
        {
            var obj = new TableDMSTask();
            obj.dms_task_id = Id;
            obj.dms_task_guid = Guid;
            obj.dms_task_id = DmsTaskId;
            obj.dms_task_guid = DmsTaskGuid;
            obj.dms_id = DmsId;
            obj.short_code = ShortCode;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableDMSTask obj)
        {
            Id = obj.dms_task_id;
            Guid = obj.dms_task_guid;
            DmsTaskId = obj.dms_task_id;
            DmsTaskGuid = obj.dms_task_guid;
            DmsId = obj.dms_id;
            ShortCode = obj.short_code;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
