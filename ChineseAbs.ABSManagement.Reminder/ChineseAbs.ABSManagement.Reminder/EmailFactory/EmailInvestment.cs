using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Foundation;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseAbs.ABSManagement.Reminder.EmailFactory
{
    class EmailInvestment : EmailBase
    {
        public EmailInvestment(List<Models.MessageReminding> remindings)
            : base(remindings)
        {
            m_remindings = remindings.Where(x => x.Type == MessageUidType.Investment).ToList();
            m_emailTitle = "合格投资提醒";
            m_emailDisplayAccount = "ABS存续期管理系统";

            if (m_remindings.Count > 0)
            {
                Log.Info("开始合格投资邮件提醒(未发送邮件数=" + m_remindings.Count + ")");
            }
        }

        public override void LoadData()
        {
            var uids = m_remindings.Select(x => x.Uid);
            m_investments = m_dbAdapter.Investment.GetByGuids(uids);
            var projectIds = m_investments.ConvertAll(x => x.ProjectId);
            m_projects = m_dbAdapter.Project.GetProjects(projectIds);
        }

        public override string GenerateContentByUser(KeyValuePair<string, List<MessageReminding>> singleUserRemindings)
        {
            LoadData();

            StringBuilder builder = new StringBuilder();
            builder.Append("<br/>");
            builder.Append("尊敬的用户您好，您管理的存续期产品有以下任务急需您处理：");
            builder.Append("<br/>");
            builder.Append("<br/>");
            builder.Append("<table style=\" border:1px solid silver;border-collapse:collapse;\" cellPadding=\"10\">");
            builder.Append("<tr>");
            var borderBackGroundCSS = "border:1px solid silver;background-color:#f0f0f0;";
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:center\">序号</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">产品名称</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">投资标的</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">到期日期</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">提醒备注</th>");
            builder.Append("</tr>");

            var prefixLink = WebConfigUtils.WebSiteABSManagerPrefix + "Investment/?projectGuid=";
            var remindingsByUser = singleUserRemindings.Value;

            int orderNum = 0;

            for (int i = 0; i < remindingsByUser.Count; i++)
            {
                var reminding = remindingsByUser[i];

                if (!m_investments.Exists(x => x.Guid == reminding.Uid)) {
                    continue;
                }
                var investment = m_investments.Single(x => x.Guid == reminding.Uid);

                if (!m_projects.Exists(x => x.ProjectId == investment.ProjectId)) {
                    continue;
                }
                orderNum++;
                var project = m_projects.Single(x => x.ProjectId == investment.ProjectId);
                var link = prefixLink + project.ProjectGuid;
                builder.Append("<tr><td style=\"border:1px solid silver;text-align:center\">" + orderNum+ "</td>");
                builder.Append("<td style=\"border:1px solid silver;word-break:break-all\">" + project.Name + "</td>");
                builder.Append("<td style=\"border:1px solid silver\">");
                builder.Append("<a href='" + link + "'>" + investment.Name + "</a></td>");
                builder.Append("<td style=\"border:1px solid silver;silver\">"
                    + investment.EndTime.ToString("yyyy-MM-dd") + "</td>");

                builder.Append("<td style=\"border:1px solid silver;\">"
                    + (string.IsNullOrWhiteSpace(reminding.Remark) ? "无" : reminding.Remark) + "</td></tr>");
            }

            if (orderNum == 0) return null;

            builder.Append("</table >");
            builder.Append("<br/>");
            builder.Append("<br/>请尽快操作，谢谢！");
            builder.Append("<br/>如果您在使用中有任何疑问和建议，欢迎您联系CNABS。");
            builder.Append("<br/>联系电话：021-31156258");
            builder.Append("<br/>");
            builder.Append("<br/>ABS存续期管理系统");
            builder.Append("<br/><br/>");

            return builder.ToString();
        }

        private List<Project> m_projects;
        private List<Investment> m_investments;
    }
}
