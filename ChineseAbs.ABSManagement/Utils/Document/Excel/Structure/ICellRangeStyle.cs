
namespace ChineseAbs.ABSManagement.Utils.Excel.Structure
{
    public interface ICellRangeStyle
    {
        NPOI.SS.UserModel.ICellStyle Get(Workbook workbook, Cell cell);
    }
}
