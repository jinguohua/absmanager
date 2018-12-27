using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.DocumentManagementSystem
{
    public class DMSFileManager : BaseManager
    {
        public DMSFileManager()
        {
            m_defaultTableName = "[dbo].[DMSFile]";
            m_defaultPrimaryKey = "dms_file_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public DMSFile Create(DMSFile dmsFile)
        {
            dmsFile.Guid = System.Guid.NewGuid().ToString();
            dmsFile.RecordStatus = RecordStatus.Valid;
            dmsFile.Id = Insert(dmsFile.GetTableObject());
            return dmsFile;
        }

        public DMSFile Update(DMSFile dmsFile) {
            dmsFile.LastModifyTime = DateTime.Now;
            dmsFile.LastModifyUserName = UserInfo.UserName;
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, dmsFile.GetTableObject());
            return dmsFile;
        }

        public DMSFile GetByGuid(string dmsFileGuid)
        {
            var record = SelectSingle<ABSMgrConn.TableDMSFile>("dms_file_guid", dmsFileGuid);
            return new DMSFile(record);
        }

        public bool isExistDMSFile(string dmsFileGuid)
        {
            m_defaultHasRecordStatusField = false;
            var record = SelectSingle<ABSMgrConn.TableDMSFile>("dms_file_guid", dmsFileGuid);
            var dmsFile = new DMSFile(record);
            m_defaultHasRecordStatusField = true;

            return dmsFile.RecordStatus == RecordStatus.Valid;
        }

        public List<DMSFile> GetFilesByFileSeriesId(int fileSeriesId)
        {
            return GetFilesByFileSeriesIds(new[] { fileSeriesId });
        }

        public List<DMSFile> GetFilesByFileSeriesIds(IEnumerable<int> fileSeriesIds)
        {
            var records = Select<ABSMgrConn.TableDMSFile, int>("dms_file_series_id", fileSeriesIds);
            return records.Select(x => new DMSFile(x)).ToList();
        }

        public int Remove(DMSFile dmsFile)
        {
            dmsFile.RecordStatus = RecordStatus.Deleted;
            dmsFile.LastModifyTime = DateTime.Now;
            dmsFile.LastModifyUserName = UserInfo.UserName;
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, dmsFile.GetTableObject());
        }
    }
}
