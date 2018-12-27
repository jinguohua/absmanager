using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Manager;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.ResourcePool;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Controllers.TaskExtension;
using ChineseAbs.ABSManagementSite.Filters;
using ChineseAbs.ABSManagementSite.Models;
using ChineseAbs.CalcService.Data.NancyData;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    [DesignAccessAttribute]
    public class DesignProductController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var modifyModelProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyModel);
            var modifyTaskProjectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);

            var projectIds = new List<int>();
            projectIds.AddRange(modifyModelProjectIds);
            projectIds.AddRange(modifyTaskProjectIds);
            var projects = m_dbAdapter.Project.GetProjects(projectIds);

            var viewModel = new DesignProjectViewModel();
            viewModel.HasCreateProductAuthority = m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject);

            viewModel.Projects = new List<DesignProjectItem>();
            foreach (var project in projects)
            {
                var projectItem = new DesignProjectItem();
                projectItem.CnabsDealId = project.CnabsDealId;
                projectItem.CreateUserName = project.CreateUserName;
                projectItem.CreateTime = project.CreateTime;
                projectItem.ProjectName = project.Name;
                projectItem.ProjectGuid = project.ProjectGuid;
                projectItem.EnterpriseName = m_dbAdapter.Authority.GetAuthorizedEnterpriseName(project.ProjectId);
                projectItem.HasModifyModelAuthority = modifyModelProjectIds.Contains(project.ProjectId);
                projectItem.HasModifyTaskAuthority = modifyTaskProjectIds.Contains(project.ProjectId);
                viewModel.Projects.Add(projectItem);
            }

            return View(viewModel);
        }

        #region Create product

        public ActionResult CreateProject(HttpPostedFileBase ymlFile, string name, int? cnabsDealId)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject), "没有创建产品的权限");

                CommUtils.Assert(ymlFile != null ^ cnabsDealId.HasValue, "请选择模型或指定YML文件（不能同时选择/指定）");
                if (ymlFile != null)
                {
                    CommUtils.Assert(ymlFile.FileName.EndsWith(".yml", StringComparison.CurrentCultureIgnoreCase),
                       "文件[{0}]格式错误,请选择.yml格式的文件", ymlFile.FileName);
                }

                ValidateUtils.Name(name, "产品名称").FileName();

                if (!CommUtils.IsLocalDeployed())
                {
                    //本地版不检查用户机构信息
                    CommUtils.Assert(m_dbAdapter.Authority.GetEnterpriseId().HasValue, "新建任务获取机构信息失败");
                }

                var folderName = string.Empty;
                NancyDealData nancyDealData = null;
                if (!cnabsDealId.HasValue)
                {
                    nancyDealData = NancyUtils.GetNancyDealDataByFile(ymlFile.InputStream);
                    //调整：模型中没有Manager字段，使用“未命名”作为文件夹名
                    //CommUtils.AssertHasContent(nancyDealData.Manager, "模型中Issuer不能为空");
                    if (string.IsNullOrWhiteSpace(nancyDealData.Manager))
                    {
                        folderName = "未命名";
                    }
                    else
                    {
                        folderName = nancyDealData.Manager;
                    }
                }

                var modelId = m_dbAdapter.Model.NewModel(name);
                LogEditProduct(EditProductType.CreateProduct, null, "创建Model:" + name, "");

                var guid = m_dbAdapter.Project.NewProject(name, modelId, cnabsDealId);
                LogEditProduct(EditProductType.CreateProduct, null, "创建Product[" + name + "][" + guid + "]", "");

                var project = m_dbAdapter.Project.GetProjectByGuid(guid);
                var projectId = project.ProjectId;

                if (cnabsDealId.HasValue)
                {
                    var projectDealInfo = m_dbAdapter.Project.GetProjectDealInfoByProjectId(projectId);
                    if (string.IsNullOrWhiteSpace(projectDealInfo.Issuer))
                    {
                        projectDealInfo.Issuer = "未命名";
                    }

                    CommUtils.AssertHasContent(projectDealInfo.Issuer, "找不到Issuer信息，创建产品失败，您可以尝试使用YML文件创建产品");
                    folderName = projectDealInfo.Issuer;
                }

                //初始化产品文件夹
                if (!string.IsNullOrEmpty(folderName))
                {
                    var shortName = lib.Parsers.CPinyinConverter.GetFirst(folderName).ToLower();
                    project.Model.ModelFolder = shortName + @"\" + project.Name + "_" + project.ProjectGuid;
                    m_dbAdapter.Model.UpdateModel(project.Model);

                    var modelFolder = WebConfigUtils.RootFolder + project.Model.ModelFolder;
                    if (!Directory.Exists(modelFolder))
                    {
                        Directory.CreateDirectory(modelFolder);
                    }

                    if (ymlFile != null)
                    {
                        var ymlFilePath = modelFolder + @"\Script.yml";
                        ymlFile.SaveAs(ymlFilePath);
                    }
                }

                //初始化note信息
                List<Note> notes = new ProjectLogicModel(CurrentUserName, project).Notes;

                notes.ForEach(x => x.ProjectId = projectId);
                notes.ForEach(x => m_dbAdapter.Dataset.NewNote(x));
                var createNoteMsg = CommUtils.Join(notes.ConvertAll(x => x.NoteName + "[" + x.NoteId + "]").ToArray());
                LogEditProduct(EditProductType.CreateProduct, projectId, "创建Note[" + projectId + "]", createNoteMsg);

                //授权该产品给创建者的机构
                LogEditProduct(EditProductType.CreateProduct, projectId, "授权Project[" + projectId + "]", "");
                CommUtils.Assert(m_dbAdapter.Authority.AuthorizeToEnterprise(projectId), "新建任务机构授权失败");

                //授权创建者对该产品的修改模型/工作权限
                bool authorize = m_dbAdapter.Authority.AuthorizeDesignProject(projectId, AuthorityType.ModifyModel | AuthorityType.ModifyTask);
                CommUtils.Assert(authorize, "新建任务中，授权修改模型/工作失败");

                return ActionUtils.Success("");
            });
        }
        #endregion

        [HttpPost]
        public ActionResult UpdateYml(HttpPostedFileBase ymlFile, string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(AuthorityType.ModifyModel), "没有修改产品的权限");

                CommUtils.Assert(ymlFile != null, "请选择YML文件");
                if (ymlFile != null)
                {
                    CommUtils.Assert(ymlFile.FileName.EndsWith(".yml", StringComparison.CurrentCultureIgnoreCase),
                       "文件[{0}]格式错误,请选择.yml格式的文件", ymlFile.FileName);
                }
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var modelFolder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);
                var backupFolder = Path.Combine(WebConfigUtils.RootFolder, "Backup");
                backupFolder = Path.Combine(backupFolder, project.Model.ModelFolder);
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                string oldPath = Path.Combine(modelFolder, "Script.yml");
                string newPath = Path.Combine(backupFolder, DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff") + "_Script.yml");


                //备份原来 script.yml
                Directory.Move(oldPath, newPath);

                LogEditProduct(EditProductType.UpdateScriptYml, project.ProjectId, "修改script.yml文件Project=[" + project.Name + "];ProjectId=[" + project.ProjectId + "];", "");


                if (!Directory.Exists(modelFolder))
                {
                    Directory.CreateDirectory(modelFolder);
                }

                if (ymlFile != null)
                {
                    var ymlFilePath = modelFolder + @"\Script.yml";
                    ymlFile.SaveAs(ymlFilePath);
                }

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult DownloadYml(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var folder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);
                string filePath = Path.Combine(folder, "Script.yml");

                if (System.IO.File.Exists(filePath))
                {
                    var fileFullName = project.Name + "script.yml";
                    LogEditProduct(EditProductType.UpdateScriptYml, project.ProjectId, "下载script.yml文件Project=[" + project.Name + "];ProjectId=[" + project.ProjectId + "];FileName=[" + fileFullName + "]", "");


                    var resource = ResourcePool.RegisterFilePath(CurrentUserName, project.Name + "_Script.yml", filePath);
                    return ActionUtils.Success(resource.Guid);
                }
                else
                {
                    return ActionUtils.Success("");
                }
            });
        }


        [HttpPost]
        public ActionResult RemoveProject(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                CommUtils.AssertHasContent(project.CreateUserName, "找不到产品[{0}]的创建者", project.Name);
                CommUtils.Assert(IsCurrentUser(project.CreateUserName), "当前用户[{0}]不是产品[{1}]的创建者[{2}]",
                    User.Identity.Name, project.Name, project.CreateUserName);

                m_dbAdapter.Project.Remove(project);
                var logMsg = "删除产品[" + project.Name + "][" + project.ProjectGuid + "]";
                LogEditProduct(EditProductType.DeleteProduct, null, logMsg, "");

                return ActionUtils.Success(projectGuid);
            });
        }

        #region Edit task
        [HttpGet]
        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyTask)]
        public ActionResult EditTask(string projectGuid)
        {
            Project project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var viewModel = new EditTaskViewModel();
            viewModel.HasCreateProjectAuthority = m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject);
            viewModel.Project = Toolkit.ConvertProject(project);
            viewModel.Tasks = new List<TaskViewModel>();
            var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId, true);

            //short code， name
            var taskShortCodeDict = new Dictionary<int, Tuple<string, string>>();
            foreach (var task in tasks)
            {
                TaskViewModel taskView = Toolkit.ConvertTask(task);

                taskShortCodeDict[int.Parse(taskView.Id)] = Tuple.Create(taskView.ShortCode, taskView.TaskName);

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

                viewModel.Tasks.Add(taskView);
            }

            return View(viewModel);
        }
        #endregion

        [HttpPost]
        public ActionResult GetAllCnabsDeals()
        {
            return ActionUtils.Json(() =>
            {
                var cnabsDeals = m_dbAdapter.Model.GetAllDeals();
                var objs = cnabsDeals.ConvertAll(x => new
                {
                    nameCN = x.Item2,
                    nameEN = lib.Parsers.CPinyinConverter.Get(x.Item2).ToLower(),
                    nameJC = lib.Parsers.CPinyinConverter.GetFirst(x.Item2).ToLower(),
                    id = x.Item1
                });

                return ActionUtils.Success(objs);
            });
        }

        private Dictionary<string, List<DateTime>> GetAllTemplateTime(int templateId)
        {
            var loopTimeDict = new Dictionary<string, List<DateTime>>();
            var templateTimes = m_dbAdapter.Template.GetTemplateTimeLists(templateId);

            foreach (var templateTime in templateTimes)
            {
                var dateList = DateUtils.GenerateDateList(
                    templateTime.BeginTime,
                    templateTime.EndTime,
                    templateTime.TimeSpan,
                    templateTime.TimeSpanUnit,
                    templateTime.TemplateTimeType,
                    templateTime.SearchDirection == TemplateTimeSearchDirection.Forward,
                    templateTime.HandleReduplicate == TemplateTimeHandleReduplicate.Ignore
                );
                loopTimeDict[templateTime.TemplateTimeName] = dateList;
            }

            return loopTimeDict;
        }

        private List<Tuple<DateTime?, DateTime?>> GetGenerateTasks(TemplateTask templateTask, Dictionary<string, List<DateTime>> loopTimeDictionary)
        {
            List<Tuple<DateTime?, DateTime?>> generateTimeList = new List<Tuple<DateTime?, DateTime?>>();
            DateTime temp;
            bool isDateContainsLetter = !DateTime.TryParse(templateTask.TriggerDate, out temp);

            if (isDateContainsLetter)
            {
                var key = DateUtils.ParseDateSyntaxKey(templateTask.TriggerDate);
                CommUtils.Assert(loopTimeDictionary.ContainsKey(key), "找不到对应的时间Key[" + key + "]");
                var timeList = loopTimeDictionary[key];

                foreach (var item in timeList)
                {
                    var time = new Dictionary<string, DateTime> { { key, item } };
                    DateTime? endTime = DateUtils.ParseDateSyntax(templateTask.TriggerDate, time);
                    DateTime? startTime = null;
                    if (!string.IsNullOrEmpty(templateTask.BeginDate))
                    {
                        startTime = DateUtils.ParseDateSyntax(templateTask.BeginDate, time);
                    }
                    generateTimeList.Add(Tuple.Create(startTime, endTime));
                }
            }
            else
            {
                DateTime? endTime = temp;
                DateTime? startTime = null;
                DateTime time;
                if (DateTime.TryParse(templateTask.BeginDate, out time))
                {
                    startTime = time;
                }
                generateTimeList.Add(Tuple.Create(startTime, endTime));
            }

            return generateTimeList;
        }

        private List<TemplateTask> GetSibbingTemplateTasks(int templateTaskId)
        {
            var templateTask = m_dbAdapter.Template.GetTemplateTask(templateTaskId);
            return m_dbAdapter.Template.GetTemplateTasks(templateTask.TemplateId.Value);
        }

        private List<CheckTaskTime> CheckTaskTimeByYear(string projectGuid, int year)
        {
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId);
            tasks.RemoveAll(x => !x.TemplateTaskId.HasValue);
            var allTasksDict = tasks.GroupBy(x => x.TemplateTaskId.Value).ToDictionary(x => x.Key, y => y.ToList());

            var cacheTemplateTask = CacheUtils.Build<int, TemplateTask>(GetSibbingTemplateTasks, x => x.TemplateTaskId);
            var cacheTemplateTime = CacheUtils.Build<int, Dictionary<string, List<DateTime>>>(GetAllTemplateTime);

            var errorTaskTimeList = new List<CheckTaskTime>();

            while (tasks.Count > 0)
            {
                var task = tasks.First();
                var currentTemplateTask = cacheTemplateTask[task.TemplateTaskId.Value];
                var loopTimeDictionary = cacheTemplateTime[currentTemplateTask.TemplateId.Value];

                var generateTaskList = GetGenerateTasks(currentTemplateTask, loopTimeDictionary);
                var analogousTasks = allTasksDict[task.TemplateTaskId.Value];

                //通过工作模板生成的工作数量与现有的同类工作数量检查
                if (generateTaskList.Count != analogousTasks.Count)
                {
                    errorTaskTimeList.AddRange(analogousTasks.ConvertAll(x => new CheckTaskTime
                    {
                        ErrorTask = x,
                        ErrorType = ErrorType.TaskRepeat.ToString()
                    }));

                    tasks = tasks.Except(analogousTasks).ToList();
                }
                else
                {
                    //判断原有的任务与生成的任务开始、截止时间是否一致
                    for (int i = 0; i < analogousTasks.Count; i++)
                    {
                        var generateTask = generateTaskList[i];
                        if (year == 0 ||
                            (generateTask.Item2.Value.Year == year
                            || generateTask.Item1.HasValue
                            && generateTask.Item1.Value.Year == year))
                        {
                            var isStartTimeRight = analogousTasks[i].StartTime == generateTask.Item1;
                            var isEndTimeRight = analogousTasks[i].EndTime == generateTask.Item2;
                            if (!isStartTimeRight || !isEndTimeRight)
                            {
                                var checkTaskTime = new CheckTaskTime();
                                checkTaskTime.ErrorTask = analogousTasks[i];
                                if (!isStartTimeRight && isEndTimeRight)
                                {
                                    checkTaskTime.ErrorType = ErrorType.StartTimeError.ToString();
                                }
                                if (isStartTimeRight && !isEndTimeRight)
                                {
                                    checkTaskTime.ErrorType = ErrorType.EndTimeError.ToString();
                                }
                                if (!isStartTimeRight && !isEndTimeRight)
                                {
                                    checkTaskTime.ErrorType = ErrorType.StartEndTimeError.ToString();
                                }

                                checkTaskTime.StartTime = generateTask.Item1;
                                checkTaskTime.EndTime = generateTask.Item2;

                                errorTaskTimeList.Add(checkTaskTime);
                            }
                        }

                        tasks.Remove(analogousTasks[i]);
                    }
                }
            }

            errorTaskTimeList = errorTaskTimeList.OrderBy(x => x.ErrorTask.EndTime).ToList();
            return errorTaskTimeList;
        }

        [HttpPost]
        public ActionResult CheckAnnualTime(string projectGuid, int year)
        {
            return ActionUtils.Json(() =>
            {
                var errorTaskTimeList = CheckTaskTimeByYear(projectGuid, year);
                return errorTaskTimeList.Count == 0 ? ActionUtils.Success("") : ActionUtils.Success(errorTaskTimeList);
            });
        }

        [HttpPost]
        public ActionResult SetTaskPaymentPeriod(string shortCodeToString, string paymentDateStr)
        {
            return ActionUtils.Json(() =>
            {
                DateTime paymentDate;
                CommUtils.Assert(DateTime.TryParse(paymentDateStr, out paymentDate), "偿付期错误，请刷新页面后重试");

                var tasks = LoadTasks(shortCodeToString);
                var taskPeriodDB = m_dbAdapter.TaskPeriod.GetTaskPeriodsByProjectId(tasks.First().ProjectId);
                var dictTaskPeriodDB = taskPeriodDB.ToDictionary(x => x.ShortCode);

                var keys = dictTaskPeriodDB.Keys.ToList();
                foreach (var task in tasks)
                {
                    var taskPeriod = new TaskPeriod();

                    if (keys.Contains(task.ShortCode))
                    {
                        taskPeriod = dictTaskPeriodDB[task.ShortCode];
                        taskPeriod.PaymentDate = paymentDate;

                        m_dbAdapter.TaskPeriod.Update(taskPeriod);
                    }
                    else
                    {
                        taskPeriod.ProjectId = task.ProjectId;
                        taskPeriod.ShortCode = task.ShortCode;
                        taskPeriod.PaymentDate = paymentDate;

                        m_dbAdapter.TaskPeriod.New(taskPeriod);
                    }
                }

                return ActionUtils.Success(tasks.Count);
            });
        }

        #region Modify Task Begin/End Time

        [HttpPost]
        public ActionResult CorrectAllTaskTime(string projectGuid, int year)
        {
            return ActionUtils.Json(() =>
            {
                var correctTaskList = new List<Task>();
                var errorTaskTimeList = CheckTaskTimeByYear(projectGuid, year);
                foreach (var errorTask in errorTaskTimeList)
                {
                    if (errorTask.ErrorType != ErrorType.TaskRepeat.ToString())
                    {
                        errorTask.ErrorTask.StartTime = errorTask.StartTime;
                        errorTask.ErrorTask.EndTime = errorTask.EndTime;
                        correctTaskList.Add(errorTask.ErrorTask);
                    }
                }

                if (correctTaskList.Count != 0)
                {
                    CheckTaskDate(correctTaskList);
                    var newVals = correctTaskList.ConvertAll(x => x.StartTime.HasValue ? x.StartTime.Value.ToString("yyyy-MM-dd") + "and" + x.EndTime.Value.ToString("yyyy-MM-dd") : "-" + "and" + x.EndTime.Value.ToString("yyyy-MM-dd"));
                    SaveTasks(correctTaskList, "开始、截止时间", newVals);
                }

                return ActionUtils.Success(correctTaskList.Count);
            });
        }

        [HttpPost]
        public ActionResult CorrectTaskTime(string shortCode, string startTime, string endTime)
        {
            return ActionUtils.Json(() =>
            {
                var task = LoadTasks(shortCode).Single();
                task.StartTime = startTime == "-" ? null : new DateTime?(DateTime.Parse(startTime));
                task.EndTime = DateTime.Parse(endTime);
                CheckTaskDate(new List<Task> { task });
                SaveTask(task, "校正开始、截止时间", startTime + endTime);
                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskBeginTimeForAppointed(string shortCode, string appointedDay)
        {
            return ActionUtils.Json(() =>
            {
                var startTime = ParseDate(appointedDay);
                var tasks = LoadTasks(shortCode);
                tasks.ForEach(x => x.StartTime = startTime);
                CheckTaskDate(tasks, false);
                SaveTasks(tasks, "开始时间", startTime.ToString());
                return ActionUtils.Success(tasks.Count());
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskEndTimeForAppointed(string shortCode, string appointedDay)
        {
            return ActionUtils.Json(() =>
            {
                var endTime = ParseDate(appointedDay);
                var tasks = LoadTasks(shortCode);
                tasks.ForEach(x => x.EndTime = endTime);
                CheckTaskDate(tasks);
                SaveTasks(tasks, "截止时间", endTime.ToString());
                return ActionUtils.Success(tasks.Count());
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskEndTimeForCondition(string shortCode, int intervalTime, string timeType)
        {
            return ActionUtils.Json(() =>
            {
                var templateTimeType = CommUtils.ParseEnum<TemplateTimeType>(timeType);
                var tasks = LoadTasks(shortCode);
                tasks.ForEach(x => x.EndTime = CalcDateTime(x.EndTime.Value, intervalTime, templateTimeType));
                CheckTaskDate(tasks);
                SaveTasks(tasks, "截止时间", tasks.Select(x => x.EndTime.Value.ToString()));
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskBeginTimeForCondition(string shortCode, int intervalTime, string timeType)
        {
            return ActionUtils.Json(() =>
            {
                var templateTimeType = CommUtils.ParseEnum<TemplateTimeType>(timeType);
                var tasks = LoadTasks(shortCode);
                tasks.ForEach(x => x.StartTime = CalcDateTime(x.StartTime, intervalTime, templateTimeType));
                CheckTaskDate(tasks, false);
                SaveTasks(tasks, "开始时间", tasks.Select(x => Toolkit.DateTimeToString(x.StartTime)));
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskBeginTimeByStandard(string shortCode, string standardTaskName, int intervalTime, string timeType)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode);
                CommUtils.AssertNotNull(standardTaskName, "基准工作名称不能为空。");
                var templateTimeType = CommUtils.ParseEnum<TemplateTimeType>(timeType);
                var allTasks = m_dbAdapter.Task.GetTasksByProjectId(tasks.First().ProjectId);
                var standardTasks = allTasks.Where(x => x.Description == standardTaskName).ToList();
                CommUtils.AssertEquals(tasks.Count, standardTasks.Count,
                    "选中的工作[{0}]的数量[{1}]与基准工作[{2}]的数量[{3}]不匹配，无法进行批量修改",
                    tasks.First().Description, tasks.Count, standardTaskName, standardTasks.Count);

                AssertAllTasks(tasks, x => !standardTasks.Any(y => y.TaskId == x.TaskId), "选中的工作与基准工作[{0}]相同");

                for (int i = 0; i < tasks.Count; ++i)
                {
                    tasks[i].StartTime = CalcDateTime(standardTasks[i].StartTime.Value, intervalTime, templateTimeType);
                }

                CheckTaskDate(tasks, false, allTasks);
                SaveTasks(tasks, "开始时间", tasks.Select(x => x.EndTime.Value.ToString()));
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskEndTimeByStandard(string shortCode, string standardTaskName, int intervalTime, string timeType)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode);
                CommUtils.AssertNotNull(standardTaskName, "基准工作名称不能为空。");
                var templateTimeType = CommUtils.ParseEnum<TemplateTimeType>(timeType);
                var allTasks = m_dbAdapter.Task.GetTasksByProjectId(tasks.First().ProjectId);
                var standardTasks = allTasks.Where(x => x.Description == standardTaskName).ToList();
                CommUtils.AssertEquals(tasks.Count, standardTasks.Count,
                    "选中的工作[{0}]的数量[{1}]与基准工作[{2}]的数量[{3}]不匹配，无法进行批量修改",
                    tasks.First().Description, tasks.Count, standardTaskName, standardTasks.Count);

                AssertAllTasks(tasks, x => !standardTasks.Any(y => x.TaskId == y.TaskId), "选中的工作与基准工作[{0}]相同");

                for (int i = 0; i < tasks.Count; ++i)
                {
                    tasks[i].EndTime = CalcDateTime(standardTasks[i].EndTime.Value, intervalTime, templateTimeType);
                }

                CheckTaskDate(tasks, true, allTasks);
                SaveTasks(tasks, "截止时间", tasks.Select(x => x.EndTime.Value.ToString()));
                return ActionUtils.Success(tasks.Count);
            });
        }
        #endregion

        #region Time modifier helper
        private DateTime? CalcDateTime(DateTime? time, int interval, TemplateTimeType timeType)
        {
            if (!time.HasValue)
            {
                return null;
            }

            switch (timeType)
            {
                case TemplateTimeType.NaturalDay:
                    return time.Value.AddDays(interval);
                case TemplateTimeType.TradingDay:
                    return DateUtils.AddTradingDay(time.Value, interval);
                case TemplateTimeType.WorkingDay:
                    return DateUtils.AddWorkingDay(time.Value, interval);
                default:
                    return time.Value;
            }
        }

        private DateTime ParseDate(string dateText)
        {
            DateTime date;
            CommUtils.Assert(!string.IsNullOrEmpty(dateText), "时间内容不能为空");
            CommUtils.Assert(DateTime.TryParse(dateText, out date), "时间[" + dateText + "]格式不正确");
            return date;
        }

        private void CheckTaskDate(List<Task> tasks, bool checkEndTime = true, List<Task> allTasks = null)
        {
            if (allTasks == null && tasks.Count != 0)
            {
                allTasks = m_dbAdapter.Task.GetTasksByProjectId(tasks.First().ProjectId);
            }

            var allTaskDict = allTasks.ToDictionary(x => x.TaskId);
            tasks.ForEach(x => allTaskDict[x.TaskId] = x);

            foreach (var task in tasks)
            {
                if (checkEndTime)
                {
                    foreach (var prevTaskId in task.PrevTaskIdArray)
                    {
                        var prevTask = allTaskDict[prevTaskId];
                        CommUtils.Assert(task.EndTime.Value >= prevTask.EndTime.Value,
                            "工作[{0}]的截止时间[{1}]不能在其前置工作[{2}]的截止时间[{3}]之前",
                            task.Description + "(" + task.ShortCode + ")",
                            task.EndTime.Value.ToShortDateString(),
                            prevTask.Description + "(" + prevTask.ShortCode + ")",
                            prevTask.EndTime.Value.ToShortDateString());
                    }

                    var postTasks = allTasks.Where(x => x.PrevTaskIdArray.Contains(task.TaskId));
                    foreach (var postTask in postTasks)
                    {
                        CommUtils.Assert(postTask.EndTime.Value >= task.EndTime.Value,
                            "工作[{0}]的截止时间[{1}]不能在其后续工作[{2}]的截止时间[{3}]之后",
                            task.Description + "(" + task.ShortCode + ")",
                            task.EndTime.Value.ToShortDateString(),
                            postTask.Description + "(" + postTask.ShortCode + ")",
                            postTask.EndTime.Value.ToShortDateString());
                    }
                }

                if (task.StartTime.HasValue)
                {
                    CommUtils.Assert(task.EndTime.Value >= task.StartTime.Value,
                        "工作[{0}]的截止时间不能小于开始时间", task.Description + "(" + task.ShortCode + ")");
                }
            }
        }
        #endregion

        #region Task modifier helper
        private void CheckAuthority(List<Task> tasks)
        {
            var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);
            CommUtils.AssertEquals(tasks.Select(x => x.ProjectId).Distinct().Count(), 1, "传入工作属于多个产品");
            CommUtils.Assert(projectIds.Contains(tasks.First().ProjectId), "没有修改权限");
        }

        private void CheckAuthority(Task task)
        {
            var projectIds = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);
            CommUtils.Assert(projectIds.Contains(task.ProjectId), "没有修改权限");
        }

        private List<Task> LoadTasks(string shortCodeText, bool withExInfo = false, List<Task> allTasks = null)
        {
            var shortCodes = CommUtils.Split(shortCodeText);
            List<Task> tasks;
            if (allTasks == null)
            {   //从数据库中加载
                tasks = m_dbAdapter.Task.GetTasks(shortCodes, withExInfo);
            }
            else
            {   //从已加载的Task中寻找
                tasks = allTasks.Where(x => shortCodes.Contains(x.ShortCode)).ToList();
            }
            CheckAuthority(tasks);
            return tasks;
        }

        private void SaveTasks(List<Task> tasks, string fieldName, IEnumerable<string> newValue)
        {
            var newValueList = newValue.ToList();
            CommUtils.AssertEquals(tasks.Count, newValueList.Count, "传入值和待修改工作数量不相等");
            for (int i = 0; i < tasks.Count; ++i)
            {
                SaveTask(tasks[i], fieldName, newValueList[i]);
            }
        }

        private void SaveTasks(List<Task> tasks, string fieldName, string newValue)
        {
            tasks.ForEach(x => SaveTask(x, fieldName, newValue));
        }

        private void SaveTask(Task task, string fieldName, string newValue)
        {
            var result = m_dbAdapter.Task.UpdateTask(task);
            CommUtils.AssertEquals(result, 1, "工作[" + task.ShortCode + "]修改失败");
            LogEditProduct(EditProductType.EditTask, task.ProjectId,
                "更新Task[" + task.ShortCode + "]，修改" + fieldName + "[" + newValue.ToString() + "]", "");
        }

        private void SaveTaskExs(List<Task> tasks, string operation)
        {
            foreach (var task in tasks)
            {
                m_dbAdapter.Task.UpdateTaskExtension(task.TaskExtension);
                LogEditProduct(EditProductType.EditTask, task.ProjectId,
                    "更新Task[" + task.Description + "(" + task.ShortCode + ")]，" + operation, "");
            }
        }

        private void AssertAllTasks(List<Task> tasks, Func<Task, bool> predicate, string errorMsg)
        {
            tasks.ForEach(x => CommUtils.Assert(predicate(x), errorMsg, x.Description + "(" + x.ShortCode + ")"));
        }

        //检查无限循环依赖
        private void CheckEndlessLoopDependency(IEnumerable<Task> tasks)
        {
            List<int> sortedIds = new List<int>();
            var allIds = tasks.Select(x => x.TaskId).ToList();
            while (sortedIds.Count < tasks.Count())
            {
                var unsortedIds = allIds.Except(sortedIds).ToList();
                var unsortedTasks = tasks.Where(x => unsortedIds.Contains(x.TaskId)).ToList();
                var topTasks = unsortedTasks.Where(x => x.PrevTaskIdArray.Intersect(unsortedIds).Count() == 0);
                var topIds = topTasks.Select(x => x.TaskId);
                CommUtils.Assert(topIds.Count() != 0, "前置工作出现循环！");
                sortedIds.AddRange(topIds);
            }
        }
        #endregion

        #region Modify Task Deatil/Target
        [HttpPost]
        public ActionResult GetTaskDetail(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode);
                var details = tasks.Select(x => x.TaskDetail).Distinct();
                var detail = (details.Count() == 1 ? details.First() : string.Empty);
                return ActionUtils.Success(detail);
            });
        }

        [HttpPost]
        public ActionResult GetTaskTarget(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode);
                var targets = tasks.Select(x => x.TaskTarget).Distinct();
                var target = (targets.Count() == 1 ? targets.First() : string.Empty);
                return ActionUtils.Success(target);
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskDetail(string shortCode, string modifyText)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(modifyText.Length <= 100000, "工作描述过长");
                var tasks = LoadTasks(shortCode);
                tasks.ForEach(x => x.TaskDetail = modifyText);
                SaveTasks(tasks, "工作描述", modifyText);
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskTarget(string shortCode, string modifyText)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(modifyText.Length <= 100000, "工作目标过长");
                var tasks = LoadTasks(shortCode);
                tasks.ForEach(x => x.TaskTarget = modifyText);
                SaveTasks(tasks, "工作目标", modifyText);
                return ActionUtils.Success(tasks.Count);
            });
        }
        #endregion

        #region Get all tasks
        [HttpPost]
        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyTask)]
        public ActionResult GetAllTask(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId, true);
                var idShortCodes = tasks.ToDictionary(x => x.TaskId, y => y.ShortCode);
                tasks.ForEach(x => x.PrevTaskShortCodeArray = x.PrevTaskIdArray.ConvertAll(id => idShortCodes[id]));

                var result = tasks.ConvertAll(x => new
                {
                    ShortCode = x.ShortCode,
                    Description = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    PrevTaskShortCodeArray = x.PrevTaskShortCodeArray,
                    PrevTaskIdArray = x.PrevTaskIdArray,
                    TaskExtension = x.TaskExtension,
                });
                return ActionUtils.Success(result);
            });
        }
        #endregion

        #region Modify task extension document
        [HttpPost]
        public ActionResult DeleteExtensionDocument(string shortCode, string documentName)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode, true);
                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.Document,
                    "工作[{0}]的工作扩展类型不是Document");

                foreach (var task in tasks)
                {
                    var taskExInfo = task.TaskExtension.TaskExtensionInfo;
                    if (taskExInfo != null)
                    {
                        var documents = CommUtils.FromJson<List<TaskExDocumentItem>>(taskExInfo);
                        CommUtils.AssertEquals(documents.Count(x => x.Name == documentName), 1, "删除失败，工作[" + task + "]的文档[" + documentName + "]有误，请刷新后重试");

                        documents.RemoveAll(x => x.Name == documentName);

                        task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(documents);
                    }
                }

                SaveTaskExs(tasks, "删除扩展任务文档[" + documentName + "]");
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult RemoveAllTaskExtensionDocuments(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode, true);
                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                var taskExDocs = new List<TaskExDocumentItem>();
                var emptyJson = CommUtils.ToJson(taskExDocs);
                tasks.ForEach(x => x.TaskExtension.TaskExtensionInfo = emptyJson);
                SaveTaskExs(tasks, "删除扩展工作全部文档");
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult AddTaskExtensionDocument(string shortCode, string documentName,
            string fileType, string documentType, bool autoGenerate, string patternType, string namingRule, bool autoConfig = false)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode, true);
                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.Document,
                    "工作[{0}]的工作扩展类型不是Document");

                var projectId = tasks.First().ProjectId;
                var project = m_dbAdapter.Project.GetProjectById(projectId);
                if (!autoConfig)
                {
                    CheckDocumentNameRuleByType(project, documentName, patternType);
                }

                foreach (var task in tasks)
                {
                    CommUtils.Assert(projectId == task.ProjectId, "当前工作[{0}]的ProjectId[{1}]与ProjectId[{2}]不属于同一产品",
                        task.Description, task.ProjectId, projectId);

                    List<TaskExDocumentItem> documents = new List<TaskExDocumentItem>();

                    if (!string.IsNullOrEmpty(task.TaskExtension.TaskExtensionInfo))
                    {
                        documents = CommUtils.FromJson<List<TaskExDocumentItem>>(task.TaskExtension.TaskExtensionInfo);
                    }

                    if (!autoConfig)
                    {
                        CommUtils.Assert(!documents.Any(x => x.Name == documentName), "工作[{0}({1})]中已经存在文档[{2}]",
                        task.Description, task.ShortCode, documentName);
                        var document = AddTaskExDocument(documentName, fileType, documentType, patternType, autoGenerate, namingRule);
                        documents.Add(document);
                    }
                    else
                    {
                        CommUtils.Assert(!documentName.Contains("-"), "自动配置文档时，文件名称[{0}]中不能包含：-", documentName);
                        if (patternType == TaskExPatternType.IncomeDistributionReport.ToString())
                        {
                            var name = documentName + "收益分配报告";
                            var document = AddTaskExDocument(name, fileType, documentType, patternType, autoGenerate, namingRule, autoConfig);
                            documents.Add(document);
                        }
                        else if (patternType == TaskExPatternType.CashInterestRateConfirmForm.ToString())
                        {
                            var cnabsNotes = new ProjectLogicModel(CurrentUserName, project).Notes;
                            foreach (var cnabsNote in cnabsNotes)
                            {
                                var name = documentName + "兑付兑息确认表-" + cnabsNote.NoteName + "（" + cnabsNote.SecurityCode + "）";
                                CommUtils.Assert(!documents.Any(x => x.Name == name), "工作[{0}({1})]中已经存在以[{2}]为开头的文档，无法进行自动配置",
                                    task.Description, task.ShortCode, documentName);
                                var document = AddTaskExDocument(name, fileType, documentType, patternType, autoGenerate, namingRule, autoConfig);
                                documents.Add(document);
                            }
                        }
                        else if (patternType == TaskExPatternType.SpecialPlanTransferInstruction.ToString())
                        {
                            var cnabsNotes = new ProjectLogicModel(CurrentUserName, project).Notes;
                            foreach (var cnabsNote in cnabsNotes)
                            {
                                var name = documentName + "划款指令-" + cnabsNote.NoteName + "（" + cnabsNote.SecurityCode + "）";
                                CommUtils.Assert(!documents.Any(x => x.Name == name), "工作[{0}({1})]中已经存在以[{2}]为开头的文档，无法进行自动配置",
                                    task.Description, task.ShortCode, documentName);
                                var document = AddTaskExDocument(name, fileType, documentType, patternType, autoGenerate, namingRule, autoConfig);
                                documents.Add(document);
                            }
                            var trusteeFeeDocument = AddTaskExDocument(documentName + "划款指令（托管费）", fileType, documentType, patternType, autoGenerate, namingRule, autoConfig);
                            var assetOrganDocument = AddTaskExDocument(documentName + "划款指令（资产服务机构费用）", fileType, documentType, patternType, autoGenerate, namingRule, autoConfig);
                            documents.Add(trusteeFeeDocument);
                            documents.Add(assetOrganDocument);
                        }
                        else if (patternType == TaskExPatternType.InterestPaymentPlanApplication.ToString())
                        {
                            var cnabsNotes = new ProjectLogicModel(CurrentUserName, project).Notes;
                            foreach (var cnabsNote in cnabsNotes)
                            {
                                var noteName = cnabsNote.NoteName;
                                if (noteName.LastIndexOf("级") == (noteName.Length - 1))
                                {
                                    noteName = noteName.Substring(0, noteName.Length - 1);
                                }
                                var name = documentName + "付息方案-" + noteName + "级-" + cnabsNote.SecurityCode;
                                CommUtils.Assert(!documents.Any(x => x.Name == name), "工作[{0}({1})]中已经存在以[{2}]为开头的文档，无法进行自动配置",
                                    task.Description, task.ShortCode, documentName);
                                var document = AddTaskExDocument(name, fileType, documentType, patternType, autoGenerate, namingRule, autoConfig);
                                documents.Add(document);
                            }
                        }
                    }

                    task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(documents);
                }

                SaveTaskExs(tasks, "增加扩展工作文档[" + documentName + "]");
                return ActionUtils.Success(tasks.Count);
            });
        }

        private void CheckDocumentNameRuleByType(Project project, string documentName, string patternType)
        {
            var type = CommUtils.ParseEnum<TaskExPatternType>(patternType);
            var cnabsNotes = new ProjectLogicModel(CurrentUserName, project).Notes;

            Func<Note, bool> containsKeyword = (x) =>
                documentName.Contains("(" + x.SecurityCode + ")")
                || documentName.Contains("（" + x.SecurityCode + "）")
                || documentName.Contains("(" + x.ShortName + ")")
                || documentName.Contains("（" + x.ShortName + "）");

            if (type == TaskExPatternType.SpecialPlanTransferInstruction)
            {
                CommUtils.Assert(cnabsNotes.Any(containsKeyword)
                    || documentName.Contains("(托管费)") || documentName.Contains("（托管费）")
                    || documentName.Contains("(资产服务机构费用)") || documentName.Contains("（资产服务机构费用）"),
                    "文档名称中必须包含证券代码或证券简称。"
                    + "<br/>例如：东海租赁一期资产支持专项计划划款指令（142815）");
            }
            else if (type == TaskExPatternType.CashInterestRateConfirmForm)
            {
                CommUtils.Assert(cnabsNotes.Any(containsKeyword), "文档名称中必须包含证券代码或证券简称。"
                    + "<br/>例如：东海租赁一期资产支持专项计划兑付兑息确认表（142815）");
            }
            else if (type == TaskExPatternType.InterestPaymentPlanApplication)
            {
                if (documentName.StartsWith("付息方案"))
                {
                    CommUtils.Assert(documentName.Contains("级"), "文档名称格式错误"
                        + "<br/>例如：付息方案A1级-142815-03.04");
                }
                else
                {
                    CommUtils.Assert(documentName.Contains("付息方案"), "文档名必须包含[付息方案]。"
                        + "<br/>例如：东海租赁一期付息方案-A1级-142815-03.04");

                    var firstIndexOfUnderline = documentName.IndexOf("-");
                    CommUtils.Assert(firstIndexOfUnderline != -1, "文档名称格式错误"
                        + "<br/>例如：东海租赁一期付息方案-A1级-142815-03.04");

                    var text = documentName.Substring(firstIndexOfUnderline + 1);
                    var secondIndexOfUnderline = text.IndexOf("-");
                    CommUtils.Assert(secondIndexOfUnderline != -1, "文档名称格式错误"
                        + "<br/>例如：东海租赁一期付息方案-A1级-142815-03.04");
                }

                CommUtils.Assert(cnabsNotes.Any(x => documentName.Contains(x.ShortName)),
                        "文档名称格式错误，找不到证券简称。"
                        + "<br/>例如：付息方案A1级-142815-03.04");

                CommUtils.Assert(cnabsNotes.Any(x => documentName.Contains("-" + x.SecurityCode)),
                        "文档名称格式错误，找不到证券代码。"
                        + "<br/>例如：付息方案A1级-142815-03.04");
            }
        }

        private TaskExDocumentItem AddTaskExDocument(string documentName, string fileType, string documentType,
            string patternType, bool autoGenerate, string namingRule, bool autoConfig = false)
        {
            var document = new TaskExDocumentItem();
            document.Guid = Guid.NewGuid().ToString();
            document.Name = documentName;
            document.FileType = CommUtils.ParseEnum<FileType>(fileType);
            document.DocumentType = CommUtils.ParseEnum<TaskExDocumentType>(documentType);
            document.AutoGenerate = autoGenerate;
            document.UpdateTime = null;
            document.PatternType = CommUtils.ParseEnum<TaskExPatternType>(patternType);
            if (autoGenerate)
            {
                CommUtils.Assert(document.PatternType != TaskExPatternType.None, "勾选自动生成后，必须选择一种文档模板");
            }
            document.NamingRule = CommUtils.ParseEnum<TaskExDocNamingRule>(namingRule);
            document.AutoConfig = autoConfig;

            return document;
        }

        [HttpPost]
        public ActionResult ModifyExtensionDocument(string shortCode, string documentName, string fileType,
            string documentType, bool autoGenerate, string patternType, string namingRule, string oldDocumentName, bool isAutoConfig, string prefixName)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode, true);
                var project = m_dbAdapter.Project.GetProjectById(tasks.First().ProjectId);
                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.Document,
                    "工作[{0}]的工作扩展类型不是Document");

                if (tasks.Count > 0)
                {
                    if (isAutoConfig)
                    {
                        CommUtils.Assert(!prefixName.Contains("-"), "文件名称[{0}]中不能包含：-", prefixName);
                    }
                    else
                    {
                        CheckDocumentNameRuleByType(project, documentName, patternType);
                    }
                }

                foreach (var task in tasks)
                {
                    List<TaskExDocumentItem> documents = new List<TaskExDocumentItem>();
                    if (!string.IsNullOrEmpty(task.TaskExtension.TaskExtensionInfo))
                    {
                        documents = CommUtils.FromJson<List<TaskExDocumentItem>>(task.TaskExtension.TaskExtensionInfo);
                    }

                    CommUtils.Assert(documents.Count(x => x.Name == documentName) <= 1, "工作[{0}({1})]中已经存在文档[{2}]",
                        task.Description, task.ShortCode, documentName);

                    foreach (var document in documents)
                    {
                        if (document.Name == oldDocumentName)
                        {
                            var patternTypeEnum = CommUtils.ParseEnum<TaskExPatternType>(patternType);
                            if (document.PatternType != patternTypeEnum)
                            {
                                CheckDocumentNameRuleByType(project, documentName, patternType);
                            }
                            document.Name = documentName;
                            document.FileType = CommUtils.ParseEnum<FileType>(fileType);
                            document.DocumentType = CommUtils.ParseEnum<TaskExDocumentType>(documentType);
                            document.AutoGenerate = autoGenerate;
                            document.PatternType = patternTypeEnum;
                            if (autoGenerate)
                            {
                                CommUtils.Assert(document.PatternType != TaskExPatternType.None, "勾选自动生成后，必须选择一种文档模板");
                            }
                            document.NamingRule = CommUtils.ParseEnum<TaskExDocNamingRule>(namingRule);

                            task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(documents);
                            break;
                        }
                    }
                }

                SaveTaskExs(tasks, "修改扩展工作文档[" + documentName + "]");
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult GetTaskDocumentList(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode, true);
                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.Document,
                    "工作[{0}]的工作扩展类型不是Document");

                var dictTaskDocs = tasks.ToDictionary<Task, string, List<TaskExDocumentItem>>(x => x.ShortCode, val =>
                {
                    var taskExInfo = val.TaskExtension.TaskExtensionInfo;
                    if (string.IsNullOrEmpty(taskExInfo))
                    {
                        return new List<TaskExDocumentItem>();
                    }
                    else
                    {
                        var taskExDocs = CommUtils.FromJson<List<TaskExDocumentItem>>(taskExInfo);
                        taskExDocs.Sort((l, r) => l.Name.CompareTo(r.Name));
                        return taskExDocs;
                    }
                });

                var jsonResult = new JsonResult();
                var firstTaskDocs = dictTaskDocs.First().Value;
                foreach (var key in dictTaskDocs.Keys)
                {
                    var curDocs = dictTaskDocs[key];
                    if (curDocs.Count != firstTaskDocs.Count)
                    {
                        return ActionUtils.Success(jsonResult);
                    }

                    for (int i = 0; i < firstTaskDocs.Count; ++i)
                    {
                        if (!curDocs[i].EqualsDoc(firstTaskDocs[i]))
                        {
                            return ActionUtils.Success(jsonResult);
                        }
                    }
                }
                jsonResult.Data = firstTaskDocs.ConvertAll(x =>
                {
                    return new
                    {
                        AutoGenerate = x.AutoGenerate,
                        DocumentType = x.DocumentType.ToString(),
                        FileType = x.FileType.ToString(),
                        Guid = x.Guid,
                        Name = x.Name,
                        NamingRule = x.NamingRule.ToString(),
                        PatternType = x.PatternType.ToString(),
                        AutoConfig = x.AutoConfig
                    };
                });
                return ActionUtils.Success(jsonResult);
            });
        }
        #endregion

        #region Modify task extension CheckList
        [HttpPost]
        public ActionResult GetTaskCheckList(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var shortCodes = CommUtils.Split(shortCode);
                List<Task> tasks = m_dbAdapter.Task.GetTasks(shortCodes, true);

                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.CheckList,
                    "工作[{0}]的工作扩展类型不是[工作要点检查]");

                var dictTaskCheckList = tasks.ToDictionary<Task, string, TaskExCheckListInfo>(x => x.ShortCode, val =>
                {
                    var taskExInfo = val.TaskExtension.TaskExtensionInfo;
                    if (string.IsNullOrEmpty(taskExInfo))
                    {
                        return new TaskExCheckListInfo();
                    }
                    else
                    {
                        var taskExDocs = CommUtils.FromJson<TaskExCheckListInfo>(taskExInfo);
                        return taskExDocs;
                    }
                });

                var jsonResult = new JsonResult();
                var firstCheckListGroup = dictTaskCheckList.First().Value;
                if (firstCheckListGroup.CheckGroups == null)
                {
                    return ActionUtils.Success(jsonResult);
                }
                foreach (var key in dictTaskCheckList.Keys)
                {
                    var curCheckListGroup = dictTaskCheckList[key];
                    if (curCheckListGroup.CheckGroups.Count != firstCheckListGroup.CheckGroups.Count)
                    {
                        return ActionUtils.Success(jsonResult);
                    }
                    else
                    {
                        for (int i = 0; i < curCheckListGroup.CheckGroups.Count; i++)
                        {
                            if (curCheckListGroup.CheckGroups[i].CheckItems.Count
                                != firstCheckListGroup.CheckGroups[i].CheckItems.Count)
                            {
                                return ActionUtils.Success(jsonResult);
                            }
                        }
                    }

                    for (int i = 0; i < curCheckListGroup.CheckGroups.Count; ++i)
                    {
                        var checkItemList = curCheckListGroup.CheckGroups[i];
                        for (int j = 0; j < checkItemList.CheckItems.Count; j++)
                        {
                            if (!checkItemList.CheckItems[j].EqualsCheckList(firstCheckListGroup.CheckGroups[i].CheckItems[j]))
                            {
                                return ActionUtils.Success(jsonResult);
                            }
                        }
                    }
                }

                jsonResult.Data = firstCheckListGroup;
                return ActionUtils.Success(jsonResult);
            });
        }

        private ProjectLogicModel LoadProjectLogicModelByTasks(List<Task> tasks)
        {
            CommUtils.Assert(tasks.Count != 0, "请选择工作");
            var projectId = tasks.First().ProjectId;
            CommUtils.Assert(tasks.All(x => x.ProjectId == projectId), "只能对同一产品下的多个工作进行操作");
            return Platform.GetProject(projectId);
        }

        [HttpPost]
        public ActionResult AddTaskExtensionCheckList(string shortCode, string groupName, string checkItemName)
        {
            return ActionUtils.Json(() =>
            {
                var shortCodes = CommUtils.Split(shortCode);
                List<Task> tasks = m_dbAdapter.Task.GetTasks(shortCodes, true);
                CommUtils.Assert(!string.IsNullOrWhiteSpace(groupName), "分组名称不能为空");

                var logicModel = LoadProjectLogicModelByTasks(tasks);

                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.CheckList,
                    "工作[{0}]的工作扩展类型不是[工作要点检查]");
                var checkItemNameList = CommUtils.Split(checkItemName, new string[] { "\n" });

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

                foreach (var task in tasks)
                {
                    m_dbAdapter.Task.CheckPrevIsFinished(task);

                    TaskExCheckListInfo taskExCheckLists = new TaskExCheckListInfo();
                    if (!string.IsNullOrEmpty(task.TaskExtension.TaskExtensionInfo))
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

                    task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(taskExCheckLists);
                }
                tasks.ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.TaskExtension.TaskExtensionInfo) && x.TaskStatus == TaskStatus.Finished)
                    {
                        var taskExCheckLists = CommUtils.FromJson<TaskExCheckListInfo>(x.TaskExtension.TaskExtensionInfo);
                        var isExistNotFinished = taskExCheckLists.CheckGroups.Any(group => group.CheckItems.Any(y => y.CheckStatus == TaskExCheckType.Unchecked.ToString()));
                        if (isExistNotFinished && x.TaskStatus == TaskStatus.Finished)
                        {
                            new TaskLogicModel(logicModel, x).Stop();
                        }
                    }
                });
                SaveTaskExs(tasks, "增加扩展工作检查项[" + checkItemName + "]");
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult DeleteExtensionCheckItem(string shortCode, string checkItemName, string groupName)
        {
            return ActionUtils.Json(() =>
            {
                var shortCodes = CommUtils.Split(shortCode);
                List<Task> tasks = m_dbAdapter.Task.GetTasks(shortCodes, true);
                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.CheckList,
                    "工作[{0}]的工作扩展类型不是[工作要点检查]");

                foreach (var task in tasks)
                {
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
                }

                SaveTaskExs(tasks, "删除扩展工作检查项[" + checkItemName + "]");
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult DeleteExtensionCheckItemGroup(string shortCode, string groupName)
        {
            return ActionUtils.Json(() =>
            {
                var shortCodes = CommUtils.Split(shortCode);
                List<Task> tasks = m_dbAdapter.Task.GetTasks(shortCodes, true);
                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.CheckList,
                    "工作[{0}]的工作扩展类型不是[工作要点检查]");

                foreach (var task in tasks)
                {
                    var taskExInfo = task.TaskExtension.TaskExtensionInfo;
                    if (taskExInfo != null)
                    {
                        var taskExCheckListInfo = CommUtils.FromJson<TaskExCheckListInfo>(taskExInfo);
                        CommUtils.AssertEquals(taskExCheckListInfo.CheckGroups.Count(x => x.GroupName == groupName),
                            1, "工作[" + task.Description + "]检查项分组[" + groupName + "]有误，请刷新后重试");

                        taskExCheckListInfo.CheckGroups.RemoveAll(x => x.GroupName == groupName);

                        task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(taskExCheckListInfo);
                    }
                }

                SaveTaskExs(tasks, "删除扩展工作检查项分组[" + groupName + "]");
                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskExtensionCheckList(string shortCode, string groupName, string checkItemName, string checkItemGuid, string checkItemType)
        {
            return ActionUtils.Json(() =>
            {
                var shortCodes = CommUtils.Split(shortCode);
                List<Task> tasks = m_dbAdapter.Task.GetTasks(shortCodes, true);
                var logicModel = LoadProjectLogicModelByTasks(tasks);

                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.CheckList,
                    "工作[{0}]的工作扩展类型不是[工作要点检查]");

                var oldItemStatus = CommUtils.ParseEnum<TaskExCheckType>(checkItemType);

                var revisionCheckType = string.Empty;
                foreach (var task in tasks)
                {
                    m_dbAdapter.Task.CheckPrevIsFinished(task);
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
                }

                SaveTaskExs(tasks, "校验扩展工作检查项[" + checkItemName + "]");

                tasks.ForEach(x =>
                {
                    m_dbAdapter.Task.AddTaskLog(x, "校验分组[" + groupName + "]中的工作要点[" + checkItemName + "]，状态为：" + Toolkit.ConvertTaskExCheckType(revisionCheckType));
                    new TaskLogicModel(logicModel, x).Start();
                });

                return ActionUtils.Success(tasks.Count);
            });
        }

        #endregion

        [HttpPost]
        public ActionResult GetProductNameByShortCode(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode);
                var project = m_dbAdapter.Project.GetProjectById(tasks.First().ProjectId);
                return ActionUtils.Success(project.Name);
            });
        }

        [HttpPost]
        public ActionResult GetAnalogousTaskShortCode(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var task = m_dbAdapter.Task.GetTask(shortCode);
                CheckAuthority(task);
                CommUtils.Assert(task.TemplateTaskId.HasValue, "工作[{0}({1)]没有同类工作",
                    task.Description, task.ShortCode);
                var tasks = m_dbAdapter.Task.GetDerivedTasksByProjectId(task.ProjectId, task.TemplateTaskId.Value);
                var list = tasks.Select(x => x.ShortCode).ToList();
                return ActionUtils.Success(list);
            });
        }

        [HttpPost]
        public ActionResult GetTasksInfo(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode);
                var list = tasks.ConvertAll(x => new
                {
                    ShortCode = x.ShortCode,
                    TaskName = x.Description,
                    StartTime = Toolkit.DateToString(x.StartTime),
                    EndTime = Toolkit.DateToString(x.EndTime),
                    Status = Toolkit.ToCnString(x.TaskStatus),
                    Detail = x.TaskDetail,
                    Target = x.TaskTarget
                }).ToList();
                return ActionUtils.Success(list);
            });
        }

        private void UpdateTaskExtensionType(Task task, string newTaskExtensionType)
        {
            var projectId = task.ProjectId;
            var shortCode = task.ShortCode;
            var newType = CommUtils.ParseEnum<TaskExtensionType>(newTaskExtensionType);
            var oldType = TaskExtensionType.None;
            if (task.TaskExtensionId.HasValue)
            {
                var taskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);
                oldType = CommUtils.ParseEnum<TaskExtensionType>(taskExtension.TaskExtensionType);
            }

            if (oldType == newType)
            {
                return;
            }

            if (newType == TaskExtensionType.None)
            {
                //删除扩展任务
                task.TaskExtensionId = null;
                m_dbAdapter.Task.UpdateTask(task);
                LogEditProduct(EditProductType.EditTask, projectId, "更新Task[" + task.ShortCode + "]，删除扩展任务[" + task.TaskExtensionId + "]", "");
            }
            else if (oldType == TaskExtensionType.None)
            {
                //新增扩展任务
                var extensionType = newType.ToString();
                var taskExtension = new ChineseAbs.ABSManagement.Models.TaskExtension();
                taskExtension.TaskExtensionName = extensionType;
                taskExtension.TaskExtensionType = extensionType;
                taskExtension.TaskExtensionStatus = 0;
                taskExtension = m_dbAdapter.Task.NewTaskExtension(taskExtension);

                task.TaskExtensionId = taskExtension.TaskExtensionId;
                m_dbAdapter.Task.UpdateTask(task);

                LogEditProduct(EditProductType.EditTask, projectId, "更新Task[" + task.ShortCode + "]，新建扩展任务[" + taskExtension.TaskExtensionId + "]", "");
            }
            else
            {
                //修改扩展任务类型
                var extensionType = newType.ToString();
                var oldTaskExtension = m_dbAdapter.Task.GetTaskExtension(task.TaskExtensionId.Value);
                var taskExtension = new ChineseAbs.ABSManagement.Models.TaskExtension();
                taskExtension.TaskExtensionName = extensionType;
                taskExtension.TaskExtensionType = extensionType;
                taskExtension.TaskExtensionStatus = 0;
                taskExtension.TaskExtensionId = oldTaskExtension.TaskExtensionId;
                m_dbAdapter.Task.UpdateTaskExtension(taskExtension);

                LogEditProduct(EditProductType.EditTask, projectId, "更新Task[" + task.ShortCode + "]，更新扩展任务类型[" + oldType.ToString() + "] -> [" + newType.ToString() + "]", "");
            }
        }

        [HttpPost]
        public ActionResult ModifyTaskExtensionType(string shortCode, string newTaskExtensionType)
        {
            return ActionUtils.Json(() =>
            {
                var tasks = LoadTasks(shortCode);
                tasks.ForEach(x => UpdateTaskExtensionType(x, newTaskExtensionType));
                return ActionUtils.Success(tasks.Count());
            });
        }

        #region
        [HttpPost]
        public ActionResult AddBatchPrevTaskShortCode(string shortCode, string prevTaskName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrEmpty(prevTaskName), "前置工作不能为空");
                var tasks = LoadTasks(shortCode);
                var allTasks = m_dbAdapter.Task.GetTasksByProjectId(tasks.First().ProjectId);
                var dictAllTasks = allTasks.ToDictionary(x => x.TaskId);
                AssertAllTasks(tasks, x => x.Description != prevTaskName, "前置工作不能是工作{0}本身");

                var prevTasks = allTasks.Where(x => x.Description == prevTaskName).ToList();
                CommUtils.AssertEquals(tasks.Count, prevTasks.Count,
                    "选中的工作的数量[{0}]与要添加为前置工作[" + prevTaskName + "]的数量[{1}]不匹配", tasks.Count, prevTasks.Count);

                for (int i = 0; i < tasks.Count; ++i)
                {
                    var task = tasks[i];
                    var prevTask = prevTasks[i];
                    CommUtils.Assert(!task.PrevTaskIdArray.Contains(prevTask.TaskId), "工作{0}({1})中已经包含前置工作{2}({3})",
                        task.Description, task.ShortCode, prevTask.Description, prevTask.ShortCode);

                    CommUtils.Assert(task.EndTime >= prevTask.EndTime, "工作{0}({1})的截止时间{4}不能小于前置工作{2}({3})的截止时间{5}",
                        task.Description, task.ShortCode, prevTask.Description, prevTask.ShortCode,
                        task.EndTime.Value.ToShortDateString(), prevTask.EndTime.Value.ToShortDateString());

                    task.PrevTaskIdArray.Add(prevTask.TaskId);
                    task.PreTaskIds = CommUtils.Join(task.PrevTaskIdArray.ConvertAll(x => x.ToString()));

                    dictAllTasks[task.TaskId].PrevTaskIdArray.Add(prevTask.TaskId);
                    dictAllTasks[task.TaskId].PreTaskIds = task.PreTaskIds;
                }

                CheckEndlessLoopDependency(allTasks);

                var newVals = tasks.ConvertAll(x => CommUtils.Join(x.PrevTaskIdArray.ConvertAll(taskId => dictAllTasks[taskId].ShortCode)));
                SaveTasks(tasks, "前置工作", newVals);

                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult AddOncePrevTaskShortCode(string shortCode, string prevTaskShortCode)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(!string.IsNullOrEmpty(prevTaskShortCode), "前置工作不能为空");
                var tasks = LoadTasks(shortCode);
                var allTasks = m_dbAdapter.Task.GetTasksByProjectId(tasks.First().ProjectId);
                var dictAllTasks = allTasks.ToDictionary(x => x.TaskId);
                AssertAllTasks(tasks, x => x.ShortCode != prevTaskShortCode, "前置工作不能是工作{0}本身");
                var prevTasks = LoadTasks(prevTaskShortCode);

                CommUtils.Assert(tasks.Count == 1 && prevTasks.Count == 1, "要修改的工作数量和前置工作数量必须为1");
                var task = tasks.Single();
                var prevTask = prevTasks.Single();

                CommUtils.Assert(!task.PrevTaskIdArray.Contains(prevTask.TaskId), "工作{0}({1})中已经包含前置工作{2}({3})",
                    task.Description, task.ShortCode, prevTask.Description, prevTask.ShortCode);

                CommUtils.Assert(task.EndTime >= prevTask.EndTime, "工作{0}({1})的截止时间{4}不能小于前置工作{2}({3})的截止时间{5}",
                    task.Description, task.ShortCode, prevTask.Description, prevTask.ShortCode,
                    task.EndTime.Value.ToShortDateString(), prevTask.EndTime.Value.ToShortDateString());

                task.PrevTaskIdArray.Add(prevTask.TaskId);
                task.PreTaskIds = CommUtils.Join(task.PrevTaskIdArray.ConvertAll(x => x.ToString()));

                dictAllTasks[task.TaskId].PrevTaskIdArray.Add(prevTask.TaskId);
                dictAllTasks[task.TaskId].PreTaskIds = task.PreTaskIds;
                CheckEndlessLoopDependency(allTasks.Where(x => x.EndTime.Value == task.EndTime.Value));

                SaveTask(task, "前置工作", CommUtils.Join(task.PrevTaskIdArray.ConvertAll(x => dictAllTasks[x].ShortCode)));
                return ActionUtils.Success(1);
            });
        }
        #endregion

        #region Modify task extension RecyclingPaymentDate

        [HttpPost]
        public ActionResult ConfigAccountComparisonModes(string shortCode, string comparisonModes, string accountTypeText)
        {
            return ActionUtils.Json(() =>
            {
                var shortCodes = CommUtils.Split(shortCode);
                List<Task> tasks = m_dbAdapter.Task.GetTasks(shortCodes, true);

                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.RecyclingPaymentDate,
                    "工作[{0}]的工作扩展类型不是[确认账户余额]");
                CheckAuthority(tasks);

                var compareSign = CommUtils.ParseEnum<SignComparisonModes>(comparisonModes);
                var accountType = CommUtils.ParseEnum<EAccountType>(accountTypeText);

                foreach (var task in tasks)
                {
                    var taskExtensionInfo = new RecyclingPaymentDate();

                    if (!string.IsNullOrWhiteSpace(task.TaskExtension.TaskExtensionInfo))
                    {
                        taskExtensionInfo = CommUtils.FromJson<RecyclingPaymentDate>(task.TaskExtension.TaskExtensionInfo);
                    }

                    taskExtensionInfo.AccountType = accountType.ToString();
                    taskExtensionInfo.CompareSign = compareSign.ToString();
                    task.TaskExtension.TaskExtensionInfo = CommUtils.ToJson(taskExtensionInfo);
                }

                SaveTaskExs(tasks, "配置账户对比方式CompareSign[" + compareSign.ToString() +
                    "]AccountType[" + accountType.ToString() + "]");

                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult GetTaskExAccountComparisonModes(string shortCode)
        {
            return ActionUtils.Json(() =>
            {
                var shortCodes = CommUtils.Split(shortCode);
                List<Task> tasks = m_dbAdapter.Task.GetTasks(shortCodes, true);

                AssertAllTasks(tasks, x => x.TaskExtensionId.HasValue, "工作[{0}]不包含扩展信息");
                AssertAllTasks(tasks, x => CommUtils.ParseEnum<TaskExtensionType>(x.TaskExtension.TaskExtensionType) == TaskExtensionType.RecyclingPaymentDate,
                    "工作[{0}]的工作扩展类型不是[确认账户余额]");
                CheckAuthority(tasks);

                var dictTaskEx = tasks.ToDictionary<Task, string, RecyclingPaymentDate>(x => x.ShortCode, val =>
                {
                    var taskExInfo = val.TaskExtension.TaskExtensionInfo;
                    if (string.IsNullOrEmpty(taskExInfo))
                    {
                        return new RecyclingPaymentDate();
                    }
                    else
                    {
                        var taskExDocs = CommUtils.FromJson<RecyclingPaymentDate>(taskExInfo);
                        return taskExDocs;
                    }
                });

                var firstTaskDex = dictTaskEx.First().Value;

                foreach (var key in dictTaskEx.Keys)
                {
                    var curTaskEx = dictTaskEx[key];

                    if (curTaskEx.AccountType != firstTaskDex.AccountType ||
                        curTaskEx.CompareSign != firstTaskDex.CompareSign)
                    {
                        return ActionUtils.Success(null);
                    }
                }

                return ActionUtils.Success(firstTaskDex);
            });
        }
        #endregion

        [HttpPost]
        public ActionResult GenerateTasks(string projectGuid, string templateGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject), "没有在产品[" + project.Name + "]生成工作的权限");
                var authoritiedProjects = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);
                CommUtils.Assert(authoritiedProjects.Contains(project.ProjectId), "没有在产品[" + project.Name + "]生成工作的权限");

                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var templateTasks = m_dbAdapter.Template.GetTemplateTasks(template.TemplateId);
                var templateTimes = m_dbAdapter.Template.GetTemplateTimeLists(template.TemplateId);

                var generateTasksCount = m_dbAdapter.Task.GenerateTasks(project.ProjectId, templateTasks, templateTimes);
                return ActionUtils.Success(generateTasksCount);
            });
        }

        [HttpPost]
        public ActionResult GetTemplates()
        {
            return ActionUtils.Json(() =>
            {
                var templates = m_dbAdapter.Template.GetTemplates();
                var result = templates.ConvertAll(x => new
                {
                    TemplateGuid = x.TemplateGuid,
                    TemplateName = x.TemplateName
                });
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetGenerateTasksCount(string projectGuid, string templateGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var template = m_dbAdapter.Template.GetTemplate(templateGuid);
                var templateTasks = m_dbAdapter.Template.GetTemplateTasks(template.TemplateId);
                var templateTimes = m_dbAdapter.Template.GetTemplateTimeLists(template.TemplateId);
                var generateTaskCount = m_dbAdapter.Task.GetGenerateTasksCount(project.ProjectId, templateTasks, templateTimes);
                var taskCount = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId).Count;
                var result = new { taskCount = taskCount, generateTaskCount = generateTaskCount };
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetTasksCount(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var taskCount = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId).Count;
                return ActionUtils.Success(taskCount);
            });
        }

        [HttpPost]
        public ActionResult RemoveAllTasks(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject), "没有清空产品的工作权限");

                var authoritiedProjects = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(authoritiedProjects.Contains(project.ProjectId), "没有清空产品的工作权限");

                var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId);
                var taskStatusId = CommUtils.Join(tasks.ConvertAll(x => ((int)x.TaskStatus).ToString()));
                var taskId = CommUtils.Join(tasks.ConvertAll(x => x.TaskId.ToString()));

                int deleteTaskCount = m_dbAdapter.Task.DeleteTasks(tasks);

                LogEditProduct(EditProductType.EditTask, project.ProjectId, "清空产品任务",
                    "共删除" + deleteTaskCount.ToString() + "条产品任务，taskId=["
                    + taskId + "],原taskStatusId=[" + taskStatusId + "]");

                return ActionUtils.Success(tasks.Count);
            });
        }

        [HttpPost]
        public ActionResult RemoveSelectedTasks(string projectGuid, string ShortCodeToString)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject), "没有修改工作的权限");

                var authoritiedProjects = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(authoritiedProjects.Contains(project.ProjectId), "没有修改工作的权限");

                var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId);

                var shortCodeList = CommUtils.Split(ShortCodeToString).ToList();
                var removeTaskList = tasks.Where(x => shortCodeList.Contains(x.ShortCode)).ToList();
                var removeTaskIdList = removeTaskList.ConvertAll(x => x.TaskId);

                var updateTasks = tasks.Where(x => x.PrevTaskIdArray.Intersect(removeTaskIdList).Count() > 0).ToList();
                updateTasks.ForEach(x =>
                {
                    var ids = x.PrevTaskIdArray.Except(removeTaskIdList).ToList();
                    x.PreTaskIds = CommUtils.Join(ids.ConvertAll(id => id.ToString()).ToArray());
                    m_dbAdapter.Task.UpdateTask(x);
                });

                var resultCount = m_dbAdapter.Task.DeleteTasks(removeTaskList);
                var taskId = CommUtils.Join(tasks.ConvertAll(x => x.TaskId.ToString()));
                LogEditProduct(EditProductType.EditTask, project.ProjectId, "删除产品任务", "共删除" + resultCount +
                    "条产品任务，taskId=[" + taskId + "]");

                return ActionUtils.Success(resultCount);
            });

        }

        [HttpPost]
        public ActionResult FinishSelectedTasks(string projectGuid, string shortCodeToString)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.Assert(m_dbAdapter.Authority.IsAuthorized(AuthorityType.CreateProject), "没有修改工作的权限");

                var authoritiedProjects = m_dbAdapter.Authority.GetAuthorizedProjectIds(AuthorityType.ModifyTask);

                var project = Platform.GetProject(projectGuid);
                CommUtils.Assert(authoritiedProjects.Contains(project.Instance.ProjectId), "没有修改工作的权限");

                var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.Instance.ProjectId);

                var shortCodeList = CommUtils.Split(shortCodeToString).ToList();
                //找出不包含扩展工作的Tasks
                var finishTaskList = tasks.Where(x => shortCodeList.Contains(x.ShortCode) && !x.TaskExtensionId.HasValue).ToList();

                List<int> modifyTaskIdList = new List<int>();
                while (true)
                {
                    var oldModifyTaskCount = modifyTaskIdList.Count;
                    foreach (var task in finishTaskList)
                    {
                        if (task.TaskStatus == TaskStatus.Finished)
                        {
                            continue;
                        }

                        var prevTaskIdArray = task.PrevTaskIdArray;
                        var prevTasks = tasks.Where(x => prevTaskIdArray.Contains(x.TaskId));
                        if (prevTasks.Any(x => x.TaskStatus != TaskStatus.Finished))
                        {
                            continue;
                        }

                        var taskLogicModel = new TaskLogicModel(project, task);
                        taskLogicModel.Start("开始工作").Finish("完成工作");

                        modifyTaskIdList.Add(task.TaskId);
                    }

                    finishTaskList.RemoveAll(x => modifyTaskIdList.Contains(x.TaskId));

                    if (oldModifyTaskCount == modifyTaskIdList.Count)
                    {
                        break;
                    }
                }

                var taskId = CommUtils.Join(modifyTaskIdList.Select(x => x.ToString()).ToArray());
                LogEditProduct(EditProductType.EditTask, project.Instance.ProjectId, "快速完成工作", "完成"
                    + modifyTaskIdList.Count + "条工作，taskId=[" + taskId + "]");

                return ActionUtils.Success(modifyTaskIdList.Count);
            });

        }

        [HttpGet]
        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyModel)]
        public ActionResult EditModel()
        {
            return View();
        }

        [HttpPost]
        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyModel)]
        public ActionResult GetProjectName(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                return ActionUtils.Success(project.Name);
            });
        }

        [HttpPost]
        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyModel)]
        public ActionResult GetModels(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var logicModel = Platform.GetProject(projectGuid);
                var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(logicModel.Instance.ProjectId).ConvertAll(x => new EditModelDatasetViewModel(x));

                var allPaymentDates = logicModel.GetAllPaymentDates(logicModel.DealSchedule.Instanse.PaymentDates);
                datasets.ForEach(x =>
                {
                    int sequence = allPaymentDates.IndexOf(x.PaymentDate.Value);
                    x.Sequence = (sequence >= 0) ? (sequence + 1).ToString() : "#";
                });

                var result = new
                {
                    datasets = datasets.ConvertAll(x => new
                    {
                        sequence = x.Sequence,
                        asOfDate = x.AsOfDate,
                        paymentDate = Toolkit.DateToString(x.PaymentDate)
                    }),
                    enablePredictMode = logicModel.EnablePredictMode
                };
                ;

                return ActionUtils.Success(result);
            });
        }

        private void BackUpModel(string srcFolder, string backupFolder)
        {
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            var backupFileName = backupFolder + @"\" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff") + ".zip";
            ZipUtils.Compress(backupFileName, srcFolder);
        }

        protected class ModelFile
        {
            public ModelFile(string pathName, bool mustExist)
            {
                PathName = pathName;
                MustExist = mustExist;
            }

            public string PathName;
            public string ZipPathName;
            public bool MustExist;
        }

        protected class ModelFiles
        {
            public ModelFiles(string AsOfDate)
            {
                YmlFile = new ModelFile(@"script.yml", false);

                CurrentVariables = new ModelFile(AsOfDate + @"/CurrentVariables.csv", false);
                FutureVariables = new ModelFile(AsOfDate + @"/FutureVariables.csv", false);
                PastVariables = new ModelFile(AsOfDate + @"/PastVariables.csv", false);
                CombinedVariables = new ModelFile(AsOfDate + @"/CombinedVariables.csv", false);


                AssetCashflowTable = new ModelFile(AsOfDate + @"/AssetCashflowTable.csv", false);
                CashflowTable = new ModelFile(AsOfDate + @"/CashflowTable.csv", false);
                Amortization = new ModelFile(AsOfDate + @"/AmortizationSchedule.csv", false);
                PromisedCashflow = new ModelFile(AsOfDate + @"/PromisedCashflow.csv", false);
                Reinvestment = new ModelFile(AsOfDate + @"/Reinvestment.csv", false);
                AnalyzerResults = new ModelFile(AsOfDate + @"/AnalyzerResults.csv", false);

                collateral = new ModelFile(AsOfDate + @"/collateral.csv", false);


                
            }

            public ModelFile YmlFile;
            public ModelFile AnalyzerResults;
            public ModelFile AssetCashflowTable;
            public ModelFile CashflowTable;
            public ModelFile Amortization;
            public ModelFile PromisedCashflow;
            public ModelFile Reinvestment;
            public ModelFile collateral;
            public ModelFile CurrentVariables;
            public ModelFile FutureVariables;
            public ModelFile PastVariables;
            public ModelFile CombinedVariables { get; set; }

            public Dictionary<string, string> GetFileDictionary()
            {
                var dict = new Dictionary<string, string>();
                Action<ModelFile> addDict = (modelFile) =>
                {
                    if (!string.IsNullOrEmpty(modelFile.PathName) && !string.IsNullOrEmpty(modelFile.ZipPathName))
                    {
                        dict[modelFile.ZipPathName] = modelFile.PathName;
                    }
                };

                addDict(YmlFile);

                addDict(Amortization);
                addDict(PromisedCashflow);
                addDict(Reinvestment);

                addDict(AssetCashflowTable);
                addDict(CashflowTable);

                addDict(AnalyzerResults);
                addDict(CurrentVariables);
                addDict(FutureVariables);
                addDict(PastVariables);
                addDict(CombinedVariables);
                addDict(collateral);
                return dict;
            }
        }

        private ModelFiles CheckModelFile(string asOfDate, List<string> fileNames, List<string> folderNames)
        {
            var knownCsvFiles = new List<string> {
                asOfDate + @"/AnalyzerResults.csv",
                asOfDate + @"/CurrentVariables.csv",
                asOfDate + @"/FutureVariables.csv",
                asOfDate + @"/PastVariables.csv",
                asOfDate + @"/CombinedVariables.csv",

                asOfDate + @"/AssetCashflowTable.csv",
                asOfDate + @"/CashflowTable.csv",

                asOfDate + @"/AmortizationSchedule.csv",
                asOfDate + @"/PromisedCashflow.csv",
                asOfDate + @"/Reinvestment.csv",
            };

            //文件名称/路径完全匹配
            Func<string, string, bool> cmpFileName = (left, right) => left.Equals(right, StringComparison.CurrentCultureIgnoreCase);

            //检查文件扩展名
            Func<string, string, bool> checkExtension = (fileName, ExtensionName) =>
            {
                return fileName.Length - ExtensionName.Length - 1 ==
                    fileName.LastIndexOf("." + ExtensionName, StringComparison.CurrentCultureIgnoreCase);
            };

            //检查collateral.csv文件
            Func<string, string, bool> checkCollateral = (fileName, ExtensionName) =>
            {
                return checkExtension(fileName, ExtensionName) && !knownCsvFiles.Contains(fileName);
            };

            //查找文件
            Func<ModelFile, Func<string, string, bool>, string, string> findFile = (modelFile, cmpFunc, param) =>
            {
                var fileCount = fileNames.Count(x => cmpFunc(x, param));
                CommUtils.Assert(fileCount < 2, "查找 [" + modelFile.PathName + "] 失败!");
                if (modelFile.MustExist)
                {
                    CommUtils.Assert(fileCount > 0, "找不到文件 [" + modelFile.PathName + "] !");
                }

                return fileNames.SingleOrDefault(x => cmpFunc(x, param));
            };

            var modelFiles = new ModelFiles(asOfDate);
            CommUtils.Assert(folderNames.Any(x => x.Equals(asOfDate, StringComparison.CurrentCultureIgnoreCase)), "找不到文件夹 [{0}]", asOfDate);

            modelFiles.YmlFile.ZipPathName = findFile(modelFiles.YmlFile, checkExtension, "yml");

            modelFiles.Amortization.ZipPathName = findFile(modelFiles.Amortization, cmpFileName, modelFiles.Amortization.PathName);
            modelFiles.PromisedCashflow.ZipPathName = findFile(modelFiles.PromisedCashflow, cmpFileName, modelFiles.PromisedCashflow.PathName);
            modelFiles.Reinvestment.ZipPathName = findFile(modelFiles.Reinvestment, cmpFileName, modelFiles.Reinvestment.PathName);

            modelFiles.CashflowTable.ZipPathName = findFile(modelFiles.CashflowTable, cmpFileName, modelFiles.CashflowTable.PathName);
            modelFiles.CurrentVariables.ZipPathName = findFile(modelFiles.CurrentVariables, cmpFileName, modelFiles.CurrentVariables.PathName);
            modelFiles.FutureVariables.ZipPathName = findFile(modelFiles.FutureVariables, cmpFileName, modelFiles.FutureVariables.PathName);
            modelFiles.PastVariables.ZipPathName = findFile(modelFiles.PastVariables, cmpFileName, modelFiles.PastVariables.PathName);
            modelFiles.CombinedVariables.ZipPathName = findFile(modelFiles.CombinedVariables, cmpFileName, modelFiles.CombinedVariables.PathName);

            modelFiles.AnalyzerResults.ZipPathName = findFile(modelFiles.AnalyzerResults, cmpFileName, modelFiles.AnalyzerResults.PathName);
            modelFiles.AssetCashflowTable.ZipPathName = findFile(modelFiles.AssetCashflowTable, cmpFileName, modelFiles.AssetCashflowTable.PathName);

            //查找collateral.csv
            modelFiles.collateral.ZipPathName = findFile(modelFiles.collateral, cmpFileName, modelFiles.collateral.PathName);
            if (string.IsNullOrEmpty(modelFiles.collateral.ZipPathName))
            {
                //查找其它csv来替代collateral.csv
                modelFiles.collateral.ZipPathName = findFile(modelFiles.collateral, checkCollateral, "csv");
            }

            if (modelFiles.CombinedVariables.ZipPathName == null)
            {
                if(modelFiles.CurrentVariables.ZipPathName == null 
                    || modelFiles.FutureVariables.ZipPathName == null
                    || modelFiles.PastVariables.ZipPathName == null
                    )
                {
                    throw new Exception("变量文件不存在");
                }
            }

            return modelFiles;
        }

        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyModel)]
        public ActionResult UploadModel(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                CheckAsOfDate(asOfDate);
                CommUtils.Assert(Request.Files.Count > 0, "请选择文件");

                var file = Request.Files[0];
                CommUtils.Assert(file.ContentLength > 0, "请选择文件");
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, bytes.Length);
                file.InputStream.Seek(0, SeekOrigin.Begin);

                CommUtils.Assert(file.FileName.EndsWith(".zip", StringComparison.CurrentCultureIgnoreCase),
                    "只支持导入压缩(.zip)文件");

                var fileNames = ZipUtils.GetZipFileNames(file.InputStream);
                file.InputStream.Seek(0, SeekOrigin.Begin);
                Stream newStream = new MemoryStream(bytes);
                var folderNames = ZipUtils.GetZipFolderNames(newStream);

                //检查上传模型文件结构
                var modelFiles = CheckModelFile(asOfDate, fileNames, folderNames);

                //备份模型文件
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var modelFolder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);
                var backupFolder = Path.Combine(WebConfigUtils.RootFolder, "Backup");
                backupFolder = Path.Combine(backupFolder, project.Model.ModelFolder);
                BackUpModel(modelFolder, backupFolder);

                LogEditProduct(EditProductType.EditTask, project.ProjectId, "上传模型文件Project=[" + project.Name + "];ProjectId=[" + project.ProjectId + "];AsOfDate=[" + asOfDate + "]", "");

                var asOfDateFolder = Path.Combine(modelFolder, asOfDate);
                if (!Directory.Exists(asOfDateFolder))
                {
                    Directory.CreateDirectory(asOfDateFolder);
                }

                var fileDicts = modelFiles.GetFileDictionary();
                ZipUtils.ExtractFiles(file.InputStream, fileDicts, modelFolder);

                return ActionUtils.Success(fileNames.ToString());
            });
        }

        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyModel)]
        public ActionResult DownloadModel(string projectGuid, string asOfDate)
        {
            CheckAsOfDate(asOfDate);

            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var folder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);

            var fileNames = new List<string> {
                "script.yml",
                asOfDate + @"\AmortizationSchedule.csv",
                asOfDate + @"\PromisedCashflow.csv",
                asOfDate + @"\Reinvestment.csv",
                asOfDate + @"\AnalyzerResults.csv",
                asOfDate + @"\AssetCashflowTable.csv",
                asOfDate + @"\CashflowTable.csv",
                asOfDate + @"\collateral.csv",
                asOfDate + @"\CurrentVariables.csv",
                asOfDate + @"\FutureVariables.csv",
                asOfDate + @"\PastVariables.csv"
            };

            var ms = new MemoryStream();
            ZipUtils.CompressFiles(folder, fileNames, ms);

            var fileFullName = project.Name + "(" + asOfDate + ").zip";

            LogEditProduct(EditProductType.EditTask, project.ProjectId, "下载模型文件Project=[" + project.Name + "];ProjectId=[" + project.ProjectId + "];AsOfDate=[" + asOfDate + "];FileName=[" + fileFullName + "]", "");

            //As of now, MIME type of a zip file is application/octet-stream in Google Chrome
            var mimeType = "application/octet-stream";

            return File(ms, mimeType, fileFullName);
        }

        private void CreateDataset(Project project, string asOfDate, DateTime paymentDate)
        {
            var projectId = project.ProjectId;
            var datasets = m_dbAdapter.Dataset.GetDatasetByProjectId(projectId);

            CommUtils.Assert(!datasets.Any(x => x.AsOfDate == asOfDate),
                "产品[{0}]中,封包日为[{1}]的模型已存在。", project.Name, asOfDate);

            CommUtils.Assert(!datasets.Any(x => x.PaymentDate == paymentDate),
                "产品[{0}]中,支付日为[{1}]的模型已存在。", project.Name, paymentDate);

            var notes = m_dbAdapter.Dataset.GetNotes(projectId);

            //Create dataset
            var dataset = new Dataset();
            dataset.ProjectId = projectId;
            dataset.AsOfDate = asOfDate;
            dataset.PaymentDate = paymentDate;
            LogEditProduct(EditProductType.CreateProduct, projectId, "创建Dataset[" + projectId + "][" + asOfDate + "]", "");
            dataset = m_dbAdapter.Dataset.NewDataset(dataset);

            //Create note result
            var noteResult = new NoteResults();
            noteResult.ProjectId = projectId;
            noteResult.DatasetId = dataset.DatasetId;
            LogEditProduct(EditProductType.CreateProduct, projectId, "创建NoteResult[" + projectId + "][" + dataset.DatasetId + "]", "");
            noteResult = m_dbAdapter.Dataset.NewNoteResult(noteResult);

            //Create note data
            foreach (var note in notes)
            {
                var noteData = new NoteData();
                noteData.NoteId = note.NoteId;
                noteData.DatasetId = dataset.DatasetId;
                LogEditProduct(EditProductType.CreateProduct, projectId, "创建NoteData[" + note.NoteName + "][" + note.NoteId + "][" + dataset.DatasetId + "]", "");
                noteData = m_dbAdapter.Dataset.NewNoteData(noteData);
            }
        }

        [HttpPost]
        [DesignAccessAttribute(AuthorityType = AuthorityType.ModifyModel)]
        public ActionResult CreateDataset(string projectGuid, string asOfDate, string paymentDate)
        {
            return ActionUtils.Json(() =>
            {
                CheckAsOfDate(asOfDate);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CreateDataset(project, asOfDate, DateUtils.ParseDigitDate(paymentDate));
                return ActionUtils.Success("0");
            });
        }

        [HttpPost]
        public ActionResult DeleteDataset(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                CheckAsOfDate(asOfDate);

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                //备份原有的模型文件
                var modelFolder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);
                var backupFolder = Path.Combine(WebConfigUtils.RootFolder, "Backup", project.Model.ModelFolder);
                BackUpModel(modelFolder, backupFolder);

                //删除对应的模型文件
                var deleteFilePath = Path.Combine(modelFolder, asOfDate);
                DeleteAllFiles(deleteFilePath);
                DirectoryInfo parentPath = new DirectoryInfo(modelFolder);
                foreach (DirectoryInfo di in parentPath.GetDirectories())
                {
                    if (di.Name == asOfDate)
                    {
                        di.Delete();
                    }
                }

                //删除模型数据dbo.dataset
                var dataset = m_dbAdapter.Dataset.GetDatesetByProjectIdAsOfDate(project.ProjectId, asOfDate);
                var result = m_dbAdapter.Dataset.DeleteDataset(dataset);

                //记录dbo.dataset表的操作log
                LogEditProduct(EditProductType.DeleteDataset, project.ProjectId, "删除Dataset，dataset_id =[" + dataset.DatasetId + "]", "");

                if (result != 1)
                {
                    return ActionUtils.Failure("0");
                }
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GenerateNextDataset(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                CheckAsOfDate(asOfDate);

                var projectLogicModel = Platform.GetProject(projectGuid);
                CommUtils.Assert(projectLogicModel.EnablePredictMode, "目前系统仅支持在预测模式下生成下期模型");

                var project = projectLogicModel.Instance;
                var dealSchedule = projectLogicModel.DealSchedule.Instanse;
                CommUtils.Assert(dealSchedule != null, "当前产品没有任何模型");
                var curAsOfDate = DateUtils.ParseDigitDate(asOfDate);
                CommUtils.Assert(dealSchedule.DeterminationDates.Any(x => x > curAsOfDate), "无法确定下期模型的AsOfDate");
                var nextAsOfDate = dealSchedule.DeterminationDates.First(x => x > curAsOfDate);

                LogEditProduct(EditProductType.EditTask, project.ProjectId, "生成下期模型，当前asOfDate=[" + asOfDate + "]，下期asOfDate=[" + nextAsOfDate.ToString("yyyyMMdd") + "]", "");

                //Find payment date
                DateTime? nextPaymentDate = null;
                //创建第N期Dataset时，有模型数据，此时PaymentDate按照DealSchedule中AsOfDate对应Index查出
                if (nextAsOfDate == dealSchedule.FirstCollectionPeriodStartDate)
                {
                    nextPaymentDate = dealSchedule.PaymentDates[0];
                }
                else
                {
                    for (int i = 0; i < dealSchedule.DeterminationDates.Length - 1; ++i)
                    {
                        if (nextAsOfDate == dealSchedule.DeterminationDates[i])
                        {
                            nextPaymentDate = dealSchedule.PaymentDates[i + 1];
                            break;
                        }
                    }
                }

                CommUtils.AssertNotNull(nextPaymentDate, "无法根据AsOfDate[" + asOfDate + "]查询到对应的支付日。");

                //备份原有的模型文件
                var modelFolder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);
                var backupFolder = Path.Combine(WebConfigUtils.RootFolder, "Backup", project.Model.ModelFolder);
                BackUpModel(modelFolder, backupFolder);

                //生成下一期模型文件
                var assetModifier = new AssetModifier(CurrentUserName);
                assetModifier.Load(project.ProjectId, curAsOfDate);
                assetModifier.GenerateNextDataset(project.ProjectId);

                //数据库中生成下起Dataset
                CreateDataset(project, nextAsOfDate.ToString("yyyyMMdd"), nextPaymentDate.Value);

                return ActionUtils.Success("");
            });
        }

        private void DeleteAllFiles(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            if (dir.Exists)
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    DeleteAllFiles(di.FullName);
                    di.Delete();
                }
            }
        }

        #region 在线编辑CSv
        [HttpPost]
        public ActionResult GetCsvFiles(string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                CheckAsOfDate(asOfDate);

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                var folder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);
                folder = Path.Combine(folder, asOfDate);
                var fileNames = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>( "CurrentVariables.csv", "当期变量"),
                    new KeyValuePair<string, string>( "FutureVariables.csv", "预测变量"),
                    new KeyValuePair<string, string>( "PastVariables.csv", "历史变量"),
                    new KeyValuePair<string, string>( "collateral.csv", "资产池"),
                    new KeyValuePair<string, string>( "AmortizationSchedule.csv", "摊销计划表"),
                    new KeyValuePair<string, string>( "PromisedCashflow.csv", "资产偿付表"),
                    new KeyValuePair<string, string>( "CombinedVariables.csv", "变量"),
                 };
                List<CsvItem> dicCsvDatatable = new List<CsvItem>();
                var all = m_dbAdapter.EditModelCsv.GetAll();

                fileNames.ForEach(x =>
                {
                    string csvFilePath = Path.Combine(folder, x.Key);
                    if (System.IO.File.Exists(csvFilePath))
                    {
                        var csvItem = new CsvItem();
                        csvItem.Id = Path.GetFileNameWithoutExtension(x.Key);
                        csvItem.Title = x.Value;
                        csvItem.DataJson = DatatableToHandsonData(ExcelUtils.ReadCsv(csvFilePath));
                        csvItem.hasHistory = all.Exists(p => 
                        (p.ProjectGuid == projectGuid && p.Asofdate == asOfDate && p.Type == csvItem.Id));
                        dicCsvDatatable.Add(csvItem);
                    }
                });

                return ActionUtils.Success(dicCsvDatatable);
            });
        }

        [HttpPost]
        public ActionResult SaveCsvData(List<List<string>> dataList, string projectGuid, string asOfDate, string csvItemId, string title, string comment)
        {
            return ActionUtils.Json(() =>
            {
                DataTable dataTable = new DataTable();

                dataList[0].ForEach(x =>
                {
                    dataTable.Columns.Add(x);
                });

                dataList.Skip(1).ToList().ForEach(x =>
                {
                    DataRow dataRow = dataTable.NewRow();
                    var index = 0;
                    x.ForEach(y =>
                    {
                        dataRow[index] = y;
                        index++;
                    });
                    dataTable.Rows.Add(dataRow);

                });
                var enumCsvType = CommUtils.ParseEnum<EnumCsvType>(csvItemId);
                string csvPath = GetCsvFilePath(projectGuid, asOfDate, csvItemId);
                int resultSuccess = 0;

                if (!Directory.Exists(csvPath))
                {
                    Directory.CreateDirectory(csvPath);
                    var guid = NewEditModelCSV("原始版本", "原始版本", enumCsvType, projectGuid, asOfDate);
                    string historyCsvPath = Path.Combine(csvPath, guid + ".csv");
                    System.IO.File.Copy(csvPath + ".csv", historyCsvPath);

                    resultSuccess = CheckChangeAndSave(csvPath, dataTable, title, comment, enumCsvType, projectGuid, asOfDate);
                }
                else
                {
                    resultSuccess = CheckChangeAndSave(csvPath, dataTable, title, comment, enumCsvType, projectGuid, asOfDate);
                }
                return ActionUtils.Success(resultSuccess);
            });
        }

        private int CheckChangeAndSave(string csvPath, DataTable dataTable, string title, string comment, EnumCsvType enumCsvType, string projectGuid, string asOfDate)
        {
            DataTable lastCsvDt = ExcelUtils.ReadCsv(csvPath + ".csv");


            IEnumerable<DataRow> queryExcept = lastCsvDt.AsEnumerable().Except(dataTable.AsEnumerable(), DataRowComparer.Default);
            IEnumerable<DataRow> queryExcept2 = dataTable.AsEnumerable().Except(lastCsvDt.AsEnumerable(), DataRowComparer.Default);


            if ((queryExcept != null && queryExcept.Count() != 0) || (queryExcept2 != null && queryExcept2.Count() != 0))
            {
                var guid = NewEditModelCSV(title, comment, enumCsvType, projectGuid, asOfDate);
                var historyCsvPath = Path.Combine(csvPath, guid + ".csv");
                ExcelUtils.WriteCsv(dataTable, historyCsvPath);
                ExcelUtils.WriteCsv(dataTable, csvPath + ".csv");
                return 1;
            }
            else
            {
                return 0;
            }

        }

        private string NewEditModelCSV(string title, string comment, EnumCsvType csvItemId, string projectGuid, string asOfDate)
        {
            var editModelCsvItem = new EditModelCSV();
            editModelCsvItem.ProjectGuid = projectGuid;
            editModelCsvItem.Asofdate = asOfDate;
            editModelCsvItem.Comment = comment;
            editModelCsvItem.Title = title;
            editModelCsvItem.Type = csvItemId.ToString();
            var newEditModel = m_dbAdapter.EditModelCsv.New(editModelCsvItem);
            return newEditModel.Guid;
        }

        private string GetCsvFilePath(string projectGuid, string asOfDate, string csvItemId)
        {
            CheckAsOfDate(asOfDate);
            var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
            var folder = Path.Combine(WebConfigUtils.RootFolder, project.Model.ModelFolder);
            folder = Path.Combine(folder, asOfDate);
            string csvFilePath = Path.Combine(folder, csvItemId);
            return csvFilePath;
        }

        private string DatatableToHandsonData(DataTable dt)
        {
            if (dt == null) return "";

            string arrayStr = "[[";
            foreach (DataColumn cols in dt.Columns)
            {
                arrayStr += "'" + cols.ColumnName + "',";
            }
            arrayStr = arrayStr.Substring(0, arrayStr.Length - 1);
            arrayStr += "],";

            foreach (DataRow row in dt.Rows)
            {
                arrayStr += "[";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    arrayStr += "'" + row[i] + "',";
                }
                arrayStr = arrayStr.Substring(0, arrayStr.Length - 1);
                arrayStr += "],";
            }
            arrayStr = arrayStr.Substring(0, arrayStr.Length - 1);
            arrayStr += "]";
            return arrayStr;
        }

        public class CsvItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string DataJson { get; set; }
            public bool hasHistory { get; set; }
        }

        [HttpPost]
        public ActionResult GetHistoryEditInfo(string csvType, string projectGuid, string asOfDate)
        {
            return ActionUtils.Json(() =>
            {
                var type = CommUtils.ParseEnum<EnumCsvType>(csvType);
                var resultList = m_dbAdapter.EditModelCsv.GetByCsvType(type, projectGuid, asOfDate);

                var result = resultList.ConvertAll(x => new
                {
                    commnent = x.Comment,
                    createtime = Toolkit.DateTimeToString(x.CreateTime),
                    title = x.Title,
                    guid = x.EditModelCsvGuid,
                    type = x.Type,
                    createUser = Platform.UserProfile.GetRealName(x.CreateUserName)
                });
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetHistoryByGuid(string projectGuid, string asOfDate, string csvItemId, string guid)
        {
            return ActionUtils.Json(() =>
            {
                var enumCsvType = CommUtils.ParseEnum<EnumCsvType>(csvItemId);
                var path = GetCsvFilePath(projectGuid, asOfDate, csvItemId);
                var historyFilePath = Path.Combine(path, guid + ".csv");
                DataTable historyCsvDt = ExcelUtils.ReadCsv(historyFilePath);
                var prevCsv = m_dbAdapter.EditModelCsv.GetPrevCsvGuid(guid, enumCsvType, projectGuid, asOfDate);
                List<DiffPrev> queryExcept = null;
                if (prevCsv != null)
                {
                    historyFilePath = Path.Combine(path, prevCsv.EditModelCsvGuid + ".csv");
                    DataTable preHandsonDt = ExcelUtils.ReadCsv(historyFilePath);
                    queryExcept = historyCsvDt.AsEnumerable().Except(preHandsonDt.AsEnumerable(), DataRowComparer.Default)
                                .Select(x => new DiffPrev
                                {
                                    RowState = x.RowState.ToString(),
                                    ItemArray = x.ItemArray,

                                }).ToList();
                }
                var handsonData = DatatableToHandsonData(historyCsvDt);

                var result = new
                {
                    handsonData = handsonData,
                    changes = queryExcept
                };


                return ActionUtils.Success(result);
            });
        }

        #endregion

        private void CheckAsOfDate(string asOfDate)
        {
            CommUtils.AssertEquals(asOfDate.Length, 8, "封包日 [" + asOfDate + "] 长度必须为8.");
            CommUtils.Assert(asOfDate.All(Char.IsNumber), "封包日 [" + asOfDate + "] 必须为数字.");
        }

        private void LogEditProduct(EditProductType type, int? projectId, string description, string comment)
        {
            m_dbAdapter.Project.NewEditProductLog(type, projectId, description, comment);
        }
    }
    class DiffPrev
    {
        public object[] ItemArray { get; set; }
        public string RowState { get; set; }
    }
}