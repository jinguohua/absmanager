using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Pattern
{
    /// <summary>
    /// 专项计划分配指令
    /// </summary>
    public class SpecialPlanAllocationInstruction
    {
        /// <summary>
        /// 专项计划分配指令（表）
        /// </summary>
        public List<SpecialPlanAllocationInstructionItem> Payments { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 指令发送人信息栏
        /// </summary>
        public string SenderMessage { get; set; }

        /// <summary>
        /// 划款事由
        /// </summary>
        public string TransferReason { get; set; }
    }

    /// <summary>
    /// 专项计划分配指令（行）
    /// </summary>
    public class SpecialPlanAllocationInstructionItem
    {
        /// <summary>
        /// 付款科目名
        /// </summary>
        public string PaymentSubjectName { get; set; }

        /// <summary>
        /// 收款科目名
        /// </summary>
        public string GatheringSubjectName { get; set; }

        /// <summary>
        /// 记账金额（小写）
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 记账金额（大写）
        /// </summary>
        public string MoneyCN { get; set; }

        /// <summary>
        /// 记账日期
        /// </summary>
        public DateTime AccountDate { get; set; }

        /// <summary>
        /// 记账事由
        /// </summary>
        public string AccountReason { get; set; }
    }
}
