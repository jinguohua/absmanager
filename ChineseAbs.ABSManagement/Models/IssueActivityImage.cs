using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class IssueActivityImage : BaseDataContainer<TableIssueActivityImage>
    {
        public IssueActivityImage()
        {
        }

        public IssueActivityImage(TableIssueActivityImage issueActivityImage)
            : base(issueActivityImage)
        {
        }

        public int Id { get; set; }

        public int IssueActivityId { get; set; }

        public int ImageId { get; set; }

        public override void FromTableObject(TableIssueActivityImage issueActivityImage)
        {
            Id = issueActivityImage.issue_activity_image_id;
            IssueActivityId = issueActivityImage.issue_activity_id;
            ImageId = issueActivityImage.image_id;
        }

        public override TableIssueActivityImage GetTableObject()
        {
            var issueActivityImage = new TableIssueActivityImage();
            issueActivityImage.issue_activity_image_id = Id;
            issueActivityImage.issue_activity_id = IssueActivityId;
            issueActivityImage.image_id = ImageId;

            return issueActivityImage;
        }
    }
}
