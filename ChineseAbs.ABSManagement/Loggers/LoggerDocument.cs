using System;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerDocument : AbstractLogger
    {
        public LoggerDocument(UserInfo userInfo) : base(userInfo)
        {

        }

        protected override string GetLogDescription(int projectId)
        {
            var projectName = m_dbAdapter.Project.GetProjectNameById(projectId);
            return "用户(" + UserInfo.UserName + ")对项目《" + projectName + "》的文档进行了操作。";
        }

        protected override void SetLogType()
        {
            LogType = ELogType.文档管理;
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