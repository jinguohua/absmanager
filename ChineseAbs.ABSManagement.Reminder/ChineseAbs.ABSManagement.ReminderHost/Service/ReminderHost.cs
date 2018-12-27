using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.ReminderHost
{
    public partial class ReminderHost : ServiceBase
    {
        public ReminderHost()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_thread = new System.Threading.Thread(ReminderHostService.Run)
            {
                IsBackground = true,
                Name = "ThreadReminderHostService"
            };

            m_thread.Start();
        }

        protected override void OnStop()
        {
            ReminderHostService.Stop();
            m_thread.Join();
        }

        private System.Threading.Thread m_thread;
    }
}
