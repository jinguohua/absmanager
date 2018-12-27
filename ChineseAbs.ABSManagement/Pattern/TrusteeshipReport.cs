using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Pattern
{
    /// <summary>
    /// 托管报告
    /// </summary>
    public class TrusteeshipReport
    {
        /// <summary>
        /// 第XX期
        /// </summary>
        public string Phrase { get; set; }

        /// <summary>
        /// 报告开始时间
        /// </summary>
        public DateTime ReportBeginTime { get; set; }

        /// <summary>
        /// 报告结束时间
        /// </summary>
        public DateTime ReportEndTime { get; set; }

        /// <summary>
        /// 专项计划账户收款情况（表）
        /// </summary>
        public List<PayeeInfo> PayeeInfos { get; set; }

        /// <summary>
        /// 专项计划账户付款情况（表）
        /// </summary>
        public List<PayerInfo> PayerInfos { get; set; }

        /// <summary>
        /// 专项计划账户资金余额情况（表）
        /// </summary>
        public List<BalanceInfo> BalanceInfos { get; set; }

        /// <summary>
        /// 对管理人的监督情况
        /// </summary>
        public string SupervisionInfo { get; set; }

        /// <summary>
        /// 对管理人的监督情况
        /// </summary>
        public string Others { get; set; }

        /// <summary>
        /// 银行名
        /// </summary>
        public string BankName { get; set; }
    }

    /// <summary>
    /// 专项计划账户收款情况（行）
    /// </summary>
    public class PayeeInfo
    {
        /// <summary>
        /// 专项计划账户收款情况
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 到账时间
        /// </summary>
        public DateTime ArriveAccountDate { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 进入收入科目
        /// </summary>
        public string RecordedIncome { get; set; }

        /// <summary>
        /// 记入本金科目
        /// </summary>
        public string RecordedPrincipal { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract { get; set; }
        
    }

    /// <summary>
    /// 专项计划付款情况
    /// </summary>
    public class PayerInfo
    {
        /// <summary>
        /// 专项计划账户付款情况
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// 本期支出
        /// </summary>
        public decimal Expenses { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract { get; set; }
    }

    /// <summary>
    /// 专项计划账户资金余额情况
    /// </summary>
    public class BalanceInfo
    {
        /// <summary>
        /// 专项计划账户资金余额
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 本期收入
        /// </summary>
        public decimal Income { get; set; }

        /// <summary>
        /// 本期支出
        /// </summary>
        public decimal Expenses { get; set; }

        /// <summary>
        /// 期末余额
        /// </summary>
        public decimal EndingBalance { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract { get; set; }
    }
}
