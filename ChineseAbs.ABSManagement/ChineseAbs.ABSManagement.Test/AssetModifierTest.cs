using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.DocumentFactory;
using System.IO;
using ChineseAbs.ABSManagement;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class AssetModifierTest
    {
        [TestMethod]
        public void GenerateNextDataset()
        {
            var projectId = 148;//测试收益分配报告-中港
            var assetModifier = new AssetModifier("TestUserName");
            assetModifier.Load(projectId, new DateTime(2016, 3, 1));
            assetModifier.GenerateNextDataset(projectId);
        }
    }
}
