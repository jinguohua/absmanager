using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class CacheAcfReceive : BaseModel<TableCacheAcfReceive>
    {
        public CacheAcfReceive()
        {

        }

        public CacheAcfReceive(TableCacheAcfReceive obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public DateTime PaymentDay { get; set; }
        public double Principal { get; set; }
        public double Interest { get; set; }

        public override TableCacheAcfReceive GetTableObject()
        {
            var obj = new TableCacheAcfReceive();
            obj.cache_acf_receive_id = Id;
            obj.cache_acf_receive_guid = Guid;
            obj.project_id = ProjectId;
            obj.payment_day = PaymentDay;
            obj.principal = Principal;
            obj.interest = Interest;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableCacheAcfReceive obj)
        {
            Id = obj.cache_acf_receive_id;
            Guid = obj.cache_acf_receive_guid;
            ProjectId = obj.project_id;
            PaymentDay = obj.payment_day;
            Principal = obj.principal;
            Interest = obj.interest;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
