using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    interface ICellComparison
    {
        bool CompareColumn(List<List<object>> table, int iRow);

        string ErrorMsg { get; set; }
    }
}
