using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class ProjectManagerViewModel
    {
        public ProjectManagerViewModel()
        {
            Projects = new List<ProjectViewModel>();
            PageInfo = new PageInfo();
        }

        public List<ProjectViewModel> Projects { get; set; }

        public PageInfo PageInfo { get; set; }

        public bool HasCreateProjectAuthority { get; set; }

        public bool HasEditProjectAuthority { get; set; }

        public bool HasEditModelAuthority { get; set; }

        public bool IsSuperUser { set; get; }
    }

    public class DashboardViewModel
    {

    }

    public class ProjectSeriesViewModel
    {

    }

    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            Tasks = new List<TaskViewModel>();
            PageInfo = new PageInfo();
        }

        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Guid")]
        public string Guid { get; set; }

        [Display(Name = "产品名称")]
        public string ProjectName { get; set; }

        [Display(Name = "类型")]
        public string ProjectType { get; set; }

        [Display(Name = "截止日")]
        public string Deadline { get; set; }

        [Display(Name = "原始权益人")]
        public string Originator { get; set; }

        [Display(Name = "下个计算日")]
        public string NextCalcDate { get; set; }

        [Display(Name = "覆盖率")]
        public string CoverageRatio { get; set; }

        [Display(Name = "下一偿付日")]
        public string NextPaymentDate { get; set; }

        [Display(Name = "当前状态")]
        public ProjectStatus ProjectStatus { get; set; }

        [Display(Name = "当前工作")]
        public TaskViewModel CurrentTask { get; set; }

        [Display(Name = "负责人")]
        public string PersonInCharge { get; set; }

        [Display(Name = "信息")]
        public int Message { get; set; }

        public List<TaskViewModel> Tasks { get; set; }

        public PageInfo PageInfo { get; set; }
    }

    public class EditModelViewModel
    {
        public ProjectViewModel Project { get; set; }

        public List<EditModelDatasetViewModel> Datasets { get; set; }
    }

    public class EditModelDatasetViewModel : Dataset
    {
        public EditModelDatasetViewModel(Dataset dataset)
        {
            this.DatasetId = dataset.DatasetId;
            this.ProjectId = dataset.ProjectId;
            this.AsOfDate = dataset.AsOfDate;
            this.PaymentDate = dataset.PaymentDate;
        }

        public string Sequence { get; set; }
    }

    public class EditTaskViewModel
    {
        public ProjectViewModel Project { get; set; }

        public List<TaskViewModel> Tasks { get; set; }

        public bool HasCreateProjectAuthority { get; set; }
    }

    public class TemplateTimeViewModel
    {
        public int TemplateTimeId { get; set; }

        public int TemplateId { get; set; }

        public string TemplateTimeName { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public int TimeSpan { get; set; }

        public string TimeSpanUnit { get; set; }

        public string TemplateTimeType { get; set; }

        public string SearchDirection { get; set; }

        public string HandleReduplicate { get; set; }
    }

    public class TaskTemplateViewModel
    {
        public int RowSequence { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string BeginDate { get; set; }

        public string TriggerDate { get; set; }

        public string PrevTaskNames { get; set; }

        public List<int> PrevTaskIds { get; set; }

        public string PrevTaskSequences { get; set; }

        public string ExtensionName { get; set; }

        public string Detail { get; set; }

        public string Target { get; set; }
    }

    /// <summary>
    /// Stage类型
    /// </summary>
    public enum StageType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined,
        /// <summary>
        /// 存续期
        /// </summary>
        DurationPeriod,
        /// <summary>
        /// 清算日
        /// </summary>
        SettlementDate,
        /// <summary>
        /// 自定义
        /// </summary>
        Custom
    }

    /// <summary>
    /// 产品状态
    /// </summary>
    public enum ProjectStatus
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined,
        /// <summary>
        /// 收集信息
        /// </summary>
        CollectInfo,
        /// <summary>
        /// 观察
        /// </summary>
        InObservation,
        /// <summary>
        /// 正常
        /// </summary>
        Normal
    }
}
