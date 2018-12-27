using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class CalendarCacheTest
    {
        [TestMethod]
        public void TestGetAllTradingDay()
        {
            var endDate = DateTime.Now.AddYears(20);
            var date = DateTime.Now.AddYears(-10);
            
            string result = string.Empty;

            while (date < endDate)
            {
                //CalendarCache.IsTradingDay
                if (CalendarCache.IsWorkingDay(date))
                {
                    result += date.ToString("yyyy-MM-dd");
                    result += "\r\n";
                }

                date = date.AddDays(1);
            }

            Assert.IsTrue(CalendarCache.IsTradingDay(DateTime.Parse("2016-09-22")));
            Assert.IsTrue(!CalendarCache.IsTradingDay(DateTime.Parse("2016-09-24")));
        }
    }
}
