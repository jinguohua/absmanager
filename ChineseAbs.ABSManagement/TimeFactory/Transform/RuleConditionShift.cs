using ChineseAbs.ABSManagement.Utils;
using System;
using System.Linq;

namespace ChineseAbs.ABSManagement.TimeFactory.Transform
{
    public class RuleConditionShift : ITransform
    {
        public RuleConditionShift(DateType appointedDateType, int interval, TimeUnit timeUnit)
        {
            m_dateType = appointedDateType;
            m_interval = interval;
            m_timeUnit = timeUnit;
        }

        public TimeSeriesFactory Transform(TimeSeriesFactory timeSeries)
        {
            return new TimeSeriesFactory(timeSeries.DateTimes
                .Select(x => CheckCondition(x) ? x : x.AddTimeUnit(m_interval, m_timeUnit)));
        }

        private bool CheckCondition(DateTime dateTime)
        {
            switch (m_dateType)
            {
                case DateType.Day:
                    return true;
                case DateType.TradingDay:
                    return CalendarCache.IsTradingDay(dateTime);
                case DateType.WorkingDay:
                    return CalendarCache.IsWorkingDay(dateTime);
            }

            CommUtils.Assert(false, "[CheckCondition]不支持的Date类型");
            return false;
        }

        private int m_interval;

        private TimeUnit m_timeUnit;

        private DateType m_dateType;
    }
}
