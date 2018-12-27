using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Pattern
{
    public class TemplateTimePattern
    {
        public List<TemplateTimeItem> TemplateTimeList { get; set; }
    }

    public class TemplateTimeItem
    {
        public string TemplateName { get; set; }

        public string TemplateTimeName { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public int TimeSpan { get; set; }

        public TimeSpanUnit TimeSpanUnit { get; set; }

        public TemplateTimeType TemplateTimeType { get; set; }

        public TemplateTimeSearchDirection SearchDirection { get; set; }

        public TemplateTimeHandleReduplicate HandleReduplicate { get; set; }
    }
}
