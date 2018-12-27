using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Pattern
{
    public class PaymentDetail
    {
        /// <summary>
        /// 序号，1~N
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 中文名
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
        /// 剩余规模（本金剩余金额，本期还款前）
        /// </summary>
        public decimal Residual { get; set; }

        /// <summary>
        /// 每份资金
        /// </summary>
        public decimal UnitMoney { get; set; }

        /// <summary>
        /// 每份本金
        /// </summary>
        public decimal UnitPrincipal { get; set; }

        /// <summary>
        /// 每份利息
        /// </summary>
        public decimal UnitInterest { get; set; }

        /// <summary>
        /// 原始本金
        /// </summary>
        public decimal Notional { get; set; }

        /// <summary>
        /// 份数
        /// </summary>
        public int UnitCount { get; set; }

        /// <summary>
        /// 资金合计
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 本金
        /// </summary>
        public decimal Principal { get; set; }

        /// <summary>
        /// 利息
        /// </summary>
        public decimal Interest { get; set; }

        /// <summary>
        /// 面额
        /// </summary>
        public decimal Denomination { get; set; }

        /// <summary>
        /// 是否次级
        /// </summary>
        public bool IsEquity { get; set; }

        /// <summary>
        /// 预期收益率（该优先级资产支持证券的）
        /// </summary>
        public string CouponString { set; get; }

        /// <summary>
        /// 还款比例（相对于本期期初本金）
        /// </summary>
        public decimal PrincipalPercentInDataset { set; get; }

        /// <summary>
        /// 还款比例（相对于初始期初本金）
        /// </summary>
        public decimal PrincipalPercent { set; get; }

        /// <summary>
        /// 本期期末余额（本金剩余金额，本期还款后）
        /// </summary>
        public decimal EndingBalance { set; get; }

        /// <summary>
        /// 总支付金额
        /// </summary>
        public decimal SumPaymentAmount { set; get; }
    }

    /// <summary>
    /// 收益分配报告
    /// </summary>
    public class IncomeDistributionReport
    {
        /// <summary>
        /// 第N期
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 第N期(中文小写 一二三)
        /// </summary>
        public string SequenceCN { get; set; }

        /// <summary>
        /// 证券偿付信息
        /// </summary>
        public Dictionary<string, PaymentDetail> Security { get; set; }

        /// <summary>
        /// 优先级证券偿付信息
        /// </summary>
        public List<PaymentDetail> PriorSecurityList { get; set; }

        /// <summary>
        /// 次级证券偿付信息
        /// </summary>
        public List<PaymentDetail> SubSecurityList { get; set; }

        /// <summary>
        /// 全部证券偿付信息
        /// </summary>
        public List<PaymentDetail> SecurityList { get; set; }

        /// <summary>
        /// 汇总
        /// </summary>
        public PaymentDetail Sum { get; set; }

        /// <summary>
        /// 优先级汇总
        /// </summary>
        public PaymentDetail SumPrior { get; set; }

        /// <summary>
        /// 次级汇总
        /// </summary>
        public PaymentDetail SumSub { get; set; }

        /// <summary>
        /// 分配本金情况表
        /// </summary>
        public List<CashItem> PrincipalTable { get; set; }

        /// <summary>
        /// 分配本金情况汇总
        /// </summary>
        public CashItem SumPrincipalTable { get; set; }

        /// <summary>
        /// 面额信息（对应白鹭2016-1中的【分配方案】下的说明）
        /// </summary>
        public string DenominationDetail { get; set; }

        /// <summary>
        /// 分期偿还情况
        /// </summary>
        public string RepayDetail { get; set; }

        /// <summary>
        /// 分期偿还情况（使用带-的券名）
        /// </summary>
        public string RepayDetailWithHyphen { get; set; }

        /// <summary>
        /// 分期偿还情况（使用带-的券名)(金泰专用）
        /// </summary>
        public string RepayDetailWithHyphenByJinTai { get; set; }

        /// <summary>
        /// 分期偿还本金情况（对应白鹭2016-1中的【注：】中内容）
        /// </summary>
        public string RepayPrincipalDetail { get; set; }

        /// <summary>
        /// 权益登记日详情（对应白鹭2016-1）
        /// </summary>
        public string EquityRegisterDetail { get; set; }

        /// <summary>
        /// 权益登记日详情（金泰专用）
        /// </summary>
        public string EquityRegisterDetailByJinTai { get; set; }

        /// <summary>
        /// 权益登记日详情（中港专用）
        /// </summary>
        public string EquityRegisterDetailByZhongGang { get; set; }

        /// <summary>
        /// 权益登记日详情（迎宾馆2016-1专用）
        /// </summary>
        public string EquityRegisterDetailByYingBinGuan { get; set; }

        /// <summary>
        /// 当期天数
        /// </summary>
        public int DurationDayCount { get; set; }

        /// <summary>
        /// 前一个兑付日
        /// </summary>
        public DateTime PreviousT { get; set; }

        /// <summary>
        /// 兑付日
        /// </summary>
        public DateTime T { get; set; }

        /// <summary>
        /// 权益登记日
        /// </summary>
        public DateTime T_1 { get; set; }

        /// <summary>
        /// 签名日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 开始计息时间（含）
        /// </summary>
        public DateTime BeginAccrualDate { get; set; }

        /// <summary>
        /// 截止计息时间（不含）
        /// </summary>
        public DateTime EndAccrualDate { get; set; }

        /// <summary>
        /// 计息时间实际天数（截止计息时间-开始计息时间）
        /// </summary>
        public int AccrualDateSum { get; set; }

        /// <summary>
        /// 工作截止时间
        /// </summary>
        public DateTime TaskEndTime { get; set; }
    }

    public class CashItem
    {
        /// <summary>
        /// 兑付日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 本金兑付比例
        /// </summary>
        public Dictionary<string, decimal> Percent { get; set; }
    }
}
