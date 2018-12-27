using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeOriginManager
        : BaseModelManager<TimeOrigin, ABSMgrConn.TableTimeOrigin>
    {
        public TimeOriginManager()
        {
            m_defaultTableName = "dbo.TimeOrigin";
            m_defaultPrimaryKey = "time_origin_id";
            m_defalutFieldPrefix = "time_origin_";
        }

        public TimeOrigin GetByTimeSeriesId(int timeSeriesId)
        {
            var record = SelectSingle<ABSMgrConn.TableTimeOrigin>("time_series_id", timeSeriesId);
            return new TimeOrigin(record);
        }
    }
}
