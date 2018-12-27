using System;

namespace ChineseAbs.ABSManagement.Pattern
{
    /// <summary>
    /// 付息方案申请
    /// </summary>
    public class InterestPaymentPlanApplication
    {
        /// <summary>
        /// 证券简称
        /// </summary>
        public string BondShortName { get; set; }

        /// <summary>
        /// 证券代码
        /// </summary>
        public string BondCode { get; set; }

        /// <summary>
        /// 分配利息总额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 付息登记日期
        /// </summary>
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// 付息文件发放日期
        /// </summary>
        public DateTime FileDate { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime Date { get; set; }
    }
}
