using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeRuleShiftManager
        : BaseModelManager<TimeRuleShift, ABSMgrConn.TableTimeRuleShift>
    {
        public TimeRuleShiftManager()
        {
            m_defaultTableName = "dbo.TimeRuleShift";
            m_defaultPrimaryKey = "time_rule_shift_id";
            m_defalutFieldPrefix = "time_rule_shift_";
        }
    }
}
