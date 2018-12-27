using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class TaskPeriod : BaseModel<TableTaskPeriod>
    {
        public TaskPeriod()
        {

        }

        public TaskPeriod(TableTaskPeriod obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public string ShortCode { get; set; }
        public DateTime PaymentDate { get; set; }

        public override TableTaskPeriod GetTableObject()
        {
            var obj = new TableTaskPeriod();
            obj.task_period_id = Id;
            obj.task_period_guid = Guid;
            obj.project_id = ProjectId;
            obj.short_code = ShortCode;
            obj.payment_date = PaymentDate;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTaskPeriod obj)
        {
            Id = obj.task_period_id;
            Guid = obj.task_period_guid;
            ProjectId = obj.project_id;
            ShortCode = obj.short_code;
            PaymentDate = obj.payment_date;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
