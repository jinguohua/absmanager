using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class Dataset : BaseDataContainer<ABSMgrConn.TableDataset>
    {
        public Dataset(ABSMgrConn.TableDataset obj) : base(obj) { }

        public Dataset() { }

        public int DatasetId { get; set; }

        public int ProjectId { get; set; }

        /// <summary>
        /// 封包日
        /// </summary>
        public string AsOfDate { get; set; }

        /// <summary>
        /// 支付日
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        public override ABSMgrConn.TableDataset GetTableObject()
        {
            var t = new ABSMgrConn.TableDataset();
            t.dataset_id = DatasetId;
            t.project_id = ProjectId;
            t.payment_date = PaymentDate;
            t.as_of_date = AsOfDate;
            return t;
        }

        public override void FromTableObject(ABSMgrConn.TableDataset obj)
        {
            DatasetId = obj.dataset_id;
            ProjectId = obj.project_id;
            PaymentDate = obj.payment_date;
            AsOfDate = obj.as_of_date;
        }
    }
}
