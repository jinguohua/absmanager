using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class IssueController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetIssueActivityListInHomePage(string projectGuid, int issueActivityCount)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到ProjectSeries[projectGuid={0}]信息", projectGuid);

                var issues = m_dbAdapter.Issue.GetIssueListByProjectId(project.ProjectId);
                var dictIssue = issues.ToDictionary(x => x.Id);
                var issueIds = issues.ConvertAll(x => x.Id);

                var issueActivityList = m_dbAdapter.IssueActivity.GetIssueActivityListByIssueIds(issueIds);
                var dictIssueActivityList = issueActivityList.GroupBy(x => x.IssueId).ToDictionary(x => x.Key, y => y.ToList());

                var recordIssueIds = new List<int>();
                var record = new Dictionary<int,IssueActivity>();
                var issueActivityIds = new List<int>();

                foreach (var issueId in issueIds)
                {
                    if (dictIssueActivityList.ContainsKey(issueId))
                    {
                        var issueActivities = dictIssueActivityList[issueId];
                        issueActivities = issueActivities.OrderByDescending(x => x.LastModifyTime).ToList();
                        var issueActivity = issueActivities.First();

                        if (issueActivities.Count == 2 
                            && issueActivities.Count(x => x.IssueActivityTypeId == IssueActivityType.SystemGenerate) == 1
                            && issueActivities.Count(x => x.IssueActivityTypeId == IssueActivityType.Original) == 1)
                        {
                            issueActivity = issueActivities.Single(x => x.IssueActivityTypeId == IssueActivityType.Original);
                            issueActivity.Comment = "提出了此问题";
                        }

                        dictIssue[issueId].CreateTime = issueActivity.LastModifyTime;
                        record[issueId] = issueActivity;
                        recordIssueIds.Add(issueActivity.IssueId);
                        issueActivityIds.Add(issueActivity.IssueActivityId);
                    }
                }

                var fileIds = m_dbAdapter.File.GetFileIdsByIssueActivityIds(issueActivityIds);
                var dictFiles = fileIds.GroupBy(x => x.IssueActivityId).ToDictionary(x => x.Key, y => y.ToList());

                var imageIds = m_dbAdapter.Image.GetImageIdsByIssueActivityIds(issueActivityIds);
                var dictImages = imageIds.GroupBy(x => x.IssueActivityId).ToDictionary(x => x.Key, y => y.ToList());

                Platform.UserProfile.Precache(record.ToList().ConvertAll(x => x.Value.CreateUserName));

                var recordIssue = new List<Issue>();

                recordIssueIds.ForEach(x => recordIssue.Add(dictIssue[x]));
                recordIssue = recordIssue.OrderBy(x => x.IssueStatus).ThenByDescending(x => x.IssueEmergencyLevel).ThenByDescending(x => x.CreateTime).ToList();

                var resultIssue = recordIssue.Take(issueActivityCount);

                var result = resultIssue.Select(x =>
                {
                    var issueActivities = dictIssueActivityList[x.Id];
                    var activity = record[x.Id];
                    var userInfo = Platform.UserProfile.Get(activity.CreateUserName);
                    var systemGenerateCount = issueActivities.Count(y => y.IssueActivityTypeId == IssueActivityType.SystemGenerate);
                    return new {
                    UserInfo = new 
                    {
                        UserName = userInfo != null ? userInfo.UserName : "未知",
                        RealName = userInfo != null ? userInfo.RealName : "未知"
                    },
                    LastModifyTime = activity.LastModifyTime.ToString(),
                    Comment = activity.Comment,
                    IssueGuid = x.IssueGuid,
                    IssueName = x.IssueName,
                    EmergencyLevel = x.IssueEmergencyLevel.ToString(),
                    IsExistFile = dictFiles.ContainsKey(activity.IssueActivityId),
                    IsExistImage = dictImages.ContainsKey(activity.IssueActivityId),
                    IsCloseIssue = x.IssueStatus == IssueStatus.Finished,
                    IssueOperationType =
                        systemGenerateCount == 2 && activity.IssueActivityTypeId == IssueActivityType.SystemGenerate
                        ? IssueOperationType.CloseIssue.ToString() :
                        systemGenerateCount == 1 && issueActivities.Count == 2 
                        ? IssueOperationType.CreateIssue.ToString() : 
                        activity.RecordStatus == RecordStatus.Valid
                        ? IssueOperationType.AdditionalIssue.ToString() : IssueOperationType.DeleteIssueActivity.ToString(),

                        
                        IsValid = activity.RecordStatus == RecordStatus.Valid
                    };
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CreateIssue(string projectGuid, string issueName, string description,
            string emergencyLevelText, string allotUser, string shortCodeText)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project,projectGuid,PermissionType.Read);
                var files = Request.Files;

                ValidateUtils.Name(issueName, "问题名称");
                CommUtils.Assert(description.Length <= 10000, "描述不能超过10000个字符数");

                if (!string.IsNullOrWhiteSpace(allotUser))
                {
                    CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(allotUser), "用户[{0}]不存在，请刷新页面后重试", allotUser);
                }

                var emergencyLevel = CommUtils.ParseEnum<IssueEmergencyLevel>(emergencyLevelText);

                //上传文件和图片
                var dicFileAndImageIds = UploadFileAndImage(files);

                //创建问题
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var issue = new Issue();
                issue.ProjectId = project.ProjectId;
                issue.IssueName = issueName;
                issue.Description = description;
                issue.IssueEmergencyLevel = emergencyLevel;
                issue.RelatedTaskShortCode = "";
                issue.IssueStatus = IssueStatus.Preparing;
                issue.CreateTime = DateTime.Now;
                issue.CreateUserName = CurrentUserName;
                issue.RecordStatus = RecordStatus.Valid;
                issue.AllotUser = allotUser;
                if (!string.IsNullOrWhiteSpace(allotUser))
                {
                    issue.IssueAllotTime = DateTime.Now;
                }
                var newIssue = m_dbAdapter.Issue.NewIssue(issue);

                //保存受阻工作
                if (!string.IsNullOrWhiteSpace(shortCodeText))
                {
                    var shortCodes = CommUtils.Split(shortCodeText).ToList();
                    var tasks = m_dbAdapter.Task.GetTasks(shortCodes);
                    CommUtils.AssertEquals(tasks.Count, shortCodes.Count, "受阻工作异常，请刷新页面后重试");

                    shortCodes.ForEach(x =>
                    {
                        var issueConnectionTask = new IssueConnectionTasks();
                        issueConnectionTask.IssueId = newIssue.Id;
                        issueConnectionTask.TaskShortCode = x;
                        m_dbAdapter.IssueConnectionTasks.New(issueConnectionTask);
                    });
                }

                //创建事件，并且匹配其对应的文件与图片
                CreateIssueActivity(newIssue.Id, newIssue.Description, IssueActivityType.Original, dicFileAndImageIds);

                CreateIssueActivity(newIssue.Id, "提出了此问题", IssueActivityType.SystemGenerate, null);

                var result = new
                {
                    IssueGuid = newIssue.IssueGuid
                };
                return ActionUtils.Success(result);
            });
        }

        private Dictionary<string, List<string>> GetTaskGroupGuidAndTaskShortCode(string hinderTaskGroupAndTaskJson)
        {
            var dictGroupAndTaskGuid = new Dictionary<string, List<string>>();
            var javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue;
            var taskGroupAndTask = javaScriptSerializer.Deserialize<ArrayList>(hinderTaskGroupAndTaskJson);
            if (taskGroupAndTask.Count > 0)
            {
                foreach (Dictionary<string, object> item in taskGroupAndTask)
                {
                    if (item.Keys.Count == 2)
                    {
                        var taskGroupGuid = item["taskGroupGuid"] as string;
                        var shortCodes = item["taskGuid"] as string;
                        var value = CommUtils.Split(shortCodes).ToList();
                        if (dictGroupAndTaskGuid.Keys.Contains(taskGroupGuid))
                        {
                            dictGroupAndTaskGuid[taskGroupGuid].AddRange(value);
                        }
                        else
                        {
                            dictGroupAndTaskGuid[taskGroupGuid] = value;
                        }
                    }
                }
            }

            return dictGroupAndTaskGuid;
        }

        private void CreateIssueActivity(int issueId, string comment, IssueActivityType issueActivityType,
            Dictionary<string, List<int>> dicFileAndImageIds)
        {
            var issueActivity = new IssueActivity();
            issueActivity.IssueId = issueId;
            issueActivity.IssueActivityTypeId = issueActivityType;
            issueActivity.Comment = comment;
            issueActivity.CreateTime = DateTime.Now;
            issueActivity.CreateUserName = CurrentUserName;
            issueActivity.LastModifyTime = DateTime.Now;
            issueActivity.LastModifyUserName = CurrentUserName;
            issueActivity.RecordStatus = RecordStatus.Valid;

            var newIssueActivity = m_dbAdapter.IssueActivity.NewIssueActivity(issueActivity);

            if (dicFileAndImageIds != null)
            {
                dicFileAndImageIds["file"].ForEach(x => m_dbAdapter.File.NewIssueActivityFile(newIssueActivity.IssueActivityId, x));
                dicFileAndImageIds["image"].ForEach(x => m_dbAdapter.Image.NewIssueActivityImage(newIssueActivity.IssueActivityId, x));
            }
        }

        //检查所选择问题中是否包含领取人
        [HttpPost]
        public ActionResult IsExistAllotUser(string projectGuid, string issueGuidText)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                if (!string.IsNullOrWhiteSpace(issueGuidText))
                {
                    var issueGuids = CommUtils.Split(issueGuidText).ToList();
                    var issueList = m_dbAdapter.Issue.GetByGuids(issueGuids);
                    issueList = issueList.Where(x =>!string.IsNullOrWhiteSpace(x.AllotUser)).ToList();

                    var allotUsers = issueList.ConvertAll(x => x.AllotUser);
                    Platform.UserProfile.Precache(allotUsers);

                    var result = issueList.ConvertAll(x => new
                    {
                        issueName = x.IssueName,
                        issueGuid = x.IssueGuid,
                        userName = x.AllotUser,
                        realName = Platform.UserProfile.Get(x.AllotUser).RealName,
                        issueStatus = x.IssueStatus
                    });

                    return ActionUtils.Success(result);
                }
                return ActionUtils.Success("");
            });
        }

        //领取问题
        [HttpPost]
        public ActionResult SetAllotUser(string projectGuid, string issueGuidText)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                if (!string.IsNullOrWhiteSpace(issueGuidText))
                {
                    var issueGuids = CommUtils.Split(issueGuidText).ToList();
                    var issueList = m_dbAdapter.Issue.GetByGuids(issueGuids);
                    CommUtils.Assert(issueList.Select(x => x.ProjectId).Distinct().Count() == 1,
                "不能领取不属于同一产品的问题");
                    CommUtils.AssertEquals(issueGuids.Count, issueList.Count, "问题列表的信息错误，请刷新页面后重试");
                    issueList.ForEach(x =>
                    {
                        CommUtils.Assert(x.IssueStatus != IssueStatus.Finished, "问题[{0}]已经解决，无法继续领取", x.IssueName);
                        if (!string.IsNullOrWhiteSpace(x.AllotUser) && x.AllotUser != "-")
                        {
                            CommUtils.Assert(x.AllotUser != CurrentUserName, "您已经领取过问题[{0}]", x.IssueName);
                        }
                    });

                    issueList.ForEach(x =>
                    {
                        x.AllotUser = CurrentUserName;
                        x.IssueAllotTime = DateTime.Now;
                        x.IssueStatus = IssueStatus.Running;
                        x.LastModifyTime = DateTime.Now;
                        x.LastModifyUserName = CurrentUserName;
                        m_dbAdapter.Issue.UpdateIssue(x);
                        CreateIssueActivity(x.Id, "领取了此问题", IssueActivityType.SystemGenerate, null);
                    });

                    return ActionUtils.Success(issueList.Count);
                }
                return ActionUtils.Success(0);
            });
        }

        //获取问题列表（包含筛选条件）
        [HttpPost]
        public ActionResult GetIssueList(string projectGuid, string issueStatusText, string emergencyLevelText)
        {
            return ActionUtils.Json(() =>
            {
                if (string.IsNullOrWhiteSpace(issueStatusText) || string.IsNullOrWhiteSpace(emergencyLevelText))
                {
                    return ActionUtils.Success("");
                }

                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                var issueStatusStr = CommUtils.Split(issueStatusText).ToList();
                var emergencyLevelStr = CommUtils.Split(emergencyLevelText).ToList();

                var issueList = m_dbAdapter.Issue.GetIssueListByProjectId(project.ProjectId);
                issueList = issueList.OrderBy(x => x.IssueStatus).ToList();

                if (!issueStatusStr.Contains("All"))
                {
                    var issueStatus = issueStatusStr.ConvertAll(x => CommUtils.ParseEnum<IssueStatus>(x)).ToList();
                    issueList = issueList.Where(x => issueStatus.Contains(x.IssueStatus)).ToList();
                }

                if (!emergencyLevelStr.Contains("All"))
                {
                    var emergencyLevel = emergencyLevelStr.ConvertAll(x => CommUtils.ParseEnum<IssueEmergencyLevel>(x)).ToList();
                    issueList = issueList.Where(x => emergencyLevel.Contains(x.IssueEmergencyLevel)).ToList();
                }

                var allNames = new List<string>();
                allNames.AddRange(issueList.ConvertAll(x => x.CreateUserName));
                allNames.AddRange(issueList.ConvertAll(x => x.AllotUser));
                Platform.UserProfile.Precache(allNames.Distinct());

                var result = issueList.ConvertAll(x => new
                {
                    IssueGuid = x.IssueGuid,
                    IssueName = x.IssueName,
                    EmergencyLevel = x.IssueEmergencyLevel.ToString(),
                    IssueStatus = x.IssueStatus.ToString(),
                    CreateUser = new {
                        UserName = x.CreateUserName,
                        RealName = Platform.UserProfile.GetRealName(x.CreateUserName) ?? string.Empty,
                    },
                    CreateTime = Toolkit.DateTimeToString(x.CreateTime),
                    AllotUser = new {
                        UserName = x.AllotUser,
                        RealName = Platform.UserProfile.GetRealName(x.AllotUser) ?? string.Empty,
                    },
                    UpdateTime = Toolkit.DateTimeToString(x.IssueAllotTime),
                    LastModifyTime = Toolkit.DateTimeToString(x.LastModifyTime)
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetProjectProcessInfo(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                var issueList = m_dbAdapter.Issue.GetIssueListByProjectId(project.ProjectId);
                var finishedIssue = issueList.Count(x => x.IssueStatus == IssueStatus.Finished);
                var percentCompleted = CommUtils.Percent(finishedIssue, issueList.Count);

                var result = new
                {
                    issueCount = issueList.Count,
                    finishedIssueCount = finishedIssue,
                    percentCompleted = percentCompleted
                };
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CloseIssue(string projectGuid, string issueGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                var issue = m_dbAdapter.Issue.GetIssueByIssueGuid(issueGuid);
                issue.IssueStatus = IssueStatus.Finished;
                CommUtils.Assert(IsCurrentUser(issue.CreateUserName), "当前用户[{0}]不是问题[{1}]的提出者，无法关闭该问题", CurrentUserName, issue.IssueName);

                m_dbAdapter.Issue.UpdateIssue(issue);
                CreateIssueActivity(issue.Id, "关闭了此问题", IssueActivityType.SystemGenerate, null);
                return ActionUtils.Success("");
            });
        }

        //解决问题
        [HttpPost]
        public ActionResult ResolveIssue(string projectGuid, string issueGuidText)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                if (!string.IsNullOrWhiteSpace(issueGuidText))
                {
                    var issueGuids = CommUtils.Split(issueGuidText).ToList();
                    var issueList = m_dbAdapter.Issue.GetByGuids(issueGuids);

                    issueList.ForEach(x => CommUtils.Assert(x.IssueStatus != IssueStatus.Finished, "问题[{0}]已经解决。", x.IssueName));
                    issueList.ForEach(x =>
                    {
                        x.IssueStatus = IssueStatus.Finished;
                        m_dbAdapter.Issue.UpdateIssue(x);
                        CreateIssueActivity(x.Id, "解决了此问题", IssueActivityType.SystemGenerate, null);
                    });
                }

                return ActionUtils.Success("");
            });
        }

        //重置问题状态:完成→进行中
        [HttpPost]
        public ActionResult ResetIssueStatus(string projectGuid, string issueGuidText)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                CommUtils.Assert(!string.IsNullOrWhiteSpace(issueGuidText), "请选择一个问题");

                var issueGuids = CommUtils.Split(issueGuidText).ToList();
                var issueList = m_dbAdapter.Issue.GetByGuids(issueGuids);

                issueList.ForEach(x => CommUtils.Assert(x.IssueStatus == IssueStatus.Finished, "问题[{0}]未解决，无法进行此操作", x.IssueName));
                issueList.ForEach(x =>
                {
                    x.IssueStatus = IssueStatus.Running;
                    x.AllotUser = CurrentUserName;
                    x.IssueAllotTime = DateTime.Now;
                    m_dbAdapter.Issue.UpdateIssue(x);
                    CreateIssueActivity(x.Id, "解决了此问题", IssueActivityType.SystemGenerate, null);
                });

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DeleteIssue(string projectGuid, string issueGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var issue = m_dbAdapter.Issue.GetIssueByIssueGuid(issueGuid);
                CommUtils.Assert(IsCurrentUser(issue.CreateUserName), "当前用户[{0}]不是问题[{1}]的提出者，无法删除该问题", CurrentUserName, issue.IssueName);
                CommUtils.AssertEquals(issue.ProjectId, project.ProjectId, "追加的问题[{0}]和当前项目[{1}]不属于同一个项目，请刷新页面后重试", issue.IssueName, project.Name);

                m_dbAdapter.Issue.DeleteIssue(issue);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DeleteIssueActivity(string projectGuid, string issueGuid, string issueActivityGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                var issue = m_dbAdapter.Issue.GetIssueByIssueGuid(issueGuid);
                var issueActivity = m_dbAdapter.IssueActivity.GetIssueActivityByGuid(issueActivityGuid);

                CommUtils.Assert(issue.RecordStatus == RecordStatus.Valid, "问题[{0}]已经被删除，请刷新页面后重试",issue.IssueName);
                CommUtils.Assert(IsCurrentUser(issueActivity.CreateUserName),
                    "当前用户[{0}]不是该内容的创建者，无法删除该内容", CurrentUserName);
                CommUtils.Assert(issue.Id == issueActivity.IssueId,
                    "当前活动issueActivity.IssueId[{0}]与问题issue.Id[{1}]不一致，请刷新后重试",
                    issueActivity.IssueId, issue.Id);
                CommUtils.Assert(issueActivity.RecordStatus == RecordStatus.Valid, "该内容已经被删除");
                CommUtils.Assert(issueActivity.IssueActivityTypeId != IssueActivityType.SystemGenerate,"系统生成的内容无法被删除");

                issueActivity.LastModifyTime = DateTime.Now;
                issueActivity.LastModifyUserName = CurrentUserName;
                m_dbAdapter.IssueActivity.DeleteIssueActivity(issueActivity);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult GetIssueActivityList(string issueGuid)
        {
            return ActionUtils.Json(() =>
            {
                var issue = m_dbAdapter.Issue.GetIssueByIssueGuid(issueGuid);
                var project = m_dbAdapter.Project.GetProjectById(issue.ProjectId);
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var issueActivityList = m_dbAdapter.IssueActivity.GetIssueActivityListByIssueId(issue.Id);
                issueActivityList = issueActivityList.OrderByDescending(x => x.CreateTime).ToList();
                var allUserName = issueActivityList.ConvertAll(x => x.CreateUserName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                Platform.UserProfile.Precache(allUserName);

                var dictIssueActivityFileAndImage = new Dictionary<int, Tuple<List<int>, List<int>>>();
                var issueActivityIds = issueActivityList.ConvertAll(x => x.IssueActivityId);
                var files = m_dbAdapter.File.GetFileIdsByIssueActivityIds(issueActivityIds).GroupBy(x => 
                    x.IssueActivityId).ToDictionary(x => x.Key, y => y.Select(x => x.FileId).ToList());
                var images = m_dbAdapter.Image.GetImageIdsByIssueActivityIds(issueActivityIds).GroupBy(x =>
                    x.IssueActivityId).ToDictionary(x => x.Key, y => y.ToList().ConvertAll(x => x.ImageId));

                foreach (var issueActivity in issueActivityList)
                {
                    var issueActivityId = issueActivity.IssueActivityId;

                    var issueActivityFileId = files.ContainsKey(issueActivityId) ? files[issueActivityId] : new List<int>();
                    var issueActivityImageId = images.ContainsKey(issueActivityId) ? images[issueActivityId] : new List<int>();
                    dictIssueActivityFileAndImage[issueActivityId] = new Tuple<List<int>, List<int>>(issueActivityFileId, issueActivityImageId);
                }

                var connectionTasks = m_dbAdapter.IssueConnectionTasks.GetConnectionTasksByIssueId(issue.Id);
                var shortCodes = connectionTasks.ConvertAll(x => x.TaskShortCode).ToList();
                var tasks = m_dbAdapter.Task.GetTasks(shortCodes);

                var result = new { 
                    issueStatus = issue.IssueStatus.ToString(),
                    issueActivityList = issueActivityList.ConvertAll(x => 
                    {
                        var createUserInfo = Platform.UserProfile.Get(x.CreateUserName);
                        return new
                        {
                            Guid = x.IssueActivityGuid,
                            Description = x.Comment,
                            CreateUserName = createUserInfo.UserName,
                            CreateRealUserName = createUserInfo.RealName,
                            CreateTime = Toolkit.DateTimeToString(x.CreateTime),
                            LastModifyTime = Toolkit.DateTimeToString(x.LastModifyTime),
                            ActivityType = x.IssueActivityTypeId.ToString(),
                            FileInfo = m_dbAdapter.File.GetFilesByIds(dictIssueActivityFileAndImage[x.IssueActivityId].Item1).ConvertAll(file => new
                            {
                                Guid = file.Guid,
                                Name = file.Name
                            }),
                            ImageInfo = m_dbAdapter.Image.GetImagesByIds(dictIssueActivityFileAndImage[x.IssueActivityId].Item2).ConvertAll(image => new
                            {
                                Guid = image.Guid,
                                Name = image.Name
                            }),
                            IsValid = x.RecordStatus == RecordStatus.Valid,
                            ConnectionTasks = tasks.ConvertAll(task => new
                            {
                                taskName = task.Description,
                                shortCode = task.ShortCode
                            })
                        };
                    })
                };
                return ActionUtils.Success(result);
            });
        }

        //获取问题描述
        [HttpPost]
        public ActionResult GetIssueDescription(string projectGuid, string issueGuid, string issueActivityGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var issue = m_dbAdapter.Issue.GetIssueByIssueGuid(issueGuid);
                var issueActivity = m_dbAdapter.IssueActivity.GetIssueActivityByGuid(issueActivityGuid);
                CommUtils.Assert(issue.IssueStatus != IssueStatus.Finished, "问题[{0}]已经被解决，请刷新页面后重试", issue.IssueName);
                CommUtils.AssertEquals(issue.Id, issueActivity.IssueId,
                    "当前描述issueActivityGuid[{0}]不属于当前所选择的问题issueGuid[{1}]", issueActivityGuid, issueGuid);
                CommUtils.Assert(issueActivity.IssueActivityTypeId == IssueActivityType.Original, "问题描述的类型不是");

                var issueActivityId = issueActivity.IssueActivityId;
                var fileIds = m_dbAdapter.File.GetFileIdsByIssueActivityId(issueActivityId).ConvertAll(x => x.FileId).ToList();
                var imageIds = m_dbAdapter.Image.GetImageIdsByIssueActivityId(issueActivityId).ConvertAll(x => x.ImageId).ToList();

                var files = m_dbAdapter.File.GetFilesByIds(fileIds);
                var images = m_dbAdapter.Image.GetImagesByIds(imageIds);

                var createUserInfo = Platform.UserProfile.Get(issueActivity.CreateUserName);
                var connectionTasks = m_dbAdapter.IssueConnectionTasks.GetConnectionTasksByIssueId(issue.Id);
                var shortCodes = connectionTasks.ConvertAll(x => x.TaskShortCode).ToList();
                var tasks = m_dbAdapter.Task.GetTasks(shortCodes);

                var result = new {
                    Guid = issueActivityGuid,
                    Description = issueActivity.Comment,
                    CreateUserName = createUserInfo.UserName,
                    CreateRealUserName = createUserInfo.RealName,
                    CreateTime = Toolkit.DateTimeToString(issueActivity.CreateTime),
                    LastModifyTime = Toolkit.DateTimeToString(issueActivity.LastModifyTime),
                    ActivityType = issueActivity.IssueActivityTypeId.ToString(),
                    FileInfo = files.ConvertAll(file => new
                    {
                        Guid = file.Guid,
                        Name = file.Name
                    }),
                    ImageInfo = images.ConvertAll(image => new
                    {
                        Guid = image.Guid,
                        Name = image.Name
                    }),
                    IsValid = issueActivity.RecordStatus == RecordStatus.Valid,
                    ConnectionTasks = tasks.ConvertAll(x => new {
                        taskName = x.Description,
                        shortCode = x.ShortCode
                    })
                };

                return ActionUtils.Success(result);
            });
        }

        //修改问题描述
        [HttpPost]
        public ActionResult ModifyIssueDescription(string projectGuid, string issueGuid, string issueActivityGuid,
            string issueName, string description, string emergencyLevelText, string allotUser,
            string fileGuidsText, string imageGuidsText, string shortCodeText)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);

                var issue = m_dbAdapter.Issue.GetIssueByIssueGuid(issueGuid);
                var issueActivity = m_dbAdapter.IssueActivity.GetIssueActivityByGuid(issueActivityGuid);
                CommUtils.Assert(issue.IssueStatus != IssueStatus.Finished, "问题[{0}]已经被解决，不能编辑，请刷新页面后重试", issue.IssueName);
                CommUtils.AssertEquals(issue.Id, issueActivity.IssueId,
                    "当前描述issueActivityGuid[{0}]不属于当前所选择的问题issueGuid[{1}]，请刷新页面后重试", issueActivityGuid, issueGuid);
                CommUtils.Assert(issueActivity.IssueActivityTypeId == IssueActivityType.Original, "问题描述的类型不是");
                CommUtils.AssertHasContent(issueName, "问题名称不能为空");
                CommUtils.Assert(issueName.Length <= 30, "问题名称不能超过30个字符数");
                CommUtils.Assert(description.Length <= 10000, "问题描述不能超过10000个字符数");
                if (!string.IsNullOrWhiteSpace(allotUser))
                {
                    CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(allotUser), "用户[{0}]不存在，请刷新页面后重试", allotUser);
                }

                var emergencyLevel = CommUtils.ParseEnum<IssueEmergencyLevel>(emergencyLevelText);

                //上传文件和图片
                var files = Request.Files;
                var dicFileAndImageIds = UploadFileAndImage(files);

                issue.IssueName = issueName;
                issue.Description = description;
                issue.IssueEmergencyLevel = emergencyLevel;
                issue.LastModifyTime = DateTime.Now;
                issue.LastModifyUserName = CurrentUserName;
                issue.AllotUser = allotUser;

                m_dbAdapter.Issue.UpdateIssue(issue);

                issueActivity.Comment = description;
                m_dbAdapter.IssueActivity.UpdateIssueActivity(issueActivity);

                //删除IssueActivity原有的图片及文件
                DeleteActivityFilesAndImages(issueActivity.IssueActivityId, fileGuidsText, imageGuidsText);

                //添加新的文件及图片
                if (dicFileAndImageIds != null)
                {
                    dicFileAndImageIds["file"].ForEach(x => m_dbAdapter.File.NewIssueActivityFile(issueActivity.IssueActivityId, x));
                    dicFileAndImageIds["image"].ForEach(x => m_dbAdapter.Image.NewIssueActivityImage(issueActivity.IssueActivityId, x));
                }

                return ActionUtils.Success("");
            });
        }

        private void DeleteActivityFilesAndImages(int issueActivityId, string fileGuidsText, string imageGuidsText)
        {
            fileGuidsText = fileGuidsText ?? string.Empty;
            imageGuidsText = imageGuidsText ?? string.Empty;
            var fileGuids = CommUtils.Split(fileGuidsText).ToList();
            var imageGuids = CommUtils.Split(imageGuidsText).ToList();

            var activityFiles = m_dbAdapter.File.GetFileIdsByIssueActivityId(issueActivityId).ToList();
            var activityImages = m_dbAdapter.Image.GetImageIdsByIssueActivityId(issueActivityId).ToList();

            var oldFiles = m_dbAdapter.File.GetFilesByIds(activityFiles.ConvertAll(x => x.FileId));
            var oldImages = m_dbAdapter.Image.GetImagesByIds(activityImages.ConvertAll(x => x.ImageId));

            var deleteFileIds = oldFiles.Where(x => !fileGuids.Contains(x.Guid)).ToList().ConvertAll(x => x.Id).ToList();
            var deleteImageIds = oldImages.Where(x => !imageGuids.Contains(x.Guid)).ToList().ConvertAll(x => x.Id).ToList();

            var deleteActivityFiles = activityFiles.Where(x => deleteFileIds.Contains(x.FileId)).ToList();
            var deleteActivityImages = activityImages.Where(x => deleteImageIds.Contains(x.ImageId)).ToList();
            m_dbAdapter.File.DeleteIssueActivityFiles(deleteActivityFiles);
            m_dbAdapter.Image.DeleteIssueActivityImages(deleteActivityImages);
        }

        [HttpPost]
        public ActionResult CreateIssueActivity(string projectGuid, string issueGuid, string comment)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                CommUtils.AssertHasContent(comment, "追加评论不能为空");
                CommUtils.Assert(comment.Length <= 10000, "追加评论不能超过10000个字符数");
                var issue = m_dbAdapter.Issue.GetIssueByIssueGuid(issueGuid);
                CommUtils.Assert(issue.IssueStatus != IssueStatus.Finished,"问题[{0}]已经解决，不能够进行追加", issue.IssueName);
                var files = Request.Files;

                //上传文件和图片
                var dicFileAndImageIds = UploadFileAndImage(files);

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.AssertEquals(issue.ProjectId, project.ProjectId, "追加的问题[{0}]和当前项目[{1}]不属于同一个项目，请刷新页面后重试",issue.IssueName,project.Name);

                CreateIssueActivity(issue.Id, comment, IssueActivityType.Additional, dicFileAndImageIds);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DownloadFile(string fileGuid)
        {
            return ActionUtils.Json(() =>
            {
                var file = Platform.Repository.GetFile(fileGuid);
                var resultFilePath = file.GetFilePath();

                var issueActivityId = m_dbAdapter.File.GetIssueActivityFileByFileId(file.Id).IssueActivityId;
                CommUtils.Assert(m_dbAdapter.IssueActivity.IsValidIssueActivity(issueActivityId), "找不到文件[{0}]，请刷新页面后重试", file.Name);

                var issue = m_dbAdapter.Issue.GetIssueByFileId(file.Id);
                CommUtils.Assert(issue.RecordStatus == RecordStatus.Valid, "问题[{0}]已经被删除，请刷新页面后重试", issue.IssueName);

                var project = m_dbAdapter.Project.GetProjectById(issue.ProjectId);
                CommUtils.Assert(project.ProjectId == issue.ProjectId,
                    "您要下载的文件fileGuid[{0}]，与当前产品projectGuid[{1}]不一致，请刷新页面后重试", fileGuid, project.ProjectGuid);
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var resource = ResourcePool.RegisterFilePath(CurrentUserName, file.Name, resultFilePath);
                return ActionUtils.Success(resource.Guid.ToString());
            });
        }

        [HttpPost]
        public ActionResult DownloadImage(string imageGuid, string imageSharpnessTypeText)
        {
            return ActionUtils.Json(() =>
            {
                var image = Platform.Repository.GetImage(imageGuid);
                var imageSharpnessType = CommUtils.ParseEnum<ImageSharpnessType>(imageSharpnessTypeText);

                var issueActivityId = m_dbAdapter.Image.GetIssueActivityImageByImageId(image.Id).IssueActivityId;
                CommUtils.Assert(m_dbAdapter.IssueActivity.IsValidIssueActivity(issueActivityId), "找不到图片[{0}]，请刷新页面后重试", image.Name);

                var issue = m_dbAdapter.Issue.GetIssueByImageId(image.Id);
                CommUtils.Assert(issue.RecordStatus == RecordStatus.Valid, "问题[{0}]已经被删除，请刷新页面后重试", issue.IssueName);

                var project = m_dbAdapter.Project.GetProjectById(issue.ProjectId);
                CommUtils.Assert(project.ProjectId == issue.ProjectId,
                    "您要下载的图片imageGuid[{0}]，与当前产品projectGuid[{1}]不一致，请刷新页面后重试", imageGuid, project.ProjectGuid);
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var resultImagePath = image.GetImagePath();

                var imageName = Path.GetFileName(resultImagePath);
                var imageExtensionName = imageName.Substring(imageName.LastIndexOf('.') + 1);

                ImageFileType fileType;
                if (!Enum.TryParse<ImageFileType>(imageExtensionName.ToUpperInvariant(), out fileType))
                {
                    throw new ApplicationException("下载图片[" + imageName + "]失败，原图发生错误，重新上传后才可以下载");
                }

                var resource = ResourcePool.RegisterFilePath(CurrentUserName, imageName, resultImagePath);
                return ActionUtils.Success(resource.Guid.ToString());
            });
        }

        private Dictionary<string, List<int>> UploadFileAndImage(HttpFileCollectionBase files)
        {
            if (files.Count == 0)
            {
                return null;
            }
            var imageGuids = new List<string>();
            var fileGuids = new List<string>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                var index = Math.Max(file.FileName.LastIndexOf('\\'), file.FileName.LastIndexOf('/'));
                var fileName = index < 0 ? file.FileName : file.FileName.Substring(index + 1);
                var fileExtensionName = fileName.Substring(fileName.LastIndexOf('.') + 1);

                ImageFileType fileType;
                if (Enum.TryParse<ImageFileType>(fileExtensionName.ToUpperInvariant(), out fileType))
                {
                    var memoryStream = new MemoryStream(new BinaryReader(file.InputStream).ReadBytes(file.ContentLength));
                    var newImage = Platform.Repository.AddImage(fileName, memoryStream);
                    imageGuids.Add(newImage.Guid);
                }
                else
                {
                    var memoryStream = new MemoryStream(new BinaryReader(file.InputStream).ReadBytes(file.ContentLength));
                    var newFile = Platform.Repository.AddFile(fileName, memoryStream);
                    fileGuids.Add(newFile.Guid);
                }
            }

            var newFiles = m_dbAdapter.File.GetFilesByGuids(fileGuids);
            var newImages = m_dbAdapter.Image.GetImagesByGuids(imageGuids);

            var dicFileAndImageIds = new Dictionary<string, List<int>>();
            dicFileAndImageIds["file"] = newFiles.ConvertAll(x => x.Id);
            dicFileAndImageIds["image"] = newImages.ConvertAll(x => x.Id);
            return dicFileAndImageIds;
        }

        public enum ImageSharpnessType
        {
            //预览图
            PreviewImage = 1,
            //缩略图
            Thumbnail = 2,
            //原图
            MasterMap = 3,
        }

        public ActionResult ShowImage(string projectGuid, string imageGuid, string imageSharpnessTypeText)
        {
            CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
            var imageSharpnessType = CommUtils.ParseEnum<ImageSharpnessType>(imageSharpnessTypeText);
            var resultImagePath = string.Empty;

            var image = Platform.Repository.GetImage(imageGuid);
            var issueActivityId = m_dbAdapter.Image.GetIssueActivityImageByImageId(image.Id).IssueActivityId;
            CommUtils.Assert(m_dbAdapter.IssueActivity.IsValidIssueActivity(issueActivityId), "找不到图片[{0}]，请刷新页面后重试", image.Name);

            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var issue = m_dbAdapter.Issue.GetIssueByImageId(image.Id);
            CommUtils.Assert(project.ProjectId == issue.ProjectId,
                "您要查看的图片imageGuid[{0}]，与当前产品projectGuid[{1}]不一致，请刷新页面后重试", imageGuid, projectGuid);

            switch (imageSharpnessType)
            {
                case ImageSharpnessType.Thumbnail:
                    resultImagePath = image.GetThumbnailPath();
                    break;
                case ImageSharpnessType.PreviewImage:
                    resultImagePath = image.GetPreviewImagePath();
                    break;
            }

            var imageName = Path.GetFileName(resultImagePath);
            var imageExtensionName = imageName.Substring(imageName.LastIndexOf('.') + 1);

            ImageFileType fileType;
            if (!Enum.TryParse<ImageFileType>(imageExtensionName.ToUpperInvariant(), out fileType))
            {
                throw new ApplicationException("加载图片[" + imageName + "]失败，该文件不是图片类型");
            }

            return base.File(resultImagePath, "image/" + imageExtensionName.ToLowerInvariant());
        }

        public ActionResult DownloadCompressFiles(string projectGuid, string fileGuids, string imageGuids)
        {
            CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
            CommUtils.Assert(!string.IsNullOrWhiteSpace(fileGuids) || !string.IsNullOrWhiteSpace(imageGuids), "必须选择一个文件进行下载");

            fileGuids = string.IsNullOrWhiteSpace(fileGuids) ? "" : fileGuids;
            var fileGuidList = CommUtils.Split(fileGuids).ToList();
            var files = m_dbAdapter.File.GetFilesByGuids(fileGuidList);
            var filePaths = files.ConvertAll(x => x.Path);

            imageGuids = string.IsNullOrWhiteSpace(imageGuids) ? "" : imageGuids;
            var imageGuidList = CommUtils.Split(imageGuids).ToList();
            var images = m_dbAdapter.Image.GetImagesByGuids(imageGuidList);
            var imagePaths = images.ConvertAll(x => x.Path);

            var ms = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(ms);
            zipStream.SetLevel(3);

            CompressFiles(zipStream, WebConfigUtils.RepositoryFilePath, filePaths, ms);
            CompressFiles(zipStream, WebConfigUtils.RepositoryImagePath, imagePaths, ms);

            zipStream.Flush();
            zipStream.Finish();

            ms.Seek(0, SeekOrigin.Begin);

            var dateTime = DateTime.Now.ToString("yyyyMMddhhmmssms");
            return File(ms, "application/octet-stream", dateTime + "打包文件.zip");
        }

        private void CompressFiles(ZipOutputStream zipStream, string folder, List<string> fileNames, MemoryStream ms)
        {
            var nameList = new List<string>();
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

            if (fileNameList.Contains(Path.GetFileName(path)))
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
                } while (fileNameList.Contains(Path.GetFileName(newFullPath)));
            }
            return Path.GetFileName(newFullPath);
        }

        [HttpPost]
        public ActionResult GetIssueInfo(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var connectionTasks = m_dbAdapter.IssueConnectionTasks.GetConnectionTasksByShortCode(shortCode);
                var issueIds = connectionTasks.ConvertAll(x => x.IssueId).ToList();
                var issueList = m_dbAdapter.Issue.GetIssueByIssueIds(issueIds);

                var usernames = new List<string>();
                issueList.ForEach(x => {
                    usernames.Add(x.AllotUser);
                    usernames.Add(x.CreateUserName);
                });
                Platform.UserProfile.Precache(usernames.Distinct());

                var result = issueList.ConvertAll(x => new
                {
                    issueName = x.IssueName,
                    issueGuid = x.IssueGuid,
                    issueDescription = x.Description,
                    issueStatus = x.IssueStatus.ToString(),
                    issueEmergencyLevel = x.IssueEmergencyLevel.ToString(),
                    CreateUser = new
                    {
                        UserName = x.CreateUserName,
                        RealName = Platform.UserProfile.GetRealName(x.CreateUserName) ?? string.Empty,
                    },
                    CreateTime = Toolkit.DateTimeToString(x.CreateTime),
                    AllotUser = new
                    {
                        UserName = x.AllotUser,
                        RealName = Platform.UserProfile.GetRealName(x.AllotUser) ?? string.Empty,
                    },
                    UpdateTime = Toolkit.DateTimeToString(x.IssueAllotTime),
                });
                return ActionUtils.Success(result);
            });
        }
    }
}
