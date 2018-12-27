using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class ColumnJudge : ICellJudge
    {
        public ColumnJudge(List<CellRange> cellRanges, Func<string, bool> exceptCondition)
        {
            CellRanges = cellRanges;
            ExceptCondition = exceptCondition;
        }

        public Dictionary<int, object> ExceptCellByColumn(List<List<object>> table, int iRow)
        {
            if (table.Count <= 0)
            {
                return new Dictionary<int, object>();
            }

            var columnBegin = CellRanges.Min(x => x.ColumnBegin);
            var columnEnd = CellRanges.Min(x => x.ColumnEnd);
            var columns = CellRanges.Select(x => x.ColumnBegin).ToList();

            var row = table[iRow].ToList();
            if (columns.Any(x => ExceptCondition(row[x].ToString())))
            {
                return row.Select((cell, i) => new { cell, i }).ToDictionary(x => x.i, x => x.cell);
            }
            else 
            {
                var newRowDic = new Dictionary<int, object>();
                for (int i = 0; i < row.Count; i++)
                {
                    var cell = row[i];
                    if (!columns.Contains(i))
                    {
                        newRowDic[i] = cell;
                    }
                }
                return newRowDic;
            }
        }

        private List<CellRange> CellRanges { get; set; }

        private Func<string, bool> ExceptCondition { get; set; }
    }
}
