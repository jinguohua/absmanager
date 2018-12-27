using ChineseAbs.ABSManagement.LocalRepository;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DocumentCompareController : BaseController
    {
        static string imgfilePath = CommUtils.GetTemporaryRootFolder() + "\\CompareImgTemp";

        [HttpPost]
        public ActionResult GetImgs(string firstDocGuid, string secondDocGuid)
        {
            return ActionUtils.Json(() =>
            {
                var firstFile = GetRepoFile(firstDocGuid);
                var secondFile = GetRepoFile(secondDocGuid);

                string filename = firstFile.Id + "_" + secondFile.Id;
                var compareUtil = new ComparisonUtil(imgfilePath, filename,ComparisonUtil.SaveType.Html);
                compareUtil.Compare2Doc(firstFile.GetFilePath(), secondFile.GetFilePath());

                var result = new
                {
                    addCount = compareUtil.AddCount,
                    deleteCount = compareUtil.DeleteCount,
                    imgCount = compareUtil.FileCount,
                    imgPath = compareUtil.FilePath,
                };
                return ActionUtils.Success(result);
            });
        }

        private RepositoryFile GetRepoFile(string docGuid)
        {
            CommUtils.Assert(m_dbAdapter.DMSFile.isExistDMSFile(docGuid), "找不到文件fileGuid[{0}]，请刷新页面后重试", docGuid);

            var dmsFile = m_dbAdapter.DMSFile.GetByGuid(docGuid);
            var repoFile = Platform.Repository.GetFile(dmsFile.RepoFileId);
            var dms = m_dbAdapter.DMS.GetById(dmsFile.DMSId);
            var project = m_dbAdapter.Project.GetProjectById(dms.ProjectId);
            CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

            return repoFile;
        }

        public ActionResult GetImg(string imgPath, string imgName)
        {
            string imgfullPath = imgfilePath + "\\" + imgPath + "\\" + imgName;
            string imgType = imgName.Substring(imgName.LastIndexOf('.') + 1, imgName.Length - imgName.LastIndexOf('.') - 1);
           // return base.File(imgfullPath, "image/" + imgType);
            return base.File(imgfullPath, "text/" + imgType);
        }
    }
}