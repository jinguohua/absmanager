using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class CacheAcfAssetReceive : BaseModel<TableCacheAcfAssetReceive>
    {
        public CacheAcfAssetReceive()
        {

        }

        public CacheAcfAssetReceive(TableCacheAcfAssetReceive obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public DateTime PaymentDay { get; set; }
        public int AssetId { get; set; }
        public double Principal { get; set; }
        public double Interest { get; set; }
        public double Perform { get; set; }
        public double Loss { get; set; }
        public double Defaulted { get; set; }
        public double Fee { get; set; }

        public override TableCacheAcfAssetReceive GetTableObject()
        {
            var obj = new TableCacheAcfAssetReceive();
            obj.cache_acf_asset_receive_id = Id;
            obj.cache_acf_asset_receive_guid = Guid;
            obj.project_id = ProjectId;
            obj.payment_day = PaymentDay;
            obj.asset_id = AssetId;
            obj.principal = Principal;
            obj.interest = Interest;
            obj.perform = Perform;
            obj.loss = Loss;
            obj.defaulted = Defaulted;
            obj.fee = Fee;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableCacheAcfAssetReceive obj)
        {
            Id = obj.cache_acf_asset_receive_id;
            Guid = obj.cache_acf_asset_receive_guid;
            ProjectId = obj.project_id;
            PaymentDay = obj.payment_day;
            AssetId = obj.asset_id;
            Principal = obj.principal;
            Interest = obj.interest;
            Perform = obj.perform;
            Loss = obj.loss;
            Defaulted = obj.defaulted;
            Fee = obj.fee;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
