using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public enum MetaTaskTimeType
    {
        StartTime = 1,
        EndTime = 2,
    }

    public class MetaTask : BaseModel<TableMetaTask>
    {
        public MetaTask()
        {

        }

        public MetaTask(TableMetaTask metaTask)
            : base(metaTask)
        {

        }

        public int ProjectId { get; set; }

        public string Name { get; set; }

        public int? StartTimeSeriesId { get; set; }

        public int EndTimeSeriesId { get; set; }

        public string PreMetaTaskIds { get; set; }

        public TaskExtensionType TaskExtensionType { get; set; }

        public string Detail { get; set; }

        public string Target { get; set; }

        public override TableMetaTask GetTableObject()
        {
            var obj = new TableMetaTask();
            obj.meta_task_id = Id;
            obj.meta_task_guid = Guid;
            obj.project_id = ProjectId;
            obj.name = Name;
            obj.start_time_series_id = StartTimeSeriesId.HasValue? StartTimeSeriesId.Value : 0;
            obj.end_time_series_id = EndTimeSeriesId;
            obj.pre_meta_task_ids = PreMetaTaskIds;
            obj.extension_type = TaskExtensionType.ToString();
            obj.detail = Detail;
            obj.target = Target;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableMetaTask obj)
        {
            Id = obj.meta_task_id;
            Guid = obj.meta_task_guid;
            ProjectId = obj.project_id;
            Name = obj.name;
            StartTimeSeriesId = obj.start_time_series_id;
            EndTimeSeriesId = obj.end_time_series_id;
            PreMetaTaskIds = obj.pre_meta_task_ids;
            TaskExtensionType = obj.extension_type == null ? TaskExtensionType.None : CommUtils.ParseEnum<TaskExtensionType>(obj.extension_type);
            Detail = obj.detail;
            Target = obj.target;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
