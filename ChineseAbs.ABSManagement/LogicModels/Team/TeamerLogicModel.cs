namespace ChineseAbs.ABSManagement.LogicModels.Team
{
    public enum TeamerType
    {
        //创建者/负责人
        Chief,
        //管理员
        Admin,
        //项目成员
        Member,
    }

    public class TeamerLogicModel
    {
        public TeamerLogicModel(string UserName, TeamerType teamerType)
        {
            m_userName = UserName;
            m_teamerType = teamerType;
        }

        public string UserName { get { return m_userName; } }

        public TeamerType TeamerType { get { return m_teamerType; } }

        private string m_userName;

        private TeamerType m_teamerType;
    }
}
