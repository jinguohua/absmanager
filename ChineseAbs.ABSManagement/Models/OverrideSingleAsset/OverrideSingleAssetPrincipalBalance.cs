using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class OverrideSingleAssetPrincipalBalance : BaseModel<TableOverrideSingleAssetPrincipalBalance>
    {
        public OverrideSingleAssetPrincipalBalance()
        {

        }

        public OverrideSingleAssetPrincipalBalance(TableOverrideSingleAssetPrincipalBalance obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int AssetId { get; set; }
        public double PrincipalBalance { get; set; }
        public string Comment { get; set; }

        public override TableOverrideSingleAssetPrincipalBalance GetTableObject()
        {
            var obj = new TableOverrideSingleAssetPrincipalBalance();
            obj.override_single_asset_principal_balance_id = Id;
            obj.override_single_asset_principal_balance_guid = Guid;
            obj.project_id = ProjectId;
            obj.payment_date = PaymentDate;
            obj.asset_id = AssetId;
            obj.principal_balance = PrincipalBalance;
            obj.comment = Comment;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableOverrideSingleAssetPrincipalBalance obj)
        {
            Id = obj.override_single_asset_principal_balance_id;
            Guid = obj.override_single_asset_principal_balance_guid;
            ProjectId = obj.project_id;
            PaymentDate = obj.payment_date;
            AssetId = obj.asset_id;
            PrincipalBalance = obj.principal_balance;
            Comment = obj.comment;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
