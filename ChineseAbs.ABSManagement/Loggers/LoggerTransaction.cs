using System;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerTransaction: AbstractLogger
    {
        public LoggerTransaction(UserInfo userInfo)
            : base(userInfo) { }

        protected override void SetLogType()
        {
            LogType = ELogType.账户管理;
        }

        protected override string GetLogDescription(int projectId)
        {
            var projectName = m_dbAdapter.Project.GetProjectNameById(projectId);
            return "用户(" + UserInfo.UserName + ")对项目《" + projectName + "》的账户的交易流水进行了操作";
        }

        public override void Log(int projectId, string commment)
        {
            SetLogType();
            var task = new System.Threading.Tasks.Task(new Action(() => {
                UserLog log = new UserLog();
                log.TimeStampUserName = UserInfo.UserName;
                log.TimeStamp = DateTime.Now;
                log.LogTypeId = (int)LogType;
                log.ProjectId = projectId;
                log.Comment = commment;
                log.Description = GetLogDescription(projectId);
                InsertLog(log);
            }));
            task.Start();
        }
    }
}
