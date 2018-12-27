using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using ChineseAbs.ABSManagement.ResourcePool;

namespace ChineseAbs.ABSManagement.Utils.Excel.Structure
{
    public class Workbook
    {
        public Workbook(string fileName, Sheet sheet)
        {
            FileName = fileName;
            Sheets = new List<Sheet>();
            Sheets.Add(sheet);
        }

        public Workbook(string fileName, List<Sheet> sheets)
        {
            FileName = fileName;
            Sheets = sheets;
        }

        private MemoryStream ToMemoryStream()
        {
            m_workbook = new XSSFWorkbook();
            var sheetCount = Sheets.Count;
            if (sheetCount == 0)
            {
                return new MemoryStream();
            }

            foreach (var sheet in Sheets)
			{
                sheet.Set(this);
			}
            
            //转为字节数组  
            MemoryStream ms = new MemoryStream();
            m_workbook.Write(ms);
            var buf = ms.ToArray();

            var temproaryFolder = CommUtils.CreateTemporaryFolder("Temporary");
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

        public Resource ToExcel(string userName)
        {
            return ResourcePool.ResourcePool.RegisterMemoryStream(userName, FileName + ".xlsx", this.ToMemoryStream());
        }

        public StyleFactory StyleFactory
        {
            get
            {
                if (m_styleFactory == null)
                {
                    m_styleFactory = new StyleFactory(this);
                }

                return m_styleFactory;
            }
        }

        private StyleFactory m_styleFactory = null;

        public IWorkbook GetIWorkbook()
        {
            return m_workbook;
        }

        private IWorkbook m_workbook;

        public string FileName { get; set; }

        public List<Sheet> Sheets { get; set; }
    }
}
