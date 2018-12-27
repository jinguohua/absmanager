using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Loggers;
using ChineseAbs.ABSManagement.Models;
using ABSMgrConn;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class ParallelLogTest
    {
        [TestMethod]
        public void ParallelAddLog()
        {
            DateTime begin = DateTime.Now;

            //TestMultiLog(500, Log, "Log");

            //TestMultiLog(500, LogUseTask, "LogUseTask");

            //int lastCount = m_taskList.Count;
            //while (m_taskList.Count != 0)
            //{
            //    m_taskList.RemoveAll(x => x.Status == System.Threading.Tasks.TaskStatus.RanToCompletion);
            //    System.Diagnostics.Debug.WriteLine("TaskCount=" + m_taskList.Count);
            //    Thread.Sleep(2000);
            //}
            DateTime end = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("TotalTime:" + (end - begin).TotalMilliseconds);
        }

        private List<System.Threading.Tasks.Task> m_taskList = new List<System.Threading.Tasks.Task>();

        private void TestMultiLog(int logCount, Action<DateTime, int, string> func, string funcName)
        {
            var now = DateTime.Now;
            ParallelLoopResult result = Parallel.For(0, logCount, i =>
            {
                func(now, i, funcName);
            });

            var isCompleted = result.IsCompleted;
            while (!isCompleted)
            {
                isCompleted = result.IsCompleted;
            }

            bool xxx = isCompleted;
            isCompleted = xxx;

            var end = DateTime.Now;

            var threadsCount = Process.GetCurrentProcess().Threads.Count;

            System.Diagnostics.Debug.WriteLine(funcName + ":" + (end - now).TotalMilliseconds + ";ThreadsCount=" + threadsCount.ToString());
        }

        private void LogUseTask(DateTime dateTime, int i, string funcName)
        {
            ABSMgrConnDB db = ABSMgrConnDB.GetInstance();
            var task = new System.Threading.Tasks.Task(new Action(() =>
            {
                for (int j = 0; j < 1000; ++j)
                {
                    UserLog log = new UserLog();
                    log.TimeStampUserName = "ParallelLogTest-ParallelAddLog";
                    log.ProjectId = -1;
                    log.TimeStamp = dateTime;
                    log.LogTypeId = 1;
                    log.Comment = "ParallelLogTest-ParallelAddLog-" + i.ToString();
                    log.Description = funcName;
                    db.Insert(log.GetTableObject());
                }
            }));
            task.Start();
            m_taskList.Add(task);
        }

        private void Log(DateTime dateTime, int i, string funcName)
        {
            for (int j = 0; j < 1000; ++j)
            {
                ABSMgrConnDB db = ABSMgrConnDB.GetInstance();
                UserLog log = new UserLog();
                log.TimeStampUserName = "ParallelLogTest-ParallelAddLog";
                log.ProjectId = -1;
                log.TimeStamp = dateTime;
                log.LogTypeId = 1;
                log.Comment = "ParallelLogTest-ParallelAddLog-" + i.ToString();
                log.Description = funcName;
                db.Insert(log.GetTableObject());
            }
        }
    }
}
