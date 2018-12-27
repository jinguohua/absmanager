
namespace ChineseAbs.ABSManagement.Utils.Excel.Structure
{
    public class Cell
    {
        public Cell(object cellObj)
        {
            m_cellObj = cellObj;
        }

        public bool IsEmpty
        {
            get
            {
                if (m_cellObj == null)
                {
                    return true;
                }
                var cellString = m_cellObj.ToString();

                return string.IsNullOrWhiteSpace(cellString)
                    || cellString.Trim() == "-";
            }
        }
        public string GetCellValue()
        {
            return m_cellObj.ToString();
        }

        private object m_cellObj;
    }
}
