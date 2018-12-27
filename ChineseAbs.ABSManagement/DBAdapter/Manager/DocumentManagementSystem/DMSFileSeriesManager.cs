using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.DocumentManagementSystem
{
    public class DMSFileSeriesManager : BaseManager
    {
        public DMSFileSeriesManager()
        {
            m_defaultTableName = "[dbo].[DMSFileSeries]";
            m_defaultPrimaryKey = "dms_file_series_id";
            m_defaultHasRecordStatusField = true;
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public DMSFileSeries Create(DMSFileSeries dmsFileSeries)
        {
            dmsFileSeries.Guid = System.Guid.NewGuid().ToString();
            dmsFileSeries.RecordStatus = RecordStatus.Valid;
            dmsFileSeries.Id = Insert(dmsFileSeries.GetTableObject());
            return dmsFileSeries;
        }

        public DMSFileSeries Update(DMSFileSeries dmsFileSeries)
        {
            dmsFileSeries.LastModifyTime = DateTime.Now;
            dmsFileSeries.LastModifyUserName = UserInfo.UserName;
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, dmsFileSeries.GetTableObject());
            return dmsFileSeries;
        }

        public List<DMSFileSeries> GetFileSeriesByFolderId(int dmsFolderId)
        {
            var records = Select<ABSMgrConn.TableDMSFileSeries>("dms_folder_id", dmsFolderId);
            return records.Select(x => new DMSFileSeries(x)).ToList();
        }

        public List<DMSFileSeries> GetFileSeriesByFolderIds(IEnumerable<int> dmsFolderIds)
        {
            var records = Select<ABSMgrConn.TableDMSFileSeries, int>("dms_folder_id", dmsFolderIds);
            return records.Select(x => new DMSFileSeries(x)).ToList();
        }

        public List<DMSFileSeries> GetByIds(IEnumerable<int> dmsFileSeriesIds)
        {
            var records = Select<ABSMgrConn.TableDMSFileSeries, int>("dms_file_series_id", dmsFileSeriesIds);
            return records.Select(x => new DMSFileSeries(x)).ToList();
        }

        public DMSFileSeries GetById(int dmsFileSeriesId)
        {
            var record = SelectSingle<ABSMgrConn.TableDMSFileSeries>(dmsFileSeriesId);
            return new DMSFileSeries(record);
        }

        public DMSFileSeries GetByGuid(string dmsFileSeriesGuid)
        {
            var record = SelectSingle<ABSMgrConn.TableDMSFileSeries>("dms_file_series_guid", dmsFileSeriesGuid);
            return new DMSFileSeries(record);
        }

        public List<DMSFileSeries> GetByGuids(List<string> fileSeriesGuids)
        {
            var records = Select<ABSMgrConn.TableDMSFileSeries,string>("dms_file_series_guid", fileSeriesGuids);
            return records.Select(x => new DMSFileSeries(x)).ToList();
        }

        public int Remove(IEnumerable<DMSFileSeries> dmsFileSerieList)
        {
            var removeTime = DateTime.Now;
            foreach (var dmsFileSeries in dmsFileSerieList)
            {
                dmsFileSeries.RecordStatus = RecordStatus.Deleted;
                dmsFileSeries.LastModifyTime = removeTime;
                dmsFileSeries.LastModifyUserName = UserInfo.UserName;
                m_db.Update(m_defaultTableName, m_defaultPrimaryKey, dmsFileSeries.GetTableObject());
            }
            return dmsFileSerieList.Count();
        }

        public int Remove(DMSFileSeries dmsFileSeries)
        {
            dmsFileSeries.RecordStatus = RecordStatus.Deleted;
            dmsFileSeries.LastModifyTime = DateTime.Now;
            dmsFileSeries.LastModifyUserName = UserInfo.UserName;
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, dmsFileSeries.GetTableObject());
        }
    }
}
