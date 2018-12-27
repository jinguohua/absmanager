using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;

namespace ChineseAbs.ABSManagement
{
    public class TeamMemberManager : BaseManager
    {
        public TeamMemberManager()
        {
            m_defaultTableName = "dbo.TeamMember";
            m_defaultPrimaryKey = "team_member_id";
            m_defaultOrderBy = "";
        }

        public List<TeamMember> GetByProjectId(int projectId)
        {
            var teamMembers = Select<ABSMgrConn.TableTeamMember>("project_id", projectId);
            return teamMembers.ToList().ConvertAll(x => new TeamMember(x));
        }

        public bool Exists(string teamMemberGuid)
        {
            return Exists<ABSMgrConn.TableTeamMember>("team_member_guid", teamMemberGuid);
        }

        public TeamMember GetByGuid(string teamMemberGuid)
        {
            var record = SelectSingle<ABSMgrConn.TableTeamMember>("team_member_guid", teamMemberGuid);
            return new TeamMember(record);
        }

        public int Remove(TeamMember teamMember)
        {
            return m_db.Delete(m_defaultTableName, m_defaultPrimaryKey, teamMember.GetTableObject());
        }

        public TeamMember Add(TeamMember teamMember)
        {
            teamMember.Guid = Guid.NewGuid().ToString();
            teamMember.Id = Insert(teamMember.GetTableObject());
            return teamMember;
        }

        public bool IsTeamMember(int projectId, string userName)
        {
            var teamMembers = GetByProjectId(projectId);
            return teamMembers.Any(x => x.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }
    }
}
