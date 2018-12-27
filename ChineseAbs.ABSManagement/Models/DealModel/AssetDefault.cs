using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class AssetDefault: BaseDataContainer<TableAssetDefault>
    {
        public AssetDefault()
        {
        }

        public AssetDefault(TableAssetDefault assetDefault)
            : base(assetDefault)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int AssetDefaultSetId { get; set; }

        public int AssetId { get; set; }

        public DateTime AssetDefaultDate { get; set; }

        public double RecoveryRate { get; set; }

        public double RecoveryLag { get; set; }

        public DateTime RecoveryDate { get; set; }


        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableAssetDefault GetTableObject()
        {
            var obj = new TableAssetDefault();
            obj.asset_default_id = Id;
            obj.asset_default_guid = Guid;

            obj.asset_default_set_id = AssetDefaultSetId;
            obj.asset_id = AssetId;
            obj.asset_default_date = AssetDefaultDate;
            obj.recovery_rate = RecoveryRate;
            obj.recovery_lag = RecoveryLag;
            obj.recovery_date = RecoveryDate;

            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableAssetDefault obj)
        {
            Id = obj.asset_default_id;
            Guid = obj.asset_default_guid;

            AssetDefaultSetId = obj.asset_default_set_id;
            AssetId = obj.asset_id;
            AssetDefaultDate = obj.asset_default_date;
            RecoveryRate = obj.recovery_rate;
            RecoveryLag = obj.recovery_lag;
            RecoveryDate = obj.recovery_date;

            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
