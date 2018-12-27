using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Pattern
{
    public class TemplateTaskPattern
    {
        public List<TemplateTaskItem> TemplateTaskList { get; set; }
    }

    public class TemplateTaskItem
    {
        public string TemplateName { get; set; }

        public int TemplateTaskId { get; set; }

        public string TemplateTaskName { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string PrevTemplateTaskIds { get; set; }

        public string TemplateTaskDetail { get; set; }

        public string TemplateTaskTarget { get; set; }

        public string TemplateTaskExtensionName { get; set; }
    }
}
