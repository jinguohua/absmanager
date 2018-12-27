using System;

namespace CNABS.Mgr.Deal.Model.Result
{
    /// <summary>
    /// 单期多笔资产的合计数据
    /// </summary>
    public class AcfUnitDatasetSum : AcfUnitBase
    {
        public AcfUnitDatasetSum(DateTime paymentDay)
        {
            PaymentDay = paymentDay;
        }

        public DateTime PaymentDay { get; set; }
    }
}
