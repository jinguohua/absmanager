using ChineseAbs.ABSManagement.Utils.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Test
{
    [TestClass]
    public class ExcelUtilsTest
    {

        [TestMethod]
        public void TestValidation()
        {
            var table = new List<List<object>>();
            table.Add(new List<object> { 7657, 1, 2, 3, "2016-09-12" });
            table.Add(new List<object> { 0, 1, 2, 3, "2016-09-12" });
            table.Add(new List<object> { 0, 1, 2, 3, "2016/09/12" });
            table.Add(new List<object> { 0, "123.5", 2, 3, "2016-09-12" });
            table.Add(new List<object> { 0, "123.5", 2, 3, "2016-09-12" });
            table.Add(new List<object> { 0, "23.5", 2, 3, "2016-09-12" });
            table.Add(new List<object> { 0, "23.5", 2, 3, "2016-09-12" });

            var validation = new ExcelValidation();
            validation.Add(CellRange.Cell(0, 0), new CellTextValidation(0, 10));
            validation.Add(CellRange.Column(4), new CellTextValidation(0, 10));
            validation.Add(CellRange.Column(1), new CellNumberValidation());
            validation.Add(CellRange.Column(4), new CellDateValidation());
            validation.Add(CellRange.Row(6), new CellNumberValidation());
            var tableHeaderRowsCount = 1;
            validation.Check(table, tableHeaderRowsCount);
        }
    }
}
