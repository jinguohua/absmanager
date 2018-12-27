using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class AssetDefaultSet: BaseDataContainer<TableAssetDefaultSet>
    {
        public AssetDefaultSet()
        {
        }

        public AssetDefaultSet(TableAssetDefaultSet assetDefaultSet)
            : base(assetDefaultSet)
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

        public override TableAssetDefaultSet GetTableObject()
        {
            var obj = new TableAssetDefaultSet();
            obj.asset_default_set_id = Id;
            obj.asset_default_set_guid = Guid;
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

        public override void FromTableObject(TableAssetDefaultSet obj)
        {
            Id = obj.asset_default_set_id;
            Guid = obj.asset_default_set_guid;
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
