using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.DocumentManagementSystem
{
    public class DMSManager : BaseManager
    {
        public DMSManager()
        {
            m_defaultTableName = "[dbo].[DMS]";
            m_defaultPrimaryKey = "dms_id";
            m_defaultHasRecordStatusField = true;
            m_dbAdapter = new DBAdapter();
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public DMS GetById(int dmsId)
        {
            var record = SelectSingle<ABSMgrConn.TableDMS>(dmsId);
            return new DMS(record);
        }

        public List<DMS> GetByProjectId(int projectId)
        {
            var records = Select<ABSMgrConn.TableDMS>("project_id", projectId);
            return records.Select(x => new DMS(x)).ToList();
        }

        public List<DMS> GetByUid(DMSType dmsType, string uid)
        {
            var dmsList = new List<DMS>();
            if (dmsType == DMSType.Task)
            {
                var dmstasks = m_dbAdapter.DMSTask.GetByShortCode(uid);
                dmstasks.ForEach(x => { dmsList.Add(GetById(x.DmsId)); });
            }
            else if (dmsType == DMSType.DurationManagementPlatform)
            {
                var dmsDuration = m_dbAdapter.DMSDuration.GetByProjectId(uid);
                dmsDuration.ForEach(x => { dmsList.Add(GetById(x.DmsId)); });
            }
            return dmsList;
        }

        public DMS Create(int projectId)
        {
            var dms = new DMS();
            dms.ProjectId = projectId;
            dms.Guid = System.Guid.NewGuid().ToString();
            dms.RecordStatus = RecordStatus.Valid;
            dms.Id = (int)m_db.Insert(m_defaultTableName, m_defaultPrimaryKey, dms.GetTableObject());
            return dms;
        }

        public DMS Create(DMSType dmsType)
        {
            var dms = new DMS();
            dms.ProjectId = (int)dmsType;
            dms.Guid = System.Guid.NewGuid().ToString();
            dms.RecordStatus = RecordStatus.Valid;
            dms.Id = (int)m_db.Insert(m_defaultTableName, m_defaultPrimaryKey, dms.GetTableObject());
            return dms;
        }

        private DBAdapter m_dbAdapter;
    }
}
