using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class CashflowVariable : BaseModel<TableCashflowVariable>
    {
        public CashflowVariable()
        {

        }

        public CashflowVariable(TableCashflowVariable obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public double Value { get; set; }

        public override TableCashflowVariable GetTableObject()
        {
            var obj = new TableCashflowVariable();
            obj.cashflow_variable_id = Id;
            obj.cashflow_variable_guid = Guid;
            obj.project_id = ProjectId;
            obj.payment_date = PaymentDate;
            obj.chinese_name = ChineseName;
            obj.english_name = EnglishName;
            obj.value = Value;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableCashflowVariable obj)
        {
            Id = obj.cashflow_variable_id;
            Guid = obj.cashflow_variable_guid;
            ProjectId = obj.project_id;
            PaymentDate = obj.payment_date;
            ChineseName = obj.chinese_name;
            EnglishName = obj.english_name;
            Value = obj.value;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
