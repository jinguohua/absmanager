using log4net.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Reminder
{
    public partial class ServiceReminder : ServiceBase
    {
        public ServiceReminder()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_thread = new System.Threading.Thread(ReminderService.Run)
            {
                IsBackground = true,
                Name = "ThreadReminderService"
            };

            m_thread.Start();
        }

        protected override void OnStop()
        {
            ReminderService.Stop();
            m_thread.Join();
        }

        private System.Threading.Thread m_thread;
    }
}
