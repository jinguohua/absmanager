using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class AssetPrepaymentHistory : BaseDataContainer<ABSMgrConn.TableAssetPrepaymentHistory>
    {
        public AssetPrepaymentHistory(ABSMgrConn.TableAssetPrepaymentHistory obj) : base(obj) { }

        public AssetPrepaymentHistory() { }

        public int AssetPrepaymentHistoryId { get; set; }
        public int ProjectId { get; set; }
        public int DatasetId { get; set; }
        public int AssetId { get; set; }
        public DateTime PrepayTime { get; set; }
        public double PrepayAmount { get; set; }
        public string DistributionType { get; set; }
        public string DistributionDetail { get; set; }
        public string Comment { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TimeStampUserName { get; set; }

        public override ABSMgrConn.TableAssetPrepaymentHistory GetTableObject()
        {
            var t = new ABSMgrConn.TableAssetPrepaymentHistory();
            t.asset_prepayment_history_id = AssetPrepaymentHistoryId;
            t.project_id = ProjectId;
            t.dataset_id = DatasetId;
            t.asset_id = AssetId;
            t.prepay_time = PrepayTime;
            t.prepay_amount = PrepayAmount;
            t.distribution_type = DistributionType;
            t.distribution_detail = DistributionDetail;
            t.comment = Comment;
            t.time_stamp = TimeStamp;
            t.time_stamp_user_name = TimeStampUserName;
            return t;
        }

        public override void FromTableObject(ABSMgrConn.TableAssetPrepaymentHistory obj)
        {
            AssetPrepaymentHistoryId = obj.asset_prepayment_history_id;
            ProjectId = obj.project_id;
            DatasetId = obj.dataset_id;
            AssetId = obj.asset_id;
            PrepayTime = obj.prepay_time;
            PrepayAmount = obj.prepay_amount;
            DistributionType = obj.distribution_type;
            DistributionDetail = obj.distribution_detail;
            Comment = obj.comment;
            TimeStamp = obj.time_stamp;
            TimeStampUserName = obj.time_stamp_user_name;
        }
    }
}
