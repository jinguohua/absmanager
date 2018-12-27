namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class ReinvestmentCsv
    {

        public ReinvestmentCsv()
        {
        }

        private string m_filePath;

        public void Load(string filePath)
        {
            m_filePath = filePath;
        }
    }
}
