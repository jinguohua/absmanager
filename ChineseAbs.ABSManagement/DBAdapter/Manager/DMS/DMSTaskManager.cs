
using ABSMgrConn;
using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager
{
    public class DMSTaskManager
        : BaseModelManager<DMSTask, ABSMgrConn.TableDMSTask>
    {
        public DMSTaskManager()
        {
            m_defaultTableName = "dbo.DMSTask";
            m_defaultPrimaryKey = "dms_task_id";
            m_defalutFieldPrefix = "dms_task_";
            m_defaultHasRecordStatusField = true;
            m_dbAdapter = new DBAdapter();
        }

        public List<DMSTask> GetByShortCode(string shortCode)
        {
            var DmsTasks = Select<TableDMSTask>("short_code", shortCode);
            return DmsTasks.Select(x=>new DMSTask(x)).ToList();
        }

        public int GetProjectIdByDMSId(int DMSId)
        {
            var DmsTask =new DMSTask( SelectSingle<TableDMSTask>("dms_id", DMSId));
            var task = m_dbAdapter.Task.GetTask(DmsTask.ShortCode);
            return task.ProjectId;
        }

        DBAdapter m_dbAdapter;
    }
}
