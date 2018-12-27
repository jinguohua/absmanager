
using System.ServiceProcess;
namespace ChineseAbs.ABSManagement.ReminderHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var thread = new System.Threading.Thread(ReminderHostService.Run)
            {
                IsBackground = true,
                Name = "ThreadReminderHostService"
            };
            thread.Start();
            thread.Join();
#else
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[] 
            { 
                new ReminderHost() 
            };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
