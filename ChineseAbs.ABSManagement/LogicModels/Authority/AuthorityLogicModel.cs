namespace ChineseAbs.ABSManagement.LogicModels.Authority
{
    public class AuthorityLogicModel : BaseLogicModel
    {
        public AuthorityLogicModel(ProjectLogicModel project)
            : base(project)
        {
        }

        public DownloadFileAuthorityLogicModel DownloadFile
        {
            get
            {
                if (m_downloadFile == null)
                {
                    m_downloadFile = new DownloadFileAuthorityLogicModel(ProjectLogicModel);
                }

                return m_downloadFile;
            }
        }

        private DownloadFileAuthorityLogicModel m_downloadFile;
    }
}
