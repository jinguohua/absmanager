using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public class TimeSeries : BaseModel<TableTimeSeries>
    {
        public TimeSeries()
        {

        }

        public TimeSeries(TableTimeSeries timeSeries)
            : base(timeSeries)
        {

        }

        public string Name { get; set; }

        public override TableTimeSeries GetTableObject()
        {
            var obj = new TableTimeSeries();
            obj.time_series_id = Id;
            obj.time_series_guid = Guid;
            obj.name = Name;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeSeries obj)
        {
            Id = obj.time_series_id;
            Guid = obj.time_series_guid;
            Name = obj.name;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
