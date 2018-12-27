using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils.Excel.Structure
{
    public class SheetStyle
    {
        public void Add(CellRangeStyle cellRangeStyle)
        {
            if (CellRangeStyles == null)
            {
                CellRangeStyles = new List<CellRangeStyle>();
            }

            CellRangeStyles.Add(cellRangeStyle);
        }

        public List<CellRangeStyle> CellRangeStyles { get; set; }
    }
}
