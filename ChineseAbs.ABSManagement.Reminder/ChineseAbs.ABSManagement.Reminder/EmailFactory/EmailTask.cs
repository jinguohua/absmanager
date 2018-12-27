using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Foundation;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace ChineseAbs.ABSManagement.Reminder.EmailFactory
{
    class EmailTask : EmailBase
    {
        public EmailTask(List<Models.MessageReminding> remindings)
            : base(remindings)
        {
            m_remindings = remindings.Where(x => x.Type == MessageUidType.Task).ToList();
            m_emailTitle = "工作提醒";
            m_emailDisplayAccount = "ABS发行协作平台";
            if (m_remindings.Count > 0)
            {
                Log.Info("开始工作邮件提醒(未发送邮件数=" + m_remindings.Count + ")");
            }
        }

        public override void LoadData()
        {
            var uids = m_remindings.Select(x => x.Uid);
            m_tasks = m_dbAdapter.Task.GetTasks(uids);
            var projectIds = m_tasks.ConvertAll(x => x.ProjectId);
            m_projects = m_dbAdapter.Project.GetProjects(projectIds);
        }

        public override string GenerateContentByUser(KeyValuePair<string, List<MessageReminding>> singleUserRemindings)
        {
            LoadData();

            StringBuilder builder = new StringBuilder();
            builder.Append("<br/>");
            builder.Append("尊敬的用户您好，您管理的发行期产品有以下任务急需您处理：");
            builder.Append("<br/>");
            builder.Append("<br/>");
            builder.Append("<table style=\" border:1px solid silver;border-collapse:collapse;\" cellPadding=\"10\">");
            builder.Append("<tr>");
            var borderBackGroundCSS = "border:1px solid silver;background-color:#f0f0f0;";
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:center\">序号</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">工作名称</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">产品名称</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">工作描述</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">开始时间</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">截止时间</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">提醒备注</th>");
            builder.Append("</tr>");

            var prefixLink = WebConfigUtils.WebSiteABSManagerPrefix + "Task?shortCode=";
            var remindingsByUser = singleUserRemindings.Value;

            int orderNum = 0;

            for (int i = 0; i < remindingsByUser.Count; i++)
            {
                var reminding = remindingsByUser[i];

                if (!m_tasks.Exists(x => x.ShortCode == reminding.Uid))
                {
                    continue;
                }
                var task = m_tasks.Single(x => x.ShortCode == reminding.Uid);

                if (!m_projects.Exists(x => x.ProjectId == task.ProjectId))
                {
                    continue;
                }
                orderNum++;
                var project = m_projects.Single(x => x.ProjectId == task.ProjectId);
                var link = prefixLink + task.ShortCode;
                string startTime = task.StartTime == null ? "无" : Convert.ToDateTime(task.StartTime).ToString("yyyy-MM-dd");
                string endTime = task.EndTime == null ? "无" : Convert.ToDateTime(task.EndTime).ToString("yyyy-MM-dd");
                string borderCss = "border:1px solid silver";
                builder.Append("<tr><td style=\"" + borderCss + ";text-align:center\">" + orderNum + "</td>");
                builder.Append("<td style=\"" + borderCss + ";word-break:break-all\"><a href='" + link + "'>" + task.Description + "</a></td>");
                builder.Append("<td style=\"" + borderCss + ";word-break:break-all\">" + project.Name + "</td>");
                builder.Append("<td style=\"" + borderCss + "\">" + task.TaskDetail + "</td>");
                builder.Append("<td style=\"" + borderCss + "\">" + startTime + "</td>");
                builder.Append("<td style=\"" + borderCss + "\">" + endTime + "</td>");
                builder.Append("<td style=\"" + borderCss + "\">"
                    + (string.IsNullOrWhiteSpace(reminding.Remark) ? "无" : reminding.Remark) + "</td></tr>");
            }

            if (orderNum == 0) return null;

            builder.Append("</table>");
            builder.Append("<br/>");
            builder.Append("<br/>请尽快操作，谢谢！");
            builder.Append("<br/>如果您在使用中有任何疑问和建议，欢迎您联系CNABS。");
            builder.Append("<br/>联系电话：021-31156258");
            builder.Append("<br/>");
            builder.Append("<br/>ABS发行协作平台");
            builder.Append("<br/><br/>");

            return builder.ToString();
        }

        private List<Project> m_projects;
        private List<Task> m_tasks;
    }
}
