using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils.Excel
{
    interface ICellJudge
    {
        Dictionary<int, object> ExceptCellByColumn(List<List<object>> table, int iRow);
    }
}
