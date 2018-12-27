using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Manager
{
    public class TimeOriginTaskSelfTimeManager
        : BaseModelManager<TimeOriginTaskSelfTime, ABSMgrConn.TableTimeOriginTaskSelfTime>
    {
        public TimeOriginTaskSelfTimeManager()
        {
            m_defaultTableName = "dbo.TimeOriginTaskSelfTime";
            m_defaultPrimaryKey = "time_origin_task_self_time_id";
            m_defalutFieldPrefix = "time_origin_task_self_time_";
        }
    }
}
