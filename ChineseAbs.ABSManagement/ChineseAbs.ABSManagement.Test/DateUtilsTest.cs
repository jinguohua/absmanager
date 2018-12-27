using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class DateUtilsTest
    {
        [TestMethod]
        public void ParseDateTest()
        {
            var dt = new DateTime(2016, 7, 25);

            Assert.AreEqual(dt, DateUtils.Parse("20160725"));
            Assert.AreEqual(dt, DateUtils.Parse("2016/7/25"));
            Assert.AreEqual(dt, DateUtils.Parse("2016/07/25"));
            Assert.AreEqual(dt, DateUtils.Parse("2016-7-25"));
            Assert.AreEqual(dt, DateUtils.Parse("2016-07-25"));

            Assert.AreEqual(null, DateUtils.Parse(""));
            Assert.AreEqual(null, DateUtils.Parse("-"));
            Assert.AreEqual(null, DateUtils.Parse(" "));
            Assert.AreEqual(null, DateUtils.Parse("     "));
            Assert.AreEqual(null, DateUtils.Parse("  -\t   "));
            Assert.AreEqual(null, DateUtils.Parse("\t"));

            try
            {
                Assert.AreEqual(dt, DateUtils.Parse("2016725"));
            }
            catch (ApplicationException)
            {
                Assert.IsTrue(true);
            }

            try
            {
                Assert.AreEqual(dt, DateUtils.Parse("2016//25"));
            }
            catch (ApplicationException)
            {
                Assert.IsTrue(true);
            }
        }
    }
}
