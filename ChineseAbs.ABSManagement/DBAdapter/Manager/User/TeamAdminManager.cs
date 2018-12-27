using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager
{
    public class TeamAdminManager
        : BaseModelManager<TeamAdmin, ABSMgrConn.TableTeamAdmin>
    {
        public TeamAdminManager()
        {
            m_defaultTableName = "dbo.TeamAdmin";
            m_defaultPrimaryKey = "team_admin_id";
            m_defalutFieldPrefix = "team_admin_";
            m_defaultOrderBy = " ORDER BY create_time DESC";
        }

        public List<TeamAdmin> GetByProjectId(int projectId)
        {
            var records = Select<ABSMgrConn.TableTeamAdmin>("project_id", projectId);
            return records.Select(x => new TeamAdmin(x)).ToList();
        }

        public bool IsTeamAdmin(int projectId, string userName)
        {
            var teamAdmins = GetByProjectId(projectId);
            return teamAdmins.Any(x => x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
