using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public class TimeRuleShift : BaseModel<TableTimeRuleShift>
    {
        public TimeRuleShift()
        {

        }

        public TimeRuleShift(TableTimeRuleShift timeRuleShift)
            : base(timeRuleShift)
        {

        }

        public int TimeSeriesId { get; set; }

        public int TimeInterval { get; set; }

        public string TimeRuleUnitType { get; set; }

        public override TableTimeRuleShift GetTableObject()
        {
            var obj = new TableTimeRuleShift();
            obj.time_rule_shift_id = Id;
            obj.time_rule_shift_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.time_interval = TimeInterval;
            obj.time_rule_unit_type_id = (int)CommUtils.ParseEnum<TimeRuleUnitType>(TimeRuleUnitType);
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeRuleShift obj)
        {
            Id = obj.time_rule_shift_id;
            Guid = obj.time_rule_shift_guid;
            TimeSeriesId = obj.time_series_id;
            TimeInterval = obj.time_interval;
            TimeRuleUnitType = ((TimeRuleUnitType)obj.time_rule_unit_type_id).ToString();
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
