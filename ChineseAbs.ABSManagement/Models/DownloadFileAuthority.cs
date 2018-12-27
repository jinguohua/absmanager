using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public enum DownloadFileAuthorityType
    {
        //禁止下载任何文件
        AllForbidden = 1,
        //可以下载任何文件
        AllAllowed = 2,
        //下载时，word文件自动转pdf
        Word2Pdf = 3,
        //下载时，word文件自动转pdf(加水印)
        Word2PdfWithWatermark = 4,
    }

    public class DownloadFileAuthority: BaseDataContainer<TableDownloadFileAuthority>
    {
        public DownloadFileAuthority()
        {
        }

        public DownloadFileAuthority(TableDownloadFileAuthority downloadFileAuthority)
            : base(downloadFileAuthority)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public string UserName { get; set; }

        public DownloadFileAuthorityType DownloadFileAuthorityType { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public DateTime LastModifyTime { get; set; }

        public string LastModifyUserName { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableDownloadFileAuthority GetTableObject()
        {
            var obj = new TableDownloadFileAuthority();
            obj.download_file_authority_id = Id;
            obj.download_file_authority_guid = Guid;
            obj.project_id = ProjectId;
            obj.user_name = UserName;
            obj.download_file_authority_type_id = (int)DownloadFileAuthorityType;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableDownloadFileAuthority obj)
        {
            Id = obj.download_file_authority_id;
            Guid = obj.download_file_authority_guid;
            ProjectId = obj.project_id;
            UserName = obj.user_name;
            DownloadFileAuthorityType = (DownloadFileAuthorityType)obj.download_file_authority_type_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
