
namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class CellRange
    {
        public CellRange(int rowBegin, int rowEnd, int columnBegin, int columnEnd)
        {
            RowBegin = rowBegin;
            RowEnd = rowEnd;
            ColumnBegin = columnBegin;
            ColumnEnd = columnEnd;
        }

        public static CellRange Cell(int row, int column)
        {
            return new CellRange(row, row, column, column);
        }

        public static CellRange Row(int row)
        {
            return new CellRange(row, row, 0, 100000);
        }

        public static CellRange Row(int rowBegin, int rowEnd)
        {
            return new CellRange(rowBegin, rowEnd, 0, 100000);
        }

        public static CellRange Column(int column)
        {
            return new CellRange(0, 100000, column, column);
        }

        public static CellRange Column(int columnBegin, int columnEnd)
        {
            return new CellRange(0, 100000, columnBegin, columnEnd);
        }

        public bool Contains(int row, int column)
        {
            return row >= RowBegin && row <= RowEnd && column >= ColumnBegin && column <= ColumnEnd;
        }

        public int RowBegin { get; set; }

        public int ColumnBegin { get; set; }

        public int RowEnd { get; set; }

        public int ColumnEnd { get; set; }
    }
}
