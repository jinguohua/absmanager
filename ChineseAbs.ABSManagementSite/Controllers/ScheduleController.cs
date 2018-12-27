using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Pattern;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using FilePattern;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class ScheduleController : BaseController
    {
        public ActionResult Index(int? page, int? pageSize, string timeRange, string projectGuid, string paymentDay, string taskStatusList)
        {
            int projectId = -1;

            //[Check Authorization]
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();

            var scheduleView = new ScheduleViewModel();

            var upperLimitDate = DateTime.Parse("9999-12-31");
            var lowerLimitDate = DateTime.Parse("1753-01-02");
            if (!string.IsNullOrWhiteSpace(projectGuid))
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                projectId = project.ProjectId;

                CommUtils.Assert(authorizedProjectIds.Contains(projectId), "当前用户没有读取产品[{0}]偿付期列表的权限", project.Name);
                if (paymentDay != null)
                {
                    GetLimitDates(projectGuid, paymentDay, ref upperLimitDate, ref lowerLimitDate);
                    scheduleView.PaymentDay = paymentDay;
                }
            }

            DateTime? endTime = null;
            if (timeRange != null)
            {
                var enumTime = CommUtils.ParseEnum<TaskFilterTime>(timeRange);
                scheduleView.FilterTime = timeRange;
                if (enumTime != TaskFilterTime.All)
                {
                    endTime = ParseFilterTime(enumTime);
                }
            }

            var taskStatusValues = new List<TaskStatus>();
            if (taskStatusList != null)
            {
                taskStatusValues = CommUtils.ParseEnumList<TaskStatus>(taskStatusList);
                scheduleView.FilterStatus = CommUtils.Split(taskStatusList).ToList();
            }
            else 
            {
                taskStatusValues = new List<TaskStatus>(){
                    TaskStatus.Waitting,
                    TaskStatus.Running, 
                    TaskStatus.Finished, 
                    TaskStatus.Skipped,
                    TaskStatus.Overdue,
                    TaskStatus.Error
                };
            }

            var tasks = m_dbAdapter.Task.GetTasks(page ?? 1, pageSize ?? 10, endTime, projectId,
                taskStatusValues, authorizedProjectIds, Toolkit.DateToString(upperLimitDate),
                Toolkit.DateToString(lowerLimitDate), paymentDay ?? "1753-01-02");

            scheduleView.PageInfo = Toolkit.ConvertPageInfo(tasks);
            
            
            //short code， name
            var taskShortCodeDict = new Dictionary<int, Tuple<string, string>>();
            foreach (var task in tasks.Items)
            {
                var taskView = Toolkit.ConvertTask(task);
                if (taskView != null)
                {
                    scheduleView.Tasks.Add(taskView);
                    taskShortCodeDict[int.Parse(taskView.Id)] = Tuple.Create(taskView.ShortCode, taskView.TaskName);
                }
            }

            var projectCache = new Dictionary<int, Project>();
            for (int i = 0; i < scheduleView.Tasks.Count; ++i)
            {
                var task = tasks.Items[i];
                var taskView = scheduleView.Tasks[i];
                var curProjectId = task.ProjectId;
                if (projectCache.ContainsKey(curProjectId))
                {
                    taskView.ProjectName = projectCache[curProjectId].Name;
                    taskView.ProjectGuid = projectCache[curProjectId].ProjectGuid;
                }
                else
                {
                    var project = m_dbAdapter.Project.GetProjectById(curProjectId);
                    taskView.ProjectName = project.Name;
                    taskView.ProjectGuid = project.ProjectGuid;
                    projectCache[curProjectId] = project;
                }

                taskView.PrevTaskShortCodeArray = new List<string>();
                taskView.PrevTaskNameArray = new List<string>();
                if (!string.IsNullOrEmpty(taskView.PrevTasksNames) && taskView.PrevTasksNames != "-")
                {
                    string prevTaskNames = string.Empty;
                    var prevTaskIds = CommUtils.Split(taskView.PrevTasksNames).ToList().ConvertAll(x => int.Parse(x));
                    foreach (var prevTaskId in prevTaskIds)
                    {
                        if (!taskShortCodeDict.ContainsKey(prevTaskId))
                        {
                            var tempTask = m_dbAdapter.Task.GetTask(prevTaskId);
                            taskShortCodeDict[tempTask.TaskId] = Tuple.Create(tempTask.ShortCode, tempTask.Description);
                        }

                        if (!string.IsNullOrEmpty(prevTaskNames))
                        {
                            prevTaskNames += CommUtils.Spliter;
                        }

                        taskView.PrevTaskShortCodeArray.Add(taskShortCodeDict[prevTaskId].Item1);
                        taskView.PrevTaskNameArray.Add(taskShortCodeDict[prevTaskId].Item2);
                        prevTaskNames += taskShortCodeDict[prevTaskId].Item1;
                    }

                    taskView.PrevTasksNames = prevTaskNames;
                }
            }

            //Get projects info
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
            scheduleView.Projects = new List<ProjectViewModel>();
            foreach (var project in projects)
            {
                var projectView = Toolkit.ConvertProject(project);
                if (projectView != null)
                {
                    scheduleView.Projects.Add(projectView);
                }
            }
            
            return View(scheduleView);
        }

        private void GetLimitDates(string projectGuid, string paymentDay, ref DateTime upperLimitDate, ref DateTime lowerLimitDate)
        {
            CommUtils.Assert(DateTime.TryParse(paymentDay, out upperLimitDate), "偿付期[{0}]格式错误", paymentDay);

            var logicModel = Platform.GetProject(projectGuid);
            var allPaymentDates = logicModel.GetAllPaymentDates(logicModel.DealSchedule.Instanse.PaymentDates);
            var index = allPaymentDates.IndexOf(upperLimitDate);
            CommUtils.Assert(index != -1, "偿付期[{0}]不存在", paymentDay);
            if (index != 0)
            {
                lowerLimitDate =  allPaymentDates[index - 1];
            }
        }

        [HttpPost]
        public ActionResult GetPaymentDays(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var authorizedIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                CommUtils.Assert(authorizedIds.Contains(project.ProjectId), "当前用户没有读取产品[{0}]偿付期列表的权限", project.Name);

                ProjectLogicModel logicModel = null;
                try
                {
                    logicModel = Platform.GetProject(projectGuid);
                }
                catch (ApplicationException e)
                {
                    var errorResult = new
                    {
                        paymentDays = new List<object>(),
                        briefMessage = "* 该产品模型加载失败",
                        fullMessage = e.Message
                    };
                    return ActionUtils.Success(errorResult);
                }

                var allPaymentDates = logicModel.GetAllPaymentDates(logicModel.DealSchedule.Instanse.PaymentDates);
                var now = DateTime.Now;

                DateTime? currentPaymentDay = null;
                if (allPaymentDates.Count > 0)
                {
                    var date = now.AddDays(-1);
                    if (date <= allPaymentDates[0])
                    {
                        currentPaymentDay = allPaymentDates[0];
                    }
                    else
                    {
                        currentPaymentDay = allPaymentDates.FirstOrDefault(x => x >= date && date > allPaymentDates[allPaymentDates.IndexOf(x) - 1]);
                    }
                }

                var result = new
                {
                    paymentDays = allPaymentDates.Select(x => new
                    {
                        PaymentDay = Toolkit.DateToString(x),
                        Sequence = allPaymentDates.IndexOf(x) + 1,
                        IsCurrentPaymentDay = currentPaymentDay != null && (x == currentPaymentDay.Value)
                    }).ToList(),
                    briefMessage = allPaymentDates.Count() > 0 ? "" : "* 模型数据错误，无偿付期数据",
                    fullMessage = allPaymentDates.Count() > 0 ? "" : "* 模型数据错误，无偿付期数据"
                };
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetNewestTaskStatus(string shortCodeList, string taskStatusList)
        {
            return ActionUtils.Json(() =>
            {
                var authorizedIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var shortCodes = CommUtils.Split(shortCodeList);
                var tasks = m_dbAdapter.Task.GetTasks(shortCodes);

                var taskStatus = CommUtils.ParseEnumList<TaskStatus>(taskStatusList);

                var resultList = new List<TaskShortCodeStatus>();

                if (tasks.Count != taskStatus.Count)
                {
                    return ActionUtils.Success(resultList);
                }

                for (int i = 0; i < tasks.Count; i++)
                {
                    var task = tasks[i];
                    CommUtils.Assert(authorizedIds.Contains(task.ProjectId), "工作列表页面发生异常，请刷新页面后重试");
                    if (task.TaskStatus != taskStatus[i])
                    {
                        var taskShortCodeStatus = new TaskShortCodeStatus();
                        taskShortCodeStatus.ShortCode = task.ShortCode;
                        taskShortCodeStatus.TaskStatus = task.TaskStatus.ToString();
                        resultList.Add(taskShortCodeStatus);
                    }
                }

                return ActionUtils.Success(resultList);
            });
        }

        [HttpPost]
        public JsonResult GetTasks(string timeRange, string projectGuid, string taskStatusList,string paymentDay)
        {
            int projectId = -1;
            var upperLimitDate = DateTime.Parse("9999-12-31");
            var lowerLimitDate = DateTime.Parse("1753-01-02");
            if (!string.IsNullOrWhiteSpace(projectGuid))
            {
                var authorizedIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                projectId = project.ProjectId;

                CommUtils.Assert(authorizedIds.Contains(projectId), "当前用户没有读取产品[{0}]偿付期列表的权限", project.Name);
                if (paymentDay != null)
                {
                    GetLimitDates(projectGuid, paymentDay, ref upperLimitDate, ref lowerLimitDate);
                }

            }

            DateTime? endTime = null;
            if (timeRange != string.Empty)
            {
                var enumTime = CommUtils.ParseEnum<TaskFilterTime>(timeRange);
                if (enumTime != TaskFilterTime.All)
                {
                    endTime = ParseFilterTime(enumTime);
                }
            }

            var taskStatusValues = CommUtils.ParseEnumList<TaskStatus>(taskStatusList);

            //[Check Authorization]
            var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
            var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
            var dictProject = projects.ToDictionary(x => x.ProjectId);

            var tasks = m_dbAdapter.Task.GetTasks(1, 100000, endTime, projectId, taskStatusValues, authorizedProjectIds, Toolkit.DateToString(upperLimitDate), Toolkit.DateToString(lowerLimitDate), paymentDay ?? "1753-01-02");
            

            var result = new SAFS.Utility.Web.JsonResultDataEntity<List<dynamic>>();
            result.Data = tasks.Items.Select(x => new
            {
                ShortCode = x.ShortCode,
                TaskName = x.Description,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                Status = x.TaskStatus.ToString(),
                Detail = x.TaskDetail,
                Target = x.TaskTarget,
                ProjectName = dictProject[x.ProjectId].Name
            }).ToList().Cast<dynamic>().ToList();

            return Json(result);
        }

        private DateTime ParseFilterTime(TaskFilterTime time)
        {
            DateTime result = DateTime.Today;

            switch (time)
            {
                case TaskFilterTime.MonthAhead:
                    result = result.AddMonths(1);
                    break;
                case TaskFilterTime.WeekAhead:
                    result = result.AddDays(7);
                    break;
                case TaskFilterTime.Future:
                    result = result.AddYears(100);
                    break;
                case TaskFilterTime.Today:
                    break;
                default:
                    throw new ApplicationException("ParseFilterTime [" + time + "] failed.");
            }

            return result;
        }

        [HttpPost]
        public ActionResult IsValidTaskShortCode(string shortCode)
        {
            return ActionUtils.Json(() => { 
                CommUtils.Assert(m_dbAdapter.Task.TaskExists(shortCode), "工作代码[" + shortCode + "]不存在");
                return ActionUtils.Success(shortCode);
            });
        }

        [HttpPost]
        public ActionResult GetAllShortCode()
        {
            return ActionUtils.Json(() =>
            {
                var ids = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                CommUtils.Assert(ids.Count() != 0, "暂无相关产品，请联系管理员创建相关产品");

                var shortCodeList = m_dbAdapter.Task.GetShortCodesByProjectIds(ids);
                return ActionUtils.Success(shortCodeList);
            });
        }

        private List<Task> ParseTaskTable(List<List<object>> table)
        {
            var excelTaskNames = table.ConvertAll(x => x[1].ToString());

            for (int i = 0; i < excelTaskNames.Count; i++)
            {
                CommUtils.Assert(excelTaskNames[i].Length <= 30, "工作列表文件解析错误（Row:{0}）:工作名称[{1}]的最大长度为30字符数", (i + 1), excelTaskNames[i]);
            }

            var tasks = new List<Task>();
            for (int i = 0; i < table.Count; i++)
            {
                try
                {
                    tasks.Add(ParseTaskRow(table[i]));
                }
                catch (Exception e)
                {
                    CommUtils.Assert(false, "工作列表文件解析错误（Row:" + (i + 1).ToString() + "）:" + e.Message);
                }
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                CommUtils.AssertHasContent(tasks[i].Description, "工作列表文件解析错误（Row:" + (i + 1) + "）:" + "模板工作名称不能为空");
            }

            return tasks;
        }

        private Task ParseTaskRow(List<object> objs)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i] == null)
                {
                    objs[i] = string.Empty;
                }
            }

            Task task = new Task();
            task.Description = objs[1].ToString();
            DateTime date;
            if (DateTime.TryParse(objs[2].ToString(), out date))
            {
                task.StartTime = date;
            }

            task.EndTime = DateTime.Parse(objs[3].ToString());

            task.TaskDetail = objs[4].ToString();
            task.TaskTarget = objs[5].ToString();

            var taskExType = objs[6].ToString();
            if (!string.IsNullOrWhiteSpace(taskExType))
            {
                task.TaskExtension = new ABSManagement.Models.TaskExtension();
                task.TaskExtension.TaskExtensionName = CommUtils.ParseEnum<TaskExtensionType>(taskExType).ToString();
            }

            return task;
        }

        [HttpPost]
        public ActionResult ImportTasks(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(projectGuid, "请选择产品");

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(Request.Files.Count > 0, "请选择文件");

                var file = Request.Files[0];
                CommUtils.Assert(file.ContentLength > 0, "请选择文件");

                CommUtils.Assert(file.FileName.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase),
                    "只支持导入Excel(.xlsx)文件");

                file.InputStream.Seek(0, SeekOrigin.Begin);
                var table = ExcelUtils.ParseExcel(file.InputStream, 0, 1, 0, 7);
                CommUtils.Assert(table.Count >= 1, "导入工作为空");

                var projectNames = table.Select(x => x[0].ToString()).ToList();


                var index = projectNames.FindIndex(x => x != project.Name);
                if (index >= 0)
                {
                    CommUtils.Assert(false, "上传工作中包含[{0}]的产品名称，和当前产品[{1}]不一致", projectNames[index], project.Name);
                }

                List<Task> tasks = ParseTaskTable(table);
                foreach (var task in tasks)
                {
                    if (task.StartTime.HasValue && task.EndTime.HasValue)
                    {
                        CommUtils.Assert(task.EndTime.Value >= task.StartTime.Value, "工作[{0}]的截止时间[{1}]不能早于开始时间[{2}]",
                            task.Description, Toolkit.DateToString(task.EndTime), Toolkit.DateToString(task.StartTime));
                    }

                    task.ProjectId = project.ProjectId;
                    task.TaskStatus = (DateTime.Today <= task.EndTime) ? TaskStatus.Waitting : TaskStatus.Overdue;
                }

                foreach (var task in tasks)
                {
                    ABSManagement.Models.TaskExtension taskEx = null;
                    if (task.TaskExtension != null && !string.IsNullOrWhiteSpace(task.TaskExtension.TaskExtensionName))
                    {
                        taskEx = new ABSManagement.Models.TaskExtension();
                        taskEx.TaskExtensionName = task.TaskExtension.TaskExtensionName;
                        taskEx.TaskExtensionType = task.TaskExtension.TaskExtensionName;
                        taskEx = m_dbAdapter.Task.NewTaskExtension(taskEx);
                    }

                    if (taskEx != null)
                    {
                        task.TaskExtensionId = taskEx.TaskExtensionId;
                    }

                    var newTask = m_dbAdapter.Task.NewTask(task);

                    LogEditProduct(EditProductType.EditTask, null,
                        "从Excel导入工作", "projectGuid=[" + newTask.ProjectId + "] taskId=[" + newTask.TaskId
                        + "] shortCode=[" + newTask.ShortCode + "] taskName=[" + newTask.Description + "]");
                }

                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult ExportTasks(string projectGuid, string paymentDay, string timeRange,
            string taskStatusList)
        {
            return ActionUtils.Json(() =>
            {
                var projectId = -1;
                var authorizedProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds();
                var projects = m_dbAdapter.Project.GetProjects(authorizedProjectIds);
                var dictProjects = projects.ToDictionary(x => x.ProjectId);
                var upperLimitDate = DateTime.Parse("9999-12-31");
                var lowerLimitDate = DateTime.Parse("1753-01-02");

                if (!string.IsNullOrWhiteSpace(projectGuid))
                {
                    var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                    projectId = project.ProjectId;

                    CommUtils.Assert(authorizedProjectIds.Contains(projectId), "当前用户没有读取产品[{0}]偿付期列表的权限", project.Name);
                    if (!string.IsNullOrWhiteSpace(paymentDay))
                    {
                        GetLimitDates(projectGuid, paymentDay, ref upperLimitDate, ref lowerLimitDate);
                    }
                }

                var taskStatusValues = new List<TaskStatus>();
                if (taskStatusList != null)
                {
                    taskStatusValues = CommUtils.ParseEnumList<TaskStatus>(taskStatusList);
                }
                else
                {
                    taskStatusValues = new List<TaskStatus>(){
                        TaskStatus.Waitting,
                        TaskStatus.Running, 
                        TaskStatus.Finished, 
                        TaskStatus.Skipped,
                        TaskStatus.Overdue,
                        TaskStatus.Error
                    };
                }

                DateTime? endTime = null;
                if (!string.IsNullOrWhiteSpace(timeRange))
                {
                    var enumTime = CommUtils.ParseEnum<TaskFilterTime>(timeRange);
                    if (enumTime != TaskFilterTime.All)
                    {
                        endTime = ParseFilterTime(enumTime);
                    }
                }

                var tasks = m_dbAdapter.Task.GetTasks(1, 1000000, endTime, projectId,
                taskStatusValues, authorizedProjectIds, Toolkit.DateToString(upperLimitDate),
                Toolkit.DateToString(lowerLimitDate), paymentDay ?? "1753-01-02");

                TaskPattern patternObj = new TaskPattern();

                patternObj.Tasks = tasks.Items.ConvertAll(x => new TaskItem
                {
                    ProjectName = dictProjects[x.ProjectId].Name,
                    TaskName = x.Description,
                    BeginTime = Toolkit.DateToString(x.StartTime),
                    EndTime = Toolkit.DateToString(x.EndTime),
                    TaskDetail = x.TaskDetail,
                    TaskTarget = x.TaskTarget,
                    TaskExtensionName = (x.TaskExtension != null
                        && !string.IsNullOrWhiteSpace(x.TaskExtension.TaskExtensionName)
                        ? x.TaskExtension.TaskExtensionName : string.Empty)
                }).ToList();

                var excelPattern = new ExcelPattern();
                string patternFilePath = DocumentPattern.GetPath(DocPatternType.TaskList);

                MemoryStream ms = new MemoryStream();
                if (!excelPattern.Generate(patternFilePath, patternObj, ms))
                {
                    CommUtils.Assert(false, "Generate file failed.");
                }
                ms.Seek(0, SeekOrigin.Begin);

                string fileFullName = projectId == -1 ? "工作列表.xlsx"
                    : dictProjects[projectId].Name + "_工作列表.xlsx";
                var resource = ResourcePool.RegisterMemoryStream(CurrentUserName, fileFullName, ms);
                return ActionUtils.Success(resource.Guid.ToString());
            });
        }

        private void LogEditProduct(EditProductType type, int? projectId, string description, string comment)
        {
            m_dbAdapter.Project.NewEditProductLog(type, projectId, description, comment);
        }
    }
}