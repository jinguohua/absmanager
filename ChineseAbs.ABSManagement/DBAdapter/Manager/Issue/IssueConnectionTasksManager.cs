
using ChineseAbs.ABSManagement.Models;
using System.Collections.Generic;
using System.Linq;


namespace ChineseAbs.ABSManagement.Manager
{
    public class IssueConnectionTasksManager
        : BaseModelManager<IssueConnectionTasks, ABSMgrConn.TableIssueConnectionTasks>
    {
        public IssueConnectionTasksManager()
        {
            m_defaultTableName = "dbo.IssueConnectionTasks";
            m_defaultPrimaryKey = "issue_connection_tasks_id";
            m_defalutFieldPrefix = "issue_connection_tasks_";
        }

        public List<IssueConnectionTasks> GetConnectionTasksByIssueId(int issueId)
        {
            var tasks = Select<ABSMgrConn.TableIssueConnectionTasks>(m_defaultTableName, "issue_id", issueId);
            return tasks.Select(x => new IssueConnectionTasks(x)).ToList();
        }

        public List<IssueConnectionTasks> GetConnectionTasksByShortCodes(List<string> shortCodes)
        {
            if (shortCodes.Count == 0)
            {
                return new List<IssueConnectionTasks>();
            }

            var tasks = Select<ABSMgrConn.TableIssueConnectionTasks, string>(m_defaultTableName, "task_short_code", shortCodes, null);
            return tasks.Select(x => new IssueConnectionTasks(x)).ToList();
        }

        public List<IssueConnectionTasks> GetConnectionTasksByShortCode(string shortCode)
        {
            var tasks = Select<ABSMgrConn.TableIssueConnectionTasks>(m_defaultTableName, "task_short_code", shortCode);
            return tasks.Select(x => new IssueConnectionTasks(x)).ToList();
        }

        public int DeleteConnectionTasks(List<IssueConnectionTasks> issueConnectionTasks)
        {
            if (issueConnectionTasks.Count == 0)
            {
                return 0;
            }

            var ids = issueConnectionTasks.Select(x => x.Id);
            var sql = "delete from " + m_defaultTableName + " where " + m_defaultPrimaryKey + " in (@values)";
            return m_db.Execute(sql, new { values = ids });
        }
    }
}
