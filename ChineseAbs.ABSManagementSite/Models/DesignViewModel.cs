using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class DesignViewModel
    {
    }

    public class DesignProjectViewModel
    {
        public List<DesignProjectItem> Projects { get; set; }

        public bool HasCreateProductAuthority { get; set; }
    }

    public class DesignProjectItem
    {
        public string ProjectName { get; set; }

        public string ProjectGuid { get; set; }

        public string EnterpriseName { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public int? CnabsDealId { get; set; }

        public bool HasModifyTaskAuthority { get; set; }

        public bool HasModifyModelAuthority { get; set; }
    }

    #region Design Template

    public class DesignTemplateViewModel
    {
        public List<TemplateViewModel> Templates { get; set; }
    }

    public class TemplateViewModel
    {
        public string TemplateGuid { get; set; }

        public string TemplateName { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }
    }

    public class EditTemplateTaskViewModel
    {
        public string TemplateGuid { set; get; }

        public string TemplateName { set; get; }
        
        public List<TaskTemplateViewModel> TemplateTasks { get; set; }

        public List<TemplateTimeViewModel> TemplateTimes { get; set; }
    }
    public class CheckTaskTimeViewModel
    {
        public List<CheckTaskTime> CheckTaskTime { set; get; }
        
    }
    public class CheckTaskTime
    {
        public Task ErrorTask { set; get; }

        public DateTime? StartTime { set; get; }

        public DateTime? EndTime { set; get; }

        public string ErrorType { set; get; }
    }

    public enum ErrorType
    {
        StartTimeError = 1,
        EndTimeError = 2,
        StartEndTimeError = 3,
        TaskRepeat = 4
    }

    #endregion
}