using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DatasetModel;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Pattern.Demo;
using ChineseAbs.ABSManagement.Utils;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using PatternTableRow = System.Collections.Generic.Dictionary<string, string>;
using PatternTable = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>;

namespace ChineseAbs.ABSManagement.DocumentFactory.Maker.Demo
{
    public class DemoJianYuanReportMaker : DocumentMakerBase
    {
        public DemoJianYuanReportMaker(string userName)
            :base(userName)
        {
        }

        protected override string GetPatternFilePath()
        {
            return DocumentPattern.GetPath(m_project, DocPatternType.DemoJianYuanReport);
        }

        protected override void ParseParams(params object[] param)
        {
            if (param.Length == 0)
            {
                throw new ApplicationException("Demo建元模板:Generate need shrotCode.");
            }

            var shortCode = (string)param[0];
            var excelStream = (Stream)param[1];
            var excelFileName = (string)param[2];

            m_task = m_dbAdapter.Task.GetTask(shortCode);
            m_project = m_dbAdapter.Project.GetProjectById(m_task.ProjectId);
            m_excelStream = excelStream;
            m_excelFileName = excelFileName;
        }

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

        protected override object MakeObjectInstance()
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

            //var 收入账_其他收入_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入账_其他收入_上次报告期);

            //var 收入账_其他收入_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入账_其他收入_本次报告期);

            //var 收入账_上期转存_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入账_上期转存_上次报告期);

            //var 收入账_上期转存_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入账_上期转存_本次报告期);

            //var 收入账_合格投资_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入账_合格投资_上次报告期);

            //var 收入账_合格投资_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入账_合格投资_本次报告期);

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

            //var 本金账_其他收入_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(本金账_其他收入_上次报告期);

            //var 本金账_其他收入_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(本金账_其他收入_本次报告期);

            //var 本金账_上期转存_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(本金账_上期转存_上次报告期);

            //var 本金账_上期转存_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(本金账_上期转存_本次报告期);

            var 本金账_合计_上次报告期 = new CellKeyword("收款信息", "本金回收款", "上一收款期");
            keywordList.Add(本金账_合计_上次报告期);

            var 本金账_合计_本次报告期 = new CellKeyword("收款信息", "本金回收款", "本收款期");
            keywordList.Add(本金账_合计_本次报告期);

            //var 收入及本金合计_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入及本金合计_上次报告期);

            //var 收入及本金合计_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(收入及本金合计_本次报告期);

            //var 税收_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(税收_上次报告期);

            //var 税收_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(税收_本次报告期);

            //var 费用支出_服务总费用支出_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(费用支出_服务总费用支出_上次报告期);

            //var 费用支出_服务总费用支出_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(费用支出_服务总费用支出_本次报告期);

            //var 费用支出_其他费用支出_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(费用支出_其他费用支出_上次报告期);

            //var 费用支出_其他费用支出_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(费用支出_其他费用支出_本次报告期);

            //var 证券账户支出_证券利息总支出_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(证券账户支出_证券利息总支出_上次报告期);

            //var 证券账户支出_证券利息总支出_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(证券账户支出_证券利息总支出_本次报告期);

            //var 证券账户支出_证券本金总支出_上次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(证券账户支出_证券本金总支出_上次报告期);

            //var 证券账户支出_证券本金总支出_本次报告期 = new CellKeyword("收款信息", "计划内还款", "利息");
            //keywordList.Add(证券账户支出_证券本金总支出_本次报告期);

            //var 信托核算日 = new CellKeyword("资产池存续期总体信息", "", "时间111");
            //keywordList.Add(信托核算日);
            #endregion

            var cumlossKey = new CellKeyword("违约及严重拖欠信息", "累计违约时点违约抵押贷款金额", "上一收款期间期末");
            keywordList.Add(cumlossKey);



            var dictValues = GetCellValues(book, keywordList);

            var idrObj = new DemoJianYuanReport();

            idrObj.违约抵押贷款在本收款期间期末所处的处置状态 = new PatternTable();
            GetPatternTable(book, idrObj.违约抵押贷款在本收款期间期末所处的处置状态,
                "违约抵押贷款在本收款期间期末所处的处置状态及抵销权风险监控",
                "占初始起算日资产池余额百分比", "汇总");

            idrObj.入池资产笔数与金额特征 = new PatternTable();
            GetPatternTable(book, idrObj.入池资产笔数与金额特征,
                "资产池存续期总体信息",
                "单笔贷款平均本金余额", new []{"1、入池资产笔数与金额特征", "本次报告期"});

            idrObj.入池资产的期限特征 = new PatternTable();
            GetPatternTable(book, idrObj.入池资产的期限特征,
                "资产池存续期总体信息",
                "贷款最短到期期限", new []{"2、入池资产的期限特征", "本次报告期"});

            idrObj.入池资产利率特征 = new PatternTable();
            GetPatternTable(book, idrObj.入池资产利率特征,
                "资产池存续期总体信息",
                "最低贷款利率(%)", new []{"3、入池资产利率特征", "本次报告期"});


            var logicModel = new ProjectLogicModel(m_userName, m_project.ProjectId);
            var schedule = logicModel.DealSchedule.Instanse;

            List<DateTime> paymentDates = schedule.PaymentDates.ToList();

            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(this.m_task.ShortCode);
            var paymentDate = m_task.EndTime.Value;
            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
            }

            var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(m_project.ProjectId);
            paymentDate = paymentDates.First(x => x >= paymentDate);

            datasets = datasets.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value <= paymentDate).ToList();
            var findDatasets = datasets.Where(x => x.PaymentDate.HasValue && x.PaymentDate.Value == paymentDate).ToList();
            findDatasets.Sort((l, r) => l.AsOfDate.CompareTo(r.AsOfDate));
            CommUtils.Assert(findDatasets.Count >= 1, "找不到偿付期为 [{0}] 的数据模型", DateUtils.DateToString(paymentDate));

            var dataset = findDatasets[0];


            var scheduledPaymentDates = paymentDates.Select(x => x).ToList();
            var rootFolder = WebConfigUtils.RootFolder;
            var project = m_dbAdapter.Project.GetProjectById(logicModel.Instance.ProjectId);
            var modelFolder = Path.Combine(rootFolder, project.Model.ModelFolder);
            var ymlFilePath = modelFolder + @"\Script.yml";
            if (File.Exists(ymlFilePath))
            {
                using (StreamReader sr = new StreamReader(ymlFilePath))
                {
                    var nancyDealData = NancyUtils.GetNancyDealDataByFile(sr.BaseStream);
                    if (nancyDealData != null)
                    {
                        scheduledPaymentDates = nancyDealData.ScheduleData.PaymentSchedule.GetScheduledPaymentDates().ToList();
                    }
                }
            }


            var dealScheduleLogicModel = logicModel.DealSchedule.GetByPaymentDay(findDatasets[0].PaymentDate.Value);

            var dbAdapter = new DBAdapter();
            var datasetFolder = dbAdapter.Dataset.GetDatasetFolder(logicModel.Instance, dataset.AsOfDate);

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
                return temp/100;
            };

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

            var dict = SaveNoteData(m_project.ProjectId);
            idrObj.ExpenseReceived = dict["Expense.Received"];
            idrObj.TaxReceived = dict["Tax.Received"];

            var maker = new IncomeDistributionReportMaker(m_userName);
            idrObj.DistInfo = (IncomeDistributionReport)maker.GetObjInstance(m_task.ProjectId, paymentDate, m_task.EndTime.Value);


            //从第N期开始模型数据时，getDealSchedule中不包含前几期的PaymentDate
            if (m_project.CnabsDealId.HasValue)
            {
                paymentDates = m_dbAdapter.Model.GetPaymentDates(m_project.CnabsDealId.Value);
            }

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
            //idrObj.收入账_其他收入_上次报告期 = (decimal)parseDouble(收入账_其他收入_上次报告期);
            //idrObj.收入账_其他收入_本次报告期 = (decimal)parseDouble(收入账_其他收入_本次报告期);
            //idrObj.收入账_上期转存_上次报告期 = (decimal)parseDouble(收入账_上期转存_上次报告期);
            //idrObj.收入账_上期转存_本次报告期 = (decimal)parseDouble(收入账_上期转存_本次报告期);
            //idrObj.收入账_合格投资_上次报告期 = (decimal)parseDouble(收入账_合格投资_上次报告期);
            //idrObj.收入账_合格投资_本次报告期 = (decimal)parseDouble(收入账_合格投资_本次报告期);
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
            //idrObj.本金账_其他收入_上次报告期 = (decimal)parseDouble(本金账_其他收入_上次报告期);
            //idrObj.本金账_其他收入_本次报告期 = (decimal)parseDouble(本金账_其他收入_本次报告期);
            //idrObj.本金账_上期转存_上次报告期 = (decimal)parseDouble(本金账_上期转存_上次报告期);
            //idrObj.本金账_上期转存_本次报告期 = (decimal)parseDouble(本金账_上期转存_本次报告期);
            idrObj.本金账_合计_上次报告期 = (decimal)parseDouble(本金账_合计_上次报告期);
            idrObj.本金账_合计_本次报告期 = (decimal)parseDouble(本金账_合计_本次报告期);

            idrObj.收入及本金合计_上次报告期 = idrObj.本金账_合计_上次报告期 + idrObj.收入账_合计_上次报告期;
            idrObj.收入及本金合计_本次报告期 = idrObj.本金账_合计_本次报告期 + idrObj.收入账_合计_本次报告期;
            
            //idrObj.税收_上次报告期 = (decimal)parseDouble(税收_上次报告期);
            //idrObj.税收_本次报告期 = (decimal)parseDouble(税收_本次报告期);
            //idrObj.费用支出_服务总费用支出_上次报告期 = (decimal)parseDouble(费用支出_服务总费用支出_上次报告期);
            //idrObj.费用支出_服务总费用支出_本次报告期 = (decimal)parseDouble(费用支出_服务总费用支出_本次报告期);
            //idrObj.费用支出_其他费用支出_上次报告期 = (decimal)parseDouble(费用支出_其他费用支出_上次报告期);
            //idrObj.费用支出_其他费用支出_本次报告期 = (decimal)parseDouble(费用支出_其他费用支出_本次报告期);
            //idrObj.证券账户支出_证券利息总支出_上次报告期 = (decimal)parseDouble(证券账户支出_证券利息总支出_上次报告期);
            //idrObj.证券账户支出_证券利息总支出_本次报告期 = (decimal)parseDouble(证券账户支出_证券利息总支出_本次报告期);
            //idrObj.证券账户支出_证券本金总支出_上次报告期 = (decimal)parseDouble(证券账户支出_证券本金总支出_上次报告期);
            //idrObj.证券账户支出_证券本金总支出_本次报告期 = (decimal)parseDouble(证券账户支出_证券本金总支出_本次报告期);
            //idrObj.信托核算日 = dictValues[信托核算日];

            //查找收款期间截止日
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

            return idrObj;
        }

        private List<CellKeyword> GetCellKeyWord()
        {
            return new List<CellKeyword>();
        }

        private Dictionary<string, string> SaveNoteData(int projectId)
        {
            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(this.m_task.ShortCode);
            var paymentDate = this.m_task.EndTime.Value;
            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
            }

            var projectLogicModel = new ProjectLogicModel(m_userName, projectId);
            var project = projectLogicModel.Instance;
            var dataset = m_dbAdapter.Dataset.GetDatasetByDurationPeriod(project.ProjectId, paymentDate);
            var results = NancyUtils.GetStaticAnalyticsResult(projectId, null, dataset.AsOfDate);
            var cfTable = results.CashflowDt;

            Func<string, string> findValue = (key) => { 
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


            var notes = m_dbAdapter.Dataset.GetNotes(project.ProjectId);

            var currentDatasetColumnIndex = 2;
            for (int i = 0; i < cfTable.Columns.Count; ++i)
            {
                DateTime temp;
                if (DateTime.TryParse(cfTable.Columns[i].ColumnName, out temp)
                    && dataset.PaymentDate.Value == temp)
                {
                    currentDatasetColumnIndex = i;
                    break;
                }
            }

            foreach (var n in notes)
            {
                var nd = m_dbAdapter.Dataset.GetNoteData((int)n.NoteId, dataset.DatasetId);
                var prin = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(n.ShortName + ".Principal") && r[1].ToString().Contains("Received"))[currentDatasetColumnIndex].ToString();
                nd.PrincipalPaid = (decimal)((prin == string.Empty || prin == "-") ? 0.0 : double.Parse(prin));
                var interest = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(n.ShortName + ".Interest") && r[1].ToString().Contains("Received"))[currentDatasetColumnIndex].ToString();
                nd.InterestPaid = (decimal)((interest == string.Empty || interest == "-") ? 0.0 : double.Parse(interest));
                var end = cfTable.AsEnumerable().FirstOrDefault(r => r[1].ToString().Contains(n.ShortName + ".Beginning") && r[1].ToString().Contains("Outstanding"))[currentDatasetColumnIndex + 1].ToString();
                nd.EndingBalance = (decimal)((end == string.Empty || end == "-") ? 0.0 : double.Parse(end));
                m_dbAdapter.Dataset.UpdateNoteData(nd);
            }
            //var nr = m_dbAdapter.Dataset.GetNoteResultByProjectIdAndDatasetId(project.ProjectId, dataset.DatasetId);
            //DataTable drResult = results.AssetCashflowDt;
            //DataTable drCf = results.CashflowDt;
            //nr.NoteResultTable = results.Tables[0];
            //nr.NoteCashflowTable = CommUtils.ToJson(drCf);
            //m_dbAdapter.Dataset.UpdateNoteResult(nr);

            return dict;
        }

        private Task m_task;

        private Stream m_excelStream;
        private string m_excelFileName;
    }
}
