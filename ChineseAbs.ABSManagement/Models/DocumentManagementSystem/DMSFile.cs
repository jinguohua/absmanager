using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models.DocumentManagementSystem
{
    public class DMSFile : BaseDataContainer<TableDMSFile>
    {
        public DMSFile()
        {
        }

        public DMSFile(TableDMSFile dmsFile)
            : base(dmsFile)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int DMSId { get; set; }

        public int RepoFileId { get; set; }

        public string Name { get; set; }

        public int Version { get; set; }

        public string Description { get; set; }

        public long Size { get; set; }

        public int DMSFileSeriesId { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableDMSFile GetTableObject()
        {
            var obj = new TableDMSFile();
            obj.dms_file_id = Id;
            obj.dms_file_guid = Guid;
            obj.dms_id = DMSId;
            obj.file_id = RepoFileId;
            obj.name = Name;
            obj.version = Version;
            obj.description = Description;
            obj.size = Size;
            obj.dms_file_series_id = DMSFileSeriesId;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableDMSFile obj)
        {
            Id = obj.dms_file_id;
            Guid = obj.dms_file_guid;
            DMSId = obj.dms_id;
            RepoFileId = obj.file_id;
            Name = obj.name;
            Version = obj.version;
            Description = obj.description;
            Size = obj.size;
            DMSFileSeriesId = obj.dms_file_series_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }

    public class GetHighLightKeywords
    {
        public string Keyword { set; get; }

        public bool isHighLight { set; get; }
    }
}
