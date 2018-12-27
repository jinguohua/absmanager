using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class AccountAndTransactions
    {
        [TestMethod]
        public void BalanceAndTransactionConsistencyCheck()
        {
            var trans1 = new AccountTransaction();
            trans1.Amount = 100;
            var trans2 = new AccountTransaction();
            trans2.Amount = 0.5m;
            var trans3 = new AccountTransaction();
            trans3.Amount = -10;

            AccountTransactions list = new AccountTransactions();
            list.Add(trans1);
            list.Add(trans2);
            list.Add(trans3);

        }

        [TestMethod]
        public void UserLogTypeTest()
        {
            UserLog log = new UserLog();
            log.LogTypeId = 1;
            Assert.AreEqual(ELogType.系统日志, log.LogType);
            log.LogTypeId = 2;
            Assert.AreEqual(ELogType.用户操作, log.LogType);
            Assert.AreEqual(2, log.LogTypeId);

            log.LogType = ELogType.任务状态;
            Assert.AreEqual(3, log.LogTypeId);
        }
    }
}
