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
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class DMSController : BaseController
    {
        //获取所有folder
        [HttpPost]
        public ActionResult GetAllFolders(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                dynamic tree = projectLogicModel.DMS.Root.ToTree();
                return ActionUtils.Success(tree);
            });
        }

        //获取一个folder的路径
        [HttpPost]
        public ActionResult GetFolderPath(string projectGuid, string folderGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

                var folder = projectLogicModel.DMS.FindFolder(folderGuid);

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

        //创建一些folder
        [HttpPost]
        public ActionResult CreateFolders(string projectGuid, string parentFolderGuid, List<string> folderNames, List<string> folderDescriptions)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);

                ValidateUtils.Name(folderNames, "文件夹名称", 100).FileName();

                CommUtils.AssertEquals(folderNames.Count, folderDescriptions.Count, "传入folderNames和folderDescriptions长度不相等");
                CommUtils.AssertEquals(folderNames.Select(x => x.ToLower()).Distinct().Count(),
                    folderNames.Count, "传入了重复的文件夹名");

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var dms = projectLogicModel.DMS.Instance;

                var parentFolder = m_dbAdapter.DMSFolder.GetByGuid(parentFolderGuid);
                CommUtils.AssertEquals(parentFolder.DMSId, dms.Id, "传入projectGuid和folderGuid不匹配");

                var sibbingFolders = projectLogicModel.DMS.AllFolders.Where(x => x.ParentFolderId.HasValue
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
                    folder.DMSId = dms.Id;
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

        //删除一些folder
        [HttpPost]
        public ActionResult RemoveFolders(string projectGuid, List<string> folderGuids)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

                var removeFolders = new List<DMSFolder>();
                var allFolders = projectLogicModel.DMS.AllFolders;

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
                return ActionUtils.Success(removeFolders.Count);
            });
        }

        [HttpPost]
        public ActionResult GetLatestFiles(string projectGuid, int limitFileCount = 10)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                CommUtils.Assert(limitFileCount <= 100, "获取最近文件更新动态数不能大于100");

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var allFolders = projectLogicModel.DMS.AllFolders;
                var allFolderIds = allFolders.Select(x => x.Id);
                var fileSeriesList = m_dbAdapter.DMSFileSeries.GetFileSeriesByFolderIds(allFolderIds);
                var allFileSeriesIds = fileSeriesList.Select(x => x.Id);

                var allFiles = m_dbAdapter.DMSFile.GetFilesByFileSeriesIds(allFileSeriesIds);


                var resultFiles = allFiles.OrderByDescending(x => x.LastModifyTime)
                    .Take(Math.Min(100, limitFileCount));

                var dictAllFolders = allFolders.ToDictionary(x => x.Id);
                var dictFileSeriesList = fileSeriesList.ToDictionary(x => x.Id);

                Platform.UserProfile.Precache(resultFiles.Select(x => x.CreateUserName));
                Platform.UserProfile.Precache(resultFiles.Select(x => x.LastModifyUserName));

                var result = resultFiles.Select(x => new
                {
                    fileName = x.Name,
                    version = x.Version,
                    description = x.Description,
                    size = FileUtils.FormatSize(x.Size),
                    createTime = Toolkit.DateTimeToString(x.CreateTime),
                    createUser = Platform.UserProfile.Get(x.CreateUserName),
                    lastModifyTime = Toolkit.DateTimeToString(x.LastModifyTime),
                    lastModifyUser = Platform.UserProfile.Get(x.LastModifyUserName),
                    folderGuid = dictAllFolders[dictFileSeriesList[x.DMSFileSeriesId].DMSFolderId].Guid,
                    fileSeriesName = dictFileSeriesList[x.DMSFileSeriesId].Name
                });

                return ActionUtils.Success(result);
            });
        }

        //获取文件夹下的所有文件夹
        [HttpPost]
        public ActionResult GetFolders(string projectGuid, string folderGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);
                CommUtils.AssertNotNull(folder, "在Project[{0}]中，找不到folderGuid[{1}]，请刷新后再试",
                    projectLogicModel.Instance.Name, folderGuid);

                Platform.UserProfile.Precache(folder.SubFolders.Select(x => x.Instance.CreateUserName));

                var result = folder.SubFolders.Select(x => new
                {
                    folderGuid = x.Instance.Guid,
                    folderName = x.Instance.Name,
                    description = x.Instance.Description,
                    createUser = Platform.UserProfile.Get(x.Instance.CreateUserName),
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
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);
                CommUtils.AssertNotNull(folder, "在Project[{0}]中，找不到folderGuid[{1}]，请刷新后再试",
                    projectLogicModel.Instance.Name, folderGuid);

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
                    createUser = Platform.UserProfile.Get(x.LatestVerFile.CreateUserName),
                    lastModifyTime = Toolkit.DateTimeToString(x.LatestVerFile.LastModifyTime),
                    lastModifyUser = Platform.UserProfile.Get(x.LatestVerFile.LastModifyUserName),
                });

                return ActionUtils.Success(result);
            });
        }

        //根据关键字获取产品下当前文件夹的所有子文件夹、文件的信息路径等
        [HttpPost]
        public ActionResult SearchAllFolderAndFileInfoByKeyword(string projectGuid, string folderGuid, string searchText)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(searchText, "请输入搜索内容");
                searchText = searchText.ToLower();
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

                //获取当前文件夹的父级文件目录
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);
                var currentFolderCatalog = folder.GetParentFolderPath();

                //获取当前产品下的所有文件夹及文件的【文件结构路径】
                var folderAndFilePath = new Tuple<Dictionary<string, string>, Dictionary<string, string>>(new Dictionary<string, string>(), new Dictionary<string, string>());
                folderAndFilePath = GetAllFolderFilePathByfolderGuid(projectLogicModel,
                new List<string> { folderGuid }, currentFolderCatalog, folderAndFilePath);
                var dictAllFilesPath = folderAndFilePath.Item1;
                var dictAllFolderPath = folderAndFilePath.Item2;

                //截取所有【文件结构路径】的【父级路径】替换原来的路径
                dictAllFilesPath.Keys.ToList().ForEach(x => dictAllFilesPath[x] = dictAllFilesPath[x].Substring(0, dictAllFilesPath[x].LastIndexOf(FileUtils.PathSeparator)));
                dictAllFolderPath.Keys.ToList().ForEach(x => dictAllFolderPath[x] = dictAllFolderPath[x].Substring(0, dictAllFolderPath[x].LastIndexOf(FileUtils.PathSeparator)));

                //获取包含关键字的文件和文件夹
                var resultFileAndFolder = GetFilesAndFoldersBySearchText(dictAllFilesPath, dictAllFolderPath, searchText);
                var resultFile = resultFileAndFolder.Item1;
                var resultFolder = resultFileAndFolder.Item2;
                var resultFileSeries = resultFileAndFolder.Item3;

                var folderParentFolderIds = resultFolder.ConvertAll(x => x.ParentFolderId.Value);
                var folderParentFolder = m_dbAdapter.DMSFolder.GetByIds(folderParentFolderIds);
                var dictFolderParentFolder = folderParentFolder.ToDictionary(x => x.Id);

                var fileParentFolderIds = resultFileSeries.ConvertAll(x => x.DMSFolderId);
                var fileParentFolder = m_dbAdapter.DMSFolder.GetByIds(fileParentFolderIds);
                var dictFileParentFolder = fileParentFolder.ToDictionary(x => x.Id); ;

                var dictFileSeries = resultFileSeries.ToDictionary(x => x.Id);

                var result = new
                {
                    folders = resultFolder.ConvertAll(x =>
                    {
                        return new
                        {
                            catalog = dictAllFolderPath[x.Guid].Contains(FileUtils.PathSeparator) ? "所有文件" + dictAllFolderPath[x.Guid].Substring(4) : "所有文件",
                            parentFolderGuid = dictFolderParentFolder[x.ParentFolderId.Value].Guid,
                            folderGuid = x.Guid,
                            folderName = x.Name,
                            description = x.Description,
                            createUser = Platform.UserProfile.Get(x.CreateUserName),
                            createTime = Toolkit.DateTimeToString(x.CreateTime),
                            highLightKeywordIndex = GetHighLightKeyword(x.Name, searchText),
                        };
                    }),
                    files = resultFile.ConvertAll(x =>
                    {
                        var fileSeries = dictFileSeries[x.DMSFileSeriesId];
                        return new
                        {
                            folderGuid = dictFileParentFolder[fileSeries.DMSFolderId].Guid,
                            catalog = dictAllFilesPath[x.Guid].Contains(FileUtils.PathSeparator) ? "所有文件" + dictAllFilesPath[x.Guid].Substring(4) : "所有文件",
                            latestVerFileGuid = x.Guid,
                            fileSeriesGuid = fileSeries.Guid,
                            fileSeriesName = fileSeries.Name,
                            latestVerFileName = x.Name,
                            version = x.Version,
                            description = x.Description,
                            size = FileUtils.FormatSize(x.Size),
                            createTime = Toolkit.DateTimeToString(x.CreateTime),
                            createUser = Platform.UserProfile.Get(x.CreateUserName),
                            lastModifyTime = Toolkit.DateTimeToString(x.LastModifyTime),
                            lastModifyUser = Platform.UserProfile.Get(x.LastModifyUserName),
                            highLightKeywordIndex = GetHighLightKeyword(fileSeries.Name, searchText),
                        };
                    }),
                };
                return ActionUtils.Success(result);
            });
        }

        private List<GetHighLightKeywords> GetHighLightKeyword(string name, string searchText)
        {
            var highLightKeyword = new List<GetHighLightKeywords>();

            if (name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var temporaryName = name;
                var keyword1 = new GetHighLightKeywords();
                while (temporaryName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var str1 = temporaryName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
                    var keyword2 = new GetHighLightKeywords();
                    keyword2.Keyword = temporaryName.Substring(0, str1);
                    keyword2.isHighLight = false;
                    highLightKeyword.Add(keyword2);

                    temporaryName = temporaryName.Substring(str1);
                    var keyword3 = new GetHighLightKeywords();
                    keyword3.Keyword = temporaryName.Substring(0, searchText.Length);
                    keyword3.isHighLight = true;
                    highLightKeyword.Add(keyword3);
                    temporaryName = temporaryName.Substring(searchText.Length);
                }

                keyword1.Keyword = temporaryName;
                keyword1.isHighLight = false;
                highLightKeyword.Add(keyword1);
            }

            var emptyKeyword = highLightKeyword.Where(x => x.Keyword == "" && x.isHighLight == false).ToList();
            emptyKeyword.ForEach(x => highLightKeyword.Remove(x));
            return highLightKeyword;
        }

        private Tuple<List<DMSFile>, List<DMSFolder>, List<DMSFileSeries>> GetFilesAndFoldersBySearchText(Dictionary<string, string> dictAllFilesPath, Dictionary<string, string> dictAllFolderPath, string searchText)
        {
            var allFiles = dictAllFilesPath.Keys.ToList().ConvertAll(x => m_dbAdapter.DMSFile.GetByGuid(x));
            var allFolders = dictAllFolderPath.Keys.ToList().ConvertAll(x => m_dbAdapter.DMSFolder.GetByGuid(x));

            //获取文件对应的文件系列信息
            var allFileSeriesIds = allFiles.ConvertAll(x => x.DMSFileSeriesId);
            var allFileSeries = m_dbAdapter.DMSFileSeries.GetByIds(allFileSeriesIds);

            //获取包含关键字的文件
            var resultFileSeries = allFileSeries.Where(x => x.Name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            //获取包含关键字的文件夹
            var resultFolder = allFolders.Where(x => x.Name.ToLower().IndexOf(searchText) >= 0).ToList();

            //排序
            resultFileSeries.Sort((x, y) => x.Name.ToLower().IndexOf(searchText).CompareTo(y.Name.ToLower().IndexOf(searchText)));
            resultFolder.Sort((x, y) => x.Name.ToLower().IndexOf(searchText).CompareTo(y.Name.ToLower().IndexOf(searchText)));

            var dictResultFileSeries = resultFileSeries.ToDictionary(x => x.Id);
            var resultFiles = allFiles.Where(x => dictResultFileSeries.ContainsKey(x.DMSFileSeriesId)).ToList();

            return new Tuple<List<DMSFile>, List<DMSFolder>, List<DMSFileSeries>>(resultFiles, resultFolder, resultFileSeries);
        }

        private Tuple<Dictionary<string, string>, Dictionary<string, string>> GetAllFolderFilePathByfolderGuid(ProjectLogicModel projectLogicModel,
            List<string> folderGuids, string currFolderName, Tuple<Dictionary<string, string>, Dictionary<string, string>> folderAndFilePath)
        {
            foreach (var folderGuid in folderGuids)
            {
                var parentFilePath = currFolderName;
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);
                CommUtils.AssertNotNull(folder, "在Project[{0}]中，找不到folderGuid[{1}]，请刷新后再试",
                    projectLogicModel.Instance.Name, folderGuid);

                parentFilePath += folder.Instance.Name + FileUtils.PathSeparator;
                var subFolders = folder.SubFolders;
                subFolders.ForEach(x => folderAndFilePath.Item2[x.Instance.Guid] = parentFilePath + x.Instance.Name);

                var files = folder.Files;
                files.ForEach(x => folderAndFilePath.Item1[x.LatestVerFile.Guid] = parentFilePath + x.FileSeries.Name);

                if (subFolders.Count > 0)
                {
                    var subFolderGuids = subFolders.ConvertAll(x => x.Instance.Guid);
                    foreach (var subFolder in subFolders)
                    {
                        GetAllFolderFilePathByfolderGuid(projectLogicModel, subFolderGuids, parentFilePath, folderAndFilePath);
                    }
                }
            }

            return folderAndFilePath;
        }

        //创建新文档
        [HttpPost]
        public ActionResult CreateFile(string projectGuid, string folderGuid, string fileSeriesNames, string description)
        {
            List<string> fileSeriesNamesList = fileSeriesNames.Split(',').ToList();

            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);
                CommUtils.AssertNotNull(folder, "在Project[{0}]中，找不到folderGuid[{1}]，请刷新后再试",
                    projectLogicModel.Instance.Name, folderGuid);

                var dict = new Dictionary<RepositoryFile, HttpPostedFileBase>();

                var files = Request.Files;
                CommUtils.Assert(files.Count > 0, "请选择上传文件");
                //ValidateUtils.Name(fileSeriesNamesList, "文档名称", 100).FileName();
                CommUtils.AssertEquals(fileSeriesNamesList.Count, files.Count, "上传文件数和fileSeriesNames数不一致");

                var fileGuids = new List<string>();

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    var index = Math.Max(file.FileName.LastIndexOf('\\'), file.FileName.LastIndexOf('/'));
                    var fileName = index < 0 ? file.FileName : file.FileName.Substring(index + 1);

                    CommUtils.AssertHasContent(fileName, "文件名不能为空");
                    //CommUtils.Assert(fileName.Length <= 100, "选择的文件名称[{0}]不能超过200个字符数", fileName);
                    CommUtils.Assert(!folder.Files.Any(x => x.FileSeries.Name == fileSeriesNamesList[i]),
                        "文件[{0}]已经存在", fileSeriesNames[i]);

                    var memoryStream = new MemoryStream(new BinaryReader(file.InputStream).ReadBytes(file.ContentLength));
                    var newFile = Platform.Repository.AddFile(fileName, memoryStream);
                    fileGuids.Add(newFile.Guid);

                    dict[newFile] = file;
                }

                var result = new List<DMSFile>();
                for (int i = 0; i < dict.Keys.Count; i++)
                {
                    var repoFile = dict.Keys.ElementAt(i);
                    var fileSeriesName =Path.GetFileNameWithoutExtension(files[i].FileName);
                    if (dict.Keys.Count == 1) fileSeriesName = fileSeriesNamesList[i];//单个文件

                    var now = DateTime.Now;
                    var dmsFileSeries = new DMSFileSeries();
                    dmsFileSeries.DMSId = folder.Instance.DMSId;
                    dmsFileSeries.DMSFolderId = folder.Instance.Id;
                    dmsFileSeries.Name = fileSeriesName;
                    dmsFileSeries.LastModifyUserName = CurrentUserName;
                    dmsFileSeries.LastModifyTime = now;
                    dmsFileSeries.CreateUserName = CurrentUserName;
                    dmsFileSeries.CreateTime = now;
                    dmsFileSeries = m_dbAdapter.DMSFileSeries.Create(dmsFileSeries);


                    DMSFile dmsFile = new DMSFile();
                    dmsFile.DMSId = projectLogicModel.DMS.Instance.Id;
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
                }

                return ActionUtils.Success(true);
            });
        }

        //上传新版本文档
        [HttpPost]
        public ActionResult UploadFile(string projectGuid, string fileSeriesGuid, string description)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

                var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);
                var dms = m_dbAdapter.DMS.GetById(dmsFolder.DMSId);
                CommUtils.AssertEquals(dms.ProjectId, projectLogicModel.Instance.ProjectId,
                    "FileSeriesGuid[{0}]不在产品[{1}]中", fileSeriesGuid, projectLogicModel.Instance.Name);

                var files = Request.Files;

                CommUtils.Assert(files.Count > 0, "请选择上传文件");
                // CommUtils.AssertEquals(files.Count, 1, "只能上传一个文件");

                var dict = new Dictionary<RepositoryFile, HttpPostedFileBase>();
                for (int i = 0; i < files.Count; i++)
                {

                    var file = files[i];
                    var index = Math.Max(file.FileName.LastIndexOf('\\'), file.FileName.LastIndexOf('/'));
                    var fileName = index < 0 ? file.FileName : file.FileName.Substring(index + 1);

                    CommUtils.Assert(file.ContentLength > 0, "上传文件不能为空");
                    CommUtils.AssertHasContent(fileName, "文件名不能为空");
                    //CommUtils.Assert(fileName.Length <= 100, "文件名不能超过100个字符数");
                    var memoryStream = new MemoryStream(new BinaryReader(file.InputStream).ReadBytes(file.ContentLength));
                    var repoFile = Platform.Repository.AddFile(fileName, memoryStream);

                    dict[repoFile] = file;
                }
                var allFiles = m_dbAdapter.DMSFile.GetFilesByFileSeriesId(dmsFileSeries.Id);
                var currentVer = allFiles.Max(x => x.Version);

                for (int i = 0; i < dict.Keys.Count; i++)
                {
                    var repoFile = dict.Keys.ElementAt(i);


                    DMSFile newDMSFile = new DMSFile();
                    newDMSFile.DMSId = projectLogicModel.DMS.Instance.Id;
                    newDMSFile.DMSFileSeriesId = dmsFileSeries.Id;

                    newDMSFile.RepoFileId = repoFile.Id;
                    newDMSFile.Name = repoFile.Name;
                    newDMSFile.Description = description ?? string.Empty;

                    newDMSFile.Size = dict[repoFile].ContentLength;
                    newDMSFile.Version = currentVer + i + 1;
                    var now = DateTime.Now;
                    newDMSFile.LastModifyUserName = CurrentUserName;
                    newDMSFile.LastModifyTime = now;
                    newDMSFile.CreateUserName = CurrentUserName;
                    newDMSFile.CreateTime = now;

                    newDMSFile = m_dbAdapter.DMSFile.Create(newDMSFile);
                }
                return ActionUtils.Success(1);
            });
        }

        //下载文档
        [HttpPost]
        public ActionResult DownloadFile(string projectGuid, List<string> fileGuids)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var resourceGuids = new List<string>();
                List<Tuple<string, string>> fileTuples = new List<Tuple<string, string>>();

                foreach (var fileGuid in fileGuids)
                {
                    CommUtils.Assert(m_dbAdapter.DMSFile.isExistDMSFile(fileGuid), "找不到文件fileGuid[{0}]，请刷新页面后重试", fileGuid);
                    var dmsFile = m_dbAdapter.DMSFile.GetByGuid(fileGuid);
                    var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetById(dmsFile.DMSFileSeriesId);
                    var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);
                    var dms = m_dbAdapter.DMS.GetById(dmsFolder.DMSId);
                    CommUtils.AssertEquals(dms.ProjectId, projectLogicModel.Instance.ProjectId,
                        "FileGuid[{0}]不在产品[{1}]中", fileGuid, projectLogicModel.Instance.Name);

                    var repoFile = Platform.Repository.GetFile(dmsFile.RepoFileId);
                    var resultFilePath = repoFile.GetFilePath();

                    fileTuples.Add(Tuple.Create(dmsFile.Name, resultFilePath));
                }

                foreach (var fileTuple in fileTuples)
                {
                    var resource = ResourcePool.RegisterFilePath(CurrentUserName, fileTuple.Item1, fileTuple.Item2);
                    resourceGuids.Add(resource.Guid.ToString());
                }

                return ActionUtils.Success(resourceGuids);
            });
        }

        public ActionResult DownloadCompressFiles(string projectGuid, string fileGuidsText, string folderGuidsText, bool isSearchFile = false)
        {
            CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
            var fileGuids = string.IsNullOrWhiteSpace(fileGuidsText) ? new List<string>() : CommUtils.Split(fileGuidsText).ToList();
            var folderGuids = string.IsNullOrWhiteSpace(folderGuidsText) ? new List<string>() : CommUtils.Split(folderGuidsText).ToList();
            CommUtils.Assert(fileGuids.Count != 0 || folderGuids.Count != 0, "必须选择一个[文件/文件夹]进行下载");

            var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

            //获取待压缩文件夹下的所有文件/文件夹路径
            var dictAllFiles = new Dictionary<string, string>();
            var allEmptyFolderPath = new List<string>();
            if (folderGuids.Count != 0)
            {
                var folderAndFilePath = new Tuple<List<string>, Dictionary<string, string>>(new List<string>(), new Dictionary<string, string>());
                folderAndFilePath = GetCompressFilesPath(projectLogicModel, folderGuids, null, folderAndFilePath);
                dictAllFiles = folderAndFilePath.Item2;
                allEmptyFolderPath = folderAndFilePath.Item1;
            }

            //获取所有待压缩文件的路径
            var files = new List<DMSFile>();
            foreach (var fileGuid in fileGuids)
            {
                CommUtils.Assert(m_dbAdapter.DMSFile.isExistDMSFile(fileGuid), "找不到文件fileGuid[{0}]，请刷新页面后重试", fileGuid);
                var dmsFile = m_dbAdapter.DMSFile.GetByGuid(fileGuid);
                var dms = m_dbAdapter.DMS.GetById(dmsFile.DMSId);
                CommUtils.AssertEquals(dms.ProjectId, projectLogicModel.Instance.ProjectId,
                    "FileGuid[{0}]不在产品[{1}]中", fileGuid, projectLogicModel.Instance.Name);

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
                resultZipFileName = "项目文件";
            }
            else
            {
                if (fileGuids.Count == 0 && folderGuids.Count == 1)
                {
                    var currFolder = projectLogicModel.DMS.FindFolder(folderGuids.First()).Instance;
                    resultZipFileName = currFolder.Name;
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
                        resultZipFileName = currFolder.ParentFolderId.HasValue ? currFolder.Name : "项目文件";
                    }
                }

                if (string.IsNullOrEmpty(resultZipFileName))
                {
                    var currFolder = projectLogicModel.DMS.FindFolder(folderGuids.First()).Instance;
                    var parentFolder = m_dbAdapter.DMSFolder.GetById(currFolder.ParentFolderId.Value);

                    resultZipFileName = parentFolder.ParentFolderId.HasValue ? parentFolder.Name : "项目文件";
                }
            }

            return File(ms, "application/octet-stream", resultZipFileName + ".zip");
        }

        private Tuple<List<string>, Dictionary<string, string>> GetCompressFilesPath(ProjectLogicModel projectLogicModel,
            List<string> folderGuids, string currFolderName, Tuple<List<string>, Dictionary<string, string>> folderAndFilePath)
        {
            foreach (var folderGuid in folderGuids)
            {
                var parentFilePath = currFolderName;
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);
                CommUtils.AssertNotNull(folder, "在Project[{0}]中，找不到folderGuid[{1}]，请刷新后再试",
                    projectLogicModel.Instance.Name, folderGuid);

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
                    subFolders.ForEach(x => GetCompressFilesPath(projectLogicModel, subFolderGuids, parentFilePath, folderAndFilePath));
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

        //获取文档所有历史版本
        [HttpPost]
        public ActionResult GetHistoricalFiles(string projectGuid, string fileSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

                var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);
                var dms = m_dbAdapter.DMS.GetById(dmsFolder.DMSId);
                CommUtils.AssertEquals(dms.ProjectId, projectLogicModel.Instance.ProjectId,
                    "fileSeriesGuid[{0}]不在产品[{1}]中", fileSeriesGuid, projectLogicModel.Instance.Name);

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
                        createUser = Platform.UserProfile.Get(x.CreateUserName),
                        lastModifyTime = Toolkit.DateTimeToString(x.LastModifyTime),
                        lastModifyUser = Platform.UserProfile.Get(x.LastModifyUserName),
                    }
                );

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult RemoveFileSeries(string projectGuid, List<string> fileSeriesGuids)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

                var fileSeriesList = new List<DMSFileSeries>();
                foreach (var fileSeriesGuid in fileSeriesGuids)
                {
                    var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                    var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);
                    var dms = m_dbAdapter.DMS.GetById(dmsFolder.DMSId);
                    CommUtils.AssertEquals(dms.ProjectId, projectLogicModel.Instance.ProjectId,
                        "fileSeriesGuid[{0}]不在产品[{1}]中", fileSeriesGuid, projectLogicModel.Instance.Name);

                    CommUtils.Assert(IsCurrentUser(dmsFileSeries.CreateUserName),
                        "当前用户[{0}]不是[{1}]的创建者[{2}]", CurrentUserName, dmsFileSeries.Name, dmsFileSeries.CreateUserName);

                    fileSeriesList.Add(dmsFileSeries);
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
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);

                var dmsFiles = new List<DMSFile>();
                foreach (var fileGuid in fileGuids)
                {
                    var dmsFile = m_dbAdapter.DMSFile.GetByGuid(fileGuid);
                    var dmsFileSeries = m_dbAdapter.DMSFileSeries.GetById(dmsFile.DMSFileSeriesId);
                    var dmsFolder = m_dbAdapter.DMSFolder.GetById(dmsFileSeries.DMSFolderId);
                    var dms = m_dbAdapter.DMS.GetById(dmsFolder.DMSId);
                    CommUtils.AssertEquals(dms.ProjectId, projectLogicModel.Instance.ProjectId,
                        "FileGuid[{0}]不在产品[{1}]中", fileGuid, projectLogicModel.Instance.Name);

                    CommUtils.Assert(IsCurrentUser(dmsFile.CreateUserName),
                        "当前用户[{0}]不是[{1}]的创建者[{2}]", CurrentUserName, dmsFile.Name, dmsFile.CreateUserName);

                    dmsFiles.Add(dmsFile);
                }

                foreach (var dmsFile in dmsFiles)
                {
                    m_dbAdapter.DMSFile.Remove(dmsFile);
                }
                return ActionUtils.Success(dmsFiles.Count);
            });
        }

        [HttpPost]
        public ActionResult GetExportFileUserNames(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);
                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var project = projectLogicModel.Instance;
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "当前项目guid[{0}]不属于任何一个项目系列，请刷新页面后重试。", projectGuid);

                var adminUserNames = projectLogicModel.Team.Chiefs.Select(x => x.UserName).ToList();

                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(project.ProjectId);
                var teamMemberUserNames = teamMembers.Select(x => x.UserName).ToList();

                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(project.ProjectId);
                var teamAdminUserNames = teamAdmins.Select(x => x.UserName).ToList();

                var allUserNames = new List<string>();
                allUserNames.AddRange(adminUserNames);
                allUserNames.AddRange(teamAdminUserNames);
                allUserNames.AddRange(teamMemberUserNames);
                allUserNames = allUserNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var allPermission = m_dbAdapter.Permission.GetAllPermission(allUserNames, projectGuid);
                var resultUserNames = allPermission.Keys.ToList().Where(x => allPermission[x].Any(y => y.Type == PermissionType.Write)).ToList();

                Platform.UserProfile.Precache(resultUserNames);

                var result = resultUserNames.ConvertAll(x => new
                {
                    UserInfo = Platform.UserProfile.Get(x),
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult ExportUploadFileRecordToExcel(string projectGuid, string startTime, string endTime,
            List<string> usernames, string folderGuidsText, string fileSeriesGuidsText)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Write);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, projectGuid);
                var project = projectLogicModel.Instance;
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "当前项目guid[{0}]不属于任何一个项目系列，请刷新页面后重试。", projectGuid);

                var fileSeriesGuids = string.IsNullOrWhiteSpace(fileSeriesGuidsText) ? new List<string>() : CommUtils.Split(fileSeriesGuidsText).ToList();
                var folderGuids = string.IsNullOrWhiteSpace(folderGuidsText) ? new List<string>() : CommUtils.Split(folderGuidsText).ToList();
                CommUtils.Assert(fileSeriesGuids.Count != 0 || folderGuids.Count != 0, "必须选择一个[文件/文件夹]才能导出上传记录");

                CommUtils.Assert(DateUtils.IsNullableDate(startTime), "开始时间必须为[YYYY-MM-DD]格式或者为空");
                CommUtils.Assert(DateUtils.IsNullableDate(endTime), "结束时间必须为[YYYY-MM-DD]格式或者为空");

                var uploadStartTime = DateUtils.Parse(startTime);
                var uploadEndTime = DateUtils.Parse(endTime);
                if (uploadEndTime.HasValue)
                {
                    uploadEndTime = uploadEndTime.Value.Hour == 0
                        && uploadEndTime.Value.Minute == 0
                        && uploadEndTime.Value.Second == 0
                        ? uploadEndTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : uploadEndTime.Value;
                }

                var fileUploadRecordTable = new DataTable();
                fileUploadRecordTable.Columns.Add("用户名", typeof(System.String));
                fileUploadRecordTable.Columns.Add("真实姓名", typeof(System.String));
                fileUploadRecordTable.Columns.Add("文件名", typeof(System.String));
                fileUploadRecordTable.Columns.Add("版本号", typeof(System.String));
                fileUploadRecordTable.Columns.Add("文件路径", typeof(System.String));
                fileUploadRecordTable.Columns.Add("上传时间", typeof(System.String));
                fileUploadRecordTable.Columns.Add("大小", typeof(System.String));
                fileUploadRecordTable.Columns.Add("备注", typeof(System.String));

                var allFileSeries = new List<DMSFileSeries>();

                //获取选中文件夹下的所有文件夹及文件的【目录】
                var dictFileSeriesPath = new Dictionary<int, string>();
                dictFileSeriesPath = GetFilePathByfolderGuids(projectLogicModel, folderGuids, null, dictFileSeriesPath);

                //获取选中文件的【目录】
                var fileSeriesPath = GetFilePathByfileSeriesGuids(projectLogicModel, fileSeriesGuids);
                fileSeriesPath.Keys.ToList().ForEach(x => dictFileSeriesPath[x] = fileSeriesPath[x]);

                var folderFileSeries = GetFileSeriesByFolderGuids(projectLogicModel, folderGuids);
                var fileSeries = m_dbAdapter.DMSFileSeries.GetByGuids(fileSeriesGuids);

                allFileSeries.AddRange(folderFileSeries);
                allFileSeries.AddRange(fileSeries);

                var fileSeriesIds = allFileSeries.ConvertAll(x => x.Id);
                var dictFileSeriesIds = allFileSeries.Distinct().ToDictionary(x => x.Id);
                var files = m_dbAdapter.DMSFile.GetFilesByFileSeriesIds(fileSeriesIds);

                if (usernames != null && usernames.Count != 0)
                {
                    files = files.Where(x => usernames.Contains(x.CreateUserName)).ToList();
                }

                if (uploadStartTime.HasValue && uploadEndTime.HasValue)
                {
                    files = files.Where(x => uploadStartTime.Value <= x.CreateTime && x.CreateTime <= uploadEndTime.Value).ToList();
                }
                else if (uploadStartTime.HasValue && !uploadEndTime.HasValue)
                {
                    files = files.Where(x => uploadStartTime.Value <= x.CreateTime).ToList();
                }
                else if (!uploadStartTime.HasValue && uploadEndTime.HasValue)
                {
                    files = files.Where(x => x.CreateTime <= uploadEndTime.Value).ToList();
                }

                files = files.OrderBy(x => x.CreateUserName).ThenBy(x => x.CreateTime).ToList();

                var allUserName = files.ConvertAll(x => x.CreateUserName).Distinct();
                Platform.UserProfile.Precache(allUserName);

                dictFileSeriesPath.Keys.ToList().ForEach(x => dictFileSeriesPath[x] = dictFileSeriesPath[x].Substring(0, dictFileSeriesPath[x].LastIndexOf(FileUtils.PathSeparator)));

                foreach (var file in files)
                {
                    var filePath = dictFileSeriesPath[file.DMSFileSeriesId];
                    var row = fileUploadRecordTable.NewRow();
                    row["用户名"] = file.CreateUserName;
                    row["真实姓名"] = Platform.UserProfile.Get(file.CreateUserName).RealName;
                    row["文件名"] = dictFileSeriesIds[file.DMSFileSeriesId].Name;
                    row["版本号"] = "V" + file.Version;
                    row["文件路径"] = "所有文件" + filePath.Substring(4);
                    row["上传时间"] = Toolkit.DateTimeToString(file.CreateTime);
                    row["大小"] = FileUtils.FormatSize(file.Size);
                    row["备注"] = file.Description;

                    fileUploadRecordTable.Rows.Add(row);
                }

                var tempFolder = CommUtils.CreateTemporaryFolder(CurrentUserName);
                var tempFilePath = Path.Combine(tempFolder, "上传文件记录.xlsx");
                ExcelUtils.TableToExcel(fileUploadRecordTable, tempFilePath);

                var buffer = System.IO.File.ReadAllBytes(tempFilePath);
                CommUtils.DeleteFolderAync(tempFolder);

                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "上传文件记录.xlsx", new MemoryStream(buffer));
                return ActionUtils.Success(resource.Guid);
            });
        }

        private Dictionary<int, string> GetFilePathByfileSeriesGuids(ProjectLogicModel projectLogicModel, List<string> fileSeriesGuids)
        {
            var dictFileSeriesPath = new Dictionary<int, string>();
            foreach (var fileSeriesGuid in fileSeriesGuids)
            {
                var fileSeries = m_dbAdapter.DMSFileSeries.GetByGuid(fileSeriesGuid);
                var currFolder = m_dbAdapter.DMSFolder.GetById(fileSeries.DMSFolderId);
                var folder = projectLogicModel.DMS.FindFolder(currFolder.Guid);
                var parentFilePath = folder.GetParentFolderPath();
                if (string.IsNullOrWhiteSpace(parentFilePath))
                {
                    parentFilePath = folder.Instance.Name + FileUtils.PathSeparator;
                }
                dictFileSeriesPath[fileSeries.Id] = parentFilePath;
            }

            return dictFileSeriesPath;
        }

        private Dictionary<int, string> GetFilePathByfolderGuids(ProjectLogicModel projectLogicModel,
            List<string> folderGuids, string currFolderName, Dictionary<int, string> dictFileSeriesPath)
        {
            var parentFilePath = currFolderName;
            foreach (var folderGuid in folderGuids)
            {
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);

                if (string.IsNullOrWhiteSpace(currFolderName))
                {
                    parentFilePath = folder.GetParentFolderPath();
                }

                CommUtils.AssertNotNull(folder, "在Project[{0}]中，找不到folderGuid[{1}]，请刷新后再试",
                    projectLogicModel.Instance.Name, folderGuid);

                parentFilePath += folder.Instance.Name + FileUtils.PathSeparator;
                var subFolders = folder.SubFolders;

                var files = folder.Files;
                files.ForEach(x => dictFileSeriesPath[x.FileSeries.Id] = parentFilePath);

                if (subFolders.Count > 0)
                {
                    var subFolderGuids = subFolders.ConvertAll(x => x.Instance.Guid);
                    foreach (var subFolder in subFolders)
                    {
                        GetFilePathByfolderGuids(projectLogicModel, subFolderGuids, parentFilePath, dictFileSeriesPath);
                    }
                }
            }

            return dictFileSeriesPath;
        }

        private List<DMSFileSeries> GetFileSeriesByFolderGuids(ProjectLogicModel projectLogicModel, List<string> folderGuids)
        {
            var result = new List<DMSFileSeries>();

            foreach (var folderGuid in folderGuids)
            {
                var folder = projectLogicModel.DMS.FindFolder(folderGuid);
                var fileSeries = folder.Files.ConvertAll(x => x.FileSeries);

                result.AddRange(fileSeries);
                var subFolders = folder.SubFolders;

                if (subFolders.Count > 0)
                {
                    var subFolderGuids = subFolders.ConvertAll(x => x.Instance.Guid);
                    subFolders.ForEach(x => result.AddRange(GetFileSeriesByFolderGuids(projectLogicModel, subFolderGuids)));
                }
            }
            return result;
        }
    }
}

