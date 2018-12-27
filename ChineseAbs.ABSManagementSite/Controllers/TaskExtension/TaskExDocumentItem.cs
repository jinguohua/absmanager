using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    /// <summary>
    /// 文档类型
    /// </summary>
    public enum TaskExDocumentType
    {
        /// <summary>
        /// 发行文件
        /// </summary>
        IssuerFile = 1,
        /// <summary>
        /// 定期报告
        /// </summary>
        RegularReport = 2,
        /// <summary>
        /// 凭证单据
        /// </summary>
        Certificate = 3,
        /// <summary>
        /// 评级报告
        /// </summary>
        GradeReport = 4,
        /// <summary>
        /// 其它文档
        /// </summary>
        Others = 5,
    }

    /// <summary>
    /// 扩展任务中的文档在文档系统中的命名方式
    /// </summary>
    public enum TaskExDocNamingRule
    {
        /// <summary>
        /// [默认] 使用ShortCode作为后缀，e.g. 回购方案(JKLAGG)
        /// </summary>
        ByShortCode = 0,
        /// <summary>
        /// 使用一个Dataset的持续时间命名，e.g. 回购方案(20160731-20161031)
        /// </summary>
        ByDatasetDuration = 1,
    }

    /// <summary>
    /// 文档模板类型
    /// </summary>
    public enum TaskExPatternType
    {
        /// <summary>
        /// 无（不支持根据文件模板自动生成）
        /// </summary>
        None = 0,
        /// <summary>
        /// 收益分配报告
        /// </summary>
        IncomeDistributionReport = 1,
        /// <summary>
        /// 专项计划划款指令
        /// </summary>
        SpecialPlanTransferInstruction = 2,
        /// <summary>
        /// 兑付兑息确认表
        /// </summary>
        CashInterestRateConfirmForm = 3,
        /// <summary>
        /// 付息方案申请
        /// </summary>
        InterestPaymentPlanApplication = 4,
    }

    public class TaskExDocumentItem
    {
        public string Guid { get; set; }

        public string Name { get; set; }

        public TaskExDocumentType DocumentType { get; set; }

        public FileType FileType { get; set; }

        public bool AutoGenerate { get; set; }
    
        public TaskExPatternType PatternType { get; set; }

        public TaskExDocNamingRule NamingRule { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool AutoConfig { get; set; }

        public bool EqualsDoc(TaskExDocumentItem obj)
        {
            return obj.Name == this.Name
                && obj.DocumentType == this.DocumentType
                && obj.AutoGenerate == this.AutoGenerate
                && obj.FileType == this.FileType
                && obj.PatternType == this.PatternType;
        }
    }

    public class TaskExCheckListInfo
    {
        public TaskExCheckListInfo()
        {
            CheckGroups = new List<TaskExCheckGroup>();
        }

        public List<TaskExCheckGroup> CheckGroups { set; get; }
    }

    public class TaskExCheckGroup
    {
        public TaskExCheckGroup()
        {
            CheckItems = new List<TaskExCheckItem>();
        }

        public string GroupName { set; get; }

        public List<TaskExCheckItem> CheckItems { set; get; }
    }

    public class TaskExCheckItem
    {
        public string Guid { set; get; }

        public string Name { set; get; }

        public string CheckStatus { set; get; }

        public bool EqualsCheckList(TaskExCheckItem obj)
        {
            return obj.Name == this.Name
                && obj.CheckStatus == this.CheckStatus;
        }
    }

    /// <summary>
    /// CheckType（检查状态）
    /// </summary>
    public enum TaskExCheckType
    {
        /// <summary>
        /// 已检查
        /// </summary>
        Checked = 1,

        /// <summary>
        /// 未检查
        /// </summary>
        Unchecked = 2
    }
}