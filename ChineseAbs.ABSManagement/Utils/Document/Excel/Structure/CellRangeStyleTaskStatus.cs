
namespace ChineseAbs.ABSManagement.Utils.Excel.Structure
{
    public class CellRangeStyleTaskStatus : ICellRangeStyle
    {
        public NPOI.SS.UserModel.ICellStyle Get(Workbook workbook, Cell cell = null)
        {
            var styleFactory = workbook.StyleFactory;
            var style = styleFactory.Create(CellStylePattern.DefaultBorder, CellStylePattern.DefaultFont);
            style = styleFactory.AppendStyleTaskStatus(style, cell);
            return style;
        }
    }
}
