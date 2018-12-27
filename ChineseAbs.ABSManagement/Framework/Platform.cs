using ChineseAbs.ABSManagement.LocalRepository;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Utils;
using System;

namespace ChineseAbs.ABSManagement.Framework
{
    public class Platform : SAFS.Core.Dependency.ILifetimeScopeDependency
    {
        public Platform()
        {
        }

        public ProjectLogicModel GetProject(int projectId)
        {
            return new ProjectLogicModel(UserName, projectId);
        }

        public ProjectLogicModel GetProject(string projectGuid)
        {
            return new ProjectLogicModel(UserName, projectGuid);
        }

        public UserProfileLoader UserProfile
        {
            get
            {
                return m_userProfileLoader;
            }
            set
            {
                m_userProfileLoader = value;
            }
        }

        private UserProfileLoader m_userProfileLoader;

        public User User
        {
            get
            {
                if (m_user == null)
                {
                    m_user = new User(UserName);
                }

                return m_user;
            }
        }

        private User m_user;


        public Repository Repository
        {
            get
            {
                if (m_repository == null)
                {
                    m_repository = new Repository();
                }

                return m_repository;
            }
        }

        private Repository m_repository;

        public DocumentFactory.DocumentFactory DocFactory
        {
            get
            {
                if (m_docFacrory == null)
                {
                    m_docFacrory = new DocumentFactory.DocumentFactory(UserName);
                }

                return m_docFacrory;
            }
        }

        private DocumentFactory.DocumentFactory m_docFacrory;

        public static string UserName
        {
            get
            {
                return SAFS.Core.Context.ApplicationContext.Current.Operator.Name;
            }
        }
    }
}
