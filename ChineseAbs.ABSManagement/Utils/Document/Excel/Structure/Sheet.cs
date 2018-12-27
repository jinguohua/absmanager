using NPOI.SS.UserModel;
using System.Data;

namespace ChineseAbs.ABSManagement.Utils.Excel.Structure
{
    public class Sheet
    {
        public Sheet(string sheetName, DataTable dataTable)
        {
            Name = sheetName;
            m_dataTable = dataTable;
            SheetStyle = new SheetStyle();
        }

        public ISheet Set(Workbook workbook)
        {
            ISheet sheet = string.IsNullOrWhiteSpace(Name) ? workbook.GetIWorkbook().CreateSheet() : workbook.GetIWorkbook().CreateSheet(Name);
            var style = workbook.StyleFactory.Create(CellStylePattern.DefaultBorder, CellStylePattern.DefaultFont);

            IRow headerRow = sheet.CreateRow(0);
            foreach (DataColumn column in m_dataTable.Columns)
            {
                var cell = headerRow.CreateCell(column.Ordinal);
                cell.SetCellValue(column.Caption);
                cell.CellStyle = style;
            }

            for (int iRow = 0; iRow < m_dataTable.Rows.Count; iRow++)
            {
                var row = m_dataTable.Rows[iRow];
                IRow dataRow = sheet.CreateRow(iRow + 1);

                for (int iColumn = 0; iColumn < m_dataTable.Columns.Count; iColumn++)
                {
                    var column = m_dataTable.Columns[iColumn];
                    var cell = dataRow.CreateCell(column.Ordinal);
                    cell.CellStyle = style;
                    var cellValue = row[column].ToString();

                    cell.SetCellValue(cellValue);
                    //TODO:
                    //检查cellRangeStyle中的cellRange是否重叠, 多样式合并
                    if (this.SheetStyle.CellRangeStyles != null)
                    {
                        foreach (var cellRangeStyle in this.SheetStyle.CellRangeStyles)
                        {
                            if (cellRangeStyle.Range.Contains(iRow, iColumn))
                            {
                                if ((!string.IsNullOrWhiteSpace(cellValue) && cellValue != "-") || cellRangeStyle.Rule == CellRangeRule.IgnoreEmptyCell)
                                {
                                    cell.CellStyle = cellRangeStyle.Style.Get(workbook, new Cell(cellValue));
                                }
                                break;
                            }
                        }
                    }
                }
            }
            //列宽自适应
            for (int i = 0; i < m_dataTable.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            return sheet;
        }

        public SheetStyle SheetStyle { get; set; }

        public string Name { get; set; }

        private DataTable m_dataTable;
    }
}
