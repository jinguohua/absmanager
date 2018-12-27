using ChineseAbs.ABSManagement.Models.TimeRuleModel;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeSeriesManager
        : BaseModelManager<TimeSeries, ABSMgrConn.TableTimeSeries>
    {
        public TimeSeriesManager()
        {
            m_defaultTableName = "dbo.TimeSeries";
            m_defaultPrimaryKey = "time_series_id";
            m_defalutFieldPrefix = "time_series_";
        }

        public TimeSeries NewTimeSeries(string timeSeriesName)
        {
            var timeSeries = new TimeSeries();
            timeSeries.Name = timeSeriesName;
            return New(timeSeries);
        }
    }
}
