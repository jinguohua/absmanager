using System;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerAccount: AbstractLogger
    {
        public LoggerAccount(UserInfo userInfo)
            : base(userInfo) { }

        protected override void SetLogType()
        {
            LogType = ELogType.账户管理;
        }

        protected override string GetLogDescription(int projectId)
        {
            var projectName = m_dbAdapter.Project.GetProjectNameById(projectId);
            return "用户(" + UserInfo.UserName + ")对项目《" + projectName + "》的账户余额进行了更新操作。";
        }

        public override void Log(int projectId, string commment)
        {
            SetLogType();
            var task = new System.Threading.Tasks.Task(new Action(() => {
                UserLog log = new UserLog();
                log.TimeStampUserName = UserInfo.UserName;
                log.ProjectId = projectId;
                log.TimeStamp = DateTime.Now;
                log.LogTypeId = (int)LogType;
                log.Comment = commment;
                log.Description = GetLogDescription(projectId);
                InsertLog(log);
            }));
            task.Start();
        }
    }
}
