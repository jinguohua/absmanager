using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public class TimeOriginMetaTask : BaseModel<TableTimeOriginMetaTask>
    {
        public TimeOriginMetaTask()
        {

        }

        public TimeOriginMetaTask(TableTimeOriginMetaTask timeOriginMetaTask)
            : base(timeOriginMetaTask)
        {

        }

        public int TimeSeriesId { get; set; }

        public int MetaTaskId { get; set; }

        public string MetaTaskTimeType { get; set; }

        public override TableTimeOriginMetaTask GetTableObject()
        {
            var obj = new TableTimeOriginMetaTask();
            obj.time_origin_meta_task_id = Id;
            obj.time_origin_meta_task_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.meta_task_id = MetaTaskId;
            obj.meta_task_time_type_id = (int)CommUtils.ParseEnum<MetaTaskTimeType>(MetaTaskTimeType);
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeOriginMetaTask obj)
        {
            Id = obj.time_origin_meta_task_id;
            Guid = obj.time_origin_meta_task_guid;
            TimeSeriesId = obj.time_series_id;
            MetaTaskId = obj.meta_task_id;
            MetaTaskTimeType = ((MetaTaskTimeType)obj.meta_task_time_type_id).ToString();
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
