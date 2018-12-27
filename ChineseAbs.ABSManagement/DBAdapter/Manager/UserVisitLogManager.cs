using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using PetaPoco;

namespace ChineseAbs.ABSManagement
{
    public class UserVisitLogManager :BaseManager
    {
        public UserVisitLogManager()
        {
            m_defaultTableName = "dbo.UserVisitLog";
            m_defaultPrimaryKey = "id";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerReminder(UserInfo);
        }

        public void AddUserVisitLogs(List<UserVisitLog> userVisitLogs)
        {
            if (userVisitLogs.Count == 0)
            {
                return;
            }

            var sqls = PetaPoco.Sql.Builder;
     
            foreach(var log in userVisitLogs)  
            {
                sqls.Append(Sql.Builder.Append(" INSERT INTO dbo.UserVisitLog (user_name,request_url,user_agent,ip,time_stamp) values (@0,@1,@2,@3,@4)",
                    log.Username, log.RequestUrl, log.UserAgent, log.Ip, log.TimeStamp.ToString()));
            }  
            m_db.Execute(sqls);
        }
    }
}
