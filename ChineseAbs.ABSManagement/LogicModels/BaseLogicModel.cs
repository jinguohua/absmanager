using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.LogicModels
{
    public class BaseLogicModel : SAFS.Core.Dependency.ILifetimeScopeDependency
    {
        public BaseLogicModel(ProjectLogicModel project)
        {
            CommUtils.AssertNotNull(project.UserName, "Project's user info can't be null in BaseLogicModel initialization.");
            m_project = project;
            m_userName = project.UserName;
        }

        public BaseLogicModel(string userName, ProjectLogicModel project)
        {
            m_project = project;
            m_userName = userName;
        }

        public string UserName { get { return m_userName; } }

        public ProjectLogicModel ProjectLogicModel { get { return m_project; } }

        protected DBAdapter m_dbAdapter;
        public DBAdapter DBAdapter
        {
            get { return m_dbAdapter; }
            set
            {
                m_dbAdapter = value;
            }
        }

        protected ProjectLogicModel m_project;

        protected string m_userName;
    }
}
