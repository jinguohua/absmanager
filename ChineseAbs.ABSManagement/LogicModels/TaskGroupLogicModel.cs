using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class TaskGroupLogicModel : BaseLogicModel
    {
        public TaskGroupLogicModel(ProjectLogicModel project, TaskGroup taskGroup)
            : base(project)
        {
            m_instance = taskGroup;
        }

        public MemoryStream GenerateCompareResultExcel(DataTable newTable, int taskStatusIndex)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("TaskGroupTable");
            ICellStyle style = ExcelUtils.CreateBorderedStyle(workbook);

            //文字颜色的样式（四种：红色，绿色，灰色，蓝色）
            ICellStyle fontRedStyle = ExcelUtils.GetFontColorStyle(workbook, ExcelFontColor.Red);
            ICellStyle fontGreenStyle = ExcelUtils.GetFontColorStyle(workbook, ExcelFontColor.Green);
            ICellStyle fontBlueStyle = ExcelUtils.GetFontColorStyle(workbook, ExcelFontColor.Blue);

            IRow headerRow = sheet.CreateRow(0);
            foreach (DataColumn column in newTable.Columns)
            {
                var cell = headerRow.CreateCell(column.Ordinal);
                cell.SetCellValue(column.Caption);
                cell.CellStyle = style;
            }

            for (int iRow = 0; iRow < newTable.Rows.Count; iRow++)
            {
                var row = newTable.Rows[iRow];
                IRow dataRow = sheet.CreateRow(iRow + 1);

                for (int iColumn = 0; iColumn < newTable.Columns.Count; iColumn++)
                {
                    var column = newTable.Columns[iColumn];
                    var cell = dataRow.CreateCell(column.Ordinal);
                    cell.CellStyle = style;
                    var cellValue = row[column].ToString();
                    if (iColumn != taskStatusIndex || cellValue == string.Empty || cellValue == "-")
                    {
                        cell.SetCellValue(cellValue);
                    }
                    else 
                    {
                        cell.SetCellValue(TranslateUtils.ToCnString(CommUtils.ParseEnum<TaskStatus>(cellValue)));
                        if (cellValue == "Finished" || cellValue == "Error")
                        {
                            cell.CellStyle = fontGreenStyle;
                        }
                        else if (cellValue == "Running" || cellValue == "Waitting")
                        {
                            cell.CellStyle = fontBlueStyle;
                        }
                        else if (cellValue == "Overdue")
                        {
                            cell.CellStyle = fontRedStyle;
                        }
                    }
                }
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

        public TaskGroup Instance
        {
            get
            {
                return m_instance;
            }
        }

        private TaskGroup m_instance;

        public List<Task> Tasks
        {
            get
            {
                if (m_tasks == null)
                {
                    m_tasks = m_dbAdapter.Task.GetByTaskGroupId(m_instance.Id, true);
                }

                return m_tasks;
            }
        }

        private List<Task> m_tasks;
    }
}
