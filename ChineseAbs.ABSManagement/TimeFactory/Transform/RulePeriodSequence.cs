using ChineseAbs.ABSManagement.Utils;
using System;
using System.Linq;

namespace ChineseAbs.ABSManagement.TimeFactory.Transform
{
    public class RulePeriodSequence : ITransform
    {
        public RulePeriodSequence(PeriodType periodType, int sequence, TimeUnit unitType)
        {
            m_periodType = periodType;
            m_sequence = sequence;
            m_unitType = unitType;
        }

        public TimeSeriesFactory Transform(TimeSeriesFactory timeSeries)
        {
            return new TimeSeriesFactory(timeSeries.DateTimes
                .Select(x => {
                    var time = PeriodStart(x);

                    if (CheckCondition(time))
                    {
                        return time.AddTimeUnit(m_sequence - 1, m_unitType);
                    }
                    return time.AddTimeUnit(m_sequence, m_unitType);
                }));
        }

        private DateTime PeriodStart(DateTime dateTime)
        {
            switch (m_periodType)
            {
                case PeriodType.Year:
                    return dateTime.AddDays(1 - dateTime.DayOfYear);
                case PeriodType.Month:
                    return dateTime.AddDays(1 - dateTime.Day);
                case PeriodType.Week:
                    return dateTime.AddDays(1 - (int)dateTime.DayOfWeek);
            }

            CommUtils.Assert(false, "[PeriodStart]不支持的PeriodType类型");
            return dateTime;
        }

        private bool CheckCondition(DateTime dateTime)
        {
            switch (m_unitType)
            {
                case TimeUnit.Day:
                    return true;
                case TimeUnit.TradingDay:
                    return CalendarCache.IsTradingDay(dateTime);
                case TimeUnit.WorkingDay:
                    return CalendarCache.IsWorkingDay(dateTime);
                default:
                    return false;
            }
        }

        private int m_sequence;

        private PeriodType m_periodType;

        private TimeUnit m_unitType;
    }
}
