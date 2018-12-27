
using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Manager
{
    public class DMSProjectLogManager
        : BaseManager
    {
        public DMSProjectLogManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public void AddDmsProjectLog(string projectGuid,string fileSeriesGuid, string comment)
        {
            var dmsProjectLog = new DMSProjectLog();
            dmsProjectLog.ProjectGuid = projectGuid;
            dmsProjectLog.FileSerisGuid = fileSeriesGuid;
            dmsProjectLog.TimeStamp = DateTime.Now;
            dmsProjectLog.TimeStampUserName = UserInfo.UserName;
            dmsProjectLog.Comment = comment;
            NewDmsProjectLog(dmsProjectLog);
        }

        private int NewDmsProjectLog(DMSProjectLog dmsProjectLog)
        {
            var obj = dmsProjectLog.GetTableObject();
            var dmsProjectLogId = m_db.Insert("DMSProjectLog", "dms_project_log_id", true, obj);
            return (int)dmsProjectLogId;
        }

        public List<DMSProjectLog> GetLogs(string projectGuid,string fileSeriesGuid)
        {
            var items = m_db.Query<TableDMSProjectLog>(
                "SELECT * FROM dbo.DMSProjectLog WHERE project_guid = @0 and fileseries_guid=@1 ORDER BY TIME_STAMP DESC", projectGuid,fileSeriesGuid);

            var result = new List<DMSProjectLog>();
            foreach (var item in items)
            {
                result.Add(new DMSProjectLog(item));
            }
            return result;
        }
    }
}
