using ChineseAbs.ABSManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagement
{
    public class IssueActivityManager:BaseManager
    {
        public IssueActivityManager()
        {
            m_defaultTableName = "dbo.IssueActivity";
            m_defaultPrimaryKey = "issue_activity_id";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public IssueActivity NewIssueActivity(IssueActivity issueActivity)
        {
            issueActivity.IssueActivityGuid = Guid.NewGuid().ToString();

            var newId = Insert(issueActivity.GetTableObject());
            issueActivity.IssueActivityId = newId;

            return issueActivity;
        }

        public List<IssueActivity> GetIssueActivityListByIssueId(int issueId)
        {
            var records = m_db.Fetch<ABSMgrConn.TableIssueActivity>(
                "SELECT * FROM " + m_defaultTableName
                + " where issue_id = @0", issueId);

            return records.ToList().ConvertAll(x => new IssueActivity(x));
        }

        public List<IssueActivity> GetIssueActivityListByIssueIds(List<int> issueIds)
        {
            if (issueIds.Count == 0)
            {
                return new List<IssueActivity>();
            }

            var issueActivityList = Select<ABSMgrConn.TableIssueActivity, int>(m_defaultTableName, "issue_id", issueIds, null);
            return issueActivityList.Select(x => new IssueActivity(x)).ToList();
        }

        public IssueActivity GetIssueActivityByGuid(string issueActivityGuid)
        {
            var record = SelectSingle<ABSMgrConn.TableIssueActivity>("issue_activity_guid", issueActivityGuid);

            return new IssueActivity(record);
        }

        public IssueActivity GetIssueActivityById(int issueActivityId)
        {
            var record = SelectSingle<ABSMgrConn.TableIssueActivity>("issue_activity_id", issueActivityId);

            return new IssueActivity(record);
        }

        public int UpdateIssueActivity(IssueActivity issueActivity)
        {
            var issueActivityTable = issueActivity.GetTableObject();
            return m_db.Update(m_defaultTableName, m_defaultPrimaryKey, issueActivityTable, issueActivity.IssueActivityId);
        }

        public int DeleteIssueActivity(IssueActivity issueActivity)
        {
            issueActivity.RecordStatus = RecordStatus.Deleted;
            return UpdateIssueActivity(issueActivity);
        }

        public bool IsValidIssueActivity(int issueActivityId)
        {
            var issueActivity = GetIssueActivityById(issueActivityId);

            return issueActivity.RecordStatus == RecordStatus.Valid;
        }
    }
}
