using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerContact : AbstractLogger
    {
        public LoggerContact(UserInfo userInfo)
            : base(userInfo)
        { 
        
        }

        protected override void SetLogType()
        {
            LogType = Models.ELogType.用户操作;
        }

        protected override string GetLogDescription(int projectId)
        {
            var projectName = m_dbAdapter.Project.GetProjectNameById(projectId);
            return "用户(" + UserInfo.UserName + ")对项目《" + projectName + "》的机构联系人信息进行了操作。";
        }
    }
}
