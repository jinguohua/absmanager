using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public class TimeOriginCustomInput : BaseModel<TableTimeOriginCustomInput>
    {
        public TimeOriginCustomInput()
        {

        }

        public TimeOriginCustomInput(TableTimeOriginCustomInput timeOriginCustomInput)
            : base(timeOriginCustomInput)
        {

        }

        public int TimeSeriesId { get; set; }

        public string CustomTimeSeries { get; set; }

        public override TableTimeOriginCustomInput GetTableObject()
        {
            var obj = new TableTimeOriginCustomInput();
            obj.time_origin_custom_input_id = Id;
            obj.time_origin_custom_input_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.custom_time_series = CustomTimeSeries;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeOriginCustomInput obj)
        {
            Id = obj.time_origin_custom_input_id;
            Guid = obj.time_origin_custom_input_guid;
            TimeSeriesId = obj.time_series_id;
            CustomTimeSeries = obj.custom_time_series;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
