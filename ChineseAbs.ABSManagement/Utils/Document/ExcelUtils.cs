using System;
using System.Collections.Generic;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.HSSF.Util;

namespace ChineseAbs.ABSManagement.Utils
{
    public enum ExcelFontColor
    {
        Red = 1,
        Blue = 2,
        Green = 3,
        Gray = 4
    } 
    public class ExcelUtils
    {
        public static void WriteCsv(DataTable dt,string filePath)
        {
            SFL.NET.ZDataTable.DataTable2CSV(dt, filePath);
        }

        public static DataTable ReadCsv(string filePath)
        {
            return SFL.NET.ZDataTable.CSV2DataTable(filePath);
        }

        private static object GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return null;
            }

            //经测试 wps转换为日期格式后，DataFormat值为58，数据类型为普通double，需要进行转换
            if (cell.CellStyle != null && cell.CellStyle.DataFormat == 58)
            {
                return cell.DateCellValue.ToShortDateString();
            }
            else
            {
                return cell.ToString();
            }
        }

        public static void TableToExcel(DataTable table, string filePath, bool isConvertDataFormat = false)
        {
            IWorkbook workbook;
            var fileExt = Path.GetExtension(filePath).ToLower();
            var sheetName = Path.GetFileNameWithoutExtension(filePath);

            if (fileExt == ".xlsx") 
            {
                workbook = new XSSFWorkbook();
            }
            else if (fileExt == ".xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                throw new ApplicationException("文件的扩展名必须为.xlsx或.xls");
            }

            AddSheetFromTableToExcel(workbook, table, sheetName, isConvertDataFormat);

            //转为字节数组  
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            var buf = ms.ToArray();

            //保存为Excel文件  
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        public static int GetTableHeaderRowsCount(Stream stream, int iSheet, int columnBegin, List<List<string>> tableHeader)
        {
            var tableHeaderRowsCount = 0;
            var noHeader = 0;
            IWorkbook book = new XSSFWorkbook(stream);
            if (iSheet >= book.NumberOfSheets || iSheet < 0)
            {
                return noHeader;
            }

            ISheet sheet = book.GetSheetAt(iSheet);
            for (int iRow = 0; iRow < tableHeader.Count; ++iRow)
            {
                var line = tableHeader[iRow];
                IRow row = sheet.GetRow(iRow);
                if (row == null)
                {
                    return noHeader;
                }

                for (int iCell = columnBegin; iCell < line.Count; ++iCell)
                {
                    ICell cell = row.Cells[iCell];
                    if (cell.ToString() != tableHeader[iRow][iCell])
                    {
                        return noHeader;
                    }
                }
                tableHeaderRowsCount++;

            }
            return tableHeaderRowsCount;
        
        }

        public static List<List<object>> ParseExcel(Stream stream, int iSheet, int rowBegin, int columnBegin, int rowWidth)
        {
            IWorkbook book = new XSSFWorkbook(stream);
            if (iSheet >= book.NumberOfSheets || iSheet < 0)
            {
                return null;
            }

            var table = new List<List<object>>();
            ISheet sheet = book.GetSheetAt(iSheet);
            for (int iRow = rowBegin; ; ++iRow)
            {
                var line = new List<object>();
                IRow row = sheet.GetRow(iRow);
                if (row == null)
                {
                    break;
                }

                var nullCellNum = 0;

                for (int iCell = columnBegin; iCell < rowWidth; ++iCell)
                {
                    ICell cell = row.GetCell(iCell);
                    
                    if (cell == null || string.IsNullOrWhiteSpace(GetCellValue(cell).ToString()))
                    {
                        nullCellNum++;
                    }

                    line.Add(GetCellValue(cell));
                }

                if (nullCellNum == rowWidth)
                {
                    break;
                }

                table.Add(line);
            }
            return table;
        }

        public static MemoryStream ToExcelMemoryStream(DataTable table, string fileName, string userName)
        {
            var tempFolder = CommUtils.CreateTemporaryFolder(userName);
            var tempFilePath = Path.Combine(tempFolder, fileName);
            ExcelUtils.TableToExcel(table, tempFilePath);

            var buffer = System.IO.File.ReadAllBytes(tempFilePath);
            CommUtils.DeleteFolderAync(tempFolder);

            return new MemoryStream(buffer);
        }

        public static IEnumerable<T> ParseTable<T>(List<List<object>> table, int projectId,
           Func<List<object>, int, int, T> parseRow)
        {
            var rows = new List<T>();
            for (int i = 0; i < table.Count; i++)
            {
                try
                {
                    rows.Add((T)parseRow(table[i], i, projectId));
                }
                catch (Exception e)
                {
                    CommUtils.Assert(false, "文件解析错误（行:" + (i + 1).ToString() + "）:" + e.Message);
                }
            }

            return rows;
        }

        public static void AddSheetFromTableToExcel(IWorkbook workbook, DataTable table, string sheetName, bool isConvertDataFormat = false)
        {
            ISheet sheet = string.IsNullOrWhiteSpace(sheetName) ? workbook.CreateSheet() : workbook.CreateSheet(sheetName);
            IRow headerRow = sheet.CreateRow(0);

            ICellStyle style = CreateBorderedStyle(workbook);

            // handling header.  
            foreach (DataColumn column in table.Columns)
            {
                var cell = headerRow.CreateCell(column.Ordinal);
                cell.SetCellValue(column.Caption);
                cell.CellStyle = style;
            }
            // handling value.  
            int rowIndex = 1;

            foreach (DataRow row in table.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in table.Columns)
                {
                    var cell = dataRow.CreateCell(column.Ordinal);
                    cell.SetCellValue(row[column].ToString());
                    cell.CellStyle = style;
                    if (isConvertDataFormat)
                    {
                        ConvertCellTypeDataFormat(cell);
                    }
                }

                rowIndex++;
            }

            //列宽自适应
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        /// <summary>
        /// 把单元格中能够转换成double类型数值转换成#,##0.00格式
        /// </summary>
        /// <param name="cell"></param>
        public static void ConvertCellTypeDataFormat(ICell cell)
        {
            if (cell.CellType == CellType.String)
            {
                double value = 0.0;
                if (double.TryParse(cell.StringCellValue, out value))
                {
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(value);
                    cell.CellStyle.DataFormat = 4;
                }
            }
        }

        public static ICellStyle GetFontColorStyle(IWorkbook workbook, ExcelFontColor colorType)
        {
            var hwb = new HSSFWorkbook();
            HSSFPalette palette = hwb.GetCustomPalette();
            HSSFColor myColor = new HSSFColor();

            switch (colorType)
            {
                case ExcelFontColor.Red:
                    myColor = palette.FindSimilarColor(250, 0, 0);//#FF0000
                    break;
                case ExcelFontColor.Blue:
                    myColor = palette.FindSimilarColor(1, 75, 162);//#014BA2
                    break;
                case ExcelFontColor.Green:
                    myColor = palette.FindSimilarColor(0, 100, 33);//#006421
                    break;
                case ExcelFontColor.Gray:
                    myColor = palette.FindSimilarColor(164, 164, 164);//#A4A4A4
                    break;
                default:
                    break;
            }

            short palIndex = myColor.Indexed;
            IFont fontStyle = workbook.CreateFont();

            fontStyle.FontName = "微软雅黑";
            fontStyle.Color = palIndex;
            fontStyle.FontHeightInPoints = 10;

            var cellStyle = CreateBorderedStyle(workbook);
            cellStyle.SetFont(fontStyle);

            return cellStyle;
        }

        /// <summary>
        /// 设置单元格边框的样式（默认为黑色）
        /// </summary>
        public static ICellStyle CreateBorderedStyle(IWorkbook wb)
        {
            ICellStyle style = wb.CreateCellStyle();
            style.BorderRight = BorderStyle.Thin;
            style.RightBorderColor = (IndexedColors.Black.Index);
            style.BorderBottom = BorderStyle.Thin;
            style.BottomBorderColor = (IndexedColors.Black.Index);
            style.BorderLeft = BorderStyle.Thin;
            style.LeftBorderColor = (IndexedColors.Black.Index);
            style.BorderTop = BorderStyle.Thin;
            style.TopBorderColor = (IndexedColors.Black.Index);

            IFont fontStyle = wb.CreateFont();
            fontStyle.FontName = "微软雅黑";
            fontStyle.FontHeightInPoints = 10;
            style.SetFont(fontStyle);
            return style;
        }

        public static IComment AddComment(IDrawing patr, ICell cell, IRow headerRow, string value)
        {
            var anchor = new XSSFClientAnchor();
            anchor.Col1 = cell.ColumnIndex + 1;
            anchor.Col2 = cell.ColumnIndex + 3;
            anchor.Row1 = headerRow.RowNum + 1;
            anchor.Row2 = headerRow.RowNum + 5;

            IComment comment = patr.CreateCellComment(anchor);
            comment.String = (new XSSFRichTextString(value));

            return comment;
        }
    }
}
