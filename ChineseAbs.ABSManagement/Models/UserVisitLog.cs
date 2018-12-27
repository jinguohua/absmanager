using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class UserVisitLog : BaseDataContainer<TableUserVisitLog>
    {
        public UserVisitLog()
        { 

        }

        public UserVisitLog(TableUserVisitLog userVisitLog)
            : base(userVisitLog)
        { 

        }

        public int Id { get; set; }

        public string Username { get; set; }

        public string RequestUrl { get; set; }

        public string UserAgent { get; set; }

        public string Ip { get; set; }

        public DateTime? TimeStamp { get; set; }

        public override TableUserVisitLog GetTableObject()
        {
            var obj = new TableUserVisitLog();
            obj.id = Id;
            obj.user_name = Username;
            obj.request_url = RequestUrl;
            obj.user_agent = UserAgent;
            obj.ip = Ip;
            obj.time_stamp = TimeStamp;
            return obj;
        }

        public override void FromTableObject(TableUserVisitLog obj)
        {
            Id = obj.id;
            Username = obj.user_name;
            RequestUrl = obj.request_url;
            UserAgent = obj.user_agent;
            Ip = obj.ip;
            TimeStamp = obj.time_stamp;
        }
    }
}
