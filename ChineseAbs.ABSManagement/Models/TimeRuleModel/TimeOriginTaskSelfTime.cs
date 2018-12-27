using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models
{
    public enum TaskSelfTimeType
    {
        StartTime = 1,
        EndTime = 2,
    }
    public class TimeOriginTaskSelfTime : BaseModel<TableTimeOriginTaskSelfTime>
    {
        public TimeOriginTaskSelfTime()
        {

        }

        public TimeOriginTaskSelfTime(TableTimeOriginTaskSelfTime obj)
            : base(obj)
        {

        }
        public int TimeSeriesId { get; set; }
        public string TimeOriginTimeSeriesGuid { get; set; }
        public string TimeType { get; set; }

        public override TableTimeOriginTaskSelfTime GetTableObject()
        {
            var obj = new TableTimeOriginTaskSelfTime();
            obj.time_origin_task_self_time_id = Id;
            obj.time_origin_task_self_time_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.time_origin_time_series_guid = TimeOriginTimeSeriesGuid;
            obj.time_type = (int)CommUtils.ParseEnum<TaskSelfTimeType>(TimeType);
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeOriginTaskSelfTime obj)
        {
            Id = obj.time_origin_task_self_time_id;
            Guid = obj.time_origin_task_self_time_guid;
            TimeSeriesId = obj.time_series_id;
            TimeOriginTimeSeriesGuid = obj.time_origin_time_series_guid;
            TimeType = ((TaskSelfTimeType)obj.time_type).ToString();
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
