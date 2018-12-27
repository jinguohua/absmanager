
namespace ChineseAbs.ABSManagement.Utils.Excel
{
    public class CellTextValidation : ICellValidation
    {
        public CellTextValidation(int minLength = -1, int maxLength = 100000)
        {
            m_minLength = minLength;
            m_maxLength = maxLength;
        }

        public bool IsValid(string cellText)
        {
            if (cellText == null)
            {
                ErrorMsg = "文本不能为空";
                return false;
            }

            if (cellText.Length < m_minLength)
            {
                ErrorMsg = "文本长度不能小于[" + m_minLength + "]";
                return false;
            }

            if (cellText.Length > m_maxLength)
            {
                ErrorMsg = "文本长度不能超过[" + m_maxLength + "]";
                return false;
            }

            ErrorMsg = string.Empty;
            return true;
        }

        public string ErrorMsg { get; set; }

        private int m_minLength;
        private int m_maxLength;
    }
}
