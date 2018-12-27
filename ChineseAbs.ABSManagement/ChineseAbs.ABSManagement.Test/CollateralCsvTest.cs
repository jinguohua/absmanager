using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChineseAbs.ABSManagement.DocumentFactory;
using System.IO;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.Models.DatasetModel;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class CollateralCsvTest
    {
        [TestMethod]
        public void ReadWriteCollateralCsv()
        {
            var csv = new CollateralCsv();
            var csvPath = @"D:\collateral.csv";
            csv.Load(csvPath);
            csv.UpdateCellValue(3, "PrincipalBalance", "12345");
            csv.UpdateCellValue(5, "AsOfDate", "2016/12/07");
            var newCsvPath = @"D:\collateral-new.csv";
            csv.Save(newCsvPath);
        }
    }
}
