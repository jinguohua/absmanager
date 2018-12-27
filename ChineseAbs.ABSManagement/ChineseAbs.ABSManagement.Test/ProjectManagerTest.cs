using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class ProjectManagerTest
    {
        [TestMethod]
        public void ProjectCountTest()
        {
            var pm = new ProjectManager(new Models.UserInfo("jack"));
            var projects = pm.GetProjectByGuid("jack");
        }
    }
}
