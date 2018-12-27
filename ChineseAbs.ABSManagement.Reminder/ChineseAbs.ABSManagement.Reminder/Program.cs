using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Reminder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var thread = new System.Threading.Thread(ReminderService.Run)
            {
                IsBackground = true,
                Name = "ThreadReminderService"
            };
            thread.Start();
            thread.Join();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]   
            {    
                 new ServiceReminder() 
             };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
