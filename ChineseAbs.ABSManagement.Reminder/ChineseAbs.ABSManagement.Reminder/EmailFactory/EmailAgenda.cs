using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Foundation;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace ChineseAbs.ABSManagement.Reminder.EmailFactory
{

    class EmailAgenda : EmailBase
    {
        public EmailAgenda(List<Models.MessageReminding> remindings)
            : base(remindings)
        {
            m_remindings = remindings.Where(x => x.Type == MessageUidType.Agenda).ToList();
            m_emailTitle = "日程提醒";
            m_emailDisplayAccount = "ABS发行协作平台";

            if (m_remindings.Count > 0)
            {
                Log.Info("开始日程邮件提醒(未发送邮件数=" + m_remindings.Count + ")");
            }
        }

        public override void LoadData()
        {
            var uids = m_remindings.Select(x => x.Uid).Distinct();
             m_agendas = m_dbAdapter.Agenda.GetByGuids(uids);
            var projectIds = m_agendas.ConvertAll(x => x.ProjectId);
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
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">日程名称</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">产品名称</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">开始时间</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">截止时间</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">日程描述</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">提醒备注</th>");
            builder.Append("</tr>");

            //to do*************************************/////////////////////////////////////////////////////////
            var prefixLink = WebConfigUtils.WebSiteABSManagerPrefix + "Dashboard";
            var remindingsByUser = singleUserRemindings.Value;

            int orderNum = 0;

            for (int i = 0; i < remindingsByUser.Count; i++)
            {
                var reminding = remindingsByUser[i];

                if (!m_agendas.Exists(x => x.Guid == reminding.Uid)) {
                    continue;
                }
                var agenda = m_agendas.Single(x => x.Guid == reminding.Uid);

                if (!m_projects.Exists(x => x.ProjectId == agenda.ProjectId)) {
                    continue;
                }
                orderNum++;
                var project = m_projects.Single(x => x.ProjectId == agenda.ProjectId);
                var projectSeriesGuid = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value).Guid;
                var link = prefixLink+ "?projectSeriesGuid=" + projectSeriesGuid;


                string startTime = agenda.StartTime == null ? "无" : Convert.ToDateTime(agenda.StartTime).ToString("yyyy-MM-dd");
                string endTime = agenda.EndTime == null ? "无" : Convert.ToDateTime(agenda.EndTime).ToString("yyyy-MM-dd");
                string borderCss = "border:1px solid silver";
                builder.Append("<tr><td style=\"" + borderCss + ";text-align:center\">" + orderNum + "</td>");
                builder.Append("<td style=\"" + borderCss + ";word-break:break-all\"><a href='" + link + "'>" + agenda.Name + "</a></td>");
                builder.Append("<td style=\"" + borderCss + ";word-break:break-all\">" +project.Name+ "</td>");
                builder.Append("<td style=\"" + borderCss + "\">" + startTime + "</td>");
                builder.Append("<td style=\"" + borderCss + "\">" + endTime + "</td>");
                builder.Append("<td style=\"" + borderCss + "\">" + agenda.Description + "</td>");
                builder.Append("<td style=\"" + borderCss + "\">"
                    + (string.IsNullOrWhiteSpace(reminding.Remark) ? "无" : reminding.Remark) + "</td></tr>");
            }

            if (orderNum == 0) return null;

            builder.Append("</table >");
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
        private List<Agenda> m_agendas;
    }
}
