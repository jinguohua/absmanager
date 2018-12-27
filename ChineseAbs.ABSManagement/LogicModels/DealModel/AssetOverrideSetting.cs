using ChineseAbs.ABSManagement.Models.DealModel;
using System;

namespace ChineseAbs.ABSManagement.LogicModels.DealModel
{
    /// <summary>
    /// 计算证券端现金流时，使用用户输入覆盖资产池的本金、利息
    /// </summary>
    public class AssetOverrideSetting
    {
        public AssetOverrideSetting(AssetCashflowVariable acfVariable = null)
        {
            if (acfVariable != null && acfVariable.EnableOverride)
            {
                IsOverride = true;
                Interest = acfVariable.InterestCollection;
                Principal = acfVariable.PricipalCollection;
                PaymentDate = acfVariable.PaymentDate;
            }
            else
            {
                IsOverride = false;
                Principal = 0.0;
                Interest = 0.0;
            }
        }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsOverride { get; set; }

        /// <summary>
        /// 支付日（覆盖哪一期）
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// 本金
        /// </summary>
        public double Principal { get; set; }

        /// <summary>
        /// 利息
        /// </summary>
        public double Interest { get; set; }
    }
}
