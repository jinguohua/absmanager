using ChineseAbs.ABSManagement.Foundation;
using ChineseAbs.ABSManagement.Reminder.SmsFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Reminder
{
   public  class SmsReminderService
    {

        public static void Run()
        {
            try
            {
//                 Log.Info("启动ABSManagerReminder(" + GetVersion() + ")...");
                Log.Info("启动SmsABSManagerReminder...");
            }
            catch (Exception ex)
            {
                Log.Error("启动SmsABSManagerReminder失败", ex);
            }

            m_running = true;

            while (m_running)
            {
                try
                {
                    Log.Info("轮询开始");

                    SmsBase.SendSmsUserThread("13916924969", "这是第一条短信", "成功了吗？", "成功了");

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                finally
                {
                    Log.Info("轮询结束");
                }
                m_running = false;
                System.Threading.Thread.Sleep(5000);
            }
        }

        private static bool m_running = false;

    }
}
