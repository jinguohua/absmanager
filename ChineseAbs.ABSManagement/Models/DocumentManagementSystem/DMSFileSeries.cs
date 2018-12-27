using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models.DocumentManagementSystem
{
    public class DMSFileSeries : BaseDataContainer<TableDMSFileSeries>
    {
        public DMSFileSeries()
        {
        }

        public DMSFileSeries(TableDMSFileSeries dmsFileSeries)
            : base(dmsFileSeries)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int DMSId { get; set; }

        public string Name { get; set; }

        public int DMSFolderId { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableDMSFileSeries GetTableObject()
        {
            var obj = new TableDMSFileSeries();
            obj.dms_file_series_id = Id;
            obj.dms_file_series_guid = Guid;
            obj.dms_id = DMSId;
            obj.dms_file_series_name = Name;
            obj.dms_folder_id = DMSFolderId;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableDMSFileSeries obj)
        {
            Id = obj.dms_file_series_id;
            Guid = obj.dms_file_series_guid;
            DMSId = obj.dms_id;
            Name = obj.dms_file_series_name;
            DMSFolderId = obj.dms_folder_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
