using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.LogicModels.Authority
{
    public class DownloadFileAuthorityLogicModel : BaseLogicModel
    {
        public DownloadFileAuthorityLogicModel(ProjectLogicModel project)
            : base(project)
        {
        }

        private void LoadAuthorities()
        {
            m_authorities = m_dbAdapter.DownloadFileAuthority.GetByProjectId(ProjectLogicModel.Instance.ProjectId);
            var allUsers = m_dbAdapter.Authority.GetAllAuthedAccount();
            var undefinedAuthorityUserNames = new List<string>();
            foreach (var user in allUsers)
            {
                if (m_authorities.Any(x => x.UserName.Equals(user.UserName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }

                m_authorities.Add(new DownloadFileAuthority
                {
                    ProjectId = ProjectLogicModel.Instance.ProjectId,
                    UserName = user.UserName,
                    DownloadFileAuthorityType = DownloadFileAuthorityType.AllAllowed,
                });
            }

            m_authorities = m_authorities.OrderBy(x => x.UserName).ToList();
        }

        public DownloadFileAuthorityType CurrentUserAuthority
        {
            get
            {
                if (m_authorities == null)
                {
                    LoadAuthorities();
                }

                CommUtils.Assert(m_authorities.Any(x => x.UserName.Equals(m_userName, StringComparison.CurrentCultureIgnoreCase)
                    && ProjectLogicModel.Instance.ProjectId == x.ProjectId), "查找用户[{0}]的下载文件权限失败", m_userName);

                var authority = m_authorities.Single(x => x.UserName.Equals(m_userName, StringComparison.CurrentCultureIgnoreCase)
                    && ProjectLogicModel.Instance.ProjectId == x.ProjectId);
                return authority.DownloadFileAuthorityType;
            }
        }

        public List<DownloadFileAuthority> Authorities
        {
            get
            {
                if (m_authorities == null)
                {
                    LoadAuthorities();
                }

                return m_authorities;
            }
        }

        List<DownloadFileAuthority> m_authorities;
    }
}
