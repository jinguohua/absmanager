using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class ExcelValidation
    {
        public ExcelValidation()
        {
            m_validations = new List<ExcelValidationItem>();
        }
        
        public void Add(ExcelValidationItem validation)
        {
            m_validations.Add(validation);
        }

        public void Add(CellRange cellRange, ICellValidation cellValidation)
        {
            m_validations.Add(new ExcelValidationItem(cellRange, cellValidation));
        }

        public void Clear()
        {
            m_validations.Clear();
        }

        public void Check(List<List<object>> table, int tableHeaderRowsCount)
        {
            if (table.Count <= 0)
            {
                return;
            }

            var rowBegin = m_validations.Min(x => x.CellRange.RowBegin);
            var columnBegin = m_validations.Min(x => x.CellRange.ColumnBegin);
            var rowEnd = m_validations.Max(x => x.CellRange.RowEnd);
            var columnEnd = m_validations.Max(x => x.CellRange.ColumnEnd);

            for (int i = 0; i < table.Count; i++)
            {
                if (i < rowBegin || i > rowEnd)
                {
                    continue;
                }

                var row = table[i];
                var newRowDic = JudgeColumn == null 
                    ? row.ToList().Select((cell, iCell) => new { cell, iCell }).ToDictionary(x => x.iCell, x => x.cell)
                    : JudgeColumn.ExceptCellByColumn(table, i);
                for (int j = 0; j < row.Count; j++)
                {
                    if (j < columnBegin || j > columnEnd || (!newRowDic.Keys.Contains(j)))
                    {
                        continue;
                    }

                    var cell = row[j];
                    var cellText = cell == null ? string.Empty : cell.ToString();
                    foreach (var validation in m_validations)
                    {
                        if (validation.CellRange.Contains(i, j)
                            && !validation.CellValidation.IsValid(cellText))
                        {
                            CommUtils.Assert(false, "文档检查错误，第{0}行第{1}列：{2}",
                                i + 1 + tableHeaderRowsCount, j + 1, validation.CellValidation.ErrorMsg);
                        }
                    }
                }

                if (CompareColumn != null)
                {
                    CommUtils.Assert(CompareColumn.CompareColumn(table, i), "文档检查错误, 第{0}行:{1}",
                        i + 1, CompareColumn.ErrorMsg);
                }

            }
        }

        private List<ExcelValidationItem> m_validations;

        public ColumnJudge JudgeColumn;

        public ColumnComparison CompareColumn;
    }
}
