using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class NewsManager : BaseManager
    {
        public NewsManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public PagedNews GetProjectNewsDetail(int PID, int? page, int? status, int? pageSize, DateTime? start, DateTime? end)
        {
            PagedNews newsDetail = new PagedNews() {
                CurrentPage = page.HasValue ? (int)page : 1,
                PageSize = pageSize.HasValue ? pageSize.Value : NewsConsts.DefaultNewsPageSize,
                Status = status.HasValue ? (int)status : NewsConsts.NewsStatusAll,
            };
            if (start.HasValue || end.HasValue)
            {
                //最低查询10年前新闻 (筛选出来的都是2010年之后的)
                newsDetail.Min = (start.HasValue ? start.Value : DateTime.Now.AddYears(-10));
                newsDetail.Max = (end.HasValue ? end.Value : DateTime.Now);
                newsDetail.TotalCount = CountNewsByProjectAndStatusAndTime(PID, NewsConsts.NewsStatusAll, newsDetail.Min, newsDetail.Max);
                newsDetail.ReadCount = CountNewsByProjectAndStatusAndTime(PID, NewsConsts.NewsStatusRead, newsDetail.Min, newsDetail.Max);
                newsDetail.News = GetNewsByProjectIDAndTime(PID, newsDetail.CurrentPage, newsDetail.PageSize, newsDetail.Min, newsDetail.Max);
            } else
            {
                newsDetail.TotalCount = CountNewsByProjectAndStatus(PID, NewsConsts.NewsStatusAll);
                newsDetail.ReadCount = CountNewsByProjectAndStatus(PID, NewsConsts.NewsStatusRead);
                newsDetail.News = GetNewsByProjectIDAndStatus(PID, newsDetail.CurrentPage, newsDetail.PageSize, newsDetail.Status);
            }
            newsDetail.UnreadCount = newsDetail.TotalCount - newsDetail.ReadCount;
            newsDetail.TotalPage = newsDetail.TotalCount / newsDetail.PageSize + (newsDetail.TotalCount % newsDetail.PageSize == 0 ? 0 : 1);
            if (newsDetail.TotalPage == 0)
            {
                newsDetail.TotalPage = 1;
            }
            return newsDetail;
        }

        public Int64 CreateNewsRecord(News n)
        {
            m_db.Insert("news_records", "id", n);
            return n.ID;
        }

        private List<ExtendNews> GetNewsByProjectID(int ID, int page, int pageSize)
        {
            string username = UserInfo.UserName;
            int minRN = (page - 1) * pageSize;
            int maxRN = page * pageSize;
            var newsIE = m_db.Query<ExtendNews>(@"with uns as (select [record_id], [project_id], [username], [status] FROM 
            [dbo].[User_news_status] where project_id = @0 and username = @1)

            select * from (SELECT ROW_NUMBER() over (order by id desc) as rn, [news_records].[id], [news_records].[project_id], [title], [url], 
            [source_from], [timestamp], [news_date], [agency], [origin_date], uns.[status] FROM [dbo].[news_records] left join uns on 
            [dbo].[news_records].[id] = uns.record_id where [news_records].project_id = @2 and [news_records].[status] = @3 and 
            (uns.[username] = @4 or uns.[username] is null )) a where rn > @5 and rn <= @6
            ", ID, username, ID, NewsConsts.NewsStatusApproved, username, minRN, maxRN);
            return newsIE.ToList<ExtendNews>();
        }

        public List<ExtendNews> GetNewsByProjectIDAndStatus(int ID, int page, int pageSize, int status)
        {
            if (status == NewsConsts.NewsStatusRead)
            {
                var newsIE = m_db.Page<ExtendNews>(page, pageSize, @"SELECT [news_records].[id], [news_records].[project_id], [title], [url], 
                [source_from], [timestamp], [news_date], [agency], [origin_date], [dbo].[User_news_status].[status] FROM 
                [dbo].[news_records] left join [dbo].[User_news_status] on [dbo].[news_records].[id] = 
                [dbo].[User_news_status].record_id where [news_records].project_id = @0 and [User_news_status].[username] = @1 and 
                [User_news_status].[status] = @2 and [news_records].[status] = @3 
                ", ID, UserInfo.UserName, NewsConsts.NewsStatusRead, NewsConsts.NewsStatusApproved);
                return newsIE.Items;
            } else if (status == NewsConsts.NewsStatusUnread)
            {
                string username = UserInfo.UserName;
                int minRN = (page - 1) * pageSize;
                int maxRN = page * pageSize;
                var newsIE = m_db.Query<ExtendNews>(@"with uns as (select [record_id], [project_id], [username], [status] FROM 
                [dbo].[User_news_status] where project_id = @0 and username = @1)

                select * from (SELECT ROW_NUMBER() over (order by id desc) as rn, [news_records].[id], [news_records].[project_id], [title], [url], 
                [source_from], [timestamp], [news_date], [agency], [origin_date], uns.[status] FROM [dbo].[news_records] left join uns on 
                [dbo].[news_records].[id] = uns.record_id where [news_records].project_id = @2 and [news_records].[status] = @3 and 
                (uns.[username] is null )) a where rn > @4 and rn <= @5
                ", ID, username, ID, NewsConsts.NewsStatusApproved, minRN, maxRN);



                return newsIE.ToList<ExtendNews>();
            }
            return GetNewsByProjectID(ID, page, pageSize);
        }

        public List<ExtendNews> GetNewsByProjectIDAndTime(int ID, int page, int pageSize, DateTime min, DateTime max)
        {
            string username = UserInfo.UserName;
            int minRN = (page - 1) * pageSize;
            int maxRN = page * pageSize;
            var newsIE = m_db.Query<ExtendNews>(@"with uns as (select [record_id], [project_id], [username], [status] FROM 
            [dbo].[User_news_status] where project_id = @0 and username = @1)

            select * from (SELECT ROW_NUMBER() over (order by id desc) as rn, [news_records].[id], [news_records].[project_id], [title], [url], 
            [source_from], [timestamp], [news_date], [agency], [origin_date], uns.[status] FROM [dbo].[news_records] left join uns on 
            [dbo].[news_records].[id] = uns.record_id where [news_records].project_id = @2 and origin_date > @3 and origin_date < @4 
            and [news_records].[status] = @5 and (uns.[username] = @6 or uns.[username] is null )) a where rn > @7 and rn <= @8 
            ", ID, username, ID, min, max, NewsConsts.NewsStatusApproved, username, minRN, maxRN);
            return newsIE.ToList<ExtendNews>();
        }

        //该函数只计算已读 或者 全部新闻数量: 未读请使用全部 减去 已读
        public int CountNewsByProjectAndStatus(int ID, int status)
        {
            int count = 0;

            //已读新闻数量
            if (status == NewsConsts.NewsStatusRead)
            {
                count = m_db.ExecuteScalar<int>(@"select count(distinct record_id) from [dbo].[news_records] inner join 
                [dbo].[User_news_status] on [news_records].id = [User_news_status].record_id where [news_records].[project_id] = @0 
                and username = @1 and [User_news_status].[status] = @2 and [news_records].[status] = @3 
                ", ID, UserInfo.UserName, NewsConsts.NewsStatusRead, NewsConsts.NewsStatusApproved);
                return count;
            }

            //全部新闻数量
            count = m_db.ExecuteScalar<int>(@"select count(1) from [dbo].[news_records] where project_id = @0 and status = @1 ", ID, NewsConsts.NewsStatusApproved);
            return count;
        }

        //该函数只计算已读 或者 全部新闻数量: 未读请使用全部 减去 已读
        public int CountNewsByProjectAndStatusAndTime(int ID, int status, DateTime min, DateTime max)
        {
            int count = 0;

            //已读新闻数量
            if (status == NewsConsts.NewsStatusRead)
            {
                count = m_db.ExecuteScalar<int>(@"select count(1) from [dbo].[news_records] inner join 
                [dbo].[User_news_status] on [news_records].id = [User_news_status].record_id where [news_records].[project_id] = @0 
                and username = @1 and [User_news_status].[status] = @2 and [news_records].[status] = @3 and origin_date > @4 and origin_date < @5
                ", ID, UserInfo.UserName, NewsConsts.NewsStatusRead, NewsConsts.NewsStatusApproved, min, max);
                return count;
            }

            //全部新闻数量
            count = m_db.ExecuteScalar<int>(@"select count(1) from [dbo].[news_records] where project_id = @0 and status = @1 and 
            origin_date > @2 and origin_date < @3", ID, NewsConsts.NewsStatusApproved, min, max);
            return count;
        }

        public bool SetNewsStatusRead(Int64 nid, int pid)
        {
            var ns = m_db.FirstOrDefault<NewsStatus>("SELECT * FROM User_news_status where record_id=@0 and username=@1", nid, UserInfo.UserName);
            if (ns == null)
            {
                NewsStatus status = new NewsStatus();
                status.RecordID = nid;
                status.Status = 1;
                status.Username = UserInfo.UserName;
                status.ProjectID = pid;
                m_db.Insert("User_news_status", status);
                return true;
            }
            m_db.Execute("update User_news_status set status=@0 where record_id=@1 and username=@2", NewsConsts.NewsStatusRead, nid, UserInfo.UserName);
            return true;
        }

        public bool SetAllNewsRead(int pid)
        {
            //取所有未读新闻标记 前1000条,超出无法标记后续
            var news = GetNewsByProjectIDAndStatus(pid, 1, 1000, NewsConsts.NewsStatusUnread);

            foreach (var n in news)
            {
                NewsStatus status = new NewsStatus();
                status.ProjectID = n.ProjectID;
                status.Status = NewsConsts.NewsStatusRead;
                status.Username = UserInfo.UserName;
                status.RecordID = n.ID;
                m_db.Insert("User_news_status", status);
            }
            return true;
        }
    }
}
