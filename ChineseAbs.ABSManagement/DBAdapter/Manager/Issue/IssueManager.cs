using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public class IssueManager : BaseManager
    {
        public IssueManager()
        {
            m_defaultTableName = "dbo.Issue";
            m_defaultPrimaryKey = "issue_id";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public List<Issue> GetIssueListByProjectId(int projectId)
        {
            var records = m_db.Fetch<ABSMgrConn.TableIssue>(
                "SELECT * FROM " + m_defaultTableName
                + " WHERE project_id = @0 and record_status_id = @1"
                + " ORDER BY issue_emergency_level_id DESC, create_time", projectId, (int)RecordStatus.Valid);

            return records.ToList().ConvertAll(x => new Issue(x));
        }

        public Issue GetIssueByImageId(int imageId)
        {
            var sql = "select * from Issue a left join issueActivity b on a.issue_id = b.issue_id"+
                " left join IssueActivityImage c on b.issue_activity_id = c.issue_activity_id"+
                " where c.image_id =" + imageId;
            var records = m_db.Query<ABSMgrConn.TableIssue>(sql);
            if (records.Count() == 0)
            {
                return new Issue();
            }

            if (records.Count() > 1)
            {
                throw new ApplicationException("Get data failed,data of number greater than two,please contact the admin.");
            }

            return new Issue(records.First());
        }

        public Issue GetIssueByFileId(int fileId)
        {
            var sql = "select a.* from Issue a left join issueActivity b on a.issue_id = b.issue_id" +
                " left join IssueActivityFile c on b.issue_activity_id = c.issue_activity_id" +
                " where c.file_id =" + fileId;
            var records = m_db.Query<ABSMgrConn.TableIssue>(sql);
            if (records.Count() == 0)
            {
                return new Issue();
            }

            if (records.Count() > 1)
            {
                throw new ApplicationException("Get data failed,data of number greater than two,please contact the admin.");
            }

            return new Issue(records.First());
        }

        public Issue GetIssueByIssueGuid(string issueGuid)
        {
            var record = SelectSingle<ABSMgrConn.TableIssue>("issue_guid", issueGuid);

            return new Issue(record);
        }

        public Issue GetIssueByIssueId(int issueId)
        {
            var record = SelectSingle<ABSMgrConn.TableIssue>("issue_id", issueId);

            return new Issue(record);
        }

        public Issue NewIssue(Issue issue)
        {
            issue.IssueGuid = Guid.NewGuid().ToString();

            var newId = Insert(issue.GetTableObject());
            issue.Id = newId;
            return issue;
        }

        public int UpdateIssue(Issue issue)
        {
            var issueTable = issue.GetTableObject();
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, issueTable, issue.Id);
        }

        public int DeleteIssue(Issue issue)
        {
            issue.RecordStatus = RecordStatus.Deleted;
            return UpdateIssue(issue);
        }

        public List<Issue> GetByGuids(List<string> issueGuids)
        {
            m_defaultHasRecordStatusField = true;
            var records = Select<ABSMgrConn.TableIssue, string>("issue_guid", issueGuids);
            return records.ToList().ConvertAll(x => new Issue(x));
        }

        public List<Issue> GetIssueByIssueIds(List<int> issueIds)
        {
            m_defaultHasRecordStatusField = true;
            var records = Select<ABSMgrConn.TableIssue, int>("issue_id", issueIds);
            return records.ToList().ConvertAll(x => new Issue(x));
        }
    }
}
