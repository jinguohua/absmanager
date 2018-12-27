using System;

namespace ChineseAbs.ABSManagement.Pattern
{
    /// <summary>
    /// 专项计划划款指令
    /// </summary>
    public class SpecialPlanTransferInstruction
    {
        public SpecialPlanTransferInstruction()
        {
            Payer = new BankAccountInfo();
            Payee = new BankAccountInfo();
        }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 划款日期
        /// </summary>
        public DateTime TransferDate { get; set; }

        /// <summary>
        /// 最迟到账日期
        /// </summary>
        public DateTime ArriveAccountDeadline { get; set; }

        /// <summary>
        /// 付款方
        /// </summary>
        public BankAccountInfo Payer { get; set; }

        /// <summary>
        /// 收款方
        /// </summary>
        public BankAccountInfo Payee { get; set; }

        /// <summary>
        /// 大写金额（证券的本金支付+利息支付）
        /// </summary>
        public string MoneyCN { get; set; }
        public decimal Money { get; set; }

        /// <summary>
        /// 大写金额（证券的本金支付+利息支付+中证手续费）
        /// </summary>
        public string MoneyWithCsdcFeeCN { get; set; }
        public decimal MoneyWithCsdcFee { get; set; }

        /// <summary>
        /// 大写金额（证券的本金支付+利息支付+中证手续费+1000元长款逻辑）
        /// 每百元偿付金额小数点第三位后有值（比如56.321）：金额 + 1000
        /// 每百元偿付金额小数点第三位后没有有值（比如56.32）：不处理
        /// </summary>
        public string MoneyWithCsdcFee1000CN { get; set; }
        public decimal MoneyWithCsdcFee1000 { get; set; }

        /// <summary>
        /// 用途及备注
        /// </summary>
        public string UsesAndComment { get; set; }
    }

    /// <summary>
    /// 账户信息
    /// </summary>
    public class BankAccountInfo
    {
        /// <summary>
        /// 户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { get; set; }
    }
}
