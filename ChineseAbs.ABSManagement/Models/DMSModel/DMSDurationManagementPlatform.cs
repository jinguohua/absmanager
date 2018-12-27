using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class DMSDurationManagementPlatform : BaseModel<TableDMSDurationManagementPlatform>
    {
        public DMSDurationManagementPlatform()
        {

        }

        public DMSDurationManagementPlatform(TableDMSDurationManagementPlatform obj)
            : base(obj)
        {

        }
        public int DmsDurationManagementPlatformId { get; set; }
        public string DmsDurationManagementPlatformGuid { get; set; }
        public int ProjectId { get; set; }
        public int DmsId { get; set; }

        public override TableDMSDurationManagementPlatform GetTableObject()
        {
            var obj = new TableDMSDurationManagementPlatform();
            obj.dms_duration_management_platform_id = Id;
            obj.dms_duration_management_platform_guid = Guid;
            obj.dms_duration_management_platform_id = DmsDurationManagementPlatformId;
            obj.dms_duration_management_platform_guid = DmsDurationManagementPlatformGuid;
            obj.project_id = ProjectId;
            obj.dms_id = DmsId;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableDMSDurationManagementPlatform obj)
        {
            Id = obj.dms_duration_management_platform_id;
            Guid = obj.dms_duration_management_platform_guid;
            DmsDurationManagementPlatformId = obj.dms_duration_management_platform_id;
            DmsDurationManagementPlatformGuid = obj.dms_duration_management_platform_guid;
            ProjectId = obj.project_id;
            DmsId = obj.dms_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
