using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerUserLogin: AbstractLogger
    {
        public LoggerUserLogin(UserInfo userInfo)
            : base(userInfo) { }

        protected override void SetLogType()
        {
            LogType = Models.ELogType.系统日志;
        }

        protected override string GetLogDescription(int projectId)
        {
            return "登录系统";
        }
    }
}
