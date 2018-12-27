using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class ProjectSeriesManagerTest
    {
        [TestMethod]
        public void ProjectSeriesNewRecord()
        {
            var mgr = new ProjectSeriesManager(new Models.UserInfo("cgzhou"));
            var begin = DateTime.Now;
            var end = begin.AddDays(30);
            var projectSeries = mgr.NewProjectSeries("MyProjectSeries", ProjectSeriesType.融资租赁, "persionInChargeTest", begin, end, "12@qq.com");

            var projectSeriesDB = mgr.GetByGuid(projectSeries.Guid);
            Assert.AreEqual(projectSeriesDB.Name, projectSeries.Name);

            var projectSeriesDB2 = mgr.GetById(projectSeries.Id);
            Assert.AreEqual(projectSeriesDB2.Name, projectSeries.Name);

            var projectSeries2 = mgr.NewProjectSeries("MyProjectSeries", ProjectSeriesType.融资租赁, "persionInChargeTest", begin, end, "12@qq.com");

            var projectSeriesList = mgr.GetByIds(new List<int> { projectSeries.Id, projectSeries2.Id });
            Assert.IsTrue(projectSeriesList.Count == 2);
            Assert.IsTrue(projectSeriesList.Any(x => x.Id == projectSeries.Id));
            Assert.IsTrue(projectSeriesList.Any(x => x.Id == projectSeries2.Id));
        }
    }
}
