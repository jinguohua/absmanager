using ChineseAbs.ABSManagement.Models.TimeRuleModel;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement.Manager.TimeRuleManager
{
    public class MetaTaskManager
        : BaseModelManager<MetaTask, ABSMgrConn.TableMetaTask>
    {
        public MetaTaskManager()
        {
            m_defaultTableName = "dbo.MetaTask";
            m_defaultPrimaryKey = "meta_task_id";
            m_defalutFieldPrefix = "meta_task_";
        }

        public List<MetaTask> GetMetaTaskByProjectId(int projectId)
        {
            var records = Select<ABSMgrConn.TableMetaTask>("project_id",projectId);
            return records.ToList().ConvertAll(x => new MetaTask(x));
        }

        public List<MetaTask> GetMetaTaskByGuids(List<string> prevMetaTaskGuidList)
        {
            if (prevMetaTaskGuidList.Count > 0)
            {
                var records = Select<ABSMgrConn.TableMetaTask,string>("meta_task_guid", prevMetaTaskGuidList);

                if (records.Count() > 0)
                {
                    return records.ToList().ConvertAll(x => new MetaTask(x));
                }
            }
            return new List<MetaTask>();
        }
    }
}
