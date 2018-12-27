using System;

namespace ChineseAbs.ABSManagement.Pattern
{
    /// <summary>
    /// 兑付兑息确认表
    /// </summary>
    public class CashInterestRateConfirmForm
    {
        /// <summary>
        /// 证券代码
        /// </summary>
        public string BondCode { set; get; }
        /// <summary>
        /// 证券简称
        /// </summary>
        public string ShortBond { set; get; }
        /// <summary>
        /// 中文名 = 证券简称
        /// </summary>
        public string NameCN { get; set; }
        /// <summary>
        /// 带连字符-的中文名
        /// </summary>
        public string NameCNHyphen { get; set; }
        /// <summary>
        /// 带连字符-的全称中文名
        /// </summary>
        public string NameCNFullHyphen { get; set; }
        /// <summary>
        /// 英文名
        /// </summary>
        public string NameEN { get; set; }
        /// <summary>
        /// 带下划线_的英文名
        /// </summary>
        public string NameENUnderline { get; set; }
        /// <summary>
        /// 带连字符-的英文名
        /// </summary>
        public string NameENHyphen { get; set; }
        /// <summary>
        /// 是否浮动利率
        /// </summary>
        public string IsFloatInterestRate { set; get; }
        /// <summary>
        /// 是否分期偿还
        /// </summary>
        public string IsAmortize { set; get; }
        /// <summary>
        /// 每百元兑付（兑息）金额（元）
        /// </summary>
        public decimal CenturyInterestRateMoney { set; get; }
        /// <summary>
        /// 每百元分期偿还本金金额（元）
        /// </summary>
        public decimal? CenturyAmortizeMoney { set; get; }
        /// <summary>
        /// 每千元兑付（兑息）金额（元）
        /// </summary>
        public decimal ThousandInterestRateMoney { set; get; }
        /// <summary>
        /// 每千元分期偿还本金金额（元）
        /// </summary>
        public decimal? ThousandAmortizeMoney { set; get; }
        /// <summary>
        /// 代发证券数量（面值元）
        /// </summary>
        public decimal IssuingBondNum { set; get; }
        /// <summary>
        /// 兑付（兑息、分期偿还）金额（元）
        /// </summary>
        public decimal InterestRateMoney { set; get; }
        /// <summary>
        /// 手续费金额（元）
        /// </summary>
        public decimal FactorageMoney { set; get; }
        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalMoney { set; get; }
        /// <summary>
        /// 债权登记日
        /// </summary>
        public DateTime DebtRegisterDay { set; get; }
        /// <summary>
        /// 兑付（兑息）日
        /// </summary>
        public DateTime InterestRateDay { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime ApplicationDate { set; get; }
    }
}
