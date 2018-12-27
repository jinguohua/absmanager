using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class NegativeNews : BaseModel<TableNegativeNews>
    {
        public NegativeNews()
        {

        }

        public NegativeNews(TableNegativeNews obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public string Username { get; set; }
        public DateTime SubscribeTime { get; set; }

        public override TableNegativeNews GetTableObject()
        {
            var obj = new TableNegativeNews();
            obj.negative_news_id = Id;
            obj.negative_news_guid = Guid;
            obj.project_id = ProjectId;
            obj.username = Username;
            obj.subscribe_time = SubscribeTime;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableNegativeNews obj)
        {
            Id = obj.negative_news_id;
            Guid = obj.negative_news_guid;
            ProjectId = obj.project_id;
            Username = obj.username;
            SubscribeTime = obj.subscribe_time;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
