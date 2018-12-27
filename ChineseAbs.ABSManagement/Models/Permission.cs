using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public enum PermissionType
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }

    public enum PermissionObjectType
    {
        None = 0,
        ProjectSeries = 1,
        TaskGroup = 2,
        Task = 3,
        Project = 4,
    }

    public class Permission: BaseDataContainer<TablePermission>
    {
        public Permission()
        {
        }

        public Permission(TablePermission permission)
            : base(permission)
        {

        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public PermissionObjectType ObjectType { get; set; }
        
        public string ObjectUniqueIdentifier { get; set; }

        public PermissionType Type { get; set; }

        public override TablePermission GetTableObject()
        {
            var obj = new TablePermission();
            obj.permission_id = Id;
            obj.user_name = UserName;
            obj.permission_object_type_id = (int)ObjectType;
            obj.permission_object_unique_identifier = ObjectUniqueIdentifier;
            obj.permission_type = (int)Type;
            return obj;
        }

        public override void FromTableObject(TablePermission obj)
        {
            Id = obj.permission_id;
            UserName = obj.user_name;
            ObjectType = (PermissionObjectType)obj.permission_object_type_id;
            ObjectUniqueIdentifier = obj.permission_object_unique_identifier;
            Type = (PermissionType)obj.permission_type;
        }
    }
}
