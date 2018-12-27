using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models
{
    public class CalendarData
    {
        public IEnumerable<string> permission;

        public enum AgendaStatus
        {
            Undefined = 1,
            Waitting = 2,
            Running = 3,
            Finished = 4,
            Skipped = 5,
            Overdue = 6,
            Error = 7
        }

        public string guid { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string desc { get; set; }
        public string createMan { get; set; }
        public string createTime { get; set; }
        public string backgroundColor { get; set; }
        public object reminderInfo { get; set; }
    }

    public class AgendaInfo
    {
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
    }

    public class AgendaDay
    {
        public DateTime StartDate { get; set; }
        public string CnStartDate { get { return GetCnDate(StartDate); } }
        public List<AgendaInfo> AgendaInfos { get; set; }

        public AgendaDay(string strDate)
        {
            StartDate = DateUtils.Parse(strDate).Value;
        }

        public AgendaDay(DateTime date)
        {
            StartDate = date;
        }

        private string GetCnDate(DateTime date)
        {
            var nowDate = DateTime.Today;
            if (date == nowDate)
            {
                return "今天";
            }
            else if (date == nowDate.AddDays(1))
            {
                return "明天";
            }
            else if (date == nowDate.AddDays(2))
            {
                return "后天";
            }

            return date.ToString("MM月dd日");
        }
    }
}
