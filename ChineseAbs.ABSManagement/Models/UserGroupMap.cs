using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class UserGroupMap : BaseModel<TableUserGroupMap>
    {
        public UserGroupMap()
        {

        }

        public UserGroupMap(TableUserGroupMap obj)
            : base(obj)
        {

        }
        public string UserGroupGuid { get; set; }
        public string UserName { get; set; }

        public override TableUserGroupMap GetTableObject()
        {
            var obj = new TableUserGroupMap();
            obj.user_group_map_id = Id;
            obj.user_group_map_guid = Guid;
            obj.user_group_guid = UserGroupGuid;
            obj.user_name = UserName;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableUserGroupMap obj)
        {
            Id = obj.user_group_map_id;
            Guid = obj.user_group_map_guid;
            UserGroupGuid = obj.user_group_guid;
            UserName = obj.user_name;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
