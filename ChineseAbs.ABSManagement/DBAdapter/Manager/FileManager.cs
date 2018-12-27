using ABSMgrConn;
using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.Repository;

namespace ChineseAbs.ABSManagement
{
    public class FileManager : BaseManager
    {
        public FileManager()
        {
            m_defaultTableName = "[dbo].[File]";
            m_defaultPrimaryKey = "file_id";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public File New(string name, string path)
        {
            var now = DateTime.Now;

            var file = new File();
            file.Name = name;
            file.Path = path;

            file.Guid = Guid.NewGuid().ToString();
            file.CreateTime = now;
            file.CreateUserName = UserInfo.UserName;
            file.LastModifyTime = now;
            file.LastModifyUserName = UserInfo.UserName;
            file.Id = Insert(file.GetTableObject());
            return file;
        }

        public File Update(File file)
        {
            var obj = file.GetTableObject();
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, obj, obj.file_id);
            return file;
        }

        public File GetById(int id)
        {
            var record = SelectSingle<TableFile>(id);
            return new File(record);
        }

        public File GetByGuid(string guid)
        {
            var record = SelectSingle<TableFile>("file_guid", guid);
            return new File(record);
        }

        public List<File> GetFilesByIds(List<int> fileIds)
        {
            if (fileIds.Count == 0)
            {
                return new List<File>();
            }

            var files = Select<ABSMgrConn.TableFile, int>(m_defaultTableName, m_defaultPrimaryKey, fileIds, null);
            return files.Select(x => new File(x)).ToList();
        }

        public List<File> GetFilesByGuids(List<string> fileGuids)
        {
            if (fileGuids.Count == 0)
            {
                return new List<File>();
            }

            var files = Select<ABSMgrConn.TableFile, string>(m_defaultTableName, "file_guid", fileGuids, null);
            return files.Select(x => new File(x)).ToList();
        }

        public IssueActivityFile NewIssueActivityFile(int issueActivityId, int fileId)
        {
            var issueActivityFile = new IssueActivityFile();
            issueActivityFile.IssueActivityId = issueActivityId;
            issueActivityFile.FileId = fileId;

            issueActivityFile.Id = Insert("dbo.IssueActivityFile", "issue_activity_file_id", issueActivityFile.GetTableObject());
            return issueActivityFile;
        }

        public List<IssueActivityFile> GetFileIdsByIssueActivityIds(List<int> issueActivityIds)
        {
            if (issueActivityIds.Count == 0)
            {
                return new List<IssueActivityFile>();
            }

            var issueActivityFiles = Select<ABSMgrConn.TableIssueActivityFile, int>("dbo.IssueActivityFile", "issue_activity_id", issueActivityIds, null);
            return issueActivityFiles.Select(x => new IssueActivityFile(x)).ToList();
        }

        public List<IssueActivityFile> GetFileIdsByIssueActivityId(int issueActivityId)
        {
            var issueActivityFiles = Select<ABSMgrConn.TableIssueActivityFile>("dbo.IssueActivityFile", "issue_activity_id", issueActivityId);
            return issueActivityFiles.Select(x => new IssueActivityFile(x)).ToList();
        }

        public IssueActivityFile GetIssueActivityFileByFileId(int fileId)
        {
            var record = SelectSingle<TableIssueActivityFile>("dbo.IssueActivityFile", "file_id", fileId);
            return new IssueActivityFile(record);
        }

        public int DeleteIssueActivityFile(IssueActivityFile issueActivityFile)
        {
            return m_db.Delete("dbo.IssueActivityFile", "issue_activity_id", issueActivityFile.GetTableObject());
        }

        public int DeleteIssueActivityFiles(List<IssueActivityFile> issueActivityFiles)
        {
            if (issueActivityFiles.Count == 0)
            {
                return 0;
            }

            var ids = issueActivityFiles.Select(x => x.Id);
            var sql = "delete from dbo.IssueActivityFile where issue_activity_id in (@issueActivityFiles)";
            return m_db.Execute(sql, new { issueActivityFiles = ids });
        }
    }
}
