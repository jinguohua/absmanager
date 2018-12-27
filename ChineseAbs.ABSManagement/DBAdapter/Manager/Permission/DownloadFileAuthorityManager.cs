using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager
{
    public class DownloadFileAuthorityManager : BaseManager
    {
        public DownloadFileAuthorityManager()
        {
            m_defaultTableName = "[dbo].[DownloadFileAuthority]";
            m_defaultPrimaryKey = "download_file_authority_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public List<DownloadFileAuthority> GetByProjectId(int projectId)
        {
            var records = Select<ABSMgrConn.TableDownloadFileAuthority>("project_id", projectId);
            return records.Select(x => new DownloadFileAuthority(x)).ToList();
        }

        public DownloadFileAuthority Update(DownloadFileAuthority downloadFileAuthority)
        {
            downloadFileAuthority.LastModifyTime = System.DateTime.Now;
            downloadFileAuthority.LastModifyUserName = UserInfo.UserName;
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, downloadFileAuthority.GetTableObject(), downloadFileAuthority.Id);
            return downloadFileAuthority;
        }

        public DownloadFileAuthority Create(int projectId, string userName, DownloadFileAuthorityType authorityType)
        {
            var now = System.DateTime.Now;
            var authority = new DownloadFileAuthority { 
                Guid = System.Guid.NewGuid().ToString(),
                ProjectId = projectId,
                UserName = userName,
                DownloadFileAuthorityType = authorityType,
                CreateTime = now,
                CreateUserName = UserInfo.UserName,
                LastModifyTime = now,
                LastModifyUserName = UserInfo.UserName,
                RecordStatus = Models.RecordStatus.Valid
            };

            authority.Id = (int)m_db.Insert(m_defaultTableName, m_defaultPrimaryKey, true, authority.GetTableObject());
            return authority;
        }
    }
}
