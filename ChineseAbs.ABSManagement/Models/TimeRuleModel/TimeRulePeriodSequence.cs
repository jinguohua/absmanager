using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public class TimeRulePeriodSequence : BaseModel<TableTimeRulePeriodSequence>
    {
        public TimeRulePeriodSequence()
        {

        }

        public TimeRulePeriodSequence(TableTimeRulePeriodSequence timeRulePeriodSequence)
            : base(timeRulePeriodSequence)
        {

        }

        public int TimeSeriesId { get; set; }

        public string TimeRulePeriodType { get; set; }

        public int Sequence { get; set; }

        public string TimeRuleUnitType { get; set; }

        public override TableTimeRulePeriodSequence GetTableObject()
        {
            var obj = new TableTimeRulePeriodSequence();
            obj.time_rule_period_sequence_id = Id;
            obj.time_rule_period_sequence_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.time_rule_period_type_id = (int)CommUtils.ParseEnum<TimeRulePeriodType>(TimeRulePeriodType);
            obj.time_rule_period_sequence = Sequence;
            obj.time_rule_unit_type_id = (int)CommUtils.ParseEnum<TimeRuleUnitType>(TimeRuleUnitType);
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeRulePeriodSequence obj)
        {
            Id = obj.time_rule_period_sequence_id;
            Guid = obj.time_rule_period_sequence_guid;
            TimeSeriesId = obj.time_series_id;
            TimeRulePeriodType = ((TimeRulePeriodType)obj.time_rule_period_type_id).ToString();
            Sequence = obj.time_rule_period_sequence;
            TimeRuleUnitType = ((TimeRuleUnitType)obj.time_rule_unit_type_id).ToString();
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
