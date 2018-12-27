using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class ColumnComparison : ICellComparison
    {
        public ColumnComparison(CellRange cellRangeLeft, CellRange cellRangeRight, Func<object, object, string> compare)
        {
            ErrorMsg = string.Empty;
            CellRangeLeft = cellRangeLeft;
            CellRangeRight = cellRangeRight;
            Compare = compare;
        }

        public bool CompareColumn(List<List<object>> table, int iRow)
        {
            if (table.Count <= 0)
            {
                return true;
            }

            var columnLeft = CellRangeLeft.ColumnBegin;
            var columnRight = CellRangeRight.ColumnBegin;
            var columnBegin = 0;
            var columnEnd = table[0].Count;

            var row = table[iRow].ToList();
            CommUtils.Assert(columnLeft >= columnBegin && columnRight >= columnBegin, "列下标必须在{0}-{1}之间", columnBegin, columnEnd);
            CommUtils.Assert(columnLeft <= columnEnd && columnRight <= columnEnd, "列下标必须在{0}-{1}之间", columnBegin, columnEnd);
            ErrorMsg = Compare(row[columnLeft], row[columnRight]);
            if(ErrorMsg == string.Empty)
            {
                return true;
            }
            
            return false;
        }

        public string ErrorMsg { get; set; }

        private CellRange CellRangeLeft { get; set; }

        private CellRange CellRangeRight { get; set; }

        private Func<object, object, string> Compare { get; set; }
    }
}
