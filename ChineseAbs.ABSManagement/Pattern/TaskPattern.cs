using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Pattern
{
    public class TaskPattern
    {
        public List<TaskItem> Tasks { get; set; }
    }

    public class TaskItem
    {
        public string ProjectName { get; set; }

        public string TaskName { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string TaskDetail { get; set; }

        public string TaskTarget { get; set; }

        public string TaskExtensionName { get; set; }
    }
}
