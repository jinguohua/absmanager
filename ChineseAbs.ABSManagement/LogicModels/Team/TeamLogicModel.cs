using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.LogicModels.Team
{
    public class TeamLogicModel : BaseLogicModel
    {
        public TeamLogicModel(ProjectLogicModel project)
            :base(project)
        {
            Initialize();
        }

        public List<TeamerLogicModel> Chiefs
        {
            get
            {
                if (m_chiefs == null)
                {
                    m_chiefs = new List<TeamerLogicModel>();

                    if (!string.IsNullOrWhiteSpace(m_projectSeries.PersonInCharge))
                    {
                        m_chiefs.Add(new TeamerLogicModel(m_projectSeries.PersonInCharge, TeamerType.Chief));
                    }

                    if (!string.IsNullOrWhiteSpace(m_projectSeries.CreateUserName))
                    {
                        m_chiefs.Add(new TeamerLogicModel(m_projectSeries.CreateUserName, TeamerType.Chief));
                    }
                }

                return m_chiefs;
            }
        }

        private List<TeamerLogicModel> m_chiefs;

        //public List<TeamerLogicModel> Admins
        //{
        //    get
        //    {
        //        if (m_admins == null)
        //        {
        //            //TODO:
        //        }

        //        return m_admins;
        //    }
        //}

        //private List<TeamerLogicModel> m_admins;

        //public List<TeamerLogicModel> Members
        //{
        //    get
        //    {
        //        return m_members;
        //    }
        //}

        //private List<TeamerLogicModel> m_members;

        private void Initialize()
        {
            CommUtils.AssertNotNull(m_project.Instance.ProjectSeriesId, "ProjectSeriesId is null");

            if (m_projectSeries == null)
            {
                m_projectSeries = m_dbAdapter.ProjectSeries.GetById(m_project.Instance.ProjectSeriesId.Value);
            }
        }

        private ProjectSeries m_projectSeries;
    }
}
