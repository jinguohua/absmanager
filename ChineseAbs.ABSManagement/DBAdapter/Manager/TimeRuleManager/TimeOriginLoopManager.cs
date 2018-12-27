using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeOriginLoopManager
        : BaseModelManager<TimeOriginLoop, ABSMgrConn.TableTimeOriginLoop>
    {
        public TimeOriginLoopManager()
        {
            m_defaultTableName = "dbo.TimeOriginLoop";
            m_defaultPrimaryKey = "time_origin_loop_id";
            m_defalutFieldPrefix = "time_origin_loop_";
        }
    }
}
