using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.LogicModels.DealModel;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Controllers.TaskExtension;
using ChineseAbs.ABSManagementSite.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class TaskController : BaseController
    {
        #region results

        public ActionResult Index(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
            {
                return RedirectToAction("Index", "Schedule");
            }

            var task = m_dbAdapter.Task.GetTask(shortCode);
            var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);
            task.ProjectName = project.Name;

            var taskViewModel = Toolkit.ConvertTask(task);
            GetPrevAndNextTaskShortCode(taskViewModel,task);
            taskViewModel.ProjectGuid = project.ProjectGuid;
            taskViewModel.TaskHandler = Platform.UserProfile.GetDisplayRealNameAndUserName(taskViewModel.TaskHandler);

            taskViewModel.PrevTaskNameArray = new List<string>();
            taskViewModel.PrevTaskShortCodeArray = new List<string>();
            foreach (var prevTaskId in task.PrevTaskIdArray)
            {
                var prevTask = m_dbAdapter.Task.GetTask(prevTaskId);
                taskViewModel.PrevTaskShortCodeArray.Add(prevTask.ShortCode);
                taskViewModel.PrevTaskNameArray.Add(prevTask.Description);
            }

            //Get task extension info.
            if (task.TaskExtensionId.HasValue)
            {
                var taskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);
                taskExtension.TaskExtensionHandler = Platform.UserProfile.GetDisplayRealNameAndUserName(taskExtension.TaskExtensionHandler);
                taskViewModel.TaskExtension = Toolkit.ConvertTaskExtension(taskExtension);

                var instance = TaskExFactory.CreateInstance(taskViewModel.TaskExtension.Type, shortCode, CurrentUserName);
                if (instance != null)
                {
                    //扩展工作页面初始化时的异常返回错误信息
                    object entity;
                    try
                    {
                        entity = instance.GetEntity();
                    }
                    catch (ApplicationException e)
                    {
                        entity = e.Message;
                    }
                    taskViewModel.TaskExtension.Info = entity;
                }
            }

            //Get task status history info.
            var taskStatusHitory = m_dbAdapter.Task.GetTaskStatusHistory(task.TaskId);
            if (taskStatusHitory != null)
            {
                var usernameCache = new Dictionary<string, string>();

                taskViewModel.TaskStatusHistory = new List<TaskStatusHistoryViewModel>();
                foreach (var item in taskStatusHitory)
                {
                    var viewModel = Toolkit.ConvertTaskStatusHistory(item);

                    string name = string.Empty;
                    if (usernameCache.ContainsKey(viewModel.TimeStampUserName))
                    {
                        name = usernameCache[viewModel.TimeStampUserName];
                    }
                    else
                    {
                        name = m_dbAdapter.Authority.GetNameByUserName(viewModel.TimeStampUserName);
                        usernameCache[viewModel.TimeStampUserName] = name;
                    }

                    name = string.IsNullOrEmpty(name) ? viewModel.TimeStampUserName
                        : (name + "(" + viewModel.TimeStampUserName + ")");
                    viewModel.TimeStampUserName = name;

                    taskViewModel.TaskStatusHistory.Add(viewModel);
                }
            }

            taskViewModel.ProjectSeriesStage = ProjectSeriesStage.存续期;
            if (project.ProjectSeriesId.HasValue && project.TypeId.HasValue && project.TypeId.Value == 1)
            {
                taskViewModel.ProjectSeriesStage = ProjectSeriesStage.发行;

                var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
                taskViewModel.ProjectSeriesGuid = projectSeries.Guid;
            }

            return View(taskViewModel);
        }

        private void GetPrevAndNextTaskShortCode(TaskViewModel taskViewModel, Task task)
        {
            var tasks = new List<Task>();
            if (task.TaskGroupId.HasValue)
            {
                tasks = m_dbAdapter.Task.GetByTaskGroupId(task.TaskGroupId.Value);
            }
            else
            {
                tasks = m_dbAdapter.Task.GetTasksByProjectId(task.ProjectId);
            }

            var shortCodes = tasks.ConvertAll(x => x.ShortCode);
            var index = shortCodes.IndexOf(task.ShortCode);
            if (index != -1 && tasks.Count > 1)
            {
                if (index == 0)
                {
                    taskViewModel.PreviousTaskShortCode = null;
                    taskViewModel.PreviousTaskName = null;
                    taskViewModel.NextTaskShortCode = tasks[1].ShortCode;
                    taskViewModel.NextTaskName = tasks[1].Description;
                }
                else if (index == tasks.Count - 1)
                {
                    taskViewModel.PreviousTaskShortCode = tasks[index - 1].ShortCode;
                    taskViewModel.PreviousTaskName = tasks[index - 1].Description;
                    taskViewModel.NextTaskShortCode = null;
                    taskViewModel.NextTaskName = null;
                }
                else
                {
                    taskViewModel.PreviousTaskShortCode = tasks[index - 1].ShortCode;
                    taskViewModel.PreviousTaskName = tasks[index - 1].Description;
                    taskViewModel.NextTaskShortCode = tasks[index + 1].ShortCode;
                    taskViewModel.NextTaskName = tasks[index + 1].Description;
                }
            }
            
        }

        [HttpPost]
        public ActionResult RemoveTask(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var task = m_dbAdapter.Task.GetTask(shortCode);
                var project = m_dbAdapter.Project.GetProjectById(task.ProjectId);
                CommUtils.AssertNotNull(project.ProjectSeriesId, "查找ProjectSeries失败，shortCode={0}", shortCode);
                CommUtils.AssertNotNull(task.TaskGroupId, "查找TaskGroup失败，shortCode={0}", shortCode);

                CheckPermission(PermissionObjectType.Task, shortCode, PermissionType.Write);
                task.TaskStatus = TaskStatus.Deleted;
                int removeRecordsCount = m_dbAdapter.Task.UpdateTask(task);

                //删除项目问题中对应的受阻工作
                var connectTasks = m_dbAdapter.IssueConnectionTasks.GetConnectionTasksByShortCode(shortCode);
                m_dbAdapter.IssueConnectionTasks.DeleteConnectionTasks(connectTasks);

                var logicModel = Platform.GetProject(project.ProjectGuid);
                logicModel.Activity.Add(project.ProjectId, ActivityObjectType.Task, task.ShortCode, "删除工作：" + task.Description);

                LogEditProduct(EditProductType.EditTask, project.ProjectId, "删除任务",
                    "删除一条任务：id=" + task.TaskId + ";name="+ task.Description + ";shortCode=" + task.ShortCode);

                return ActionUtils.Success(removeRecordsCount);
            });
        }

        [HttpPost]
        public ActionResult GetTaskInfo(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var task = m_dbAdapter.Task.GetTask(shortCode);

                CommUtils.AssertNotNull(task.TaskGroupId, "查找TaskGroup失败，shortCode={0}", shortCode);
                CheckPermission(PermissionObjectType.Task, shortCode, PermissionType.Read);

                if (task.TaskExtensionId.HasValue)
                {
                    task.TaskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);
                }

                var taskInfo = new
                {
                    shortCode = task.ShortCode,
                    taskName = task.Description,
                    startTime = Toolkit.DateToString(task.StartTime),
                    endTime = Toolkit.DateToString(task.EndTime),
                    taskExType = task.TaskExtensionId.HasValue ? task.TaskExtension.TaskExtensionType.ToString() : string.Empty,
                    status = Toolkit.ToCnString(task.TaskStatus),
                    detail = task.TaskDetail,
                    target = task.TaskTarget,
                    personInCharge = task.PersonInCharge,
                    personInChargeUserProfile = Platform.UserProfile.Get(task.PersonInCharge),
                };

                return ActionUtils.Success(taskInfo);
            });
        }

        [HttpPost]
        public ActionResult ModifyTask(string shortCode, string name, string startTime, string endTime,
            string taskExtensionType, string taskDetail, string taskTarget, string personInCharge)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(DateUtils.IsNullableDate(startTime), "开始时间必须为[YYYY-MM-DD]格式或者为空");

                var taskStartTime = DateUtils.Parse(startTime);
                if (taskStartTime != null)
                {
                    CommUtils.Assert(DateTime.Parse(endTime) >= taskStartTime.Value,
                        "开始时间[{0}]不能大于截止时间[{1}]", startTime, endTime);
                }

                var task = m_dbAdapter.Task.GetTask(shortCode);
                if (task.TaskExtensionId.HasValue)
                {
                    task.TaskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);
                }

                if (!string.IsNullOrWhiteSpace(personInCharge))
                {
                    CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(personInCharge), "负责人[{0}]不存在", personInCharge);
                }

                CommUtils.AssertNotNull(task.TaskGroupId, "查找TaskGroup失败，shortCode={0}", shortCode);

                CheckPermission(PermissionObjectType.Task, shortCode, PermissionType.Write);

                ValidateUtils.Name(name, "工作名");
                CommUtils.AssertHasContent(endTime, "截止时间不能为空");

                if (string.IsNullOrWhiteSpace(taskExtensionType))
                {
                    task.TaskExtensionId = null;
                }
                else
                {
                    if (!task.TaskExtensionId.HasValue
                        || task.TaskExtension.TaskExtensionType != CommUtils.ParseEnum<TaskExtensionType>(taskExtensionType).ToString())
                    {
                        var taskExType = CommUtils.ParseEnum<TaskExtensionType>(taskExtensionType);
                        var taskEx = ChineseAbs.ABSManagement.Models.TaskExtension.Create(taskExType);
                        taskEx = m_dbAdapter.Task.NewTaskExtension(taskEx);
                        task.TaskExtensionId = taskEx.TaskExtensionId;
                    }
                }

                var taskEndTime = DateTime.Parse(endTime);

                task.Description = name;
                task.StartTime = taskStartTime;
                task.EndTime = taskEndTime;
                task.PersonInCharge = string.IsNullOrWhiteSpace(personInCharge) ? null : personInCharge;
                task.TaskDetail = taskDetail;
                task.TaskTarget = taskTarget;

                if (task.TaskStatus == TaskStatus.Overdue && taskEndTime >= DateTime.Now.Date)
                {
                    task.TaskStatus = TaskStatus.Waitting;
                }
                if (task.TaskStatus == TaskStatus.Waitting && taskEndTime < DateTime.Now.Date)
                {
                    task.TaskStatus = TaskStatus.Overdue;
                }

                var result = m_dbAdapter.Task.UpdateTask(task);

                var msg = string.Format("修改task:shortCode={0};name={1};startTime={2};endTime={3};taskExtensionType={4};taskDetail={5};taskTarget={6}",
                    shortCode, name, startTime, endTime, taskExtensionType, taskDetail, taskTarget);
                LogEditProduct(EditProductType.EditTask, task.ProjectId, msg, "");

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CreateTask(string projectGuid, string name, string startTime, string endTime,
            string prevTaskShortCodes, string taskExtensionType, string taskDetail, string taskTarget,
            string taskGroupGuid, string personInCharge)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.TaskGroup, taskGroupGuid, PermissionType.Write);

                CommUtils.Assert(DateUtils.IsNullableDate(startTime), "开始时间必须为[YYYY-MM-DD]格式或者为空");
                ValidateUtils.Name(name, "工作名称");
                CommUtils.AssertHasContent(endTime, "截止时间不能为空");

                CommUtils.Assert(taskTarget.Length <= 100000, "工作目标不能超过100000个字符");
                CommUtils.Assert(taskDetail.Length <= 100000, "工作描述不能超过100000个字符");
                if (!string.IsNullOrWhiteSpace(personInCharge))
                {
                    CommUtils.Assert(m_dbAdapter.Authority.IsUserExist(personInCharge), "负责人[{0}]不存在", personInCharge);
                }
                var taskStartTime = DateUtils.Parse(startTime);
                if (taskStartTime != null)
                {
                    CommUtils.Assert(DateTime.Parse(endTime) >= taskStartTime,
                        "开始时间[{0}]不能大于截止时间[{1}]", startTime, endTime);
                }

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                int? taskGroupId = null;
                if (!string.IsNullOrWhiteSpace(taskGroupGuid))
                {
                    var taskGroup = m_dbAdapter.TaskGroup.GetByGuid(taskGroupGuid);
                    CommUtils.AssertEquals(taskGroup.ProjectId, project.ProjectId, "输入工作组[TaskGroupGuid={0}]和项目[ProjectGuid={1}]不属于同一个项目", taskGroupGuid, project.ProjectGuid);
                    taskGroupId = taskGroup.Id;
                }

                int? taskExId = null;
                if (!string.IsNullOrWhiteSpace(taskExtensionType))
                {
                    var taskExType = CommUtils.ParseEnum<TaskExtensionType>(taskExtensionType);
                    var taskEx = ChineseAbs.ABSManagement.Models.TaskExtension.Create(taskExType);
                    taskEx = m_dbAdapter.Task.NewTaskExtension(taskEx);
                    taskExId = taskEx.TaskExtensionId;
                }

                var task = new Task();
                task.ProjectId = project.ProjectId;
                task.Description = name;
                task.ProjectName = project.Name;
                task.StartTime = taskStartTime;
                task.EndTime = DateTime.Parse(endTime);
                task.TaskExtensionId = taskExId;
                task.PersonInCharge = string.IsNullOrWhiteSpace(personInCharge) ? null : personInCharge;

                if (!string.IsNullOrWhiteSpace(prevTaskShortCodes))
                {
                    var shortCodes = CommUtils.Split(prevTaskShortCodes);
                    var prevTasks = m_dbAdapter.Task.GetTasks(shortCodes);
                    var prevTaskIds = prevTasks.Select(x => x.TaskId.ToString()).ToArray();
                    task.PreTaskIds = CommUtils.Join(prevTaskIds); 
                }

                task.TaskDetail = taskDetail;
                task.TaskTarget = taskTarget;
                task.TaskStatus = (DateTime.Today <= task.EndTime) ? TaskStatus.Waitting : TaskStatus.Overdue;
                task.TaskGroupId = taskGroupId;
                task = m_dbAdapter.Task.NewTask(task);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, project);
                var permissionLogicModel = new PermissionLogicModel(projectLogicModel);
                var projectSeries = m_dbAdapter.ProjectSeries.GetById(project.ProjectSeriesId.Value);
                permissionLogicModel.SetPermission(projectSeries, project, task.ShortCode, PermissionObjectType.Task);

                projectLogicModel.Activity.Add(project.ProjectId, ActivityObjectType.Task, task.ShortCode, "创建工作：" + task.Description);

                var msg = string.Format("创建task:shortCode={0};name={1};startTime={2};endTime={3};taskExtensionType={4};"
                    + "taskDetail={5};taskTarget={6};prevTaskShortCodes={7};projectGuid={8};taskGroupGuid={9}",
                    task.ShortCode, name, startTime, endTime, taskExtensionType, taskDetail, taskTarget, prevTaskShortCodes, projectGuid, taskGroupGuid);
                LogEditProduct(EditProductType.CreateTask, task.ProjectId, msg, "");

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult ExportTable(string tableBodyJson)
        {
            return ActionUtils.Json(() =>
            {
                var table = tableBodyJson.ToDataTable();

                var ms = ExcelUtils.ToExcelMemoryStream(table, "TaskTable.xlsx", CurrentUserName);
                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, "TaskTable.xlsx", ms);

                return ActionUtils.Success(resource.Guid);
            });
        }

        [HttpPost]
        public ActionResult GetPersonInCharges(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var projectSeries = projectSeriesLogicModel.Instance;
                var projectId = projectSeriesLogicModel.CurrentProject.Instance.ProjectId;

                CheckPermission(PermissionObjectType.ProjectSeries, projectSeriesGuid, PermissionType.Read);

                var adminUserNames = projectSeriesLogicModel.CurrentProject.Team.Chiefs.Select(x => x.UserName).ToList();

                var teamMembers = m_dbAdapter.TeamMember.GetByProjectId(projectId);
                var teamMemberUserNames = teamMembers.Select(x => x.UserName).ToList();

                var teamAdmins = m_dbAdapter.TeamAdmin.GetByProjectId(projectId);
                var teamAdminUserNames = teamAdmins.Select(x => x.UserName).ToList();

                var allUserNames = new List<string>();
                allUserNames.AddRange(adminUserNames);
                allUserNames.AddRange(teamAdminUserNames);
                allUserNames.AddRange(teamMemberUserNames);
                allUserNames = allUserNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                Platform.UserProfile.Precache(allUserNames);

                var result = allUserNames.ConvertAll(x => new {
                    UserInfo = Platform.UserProfile.Get(x),
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult ConfirmRecyclingPaymentDate(string shortCode, double currentAccountBalance, double paymentMoney)
        {
            return ActionUtils.Json(() =>
            {
                var handler = new TaskExRecyclingPaymentDate(CurrentUserName, shortCode);
                handler.Confirm(currentAccountBalance, paymentMoney);
                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult UpdateTaskExtensionStatus(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var task = m_dbAdapter.Task.GetTask(shortCode);
                CommUtils.Assert(task.TaskExtensionId.HasValue,"确认核对失败，工作[{0}]不包含扩展信息",task.Description);

                var taskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);
                taskExtension.TaskExtensionStatus = TaskExtensionStatus.NotMatch;

                m_dbAdapter.Task.UpdateTaskExtension(taskExtension);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult GetTaskStatus(string shortCode)
        {
            var task = m_dbAdapter.Task.GetTask(shortCode);
            var prevTaskStatus = new List<Dictionary<string, string>>();
            foreach (var prevTaskId in task.PrevTaskIdArray)
            {
                var prevTask = m_dbAdapter.Task.GetTask(prevTaskId);
                prevTaskStatus.Add(new Dictionary<string, string>
                {
                     { "ShortCode", prevTask.ShortCode },
                     { "TaskStatus", Toolkit.ToCnString(prevTask.TaskStatus) }
                });
            }

            var profile = Platform.UserProfile.Get(task.TaskHandler);
            var result = new {
                ShortCode = task.ShortCode,
                TaskStatus = Toolkit.ToCnString(task.TaskStatus),
                TaskHandlerUserName = profile == null ? task.TaskHandler : profile.UserName,
                TaskHandlerName = profile == null ? task.TaskHandler : profile.RealName,
                PrevTaskStatus = prevTaskStatus,
            };

            return new JsonResult() { Data = result };
        }

        [HttpPost]
        public ActionResult GetTaskStatusHistory(string shortCode, int? cachedRecordCount)
        {
            return ActionUtils.Json(() =>
            {
                var task = m_dbAdapter.Task.GetTask(shortCode);
                var taskStatusHitory = m_dbAdapter.Task.GetTaskStatusHistory(task.TaskId);
                if (taskStatusHitory == null
                    || (cachedRecordCount.HasValue && cachedRecordCount.Value == taskStatusHitory.Count))
                {
                    return ActionUtils.Success(new List<object>());
                }

                Platform.UserProfile.Precache(taskStatusHitory.Select(x => x.TimeStampUserName));
                var result = taskStatusHitory.Select(x => new
                {
                    Time = Toolkit.DateTimeToString(x.TimeStamp),
                    UserName = Platform.UserProfile.GetDisplayRealNameAndUserName(x.TimeStampUserName),
                    Comment = x.Comment,
                    Status = Toolkit.ToCnString(x.NewStatusId)
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult AddTaskLog(string shortCode, string comment)
        {
            return ActionUtils.Json(() =>
            {
                m_dbAdapter.Task.AddTaskLog(shortCode, comment);
                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult GetSubsequentTasksRecursively(string shortCode)
        {
            var tasks = m_dbAdapter.Task.GetSubsequentTasksRecursively(shortCode);
            var list = tasks.ConvertAll(x => new
            {
                ShortCode = x.ShortCode,
                TaskName = x.Description,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Status = x.TaskStatus.ToString(),
                Detail = x.TaskDetail,
                Target = x.TaskTarget
            }).ToList();

            return Content(JsonConvert.SerializeObject(list));
        }

        //修改Task状态
        [HttpPost]
        public ActionResult ChangeTaskStatus(string shortCode, string newTaskStatus, string comment)
        {
            return ActionUtils.Json(() =>
            {
                var task = m_dbAdapter.Task.GetTask(shortCode);
                var logicModel = Platform.GetProject(task.ProjectId);

                var oldStatus = task.TaskStatus;
                var newStatus = CommUtils.ParseEnum<TaskStatus>(newTaskStatus);

                if (newStatus == TaskStatus.Waitting && IsOverdue(task))
                {
                    newStatus = TaskStatus.Overdue;
                }

                var taskEx = CheckChangeTaskStatus(task, newStatus);

                //如果当前工作状态从完成修改为开始，修改后续工作的状态为未完成
                if (oldStatus == TaskStatus.Finished && newStatus != TaskStatus.Finished)
                {
                    var subsequentTasks = m_dbAdapter.Task.GetSubsequentTasksRecursively(shortCode);
                    subsequentTasks = subsequentTasks.Where(x => x.TaskStatus == TaskStatus.Finished).ToList();
                    subsequentTasks.ForEach(x => new TaskLogicModel(logicModel, x).Stop(comment));
                }

                var taskLogicModel = new TaskLogicModel(logicModel, task);

                //修改当前工作状态
                if ((task.TaskStatus == TaskStatus.Error
                    || task.TaskStatus == TaskStatus.Waitting
                    || task.TaskStatus == TaskStatus.Overdue)
                    && newStatus == TaskStatus.Finished)
                {
                    taskLogicModel.Start(comment).Finish(comment);
                }
                else
                {
                    m_dbAdapter.Task.ChangeTaskStatus(task, newStatus, comment);
                }

                //增加Activity
                logicModel.Activity.Add(task.ProjectId, ActivityObjectType.Task, task.ShortCode, "修改工作[" + task.Description + "]的工作状态为：" + Toolkit.ToCnString(newStatus));

                //触发工作状态改变事件
                if (taskEx != null)
                {
                    taskEx.InvokeStatusChanged(oldStatus, newStatus);
                }

                return ActionUtils.Success(Toolkit.ToCnString(newStatus));
            });
        }

        //验证Task状态是否可以被修改
        [HttpPost]
        public ActionResult CheckChangeTaskStatus(string shortCode, string newTaskStatus)
        {
            return ActionUtils.Json(() =>
            {
                var task = m_dbAdapter.Task.GetTask(shortCode);

                var newStatus = CommUtils.ParseEnum<TaskStatus>(newTaskStatus);
                if (newStatus == TaskStatus.Waitting && IsOverdue(task))
                {
                    newStatus = TaskStatus.Overdue;
                }

                CheckChangeTaskStatus(task, newStatus);
                return ActionUtils.Success("Pass");
            });
        }

        private TaskExBase CheckChangeTaskStatus(Task task, TaskStatus newStatus)
        {
            //新状态是否和现有状态相同
            CommUtils.Assert(task.TaskStatus != newStatus, "无法将任务状态从[{0}]修改为[{1}]",
                TranslateUtils.ToCnString(task.TaskStatus), TranslateUtils.ToCnString(newStatus));

            //前置工作是否完成
            if (newStatus == TaskStatus.Running || newStatus == TaskStatus.Finished)
            {
                foreach (var prevTaskId in task.PrevTaskIdArray)
                {
                    var prevTask = m_dbAdapter.Task.GetTask(prevTaskId);
                    CommUtils.Assert(prevTask.TaskStatus == TaskStatus.Finished || prevTask.TaskStatus == TaskStatus.Skipped,
                        "前置工作[{0}][{1}]未完成", prevTask.ShortCode, prevTask.Description);
                }
            }

            //扩展工作未完成，不能完成工作
            TaskExBase taskEx = null;
            if (task.TaskExtensionId.HasValue)
            {
                var taskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);
                taskEx = TaskExFactory.CreateInstance(taskExtension.TaskExtensionType, task.ShortCode, CurrentUserName);
                if (taskEx != null)
                {
                    var eventResult = taskEx.InvokeStatusChanging(task.TaskStatus, newStatus);
                    CommUtils.Assert(eventResult.EventResult != EventResult.Cancel, eventResult.Message);
                }
            }

            return taskEx;
        }

        private bool IsOverdue(Task task)
        {
            //工作的截止时间以天为单位
            return DateTime.Today > task.EndTime;
        }

        [HttpPost]
        public ActionResult ConfirmAssetCashflow(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var handler = new TaskExAssetCashflow(CurrentUserName, shortCode);
                handler.ConfirmAssetCashflow();

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult SaveNoteData(string shortCode)
        {
            try
            {
                var handler = new TaskExCashflow(CurrentUserName, shortCode);
                handler.SaveNoteData();
            }
            catch (Exception e)
            {
                return Content(e.Message + Environment.NewLine + e.StackTrace);
            }
            return Content("success");
        }

        #endregion

        private void LogEditProduct(EditProductType type, int? projectId, string description, string comment)
        {
            m_dbAdapter.Project.NewEditProductLog(type, projectId, description, comment);
        }
    }
}