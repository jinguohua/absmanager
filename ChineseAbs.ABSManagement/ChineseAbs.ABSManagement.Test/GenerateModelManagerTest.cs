using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Manager.DealModel;
using ChineseAbs.ABSManagement.Models.DealModel;
using System.Linq;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class GenerateModelManagerTest
    {
        [TestMethod]
        public void GenerateModelManager()
        {
            var mgr = new AssetCashflowVariableManager(new Models.UserInfo("cgzhou"));

            var record1 = mgr.New(new AssetCashflowVariable{
                ProjectId = -1,
                PaymentDate = DateTime.Now.Date,
                InterestCollection = 1111.00,
                PricipalCollection = 2222.00
            });

            var record2 = mgr.New(new AssetCashflowVariable{
                ProjectId = -2,
                PaymentDate = DateTime.Now.Date,
                InterestCollection = 3333.00,
                PricipalCollection = 4444.00
            });

            var loadRecords = mgr.GetByIds(new int [] { record1.Id, record2.Id });
            Assert.IsTrue(loadRecords.Count == 2);
            Assert.IsTrue(loadRecords.Sum(x => x.InterestCollection) == 4444);
            Assert.IsTrue(loadRecords.Sum(x => x.PricipalCollection) == 6666);

            mgr.Delete(loadRecords.Last());
            loadRecords = mgr.GetByIds(new int[] { record1.Id, record2.Id });
            Assert.IsTrue(loadRecords.Count == 1);
            Assert.IsTrue(loadRecords.Sum(x => x.InterestCollection) == 1111);
            Assert.IsTrue(loadRecords.Sum(x => x.PricipalCollection) == 2222);

            loadRecords.First().InterestCollection = 3333;
            mgr.Update(loadRecords.First());
            var loadRecord = mgr.GetById(loadRecords.First().Id);
            Assert.IsTrue(loadRecord.InterestCollection == 3333);
        }
    }
}
