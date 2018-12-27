
namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public interface ICellValidation
    {
        bool IsValid(string cellText);

        string ErrorMsg { get; set; }
    }
}
