using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Controllers.TaskExtension;
using ChineseAbs.ABSManagementSite.Controllers.TaskExtension.Demo;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System;
using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.LogicModels;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class TaskExController : BaseController
    {
        #region results

        public ActionResult UploadTaskExDocument(string shortCode, string taskExDocumentGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(Request.Files.Count > 0, "请选择上传文件");
                var file = Request.Files[0];
                var handler = new TaskExDocument(CurrentUserName, shortCode);
                var fileCount = handler.UploadTaskExDocument(file, shortCode, taskExDocumentGuid);
                return ActionUtils.Success(fileCount.ToString());
            });
        }

        public ActionResult DownloadTaskExDocument(string shortCode, string taskExDocumentGuid)
        {
            var handler = new TaskExDocument(CurrentUserName, shortCode);
            var fileInfo = handler.DownloadTaskExDocument(shortCode, taskExDocumentGuid);

            var task = m_dbAdapter.Task.GetTask(shortCode);
            var logicModel = Platform.GetProject(task.ProjectId);
            var downFileAuthority = logicModel.Authority.DownloadFile.CurrentUserAuthority;
            return CnabsFile(fileInfo.AbsultePath, fileInfo.MIME, fileInfo.DisplayName, downFileAuthority);
        }

        [HttpPost]
        public ActionResult GenerateDocument(string shortCode, string taskExDocumentGuid, bool autoUpload)
        {
            return ActionUtils.Json(() =>
            {
                var handler = new TaskExDocument(CurrentUserName, shortCode);
                var result = handler.GenerateDocument(shortCode, taskExDocumentGuid, autoUpload);
                var fileInfo = result.Item2;
                var ms = result.Item1;

                var task = m_dbAdapter.Task.GetTask(shortCode);
                var logicModel = Platform.GetProject(task.ProjectId);
                var downFileAuthority = logicModel.Authority.DownloadFile.CurrentUserAuthority;

                var cnabsFile = CnabsFile(fileInfo.DisplayName, ms, downFileAuthority);
                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, cnabsFile.Item1, cnabsFile.Item2);
                return ActionUtils.Success(resource.Guid.ToString());
            });
        }
        #endregion

        #region Demo建元生成报告
        public ActionResult UploadDemoJianYuanReport(HttpPostedFileBase file, string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var fileName = file.FileName;
                if (!fileName.EndsWith("xls", StringComparison.CurrentCultureIgnoreCase)
                    && !fileName.EndsWith("xlsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    CommUtils.Assert(false, "根据[{0}]生成文档失败，请选择Excel文件上传", fileName);
                }

                var handler = new TaskExDemoJianYuanReport(CurrentUserName, shortCode);
                //上传服务商报告
                var result = handler.UploadDemoJianYuanReport(file, shortCode);
                
                //生成报告
                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, result.Item2.DisplayName, result.Item1);

                m_dbAdapter.Task.AddTaskLog(shortCode, "上传[" + file.FileName + "]，转换为[" + result.Item2.DisplayName  + "]");
                return ActionUtils.Success(resource.Guid.ToString());
            });
        }
        #endregion

        #region Modify task extension CheckList

        [HttpPost]
        public ActionResult GetTaskExCheckList(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Task, shortCode, PermissionType.Read);
                var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);

                CommUtils.Assert(task.TaskExtensionId.HasValue, "工作[{0}][{1}]不包含扩展信息",
                    task.Description,shortCode);
                CommUtils.Assert(task.TaskExtension.TaskExtensionType == TaskExtensionType.CheckList.ToString(), 
                    "工作[{0}][{1}]的工作扩展类型不是[工作要点检查]", task.Description, shortCode);

                var taskExtensionInfo = task.TaskExtension.TaskExtensionInfo;

                var result = new TaskExCheckListInfo();
                if (!string.IsNullOrWhiteSpace(taskExtensionInfo))
                {
                    result = CommUtils.FromJson<TaskExCheckListInfo>(taskExtensionInfo);
                }

                return ActionUtils.Success(result);
            });
        }
        
        [HttpPost]
        public ActionResult AddTaskExtensionCheckList(string shortCode, string groupName, string checkItemName)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Task, shortCode, PermissionType.Write);
                var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);
                m_dbAdapter.Task.CheckPrevIsFinished(task);

                CommUtils.Assert(CommUtils.ParseEnum<TaskExtensionType>(task.TaskExtension.TaskExtensionType) == TaskExtensionType.CheckList,
                    "工作[" + task.Description + "]的工作扩展类型不是[工作要点检查]");
                CommUtils.Assert(!string.IsNullOrWhiteSpace(groupName), "分组名称不能为空");
                CommUtils.Assert(!string.IsNullOrWhiteSpace(checkItemName), "工作要点不能为空");
                var checkItemNameList = CommUtils.Split(checkItemName, new string[] { "\n" });

                CheckedByCheckItemNamesRepeat(checkItemNameList);
                
                TaskExCheckListInfo taskExCheckLists = new TaskExCheckListInfo();
                if (!string.IsNullOrWhiteSpace(task.TaskExtension.TaskExtensionInfo))
                {
                    taskExCheckLists = CommUtils.FromJson<TaskExCheckListInfo>(task.TaskExtension.TaskExtensionInfo);
                }
                if (taskExCheckLists.CheckGroups.Count != 0 &&
                    taskExCheckLists.CheckGroups.Any(x => x.GroupName == groupName))
                {
                    foreach (var checkItemGroupList in taskExCheckLists.CheckGroups)
                    {
                        if (checkItemGroupList.GroupName == groupName)
                        {
                            foreach (var item in checkItemNameList)
                            {
                                CommUtils.Assert(!checkItemGroupList.CheckItems.Any(x => x.Name == item), "工作[{0}({1})]中已经存在检查项[{2}]",
                                    task.Description, task.ShortCode, checkItemName);

                                var checkItem = new TaskExCheckItem();
                                checkItem.Guid = Guid.NewGuid().ToString();
                                checkItem.Name = item;
                                checkItem.CheckStatus = TaskExCheckType.Unchecked.ToString();

                                checkItemGroupList.CheckItems.Add(checkItem);
                            }
                        }
                    }
                }
                else
                {
                    TaskExCheckGroup checkItemGroup = new TaskExCheckGroup();
                    checkItemGroup.GroupName = groupName;

                    foreach (var item in checkItemNameList)
                    {
                        var checkItem = new TaskExCheckItem();
                        checkItem.Guid = Guid.NewGuid().ToString();
                        checkItem.Name = item;
                        checkItem.CheckStatus = TaskExCheckType.Unchecked.ToString();
                        checkItemGroup.CheckItems.Add(checkItem);
                    }

                    taskExCheckLists.CheckGroups.Add(checkItemGroup);

                }
                var isExistNotFinished = taskExCheckLists.CheckGroups.Any(group => group.CheckItems.Any(y => y.CheckStatus == TaskExCheckType.Unchecked.ToString()));
                if (isExistNotFinished && task.TaskStatus != TaskStatus.Running)
                {
                    var logicModel = Platform.GetProject(task.ProjectId);
                    new TaskLogicModel(logicModel, task).Start();
                }
                else
                {
                    m_dbAdapter.Task.UpdateTask(task);
                }

                task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(taskExCheckLists);
                task.TaskHandler = CurrentUserName;

                m_dbAdapter.Task.UpdateTaskExtension(task.TaskExtension);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditTask, task.ProjectId,
                    "工作[" + task.Description + "][" + shortCode + "],中添加检查项GroupName[" + groupName + "]CheckItem[" + CommUtils.Join(checkItemNameList) + "]", "");

                var comment = "增加分组[" + groupName + "]中的工作要点[" + string.Join(";",checkItemNameList.ToArray()) + "]";
                m_dbAdapter.Task.AddTaskLog(task, comment);

                return ActionUtils.Success(1);
            });
        }

        private void CheckedByCheckItemNamesRepeat(string[] checkItemNameList)
        {
            if (checkItemNameList.Count() != checkItemNameList.Distinct().Count())
            {
                var nameList = new List<string>();
                var repeatNames = new List<string>();
                for (int i = 0; i < checkItemNameList.Count(); i++)
                {
                    if (nameList.Contains(checkItemNameList[i]))
                    {
                        repeatNames.Add(checkItemNameList[i]);
                    }
                    else
                    {
                        nameList.Add(checkItemNameList[i]);
                    }
                }
                throw new ApplicationException("检查项中包含重复的项[" + string.Join(",", repeatNames.Distinct()) + "]");
            }
        }

        [HttpPost]
        public ActionResult ModifyTaskExtensionCheckList(string shortCode, string groupName, string checkItemName, string checkItemGuid, string checkItemType)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Task, shortCode, PermissionType.Execute);
                var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);
                m_dbAdapter.Task.CheckPrevIsFinished(task);

                CommUtils.Assert(CommUtils.ParseEnum<TaskExtensionType>(task.TaskExtension.TaskExtensionType) == TaskExtensionType.CheckList,
                    "工作[" + task.Description + "]的工作扩展类型不是[工作要点检查]");

                var oldItemStatus = CommUtils.ParseEnum<TaskExCheckType>(checkItemType);

                var revisionCheckType = string.Empty;
                
                CommUtils.AssertHasContent(task.TaskExtension.TaskExtensionInfo, "当前工作不包含扩展信息。");
                var taskExInfo = task.TaskExtension.TaskExtensionInfo;
                var taskExCheckListInfo = CommUtils.FromJson<TaskExCheckListInfo>(taskExInfo);

                CommUtils.AssertEquals(taskExCheckListInfo.CheckGroups.Count(x => x.GroupName == groupName), 1, "找不到检查项分组{0}", groupName);
                var group = taskExCheckListInfo.CheckGroups.Single(x => x.GroupName == groupName);

                CommUtils.AssertEquals(group.CheckItems.Count(x => x.Name == checkItemName), 1, "检查项分组{0}中，找不到检查项{1}", checkItemName);
                var checkItem = group.CheckItems.Single(x => x.Name == checkItemName);

                CommUtils.AssertEquals(checkItem.Guid, checkItemGuid, "检查项分组{0}中，检查项{1}匹配失败", groupName, checkItemName);
                CommUtils.AssertEquals(checkItem.CheckStatus, checkItemType, "检查项分组{0}中，检查项{1}状态有误，请刷新后再试", groupName, checkItemName);

                if (oldItemStatus == TaskExCheckType.Unchecked)
                {
                    checkItem.CheckStatus = TaskExCheckType.Checked.ToString();
                }
                else if (oldItemStatus == TaskExCheckType.Checked)
                {
                    checkItem.CheckStatus = TaskExCheckType.Unchecked.ToString();
                }
                else
                {
                    CommUtils.Assert(false, "无法识别的检查项内容[{0}]", oldItemStatus.ToString());
                }

                revisionCheckType = checkItem.CheckStatus;
                task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(taskExCheckListInfo);
                task.TaskHandler = CurrentUserName;

                m_dbAdapter.Task.UpdateTaskExtension(task.TaskExtension);

                var logicModel = Platform.GetProject(task.ProjectId);
                new TaskLogicModel(logicModel, task).Start();
                m_dbAdapter.Task.UpdateTask(task);
                m_dbAdapter.Task.AddTaskLog(task, "校验分组[" + groupName + "]中的工作要点[" + checkItemName + "]，状态为：" + Toolkit.ConvertTaskExCheckType(revisionCheckType));

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult DeleteExtensionCheckItemGroup(string shortCode, string groupName)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Task,shortCode,PermissionType.Write);
                var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);
                var taskExInfo = task.TaskExtension.TaskExtensionInfo;
                if (taskExInfo != null)
                {
                    var taskExCheckListInfo = CommUtils.FromJson<TaskExCheckListInfo>(taskExInfo);
                    CommUtils.AssertEquals(taskExCheckListInfo.CheckGroups.Count(x => x.GroupName == groupName),
                        1, "工作[" + task.Description + "]检查项分组[" + groupName + "]有误，请刷新后重试");

                    taskExCheckListInfo.CheckGroups.RemoveAll(x => x.GroupName == groupName);

                    task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(taskExCheckListInfo);
                }

                task.TaskHandler = CurrentUserName;

                m_dbAdapter.Task.UpdateTaskExtension(task.TaskExtension);
                m_dbAdapter.Task.UpdateTask(task);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditTask, task.ProjectId,
                    "更新Task[" + task.Description + "(" + task.ShortCode + ")]，" + "删除扩展工作检查项分组[" + groupName + "]", "");
                var comment = "删除分组[" + groupName + "]";
                m_dbAdapter.Task.AddTaskLog(task, comment);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DeleteExtensionCheckItem(string shortCode, string checkItemName, string groupName)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Task,shortCode,PermissionType.Write);
                var task = m_dbAdapter.Task.GetTaskWithExInfo(shortCode);
                var taskExInfo = task.TaskExtension.TaskExtensionInfo;
                if (taskExInfo != null)
                {
                    var taskExCheckListInfo = CommUtils.FromJson<TaskExCheckListInfo>(taskExInfo);
                    foreach (var checkItemGroup in taskExCheckListInfo.CheckGroups)
                    {
                        if (checkItemGroup.GroupName == groupName)
                        {
                            CommUtils.AssertEquals(checkItemGroup.CheckItems.Count(x => x.Name == checkItemName), 1,
                            "工作[" + task + "]检查项分组[" + checkItemGroup.GroupName + "]下的[" + checkItemName + "]项有误，请刷新后重试");
                            checkItemGroup.CheckItems.RemoveAll(x => x.Name == checkItemName);
                        }
                    }
                    task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(taskExCheckListInfo);
                }

                task.TaskHandler = CurrentUserName;

                m_dbAdapter.Task.UpdateTaskExtension(task.TaskExtension);
                m_dbAdapter.Task.UpdateTask(task);
                m_dbAdapter.Project.NewEditProductLog(EditProductType.EditTask, task.ProjectId,
                    "更新Task[" + task.Description + "(" + task.ShortCode + ")]，" + "删除扩展工作检查项分组[" + groupName + "]下的检查项[" + checkItemName + "]", "");
                var comment = "删除分组[" + groupName + "]中的工作要点[" + checkItemName + "]";
                m_dbAdapter.Task.AddTaskLog(task, comment);

                return ActionUtils.Success("");
            });
        }

        #endregion

    }
}