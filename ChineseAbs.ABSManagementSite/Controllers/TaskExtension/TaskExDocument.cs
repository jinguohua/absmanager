using ChineseAbs.ABSManagement.DocumentFactory;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ChineseAbs.ABSManagementSite.Controllers.TaskExtension
{
    public class TaskExDocument : TaskExBase
    {
        public TaskExDocument(string userName, string shortCode)
            : base(userName, shortCode)
        {
            this.OnFinishing += TaskExDocument_OnFinishing;
        }

        HandleResult TaskExDocument_OnFinishing()
        {
            if (!string.IsNullOrEmpty(Task.TaskExtension.TaskExtensionInfo))
            {
                var taskExDocs = CommUtils.FromJson<List<TaskExDocumentItem>>(Task.TaskExtension.TaskExtensionInfo);
                taskExDocs = taskExDocs.Where(x => !x.UpdateTime.HasValue).ToList();
                if (taskExDocs.Count > 0)
                {
                    var unuploadFiles = string.Join("; ", taskExDocs.ConvertAll(x => x.Name).ToArray());
                    return new HandleResult(EventResult.Cancel, "未上传文件：" + unuploadFiles);
                }
            }

            return new HandleResult();
        }

        public override object GetEntity()
        {
            if (string.IsNullOrEmpty(Task.TaskExtension.TaskExtensionInfo))
            {
                return null;
            }

            var viewModel = new TaskExDocumentViewModel();
            viewModel.Documents = CommUtils.FromJson<List<TaskExDocumentItem>>(Task.TaskExtension.TaskExtensionInfo);

            var projectLogicModel = new ProjectLogicModel(m_userName, Task.ProjectId);
            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(Task.ShortCode);
            var paymentDate = Task.EndTime.Value;
            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
                viewModel.PaymentDay = Toolkit.DateToString(paymentDate);
            }

            var project = projectLogicModel.Instance;
            var sysDocs = m_dbAdapter.Document.GetAllDocument(project.ProjectGuid);

            if (viewModel.Documents.Any(x => x.NamingRule == TaskExDocNamingRule.ByDatasetDuration))
            {
                var datasetSchedule = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate);
                CommUtils.AssertNotNull(datasetSchedule.Dataset.Instance, "第{0}期模型未生成", Toolkit.DateToString(datasetSchedule.PaymentDate));

                viewModel.PaymentDay = Toolkit.DateToString(datasetSchedule.PaymentDate);
            }

            foreach (var taskExDoc in viewModel.Documents)
            {
                var sysDocName = GetSysDocName(Task, taskExDoc);

                var sysDoc = sysDocs.FirstOrDefault(x => x.DocumentName == sysDocName);
                if (sysDoc != null)
                {
                    sysDoc = m_dbAdapter.Document.GetDocumentById(sysDoc.DocumentId);
                    var extension = new DocFileInfo { LogicName = sysDoc.Path }.Extension;
                    taskExDoc.FileType = FileUtils.GetFileType(extension);
                }
            }

            return viewModel;
        }

        public int UploadTaskExDocument(HttpPostedFileBase file, string shortCode, string taskExDocumentGuid)
        {
            CommUtils.Assert(file != null && file.ContentLength != 0, "File is null or empty.");
            FileUtils.CheckExtension(file.FileName);

            var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);
            m_dbAdapter.Task.CheckPrevIsFinished(Task);
            var projectId = task.ProjectId;

            var taskExType = CommUtils.ParseEnum<TaskExtensionType>(task.TaskExtension.TaskExtensionType);
            CommUtils.AssertEquals(taskExType, TaskExtensionType.Document, "Task extension type is not document.");

            var taskExDocs = CommUtils.FromJson<List<TaskExDocumentItem>>(task.TaskExtension.TaskExtensionInfo);
            CommUtils.Assert(taskExDocs.Any(x => x.Guid == taskExDocumentGuid), "Can't find task extension document guid.");

            var taskExDoc = taskExDocs.Single(x => x.Guid == taskExDocumentGuid);

            var fileInfo = new DocFileInfo { DisplayName = file.FileName };
            fileInfo.DisplayNameWithoutExtension = taskExDoc.Name;
            fileInfo.LogicNameWithoutExtension = GetSysDocName(task, taskExDoc);

            var msg = "工作[" + shortCode + "]上传《" + fileInfo.LogicName + "》";
            bool isNewDocument = UploadDocument(projectId, fileInfo.LogicName, fileInfo.MIME, file.InputStream, msg, (int)taskExDoc.DocumentType);
            CommUtils.Assert(isNewDocument, "《" + file.FileName + "》和服务器上文件相同，不再重复上传");
            UpdateTaskExDocUpdateTime(task, taskExDocs, taskExDoc, DateTime.Now);

            var logicModel = new ProjectLogicModel(m_userName, projectId);
            new TaskLogicModel(logicModel, task).Start();
            m_dbAdapter.Task.AddTaskLog(task, "上传《" + fileInfo.DisplayName + "》");
            return 1;
        }

        private void UpdateSingleTaskExDocUpdateTime(Task task, List<TaskExDocumentItem> taskExDocs, TaskExDocumentItem taskExDoc, DateTime updateTime)
        {
            //Update the file's update time in database.
            taskExDoc.UpdateTime = updateTime;
            var json = CommUtils.ToJson(taskExDocs);
            task.TaskExtension.TaskExtensionInfo = json;
            m_dbAdapter.Task.UpdateTaskExtension(task.TaskExtension);
        }

        //更新扩展工作文档的更新时间（上传文档时触发）
        private void UpdateTaskExDocUpdateTime(Task task, List<TaskExDocumentItem> taskExDocs, TaskExDocumentItem taskExDoc, DateTime updateTime)
        {
            UpdateSingleTaskExDocUpdateTime(task, taskExDocs, taskExDoc, updateTime);

            if (taskExDoc.NamingRule == TaskExDocNamingRule.ByDatasetDuration)
            {
                var tasks = m_dbAdapter.Task.GetTasksByProjectId(task.ProjectId);
                tasks = tasks.Where(x => x.TaskExtensionId.HasValue).ToList();
                tasks.ForEach(x => x.TaskExtension = m_dbAdapter.Task.GetTaskExtension(x.TaskExtensionId.Value));
                tasks = tasks.Where(x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.Document).ToList();
                foreach (var curTask in tasks)
                {
                    if (curTask.ShortCode == task.ShortCode
                        || string.IsNullOrEmpty(curTask.TaskExtension.TaskExtensionInfo))
                    {
                        continue;
                    }

                    var curTaskExDocs = CommUtils.FromJson<List<TaskExDocumentItem>>(curTask.TaskExtension.TaskExtensionInfo);
                    var findDocsCount = curTaskExDocs.Where(x => x.NamingRule == TaskExDocNamingRule.ByDatasetDuration && x.Name == taskExDoc.Name).ToList().Count;
                    if (findDocsCount == 0)
                    {
                        //未在该Task找到同名文档
                        continue;
                    }
                    CommUtils.AssertEquals(findDocsCount, 1, "Reduplicate document name in task [" + curTask.ShortCode + "]");
                    var curTaskExDoc = curTaskExDocs.Single(x => x.NamingRule == TaskExDocNamingRule.ByDatasetDuration && x.Name == taskExDoc.Name);

                    UpdateSingleTaskExDocUpdateTime(curTask, curTaskExDocs, curTaskExDoc, updateTime);
                }
            }
        }

        private string GetSysDocName(Task task, TaskExDocumentItem taskExDoc)
        {
            return taskExDoc.Name + "(" + GetSysDocNamePostfix(task, taskExDoc) + ")";
        }

        //获取扩展工作的文档名字中的后缀，参考TaskExDocNamingRule
        private string GetSysDocNamePostfix(Task task, TaskExDocumentItem taskExDoc)
        {
            if (taskExDoc.NamingRule == TaskExDocNamingRule.ByShortCode)
            {
                return task.ShortCode;
            }
            else if (taskExDoc.NamingRule == TaskExDocNamingRule.ByDatasetDuration)
            {
                CommUtils.Assert(task.EndTime.HasValue, "Task's [" + task.ShortCode + "] end time must has value.");
                var endDate = task.EndTime.Value;
                var schedule = NancyUtils.GetDealSchedule(task.ProjectId);
                CommUtils.AssertNotNull(schedule, "Schedule of project [" + task.ProjectId + "] can't be null.");
                var paymentDates = schedule.PaymentDates;
                var closingDate = schedule.ClosingDate;
                CommUtils.Assert(paymentDates.Length > 0, "Schedule of project [" + task.ProjectId + "] can't be empty.");
                CommUtils.Assert(closingDate < paymentDates.First(), "Closing date of project [" + task.ProjectId + "] isn't before the first payment date.");

                Func<DateTime, string> convertDate = (x) => x.ToString("yyyyMMdd");

                var beginStr = string.Empty;
                var endStr = string.Empty;
                if (endDate < closingDate)
                {
                    //工作截止时间在起息日前
                    endStr = convertDate(closingDate);
                }
                else if (endDate <= paymentDates.First())
                {
                    //工作截止时间在起息日（含）和第一个付息日（含）之间
                    beginStr = convertDate(closingDate);
                    endStr = convertDate(paymentDates.First());
                }
                else if (endDate > paymentDates.Last())
                {
                    //工作截止时间在最后一个付息日（不含）之后
                    beginStr = convertDate(paymentDates.Last());
                }
                else
                {
                    //工作截止时间在N多个付息日之间
                    for (int i = 1; i < paymentDates.Length; ++i)
                    {
                        if (endDate <= paymentDates[i] && endDate > paymentDates[i - 1])
                        {
                            beginStr = convertDate(paymentDates[i - 1]);
                            endStr = convertDate(paymentDates[i]);
                            break;
                        }
                    }
                }

                return beginStr + "~" + endStr;
            }

            throw new ApplicationException("Undefined TaskExDocNamingRule.");
        }

        public DocFileInfo DownloadTaskExDocument(string shortCode, string taskExDocumentGuid)
        {
            var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);
            var taskExDoc = GetTaskExDocument(task, taskExDocumentGuid);

            var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);
            var docs = m_dbAdapter.Document.GetAllDocument(project.ProjectGuid);
            var sysDocName = GetSysDocName(task, taskExDoc);
            CommUtils.Assert(docs.Any(x => x.DocumentName == sysDocName), "Can't find document [" + sysDocName + "]");

            var doc = docs.Single(x => x.DocumentName == sysDocName);
            doc = m_dbAdapter.Document.GetDocumentById(doc.DocumentId);

            var projectPath = WebConfigUtils.DocumentFolderPath;
            string absolutePath = Path.Combine(projectPath, doc.Path);

            var fileInfo = new DocFileInfo { AbsultePath = absolutePath };
            fileInfo.DisplayNameWithoutExtension = taskExDoc.Name;

            m_dbAdapter.Task.AddTaskLog(task, "下载《" + fileInfo.DisplayName + "》");
            return fileInfo;
        }

        private bool UploadDocument(int projectId, string fileFullName, string mimeType, Stream stream, string comment, int documentType)
        {
            var projectGuid = DbUtils.GetGuidById(projectId, "Project", "project_id");

            var fileInfo = new DocFileInfo { LogicName = fileFullName };
            var documentName = FileUtils.GetFileNameWithoutExtension(fileFullName);
            var fileExtension = FileUtils.GetExtension(fileFullName);

            var documentFolderPath = WebConfigUtils.DocumentFolderPath;
            var projectPath = Path.Combine(documentFolderPath, "Project_" + projectGuid);

            if (!Directory.Exists(projectPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(projectPath);
                directoryInfo.Create();
            }

            var documents = m_dbAdapter.Document.GetDocumentHistoryVersion(projectGuid, fileInfo.LogicNameWithoutExtension);
            if (documents.Count != 0)
            {
                var latestDocument = documents.Last();
                latestDocument = m_dbAdapter.Document.GetDocumentById(latestDocument.DocumentId);

                var path = Path.Combine(documentFolderPath, latestDocument.Path);
                using (var latestFs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    bool isDocx = (fileExtension == ".docx" && path.EndsWith(".docx"));
                    bool isEqualWithLatestFile = isDocx ? CommUtils.DocxEquals(latestFs, stream) : Md5Utils.Equals(latestFs, stream);
                    if (isEqualWithLatestFile)
                    {
                        m_logger.Log(projectId, "《" + fileFullName + "》和服务器上文件相同，不再重复上传。");
                        return false;
                    }
                }
            }

            int maxVersion = m_dbAdapter.Document.GetDocumentLatestVersion(projectGuid, fileInfo.LogicNameWithoutExtension);

            string fileName = FileUtils.CombineExtension(fileInfo.LogicNameWithoutExtension + "V" + (maxVersion + 1), fileInfo.Extension);
            string absolutePath = Path.Combine(projectPath, fileName);

            FileStream fs = new FileStream(absolutePath, FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(fs);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(writer.BaseStream);
            writer.Flush();
            writer.Close();
            fs.Close();

            string relativePath = Path.Combine("Project_" + projectGuid, fileName);
            m_dbAdapter.Document.UploadDocument(m_userName, projectGuid, documentName, mimeType, relativePath, maxVersion + 1, comment, documentType);
            return true;
        }

        private TaskExDocumentItem GetTaskExDocument(Task task, string taskExDocumentGuid)
        {
            var projectId = task.ProjectId;
            var taskExType = CommUtils.ParseEnum<TaskExtensionType>(task.TaskExtension.TaskExtensionType);
            CommUtils.AssertEquals(taskExType, TaskExtensionType.Document, "Task extension type is not document.");

            var taskExDocs = CommUtils.FromJson<List<TaskExDocumentItem>>(task.TaskExtension.TaskExtensionInfo);
            CommUtils.Assert(taskExDocs.Any(x => x.Guid == taskExDocumentGuid), "Can't find task extension document guid.");

            var taskExDoc = taskExDocs.Single(x => x.Guid == taskExDocumentGuid);
            return taskExDoc;
        }

        public Tuple<MemoryStream, DocFileInfo> GenerateDocument(string shortCode, string taskExDocumentGuid, bool autoUpload)
        {
            var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);
            var taskPeriod = m_dbAdapter.TaskPeriod.GetByShortCode(shortCode);
            var paymentDate = task.EndTime.Value;
            if (taskPeriod != null)
            {
                paymentDate = taskPeriod.PaymentDate;
            }

            var projectId = task.ProjectId;
            var taskExType = CommUtils.ParseEnum<TaskExtensionType>(task.TaskExtension.TaskExtensionType);
            CommUtils.AssertEquals(taskExType, TaskExtensionType.Document, "Task extension type is not document.");

            var taskExDocs = CommUtils.FromJson<List<TaskExDocumentItem>>(task.TaskExtension.TaskExtensionInfo);
            CommUtils.Assert(taskExDocs.Any(x => x.Guid == taskExDocumentGuid), "Can't find task extension document guid.");

            var taskExDoc = taskExDocs.Single(x => x.Guid == taskExDocumentGuid);
            CommUtils.Assert(taskExDoc.PatternType.ToString() != "None", "当前文档的模板类型为空，无法通过系统生成。");

            var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);

            var docPatternType = CommUtils.ParseEnum<DocPatternType>(taskExDoc.PatternType);
            var patternFileName = DocumentPattern.GetFileName(docPatternType);
            var patternPath = DocumentPattern.GetPath(project, docPatternType);

            CommUtils.Assert(File.Exists(patternPath),
                "产品[{0}]的模板[{1}]未上传,请在解决方案 -> 存续期管理平台 -> 文档设计中上传模板", project.Name, patternFileName);

            var projectLogicModel = new ProjectLogicModel(m_userName, project);
            paymentDate = projectLogicModel.DealSchedule.GetByPaymentDay(paymentDate).PaymentDate;

            var ms = new MemoryStream();
            var docFactiory = new DocumentFactory(m_userName);
            docFactiory.Generate(docPatternType, ms, project.ProjectId,
                paymentDate, task.EndTime.Value, taskExDoc.Name);

            var docFileInfo = new DocFileInfo { DisplayName = patternFileName };
            docFileInfo.DisplayNameWithoutExtension = taskExDoc.Name;
            docFileInfo.LogicNameWithoutExtension = GetSysDocName(task, taskExDoc);

            var logicModel = new ProjectLogicModel(m_userName, projectId);
            new TaskLogicModel(logicModel, task).Start();
            m_dbAdapter.Task.AddTaskLog(task, "系统生成《" + docFileInfo.DisplayName + "》");

            if (autoUpload)
            {
                ms.Seek(0, SeekOrigin.Begin);

                if (UploadDocument(task.ProjectId, docFileInfo.LogicName, docFileInfo.MIME,
                    ms, "系统上传《" + docFileInfo.LogicName + "》", (int)taskExDoc.DocumentType)
                    || !taskExDoc.UpdateTime.HasValue)
                {
                    UpdateTaskExDocUpdateTime(task, taskExDocs, taskExDoc, DateTime.Now);
                    m_dbAdapter.Task.AddTaskLog(task, "系统上传《" + docFileInfo.DisplayName + "》");
                }
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Tuple.Create(ms, docFileInfo);
        }
    }
}