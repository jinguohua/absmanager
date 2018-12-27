using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class CommUtilsTest
    {
        [TestMethod]
        public void ConvertIntToCnTest()
        {
            Assert.AreEqual(1.ToCnString(), "一");
            Assert.AreEqual(9.ToCnString(), "九");
            Assert.AreEqual(12.ToCnString(), "十二");
            Assert.AreEqual(95.ToCnString(), "九十五");
            Assert.AreEqual(101.ToCnString(), "一百零一");
            Assert.AreEqual(995.ToCnString(), "九百九十五");
            Assert.AreEqual(1068.ToCnString(), "一千零六十八");
            Assert.AreEqual(9999.ToCnString(), "九千九百九十九");
            Assert.AreEqual(10211.ToCnString(), "一万零二百一十一");
        }
    }
}
