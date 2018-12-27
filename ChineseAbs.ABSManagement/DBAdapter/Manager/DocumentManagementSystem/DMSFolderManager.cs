using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.DocumentManagementSystem
{
    public class DMSFolderManager : BaseManager
    {
        public DMSFolderManager()
        {
            m_defaultTableName = "[dbo].[DMSFolder]";
            m_defaultPrimaryKey = "dms_folder_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        
        public List<DMSFolder> GetByDMSId(int dmsId)
        {
            var records = Select<ABSMgrConn.TableDMSFolder>("dms_id", dmsId);
            return records.Select(x => new DMSFolder(x)).ToList();
        }

        public DMSFolder GetById(int id)
        {
            var folder = SelectSingle<ABSMgrConn.TableDMSFolder>(id);
            return new DMSFolder(folder);
        }

        public List<DMSFolder> GetByIds(IEnumerable<int> ids)
        {
            var records = Select<ABSMgrConn.TableDMSFolder, int>(ids);
            return records.Select(x => new DMSFolder(x)).ToList();
        }

        public DMSFolder GetByGuid(string guid)
        {
            var folder = SelectSingle<ABSMgrConn.TableDMSFolder>("dms_folder_guid", guid);
            return new DMSFolder(folder);
        }

        public DMSFolder Create(DMSFolder folder)
        {
            folder.Guid = System.Guid.NewGuid().ToString();
            folder.RecordStatus = Models.RecordStatus.Valid;

            var id = Insert(folder.GetTableObject());
            folder.Id = id;
            return folder;
        }

        public DMSFolder Update(DMSFolder folder)
        {
            folder.LastModifyTime = DateTime.Now;
            folder.LastModifyUserName = UserInfo.UserName;
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, folder.GetTableObject());
            return folder;
        }

        public void Remove(DMSFolder folder)
        {
            folder.RecordStatus = RecordStatus.Deleted;
            folder.LastModifyTime = System.DateTime.Now;
            folder.LastModifyUserName = UserInfo.UserName;
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, folder.GetTableObject());
        }
    }
}
