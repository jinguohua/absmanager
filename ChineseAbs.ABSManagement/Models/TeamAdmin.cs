using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TeamAdmin : BaseModel<TableTeamAdmin>
    {
        public TeamAdmin()
        {

        }

        public TeamAdmin(TableTeamAdmin obj)
            : base(obj)
        {

        }
        public int ProjectId { get; set; }
        public string UserName { get; set; }

        public override TableTeamAdmin GetTableObject()
        {
            var obj = new TableTeamAdmin();
            obj.team_admin_id = Id;
            obj.team_admin_guid = Guid;
            obj.project_id = ProjectId;
            obj.user_name = UserName;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTeamAdmin obj)
        {
            Id = obj.team_admin_id;
            Guid = obj.team_admin_guid;
            ProjectId = obj.project_id;
            UserName = obj.user_name;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
