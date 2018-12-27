using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models.DocumentManagementSystem
{
    public enum DmsFolderType
    {
        Root = 1,
        Normal = 2,
    }

    public class DMSFolder : BaseDataContainer<TableDMSFolder>
    {
        public DMSFolder()
        {

        }

        public DMSFolder(TableDMSFolder dmsFolder)
            : base(dmsFolder)
        {

        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int DMSId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? ParentFolderId { get; set; }

        public DmsFolderType DmsFolderType { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableDMSFolder GetTableObject()
        {
            var obj = new TableDMSFolder();
            obj.dms_folder_id = Id;
            obj.dms_folder_guid = Guid;
            obj.dms_id = DMSId;
            obj.name = Name;
            obj.description = Description;
            obj.parent_folder_id = ParentFolderId;
            obj.dms_folder_type_id = (int)DmsFolderType;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableDMSFolder obj)
        {
            Id = obj.dms_folder_id;
            Guid = obj.dms_folder_guid;
            DMSId = obj.dms_id;
            Name = obj.name;
            Description = obj.description;
            ParentFolderId = obj.parent_folder_id;
            DmsFolderType = (DmsFolderType)obj.dms_folder_type_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
