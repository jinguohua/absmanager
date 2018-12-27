using System;
using System.Collections.Generic;
using System.Linq;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement
{
    public enum TaskFilterTime
    {
        All = 1,
        Today = 2,
        WeekAhead = 3,
        MonthAhead = 4,
        Future = 5
    }


    public class TaskManager : BaseManager
    {
        public TaskManager() { }

        protected override Loggers.AbstractLogger GetLogger()
        {
            return new Loggers.LoggerGeneric(UserInfo);
        }

        public void AddTaskLog(string shortCode, string comment)
        {
            var task = GetTask(shortCode);
            if (task == null)
            {
                throw new ApplicationException("Can't add task log for task [" + shortCode + "]");
            }
            AddTaskLog(task, comment);
        }

        public void AddTaskLog(Task task, string comment)
        {
            var history = new TaskStatusHistory();
            history.PrevStatusId = task.TaskStatus;
            history.NewStatusId = task.TaskStatus;
            history.TaskId = task.TaskId;
            history.TimeStamp = DateTime.Now;
            history.TimeStampUserName = UserInfo.UserName;
            history.Comment = comment;
            NewTaskStatusHistory(history);
        }

        public void CheckPrevIsFinished(Task task)
        {
            //前置工作是否完成
            foreach (var prevTaskId in task.PrevTaskIdArray)
            {
                var prevTask = GetTask(prevTaskId);
                CommUtils.Assert(prevTask.TaskStatus == TaskStatus.Finished || prevTask.TaskStatus == TaskStatus.Skipped,
                    "前置工作[{0}][{1}]未完成", prevTask.ShortCode, prevTask.Description);
            }
        }

        public bool ChangeTaskStatus(Task task, TaskStatus newTaskStatus, string comment)
        {
            var oldTaskStatus = task.TaskStatus;
            task.TaskStatus = newTaskStatus;
            task.TaskHandler = UserInfo.UserName;

            var recordsCount = UpdateTask(task);

            //New task status history
            var history = new TaskStatusHistory();
            history.PrevStatusId = oldTaskStatus;
            history.NewStatusId = newTaskStatus;
            history.TaskId = task.TaskId;
            history.TimeStamp = DateTime.Now;
            history.TimeStampUserName = UserInfo.UserName;
            history.Comment = comment;
            NewTaskStatusHistory(history);

            return recordsCount == 1;
        }

        public int GenerateTasks(int projectId, List<TemplateTask> templateTasks, List<TemplateTime> templateTimes)
        {
            var onceTimeDict = new Dictionary<string, DateTime>();
            var loopTimeDict = new Dictionary<string, List<DateTime>>();
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
                if (dateList.Count == 1)
                {
                    onceTimeDict[templateTime.TemplateTimeName] = dateList[0];
                }
            }

            var container = new TemplateTaskContainer(templateTasks);
            var sortedTask = container.SortByDependency();

            int moduleId = 9527;

            //Dictionary<TemplateTaskId, List<Tuple<NewTaskId, WorkCode>>>
            var taskDependencyDict = new Dictionary<int, List<Tuple<int, string>>>();

            int totalTaskCount = 0;

            foreach (var templateTask in sortedTask)
            {
                taskDependencyDict[templateTask.TemplateTaskId] = new List<Tuple<int, string>>();
                var newTaskInfo = taskDependencyDict[templateTask.TemplateTaskId];

                DateTime temp;
                bool isDateContainsLetter = !DateTime.TryParse(templateTask.TriggerDate, out temp);

                if (isDateContainsLetter)
                {
                    var newTasks = GenerateLoopTask(loopTimeDict, projectId, moduleId, templateTask, taskDependencyDict);
                    newTaskInfo.AddRange(newTasks.ConvertAll(x => new Tuple<int, string>(x.TaskId, x.ShortCode)));
                    totalTaskCount += newTasks.Count;
                }
                else
                {
                    var newTask = GenerateOnceTask(onceTimeDict, projectId, moduleId, templateTask, taskDependencyDict, -1);
                    newTaskInfo.Add(new Tuple<int, string>(newTask.TaskId, newTask.ShortCode));
                    totalTaskCount += 1;
                }
            }

            return totalTaskCount;
        }

        private Task GenerateOnceTask(Dictionary<string, DateTime> timeDictionary, int projectId, int moduleId, TemplateTask templateTask, Dictionary<int, List<Tuple<int, string>>> taskDependency, int index)
        {
            Task task = new Task();
            task.TemplateTaskId = templateTask.TemplateTaskId;

            task.ProjectId = projectId;
            task.TaskModuleId = moduleId.ToString();
            task.Description = templateTask.TemplateTaskName;
            var realDate = DateUtils.ParseDateSyntax(templateTask.TriggerDate, timeDictionary);
            task.EndTime = realDate;
            if (!string.IsNullOrEmpty(templateTask.BeginDate))
            {
                task.StartTime = DateUtils.ParseDateSyntax(templateTask.BeginDate, timeDictionary);
            }
            task.TaskStatus = (DateTime.Today <= task.EndTime) ? TaskStatus.Waitting : TaskStatus.Overdue;;
            task.TaskDetail = templateTask.TemplateTaskDetail;
            task.TaskTarget = templateTask.TemplateTaskTarget;

            foreach (var prevId in templateTask.PrevIds)
            {
                if (index == -1)
                {
                    var prevTaskIds = taskDependency[prevId].ConvertAll(x => x.Item1.ToString()).ToArray();
                    if (!string.IsNullOrEmpty(task.PreTaskIds))
                    {
                        task.PreTaskIds += CommUtils.Spliter;
                    }
                    task.PreTaskIds += CommUtils.Join(prevTaskIds);
                }
                else
                {
                    var prevTaskId = taskDependency[prevId][index].Item1;
                    if (!string.IsNullOrEmpty(task.PreTaskIds))
                    {
                        task.PreTaskIds += CommUtils.Spliter;
                    }
                    task.PreTaskIds += prevTaskId;
                }
            }

            if (!string.IsNullOrEmpty(templateTask.TemplateTaskExtensionName))
            {
                var extensionType = templateTask.TemplateTaskExtensionName;
                var extension = new TaskExtension{
                    TaskExtensionName = extensionType,
                    TaskExtensionType = extensionType,
                    TaskExtensionStatus = 0
                };
                extension = NewTaskExtension(extension);
                task.TaskExtensionId = extension.TaskExtensionId;
            }

            var newTask = NewTask(task);
            return newTask;
        }

        private List<Task> GenerateLoopTask(Dictionary<string, List<DateTime>> timeDict, int projectId, int moduleId, TemplateTask templateTask, Dictionary<int, List<Tuple<int, string>>> taskDependency)
        {
            var key = DateUtils.ParseDateSyntaxKey(templateTask.TriggerDate);
            if (!timeDict.ContainsKey(key))
            {
                throw new ApplicationException("找不到对应的时间Key[" + key + "]");
            }

            List<Task> newTasks = new List<Task>();

            var timeList = timeDict[key];
            for (int i = 0; i < timeList.Count; ++i)
            {
                var dateTime = timeList[i];
                var time = new Dictionary<string, DateTime> { { key, dateTime } };
                var newTask = GenerateOnceTask(time, projectId, moduleId, templateTask, taskDependency, i);
                newTasks.Add(newTask);
            }

            return newTasks;
        }

        public int GetGenerateTasksCount(int projectId, List<TemplateTask> templateTasks, List<TemplateTime> templateTimes)
        {
            var loopTimeDict = new Dictionary<string, int>();
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
                loopTimeDict[templateTime.TemplateTimeName] = dateList.Count;
            }

            int totalTaskCount = 0;

            foreach (var templateTask in templateTasks)
            {
                DateTime temp;
                bool isDateContainsLetter = !DateTime.TryParse(templateTask.TriggerDate, out temp);
                if (isDateContainsLetter)
                {
                    var key = DateUtils.ParseDateSyntaxKey(templateTask.TriggerDate);
                    if (!loopTimeDict.ContainsKey(key))
                    {
                        throw new ApplicationException("找不到对应的时间Key[" + key + "]");
                    }
                    totalTaskCount += loopTimeDict[key];
                }
                else
                {
                    totalTaskCount += 1;
                }
            }

            return totalTaskCount;
        }


        public bool TaskExists(string shortCode)
        {
            string sql = @"select count(*) from dbo.Tasks
                        where short_code=@0";
            int count = m_db.ExecuteScalar<int>(sql, shortCode);
            return count > 0;
        }

        public int UpdateTask(Task task)
        {
            var taskTable = task.GetTableObject();
            return m_db.Update("Tasks", "task_id", taskTable, task.TaskId);
        }

        public int UpdateTaskExtension(TaskExtension taskExtension)
        {
            var obj = taskExtension.GetTableObject();
            return m_db.Update("TaskExtensions", "task_extension_id", obj, obj.task_extension_id);
        }

        public int UpdateTemplateTask(TemplateTask templateTask)
        {
            var taskTable = templateTask.GetTableObject();
            return m_db.Update("TemplateTask", "template_task_id", taskTable, templateTask.TemplateTaskId);
        }

        public List<TaskStatusHistory> GetTaskStatusHistory(int taskId)
        {
            var items = m_db.Query<ABSMgrConn.TableTaskStatusHistory>(
                "SELECT * FROM dbo.TaskStatusHistory WHERE dbo.TaskStatusHistory.task_id = @0 ORDER BY TIME_STAMP DESC", taskId);

            var result = new List<TaskStatusHistory>();
            foreach (var item in items)
            {
                result.Add(new TaskStatusHistory(item));
            }

            return result;
        }

        public int NewTaskStatusHistory(TaskStatusHistory taskStatusHistory)
        {
            var obj = taskStatusHistory.GetTableObject();
            var taskStatusHistoryId = m_db.Insert("TaskStatusHistory", "task_status_history_id", true, obj);
            return (int)taskStatusHistoryId;
        }

        public int DeleteTasks(List<Task> tasks)
        {
            if (tasks.Count == 0)
            {
                return 0;
            }

            CommUtils.Assert(tasks.Select(x => x.ProjectId).Distinct().Count() == 1,
                "不能一次删除多个产品的工作");

            var taskIds = tasks.Select(x => x.TaskId);
            var sql = "update [dbo].[Tasks] set task_status_id = @taskStatusDeleted"
                + " where task_id in (@taskIds) ";
            
            return m_db.Execute(sql, new { taskStatusDeleted = (int)TaskStatus.Deleted, taskIds = taskIds });
        }

        public void DeleteTask(Task task)
        {
            task.TaskStatus = TaskStatus.Deleted;
            var deleteRows = UpdateTask(task);
            if (deleteRows == 0)
            {
                throw new ApplicationException("Delete task [" + task.ShortCode + "] failed.");
            }
            else if (deleteRows >= 2)
            {
                throw new ApplicationException("Delete " + deleteRows + " tasks [" + task.ShortCode + "]");
            }
        }

        public TaskExtension NewTaskExtension(TaskExtension extension)
        {
            var obj = extension.GetTableObject();
            var taskExtensionId = m_db.Insert("TaskExtensions", "task_extension_id", true, obj);
            extension.TaskExtensionId = (int)taskExtensionId;
            return extension;
        }

        public TemplateTask NewTemplateTask(TemplateTask templateTask)
        {
            var table = templateTask.GetTableObject();
            var id = m_db.Insert("TemplateTask", "template_task_id", true, table);
            templateTask.TemplateTaskId = (int)id;
            return templateTask;
        }

        public Task NewTask(Task task)
        {
            var taskTable = task.GetTableObject();
            taskTable.short_code = NewTaskShortCode();
            taskTable.task_guid = Guid.NewGuid().ToString();

            var taskId = m_db.Insert("Tasks", "task_id", true, taskTable);
            task.TaskId = (int)taskId;
            task.ShortCode = taskTable.short_code;
            return task;
        }

        public Template AddNewTemplate(Template template)
        {
            var templateTable = template.GetTableObject();
            var templateId = m_db.Insert("Template", "template_id", true,templateTable);
            template.TemplateId = (int)templateId;
            return template;
        }

        public List<Template> GetTemplateName()
        {
            var templateNames = m_db.Query<ABSMgrConn.TableTemplate>(
                "SELECT template_name FROM dbo.Template");

            return templateNames.ToList().ConvertAll(item => new Template(item)).ToList();
        }

        public List<Task> GetTasks(IEnumerable<string> shortCodes, bool withExInfo = false)
        {
            int taskStatusDeleted = (int)TaskStatus.Deleted;

            if (shortCodes == null || shortCodes.Count() == 0)
            {
                return new List<Task>();
            }

            var tableTasks = m_db.Fetch<ABSMgrConn.TableTasks>(
                "SELECT * FROM dbo.Tasks Where short_code in (@shortCodes) AND task_status_id <> @taskStatusDeleted" + m_orderBy,
                new { shortCodes, taskStatusDeleted });

            var tasks = tableTasks.ConvertAll(item => new Task(item));
            if (withExInfo)
            {
                tasks = GetTaskExtensions(tasks);
            }

            return tasks;
        }

        public List<Task> GetByTaskGroupId(int taskGroupId, bool withExInfo = false)
        {
            int taskStatusDeleted = (int)TaskStatus.Deleted;
            var tableTasks = m_db.Fetch<ABSMgrConn.TableTasks>(
                "SELECT * FROM dbo.Tasks Where task_group_id = @taskGroupId AND task_status_id <> @taskStatusDeleted" + m_orderBy,
                new { taskGroupId, taskStatusDeleted });

            var tasks = tableTasks.ConvertAll(item => new Task(item));
            if (withExInfo)
            {
                tasks = GetTaskExtensions(tasks);
            }

            return tasks;
        }

        public List<Task> GetTaskExtensions(List<Task> tasks)
        {
            if (tasks.Count != 0)
            {
                var taskExIds = tasks.Where(x => x.TaskExtensionId.HasValue).Select(x => x.TaskExtensionId.Value);
                var taskExs = GetTaskExtensions(taskExIds);
                var taskExCache = new Dictionary<int, TaskExtension>();
                taskExs.ForEach(x => taskExCache[x.TaskExtensionId] = x);
                tasks.ForEach(x =>
                {
                    if (x.TaskExtensionId.HasValue)
                    {
                        x.TaskExtension = taskExCache[x.TaskExtensionId.Value];
                    }
                });
            }

            return tasks;
        }

        public List<Task> GetTasksByProjectId(int projectId, bool withExInfo = false)
        {
            var tableTasks = m_db.Query<ABSMgrConn.TableTasks>(
                "SELECT * FROM dbo.Tasks Where project_id = @0 AND task_status_id <> @1" + m_orderBy,
                projectId, (int)TaskStatus.Deleted);

            var tasks = tableTasks.ToList().ConvertAll(item => new Task(item)).ToList();
            if (withExInfo)
            {
                tasks = GetTaskExtensions(tasks);
            }

            return tasks;
        }

        public List<Task> GetTasksByPersonInCharge(string personInCharge)
        {
            int taskStatusDeleted = (int)TaskStatus.Deleted;
            var tableTasks = m_db.Fetch<ABSMgrConn.TableTasks>(
                "SELECT * FROM dbo.Tasks Where person_in_charge = @0 AND task_status_id <> @1" + m_orderBy,
                personInCharge, taskStatusDeleted);

            return tableTasks.ConvertAll(item => new Task(item));
        }

        public List<string> GetShortCodesByProjectIds(IEnumerable<int> projectIds)
        {
            var taskStatusDeleted = (int)TaskStatus.Deleted;

            if (projectIds.Count() == 0)
            {
                return new List<string>();
            }

            var tableTasks = m_db.Query<ABSMgrConn.TableTasks>(
                "SELECT * FROM dbo.Tasks Where project_id IN (@projectIds) AND task_status_id <> @taskStatusDeleted" + m_orderBy,
                new { projectIds, taskStatusDeleted });

            return tableTasks.Select(x => x.short_code).ToList();
        }

        public List<Task> GetDerivedTasksByProjectId(int projectId, int templateTaskId)
        {
            var tasks = m_db.Query<ABSMgrConn.TableTasks>(
                "SELECT * FROM dbo.Tasks Where project_id = @0 AND template_task_id = @1 AND task_status_id <> @2" + m_orderBy,
                projectId, templateTaskId, (int)TaskStatus.Deleted);
            return tasks.ToList().ConvertAll(item => new Task(item)).ToList();
        }

        public Page<Task> GetTasks(long pageNum, long itemsPerPage, DateTime? endTime,
            int projectId, List<TaskStatus> taskStatusList, List<int> authorizedProjectIds,
            string upperLimitDate = "9999-12-31", string lowerLimitDate = "1753-01-02", string paymentDay = "1753-01-02")
        {
            if (authorizedProjectIds.Count == 0 || taskStatusList.Count == 0)
            {
                return new Page<Task>
                { 
                    Items = new List<Task>()
                };
            }

            var sql = "SELECT * FROM dbo.Tasks";
            var authrizedCondition = "(" + string.Join(", ", authorizedProjectIds.ConvertAll(x => x.ToString())) + ")";
            sql += " WHERE project_id IN " + authrizedCondition + " AND ";

            List<string> sqlCondition = new List<string>();
            List<object> param = new List<object>();

            var taskStatusCondition = "(" + string.Join(", ", taskStatusList.ConvertAll(x => (int)x)) + ")";
            sql += " task_status_id IN " + taskStatusCondition + " AND ";

            if (projectId != -1)
            {
                sqlCondition.Add(" project_id = @" + sqlCondition.Count);
                param.Add(projectId);
            }

            if (endTime.HasValue)
            {
                sqlCondition.Add(" end_time >= @" + sqlCondition.Count);
                param.Add(DateTime.Today);

                sqlCondition.Add(" end_time <= @" + sqlCondition.Count);
                param.Add(endTime.Value);
            }

            sqlCondition.Add(" end_time > @" + sqlCondition.Count);
            param.Add(DateTime.Parse(lowerLimitDate));

            sqlCondition.Add(" end_time <= @" + sqlCondition.Count);
            param.Add(DateTime.Parse(upperLimitDate));

            sqlCondition.Add(" task_status_id <> @" + sqlCondition.Count);
            param.Add((int)TaskStatus.Deleted);

            if (projectId != -1 && !string.IsNullOrWhiteSpace(paymentDay) && paymentDay != "1753-01-02")
            {
                sqlCondition.Add(" not exists (select short_code from dbo.TaskPeriod where project_id = @" + sqlCondition.Count);
                param.Add(projectId);

                sqlCondition.Add(" payment_date <> @" + sqlCondition.Count);
                param.Add(DateTime.Parse(paymentDay));

                sqlCondition.Add(" short_code = dbo.Tasks.short_code) " +
                    "or short_code in (select short_code from dbo.TaskPeriod where project_id = @" + sqlCondition.Count);
                param.Add(projectId);

                sqlCondition.Add(" payment_date = @" + sqlCondition.Count + ")");
                param.Add(DateTime.Parse(paymentDay));
            }

            sql += string.Join(" AND ", sqlCondition.ToArray());
            sql += m_orderBy;

            PetaPoco.Page<ABSMgrConn.TableTasks> page = null;
            if (param.Count == 0)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql);
            }
            else if (param.Count == 1)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0]);
            }
            else if (param.Count == 2)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1]);
            }
            else if (param.Count == 3)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2]);
            }
            else if (param.Count == 4)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3]);
            }
            else if (param.Count == 5)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3], param[4]);
            }
            else if (param.Count == 6)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3], param[4], param[5]);
            }
            else if (param.Count == 7)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3], param[4], param[5], param[6]);
            }
            else if (param.Count == 8)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3], param[4], param[5], param[6], param[7]);
            }
            else if (param.Count == 9)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3], param[4], param[5], param[6], param[7], param[8]);
            }
            else if (param.Count == 10)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3], param[4], param[5], param[6], param[7], param[8], param[9]);
            }
            else if (param.Count == 11)
            {
                page = m_db.Page<ABSMgrConn.TableTasks>(pageNum, itemsPerPage, sql, param[0], param[1], param[2], param[3], param[4], param[5], param[6], param[7], param[8], param[9], param[10]);
            }
            else
            {
                throw new ApplicationException("GetTasks failed. Parameters' count is incorrect.");
            }

            var tasks = new Page<Task>().Parse(page);
            tasks.Items = page.Items.ConvertAll(item => new Task(item)).ToList();

            return tasks;
        }

        public Task GetTask(int id)
        {
            var tasks = m_db.Query<ABSMgrConn.TableTasks>(
                "SELECT * FROM dbo.Tasks WHERE dbo.Tasks.task_id = @0 AND task_status_id <> @1" + m_orderBy, id, (int)TaskStatus.Deleted);

            if (tasks.Count() != 1)
            {
                throw new ApplicationException("Get task [" + id + "] failed.");
            }

            var task = new Task(tasks.Single());
            return task;
        }

        public Task GetTask(string shortCode, bool ignoreDeletedTask = true)
        {
            IEnumerable<ABSMgrConn.TableTasks> tasks = null;
            if (ignoreDeletedTask)
            {
                tasks = m_db.Query<ABSMgrConn.TableTasks>(
                    "SELECT * FROM dbo.Tasks WHERE dbo.Tasks.short_code = @0 AND task_status_id <> @1" + m_orderBy, shortCode, (int)TaskStatus.Deleted);
            }
            else
            {
                tasks = m_db.Query<ABSMgrConn.TableTasks>(
                    "SELECT * FROM dbo.Tasks WHERE dbo.Tasks.short_code = @0" + m_orderBy, shortCode);
            }

            if (tasks.Count() != 1)
            {
                throw new ApplicationException("Get task [" + shortCode + "] failed.");
            }

            var task = new Task(tasks.Single());
            return task;
        }

        public bool Exists(string shortCode)
        {
            var tasks = GetTasks(new[] { shortCode });
            return tasks.Count > 0;
        }

        public Task GetTaskWithExInfo(string shortCode)
        {
            var task = GetTask(shortCode);
            if (!task.TaskExtensionId.HasValue)
            {
                throw new ApplicationException("Task [" + shortCode + "] doesn't have extension info.");
            }

            task.TaskExtension = GetTaskExtension(task.TaskExtensionId.Value);
            return task;
        }

        /// <summary>
        /// 递归获取任务的所有后续任务
        /// </summary>
        /// <param name="shortCode"></param>
        /// <returns></returns>
        public List<Task> GetSubsequentTasksRecursively(string shortCode)
        {
            var task = GetTask(shortCode);
            //所有task
            var tasks = GetTasksByProjectId(task.ProjectId);
            //所有有前置任务的task
            tasks = tasks.Where(x => x.PrevTaskIdArray != null && x.PrevTaskIdArray.Count > 0).ToList();

            var taskIds = new List<int>();
            taskIds.Add(task.TaskId);

            while (true)
            {
                var prevTasks = tasks.Where(x => x.PrevTaskIdArray.Intersect(taskIds).Count() > 0);
                if (prevTasks.Count() == 0)
                {
                    break;
                }

                var taskIdCount = taskIds.Count();
                taskIds.AddRange(prevTasks.ToList().ConvertAll(x => x.TaskId));
                taskIds = taskIds.Distinct().ToList();
                if (taskIds.Count == taskIdCount)
                {
                    //一次查询后，未发现后续任务
                    break;
                }
            }

            var subsequentTasks = new List<Task>();
            taskIds.RemoveAt(0);
            taskIds.ForEach(x => subsequentTasks.Add(GetTask(x)));
            return subsequentTasks;
        }

        public int SaveTaskExtension(TaskExtension obj)
        {
            var taskExtension = obj.GetTableObject();
            return m_db.Update("TaskExtensions", "task_extension_id", taskExtension, taskExtension.task_extension_id);
        }

        public List<TaskExtension> GetTaskExtensions(IEnumerable<int> taskExIds)
        {
            if (taskExIds == null || taskExIds.Count() == 0)
            {
                return new List<TaskExtension>();
            }

            var taskExs = m_db.Fetch<ABSMgrConn.TableTaskExtensions>(
                "SELECT * FROM dbo.TaskExtensions WHERE dbo.TaskExtensions.task_extension_id IN (@taskExIds)", new { taskExIds });

            return taskExs.ConvertAll(x => new TaskExtension(x));
        }

        public TaskExtension GetTaskExtension(int taskExtensionId)
        {
            var taskExtensions = m_db.Query<ABSMgrConn.TableTaskExtensions>(
                "SELECT * FROM dbo.TaskExtensions WHERE dbo.TaskExtensions.task_extension_id = @0", taskExtensionId);

            if (taskExtensions.Count() != 1)
            {
                throw new ApplicationException("Get taskExtension [" + taskExtensionId + "] failed.");
            }

            var taskExtension = new TaskExtension(taskExtensions.Single());
            return taskExtension;
        }
        
        private string NewTaskShortCode()
        {
            var shortCode = string.Empty;
            do
            {
                shortCode = ShortCodeUtils.Random();
            } while (TaskExists(shortCode));

            return shortCode;
        }

        public Dictionary<int, List<Task>> GetTasksByMetaTaskId(IEnumerable<int> prevMetaTaskIds)
        {
            int taskStatusDeleted = (int)TaskStatus.Deleted;

            if (prevMetaTaskIds == null || prevMetaTaskIds.Count() == 0)
            {
                return new Dictionary<int, List<Task>>();
            }

            var records = m_db.Query<ABSMgrConn.TableTasks>(
                    "SELECT * FROM dbo.Tasks WHERE meta_task_id in (@prevMetaTaskIds) AND task_status_id <> @taskStatusDeleted",
                    new { prevMetaTaskIds, taskStatusDeleted });

            var tasks = records.ToList().ConvertAll(x => new Task(x));
            var dictTask = tasks.GroupBy(x => x.MetaTaskId.Value).ToDictionary(x => x.Key, y => y.ToList());
            return dictTask;
        }

        private const string m_orderBy = " ORDER BY dbo.Tasks.end_time, dbo.Tasks.project_id desc, dbo.Tasks.task_id";
    }
}
