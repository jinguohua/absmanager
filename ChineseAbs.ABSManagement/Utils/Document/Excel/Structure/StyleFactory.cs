using NPOI.SS.UserModel;

namespace ChineseAbs.ABSManagement.Utils.Excel.Structure
{
    public class StyleFactory
    {
        public StyleFactory(Workbook workbook)
        {
            m_workbook = workbook;
        }

        public ICellStyle Create()
        {
            return CreateCellStyle();
        }

        public ICellStyle Create(CellStylePattern pattern)
        {
            var style = Create();
            return Append(style, pattern);
        }

        public ICellStyle Create(CellStylePattern pattern1, CellStylePattern pattern2)
        {
            var style = Create(pattern1);
            return Append(style, pattern2);
        }

        public ICellStyle Create(CellStylePattern pattern1, CellStylePattern pattern2, CellStylePattern pattern3)
        {
            var style = Create(pattern1, pattern2);
            return Append(style, pattern3);
        }

        private ICellStyle CreateCellStyle()
        {
            return m_workbook.GetIWorkbook().CreateCellStyle();
        }

        private ICellStyle Append(ICellStyle style, CellStylePattern pattern)
        {
            switch (pattern)
            {
                case CellStylePattern.DefaultBorder:
                    return AppendStyleDefaultBorder(style);
                case CellStylePattern.DefaultFont:
                    return AppendStyleDefaultFont(style);
                default:
                    throw new System.NotImplementedException("Unsupported CellStylePattern: " + pattern.ToString());
            }
        }

        private ICellStyle AppendStyleDefaultBorder(ICellStyle style)
        {
            var blackColorIndex = IndexedColors.Black.Index;
            style.BorderRight = BorderStyle.Thin;
            style.RightBorderColor = blackColorIndex;
            style.BorderBottom = BorderStyle.Thin;
            style.BottomBorderColor = blackColorIndex;
            style.BorderLeft = BorderStyle.Thin;
            style.LeftBorderColor = blackColorIndex;
            style.BorderTop = BorderStyle.Thin;
            style.TopBorderColor = blackColorIndex;
            return style;
        }

        private ICellStyle AppendStyleDefaultFont(ICellStyle style)
        {
            IFont fontStyle = m_workbook.GetIWorkbook().CreateFont();
            fontStyle.FontName = "微软雅黑";
            fontStyle.FontHeightInPoints = 10;
            style.SetFont(fontStyle);
            return style;
        }

        public ICellStyle AppendStyleTaskStatus(ICellStyle style, Cell cell)
        {
            if (cell.IsEmpty)
            {
                return AppendStyleDefaultBorder(style); 
            }
            var cellValue = cell.GetCellValue();
            if (cellValue == "完成" || cellValue == "错误")
            {
                return ExcelUtils.GetFontColorStyle(m_workbook.GetIWorkbook(), ExcelFontColor.Green);
            }
            else if (cellValue == "进行中" || cellValue == "等待")
            {
                return ExcelUtils.GetFontColorStyle(m_workbook.GetIWorkbook(), ExcelFontColor.Blue);
            }
            else if (cellValue == "逾期")
            {
                return ExcelUtils.GetFontColorStyle(m_workbook.GetIWorkbook(), ExcelFontColor.Red);
            }
            return style;
        }

        private Workbook m_workbook;
    }
}
