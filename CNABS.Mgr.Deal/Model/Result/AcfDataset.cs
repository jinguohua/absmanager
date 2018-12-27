using System;
using System.Collections.Generic;
using System.Linq;

namespace CNABS.Mgr.Deal.Model.Result
{
    /// <summary>
    /// 单期所有资产的偿付数据
    /// </summary>
    public class AcfDataset : List<AcfUnit>
    {
        public AcfDataset(DateTime paymentDay)
        {
            Sum = new AcfUnitDatasetSum(paymentDay);
            PaymentDay = paymentDay;
        }

        public bool Exists(int assetId)
        {
            return this.Any(x => x.Asset.Id == assetId);
        }

        public AcfUnit Get(int assetId)
        {
            return this.FirstOrDefault(x => x.Asset.Id == assetId);
        }

        public DateTime PaymentDay { get; set; }

        public AcfUnitDatasetSum Sum { get; set; }
    }
}
