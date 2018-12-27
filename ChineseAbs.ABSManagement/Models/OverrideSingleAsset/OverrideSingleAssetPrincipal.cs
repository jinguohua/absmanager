using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class OverrideSingleAssetPrincipal : BaseModel<TableOverrideSingleAssetPrincipal>
    {
        public OverrideSingleAssetPrincipal()
        {

        }

        public OverrideSingleAssetPrincipal(TableOverrideSingleAssetPrincipal obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int AssetId { get; set; }
        public double Principal { get; set; }
        public string Comment { get; set; }

        public override TableOverrideSingleAssetPrincipal GetTableObject()
        {
            var obj = new TableOverrideSingleAssetPrincipal();
            obj.override_single_asset_principal_id = Id;
            obj.override_single_asset_principal_guid = Guid;
            obj.project_id = ProjectId;
            obj.payment_date = PaymentDate;
            obj.asset_id = AssetId;
            obj.principal = Principal;
            obj.comment = Comment;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableOverrideSingleAssetPrincipal obj)
        {
            Id = obj.override_single_asset_principal_id;
            Guid = obj.override_single_asset_principal_guid;
            ProjectId = obj.project_id;
            PaymentDate = obj.payment_date;
            AssetId = obj.asset_id;
            Principal = obj.principal;
            Comment = obj.comment;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
