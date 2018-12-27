using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeOriginCustomInputManager
        : BaseModelManager<TimeOriginCustomInput, ABSMgrConn.TableTimeOriginCustomInput>
    {
        public TimeOriginCustomInputManager()
        {
            m_defaultTableName = "dbo.TimeOriginCustomInput";
            m_defaultPrimaryKey = "time_origin_custom_input_id";
            m_defalutFieldPrefix = "time_origin_custom_input_";
        }
    }
}
