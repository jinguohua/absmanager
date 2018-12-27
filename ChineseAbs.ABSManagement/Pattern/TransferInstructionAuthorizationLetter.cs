using System;

namespace ChineseAbs.ABSManagement.Pattern
{
    /// <summary>
    /// 划款指令授权书
    /// </summary>
    public class TransferInstructionAuthorizationLetter
    {
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime Begin { get; set; }
        
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 被授权人
        /// </summary>
        public string AuthorizedPersonNames { get; set; }
    }
}
