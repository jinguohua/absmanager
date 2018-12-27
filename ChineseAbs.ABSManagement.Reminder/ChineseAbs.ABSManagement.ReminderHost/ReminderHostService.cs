using ChineseAbs.ABSManagement.Foundation;
using Nancy.Hosting.Self;
using System;

namespace ChineseAbs.ABSManagement.ReminderHost
{
    public class ReminderHostService
    {
        public static void Run()
        {
            Log.Info("启动ReminderHostServer...");

            var uri = Config.GetSetting("ReminderHostServer");
            if (string.IsNullOrEmpty(uri))
            {
                Log.Error("未配置ReminderHostServer");
                return;
            }

            HostConfiguration hostConfig = new HostConfiguration();
            hostConfig.UrlReservations.CreateAutomatically = true;

            m_host = new NancyHost(hostConfig, new Uri(uri));
            m_host.Start();

            Log.Info("ReminderHostServer已启动...");

            m_running = true;
            while (m_running)
            {
                System.Threading.Thread.Sleep(3000);
            }
        }

        public static void Stop()
        {
            m_running = false;
            m_host.Stop();
        }

        private static bool m_running = false;
        private static NancyHost m_host;
    }
}
