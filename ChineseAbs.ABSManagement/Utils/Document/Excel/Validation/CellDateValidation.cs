using System;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class CellDateValidation: ICellValidation
    {
        public CellDateValidation(bool isNullOrNot = true)
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

            DateTime time;
            if (!string.IsNullOrWhiteSpace(cellText) && cellText != "-" && (!DateTime.TryParse(cellText, out time)))
            {
                ErrorMsg = "无法将[" + cellText + "]解析为时间(Date)";
                return false;
            }

            return true;
        }

        public string ErrorMsg { get; set; }

        public bool IsNullOrNot { get; set; }
    }
}
