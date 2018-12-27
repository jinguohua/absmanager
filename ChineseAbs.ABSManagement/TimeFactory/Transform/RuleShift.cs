using System.Linq;

namespace ChineseAbs.ABSManagement.TimeFactory.Transform
{
    public class RuleShift : ITransform
    {
        public RuleShift(int interval, TimeUnit timeUnit)
        {
            m_interval = interval;
            m_timeUnit = timeUnit;
        }

        public TimeSeriesFactory Transform(TimeSeriesFactory timeSeries)
        {
            return new TimeSeriesFactory(timeSeries.DateTimes
                .Select(x => x.AddTimeUnit(m_interval, m_timeUnit)));
        }

        private int m_interval;

        private TimeUnit m_timeUnit;
    }
}
