using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class CellCustomValidation : ICellValidation
    {
        public CellCustomValidation(Func<string, string> check, bool isNullOrNot = true)
        {
            ErrorMsg = string.Empty;
            CheckCustom = check;
            IsNullOrNot = isNullOrNot;
        }

        public CellCustomValidation(Func<List<string>, string, string> check, List<string> personInCharges, bool isNullOrNot = true)
        {
            ErrorMsg = string.Empty;
            CheckPersonInCharge = check;
            PersonInCharges = personInCharges;
            IsNullOrNot = isNullOrNot;
        }

        public bool IsValid(string cellText)
        {
            if ((!IsNullOrNot) && (string.IsNullOrWhiteSpace(cellText) || cellText == "-"))
            {
                ErrorMsg = "不能为空";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(cellText) && cellText != "-")
            {
                if (CheckCustom != null)
                {
                    ErrorMsg = CheckCustom(cellText);
                }
                else if (CheckPersonInCharge != null && PersonInCharges != null)
                {
                    ErrorMsg = CheckPersonInCharge(PersonInCharges, cellText);
                }
                if (ErrorMsg != string.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        public string ErrorMsg { get; set; }

        public Func<List<string>, string, string> CheckPersonInCharge { get; set; }

        public List<string> PersonInCharges { get; set; }

        public Func<string, string> CheckCustom { get; set; }

        public bool IsNullOrNot { get; set; }
    }
}
