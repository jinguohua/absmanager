using System;

namespace ChineseAbs.ABSManagement.Models
{
    public enum AssetCashflowHandleType
    {
        None = 0,
        SystemInit = 1,
        UserEdit = 2,
    }

    public enum AssetCashflowFieldType
    {
        None = 0,
        Pricipal = 2,
        Interest = 3,
    }

    public class AssetCashflowHistory : BaseDataContainer<ABSMgrConn.TableAssetCashflowHistory>
    {
        public AssetCashflowHistory(ABSMgrConn.TableAssetCashflowHistory obj) : base(obj) { }

        public AssetCashflowHistory() { }

        public int AssetCashflowHistoryId { get; set; }

        public int ProjectId { get; set; }

        public int AssetId { get; set; }

        public DateTime PaymentDate { get; set; }

        public string AssetName { get; set; }

        public AssetCashflowHandleType HandleType { get; set; }

        public AssetCashflowFieldType FieldType { get; set; }

        public string FieldValue { get; set; }

        public string Comment { get; set; }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        public override ABSMgrConn.TableAssetCashflowHistory GetTableObject()
        {
            var t = new ABSMgrConn.TableAssetCashflowHistory();
            t.asset_cashflow_history_id = AssetCashflowHistoryId;
            t.project_id = ProjectId;
            t.payment_date = PaymentDate;
            t.asset_id = AssetId;
            t.asset_name = AssetName;
            t.field_type = (int)FieldType;
            t.handle_type = (int)HandleType;
            t.field_value = FieldValue;
            t.comment = Comment;
            t.time_stamp = TimeStamp;
            t.time_stamp_user_name = TimeStampUserName;
            return t;
        }

        public override void FromTableObject(ABSMgrConn.TableAssetCashflowHistory obj)
        {
            AssetCashflowHistoryId = obj.asset_cashflow_history_id;
            ProjectId = obj.project_id;
            PaymentDate = obj.payment_date;
            AssetId = obj.asset_id;
            AssetName = obj.asset_name;
            HandleType = (AssetCashflowHandleType)obj.handle_type;
            FieldType = (AssetCashflowFieldType)obj.field_type;
            FieldValue = obj.field_value;
            Comment = obj.comment;
            TimeStamp = obj.time_stamp;
            TimeStampUserName = obj.time_stamp_user_name;
        }
    }
}

