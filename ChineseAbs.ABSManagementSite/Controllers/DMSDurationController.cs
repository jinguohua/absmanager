using ChineseAbs.ABSManagement.LocalRepository;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.DocumentManagementSystem;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DMSDurationController : BaseController
    {
        //编辑
        [HttpPost]
        public ActionResult EditFileName(string projectGuid, string folderGuid, string fileSeriesGuid, string fileName)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Write);
                CommUtils.AssertHasContent(fileName, "文件名称不能为空");
                ValidateUtils.FileName(fileName, "文档名称");
                CommUtils.Assert(fileName.Length <= 100, "文档名称[{0}]不能超过100个字符数", fileName);

                var folder = dms.FindFolder(folderGuid);
                folder.IgnoreNull = false;
                CommUtils.AssertNotNull(folder, "找不到文件夹[FolderGuid={0} DMSGuid={1}]，请刷新后再试",
                    folderGuid, dms.Instance.Guid);
                CommUtils.Assert(!folder.Files.Any(x => x.FileSeries.Name == fileName && x.FileSeries.Guid != fileSeriesGuid),
                       "文件[{0}]已经存在", fileName);
                CommUtils.Assert(folder.Files.Exists(x => x.FileSeries.Guid == fileSeriesGuid), "文档不在文件夹下");

                var fileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var comment = "修改文件名称[" + fileSeries.Name + "]为[" + fileName + "]";
                fileSeries.Name = fileName;
                m_dbAdapter.DMSFileSeries.Update(fileSeries);
                m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid,fileSeriesGuid,comment);
                return ActionUtils.Success(true);
            });
        }

        //获取所有folder
        [HttpPost]
        public ActionResult GetAllFolders(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Read);
                dynamic tree = dms.Root.ToTree();
                return ActionUtils.Success(tree);
            });
        }

        //获取一个folder的路径
        [HttpPost]
        public ActionResult GetFolderPath(string projectGuid, string folderGuid)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Read);
                var folder = dms.FindFolder(folderGuid);
                var folderPath = new List<DMSFolderLogicModel>();
                while (folder != null)
                {
                    folderPath.Add(folder);
                    folder = folder.ParentFolder;
                }

                folderPath.Reverse();
                var result = folderPath.Select(x => new
                {
                    folderGuid = x.Instance.Guid,
                    folderName = x.Instance.Name,
                });

                return ActionUtils.Success(result);
            });
        }

        private DMSLogicModel GetDMSAndCheckPermission(string projectGuid, PermissionType permissionType)
        {
           // CheckPermission(PermissionObjectType.Project, projectGuid, permissionType);
            var projectLogicModel = Platform.GetProject(projectGuid);
           int projectId= m_dbAdapter.Project.GetProjectByGuid(projectGuid).ProjectId;
            var dmsJoinLogicModel = new DMSLogicModel(CurrentUserName, projectLogicModel, DMSType.DurationManagementPlatform, projectId.ToString());
            return dmsJoinLogicModel;
        }

        //创建一些folder
        [HttpPost]
        public ActionResult CreateFolders(string projectGuid, string parentFolderGuid, List<string> folderNames, List<string> folderDescriptions)
        {
            return ActionUtils.Json(() =>
            {
                folderNames.ForEach(x => ValidateUtils.FileName(x, "文档名称"));
                CommUtils.AssertEquals(folderNames.Count, folderDescriptions.Count, "传入folderNames和folderDescriptions长度不相等");
                CommUtils.AssertEquals(folderNames.Select(x => x.ToLower()).Distinct().Count(),
                    folderNames.Count, "传入了重复的文件夹名");
                CommUtils.Assert(folderNames.Any(x => x.Length <= 100), "文件夹名称不能超过100个字符数");
                CommUtils.AssertHasContent(folderNames, "文件夹名称不能为空");

                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Write);

                var parentFolder = m_dbAdapter.DMSFolder.GetByGuid(parentFolderGuid);
                CommUtils.AssertEquals(parentFolder.DMSId, dms.Instance.Id, "传入projectGuid和folderGuid不在同一个DMS中");

                var sibbingFolders = dms.AllFolders.Where(x => x.ParentFolderId.HasValue
                    && x.ParentFolderId.Value == parentFolder.Id);
                foreach (var folderName in folderNames)
                {
                    CommUtils.Assert(!sibbingFolders.Any(x => x.Name.Equals(folderName, StringComparison.CurrentCultureIgnoreCase)),
                        "文件夹[{0}]已经存在，请刷新后再试", folderName);
                }

                var folders = new List<DMSFolder>();
                var now = DateTime.Now;
                for (int i = 0; i < folderNames.Count; i++)
                {
                    var folder = new DMSFolder();
                    folder.DMSId = dms.Instance.Id;
                    folder.Name = folderNames[i];
                    folder.Description = folderDescriptions[i];
                    folder.DmsFolderType = DmsFolderType.Normal;
                    folder.ParentFolderId = parentFolder.Id;
                    folder.CreateTime = now;
                    folder.CreateUserName = CurrentUserName;
                    folder.LastModifyTime = now;
                    folder.LastModifyUserName = CurrentUserName;
                    folder = m_dbAdapter.DMSFolder.Create(folder);
                    folders.Add(folder);
                }
                var folderGuids = folders.Select(x => x.Guid);
                return ActionUtils.Success(folderGuids);
            });
        }


        //创建新文档
        [HttpPost]
        public ActionResult CreateFile(string projectGuid, string folderGuid, string fileSeriesNames,
            string description, string templateType)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Execute);
                var folder = dms.FindFolder(folderGuid);
                CommUtils.AssertNotNull(folder, "找不到文件夹[FolderGuid={0} DMSGuid={1}]，请刷新后再试",
                    folderGuid, dms.Instance.Guid);
                folder.IgnoreNull = false;

                var dict = new Dictionary<RepositoryFile, HttpPostedFileBase>();

                var fileSeriesNameList = CommUtils.Split(fileSeriesNames).ToList();

                var files = Request.Files;
                //  CommUtils.Assert(Request.Files.Count > 0, "请选择上传文件");
                fileSeriesNameList.ForEach(x =>
                {
                    ValidateUtils.FileName(x, "文档名称");
                    CommUtils.Assert(x.Length <= 100, "文档名称[{0}]不能超过100个字符数", x);
                    CommUtils.Assert(!folder.Files.Any(y => y.FileSeries.Name == x),
                        "文件[{0}]已经存在", x);
                });
                CommUtils.AssertHasContent(fileSeriesNames, "文档名称不能为空");
                //  CommUtils.AssertEquals(fileSeriesNames.Count, files.Count, "上传文件数和fileSeriesNames数不一致");

                var fileGuids = new List<string>();
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    var index = Math.Max(file.FileName.LastIndexOf('\\'), file.FileName.LastIndexOf('/'));
                    var fileName = index < 0 ? file.FileName : file.FileName.Substring(index + 1);

                    CommUtils.AssertHasContent(fileName, "文件名不能为空");
                    CommUtils.Assert(fileName.Length <= 100, "选择的文件名称[{0}]不能超过100个字符数", fileName);
                    CommUtils.Assert(!folder.Files.Any(x => x.FileSeries.Name == fileSeriesNameList[i]),
                        "文件[{0}]已经存在", fileSeriesNameList[i]);

                    var memoryStream = new MemoryStream(new BinaryReader(file.InputStream).ReadBytes(file.ContentLength));
                    var newFile = Platform.Repository.AddFile(fileName, memoryStream);
                    fileGuids.Add(newFile.Guid);

                    dict[newFile] = file;
                }

                var result = new List<DMSFile>();
                string operationType = "上传";
                string fileSeriesGuid = "";

                if (dict.Keys.Count == 0)
                {
                    var isExistTemplateType = string.IsNullOrWhiteSpace(templateType);
                    var dmsFileSeriesTemplateType = DmsFileSeriesTemplateType.None;
                    if (!isExistTemplateType)
                    {
                        dmsFileSeriesTemplateType = CommUtils.ParseEnum<DmsFileSeriesTemplateType>(templateType);
                    }

                    operationType = "创建";
                    fileSeriesNameList.ForEach(x =>
                    {
                        var newFileSeries = NewFileSeries(x, folder);
                        fileSeriesGuid = newFileSeries.Guid;

                        m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, fileSeriesGuid, "文件夹[" + folder.Instance.Name + "]中" + operationType + "文件系列" + x);
                        if (!isExistTemplateType && dmsFileSeriesTemplateType != DmsFileSeriesTemplateType.None)
                        {
                            NewDMSFileSeriesTemplate(newFileSeries.Id, dmsFileSeriesTemplateType);
                        }
                    });
                }

                for (int i = 0; i < dict.Keys.Count; i++)
                {
                    var repoFile = dict.Keys.ElementAt(i);
                    var fileSeriesName = fileSeriesNameList[i];
                    var dmsFileSeries = new DMSFileSeries();
                    dmsFileSeries = NewFileSeries(fileSeriesName, folder);
                    fileSeriesGuid = dmsFileSeries.Guid;

                     var now = DateTime.Now;
                    DMSFile dmsFile = new DMSFile();
                    dmsFile.DMSId = dms.Instance.Id;
                    dmsFile.DMSFileSeriesId = dmsFileSeries.Id;

                    dmsFile.RepoFileId = repoFile.Id;
                    dmsFile.Name = repoFile.Name;
                    dmsFile.Description = description ?? string.Empty;

                    dmsFile.Size = dict[repoFile].ContentLength;
                    dmsFile.Version = 1;

                    dmsFile.LastModifyUserName = CurrentUserName;
                    dmsFile.LastModifyTime = now;
                    dmsFile.CreateUserName = CurrentUserName;
                    dmsFile.CreateTime = now;

                    result.Add(m_dbAdapter.DMSFile.Create(dmsFile));

                    m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid,dmsFileSeries.Guid, "文件夹[" + folder.Instance.Name + "]中" + operationType + "文件" +fileSeriesName);

                }
                return ActionUtils.Success(fileSeriesGuid);
            });
        }

        [HttpPost]
        public ActionResult AutoGenerateFileNamesByTemplateType(string projectGuid, string fileTemplateType)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrWhiteSpace(fileTemplateType) && fileTemplateType != "None", "模板类型不能为None");
                var templateType = CommUtils.ParseEnum<DmsFileSeriesTemplateType>(fileTemplateType);

                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Execute);
                var cnabsNotes = dms.ProjectLogicModel.Notes;
                var fileNames = new List<string>();

                switch (templateType)
                {
                    case DmsFileSeriesTemplateType.IncomeDistributionReport:
                        fileNames.Add("收益分配报告.docx");
                        break;
                    case DmsFileSeriesTemplateType.SpecialPlanTransferInstruction:
                        foreach (var cnabsNote in cnabsNotes)
                        {
                            var name = "划款指令-" + cnabsNote.NoteName + "（" + cnabsNote.SecurityCode + "）.docx";
                            fileNames.Add(name);
                        }
                        fileNames.Add("划款指令（托管费）.docx");
                        fileNames.Add("划款指令（资产服务机构费用）.docx");
                        break;
                    case DmsFileSeriesTemplateType.CashInterestRateConfirmForm:
                        foreach (var cnabsNote in cnabsNotes)
                        {
                            var name = "兑付兑息确认表-" + cnabsNote.NoteName + "（" + cnabsNote.SecurityCode + "）.docx";
                            fileNames.Add(name);
                        }
                        break;
                    case DmsFileSeriesTemplateType.InterestPaymentPlanApplication:
                        foreach (var cnabsNote in cnabsNotes)
                        {
                            var noteName = cnabsNote.NoteName;
                            if (noteName.LastIndexOf("级") == (noteName.Length- 1))
                            {
                                noteName = noteName.Substring(0, noteName.Length - 1);
                            }
                            var name = "付息方案-" + noteName + "级-" + cnabsNote.SecurityCode + ".docx";
                            fileNames.Add(name);
                        }
                        break;
                    default:
                        break;
                }

                return ActionUtils.Success(fileNames);
            });
        }

        private DMSFileSeries NewFileSeries(string fileSeriesName, DMSFolderLogicModel folder)
        {
            var now = DateTime.Now;
            var dmsFileSeries = new DMSFileSeries();
            dmsFileSeries.DMSId = folder.Instance.DMSId;
            dmsFileSeries.DMSFolderId = folder.Instance.Id;
            dmsFileSeries.Name = fileSeriesName;
            dmsFileSeries.LastModifyUserName = CurrentUserName;
            dmsFileSeries.LastModifyTime = now;
            dmsFileSeries.CreateUserName = CurrentUserName;
            dmsFileSeries.CreateTime = now;
            return m_dbAdapter.DMSFileSeries.Create(dmsFileSeries);
        }

        private DMSFileSeriesTemplate NewDMSFileSeriesTemplate(int dmsFileSeriesId, DmsFileSeriesTemplateType fileSeriesTemplateType)
        {
            var dmsFileSeriesTemplate = new DMSFileSeriesTemplate();
            dmsFileSeriesTemplate.FileSeriesId = dmsFileSeriesId;
            dmsFileSeriesTemplate.TemplateType = fileSeriesTemplateType;
            return m_dbAdapter.DMSFileSeriesTemplate.New(dmsFileSeriesTemplate);
        }


        //上传新版本文档
        [HttpPost]
        public ActionResult UploadFile(string projectGuid, string fileSeriesGuid, string description)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Execute);

                var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);
                CommUtils.AssertEquals(dms.Instance.Id, dmsFolder.DMSId,
                    "文件[fileSeriesGuid={0}]不在DMS[DMSGuid={1}]中，请刷新后再试",
                    fileSeriesGuid, dms.Instance.Guid);

                var files = Request.Files;
                CommUtils.Assert(files.Count > 0, "请选择上传文件");
                CommUtils.AssertEquals(files.Count, 1, "只能上传一个文件");

                var file = files[0];
                var index = Math.Max(file.FileName.LastIndexOf('\\'), file.FileName.LastIndexOf('/'));
                var fileName = index < 0 ? file.FileName : file.FileName.Substring(index + 1);

                CommUtils.Assert(file.ContentLength > 0, "上传文件不能为空");
                CommUtils.AssertHasContent(fileName, "文件名不能为空");
                CommUtils.Assert(fileName.Length <= 100, "文件名不能超过100个字符数");

                var memoryStream = new MemoryStream(new BinaryReader(file.InputStream).ReadBytes(file.ContentLength));
                var repoFile = Platform.Repository.AddFile(fileName, memoryStream);

                var allFiles = m_dbAdapter.DMSFile.GetFilesByFileSeriesId(dmsFileSeries.Id);
                var currentVer = allFiles.Count == 0 ? 0 : allFiles.Max(x => x.Version);

                DMSFile newDMSFile = new DMSFile();
                newDMSFile.DMSId = dms.Instance.Id;
                newDMSFile.DMSFileSeriesId = dmsFileSeries.Id;

                newDMSFile.RepoFileId = repoFile.Id;
                newDMSFile.Name = repoFile.Name;
                newDMSFile.Description = description ?? string.Empty;

                newDMSFile.Size = file.ContentLength;
                newDMSFile.Version = currentVer + 1;

                var now = DateTime.Now;
                newDMSFile.LastModifyUserName = CurrentUserName;
                newDMSFile.LastModifyTime = now;
                newDMSFile.CreateUserName = CurrentUserName;
                newDMSFile.CreateTime = now;

                newDMSFile = m_dbAdapter.DMSFile.Create(newDMSFile);

                var comment = "上传文件["
                    + dmsFileSeries.Name + "]的第" + newDMSFile.Version + "版本";
                m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid,fileSeriesGuid, comment);

                return ActionUtils.Success(newDMSFile.Guid);
            });
        }

        //获取文件夹下的所有文件夹
        [HttpPost]
        public ActionResult GetFolders(string projectGuid, string folderGuid)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Read);
                var folder = dms.FindFolder(folderGuid);

                CommUtils.AssertNotNull(folder, "找不到文件夹[folderGuid={0} DMSGuid={1}]，请刷新后再试",
                    folderGuid, dms.Instance.Guid);

                Platform.UserProfile.Precache(folder.SubFolders.Select(x => x.Instance.CreateUserName));

                var result = folder.SubFolders.Select(x => new
                {
                    folderGuid = x.Instance.Guid,
                    folderName = x.Instance.Name,
                    description = x.Instance.Description,
                    createUser = Platform.UserProfile.Get(x.Instance.CreateUserName)==null? x.Instance.CreateUserName: Platform.UserProfile.Get(x.Instance.CreateUserName).RealName,
                    createTime = Toolkit.DateTimeToString(x.Instance.CreateTime),
                });

                return ActionUtils.Success(result);
            });
        }

        //获取文件夹下的所有文件
        [HttpPost]
        public ActionResult GetFiles(string projectGuid, string folderGuid)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Read);
                var folder = dms.FindFolder(folderGuid);
                folder.IgnoreNull = false;

                CommUtils.AssertNotNull(folder, "找不到文件夹[folderGuid={0} DMSGuid={1}]，请刷新后再试",
                    folderGuid, dms.Instance.Guid);

                Platform.UserProfile.Precache(folder.Files.Select(x => x.LatestVerFile.CreateUserName));
                Platform.UserProfile.Precache(folder.Files.Select(x => x.LatestVerFile.LastModifyUserName));

                var result = folder.Files.Select(x => new
                {
                    folderGuid = folderGuid,
                    latestVerFileGuid = x.LatestVerFile.Guid,
                    fileSeriesGuid = x.FileSeries.Guid,
                    fileSeriesName = x.FileSeries.Name,
                    latestVerFileName = x.LatestVerFile.Name,
                    version = x.LatestVerFile.Version,
                    description = x.LatestVerFile.Description,
                    size = FileUtils.FormatSize(x.LatestVerFile.Size),
                    createTime = Toolkit.DateTimeToString(x.LatestVerFile.CreateTime),
                    createUser = Platform.UserProfile.Get(x.LatestVerFile.CreateUserName)==null? x.LatestVerFile.CreateUserName: Platform.UserProfile.Get(x.LatestVerFile.CreateUserName).RealName,
                    lastModifyTime = Toolkit.DateTimeToString(x.LatestVerFile.LastModifyTime),
                    lastModifyUser = Platform.UserProfile.Get(x.LatestVerFile.LastModifyUserName)==null? x.LatestVerFile.LastModifyUserName: Platform.UserProfile.Get(x.LatestVerFile.LastModifyUserName).RealName,
                });
                return ActionUtils.Success(result);
            });
        }

        //class DMSResultFile {


        //}



        //获取文档所有历史版本
        [HttpPost]
        public ActionResult GetFileSeriesDetail(string projectGuid, string fileSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Read);

                var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);
                CommUtils.AssertEquals(dmsFolder.DMSId, dms.Instance.Id, "文件[fileSeriesGuid={0}]不在DMS[{1}]中",
                    fileSeriesGuid, dms.Instance.Guid);

                var allFiles = m_dbAdapter.DMSFile.GetFilesByFileSeriesId(dmsFileSeries.Id);
                allFiles = allFiles.OrderByDescending(x => x.Version).ToList();

                Platform.UserProfile.Precache(allFiles.Select(x => x.CreateUserName));
                Platform.UserProfile.Precache(allFiles.Select(x => x.LastModifyUserName));

                var result = allFiles.Select(x =>
                    new
                    {
                        fileSeriesName = dmsFileSeries.Name,
                        fileSeriesGuid = dmsFileSeries.Guid,
                        fileGuid = x.Guid,
                        fileName = x.Name,
                        version = x.Version,
                        description = x.Description,
                        size = FileUtils.FormatSize(x.Size),
                        createTime = Toolkit.DateTimeToString(x.CreateTime),
                        createUser = Platform.UserProfile.Get(x.CreateUserName)==null? x.CreateUserName:Platform.UserProfile.Get(x.CreateUserName).RealName,
                        lastModifyTime = Toolkit.DateTimeToString(x.LastModifyTime),
                        lastModifyUser = Platform.UserProfile.Get(x.LastModifyUserName)==null?x.LastModifyUserName: Platform.UserProfile.Get(x.LastModifyUserName).RealName,
                    }
                );

                var fileSeriesTemplate = m_dbAdapter.DMSFileSeriesTemplate.GetByFileSeriesId(dmsFileSeries.Id);
                var templateType = fileSeriesTemplate == null ? "None" : fileSeriesTemplate.TemplateType.ToString();
                return ActionUtils.Success(new {
                    historicalFiles = result,
                    templateType = templateType,
                });
            });
        }

        //下载文档
        [HttpPost]
        public ActionResult DownloadFile(string projectGuid, List<string> fileGuids)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Read);
                var resourceGuids = new List<string>();
                List<Tuple<string, string>> fileTuples = new List<Tuple<string, string>>();

                foreach (var fileGuid in fileGuids)
                {
                    CommUtils.Assert(m_dbAdapter.DMSFile.isExistDMSFile(fileGuid), "找不到文件fileGuid[{0}]，请刷新页面后重试", fileGuid);
                    var dmsFile = m_dbAdapter.DMSFile.GetByGuid(fileGuid);
                    var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetById(dmsFile.DMSFileSeriesId);
                    var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);

                    CommUtils.AssertEquals(dmsFolder.DMSId, dms.Instance.Id,
                        "FileGuid[{0}]不在DMS[{1}]中", fileGuid, dms.Instance.Guid);

                    var repoFile = Platform.Repository.GetFile(dmsFile.RepoFileId);
                    var resultFilePath = repoFile.GetFilePath();

                    fileTuples.Add(Tuple.Create(dmsFile.Name, resultFilePath));

                    var comment = "下载文件[" + dmsFile.Name + "]的第" + dmsFile.Version + "版本";
                    m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, dmsFileSeries.Guid, comment);
                }

                var fileNames = new List<string>();
                foreach (var fileTuple in fileTuples)
                {
                    var resource = ResourcePool.RegisterFilePath(CurrentUserName, fileTuple.Item1, fileTuple.Item2);
                    resourceGuids.Add(resource.Guid.ToString());

                    fileNames.Add("[" + fileTuple.Item1 + "]");
                }


                return ActionUtils.Success(resourceGuids);
            });
        }

        public ActionResult DownloadCompressFiles(string projectGuid, string fileGuidsText, string folderGuidsText, bool isSearchFile = false)
        {
            var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Read);

            var fileGuids = string.IsNullOrWhiteSpace(fileGuidsText) ? new List<string>() : CommUtils.Split(fileGuidsText).ToList();
            var folderGuids = string.IsNullOrWhiteSpace(folderGuidsText) ? new List<string>() : CommUtils.Split(folderGuidsText).ToList();
            CommUtils.Assert(fileGuids.Count != 0 || folderGuids.Count != 0, "必须选择一个[文件/文件夹]进行下载");

            //获取待压缩文件夹下的所有文件/文件夹路径
            var dictAllFiles = new Dictionary<string, string>();
            var allEmptyFolderPath = new List<string>();
            if (folderGuids.Count != 0)
            {
                var folderAndFilePath = new Tuple<List<string>, Dictionary<string, string>>(new List<string>(), new Dictionary<string, string>());
                folderAndFilePath = GetCompressFilesPath(dms, folderGuids, null, folderAndFilePath);
                dictAllFiles = folderAndFilePath.Item2;
                allEmptyFolderPath = folderAndFilePath.Item1;
            }

            //获取所有待压缩文件的路径
            var files = new List<DMSFile>();
            foreach (var fileGuid in fileGuids)
            {
                CommUtils.Assert(m_dbAdapter.DMSFile.isExistDMSFile(fileGuid), "找不到文件fileGuid[{0}]，请刷新页面后重试", fileGuid);
                var dmsFile = m_dbAdapter.DMSFile.GetByGuid(fileGuid);
                var projectId = m_dbAdapter.DMSTask.GetProjectIdByDMSId(dms.Instance.Id);
                CommUtils.AssertEquals(dmsFile.DMSId, dms.Instance.Id,
                    "找不到文件[fileGuid={0}][DMSGuid={1}]", fileGuid, dms.Instance.Guid);

                dictAllFiles[fileGuid] = dmsFile.Name;
                files.Add(dmsFile);
            }

            //处理重名文件
            var keys = dictAllFiles.Keys.ToList();
            var dictResultAllFiles = new Dictionary<string, string>();
            var allFilesPath = new List<string>();

            foreach (var key in keys)
            {
                dictResultAllFiles[key] = GetNewPathForDupes(dictAllFiles[key], allFilesPath);
                allFilesPath.Add(dictResultAllFiles[key]);
            }

            //压缩-结束
            var resultFilePath = new List<string>();
            var dictResultFilePath = new Dictionary<string, string>();
            foreach (var fileGuid in dictResultAllFiles.Keys)
            {
                var dmsFile = m_dbAdapter.DMSFile.GetByGuid(fileGuid);
                var repoFile = Platform.Repository.GetFile(dmsFile.RepoFileId);
                var filePath = repoFile.GetFilePath();

                resultFilePath.Add(filePath);
                dictResultFilePath[filePath] = dictResultAllFiles[fileGuid];
            }

            var ms = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(ms);
            zipStream.SetLevel(3);

            CompressFiles(zipStream, WebConfigUtils.RepositoryFilePath, resultFilePath, dictResultFilePath, allEmptyFolderPath);

            zipStream.Flush();
            zipStream.Finish();

            ms.Seek(0, SeekOrigin.Begin);

            //设置压缩包名称
            var resultZipFileName = string.Empty;
            if (isSearchFile)
            {
                resultZipFileName = "工作文件";
            }
            else
            {
                if (fileGuids.Count == 0 && folderGuids.Count == 1)
                {
                    var currentFolder = dms.FindFolder(folderGuids.First());
                    var currFolder = currentFolder.Instance;
                    resultZipFileName = currFolder.Name;

                    var fileSeriesList=  currentFolder.Files.GroupToDictList(x => x.FileSeries);
                    fileSeriesList.ToList().ForEach(x => {
                        m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, x.Key.Guid, "下载[" + x.Key.Name + "]合并于[" + resultZipFileName + ".zip]");
                    });
                }

                if (fileGuids.Count != 0 && folderGuids.Count == 0)
                {
                    var fileSeries = files.ConvertAll(x => x.DMSFileSeriesId).ToList();
                    var currFileSeries = m_dbAdapter.DMSFileSeries.GetById(files.First().DMSFileSeriesId);
                    if (fileSeries.Distinct().ToList().Count == 1)
                    {
                        resultZipFileName = currFileSeries.Name;
                    }
                    else
                    {
                        var currFolder = m_dbAdapter.DMSFolder.GetById(currFileSeries.DMSFolderId);
                        resultZipFileName = currFolder.ParentFolderId.HasValue ? currFolder.Name : "工作文件";
                    }
                }

                if (string.IsNullOrEmpty(resultZipFileName))
                {
                    var currFolder = dms.FindFolder(folderGuids.First()).Instance;
                    var parentFolder = m_dbAdapter.DMSFolder.GetById(currFolder.ParentFolderId.Value);

                    resultZipFileName = parentFolder.ParentFolderId.HasValue ? parentFolder.Name : "工作文件";
                }


            }

         //   m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, "下载[" + resultZipFileName + ".zip]");
            return File(ms, "application/octet-stream", resultZipFileName + ".zip");
        }

        //删除一些folder
        [HttpPost]
        public ActionResult RemoveFolders(string projectGuid, List<string> folderGuids)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Write);

                var removeFolders = new List<DMSFolder>();
                var allFolders = dms.AllFolders;

                foreach (var folderGuid in folderGuids)
                {
                    CommUtils.Assert(allFolders.Any(x => x.Guid == folderGuid),
                        "找不到文件夹[{0}]，请刷新后再试", folderGuid);
                }

                removeFolders = allFolders.Where(x => folderGuids.Contains(x.Guid)).ToList();
                removeFolders.ForEach(x => CommUtils.Assert(IsCurrentUser(x.CreateUserName),
                    "当前用户[{0}]不是[{1}]的创建者[{2}]", CurrentUserName, x.Name, x.CreateUserName));
                var removeFolderCount = allFolders.RemoveAll(x => removeFolders.Any(removeFolder => removeFolder.Id == x.Id));

                //标记删除文件夹下的子文件夹
                while (removeFolderCount != 0)
                {
                    removeFolders.AddRange(allFolders.Where(x => x.ParentFolderId.HasValue
                        && removeFolders.Any(removeFolder => removeFolder.Id == x.ParentFolderId.Value)));
                    removeFolderCount = allFolders.RemoveAll(x => removeFolders.Any(removeFolder => removeFolder.Id == x.Id));
                }

                removeFolders.ForEach(x => m_dbAdapter.DMSFolder.Remove(x));

              //  var folderNames = removeFolders.ConvertAll(x => "[" + x.Name + "]").ToArray();
               // m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, "删除文件夹" + string.Join("，", folderNames));

                return ActionUtils.Success(removeFolders.Count);
            });
        }

        [HttpPost]
        public ActionResult RemoveFileSeries(string projectGuid, List<string> fileSeriesGuids)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Write);
                var fileSeriesList = new List<DMSFileSeries>();
                foreach (var fileSeriesGuid in fileSeriesGuids)
                {
                    var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                    var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);

                    CommUtils.AssertEquals(dmsFolder.DMSId, dms.Instance.Id,
                        "fileSeriesGuid[{0}]不在DMS[{1}]中", fileSeriesGuid, dms.Instance.Guid);

                    CommUtils.Assert(IsCurrentUser(dmsFileSeries.CreateUserName),
                        "当前用户[{0}]不是[{1}]的创建者[{2}]", CurrentUserName, dmsFileSeries.Name, dmsFileSeries.CreateUserName);

                    fileSeriesList.Add(dmsFileSeries);
                    m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid,fileSeriesGuid, "删除文件" + fileSeriesGuid);
                }

                var result = m_dbAdapter.DMSFileSeries.Remove(fileSeriesList);

                 return ActionUtils.Success(result);
            });
        }


        [HttpPost]
        public ActionResult RemoveFile(string projectGuid, List<string> fileGuids)
        {
            return ActionUtils.Json(() =>
            {
                var dms = GetDMSAndCheckPermission(projectGuid, PermissionType.Write);
                var dmsFiles = new List<DMSFile>();
                foreach (var fileGuid in fileGuids)
                {
                    var dmsFile = m_dbAdapter.DMSFile.GetByGuid(fileGuid);
                    var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetById(dmsFile.DMSFileSeriesId);
                    var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);

                    CommUtils.AssertEquals(dmsFolder.DMSId, dms.Instance.Id,
                        "FileGuid[{0}]不在DMS[{1}]中", fileGuid, dms.Instance.Guid);

                    CommUtils.Assert(IsCurrentUser(dmsFile.CreateUserName),
                        "当前用户[{0}]不是[{1}]的创建者[{2}]", CurrentUserName, dmsFile.Name, dmsFile.CreateUserName);

                    dmsFiles.Add(dmsFile);
                }

                var fileNames = new List<string>();
                foreach (var dmsFile in dmsFiles)
                {
                    m_dbAdapter.DMSFile.Remove(dmsFile);
                    fileNames.Add("[" + dmsFile.Name + "]");
                }

               // m_dbAdapter.DMSProjectLog.AddDmsProjectLog(projectGuid, "删除文件" + string.Join("，", fileNames.ToArray()));

                return ActionUtils.Success(dmsFiles.Count);
            });
        }


        [HttpPost]
        public ActionResult GetLog(string projectGuid,string fileSeriesGuid, int? cachedRecordCount)
        {
            return ActionUtils.Json(() =>
            {
                var logs = m_dbAdapter.DMSProjectLog.GetLogs(projectGuid,fileSeriesGuid);
                // if (logs == null
                //    || (cachedRecordCount.HasValue && cachedRecordCount.Value == logs.Count))
                //{
                //    return ActionUtils.Success(new List<object>());
                //}

                Platform.UserProfile.Precache(logs.Select(x => x.TimeStampUserName));
                var result = logs.Select(x => new
                {
                    Time = Toolkit.DateTimeToString(x.TimeStamp),
                    UserName = Platform.UserProfile.GetDisplayRealNameAndUserName(x.TimeStampUserName),
                    Comment = x.Comment,
                 });

                return ActionUtils.Success(result);
            });
        }


        private Tuple<List<string>, Dictionary<string, string>> GetCompressFilesPath(DMSLogicModel dmsLogicModel,
            List<string> folderGuids, string currFolderName, Tuple<List<string>, Dictionary<string, string>> folderAndFilePath)
        {
            foreach (var folderGuid in folderGuids)
            {
                var parentFilePath = currFolderName;
                var folder = dmsLogicModel.FindFolder(folderGuid);
                CommUtils.AssertNotNull(folder, "找不到文件夹[folderGuid={0}][DMS={1}]，请刷新后再试",
                    folderGuid, dmsLogicModel.Instance.Guid);

                var files = folder.Files;

                parentFilePath += folder.Instance.Name + FileUtils.PathSeparator;
                files.ForEach(x => folderAndFilePath.Item2[x.LatestVerFile.Guid] = parentFilePath + x.LatestVerFile.Name);
                if (files.Count == 0)
                {
                    folderAndFilePath.Item1.Add(parentFilePath);
                }
                var subFolders = folder.SubFolders;
                if (subFolders.Count > 0)
                {
                    var subFolderGuids = subFolders.ConvertAll(x => x.Instance.Guid);
                    subFolders.ForEach(x => GetCompressFilesPath(dmsLogicModel, subFolderGuids, parentFilePath, folderAndFilePath));
                }
            }
            return folderAndFilePath;
        }

        private void CompressFiles(ZipOutputStream zipStream, string folder, List<string> fileNames,
            Dictionary<string, string> dictFolderNames, List<string> allEmptyFolderPath)
        {
            var nameList = new List<string>();

            foreach (var emptyFolderPath in allEmptyFolderPath)
            {
                var folderPath = emptyFolderPath.Substring(0, emptyFolderPath.Length - 1) + "/";
                ZipEntry zeOutput = new ZipEntry(folderPath);
                zeOutput.IsUnicodeText = true;
                zipStream.PutNextEntry(zeOutput);
            }

            foreach (var file in fileNames)
            {
                var filePath = Path.Combine(folder, file);
                if (!System.IO.File.Exists(filePath))
                {
                    continue;
                }

                FileInfo fileInfo = new FileInfo(filePath);

                string entryName = ZipEntry.CleanName(fileInfo.Name);
                ZipEntry newEntry = new ZipEntry(entryName);
                if (nameList.Contains(entryName))
                {
                    var newEntryName = GetNewPathForDupes(entryName, nameList);
                    newEntry = new ZipEntry(newEntryName);
                    nameList.Add(newEntryName);
                }
                else
                {
                    nameList.Add(entryName);
                }

                if (dictFolderNames != null)
                {
                    newEntry = new ZipEntry(dictFolderNames[file]);
                }

                newEntry.DateTime = fileInfo.LastWriteTime;
                newEntry.Size = fileInfo.Length;
                newEntry.IsUnicodeText = true;

                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = System.IO.File.OpenRead(filePath))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
        }

        private string GetNewPathForDupes(string path, List<string> fileNameList)
        {
            string newFullPath = path.Trim();

            if (fileNameList.Contains(path))
            {
                string directory = Path.GetDirectoryName(path);
                string filename = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                int counter = 1;
                do
                {
                    string newFilename = string.Format("{0}({1}){2}", filename, counter, extension);
                    newFullPath = Path.Combine(directory, newFilename);
                    counter++;
                } while (fileNameList.Contains(newFullPath));
            }
            return newFullPath;
        }

    }
}