using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Loggers;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagementSite.Common;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public abstract class TaskExBase
    {
        #region Delegate and event

        public HandleResult InvokeStatusChanging(TaskStatus oldStatus, TaskStatus newStatus)
        {
            if (oldStatus == newStatus)
            {
                return new HandleResult(EventResult.Cancel, "无法将任务状态从[" + Toolkit.ToCnString(oldStatus) + "]修改为[" + Toolkit.ToCnString(newStatus) + "]");
            }

            HandleResult handleResult = null;
            switch (newStatus)
            {
                case TaskStatus.Running:
                    handleResult = InvokeEvent(OnStarting);
                    break;
                case TaskStatus.Waitting:
                    handleResult = InvokeEvent(OnStopping);
                    break;
                case TaskStatus.Finished:
                    handleResult = InvokeEvent(OnFinishing);
                    break;
            }

            return handleResult ?? new HandleResult();
        }

        public void InvokeStatusChanged(TaskStatus oldStatus, TaskStatus newStatus)
        {
            switch (newStatus)
            {
                case TaskStatus.Running:
                    InvokeEvent(OnStarted);
                    break;
                case TaskStatus.Waitting:
                    InvokeEvent(OnStopped);
                    break;
                case TaskStatus.Finished:
                    InvokeEvent(OnFinished);
                    break;
            }
        }

        private HandleResult InvokeEvent(DelegatePreChanging func)
        {
            if (func == null)
            {
                return null;
            }

            return func();
        }

        private void InvokeEvent(DelegatePostChanged func)
        {
            if (func != null) { func(); }
        }

        protected delegate HandleResult DelegatePreChanging();

        protected event DelegatePreChanging OnStarting;

        protected event DelegatePreChanging OnFinishing;

        protected event DelegatePreChanging OnStopping;

        protected delegate void DelegatePostChanged();

        protected event DelegatePostChanged OnStarted;

        protected event DelegatePostChanged OnFinished;

        protected event DelegatePostChanged OnStopped;

        #endregion

        public TaskExBase(string userName, string shortCode)
        {
            m_userName = userName;
            m_shortCode = shortCode;
        }

        public abstract object GetEntity();

        protected Task ReloadTask()
        {
            m_task = m_dbAdapter.Task.GetTaskWithExInfo(m_shortCode);
            return m_task;
        }

        private LazyConstruct<DBAdapter> m_dbAdapterInstance;
        protected DBAdapter m_dbAdapter { get { return m_dbAdapterInstance.Get(); } }

        protected AbstractLogger m_logger
        {
            get { return new LoggerGeneric(new UserInfo(m_userName)); }
        }

        protected Task Task { get { return m_task ?? ReloadTask(); } }

        private Task m_task;
        private string m_shortCode;

        protected string m_userName;
    }
}