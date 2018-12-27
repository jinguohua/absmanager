using System.Text.RegularExpressions;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class CellTelValidation : ICellValidation
    {
        public CellTelValidation(bool isNullOrNot = true)
        {
            ErrorMsg = string.Empty;
            IsNullOrNot = isNullOrNot;
        }

        public bool IsValid(string cellText)
        {
            if ((!IsNullOrNot) && (string.IsNullOrWhiteSpace(cellText) || cellText == "-"))
            {
                ErrorMsg = "不能为空";
                return false;
            }

            //不带字母的任意字符串
            Regex r = new Regex("^[^a-zA-Z]+$");
            if (!string.IsNullOrWhiteSpace(cellText) && cellText != "-" && (!r.IsMatch(cellText)))
            {
                ErrorMsg = "无法将[" + cellText + "]解析为电话号码";
                return false;
            }

            return true;
        }

        public string ErrorMsg { get; set; }

        public bool IsNullOrNot { get; set; }
    }
}
