using ChineseAbs.ABSManagement.TimeFactory.Transform;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.TimeFactory
{
    public class TimeSeriesFactory
    {
        public TimeSeriesFactory(IEnumerable<DateTime> dateTimes)
        {
            m_dateTimes = dateTimes;
        }

        public TimeSeriesFactory(DateTime begin, TimeStep timeStep, DateTime end)
        {
            m_dateTimes = GenerateLoopTimes(begin, timeStep, end);
        }

        public void Apply(IEnumerable<ITransform> transforms)
        {
            transforms.ToList().ForEach(Apply);
        }

        public void Apply(ITransform transforms)
        {
            m_dateTimes = transforms.Transform(this).DateTimes;
        }

        public IEnumerable<DateTime> DateTimes { get { return m_dateTimes; } }

        private IEnumerable<DateTime> GenerateLoopTimes(DateTime begin, TimeStep timeStep, DateTime end)
        {
            int count = 0;

            var dateTimes = new List<DateTime>();
            while (begin <= end)
            {
                dateTimes.Add(begin);
                begin = begin.AddTimeStep(timeStep);

                ++count;
                CommUtils.Assert(count < 10000, "时间序列数量过大（{0}），无法继续生成", count);
            }

            return dateTimes;
        }

        private IEnumerable<DateTime> m_dateTimes;
    }
}
