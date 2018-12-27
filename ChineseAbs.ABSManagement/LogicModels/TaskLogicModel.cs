using ChineseAbs.ABSManagement.Models;
using System;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class TaskLogicModel : BaseLogicModel
    {
        public TaskLogicModel(ProjectLogicModel project, Task task)
            : base(project)
        {
            m_instance = task;
        }

        public TaskLogicModel(ProjectLogicModel project, string shortCode)
            : base(project)
        {
            m_instance = m_dbAdapter.Task.GetTask(shortCode);
        }

        public TaskLogicModel Start(string comment = "")
        {
            UpdateStatus(TaskStatus.Running, comment);
            return this;
        }

        public TaskLogicModel Finish(string comment = "")
        {
            UpdateStatus(TaskStatus.Finished, comment);
            return this;
        }

        public TaskLogicModel Stop(string comment = "")
        {
            var status = m_instance.EndTime < DateTime.Today ? TaskStatus.Overdue : TaskStatus.Waitting;
            UpdateStatus(status, comment);
            return this;
        }

        private void UpdateStatus(TaskStatus newStatus, string comment)
        {
            if (m_instance.TaskStatus != newStatus)
            {
                if (m_dbAdapter.Task.ChangeTaskStatus(m_instance, newStatus, comment))
                {
                    m_instance.TaskStatus = newStatus;
                }
            }
        }

        public Task Instance
        {
            get
            {
                return m_instance;
            }
        }

        private Task m_instance;
    }
}
