using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models.DealModel
{
    public class AssetCashflowVariable : BaseModel<TableAssetCashflowVariable>
    {
        public AssetCashflowVariable()
        {

        }

        public AssetCashflowVariable(TableAssetCashflowVariable obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public DateTime PaymentDate { get; set; }
        public double InterestCollection { get; set; }
        public double PricipalCollection { get; set; }
        public bool EnableOverride { get; set; }

        public override TableAssetCashflowVariable GetTableObject()
        {
            var obj = new TableAssetCashflowVariable();
            obj.asset_cashflow_variable_id = Id;
            obj.asset_cashflow_variable_guid = Guid;
            obj.project_id = ProjectId;
            obj.payment_date = PaymentDate;
            obj.interest_collection = InterestCollection;
            obj.pricipal_collection = PricipalCollection;
            obj.enable_override = EnableOverride;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableAssetCashflowVariable obj)
        {
            Id = obj.asset_cashflow_variable_id;
            Guid = obj.asset_cashflow_variable_guid;
            ProjectId = obj.project_id;
            PaymentDate = obj.payment_date;
            InterestCollection = obj.interest_collection;
            PricipalCollection = obj.pricipal_collection;
            EnableOverride = obj.enable_override;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
