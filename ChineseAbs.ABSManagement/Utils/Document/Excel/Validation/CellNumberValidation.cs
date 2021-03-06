﻿
namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class CellNumberValidation : ICellValidation
    {
        public CellNumberValidation(bool isNullOrNot = true)
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

            double number;
            if (!string.IsNullOrWhiteSpace(cellText) && cellText != "-" && (!double.TryParse(cellText, out number)))
            {
                ErrorMsg = "无法将[" + cellText + "]解析为数值(Number)";
                return false;
            }
            
            return true;
        }

        public string ErrorMsg { get; set; }

        public bool IsNullOrNot { get; set; }
    }
}
