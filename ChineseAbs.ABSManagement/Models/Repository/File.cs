using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models.Repository
{
    public class File : BaseDataContainer<TableFile>
    {
        public File()
        {
        }

        public File(TableFile file)
            : base(file)
        {
        }

        public int Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUserName { get; set; }
        public DateTime LastModifyTime { get; set; }
        public string LastModifyUserName { get; set; }

        public override void FromTableObject(TableFile file)
        {
            Id = file.file_id;
            Guid = file.file_guid;
            Name = file.name;
            Path = file.path;
            CreateTime = file.create_time;
            CreateUserName = file.create_user_name;
            LastModifyTime = file.last_modify_time;
            LastModifyUserName = file.last_modify_user_name;
        }

        public override TableFile GetTableObject()
        {
            var file = new TableFile();
            file.file_id = Id;
            file.file_guid = Guid;
            file.name = Name;
            file.path = Path;
            file.create_time = CreateTime;
            file.create_user_name = CreateUserName;
            file.last_modify_time = LastModifyTime;
            file.last_modify_user_name = LastModifyUserName;
            return file;
        }
    }
}
