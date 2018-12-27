using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class IssueActivityFile : BaseDataContainer<TableIssueActivityFile>
    {
        public IssueActivityFile()
        {
        }

        public IssueActivityFile(TableIssueActivityFile issueActivityFile)
            : base(issueActivityFile)
        {
        }

        public int Id { get; set; }

        public int IssueActivityId { get; set; }

        public int FileId { get; set; }

        public override void FromTableObject(TableIssueActivityFile issueActivityFile)
        {
            Id = issueActivityFile.issue_activity_file_id;
            IssueActivityId = issueActivityFile.issue_activity_id;
            FileId = issueActivityFile.file_id;
        }

        public override TableIssueActivityFile GetTableObject()
        {
            var issueActivityFile = new TableIssueActivityFile();
            issueActivityFile.issue_activity_file_id = Id;
            issueActivityFile.issue_activity_id = IssueActivityId;
            issueActivityFile.file_id = FileId;

            return issueActivityFile;
        }
    }
}
