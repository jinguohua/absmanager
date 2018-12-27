using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeOriginMetaTaskManager
        : BaseModelManager<TimeOriginMetaTask, ABSMgrConn.TableTimeOriginMetaTask>
    {
        public TimeOriginMetaTaskManager()
        {
            m_defaultTableName = "dbo.TimeOriginMetaTask";
            m_defaultPrimaryKey = "time_origin_meta_task_id";
            m_defalutFieldPrefix = "time_origin_meta_task_";
        }
    }
}
