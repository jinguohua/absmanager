using System;
using System.IO;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.ABSManagementSite.Filters;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Loggers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DocumentController : BaseController
    {
        public ActionResult Index()
        {
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
            List<object> projectList = new List<object>();
            foreach (var project in projects)
            {
                projectList.Add(new { ProjectName = project.Name, ProjectGuid = project.ProjectGuid, Selected = false });
            }

            var documentTypes = m_dbAdapter.Document.GetAllDocumentType();
            List<object> typeList = new List<object>
            {
                new { TypeId = "default", TypeName = "所有", Selected = true }
            };
            foreach (var type in documentTypes)
            {
                typeList.Add(new { TypeId = type.DocumentTypeId, TypeName = type.TypeName, Selected = false });
            }

            var documentManagerViewModel = new DocumentManagerViewModel
            {
                Projects = projectList,
                DocumentTypes = typeList
            };
            return View(documentManagerViewModel);
        }

        public ActionResult ShowSpecifiedProjectDocuments(string projectGuid)
        {
            var specifiedProject = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
            List<object> projectList = new List<object>();
            foreach (var project in projects)
            {
                if (project.ProjectGuid == projectGuid)
                {
                    projectList.Add(new { ProjectName = project.Name, ProjectGuid = project.ProjectGuid, Selected = true });
                    continue;
                }
                projectList.Add(new { ProjectName = project.Name, ProjectGuid = project.ProjectGuid, Selected = false });
            }

            var documentTypes = m_dbAdapter.Document.GetAllDocumentType();
            List<object> typeList = new List<object>
            {
                new { TypeId = "default", TypeName = "所有", Selected = true }
            };
            foreach (var type in documentTypes)
            {
                typeList.Add(new { TypeId = type.DocumentTypeId, TypeName = type.TypeName, Selected = false });
            }

            var documentManagerViewModel = new DocumentManagerViewModel
            {
                Projects = projectList,
                DocumentTypes = typeList
            };
            return View("Index", documentManagerViewModel);
        }

        /// <summary>
        /// Gets all documents related to the project by project guid.
        /// </summary>
        /// <param name="projectGuid">The project guid.</param>
        /// <returns>Returns all documents realted to the project.</returns>
        [ProjectGuidAccess]
        public ActionResult GetAllDocumentsByProjectGuid(string projectGuid)
        {
            if (string.IsNullOrEmpty(projectGuid))
            {
                throw new ApplicationException("传入非法参数");
            }
            var documents = m_dbAdapter.Document.GetAllDocument(projectGuid);

            Platform.UserProfile.Precache(documents.Select(x => x.Username));

            var documentViewModels = documents.Select(document => new DocumentViewModel
            {
                DocumentId = document.DocumentId,
                Username = Platform.UserProfile.Get(document.Username) == null
                    ? document.Username : Platform.UserProfile.Get(document.Username).UserName,
                DocumentName = document.DocumentName,
                DocumentTypeId = document.DocumentTypeId,
                DocumentTypeName = m_dbAdapter.Document.GetDocumentTypeName(document.DocumentTypeId),
                Version = document.Version,
                UploadTime = document.UploadTime
            }).ToList();
            return Json(documentViewModels, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets all documents related to the project after the document type filter.
        /// </summary>
        /// <param name="documentTypeId">The document type id.</param>
        /// <param name="projectGuid">The project guid.</param>
        /// <returns>Returns all documents related to the project after filter.</returns>
        public ActionResult GetAllDocumentsByFilter(string projectGuid, int documentTypeId = 0)
        {
            if (string.IsNullOrEmpty(projectGuid) || documentTypeId < 0)
            {
                throw new ApplicationException("传入非法参数");
            }
            var documents = new List<Document>();

            // 'documentTypeId = 0' indicates to get all documents related to the project.
            if (documentTypeId == 0)
            {
                documents = m_dbAdapter.Document.GetAllDocument(projectGuid);
            }
            else
            {
                documents = m_dbAdapter.Document.GetAllDocumentByType(documentTypeId, projectGuid);
            }

            var documentViewModels = new List<DocumentViewModel>();
            Platform.UserProfile.Precache(documents.Select(x => x.Username));
            foreach (var document in documents)
            {
                documentViewModels.Add(new DocumentViewModel
                {
                    DocumentId = document.DocumentId,
                    Username = Platform.UserProfile.Get(document.Username).UserName,
                    DocumentName = document.DocumentName,
                    DocumentTypeId = document.DocumentTypeId,
                    DocumentTypeName = m_dbAdapter.Document.GetDocumentTypeName(document.DocumentTypeId),
                    Version = document.Version,
                    UploadTime = document.UploadTime
                });
            }
            return Json(documentViewModels, JsonRequestBehavior.AllowGet);
        }

        //cachedRecordCount 是前端已缓存的log条数（防止前端下载数据量过大）
        public ActionResult GetProjectDocumentOperationLog(string projectGuid, int? cachedRecordCount)
        {
            if (string.IsNullOrEmpty(projectGuid))
            {
                throw new ApplicationException("传入非法参数");
            }
            var userLogs = m_dbAdapter.Document.GetProjectDocumentOperationLogs(projectGuid);
            var logs = new List<object>();
            Platform.UserProfile.Precache(userLogs.Select(x => x.TimeStampUserName));
            foreach (var log in userLogs)
            {
                var profile = Platform.UserProfile.Get(log.TimeStampUserName);
                logs.Add(new {
                    Timestamp = log.TimeStamp,
                    Content = log.Comment,
                    Operator = profile == null ? log.TimeStampUserName : profile.UserName
                });
            }
            if (cachedRecordCount.HasValue && cachedRecordCount.Value == userLogs.Count)
            {
                return Json(new List<UserLogs>(), JsonRequestBehavior.AllowGet);
            }
            return Content(Serialize(logs), "text/json");
        }

        public ActionResult GetDocumentHistoryVersion(string projectGuid, string documentName)
        {
            if (string.IsNullOrEmpty(projectGuid) || string.IsNullOrEmpty(documentName))
            {
                throw new ApplicationException("传入参数非法");
            }
            var documents = m_dbAdapter.Document.GetDocumentHistoryVersion(projectGuid, documentName);
            var documentHistoryViewModels = new List<DocumentHistoryViewModel>();

            Platform.UserProfile.Precache(documents.Select(x => x.Username));
            foreach (var document in documents)
            {
                var profile = Platform.UserProfile.Get(document.Username);
                documentHistoryViewModels.Add(new DocumentHistoryViewModel
                {
                    DocumentId = document.DocumentId,
                    Username = profile == null ? document.Username : profile.UserName,
                    DocumentTypeName = m_dbAdapter.Document.GetDocumentTypeName(document.DocumentTypeId),
                    Version = document.Version,
                    UploadTime = document.UploadTime,
                    Comment = document.Comment
                });
            }
            return Json(documentHistoryViewModels, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadDocument()
        {
            var file = Request.Files["file"];
            var projectGuid = Request["projectGuid"];
            var comment = Request["comment"];
            var documentName = Request["documentName"].Trim();
            var documentTypeIdStr = Request["documentType"];
            var invalidChars = Path.GetInvalidFileNameChars();

            if (string.IsNullOrEmpty(projectGuid) || file == null || string.IsNullOrEmpty(documentName) ||
                !m_dbAdapter.Document.CheckDocumentTypeId(documentTypeIdStr))
            {
                return Json(new { IsSuccess = false, ErrMsg = "参数非法!" });
            }

            if (documentName.Where(item => invalidChars.Contains(item)).ToArray().Any())
            {
                return Json(new { IsSuccess = false, ErrMsg = "文件名称含有非法字符!"});
            }

            if (file.ContentLength > 10 * 1024 * 1024)
            {
                return Json(new { IsSuccess = false, ErrMsg = "上传的文档应不超过10M大小" });
            }

            try
            {
                var projectFolder = string.Format("Project_{0}", projectGuid);
                var projectPath = Path.Combine(WebConfigUtils.DocumentFolderPath, projectFolder);

                if (!Directory.Exists(projectPath))
                {
                    new DirectoryInfo(projectPath).Create();
                }
                var mimeType = file.ContentType;
                var username = CurrentUserName;
                var fileExtension = Path.GetExtension(file.FileName);
                int maxVersion = m_dbAdapter.Document.GetDocumentLatestVersion(projectGuid, documentName);

                if (maxVersion > 0)
                {
                    maxVersion++;
                    string fileName = string.Format("{0}_V{1}{2}", documentName, maxVersion, fileExtension);
                    string absolutePath = Path.Combine(projectPath, fileName);
                    file.SaveAs(absolutePath);
                    string relativePath = Path.Combine(projectFolder, fileName);
                    m_dbAdapter.Document.UploadDocument(username, projectGuid, documentName, mimeType, relativePath, maxVersion, comment, int.Parse(documentTypeIdStr));
                }
                else
                {
                    string fileName = string.Format("{0}_V1{1}", documentName, fileExtension);
                    string absolutePath = Path.Combine(projectPath, fileName);
                    file.SaveAs(absolutePath);
                    string relativePath = Path.Combine(projectFolder, fileName);
                    m_dbAdapter.Document.UploadDocument(username, projectGuid, documentName, mimeType, relativePath, 1, comment, int.Parse(documentTypeIdStr));
                }

                var documents = m_dbAdapter.Document.GetAllDocument(projectGuid);
                Platform.UserProfile.Precache(documents.Select(x => x.Username));
                var documentViewModels = documents.Select(document => new DocumentViewModel
                {
                    DocumentId = document.DocumentId,
                    Username = Platform.UserProfile.Get(document.Username) == null
                        ? document.Username : Platform.UserProfile.Get(document.Username).UserName,
                    DocumentName = document.DocumentName,
                    DocumentTypeId = document.DocumentTypeId,
                    DocumentTypeName = m_dbAdapter.Document.GetDocumentTypeName(document.DocumentTypeId),
                    Version = document.Version,
                    UploadTime = document.UploadTime
                }).ToList();
                return Json(new { IsSuccess = true, Data = documentViewModels }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, ErrMsg = "服务器异常！上传失败！"});
            }
        }

        [FileDownload]
        public ActionResult DownloadDocument(int documentId)
        {
            var document = m_dbAdapter.Document.GetDocumentById(documentId);

            int projectId = DbUtils.GetIdByGuid(document.ProjectGuid, "[dbo].[Project]", "project_guid");
            Logger.Log(projectId, string.Format("下载文档《{0}》", document.DocumentName));

            var projectPath = WebConfigUtils.DocumentFolderPath;
            string absolutePath = Path.Combine(projectPath, document.Path);
            string extension = Path.GetExtension(document.Path);

            var logicModel = Platform.GetProject(document.ProjectGuid);
            var downFileAuthority = logicModel.Authority.DownloadFile.CurrentUserAuthority;
            return CnabsFile(absolutePath, document.MimeType, document.DocumentName + extension, downFileAuthority);
        }

        private LoggerDocument Logger
        {
            get { return new LoggerDocument(new UserInfo(CurrentUserName)); }
        }

        private string Serialize(object o)
        {
            var setting = new JsonSerializerSettings();
            var jsonConverter = new List<JsonConverter>()
            {
                new IsoDateTimeConverter(){ DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }
            };
            setting.Converters = jsonConverter;
            return JsonConvert.SerializeObject(o, setting);
        }
    }
}
