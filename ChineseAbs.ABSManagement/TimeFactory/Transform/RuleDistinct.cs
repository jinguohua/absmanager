using System.Linq;

namespace ChineseAbs.ABSManagement.TimeFactory.Transform
{
    public class RuleDistinct : ITransform
    {
        public RuleDistinct()
        {

        }

        public TimeSeriesFactory Transform(TimeSeriesFactory timeSeries)
        {
            return new TimeSeriesFactory(timeSeries.DateTimes.Distinct());
        }
    }
}
