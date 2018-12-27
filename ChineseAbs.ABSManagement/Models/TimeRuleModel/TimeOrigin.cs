using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public enum TimeOriginType
    {
        CustomInput=1,
        Loop=2,
        MetaTask = 3,
        TaskSelfTime = 4,
    }
    public class TimeOrigin : BaseModel<TableTimeOrigin>
    {
        public TimeOrigin()
        {

        }

        public TimeOrigin(TableTimeOrigin timeOrigin)
            : base(timeOrigin)
        {

        }

        public int TimeSeriesId { get; set; }

        public string TimeOriginType { get; set; }

        public int TimeOriginInstanceId { get; set; }

        public override TableTimeOrigin GetTableObject()
        {
            var obj = new TableTimeOrigin();
            obj.time_origin_id = Id;
            obj.time_origin_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.time_origin_type_id = (int)CommUtils.ParseEnum<TimeOriginType>(TimeOriginType);
            obj.time_origin_instance_id = TimeOriginInstanceId;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeOrigin obj)
        {
            Id = obj.time_origin_id;
            Guid = obj.time_origin_guid;
            TimeSeriesId = obj.time_series_id;
            TimeOriginType = ((TimeOriginType)obj.time_origin_type_id).ToString();
            TimeOriginInstanceId = obj.time_origin_instance_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
