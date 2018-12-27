using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public class TimeRuleConditionShift : BaseModel<TableTimeRuleConditionShift>
    {
        public TimeRuleConditionShift()
        {

        }

        public TimeRuleConditionShift(TableTimeRuleConditionShift timeRuleConditionShift)
            : base(timeRuleConditionShift)
        {

        }

        public int TimeSeriesId { get; set; }

        public string TimeRuleDateType { get; set; }

        public int TimeInterval { get; set; }

        public string TimeRuleUnitType { get; set; }

        public override TableTimeRuleConditionShift GetTableObject()
        {
            var obj = new TableTimeRuleConditionShift();
            obj.time_rule_condition_shift_id = Id;
            obj.time_rule_condition_shift_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.time_rule_condition_date_type_id = (int)CommUtils.ParseEnum<TimeRuleDateType>(TimeRuleDateType);
            obj.time_interval = TimeInterval;
            obj.time_rule_unit_type_id = (int)CommUtils.ParseEnum<TimeRuleUnitType>(TimeRuleUnitType);
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeRuleConditionShift obj)
        {
            Id = obj.time_rule_condition_shift_id;
            Guid = obj.time_rule_condition_shift_guid;
            TimeSeriesId = obj.time_series_id;
            TimeRuleDateType = ((TimeRuleDateType)obj.time_rule_condition_date_type_id).ToString();
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
