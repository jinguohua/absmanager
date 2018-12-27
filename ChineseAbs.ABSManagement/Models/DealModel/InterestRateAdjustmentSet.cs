using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class InterestRateAdjustmentSet: BaseDataContainer<TableInterestRateAdjustmentSet>
    {
        public InterestRateAdjustmentSet()
        {
        }

        public InterestRateAdjustmentSet(TableInterestRateAdjustmentSet interestRateAdjustmentSet)
            : base(interestRateAdjustmentSet)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableInterestRateAdjustmentSet GetTableObject()
        {
            var obj = new TableInterestRateAdjustmentSet();
            obj.interest_rate_adjustment_set_id = Id;
            obj.interest_rate_adjustment_set_guid = Guid;
            obj.project_id = ProjectId; 
            obj.payment_date = PaymentDate;
            obj.name = Name;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableInterestRateAdjustmentSet obj)
        {
            Id = obj.interest_rate_adjustment_set_id;
            Guid = obj.interest_rate_adjustment_set_guid;
            ProjectId = obj.project_id;
            PaymentDate = obj.payment_date;
            Name = obj.name;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
