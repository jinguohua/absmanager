using System;
using System.Collections.Generic;
using PetaPoco;

namespace ChineseAbs.ABSManagement.Models
{
    public static class NewsConsts
    {
        public const int NewsStatusUnread = 0;
        public const int NewsStatusRead = 1;
        public const int NewsStatusDeleted = 2;
        public const int NewsStatusAll = 3;

        public const int NewsStatusApproved = 1;
        public const int NewsStatusUnAudit = 0;
        public const int NewsStatusDenied = -1;

        public const int DefaultNewsPageSize = 10;
    }

    [TableName("news_records")]
    public class News
    {
        [Column("id")]
        public Int64 ID { get; set; }

        [Column("project_id")]
        public int ProjectID { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("url")]
        public string URL { get; set; }

        [Column("source_from")]
        public string Source { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("origin_date")]
        public DateTime OriginDate { get; set; }
    }

    [TableName("news_records")]
    public class ExtendNews
    {
        [Column("id")]
        public Int64 ID { get; set; }

        [Column("project_id")]
        public int ProjectID { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("url")]
        public string URL { get; set; }

        [Column("source_from")]
        public string Source { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("origin_date")]
        public DateTime OriginDate { get; set; }
    }

    [TableName("User_news_status")]
    public class NewsStatus
    {
        [Column("id")]
        public int ID { get; set; }

        [Column("record_id")]
        public Int64 RecordID { get; set; }

        [Column("project_id")]
        public int ProjectID { get; set; }

        [Column("status")]
        public Int16 Status { get; set; }

        [Column("username")]
        public string Username { get; set; }
    }

    public class PagedNews
    {
        public int Status { get; set; }
        public int CurrentPage { get; set; }
        public DateTime Min { get; set; }
        public DateTime Max { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int UnreadCount { get; set; }
        public int ReadCount { get; set; }
        public int TotalCount { get; set; }
        public List<ExtendNews> News { get; set; }
    }
}
