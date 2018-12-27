using ChineseAbs.ABSManagement.Utils;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class SimulationLogicModel
    {
        public SimulationLogicModel(string userName)
        {
            m_userName = userName;
        }

        public DataTable MergeTable(DataTable currTable, DataTable prevTable)
        {
            var newTable = new DataTable();
            if (currTable.Columns.Count >= prevTable.Columns.Count)
            {
                newTable = currTable.Copy();

                if (currTable.Rows.Count < prevTable.Rows.Count)
                {
                    var num = 0;
                    for (int row = 1; row < prevTable.Rows.Count; row++)
                    {
                        if (row - num >= prevTable.Rows.Count)
                        {
                            break;
                        }
                        if (currTable.Rows[row - num][0].ToString() != prevTable.Rows[row][0].ToString()
                            || currTable.Rows[row - num][1].ToString() != prevTable.Rows[row][1].ToString())
                        {
                            var newRow = newTable.NewRow();
                            for (int i = 0; i < prevTable.Rows[row].ItemArray.Length; i++)
                            {
                                newRow[i] = prevTable.Rows[row][i];
                            }
                            newTable.Rows.InsertAt(newRow, row);
                            num++;
                        }
                    }
                }
            }
            else
            {
                newTable = prevTable.Copy();
                var num = 0;

                if (currTable.Rows.Count > prevTable.Rows.Count)
                {
                    for (int row = 1; row < currTable.Rows.Count; row++)
                    {
                        if (row - num >= prevTable.Rows.Count)
                        {
                            break;
                        }
                        if (currTable.Rows[row][0].ToString() != prevTable.Rows[row - num][0].ToString()
                            || currTable.Rows[row][1].ToString() != prevTable.Rows[row - num][1].ToString())
                        {
                            var newRow = newTable.NewRow();
                            for (int i = 0; i < currTable.Rows[row].ItemArray.Length; i++)
                            {
                                newRow[i] = currTable.Rows[row][i];
                            }
                            newTable.Rows.InsertAt(newRow, row);
                            num++;
                        }
                    }
                }

                num = 0;
                for (int rowIndex = 0; rowIndex < newTable.Rows.Count; rowIndex++)
                {
                    if (rowIndex - num >= currTable.Rows.Count)
                    {
                        break;
                    }
                    if (newTable.Rows[rowIndex][0].ToString() == currTable.Rows[rowIndex - num][0].ToString()
                        && newTable.Rows[rowIndex][1].ToString() == currTable.Rows[rowIndex - num][1].ToString())
                    {
                        for (int i = 0; i < newTable.Columns.Count; i++)
                        {
                            for (int j = 0; j < currTable.Columns.Count; j++)
                            {
                                if (newTable.Columns[i].ColumnName == newTable.Columns[j].ColumnName)
                                {
                                    newTable.Rows[rowIndex][i] = currTable.Rows[rowIndex - num][j];
                                }
                            }
                        }
                    }
                    else
                    {
                        num++;
                    }
                }
            }

            return newTable;
        }

        public MemoryStream GenerateCompareResultExcel(DataTable newTable, DataTable currTable, DataTable prevTable)
        {
            IWorkbook workbook = new XSSFWorkbook();

            ISheet sheet = workbook.CreateSheet("对比结果");
            ExcelUtils.AddSheetFromTableToExcel(workbook, prevTable, "上次测算结果", true);
            ExcelUtils.AddSheetFromTableToExcel(workbook, currTable, "本次测算结果", true);

            ICellStyle style = ExcelUtils.CreateBorderedStyle(workbook);

            //文字颜色的样式（四种：红色，绿色，灰色，蓝色）
            ICellStyle fontRedStyle = ExcelUtils.GetFontColorStyle(workbook, ExcelFontColor.Red);
            ICellStyle fontGreenStyle = ExcelUtils.GetFontColorStyle(workbook, ExcelFontColor.Green);
            ICellStyle fontGrayStyle = ExcelUtils.GetFontColorStyle(workbook, ExcelFontColor.Gray);
            ICellStyle fontBlueStyle = ExcelUtils.GetFontColorStyle(workbook, ExcelFontColor.Blue);

            //创建一个画板，用来填加批注。
            IDrawing patr = sheet.CreateDrawingPatriarch();

            // handling header.  
            IRow headerRow = sheet.CreateRow(0);
            foreach (DataColumn column in newTable.Columns)
            {
                var cell = headerRow.CreateCell(column.Ordinal);
                cell.SetCellValue(column.Caption);
                cell.CellStyle = style;
                
                if (!currTable.Columns.Contains(column.ColumnName)
                    && prevTable.Columns.Contains(column.ColumnName))
                {
                    cell.SetCellValue("- " + column.Caption);
                    cell.CellStyle = fontGrayStyle;
                }

                if (currTable.Columns.Contains(column.ColumnName)
                    && !prevTable.Columns.Contains(column.ColumnName))
                {
                    cell.SetCellValue("+ " + column.Caption);
                    cell.CellStyle = fontBlueStyle;
                }
                ExcelUtils.ConvertCellTypeDataFormat(cell);
            }

            // handling value.  
            int rowIndex = 1;
            var num = 0;
            for (int i = 0; i < newTable.Rows.Count; i++)
            {
                DataRow row = newTable.Rows[i];
                IRow dataRow = sheet.CreateRow(rowIndex);

                var currTableHas = currTable.AsEnumerable().Where(x =>
                    x[0].ToString() == row[0].ToString()
                    && x[1].ToString() == row[1].ToString()).Count() > 0;
                var prevTableHas = prevTable.AsEnumerable().Where(x =>
                    x[0].ToString() == row[0].ToString()
                    && x[1].ToString() == row[1].ToString()).Count() > 0;
                if (currTableHas && prevTableHas)
                {
                    for (int j = 0; j < newTable.Columns.Count; j++)
                    {
                        DataColumn column = newTable.Columns[j];

                        var cell = dataRow.CreateCell(column.Ordinal);
                        cell.SetCellValue(row[column].ToString());
                        cell.CellStyle = style;

                        //减少的列字体设置为灰色
                        if (!currTable.Columns.Contains(column.ColumnName)
                        && prevTable.Columns.Contains(column.ColumnName))
                        {
                            cell.CellStyle = fontGrayStyle;
                        }

                        //增加的列字体设置为蓝色
                        if (currTable.Columns.Contains(column.ColumnName)
                            && !prevTable.Columns.Contains(column.ColumnName))
                        {
                            cell.CellStyle = fontBlueStyle;
                        }

                        //根据判断数值的变化加上字体颜色及批注
                        if (currTable.Columns.Contains(column.ColumnName)
                            && prevTable.Columns.Contains(column.ColumnName))
                        {
                            if (row[0].ToString() == prevTable.Rows[i - num][0].ToString()
                                && row[1].ToString() == prevTable.Rows[i - num][1].ToString())
                            {
                                Double prevValue = 0.0;
                                Double currValue = 0.0;

                                if (Double.TryParse(prevTable.Rows[i - num][j].ToString(), out prevValue)
                                    && Double.TryParse(row[j].ToString(), out currValue))
                                {
                                    if (prevValue > currValue)
                                    {
                                        cell.CellStyle = fontGreenStyle;
                                        cell.CellComment = AddCompareResultComment(patr, cell, dataRow, prevValue.ToString("n2"), (prevValue - currValue).ToString("n2"), false);
                                    }
                                    if (prevValue < currValue)
                                    {
                                        cell.CellStyle = fontRedStyle;
                                        cell.CellComment = AddCompareResultComment(patr, cell, dataRow, prevValue.ToString("n2"), (currValue - prevValue).ToString("n2"), true);
                                    }
                                }
                            }
                        }
                        ExcelUtils.ConvertCellTypeDataFormat(cell);
                    }
                }
                else
                {
                    //对增加或减少的行增加样式
                    var currTableCount = currTable.Columns.Count;
                    var prevTableCount = prevTable.Columns.Count;
                    if (currTableCount > prevTableCount)
                    {
                        currTableCount = prevTableCount;
                    }

                    for (int columnIndex = 0; columnIndex < currTableCount; columnIndex++)
                    {
                        DataColumn column = newTable.Columns[columnIndex];
                        var cell = dataRow.CreateCell(column.Ordinal);
                        var stringValue = row[column].ToString();

                        if (currTableHas && !prevTableHas)
                        {
                            if (columnIndex == 0 || columnIndex == 1)
                            {
                                stringValue = "+ " + stringValue;
                            }
                            cell.SetCellValue(stringValue);
                            cell.CellStyle = fontBlueStyle;
                        }
                        if (!currTableHas && prevTableHas)
                        {
                            if (columnIndex == 0 || columnIndex == 1)
                            {
                                stringValue = "- " + stringValue;
                            }
                            cell.SetCellValue(stringValue);
                            cell.CellStyle = fontGrayStyle;
                        }
                        ExcelUtils.ConvertCellTypeDataFormat(cell);
                    }
                    if (currTableHas && !prevTableHas)
                    {
                        num++;
                    }
                }

                rowIndex++;
            }

            //列宽自适应
            for (int i = 0; i < newTable.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            //转为字节数组  
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            var buf = ms.ToArray();

            var temproaryFolder = CommUtils.CreateTemporaryFolder(m_userName);
            var filePath = Path.Combine(temproaryFolder, "Temporary.xlsx");

            //保存为Excel文件  
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }

            var buffer = System.IO.File.ReadAllBytes(filePath);
            CommUtils.DeleteFolderAync(temproaryFolder);

            return new MemoryStream(buffer);
        }

        private IComment AddCompareResultComment(IDrawing patr, ICell cell, IRow headerRow, string prevValue, string changeValue, bool isAdd = false)
        {
            var text = "上次测算：\n" + prevValue + "\n";
            text += isAdd ? "增加：\n" : "减少：\n";
            text += changeValue;

            var comment = ExcelUtils.AddComment(patr, cell, headerRow, text);
            return comment;
        }

        public string UserName { get { return m_userName; } }

        private string m_userName;
    }
}
