using ABSMgrConn;
using ChineseAbs.ABSManagement.Loggers;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ChineseAbs.ABSManagement
{
    public class DocumentManager : BaseManager
    {
        public DocumentManager() { }

        private AbstractLogger Logger
        {
            get
            {
                return GetLogger();
            }
        }

        protected override AbstractLogger GetLogger()
        {
            return new LoggerDocument(UserInfo);
        }

        public List<Document> GetAllDocument(string projectGuid)
        {
            var documents = m_db.Query<TableDocument>(
                @"WITH Temp AS (SELECT document_name,Version FROM [dbo].[Document] WHERE project_guid = @0),
                 LatestFile AS (SELECT document_name, MAX(Version) AS max_version FROM Temp GROUP By document_name)

                SELECT d.document_id,d.document_name,d.version,d.upload_time,d.user_name,d.document_type_id FROM LatestFile l LEFT JOIN [dbo].[Document] d
                ON l.document_name = d.document_name AND l.Max_Version = d.Version WHERE project_guid = @0 ORDER BY d.upload_time DESC", projectGuid);
            return documents.ToList().ConvertAll(item => new Document(item)).ToList();
        }

        public List<Document> GetDocumentHistoryVersion(string projectGuid, string documentName)
        {
            var documents = m_db.Query<TableDocument>(
                @"SELECT document_id,version,upload_time,user_name,comment,document_type_id FROM [dbo].[Document] WHERE project_guid = @0
                          AND document_name = @1 ORDER BY version", projectGuid, documentName);
            return documents.ToList().ConvertAll(item => new Document(item)).ToList();
        }

        public Document GetDocumentById(int documentId)
        {
            var documents = m_db.Query<TableDocument>("SELECT document_name,mime_type,path,project_guid FROM [dbo].[Document] WHERE document_id = @0", documentId);
            var document = documents.Single();
            return new Document(document);
        }

        public void UploadDocument(string username, string projectGuid, string documentName, string mimeType, string path, int version, string comment, int documentTypeId)
        {
            TableDocument document = new TableDocument()
            {
                user_name = username,
                project_guid = projectGuid,
                document_name = documentName,
                mime_type = mimeType,
                path = path,
                version = version,
                comment = comment,
                upload_time = DateTime.Now,
                document_type_id = documentTypeId
            };
            m_db.Insert("Document", "document_id", true, document);

            int projectId = DbUtils.GetIdByGuid(projectGuid, "[dbo].[Project]", "project_guid");
            if (version > 1)
            {
                Logger.Log(projectId, "上传文档《" + documentName + "》新版本" + "V" + version);
            }
            else {
                Logger.Log(projectId, "上传文档《" + documentName + "》");
            }
        }

        public int GetDocumentLatestVersion(string projectGuid, string documentName)
        {
            var maxVersion = m_db.Query<int?>("SELECT MAX(version) FROM [dbo].[Document] WHERE project_guid = @0 AND document_name = @1", projectGuid, documentName);

            if (maxVersion.Single() == null)
            {
                return 0;
            }
            return (int)maxVersion.SingleOrDefault();
        }

        public List<UserLog> GetProjectDocumentOperationLogs(string projectGuid)
        {
            int projectId = DbUtils.GetIdByGuid(projectGuid, "[dbo].[Project]", "project_guid");
            var userLogs = m_db.Query<TableUserLogs>("SELECT * FROM [dbo].[UserLogs] WHERE project_id = @0 AND log_type_id = 4 ORDER BY time_stamp DESC", projectId);
            return userLogs.ToList().ConvertAll(item => new UserLog(item)).ToList();
        }

        public List<DocumentType> GetAllDocumentType()
        {
            var documentTypes = m_db.Query<TableDocumentType>(@"SELECT id, name FROM [dbo].[DocumentType] WHERE id <> 5 UNION SELECT id, name FROM [dbo].[DocumentType] WHERE id = 5");
            return documentTypes.ToList().ConvertAll(item => new DocumentType(item)).ToList();
        }

        /// <summary>
        /// Check whether document type id is legal that is passed by the front end.
        /// </summary>
        /// <param name="documentTypeId">String of document type id</param>
        /// <returns>Result of check</returns>
        public bool CheckDocumentTypeId(string documentTypeId)
        {
            int id;
            bool result = false;

            if (int.TryParse(documentTypeId, out id))
            {
                var documentTypes = GetAllDocumentType();
                foreach (var documentType in documentTypes)
                {
                    if (documentType.DocumentTypeId == id)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public string GetDocumentTypeName(int documentTypeId)
        {
            return m_db.FirstOrDefault<string>(@"SELECT name FROM [DocumentType] WHERE id = @0", documentTypeId);
        }

        public List<Document> GetAllDocumentByType(int documentTypeId, string projectGuid)
        {
            var documents = m_db.Query<TableDocument>(
                @"WITH Temp AS (SELECT document_name,Version FROM [dbo].[Document] WHERE project_guid = @0),
                 LatestFile AS (SELECT document_name, MAX(Version) AS max_version FROM Temp GROUP By document_name)

                SELECT d.document_id,d.document_name,d.version,d.upload_time,d.user_name,d.document_type_id FROM LatestFile l LEFT JOIN [dbo].[Document] d
                ON l.document_name = d.document_name AND l.Max_Version = d.Version WHERE project_guid = @0 AND d.document_type_id  = @1
                ORDER BY d.upload_time DESC", projectGuid, documentTypeId);
            return documents.ToList().ConvertAll(item => new Document(item)).ToList();
        }

        public string GetProjectGuidById(int documentId)
        {
            return m_db.FirstOrDefault<string>("SELECT project_guid FROM [dbo].[Document] WHERE document_id = @0", documentId);
        }
    }
}
