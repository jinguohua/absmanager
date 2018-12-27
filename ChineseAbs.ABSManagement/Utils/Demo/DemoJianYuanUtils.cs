using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;

using PatternTableRow = System.Collections.Generic.Dictionary<string, string>;
using PatternTable = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>;
using System.IO;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Data;
using ChineseAbs.ABSManagement.Pattern.Demo;
using FilePattern;
using ChineseAbs.CalcService.Data.NancyData;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement.Utils.Demo
{
    public class DemoJianYuanUtils
    {
        public static string GetModelFolder()
        {
            var rootFolder = WebConfigUtils.RootFolder;
            var folder = rootFolder + "Temporay" + "\\DemoJianYuanReport\\CustomModel";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        public static string GetExcelReportPath()
        {
            var rootFolder = WebConfigUtils.RootFolder;
            var folder = rootFolder + "Temporay" + "\\DemoJianYuanReport\\CustomExcelReportFile\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return Path.Combine(folder, "ExcelReport.xls");
        }

        public static string GetKeyValuePath()
        {
            var rootFolder = WebConfigUtils.RootFolder;
            var folder = rootFolder + "Temporay" + "\\DemoJianYuanReport\\CustomKeyValueFile\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return Path.Combine(folder, "KeyValue.txt");
        }

        public static string GetTemplateFilePath()
        {
            var rootFolder = WebConfigUtils.RootFolder;
            var folder = rootFolder + "Temporay" + "\\DemoJianYuanReport\\CustomTemplateFile\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return Path.Combine(folder, DocumentPattern.GetFileName(DocPatternType.DemoJianYuanReport));
        }

        public void Generate(MemoryStream ms, Stream excelStream, string excelFileName, DateTime paymentDate, string asOfDate)
        {
            m_excelStream = excelStream;
            m_excelFileName = excelFileName;
            m_asOfDate = asOfDate;
            m_paymentDate = paymentDate;

            //制作生成模板文件需要的实例
            var obj = GetObjInstance();

            var patternFilePath = GetTemplateFilePath();

            //配置文件生成参数
            var setting = new Setting();
            setting.PatternTextStyle.SignWithColor = true;
            setting.PatternTextStyle.ForecolorName = System.Drawing.Color.Red.Name;

            //文件生成
            var wordPattern = new WordPattern(setting);
            if (!wordPattern.Generate(patternFilePath, obj, ms))
            {
                throw new ApplicationException("Generate file failed.");
            }
        }

        private Stream m_excelStream;
        private string m_excelFileName;
        private DateTime m_paymentDate;
        private string m_asOfDate;



        private IWorkbook LoadExcelBook(string fileName, Stream stream)
        {
            IWorkbook book = null;
            if (fileName.EndsWith(".xls"))
            {
                book = new HSSFWorkbook(stream);
            }
            else if (fileName.EndsWith(".xlsx"))
            {
                book = new XSSFWorkbook(stream);
            }
            return book;
        }

        List<int> m_tableRowIndexes = new List<int>();

        class CellKeyword
        {
            public CellKeyword(string title, string row, string column)
            {
                TitleName = title;
                RowName = new[] { row };
                ColumnName = new[] { column };
            }

            public CellKeyword(string title, string[] row, string[] column)
            {
                TitleName = title;
                RowName = row;
                ColumnName = column;
            }

            public CellKeyword(string title, string row, string[] column)
            {
                TitleName = title;
                RowName = new[] { row };
                ColumnName = column;
            }

            public string TitleName { get; set; }

            public string[] RowName { get; set; }

            public string[] ColumnName { get; set; }
        }

        private ISheet FindSheetByTitle(IWorkbook book, int titleRow, int titleColumn, string titleName)
        {
            for (int i = 0; i < book.NumberOfSheets; ++i)
            {
                ISheet sheet = book.GetSheetAt(i);

                var rowIndex = 0;
                var rowEnumerator = sheet.GetRowEnumerator();
                while (rowEnumerator.MoveNext())
                {
                    IRow row = rowEnumerator.Current as IRow;
                    if (row != null)
                    {
                        var cellIndex = 0;
                        var cellEnumerator = row.GetEnumerator();
                        while (cellEnumerator.MoveNext())
                        {
                            ICell cell = cellEnumerator.Current as ICell;
                            if (cell != null)
                            {
                                if (rowIndex == titleRow && cellIndex == titleColumn)
                                {
                                    var cellString = cell.ToString();
                                    if (string.IsNullOrWhiteSpace(cellString))
                                    {
                                        continue;
                                    }

                                    cellString = cellString.Trim();
                                    if (cellString == titleName)
                                    {
                                        return sheet;
                                    }
                                }
                            }

                            ++cellIndex;
                        }
                    }

                    ++rowIndex;
                }
            }

            return null;
        }

        private Tuple<int, int> FindCell(ISheet sheet, int rowBeginIndex, int columnBeginIndex, string value)
        {
            var rowIndex = 0;
            var rowEnumerator = sheet.GetRowEnumerator();
            while (rowEnumerator.MoveNext())
            {
                if (rowIndex >= rowBeginIndex)
                {
                    IRow row = rowEnumerator.Current as IRow;
                    if (row != null)
                    {
                        var cellIndex = 0;
                        var cellEnumerator = row.GetEnumerator();
                        while (cellEnumerator.MoveNext())
                        {
                            if (cellIndex >= columnBeginIndex)
                            {
                                ICell cell = cellEnumerator.Current as ICell;
                                if (cell != null)
                                {
                                    var cellString = cell.ToString();
                                    if (value.Trim().Replace("\r", "").Replace("\n", "")
                                        == cellString.Trim().Replace("\r", "").Replace("\n", ""))
                                    {
                                        return new Tuple<int, int>(rowIndex, cellIndex);
                                    }
                                }
                            }

                            ++cellIndex;
                        }
                    }
                }

                ++rowIndex;
            }

            return null;
        }

        private Tuple<int, int> FindCell(ISheet sheet, string[] valueScope)
        {
            var rowBeginIndex = 0;
            var columnBeginIndex = 0;

            Tuple<int, int> cellPos = null;
            foreach (var value in valueScope)
            {
                cellPos = FindCell(sheet, rowBeginIndex, columnBeginIndex, value);
                if (cellPos == null)
                {
                    return null;
                }

                rowBeginIndex = cellPos.Item1;
                columnBeginIndex = cellPos.Item2;
            }

            return cellPos;
        }

        private Tuple<int, int> FindCell(ISheet sheet, string value)
        {
            var rowIndex = 0;
            var rowEnumerator = sheet.GetRowEnumerator();
            while (rowEnumerator.MoveNext())
            {
                IRow row = rowEnumerator.Current as IRow;
                if (row != null)
                {
                    var cellIndex = 0;
                    var cellEnumerator = row.GetEnumerator();
                    while (cellEnumerator.MoveNext())
                    {
                        ICell cell = cellEnumerator.Current as ICell;
                        if (cell != null)
                        {
                            var cellString = cell.ToString();
                            if (value.Trim().Replace("\r", "").Replace("\n", "")
                                == cellString.Trim().Replace("\r", "").Replace("\n", ""))
                            {
                                return new Tuple<int, int>(rowIndex, cellIndex);
                            }
                        }
                        ++cellIndex;
                    }
                }
                ++rowIndex;
            }

            return null;
        }

        private string GetCellValue(ISheet sheet, int findRowIndex, int findColumnIndex)
        {
            var rowIndex = 0;
            var rowEnumerator = sheet.GetRowEnumerator();
            while (rowEnumerator.MoveNext())
            {
                IRow row = rowEnumerator.Current as IRow;
                if (row != null)
                {
                    var cellIndex = 0;
                    var cellEnumerator = row.GetEnumerator();
                    while (cellEnumerator.MoveNext())
                    {
                        ICell cell = cellEnumerator.Current as ICell;
                        if (cell != null)
                        {
                            if (rowIndex == findRowIndex && cellIndex == findColumnIndex)
                            {
                                if (cell.CellType == NPOI.SS.UserModel.CellType.Formula)
                                {
                                    if (cell.CachedFormulaResultType == CellType.Numeric)
                                    {
                                        return cell.NumericCellValue.ToString(cell.CellStyle.GetDataFormatString());
                                    }
                                    else
                                    {
                                        CommUtils.Assert(false, "cell.CachedFormulaResultType != CellType.Numeric");
                                    }
                                }

                                return cell.ToString();
                            }
                        }
                        ++cellIndex;
                    }
                }
                ++rowIndex;
            }

            return string.Empty;
        }

        private string GetCellValue(IWorkbook book, CellKeyword keyword)
        {
            var sheet = FindSheetByTitle(book, 2, 0, keyword.TitleName);
            if (sheet == null)
            {
                CommUtils.AssertNotNull(sheet, "找不到标题[{0}]", keyword.TitleName);
            }

            Tuple<int, int> findByRow = null;
            if (!keyword.RowName.Any(string.IsNullOrWhiteSpace))
            {
                findByRow = FindCell(sheet, keyword.RowName);
                CommUtils.AssertNotNull(findByRow, "找不到关键字[{0}]", keyword.RowName);
            }

            Tuple<int, int> findByColumn = null;
            if (!keyword.ColumnName.Any(string.IsNullOrWhiteSpace))
            {
                findByColumn = FindCell(sheet, keyword.ColumnName);
                CommUtils.AssertNotNull(findByColumn, "找不到关键字[{0}]", keyword.ColumnName);
            }

            CommUtils.Assert(!(findByRow == null && findByColumn == null), "行列关键字不能同时为空");

            string cellValue = string.Empty;
            if (findByRow != null && findByColumn != null)
            {
                cellValue = GetCellValue(sheet, findByRow.Item1, findByColumn.Item2);
            }
            else if (findByRow != null && findByColumn == null)
            {
                cellValue = GetCellValue(sheet, findByRow.Item1, findByRow.Item2 + 1);
            }
            else if (findByRow == null && findByColumn != null)
            {
                cellValue = GetCellValue(sheet, findByColumn.Item1 + 1, findByColumn.Item2);
            }

            return cellValue;
        }

        private Dictionary<CellKeyword, string> GetCellValues(IWorkbook book, List<CellKeyword> keywords)
        {
            var dict = new Dictionary<CellKeyword, string>();
            foreach (var keyword in keywords)
            {
                dict[keyword] = GetCellValue(book, keyword);
            }

            return dict;
        }

        private void GetPatternTable(IWorkbook book, PatternTable table, string title, string key1, string key2)
        {
            GetPatternTable(book, table, title, key1, new string[] { key2 });
        }

        private void GetPatternTable(IWorkbook book, PatternTable table, string title, string key1, string[] key2)
        {
            var sheet = FindSheetByTitle(book, 2, 0, title);
            var pos1 = FindCell(sheet, key1);
            var pos2 = FindCell(sheet, key2);

            var rowBegin = Math.Min(pos1.Item1, pos2.Item1);
            var rowEnd = Math.Max(pos1.Item1, pos2.Item1);
            var columnBegin = Math.Min(pos1.Item2, pos2.Item2);
            var columnEnd = Math.Max(pos1.Item2, pos2.Item2);

            for (int i = rowBegin + 1; i <= rowEnd; i++)
            {
                var row = new PatternTableRow();
                for (int j = columnBegin; j <= columnEnd; j++)
                {
                    var cellValue = GetCellValue(sheet, i, j);
                    row["c" + j] = cellValue;
                }

                table.Add(row);
            }
        }

        protected object GetObjInstance()
        {
            var book = LoadExcelBook(m_excelFileName, m_excelStream);

            List<CellKeyword> keywordList = new List<CellKeyword>();

            var collateralInterestCollectionKey = new CellKeyword("收款信息", "收入回收款", "");
            keywordList.Add(collateralInterestCollectionKey);
            var collateralPrincipalCollectionKey = new CellKeyword("收款信息", "本金回收款", "");
            keywordList.Add(collateralPrincipalCollectionKey);

            var collateralLossAmountKey = new CellKeyword("违约及严重拖欠信息", "新增违约抵押贷款", "金额");
            keywordList.Add(collateralLossAmountKey);

            #region Word文档中表格里的字段抓取

            var 正常贷款金额 = new CellKeyword("本收款期间资产池贷款状态特征", "正常", "贷款余额");
            keywordList.Add(正常贷款金额);

            var 正常贷款金额占比 = new CellKeyword("本收款期间资产池贷款状态特征", "正常", "占期末资产池抵押贷款余额百分比");
            keywordList.Add(正常贷款金额占比);

            var 拖欠31至60天以上贷款金额 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠31至60天", "贷款余额");
            keywordList.Add(拖欠31至60天以上贷款金额);

            var 拖欠31至60天以上贷款金额占比 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠31至60天", "占期末资产池抵押贷款余额百分比");
            keywordList.Add(拖欠31至60天以上贷款金额占比);

            var 拖欠61至90天以上贷款金额 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠61至90天", "贷款余额");
            keywordList.Add(拖欠61至90天以上贷款金额);

            var 拖欠61至90天以上贷款金额占比 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠61至90天", "占期末资产池抵押贷款余额百分比");
            keywordList.Add(拖欠61至90天以上贷款金额占比);

            var 拖欠91至120天以上贷款金额 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠91至120天", "贷款余额");
            keywordList.Add(拖欠91至120天以上贷款金额);

            var 拖欠91至120天以上贷款金额占比 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠91至120天", "占期末资产池抵押贷款余额百分比");
            keywordList.Add(拖欠91至120天以上贷款金额占比);

            var 拖欠121至150天以上贷款金额 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠121至150天", "贷款余额");
            keywordList.Add(拖欠121至150天以上贷款金额);

            var 拖欠121至150天以上贷款金额占比 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠121至150天", "占期末资产池抵押贷款余额百分比");
            keywordList.Add(拖欠121至150天以上贷款金额占比);

            var 拖欠151至180天以上贷款金额 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠151至180天", "贷款余额");
            keywordList.Add(拖欠151至180天以上贷款金额);

            var 拖欠151至180天以上贷款金额占比 = new CellKeyword("本收款期间资产池贷款状态特征", "拖欠151至180天", "占期末资产池抵押贷款余额百分比");
            keywordList.Add(拖欠151至180天以上贷款金额占比);

            var 违约抵押贷款_未被注销金额 = new CellKeyword("本收款期间资产池贷款状态特征", "违约抵押贷款（未被注销）", "贷款余额");
            keywordList.Add(违约抵押贷款_未被注销金额);

            var 违约抵押贷款_未被注销占比 = new CellKeyword("本收款期间资产池贷款状态特征", "违约抵押贷款（未被注销）", "占期末资产池抵押贷款余额百分比");
            keywordList.Add(违约抵押贷款_未被注销占比);

            var 贷款余额_汇总 = new CellKeyword("本收款期间资产池贷款状态特征", "汇总", "贷款余额");
            keywordList.Add(贷款余额_汇总);


            var 期初本金总余额 = new CellKeyword("资产池存续期总体信息", "入池总金额", "上次报告期");
            keywordList.Add(期初本金总余额);

            var 期末本金总余额 = new CellKeyword("资产池存续期总体信息", "入池总金额", "本次报告期");
            keywordList.Add(期末本金总余额);

            var 本期应收本金 = new CellKeyword("资产池存续期总体信息", "", "应收本金");
            keywordList.Add(本期应收本金);

            var 本期应收利息 = new CellKeyword("资产池存续期总体信息", "", "应收利息");
            keywordList.Add(本期应收利息);

            var 收入账_利息_正常回收_上次报告期 = new CellKeyword("收款信息", "计划内还款", new[] { "上一收款期", "利息" });
            keywordList.Add(收入账_利息_正常回收_上次报告期);

            var 收入账_利息_正常回收_本次报告期 = new CellKeyword("收款信息", "计划内还款", new[] { "本收款期", "利息" });
            keywordList.Add(收入账_利息_正常回收_本次报告期);

            var 收入账_利息_提前偿还_上次报告期 = new CellKeyword("收款信息", "提前还款", new[] { "上一收款期", "利息" });
            keywordList.Add(收入账_利息_提前偿还_上次报告期);

            var 收入账_利息_提前偿还_本次报告期 = new CellKeyword("收款信息", "提前还款", new[] { "本收款期", "利息" });
            keywordList.Add(收入账_利息_提前偿还_本次报告期);

            var 收入账_利息_拖欠回收_上次报告期 = new CellKeyword("收款信息", "拖欠回收", new[] { "上一收款期", "利息" });
            keywordList.Add(收入账_利息_拖欠回收_上次报告期);

            var 收入账_利息_拖欠回收_本次报告期 = new CellKeyword("收款信息", "拖欠回收", new[] { "本收款期", "利息" });
            keywordList.Add(收入账_利息_拖欠回收_本次报告期);

            var 收入账_利息_违约回收_上次报告期 = new CellKeyword("收款信息", "违约回收", new[] { "上一收款期", "利息" });
            keywordList.Add(收入账_利息_违约回收_上次报告期);

            var 收入账_利息_违约回收_本次报告期 = new CellKeyword("收款信息", "违约回收", new[] { "本收款期", "利息" });
            keywordList.Add(收入账_利息_违约回收_本次报告期);

            var 收入账_利息_合计_上次报告期 = new CellKeyword("收款信息", "合计", new[] { "上一收款期", "利息" });
            keywordList.Add(收入账_利息_合计_上次报告期);

            var 收入账_利息_合计_本次报告期 = new CellKeyword("收款信息", "合计", "利息");
            keywordList.Add(收入账_利息_合计_本次报告期);

            var 收入账_合计_上次报告期 = new CellKeyword("收款信息", "收入回收款", "上一收款期");
            keywordList.Add(收入账_合计_上次报告期);

            var 收入账_合计_本次报告期 = new CellKeyword("收款信息", "收入回收款", "");
            keywordList.Add(收入账_合计_本次报告期);

            var 本金账_本金_正常回收_上次报告期 = new CellKeyword("收款信息", "计划内还款", new[] { "上一收款期", "本金" });
            keywordList.Add(本金账_本金_正常回收_上次报告期);

            var 本金账_本金_正常回收_本次报告期 = new CellKeyword("收款信息", "计划内还款", new[] { "本收款期", "本金" });
            keywordList.Add(本金账_本金_正常回收_本次报告期);

            var 本金账_本金_提前偿还_上次报告期 = new CellKeyword("收款信息", "提前还款", new[] { "上一收款期", "本金" });
            keywordList.Add(本金账_本金_提前偿还_上次报告期);

            var 本金账_本金_提前偿还_本次报告期 = new CellKeyword("收款信息", "提前还款", new[] { "本收款期", "本金" });
            keywordList.Add(本金账_本金_提前偿还_本次报告期);

            var 本金账_本金_拖欠回收_上次报告期 = new CellKeyword("收款信息", "拖欠回收", new[] { "上一收款期", "本金" });
            keywordList.Add(本金账_本金_拖欠回收_上次报告期);

            var 本金账_本金_拖欠回收_本次报告期 = new CellKeyword("收款信息", "拖欠回收", new[] { "本收款期", "本金" });
            keywordList.Add(本金账_本金_拖欠回收_本次报告期);

            var 本金账_本金_违约回收_上次报告期 = new CellKeyword("收款信息", "违约回收", new[] { "上一收款期", "本金" });
            keywordList.Add(本金账_本金_违约回收_上次报告期);

            var 本金账_本金_违约回收_本次报告期 = new CellKeyword("收款信息", "违约回收", new[] { "本收款期", "本金" });
            keywordList.Add(本金账_本金_违约回收_本次报告期);

            var 本金账_本金_合计_上次报告期 = new CellKeyword("收款信息", "合计", new[] { "上一收款期", "本金" });
            keywordList.Add(本金账_本金_合计_上次报告期);

            var 本金账_本金_合计_本次报告期 = new CellKeyword("收款信息", "合计", new[] { "本收款期", "本金" });
            keywordList.Add(本金账_本金_合计_本次报告期);

            var 本金账_合计_上次报告期 = new CellKeyword("收款信息", "本金回收款", "上一收款期");
            keywordList.Add(本金账_合计_上次报告期);

            var 本金账_合计_本次报告期 = new CellKeyword("收款信息", "本金回收款", "本收款期");
            keywordList.Add(本金账_合计_本次报告期);

            #endregion

            var cumlossKey = new CellKeyword("违约及严重拖欠信息", "累计违约时点违约抵押贷款金额", "上一收款期间期末");
            keywordList.Add(cumlossKey);



            var dictValues = GetCellValues(book, keywordList);

            var idrObj = new DemoJianYuanReport();

            #region 复制excel表格到word
            idrObj.违约抵押贷款在本收款期间期末所处的处置状态 = new PatternTable();
            GetPatternTable(book, idrObj.违约抵押贷款在本收款期间期末所处的处置状态,
                "违约抵押贷款在本收款期间期末所处的处置状态及抵销权风险监控",
                "占初始起算日资产池余额百分比", "汇总");

            idrObj.入池资产笔数与金额特征 = new PatternTable();
            GetPatternTable(book, idrObj.入池资产笔数与金额特征,
                "资产池存续期总体信息",
                "单笔贷款平均本金余额", new[] { "1、入池资产笔数与金额特征", "本次报告期" });

            idrObj.入池资产的期限特征 = new PatternTable();
            GetPatternTable(book, idrObj.入池资产的期限特征,
                "资产池存续期总体信息",
                "贷款最短到期期限", new[] { "2、入池资产的期限特征", "本次报告期" });

            idrObj.入池资产利率特征 = new PatternTable();
            GetPatternTable(book, idrObj.入池资产利率特征,
                "资产池存续期总体信息",
                "最低贷款利率(%)", new[] { "3、入池资产利率特征", "本次报告期" });
            #endregion


            var ymlFilePath = Path.Combine(GetModelFolder(), "Script.yml");
            CommUtils.Assert(File.Exists(ymlFilePath), "找不到yml文件[{0}]", ymlFilePath);

            List<DateTime> scheduledPaymentDates = new List<DateTime>();
            NancyDealData nancyDealData = null;
            using (StreamReader sr = new StreamReader(ymlFilePath))
            {
                nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                if (nancyDealData != null)
                {
                    scheduledPaymentDates = nancyDealData.ScheduleData.PaymentSchedule.GetScheduledPaymentDates().ToList();
                }
            }


            List<DateTime> paymentDates = nancyDealData.ScheduleData.PaymentSchedule.GetPaymentDates().ToList();
            var paymentDate = m_paymentDate;

            var asOfDate = m_asOfDate;
            var datasetFolder = Path.Combine(GetModelFolder(), asOfDate);

            Func<CellKeyword, double> parseDouble = (x) =>
            {
                double temp = 0;
                CommUtils.Assert(double.TryParse(dictValues[x], out temp), "无法转换{0}-{1}-{2}为数值",
                    x.TitleName, x.RowName, x.ColumnName);
                return temp;
            };

            Func<CellKeyword, double> percentParseDouble = (x) =>
            {
                double temp = 0;
                CommUtils.Assert(dictValues[x].Substring(dictValues[x].Length - 1) == "%", "无法转换{0}-{1}-{2}为数值",
                    x.TitleName, x.RowName, x.ColumnName);

                var value = dictValues[x].Substring(0, dictValues[x].Length - 1);
                CommUtils.Assert(double.TryParse(value, out temp), "无法转换{0}-{1}-{2}为数值",
                    x.TitleName, x.RowName, x.ColumnName);
                return temp / 100;
            };

            #region 写入各种参数到variables.csv
            
            //FutureVariables.csv
            //Cumloss
            //违约及严重拖欠信息
            //    累计违约时点违约抵押贷款金额（行） + 上一收款期间期末（列）    16154029.17
            //var futureVariablesPath = Path.Combine(datasetFolder, "FutureVariables.csv");
            //var futureVariablesCsv = new VariablesCsv();
            //futureVariablesCsv.Load(futureVariablesPath);
            //double cumloss = parseDouble(cumlossKey);
            //futureVariablesCsv.UpdateCellValue("Cumloss", paymentDate, cumloss.ToString());
            //futureVariablesCsv.Save();

            //CurrentVariables.csv
            //Collateral.InterestCollection
            //Collateral.PrincipalCollection
            //收款信息
            //    本金回收款（行） +　下一列    123009207.33
            //    收入回收款（行） +　下一列     26277809.43
            //Collateral.Loss.Amount
            //违约及严重拖欠信息
            //    新增违约抵押贷款 （行） + 金额（列）    1834059.79
            //var currentVariablesPath = Path.Combine(datasetFolder, "CurrentVariables.csv");
            //var currentVariablesCsv = new VariablesCsv();
            //currentVariablesCsv.Load(currentVariablesPath);
            //double collateralInterestCollection = parseDouble(collateralInterestCollectionKey);
            //currentVariablesCsv.UpdateCellValue("Collateral.InterestCollection", paymentDate, collateralInterestCollection.ToString());

            //double collateralPrincipalCollection = parseDouble(collateralPrincipalCollectionKey);
            //currentVariablesCsv.UpdateCellValue("Collateral.PrincipalCollection", paymentDate, collateralPrincipalCollection.ToString());

            //double collateralLossAmount = parseDouble(collateralLossAmountKey);
            //currentVariablesCsv.UpdateCellValue("Collateral.Loss.Amount", paymentDate, collateralLossAmount.ToString());
            //currentVariablesCsv.Save();

            var fileNameLength = Path.GetFileName(ymlFilePath).Length;
            var ymlFolder = ymlFilePath.Substring(0, ymlFilePath.Length - fileNameLength);
            var analysisResult = NancyUtils.RunStaticResultByPath("0", "0", ymlFolder, datasetFolder);

            var cfTable = analysisResult.CashflowDt;

            Func<string, string> findValue = (key) =>
            {
                var rowIndex = cfTable.IndexOfRow(x => x[1].ToString() == key);
                if (rowIndex >= 0)
                {
                    return cfTable.Rows[rowIndex][2].ToString();
                }
                else
                {
                    return string.Empty;
                }
            };

            var dict = new Dictionary<string, string>();
            dict["Expense.Received"] = findValue("Expense.Received");
            dict["Tax.Received"] = findValue("Tax.Received");

            idrObj.ExpenseReceived = dict["Expense.Received"];
            idrObj.TaxReceived = dict["Tax.Received"];
            #endregion

            //TODO: 收益分配报告的数据？
            var maker = new Demo_IncomeDistributionReportMaker();
            idrObj.DistInfo = maker.MakeObjectInstance(paymentDates, datasetFolder, analysisResult, nancyDealData, m_paymentDate);

            idrObj.CurrentYear = idrObj.DistInfo.T.Year;
            idrObj.Sequence = idrObj.DistInfo.Sequence;
            idrObj.SequenceInYear = paymentDates.Where(x => x.Year == idrObj.CurrentYear)
                .Count(x => x <= idrObj.DistInfo.T);


            Func<DateTime, DateTime> findScheduledPaymentDate = (date) =>
            {
                //倒序scheduledPaymentDates 然后再查找
                scheduledPaymentDates = scheduledPaymentDates.OrderByDescending(x => x).ToList();
                var lastIndex = paymentDates.Count - paymentDates.IndexOf(date) - 1;
                CommUtils.Assert(scheduledPaymentDates.Count > lastIndex, "找不到scheduledPaymentDate，scheduledPaymentDates.Count <= lastIndex");
                return scheduledPaymentDates[lastIndex];
            };

            idrObj.ScheduledPreviousT = findScheduledPaymentDate(idrObj.DistInfo.PreviousT);
            idrObj.ScheduledT = findScheduledPaymentDate(idrObj.DistInfo.T);

            idrObj.正常贷款金额 = (decimal)parseDouble(正常贷款金额);
            idrObj.正常贷款金额占比 = (decimal)percentParseDouble(正常贷款金额占比);
            idrObj.期初本金总余额 = (decimal)parseDouble(期初本金总余额);
            idrObj.期末本金总余额 = (decimal)parseDouble(期末本金总余额);
            idrObj.本期应收本金 = (decimal)parseDouble(本期应收本金);
            idrObj.本期应收利息 = (decimal)parseDouble(本期应收利息);
            idrObj.拖欠90天以上贷款金额 = (decimal)parseDouble(拖欠91至120天以上贷款金额)
                + (decimal)parseDouble(拖欠121至150天以上贷款金额)
                + (decimal)parseDouble(拖欠151至180天以上贷款金额)
                + (decimal)parseDouble(违约抵押贷款_未被注销金额);
            idrObj.拖欠90天以上贷款金额占比 = idrObj.拖欠90天以上贷款金额 / (decimal)parseDouble(贷款余额_汇总);

            idrObj.拖欠60天以上贷款金额 = idrObj.拖欠90天以上贷款金额 + (decimal)parseDouble(拖欠61至90天以上贷款金额);
            idrObj.拖欠60天以上贷款金额占比 = idrObj.拖欠60天以上贷款金额 / (decimal)parseDouble(贷款余额_汇总);
            //idrObj.拖欠90天以上贷款金额占比 + (decimal)percentParseDouble(拖欠61至90天以上贷款金额占比);

            idrObj.拖欠30天以上贷款金额 = idrObj.拖欠60天以上贷款金额 + (decimal)parseDouble(拖欠31至60天以上贷款金额);
            idrObj.拖欠30天以上贷款金额占比 = idrObj.拖欠30天以上贷款金额 / (decimal)parseDouble(贷款余额_汇总);
            //idrObj.拖欠60天以上贷款金额占比 + (decimal)percentParseDouble(拖欠31至60天以上贷款金额占比);

            idrObj.收入账_利息_正常回收_上次报告期 = (decimal)parseDouble(收入账_利息_正常回收_上次报告期);
            idrObj.收入账_利息_提前偿还_上次报告期 = (decimal)parseDouble(收入账_利息_提前偿还_上次报告期);
            idrObj.收入账_利息_拖欠回收_上次报告期 = (decimal)parseDouble(收入账_利息_拖欠回收_上次报告期);
            idrObj.收入账_利息_违约回收_上次报告期 = (decimal)parseDouble(收入账_利息_违约回收_上次报告期);
            idrObj.收入账_利息_合计_上次报告期 = (decimal)parseDouble(收入账_利息_合计_上次报告期);

            idrObj.收入账_利息_正常回收_本次报告期 = (decimal)parseDouble(收入账_利息_正常回收_本次报告期);
            idrObj.收入账_利息_提前偿还_本次报告期 = (decimal)parseDouble(收入账_利息_提前偿还_本次报告期);
            idrObj.收入账_利息_拖欠回收_本次报告期 = (decimal)parseDouble(收入账_利息_拖欠回收_本次报告期);
            idrObj.收入账_利息_违约回收_本次报告期 = (decimal)parseDouble(收入账_利息_违约回收_本次报告期);
            idrObj.收入账_利息_合计_本次报告期 = (decimal)parseDouble(收入账_利息_合计_本次报告期);

            idrObj.收入账_合计_上次报告期 = (decimal)parseDouble(收入账_合计_上次报告期);
            idrObj.收入账_合计_本次报告期 = (decimal)parseDouble(收入账_合计_本次报告期);
            idrObj.本金账_本金_正常回收_上次报告期 = (decimal)parseDouble(本金账_本金_正常回收_上次报告期);
            idrObj.本金账_本金_正常回收_本次报告期 = (decimal)parseDouble(本金账_本金_正常回收_本次报告期);
            idrObj.本金账_本金_提前偿还_上次报告期 = (decimal)parseDouble(本金账_本金_提前偿还_上次报告期);
            idrObj.本金账_本金_提前偿还_本次报告期 = (decimal)parseDouble(本金账_本金_提前偿还_本次报告期);
            idrObj.本金账_本金_拖欠回收_上次报告期 = (decimal)parseDouble(本金账_本金_拖欠回收_上次报告期);
            idrObj.本金账_本金_拖欠回收_本次报告期 = (decimal)parseDouble(本金账_本金_拖欠回收_本次报告期);
            idrObj.本金账_本金_违约回收_上次报告期 = (decimal)parseDouble(本金账_本金_违约回收_上次报告期);
            idrObj.本金账_本金_违约回收_本次报告期 = (decimal)parseDouble(本金账_本金_违约回收_本次报告期);
            idrObj.本金账_本金_合计_上次报告期 = (decimal)parseDouble(本金账_本金_合计_上次报告期);
            idrObj.本金账_本金_合计_本次报告期 = (decimal)parseDouble(本金账_本金_合计_本次报告期);

            idrObj.本金账_合计_上次报告期 = (decimal)parseDouble(本金账_合计_上次报告期);
            idrObj.本金账_合计_本次报告期 = (decimal)parseDouble(本金账_合计_本次报告期);

            idrObj.收入及本金合计_上次报告期 = idrObj.本金账_合计_上次报告期 + idrObj.收入账_合计_上次报告期;
            idrObj.收入及本金合计_本次报告期 = idrObj.本金账_合计_本次报告期 + idrObj.收入账_合计_本次报告期;


            #region 在Excel里查找收款期间截止日
            {
                var sheet = FindSheetByTitle(book, 2, 0, "资产池存续期总体信息");
                var val = GetCellValue(sheet, 0, 0);
                var index = val.LastIndexOf(idrObj.DistInfo.T.Year.ToString());
                var strEndDate = val.Substring(index);


                var indexBeginDate = val.IndexOf(idrObj.DistInfo.T.Year.ToString());
                var indexBeginDateDay = val.IndexOf("日");
                var strBeginDate = val.Substring(indexBeginDate, indexBeginDateDay - indexBeginDate + 1);

                DateTime receiveMoneyBeginDate;
                CommUtils.Assert(DateTime.TryParse(strBeginDate, out receiveMoneyBeginDate),
                    "解析收款期间开始日失败：" + val);

                idrObj.ReceiveMoneyBeginDate = receiveMoneyBeginDate;


                DateTime receiveMoneyEndDate;
                CommUtils.Assert(DateTime.TryParse(strEndDate, out receiveMoneyEndDate),
                    "解析收款期间截止日失败：" + val);

                idrObj.ReceiveMoneyEndDate = receiveMoneyEndDate;
            }
            #endregion

            return idrObj;
        }

        private List<CellKeyword> GetCellKeyWord()
        {
            return new List<CellKeyword>();
        }
    }


    public class Demo_IncomeDistributionReportMaker
    {
        private string InsertHyphenBeforeNumber(string text)
        {
            for (int i = 1; i < text.Length; ++i)
            {
                if (Char.IsNumber(text[i]))
                {
                    return text.Insert(i, "-");
                }
            }

            return text;
        }

        public IncomeDistributionReport MakeObjectInstance(List<DateTime> paymentDates, string datasetFolder,
            NancyStaticAnalysisResult nancyResult, NancyDealData nancyDealData, DateTime paymentDate)
        {
            //nancyResult.NoteResults

            //var futureVariablesCsv = new VariablesCsv();
            //var futureVariablesPath = Path.Combine(datasetFolder, "FutureVariables.csv");
            //futureVariablesCsv.Load(futureVariablesPath);

            //var variables = futureVariablesCsv.GetVariablesByDate(paymentDate);
            //var rateResetRecords = InterestRateUtils.RateResetRecords(variables);

            var idrObj = new IncomeDistributionReport();
            idrObj.Sequence = paymentDates.IndexOf(paymentDate) + 1;
            idrObj.SequenceCN = idrObj.Sequence.ToCnString();
            idrObj.Security = new Dictionary<string, PaymentDetail>();
            idrObj.PriorSecurityList = new List<PaymentDetail>();
            idrObj.SubSecurityList = new List<PaymentDetail>();
            idrObj.SecurityList = new List<PaymentDetail>();
            //TODO:
            //idrObj.BeginAccrualDate = sequence == 0 ? schedule.FirstAccrualDate : schedule.NoteAccrualDates.First().Value[sequence - 1];
            //idrObj.EndAccrualDate = schedule.NoteAccrualDates.First().Value[sequence];
            idrObj.AccrualDateSum = (idrObj.EndAccrualDate - idrObj.BeginAccrualDate).Days;

            var cfTable = nancyResult.CashflowDt;

            var currentDatasetColumnIndex = 2;
            for (int i = 0; i < cfTable.Columns.Count; ++i)
            {
                DateTime temp;
                if (DateTime.TryParse(cfTable.Columns[i].ColumnName, out temp)
                    && paymentDate == temp)
                {
                    currentDatasetColumnIndex = i;
                    break;
                }
            }


            for (int i = 0; i < nancyDealData.Notes.Count; i++)
            {
                var nancyNote = nancyDealData.Notes[i];
                var note = new Note {
                    NoteName = nancyNote.Code,
                    ShortName = nancyNote.Name,
                    IsEquity = nancyNote.IsEquity,
                    CouponString = nancyNote.CouponString,
                    Notional = (decimal)nancyNote.Notional
                };

                //note.CouponString = InterestRateUtils.CalculateCurrentCouponRate(
                //    nancyDealData.Notes[i].CouponString, rateResetRecords);

                var noteData = new NoteData();

                var n = note;
                var prin = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(n.ShortName + ".Principal") && r[1].ToString().Contains("Received"))[currentDatasetColumnIndex].ToString();
                noteData.PrincipalPaid = (decimal)((prin == string.Empty || prin == "-") ? 0.0 : double.Parse(prin));
                var interest = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(n.ShortName + ".Interest") && r[1].ToString().Contains("Received"))[currentDatasetColumnIndex].ToString();
                noteData.InterestPaid = (decimal)((interest == string.Empty || interest == "-") ? 0.0 : double.Parse(interest));
                var end = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(n.ShortName + ".Beginning") && r[1].ToString().Contains("Outstanding"))[currentDatasetColumnIndex + 1].ToString();
                noteData.EndingBalance = (decimal)((end == string.Empty || end == "-") ? 0.0 : double.Parse(end));


                idrObj.Security[note.ShortName] = GeneratePaymentDetail(note, noteData, idrObj.Security.Count + 1);

                if (note.IsEquity)
                {
                    idrObj.SubSecurityList.Add(GeneratePaymentDetail(note, noteData, idrObj.SubSecurityList.Count + 1));
                }
                else
                {
                    idrObj.PriorSecurityList.Add(GeneratePaymentDetail(note, noteData, idrObj.PriorSecurityList.Count + 1));
                }

                idrObj.SecurityList.Add(GeneratePaymentDetail(note, noteData, idrObj.SecurityList.Count(x => x.Money != 0) + 1));
            }

            Func<IEnumerable<PaymentDetail>, PaymentDetail> sum = (values) => new PaymentDetail
            {
                Residual = values.Sum(x => x.Residual),
                Principal = values.Sum(x => x.Principal),
                Interest = values.Sum(x => x.Interest),
                Money = values.Sum(x => x.Money),
                UnitCount = values.Sum(x => x.UnitCount),
                SumPaymentAmount = values.Sum(x => x.SumPaymentAmount)
            };

            idrObj.Sum = sum(idrObj.Security.Values);
            idrObj.SumPrior = sum(idrObj.Security.Values.Where(x => !x.IsEquity));
            idrObj.SumSub = sum(idrObj.Security.Values.Where(x => x.IsEquity));

            //GeneratePercentTable(idrObj, datasets, notes, schedule.PaymentDates);

            idrObj.RepayDetail = GenerateRepayDetail(idrObj.SecurityList, x => x.NameCN);
            idrObj.RepayDetailWithHyphen = GenerateRepayDetail(idrObj.SecurityList, x => x.NameCNHyphen);
            idrObj.RepayDetailWithHyphenByJinTai = GenerateRepayDetailByJinTai(idrObj.SecurityList, x => x.NameCNHyphen);
            idrObj.RepayPrincipalDetail = GenerateRepayPrincipalDetail(idrObj.SecurityList);
            idrObj.DenominationDetail = GenerateDenominationDetail(idrObj.SecurityList);
            idrObj.EquityRegisterDetail = GenerateEquityRegisterDetail(idrObj.SecurityList, paymentDate);
            idrObj.EquityRegisterDetailByJinTai = GenerateEquityRegisterDetailByJinTai(idrObj.SecurityList, paymentDate);
            idrObj.EquityRegisterDetailByZhongGang = GenerateEquityRegisterDetailByZhongGang(idrObj.SecurityList, paymentDate);
            idrObj.EquityRegisterDetailByYingBinGuan = GenerateEquityRegisterDetailByYingBinGuan(idrObj.SecurityList, paymentDate);

            idrObj.T = paymentDate;

            if (paymentDates.First() == idrObj.T)
            {
                //TODO:
                //idrObj.PreviousT = schedule.ClosingDate;
            }
            else
            {
                var previousIndex = paymentDates.IndexOf(idrObj.T) - 1;
                idrObj.PreviousT = paymentDates[previousIndex];
            }

            idrObj.DurationDayCount = (idrObj.T - idrObj.PreviousT).Days;

            var t_1 = paymentDate.AddDays(-1);
            while (!CalendarCache.IsTradingDay(t_1))
            {
                t_1 = t_1.AddDays(-1);
            }
            idrObj.T_1 = t_1;
            idrObj.Date = DateTime.Today;
            idrObj.TaskEndTime = DateTime.Now;
            return idrObj;
        }

        private string GenerateNameCNHyphen(Note note)
        {
            if (note.IsEquity)
            {
                return "次";
            }
            else
            {
                return "优先" + InsertHyphenBeforeNumber(note.ShortName);
            }
        }

        private PaymentDetail GeneratePaymentDetail(Note note, NoteData noteData, int sequence)
        {
            var detail = new PaymentDetail();
            detail.Sequence = sequence;
            detail.NameCN = note.NoteName;
            detail.NameCNHyphen = GenerateNameCNHyphen(note);
            detail.NameCNFullHyphen = detail.NameCNHyphen + "级资产支持证券";
            detail.NameEN = note.ShortName;
            detail.NameENHyphen = InsertHyphenBeforeNumber(detail.NameEN);
            detail.IsEquity = note.IsEquity;
            detail.Residual = noteData.EndingBalance.Value + noteData.PrincipalPaid.Value;
            detail.Notional = note.Notional.Value;
            detail.UnitCount = (int)(note.Notional.Value / 100);
            detail.Principal = noteData.PrincipalPaid.Value;
            detail.Interest = noteData.InterestPaid.Value;
            detail.Money = detail.Principal + detail.Interest;
            detail.UnitPrincipal = detail.Principal / detail.UnitCount;
            detail.UnitInterest = detail.Interest / detail.UnitCount;
            detail.UnitMoney = detail.Money / detail.UnitCount;
            detail.Denomination = noteData.EndingBalance.Value / note.Notional.Value * 100;
            detail.CouponString = note.CouponString.Replace(" ", "");
            detail.PrincipalPercent = detail.Notional == 0 ? 0 : detail.Principal / detail.Notional;
            detail.PrincipalPercentInDataset = detail.Residual == 0 ? 0 : detail.Principal / detail.Residual;
            detail.EndingBalance = detail.Residual - detail.Principal;
            detail.SumPaymentAmount = detail.Interest + detail.Principal;
            return detail;
        }

        /// <summary>
        /// 生成权益登记日段落（白鹭2016-1专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetail(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_3Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal == x.Residual).ToList()
                .ConvertAll(x => x.NameCNHyphen).ToArray();
            var contentT_3 = string.Join("、", t_3Names);
            if (!string.IsNullOrEmpty(contentT_3))
            {
                var t_3 = t;
                for (int i = 0; i < 3; ++i)
                {
                    t_3 = DateUtils.GetPreviousWorkingDay(t_3);
                }

                contentT_3 += "的权益登记日均为" + t_3.ToString("yyyy年M月d日") + "。";
            }

            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCNHyphen).ToArray();
            var contentT_1 = string.Join("、", t_1Names);
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_3 + contentT_1;
        }

        /// <summary>
        /// 生成权益登记日段落（金泰专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetailByJinTai(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCNFullHyphen).ToArray();
            var contentT_1 = CommUtils.Join("、", "和", t_1Names.ToList());
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_1;
        }

        /// <summary>
        /// 生成权益登记日段落（中港专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetailByZhongGang(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCN).ToArray();
            var contentT_1 = CommUtils.Join("、", "和", t_1Names.ToList());
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_1;
        }

        /// <summary>
        /// 生成权益登记日段落（迎宾馆2016-1专用）
        /// </summary>
        /// <param name="paymentDeatil"></param>
        /// <returns></returns>
        private string GenerateEquityRegisterDetailByYingBinGuan(List<PaymentDetail> paymentDeatil, DateTime t)
        {
            var t_3Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal == x.Residual).ToList()
                .ConvertAll(x => x.NameCN).ToArray();
            var contentT_3 = CommUtils.Join("、", "和", t_3Names.ToList());
            if (!string.IsNullOrEmpty(contentT_3))
            {
                var t_3 = t;
                for (int i = 0; i < 3; ++i)
                {
                    t_3 = DateUtils.GetPreviousWorkingDay(t_3);
                }

                contentT_3 += "的权益登记日均为" + t_3.ToString("yyyy年M月d日") + "。";
            }

            var t_1Names = paymentDeatil.Where(x => x.Money != 0 && x.Principal != x.Residual).ToList()
                .ConvertAll(x => x.NameCN).ToArray();
            var contentT_1 = CommUtils.Join("、", "和", t_1Names.ToList());
            if (!string.IsNullOrEmpty(contentT_1))
            {
                var t_1 = DateUtils.GetPreviousWorkingDay(t);
                contentT_1 += "的权益登记日均为" + t_1.ToString("yyyy年M月d日") + "。";
            }

            return contentT_3 + contentT_1;
        }

        private string GenerateDenominationDetail(List<PaymentDetail> paymentDeatil)
        {
            var sentences = paymentDeatil.Where(x => x.Denomination != 100).ToList().ConvertAll(x =>
            {
                var text = string.Empty;
                text += (x.IsEquity ? "次级" : "优先" + x.NameENHyphen + "级");
                text += "资产支持证券每份面值为" + x.Denomination.ToString("n2") + "元";
                return text;
            });

            var content = string.Empty;
            content = string.Join("，", sentences.ToArray());
            if (!string.IsNullOrEmpty(content))
            {
                content += (paymentDeatil.Exists(x => x.Denomination == 100) ? "，" : "。");
            }

            sentences = paymentDeatil.Where(x => x.Denomination == 100).ToList().ConvertAll(x =>
            {
                return (x.IsEquity ? "次级" : "优先" + x.NameENHyphen + "级");
            });

            var sentence100 = string.Join("、", sentences.ToArray());
            if (!string.IsNullOrEmpty(sentence100))
            {
                sentence100 += "资产支持证券每份面值为100元。";
            }

            return content + sentence100;
        }

        private string GenerateRepayPrincipalDetail(List<PaymentDetail> paymentDeatil)
        {
            var sentences = paymentDeatil.Where(x => x.Notional != x.Residual).ToList().ConvertAll(x =>
            {
                var text = string.Empty;
                text += (x.IsEquity ? "次级" : "优先" + x.NameENHyphen + "级");
                text += "截至本收益分配报告日已分期偿还本金";
                text += ((x.Notional - x.Residual) / x.Notional).ToString("p2");
                return text;
            });

            var content = string.Empty;
            content = string.Join("；", sentences.ToArray()) + "。";
            if (paymentDeatil.Exists(x => x.Notional == x.Residual))
            {
                content += "其余证券截至本收益分配报告日尚未进行还本。";
            }
            return content;
        }

        /// <summary>
        /// 生成分期偿还情况的段落（使用带-的券名）(金泰专用)
        /// </summary>
        /// <param name="paymentDetailList"></param>
        /// <param name="getName"></param>
        /// <returns></returns>
        private string GenerateRepayDetailByJinTai(List<PaymentDetail> paymentDetailList, Func<PaymentDetail, string> getName)
        {
            var listTrading = new List<string>();//本金兑付中
            var listLastTrade = new List<string>();//最后一次兑付本金
            var repaymentPercentage = new List<string>();//偿还本金百分数
            var residueMoney = new List<string>();//剩余本金面值

            foreach (var detail in paymentDetailList)
            {
                if (!detail.IsEquity)
                {
                    if (detail.Residual - detail.Principal > 0)
                    {
                        listTrading.Add(getName(detail) + "级资产支持证券");
                        repaymentPercentage.Add((detail.Principal / detail.Notional).ToString("0.00%"));
                        residueMoney.Add(((detail.Residual - detail.Principal) * 100 / detail.Notional).ToString("n3"));
                    }
                    else if (detail.Residual - detail.Principal == 0 && detail.Principal > 0)
                    {
                        listLastTrade.Add(getName(detail) + "级资产支持证券");
                    }
                }
            }

            var content = string.Empty;
            if (listLastTrade.Count != 0)
            {
                content += string.Join("、", listLastTrade.ToArray()) + "经本次分配后，本金兑付完毕。";
            }
            if (listTrading.Count != 0)
            {
                content += string.Join("、", listTrading.ToArray()) + "经本次分配后，本金未兑付完毕，将继续进行交易。本次兑付，";
                for (int i = 0; i < listTrading.Count; i++)
                {
                    var textStr = double.Parse(repaymentPercentage[i].Replace("%", "").Trim()) == 0 ? "不变。" : "调整为" + residueMoney[i] + "元。";

                    content += listTrading[i] + "将偿还本金" + repaymentPercentage[i];
                    content += "，剩余本金面值" + textStr;
                }
            }
            if (content == string.Empty)
            {
                content = "本金全部兑付完毕。";
            }

            return content;
        }
        private string GenerateRepayDetail(List<PaymentDetail> paymentDetailList, Func<PaymentDetail, string> getName)
        {
            var listTrading = new List<string>();//本金兑付中
            var listLastTrade = new List<string>();//最后一次兑付本金

            foreach (var detail in paymentDetailList)
            {
                if (!detail.IsEquity)
                {
                    if (detail.Residual - detail.Principal > 0)
                    {
                        listTrading.Add(getName(detail));
                    }
                    else if (detail.Residual - detail.Principal == 0 && detail.Principal > 0)
                    {
                        listLastTrade.Add(getName(detail));
                    }
                }
            }

            var content = string.Empty;
            if (listLastTrade.Count != 0)
            {
                content += string.Join("、", listLastTrade.ToArray()) + "经本次分配后，本金兑付完毕。";
            }
            if (listTrading.Count != 0)
            {
                content += string.Join("、", listTrading.ToArray()) + "经本次分配后，本金未兑付完毕，将继续进行交易。";
            }
            if (content == string.Empty)
            {
                content = "本金全部兑付完毕。";
            }

            return content;
        }
    }
}
