﻿using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Loggers
{
    public class LoggerNewsKeyWords :AbstractLogger 
    {
        public LoggerNewsKeyWords(UserInfo userInfo) : base(userInfo) { }

        protected override void SetLogType()
        {
            LogType = ELogType.用户操作;
        }

        protected override string GetLogDescription(int projectId)
        {
            var projectName = m_dbAdapter.Project.GetProjectNameById(projectId);
            return "用户(" + UserInfo.UserName + ")对项目《" + projectName + "》的新闻关键词进行了操作。";
        }
    }
}
