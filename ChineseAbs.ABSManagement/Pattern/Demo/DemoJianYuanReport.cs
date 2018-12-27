using System;

using PatternTable = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>;

namespace ChineseAbs.ABSManagement.Pattern.Demo
{
    /// <summary>
    /// 建元Demo用报告
    /// </summary>
    public class DemoJianYuanReport
    {
        public DemoJianYuanReport()
        {
            DistInfo = new IncomeDistributionReport();
        }
        /// <summary>
        /// 收益分配报告实例
        /// </summary>
        public IncomeDistributionReport DistInfo { get; set; }

        public PatternTable 违约抵押贷款在本收款期间期末所处的处置状态 { get; set; }

        public PatternTable 入池资产笔数与金额特征 { get; set; }

        public PatternTable 入池资产的期限特征 { get; set; }

        public PatternTable 入池资产利率特征 { get; set; }

        //今年
        public int CurrentYear { get; set; }

        //当年第N期
        public int SequenceInYear { get; set; }

        //第N期
        public int Sequence { get; set; }

        public DateTime ScheduledT { get; set; }

        public DateTime ScheduledPreviousT { get; set; }

        //收款期间开始日
        public DateTime ReceiveMoneyBeginDate { get; set; }
        //收款期间截止日
        public DateTime ReceiveMoneyEndDate { get; set; }
        
        public string ExpenseReceived { get; set; }

        public string TaxReceived { get; set; }

        public decimal 正常贷款金额 { set; get; }

        public decimal 正常贷款金额占比 { set; get; }

        public decimal 拖欠30天以上贷款金额 { set; get; }

        public decimal 拖欠30天以上贷款金额占比 { set; get; }

        public decimal 拖欠60天以上贷款金额 { set; get; }

        public decimal 拖欠60天以上贷款金额占比 { set; get; }

        public decimal 拖欠90天以上贷款金额 { set; get; }

        public decimal 拖欠90天以上贷款金额占比 { set; get; }

        public decimal 期初本金总余额 { set; get; }

        public decimal 期末本金总余额 { set; get; }

        public decimal 本期应收本金 { set; get; }

        public decimal 本期应收利息 { set; get; }

        public string 信托核算日 { set; get; }

        public decimal 收入账_利息_正常回收_上次报告期 { set; get; }

        public decimal 收入账_利息_正常回收_本次报告期 { set; get; }

        public decimal 收入账_利息_提前偿还_上次报告期 { set; get; }

        public decimal 收入账_利息_提前偿还_本次报告期 { set; get; }

        public decimal 收入账_利息_拖欠回收_上次报告期 { set; get; }

        public decimal 收入账_利息_拖欠回收_本次报告期 { set; get; }

        public decimal 收入账_利息_违约回收_上次报告期 { set; get; }

        public decimal 收入账_利息_违约回收_本次报告期 { set; get; }

        public decimal 收入账_利息_合计_上次报告期 { set; get; }

        public decimal 收入账_利息_合计_本次报告期 { set; get; }

        public decimal 收入账_其他收入_上次报告期 { set; get; }

        public decimal 收入账_其他收入_本次报告期 { set; get; }

        public decimal 收入账_上期转存_上次报告期 { set; get; }

        public decimal 收入账_上期转存_本次报告期 { set; get; }

        public decimal 收入账_合格投资_上次报告期 { set; get; }

        public decimal 收入账_合格投资_本次报告期 { set; get; }

        public decimal 收入账_合计_上次报告期 { set; get; }

        public decimal 收入账_合计_本次报告期 { set; get; }

        public decimal 本金账_本金_正常回收_上次报告期 { set; get; }

        public decimal 本金账_本金_正常回收_本次报告期 { set; get; }

        public decimal 本金账_本金_提前偿还_上次报告期 { set; get; }

        public decimal 本金账_本金_提前偿还_本次报告期 { set; get; }

        public decimal 本金账_本金_拖欠回收_上次报告期 { set; get; }

        public decimal 本金账_本金_拖欠回收_本次报告期 { set; get; }

        public decimal 本金账_本金_违约回收_上次报告期 { set; get; }

        public decimal 本金账_本金_违约回收_本次报告期 { set; get; }

        public decimal 本金账_本金_合计_上次报告期 { set; get; }

        public decimal 本金账_本金_合计_本次报告期 { set; get; }

        public decimal 本金账_其他收入_上次报告期 { set; get; }

        public decimal 本金账_其他收入_本次报告期 { set; get; }

        public decimal 本金账_上期转存_上次报告期 { set; get; }

        public decimal 本金账_上期转存_本次报告期 { set; get; }

        public decimal 本金账_合计_上次报告期 { set; get; }

        public decimal 本金账_合计_本次报告期 { set; get; }

        public decimal 收入及本金合计_上次报告期 { set; get; }

        public decimal 收入及本金合计_本次报告期 { set; get; }

        public decimal 税收_上次报告期 { set; get; }

        public decimal 税收_本次报告期 { set; get; }

        public decimal 费用支出_服务总费用支出_上次报告期 { set; get; }

        public decimal 费用支出_服务总费用支出_本次报告期 { set; get; }

        public decimal 费用支出_其他费用支出_上次报告期 { set; get; }

        public decimal 费用支出_其他费用支出_本次报告期 { set; get; }

        public decimal 证券账户支出_证券利息总支出_上次报告期 { set; get; }

        public decimal 证券账户支出_证券利息总支出_本次报告期 { set; get; }

        public decimal 证券账户支出_证券本金总支出_上次报告期 { set; get; }

        public decimal 证券账户支出_证券本金总支出_本次报告期 { set; get; }

    }
}
