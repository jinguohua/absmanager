using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerUserLogout: AbstractLogger
    {
        public LoggerUserLogout(UserInfo userInfo)
            : base(userInfo) { }

        protected override void SetLogType()
        {
            LogType = Models.ELogType.系统日志;
        }

        protected override string GetLogDescription(int projectId)
        {
            return "退出系统";
        }
    }
}
