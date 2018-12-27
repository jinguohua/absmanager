using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Foundation;
using ChineseAbs.ABSManagement.Utils;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseAbs.ABSManagement.Reminder.EmailFactory
{
    class EmailNegativeNews : EmailBase
    {
        public EmailNegativeNews(List<Models.MessageReminding> remindings)
            : base(remindings)
        {
            InsertNews();
            m_remindings = remindings.Where(x => x.Type == MessageUidType.NegativeNews).ToList();
            m_emailTitle = "负面新闻提醒";
            m_emailDisplayAccount = "ABS存续期管理系统";

            if (m_remindings.Count > 0)
            {
                Log.Info("开始负面新闻邮件提醒(未发送邮件数=" + m_remindings.Count + ")");
            }
        }

        public override string GenerateContentByUser(KeyValuePair<string, List<MessageReminding>> singleUserRemindings)
        {
            LoadData();
            StringBuilder builder = new StringBuilder();
            builder.Append("<br/>");
            builder.Append("尊敬的用户您好，您管理的存续期产品有以下负面新闻：");
            builder.Append("<br/>");
            builder.Append("<br/>");
            builder.Append("<table style=\" border:1px solid silver;border-collapse:collapse;\" cellPadding=\"10\">");
            builder.Append("<tr>");
            var borderBackGroundCSS = "border:1px solid silver;background-color:#f0f0f0;";
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:center\">序号</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">产品名称</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left\">负面新闻标题</th>");
            builder.Append("<th style=\"" + borderBackGroundCSS + "text-align:left;width:100px\">来源</th>");
            builder.Append("</tr>");

            var prefixLink = WebConfigUtils.WebSiteABSManagerPrefix + "Monitor/project?projectGuid=";
            string borderCss = "border:1px solid silver";
            string alignCss = "text-align:center";


            var remindingsByUser = singleUserRemindings.Value;
            int orderNum = 0;
            for (int i = 0; i < remindingsByUser.Count; i++)
            {
                var reminding = remindingsByUser[i];
                var newsRecord = m_newsRecordList.Single(x => x.ID.ToString() == reminding.Uid);

                if (!m_projects.Exists(x => x.ProjectId == newsRecord.ProjectID))
                {
                    continue;
                }

                var project = m_projects.Single(x => x.ProjectId == newsRecord.ProjectID);
                var link = prefixLink + project.ProjectGuid;
                orderNum++;

                builder.Append("<tr><td style=\"" + borderCss + ";" + alignCss + "\">" + orderNum + "</td>");
                builder.Append("<td style=\"" + borderCss + ";word-break:break-all\"><a href='" + link + "'>" + project.Name + "</a></td>");
                builder.Append("<td style=\"" + borderCss + "\"><a href='" + newsRecord.URL + "'>" + newsRecord.Title + "</a></td>");
                builder.Append("<td style=\"" + borderCss + "\">"
                    + newsRecord.Source + "</td>");
                builder.Append("</tr>");
            }

            if (orderNum == 0) return null;

            builder.Append("</table>");
            builder.Append("<br/>");
            builder.Append("<br/>请尽快操作，谢谢！");
            builder.Append("<br/>如果您在使用中有任何疑问和建议，欢迎您联系CNABS。");
            builder.Append("<br/>联系电话：021-31156258");
            builder.Append("<br/>");
            builder.Append("<br/>ABS存续期管理系统");
            builder.Append("<br/><br/>");

            return builder.ToString();
        }

        public override void LoadData()
        {
            var uids = m_remindings.Select(x => x.Uid);
            m_newsRecordList = m_db.Query<News>(new Sql(" where status=1")).ToList();
            var projectIds = m_newsRecordList.ConvertAll(x => x.ProjectID);
            m_projects = m_dbAdapter.Project.GetProjects(projectIds);
        }

        /// <summary>
        /// 插入负面新闻
        /// </summary>
        public void InsertNews()
        {
            var sql = new Sql("SELECT negativeNews.username,newsRecords.id as recordid FROM [dbo].[NegativeNews] negativeNews " +
                " left join [dbo].[news_records] newsRecords"
                + " on negativeNews.project_id=newsRecords.project_id"
                + " where newsRecords.[status]=@0 and negativeNews.record_status_id!=@1 and subscribe_time<=origin_date",
                NewsConsts.NewsStatusApproved, Models.RecordStatus.Deleted);

            var query = m_db.Fetch<NegativeNewsInfo>(sql);
            var messageRemindNegative = m_db.Query<TableMessageReminding>(
                new Sql(" where type=@0 and [record_status_id]!=@1", MessageUidType.NegativeNews, Models.RecordStatus.Deleted))
                .ToList().ConvertAll(x => new MessageReminding(x));
            foreach (var item in query)
            {
                if (messageRemindNegative.Count(x => x.Uid == item.RecordId && x.UserName == item.Username) == 0)
                {
                    m_dbAdapter.MessageReminding.New(item.RecordId, item.Username, "负面新闻提醒", DateTime.Now, MessageUidType.NegativeNews);
                }
            }
        }

        private List<Project> m_projects;
        private List<News> m_newsRecordList;
    }
}
