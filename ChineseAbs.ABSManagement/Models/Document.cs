using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class Document : BaseDataContainer<TableDocument>
    {
        public Document(TableDocument document) : base(document)
        {

        }

        public int DocumentId { get; set; }

        public string Username { get; set; }

        public string ProjectGuid { get; set; }

        public string DocumentName { get; set; }

        public int DocumentTypeId { get; set; }

        public string MimeType { get; set; }

        public string Path { get; set; }

        public int Version { get; set; }

        public string Comment { get; set; }

        public string UploadTime { get; set; }

        public override void FromTableObject(TableDocument document)
        {
            DocumentId = document.document_id;
            Username = document.user_name;
            ProjectGuid = document.project_guid;
            DocumentName = document.document_name;
            MimeType = document.mime_type;
            Path = document.path;
            Version = document.version;
            Comment = document.comment;
            UploadTime = document.upload_time.ToString();
            DocumentTypeId = document.document_type_id;
        }

        public override TableDocument GetTableObject()
        {
            var tableDocument = new TableDocument {
                document_id = DocumentId,
                user_name = Username,
                project_guid = ProjectGuid,
                document_name = DocumentName,
                mime_type = MimeType,
                path = Path,
                version = Version,
                comment = Comment,
                upload_time = DateTime.Parse(UploadTime),
                document_type_id = DocumentTypeId
            };
            return tableDocument;
        }
    }
}
