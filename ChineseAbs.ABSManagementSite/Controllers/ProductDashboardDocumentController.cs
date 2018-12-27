using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using System;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ProductDashboardDocumentController : BaseController
    {
        [HttpPost]
        public ActionResult ConfigTemplate(string projectGuid, string fileSeriesGuid, string templateType)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(projectGuid, "ProjectGuid不能为空");

                var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var fileSeriesTemplateType = CommUtils.ParseEnum<DmsFileSeriesTemplateType>(templateType);
                var template = m_dbAdapter.DMSFileSeriesTemplate.GetByFileSeriesId(dmsFileSeries.Id);
                if (template == null)
                {
                    var dmsFileSeriesTemplate = new DMSFileSeriesTemplate();
                    dmsFileSeriesTemplate.FileSeriesId = dmsFileSeries.Id;
                    dmsFileSeriesTemplate.TemplateType = fileSeriesTemplateType;
                    m_dbAdapter.DMSFileSeriesTemplate.New(dmsFileSeriesTemplate);
                }
                else
                {
                    if (template.TemplateType != fileSeriesTemplateType)
                    {
                        template.TemplateType = fileSeriesTemplateType;
                        m_dbAdapter.DMSFileSeriesTemplate.Update(template);

                        var log = string.Format("更新模板文档类型为 [{0}]", Toolkit.ToCnString(fileSeriesTemplateType));
                        m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, fileSeriesGuid, log);
                    }
                }

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult GenerateDoc(string projectGuid, string paymentDay, string fileSeriesGuid, bool autoUpload = false)
        {
            return ActionUtils.Json(() =>
            {
                var project = Platform.GetProject(projectGuid);
                var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var fileSeriesTemplate = m_dbAdapter.DMSFileSeriesTemplate.GetByFileSeriesId(dmsFileSeries.Id);
                CommUtils.Assert(fileSeriesTemplate != null
                    && fileSeriesTemplate.TemplateType != DmsFileSeriesTemplateType.None,
                    "请先配置模板");

                CommUtils.Assert(DateUtils.IsDate(paymentDay), "解析兑付日失败：{0}", paymentDay);

                var paymentDate = DateUtils.Parse(paymentDay).Value;
                var templateType = fileSeriesTemplate.TemplateType;
                var docPatternType = CommUtils.ParseEnum<DocPatternType>(templateType.ToString());
                
                var ms = new MemoryStream();
                Platform.DocFactory.Generate(docPatternType, ms, project.Instance.ProjectId, paymentDate,
                    paymentDate, dmsFileSeries.Name);
                var comment = "系统生成文件[" + dmsFileSeries.Name + "]";
                m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, fileSeriesGuid, comment);
                ms.Seek(0, SeekOrigin.Begin);

                if (autoUpload)
                {
                    var repoFile = Platform.Repository.AddFile(dmsFileSeries.Name, ms);
                    var allFiles = m_dbAdapter.DMSFile.GetFilesByFileSeriesId(dmsFileSeries.Id);
                    var currentVer = allFiles.Count == 0 ? 0 : allFiles.Max(x => x.Version);

                    DMSFile newDMSFile = new DMSFile();
                    newDMSFile.DMSId = dmsFileSeries.DMSId;
                    newDMSFile.DMSFileSeriesId = dmsFileSeries.Id;

                    newDMSFile.RepoFileId = repoFile.Id;
                    newDMSFile.Name = repoFile.Name;
                    newDMSFile.Description = string.Empty;

                    newDMSFile.Size = ms.Length;
                    newDMSFile.Version = currentVer + 1;

                    var now = DateTime.Now;
                    newDMSFile.LastModifyUserName = CurrentUserName;
                    newDMSFile.LastModifyTime = now;
                    newDMSFile.CreateUserName = CurrentUserName;
                    newDMSFile.CreateTime = now;

                    newDMSFile = m_dbAdapter.DMSFile.Create(newDMSFile);

                    var logComment = "系统上传文件[" + dmsFileSeries.Name + "]的第" + newDMSFile.Version + "版本";
                    m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, fileSeriesGuid, logComment);
                }
                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, dmsFileSeries.Name, ms);
                return ActionUtils.Success(resource.Guid.ToString());
            });
        }
    }
}