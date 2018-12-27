using System;

namespace CNABS.Mgr.Deal.Model.Result
{
    /// <summary>
    /// 单期单笔资产的偿付数据
    /// </summary>
    public class AcfUnit : AcfUnitBase
    {
        public AcfUnit(Asset asset, DateTime paymentDay)
        {
            Asset = asset;
            PaymentDay = paymentDay;
        }

        public Asset Asset { get; private set; }

        public DateTime PaymentDay { get; private set; }
    }
}
