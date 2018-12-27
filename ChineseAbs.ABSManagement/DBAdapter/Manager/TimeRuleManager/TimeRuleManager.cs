using ChineseAbs.ABSManagement.Models.TimeRuleModel;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class TimeRuleManager
        : BaseModelManager<TimeRule, ABSMgrConn.TableTimeRule>
    {
        public TimeRuleManager()
        {
            m_defaultTableName = "dbo.TimeRule";
            m_defaultPrimaryKey = "time_rule_id";
            m_defalutFieldPrefix = "time_rule_";
        }

        public List<TimeRule> GetTimeRulesByTimeSeriesId(int timeSeriesId)
        {
            var records = Select<ABSMgrConn.TableTimeRule>("time_series_id",timeSeriesId);
            return records.ToList().ConvertAll(x => new TimeRule(x));
        }
    }
}
