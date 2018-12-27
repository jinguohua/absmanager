using System;
using ChineseAbs.ABSManagement.Models;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Loggers
{
    public abstract class AbstractLogger
    {
        public AbstractLogger(UserInfo userInfo)
        {
            this.UserInfo = userInfo;
        }

        public virtual void Log(int id, string commment)
        {
            UserLog log = new UserLog();
            log.TimeStampUserName = UserInfo.UserName;
            log.TimeStamp = DateTime.Now;
            log.LogTypeId = (int)LogType;
            log.Comment = commment;
            log.Description = GetLogDescription(id);
            InsertLog(log);
        }

        protected abstract void SetLogType();

        protected abstract string GetLogDescription(int id);

        protected int InsertLog(UserLog log)
        {
           return (int)m_db.Insert(log.GetTableObject());
        }

        protected ELogType LogType { get; set; }

        protected UserInfo UserInfo { get; set; }

        protected LazyConstruct<DBAdapter> m_dbAdapterInstance;
        protected DBAdapter m_dbAdapter { get { return m_dbAdapterInstance.Get(); } }

        private ABSMgrConnDB m_db = ABSMgrConnDB.GetInstance();
    }
}
