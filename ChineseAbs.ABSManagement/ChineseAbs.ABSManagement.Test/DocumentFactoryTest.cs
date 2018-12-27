using ChineseAbs.ABSManagement.Pattern;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class DocumentFactoryTest
    {
        [TestMethod]
        public void GenerateIncomeDistributionReport()
        {
            var docFactory = new DocumentFactory.DocumentFactory("testUserName");
            using (MemoryStream ms = new MemoryStream())
            {
                docFactory.Generate(DocPatternType.IncomeDistributionReport, ms, "GY6GLW", "a.docx");

                var filePath = @"D:\备份收益分配报告\Gen\收益分配报告" + DateTime.Now.ToString("yyyyMMdd HH.mm.ss") + ".docx";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(fs);
                }
            }
        }
    }
}
