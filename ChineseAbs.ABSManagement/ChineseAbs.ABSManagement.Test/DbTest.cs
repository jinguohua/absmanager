using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class DbTest
    {
        [TestMethod]
        public void CamelCaseConvertorTest()
        {
            var re = DbUtils.CamelCaseToUnderScore("HeIsA27Boy");
            Assert.AreEqual("he_is_a_27_boy", re);
        }

        [TestMethod]
        public void IdToGuidTest()
        {
            var rt = DbUtils.GetGuidById(1, "Accounts", "account_id");

            var rt2 = DbUtils.GetIdByGuid("Jack", "Accounts", "account_guid");
        }
    }
}
