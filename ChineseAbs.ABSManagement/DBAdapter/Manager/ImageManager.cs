using ABSMgrConn;
using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.Repository;

namespace ChineseAbs.ABSManagement
{
    public class ImageManager : BaseManager
    {
        public ImageManager()
        {
            m_defaultTableName = "[dbo].[Image]";
            m_defaultPrimaryKey = "image_id";
        }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public Image New(string name, string path)
        {
            var now = DateTime.Now;

            var image = new Image();
            image.Name = name;
            image.Path = path;

            image.Guid = Guid.NewGuid().ToString();
            image.CreateTime = now;
            image.CreateUserName = UserInfo.UserName;
            image.LastModifyTime = now;
            image.LastModifyUserName = UserInfo.UserName;
            image.Id = Insert(image.GetTableObject());
            return image;
        }

        public Image Update(Image image)
        {
            var obj = image.GetTableObject();
            m_db.Update(m_defaultTableName, m_defaultPrimaryKey, obj, obj.image_id);
            return image;
        }

        public Image GetById(int id)
        {
            var record = SelectSingle<TableImage>(id);
            return new Image(record);
        }

        public Image GetByGuid(string guid)
        {
            var record = SelectSingle<TableImage>("image_guid", guid);
            return new Image(record);
        }

        public List<Image> GetImagesByIds(List<int> imageIds)
        {
            if (imageIds.Count == 0)
            {
                return new List<Image>();
            }

            var files = Select<ABSMgrConn.TableImage, int>(m_defaultTableName, m_defaultPrimaryKey, imageIds, null);
            return files.Select(x => new Image(x)).ToList();
        }

        public List<Image> GetImagesByGuids(List<string> imageGuids)
        {
            if (imageGuids.Count == 0)
            {
                return new List<Image>();
            }

            var files = Select<ABSMgrConn.TableImage, string>(m_defaultTableName, "image_guid", imageGuids, null);
            return files.Select(x => new Image(x)).ToList();
        }

        public IssueActivityImage NewIssueActivityImage(int issueActivityId, int imageId)
        {
            var issueActivityImage = new IssueActivityImage();
            issueActivityImage.IssueActivityId = issueActivityId;
            issueActivityImage.ImageId = imageId;

            issueActivityImage.Id = Insert("dbo.IssueActivityImage", "issue_activity_image_id", issueActivityImage.GetTableObject());
            return issueActivityImage;
        }

        public List<IssueActivityImage> GetImageIdsByIssueActivityIds(List<int> issueActivityIds)
        {
            if (issueActivityIds.Count == 0)
            {
                return new List<IssueActivityImage>();
            }

            var issueActivityImages = Select<ABSMgrConn.TableIssueActivityImage, int>("dbo.IssueActivityImage", "issue_activity_id", issueActivityIds, null);
            return issueActivityImages.Select(x => new IssueActivityImage(x)).ToList();
        }

        public List<IssueActivityImage> GetImageIdsByIssueActivityId(int issueActivityId)
        {
            var issueActivityImages = Select<ABSMgrConn.TableIssueActivityImage>("dbo.IssueActivityImage", "issue_activity_id", issueActivityId);
            return issueActivityImages.Select(x => new IssueActivityImage(x)).ToList();
        }

        public IssueActivityImage GetIssueActivityImageByImageId(int imageId)
        {
            var record = SelectSingle<TableIssueActivityImage>("dbo.IssueActivityImage", "image_id", imageId);
            return new IssueActivityImage(record);
        }

        public int DeleteIssueActivityFile(IssueActivityImage issueActivityImage)
        {
            return m_db.Delete("dbo.IssueActivityImage", "issue_activity_id", issueActivityImage.GetTableObject());
        }

        public int DeleteIssueActivityImages(List<IssueActivityImage> issueActivityImages)
        {
            if (issueActivityImages.Count == 0)
            {
                return 0;
            }

            var ids = issueActivityImages.Select(x => x.Id);
            var sql = "delete from dbo.IssueActivityImage where issue_activity_id in (@issueActivityImages)";
            return m_db.Execute(sql, new { issueActivityImages = ids });
        }
    }
}
