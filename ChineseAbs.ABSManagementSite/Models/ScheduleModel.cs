using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class ScheduleViewModel
    {
        public ScheduleViewModel()
        {
            Tasks = new List<TaskViewModel>();
            PageInfo = new PageInfo();
            FilterStatus = new List<string>(){
                TaskStatus.Waitting.ToString(),
                TaskStatus.Running.ToString(), 
                TaskStatus.Finished.ToString(), 
                TaskStatus.Skipped.ToString(),
                TaskStatus.Overdue.ToString(),
                TaskStatus.Error.ToString()
            };
            FilterTime = TaskFilterTime.All.ToString();
            PaymentDay = string.Empty;
        }

        public List<string> FilterStatus { get; set; }

        public string FilterTime { get; set; }

        public string PaymentDay { get; set; }

        public List<TaskViewModel> Tasks { get; set; }

        public List<ProjectViewModel> Projects { get; set; }

        public PageInfo PageInfo { get; set; }

        public List<TaskShortCodeStatus> TaskShortCodeStatus { set; get; }
    }
    public class TaskShortCodeStatus
    {
        public string ShortCode { set; get; }

        public string TaskStatus { set; get; }
    }
}