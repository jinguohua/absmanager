using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeRulePeriodSequenceManager
        : BaseModelManager<TimeRulePeriodSequence, ABSMgrConn.TableTimeRulePeriodSequence>
    {
        public TimeRulePeriodSequenceManager()
        {
            m_defaultTableName = "dbo.TimeRulePeriodSequence";
            m_defaultPrimaryKey = "time_rule_period_sequence_id";
            m_defalutFieldPrefix = "time_rule_period_sequence_";
        }
    }
}
