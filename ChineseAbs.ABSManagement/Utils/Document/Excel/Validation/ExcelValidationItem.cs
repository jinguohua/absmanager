
namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class ExcelValidationItem
    {
        public ExcelValidationItem(CellRange cellRange, ICellValidation cellValidation)
        {
            CellRange = cellRange;
            CellValidation = cellValidation;
        }

        public CellRange CellRange { get; set; }

        public ICellValidation CellValidation { get; set; }
    }
}
