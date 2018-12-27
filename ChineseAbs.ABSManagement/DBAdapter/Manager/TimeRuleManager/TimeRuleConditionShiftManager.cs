using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeRuleConditionShiftManager
        : BaseModelManager<TimeRuleConditionShift, ABSMgrConn.TableTimeRuleConditionShift>
    {
        public TimeRuleConditionShiftManager()
        {
            m_defaultTableName = "dbo.TimeRuleConditionShift";
            m_defaultPrimaryKey = "time_rule_condition_shift_id";
            m_defalutFieldPrefix = "time_rule_condition_shift_";
        }
    }
}
