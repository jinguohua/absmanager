using System;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerGeneric: AbstractLogger
    {
        public LoggerGeneric(UserInfo userInfo)
            : base(userInfo) { }

        //[Obsolete("This method is obseleted in the generic logger, to use SetLogType(Elogtype t) instead.")]
        protected override void SetLogType()
        {
            // do nothing
        }

        protected void SetLogType(ELogType t)
        {
            base.LogType = t;
        }

        //[Obsolete("This method is obseleted in the generic logger, to use Log method to write log into the system.")]
        protected override string GetLogDescription(int projectId)
        {
            throw new NotImplementedException();
        }

        public override void Log(int projectId, string commment)
        {
            var task = new System.Threading.Tasks.Task(new Action(() =>
            {
                UserLog log = new UserLog();
                log.ProjectId = projectId;
                log.TimeStampUserName = UserInfo.UserName;
                log.TimeStamp = DateTime.Now;
                log.LogTypeId = (int)LogType;
                log.Comment = commment;
                InsertLog(log);
            }));
            task.Start();
        }
    }
}
