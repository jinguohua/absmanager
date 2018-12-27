using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class UserGroup : BaseModel<TableUserGroup>
    {
        public UserGroup()
        {

        }

        public UserGroup(TableUserGroup obj)
            : base(obj)
        {

        }
        public string Name { get; set; }
        public string Owner { get; set; }

        public override TableUserGroup GetTableObject()
        {
            var obj = new TableUserGroup();
            obj.user_group_id = Id;
            obj.user_group_guid = Guid;
            obj.name = Name;
            obj.owner = Owner;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableUserGroup obj)
        {
            Id = obj.user_group_id;
            Guid = obj.user_group_guid;
            Name = obj.name;
            Owner = obj.owner;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
