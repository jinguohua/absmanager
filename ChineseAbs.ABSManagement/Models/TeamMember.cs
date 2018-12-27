using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TeamMember : BaseDataContainer<TableTeamMember>
    {
        public TeamMember()
        {
        }

        public TeamMember(TableTeamMember teamMember)
            : base(teamMember)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public string UserName { get; set; }

        public bool Read { get; set; }

        public bool Write { get; set; }

        public bool Execute { get; set; }

        public override TableTeamMember GetTableObject()
        {
            var obj = new TableTeamMember();
            obj.team_member_id = Id;
            obj.team_member_guid = Guid;
            obj.project_id = ProjectId;
            obj.user_name = UserName;
            obj.can_read = Read;
            obj.can_write = Write;
            obj.can_execute = Execute;
            return obj;
        }

        public override void FromTableObject(TableTeamMember obj)
        {
            Id = obj.team_member_id;
            Guid = obj.team_member_guid;
            ProjectId = obj.project_id;
            UserName = obj.user_name;
            Read = obj.can_read;
            Write = obj.can_write;
            Execute = obj.can_execute;
        }
    }
}
