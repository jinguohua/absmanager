using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public enum TimeRulePeriodType
    {
        Year = 1,
        Month = 2,
        Week = 3,
    }

    public enum TimeRuleDateType
    {
        Day = 1,
        TradingDay = 2,
        WorkingDay = 3,
    }

    public enum TimeRuleUnitType
    {
        Year = 1,
        Month = 2,
        Day = 3,
        TradingDay = 4,
        WorkingDay = 5,
    }

    /// <summary>
    /// 移动时间方向（1 过去（向前） minus， 2 将来（向后） plus）
    /// </summary>
    public enum TimeMoveDirection
    {
        Minus = 1,
        Plus = 2
    }

    /// <summary>
    /// 规则类型
    /// </summary>
    public enum TimeRuleType
    {
        /// <summary>
        /// 移动指定日期
        /// </summary>
        MoveAppointDate = 1,
        /// <summary>
        /// 查找指定日期
        /// </summary>
        FindAppointDate = 2,
        /// <summary>
        /// 替换指定日期
        /// </summary>
        ReplaceAppointDate = 3,
        /// <summary>
        /// 去掉重复日期
        /// </summary>
        RemoveRepeatDate = 4
    }

    public class TimeRule : BaseModel<TableTimeRule>
    {
        public TimeRule()
        {

        }

        public TimeRule(TableTimeRule timeRule)
            : base(timeRule)
        {

        }

        public int TimeSeriesId { get; set; }

        public int TimeRuleOrder { get; set; }

        public TimeRuleType TimeRuleType { get; set; }

        public int TimeRuleInstanceId { get; set; }

        public override TableTimeRule GetTableObject()
        {
            var obj = new TableTimeRule();
            obj.time_rule_id = Id;
            obj.time_rule_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.time_rule_order = TimeRuleOrder;
            obj.time_rule_type_id = (int)TimeRuleType;
            obj.time_rule_instance_id = TimeRuleInstanceId;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeRule obj)
        {
            Id = obj.time_rule_id;
            Guid = obj.time_rule_guid;
            TimeSeriesId = obj.time_series_id;
            TimeRuleOrder = obj.time_rule_order;
            TimeRuleType = (TimeRuleType)obj.time_rule_type_id;
            TimeRuleInstanceId = obj.time_rule_instance_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
