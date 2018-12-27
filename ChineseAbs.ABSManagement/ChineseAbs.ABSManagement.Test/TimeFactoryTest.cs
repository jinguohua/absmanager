using ChineseAbs.ABSManagement.Models.TimeRuleModel;
using ChineseAbs.ABSManagement.TimeFactory;
using ChineseAbs.ABSManagement.TimeFactory.Transform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class TimeFactoryTest
    {
        [TestMethod]
        public void TestTimeFactoryConstructA()
        {
            TimeSeriesFactory ts = new TimeSeriesFactory(
                new DateTime(2017, 5, 5),
                new TimeStep { Interval = 1, Unit = TimeUnit.TradingDay },
                new DateTime(2017, 5, 10));

            var resultA = string.Join(",", ts.DateTimes.Select(x => x.ToShortDateString()));
            var resultB = string.Join(",", new[] {
                "2017/5/5",
                "2017/5/8",
                "2017/5/9",
                "2017/5/10",
            });
            Assert.AreEqual(resultA, resultB);
        }

        [TestMethod]
        public void TestTimeFactoryConstructB()
        {
            TimeSeriesFactory ts = new TimeSeriesFactory(new[]{
                new DateTime(2017, 5, 10),
                new DateTime(2017, 5, 11),
                new DateTime(2017, 5, 13),
                new DateTime(2017, 7, 10),
                new DateTime(2017, 8, 10),
            });

            var resultA = string.Join(",", ts.DateTimes.Select(x => x.ToShortDateString()));
            var resultB = string.Join(",", new[] {
                "2017/5/10",
                "2017/5/11",
                "2017/5/13",
                "2017/7/10",
                "2017/8/10",
            });

            Assert.AreEqual(resultA, resultB);
        }

        [TestMethod]
        public void TestTimeFactoryConstructC()
        {
            TimeSeriesFactory ts = new TimeSeriesFactory(
                new DateTime(2017, 5, 5),
                new TimeStep { Interval = 1, Unit = TimeUnit.Day },
                new DateTime(2017, 5, 10));

            var resultA = string.Join(",", ts.DateTimes.Select(x => x.ToShortDateString()));
            var resultB = string.Join(",", new[] {
                "2017/5/5",
                "2017/5/6",
                "2017/5/7",
                "2017/5/8",
                "2017/5/9",
                "2017/5/10",
            });
            Assert.AreEqual(resultA, resultB);
        }

        [TestMethod]
        public void TestTimeFactoryConstructD()
        {
            TimeSeriesFactory ts = new TimeSeriesFactory(
                new DateTime(2017, 5, 5),
                new TimeStep { Interval = 1, Unit = TimeUnit.Year },
                new DateTime(2019, 5, 10));

            var resultA = string.Join(",", ts.DateTimes.Select(x => x.ToShortDateString()));
            var resultB = string.Join(",", new[] {
                "2017/5/5",
                "2018/5/5",
                "2019/5/5",
            });
            Assert.AreEqual(resultA, resultB);
        }

        [TestMethod]
        public void TestTimeFactoryApplyRuleA()
        {
            TimeSeriesFactory ts = new TimeSeriesFactory(
                new DateTime(2017, 5, 5),
                new TimeStep { Interval = 1, Unit = TimeUnit.TradingDay },
                new DateTime(2017, 5, 10));

            ts.Apply(new RuleShift(3, TimeUnit.Day));

            var resultA = string.Join(",", ts.DateTimes.Select(x => x.ToShortDateString()));
            var resultB = string.Join(",", new[] {
                "2017/5/8",
                "2017/5/11",
                "2017/5/12",
                "2017/5/13",
            });
            Assert.AreEqual(resultA, resultB);
        }

        [TestMethod]
        public void TestTimeFactoryApplyRuleB()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-Hans");

            TimeSeriesFactory ts = new TimeSeriesFactory(
                new DateTime(2017, 5, 5),
                new TimeStep { Interval = 1, Unit = TimeUnit.TradingDay },
                new DateTime(2017, 5, 10));

            ts.Apply(new RuleShift(3, TimeUnit.TradingDay));

            var resultA = string.Join(",", ts.DateTimes.Select(x => x.ToShortDateString()));
            var resultB = string.Join(",", new[] {
                "2017/5/10",
                "2017/5/11",
                "2017/5/12",
                "2017/5/15",
            });
            Assert.AreEqual(resultA, resultB);
        }

        [TestMethod]
        public void TestTimeFactoryApplyRuleC()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-Hans");
            TimeSeriesFactory ts = new TimeSeriesFactory(new[]{
                new DateTime(2017, 5, 10),
                new DateTime(2017, 5, 11),
                new DateTime(2017, 5, 13),
                new DateTime(2017, 7, 10),
                new DateTime(2017, 8, 10),
                new DateTime(2017, 9, 10),
            });

            ts.Apply(new RulePeriodSequence(PeriodType.Month, 3,TimeUnit.Day));
            ts.Apply(new RuleDistinct());
            ts.Apply(new RuleConditionShift(DateType.TradingDay, -1, TimeUnit.TradingDay));

            var resultA = string.Join(",", ts.DateTimes.Select(x => x.ToShortDateString()));
            var resultB = string.Join(",", new[] {
                "2017/5/3",
                "2017/7/3",
                "2017/8/3",
                "2017/9/1",
            });

            Assert.AreEqual(resultA, resultB);
        }
    }
}
