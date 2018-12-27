using ABSMgrConn;
using ChineseAbs.ABSManagement.Foundation;
using ChineseAbs.ABSManagement.Reminder.EmailFactory;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ChineseAbs.ABSManagement.Reminder
{
    public class ReminderService
    {
        public static string GetVersion()
        {
            var assemblyLocation = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            var verInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation);
            var version = (verInfo.FilePrivatePart > 0 ? verInfo.FileVersion
                : string.Format("{0}.{1}.{2}", verInfo.FileMajorPart, verInfo.FileMinorPart, verInfo.FileBuildPart));
            return "版本:" + version;
        }

        public static void Run()
        {
            int remindQueryIntervalSec = 10;
            try
            {
                Log.Info("启动ABSManagerReminder(" + GetVersion() + ")...");
                remindQueryIntervalSec = Convert.ToInt16(ConfigurationManager.AppSettings["remindQueryInterval"]);
            }
            catch (Exception ex)
            {
                Log.Error("启动ABSManagerReminder失败", ex);
            }

            m_running = true;
            while (m_running)
            {
                try
                {
                    //为了防止 "轮询开始" Log过多，第一次轮询后，每隔1分钟写入log
                    //该log仅用于记录发送邮件服务异常终止的时间
                    var now = DateTime.Now;
                    if (!m_lastQueryIntervalTime.HasValue
                        || (now - m_lastQueryIntervalTime.Value).TotalMinutes > 1)
                    {
                        Log.Info("轮询开始");
                        m_lastQueryIntervalTime = now;
                    }
                        
                    var remindList = QueryRemindList();

                    new EmailNegativeNews(remindList).SendAllGroupByUser();

                    new EmailInvestment(remindList).SendAllGroupByUser();

                    new EmailAgenda(remindList).SendAllGroupByUser();

                    new EmailTask(remindList).SendAllGroupByUser();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                finally
                {
                    //防止Log过多，仅记录轮询开始时间即可
                    //Log.Info("轮询结束");
                }

                System.Threading.Thread.Sleep(remindQueryIntervalSec * 1000);
            }
        }

        public static void Stop()
        {
            Log.Info("停止ABSManagerReminder...");
            m_running = false;
        }

        /// <summary>
        /// 查询数据库待发提醒
        /// </summary>
        private static List<Models.MessageReminding> QueryRemindList()
        {
            int remindTimeSpan = Convert.ToInt16(ConfigurationManager.AppSettings["RemindTimeSpan"]);
            var queryList = m_db.Fetch<TableMessageReminding>(
                new Sql(" where record_status_id != @0 and message_status = @1 "
                    + "and abs( datediff(minute,[remind_time],getdate())) <= @2",
                Models.RecordStatus.Deleted, Models.MessageStatusEnum.UnSend, remindTimeSpan));
            return queryList.ConvertAll(x => new Models.MessageReminding(x));
        }

        private static ABSMgrConnDB m_db = ABSMgrConnDB.GetInstance();
        private static DBAdapter m_dbAdapter = new DBAdapter("system");
        private static bool m_running = false;

        private static DateTime? m_lastQueryIntervalTime;
    }

    public class NegativeNewsInfo
    {
        public string Username { get; set; }
        public string RecordId { get; set; }
    }
}
