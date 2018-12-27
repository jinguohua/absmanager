using System;
using System.Collections.Generic;
using System.Linq;

namespace CNABS.Mgr.Deal.Model.Result
{
    /// <summary>
    /// 单笔资产的所有期数据
    /// </summary>
    public class AcfAsset : List<AcfUnit>
    {
        public AcfAsset(Asset asset)
        {
            Asset = asset;
            Sum = new AcfUnitAssetSum(asset);
        }

        public bool Exists(DateTime paymentDay)
        {
            return this.Any(x => x.PaymentDay == paymentDay);
        }

        public AcfUnit Get(DateTime paymentDay)
        {
            return this.FirstOrDefault(x => x.PaymentDay == paymentDay);
        }

        public Asset Asset { get; set; }

        public AcfUnitAssetSum Sum { get; set; }
    }
}
