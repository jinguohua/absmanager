using ChineseAbs.ABSManagement;
using ChineseAbs.ABSManagement.LogicModels;
using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagement.Utils.Excel;
using ChineseAbs.ABSManagement.Utils.Excel.Structure;
using ChineseAbs.ABSManagementSite.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class TaskGroupController : BaseController
    {
        [HttpPost]
        public ActionResult GetTaskGroupList(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeries = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeries.CurrentProject.Instance;
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var taskGroups = projectSeries.CurrentProject.TaskGroups;
                var allPermissionUid = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.TaskGroup, PermissionType.Read).ToDictionary(x => x);
                var hasPermissionTaskGroups = taskGroups.Where(x => allPermissionUid.ContainsKey(x.Instance.Guid)).ToList();

                var result = hasPermissionTaskGroups.ConvertAll(x => new
                {
                    guid = x.Instance.Guid,
                    name = x.Instance.Name,
                    projectGuid = projectSeries.CurrentProject.Instance.ProjectGuid,
                    description = x.Instance.Description,
                    sequence = x.Instance.Sequence,
                    percentCompleted = CommUtils.Percent(x.Tasks.Count(task => task.TaskStatus == TaskStatus.Finished), x.Tasks.Count, 0),
                    finishedTaskCount = x.Tasks.Count(task => task.TaskStatus == TaskStatus.Finished),
                    taskCount = x.Tasks.Count,
                    createUserName = x.Instance.CreateUserName,
                    createTimeStamp = Toolkit.DateTimeToString(x.Instance.CreateTime)
                });

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult ResetSequence(string projectSeriesGuid, string orderedTaskGroupGuids)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeries = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeries.CurrentProject.Instance;
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Write);

                var taskGroups = projectSeries.CurrentProject.TaskGroups;

                var oldGuids = taskGroups.Select(x => x.Instance.Guid).ToArray();
                var newGuids = CommUtils.Split(orderedTaskGroupGuids);
                CommUtils.Assert(CommUtils.IsEqual(oldGuids, newGuids), "当前工作组和服务器上不一致，请刷新后再试");

                var oldSequences = taskGroups.Select(x => x.Instance.Sequence.Value).ToList();
                taskGroups.ForEach(x => x.Instance.Sequence = oldSequences[Array.IndexOf(newGuids, x.Instance.Guid)]);

                var newTaskGroups = new List<TaskGroup>();
                for (int i = 0; i < taskGroups.Count(); i++)
                {
                    var taskGroup = taskGroups[i].Instance;
                    if (taskGroup.Sequence != oldSequences[i])
                    {
                        CheckPermission(PermissionObjectType.TaskGroup, taskGroup.Guid, PermissionType.Write);
                        newTaskGroups.Add(taskGroups[i].Instance);
                    }
                }

                newTaskGroups.ForEach(x => m_dbAdapter.TaskGroup.UpdateTaskGroup(x));

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult NewTaskGroup(string projectSeriesGuid, string name, string description)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeries = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeries.CurrentProject.Instance;
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Write);

                var newTaskGroup = projectSeries.CurrentProject.NewTaskGroup(projectSeriesGuid, name, description).Instance;
                return ActionUtils.Success(newTaskGroup.Guid);
            });
        }


        [HttpPost]
        public ActionResult ImportTable(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeriesLogicModel = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeriesLogicModel.CurrentProject.Instance;
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Write);

                var file = Request.Files[0];
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, bytes.Length);
                file.InputStream.Seek(0, SeekOrigin.Begin);

                Stream newStream = new MemoryStream(bytes);
                var tableHeaderRowsCount = ExcelUtils.GetTableHeaderRowsCount(newStream, 0, 0, tableHeader);
                var table = ExcelUtils.ParseExcel(file.InputStream, 0, tableHeaderRowsCount, 0, 9);

                var validation = new ExcelValidation();
                validation.Add(CellRange.Column(0), new CellTextValidation(1, 30));
                validation.Add(CellRange.Column(1), new CellTextValidation(1, 500));
                validation.Add(CellRange.Column(2), new CellTextValidation(1, 30));
                validation.Add(CellRange.Column(3), new CellDateValidation());
                validation.Add(CellRange.Column(4), new CellDateValidation(false));
                //检查工作状态
                Func<string, string> checkTaskStatus = (cellText) =>
                {
                   
                    if(!string.IsNullOrWhiteSpace(cellText) && cellText != "-" && cellText != TranslateUtils.ToCnString(TaskStatus.Waitting) 
                        && cellText != TranslateUtils.ToCnString(TaskStatus.Overdue))
                    {
                        return "导入工作的工作状态只能为空、[等待]或[逾期]";
                    }
                    return string.Empty;
                };
                validation.Add(CellRange.Column(5), new CellCustomValidation(checkTaskStatus));
                //检查负责人
                int projectId = project.ProjectId;
                var personInCharges = GetPersonInCharges(projectSeriesLogicModel, projectId);
                Func<List<string>, string, string> checkPersonInCharge = (personInChargeList, cellText) =>
                {
                    var userName = GetPersonInCharge(cellText);
                    if (!personInChargeList.Contains(userName))
                    {
                        return "负责人[" + cellText + "]不存在";
                    }
                    return string.Empty;
                };
                validation.Add(CellRange.Column(6), new CellCustomValidation(checkPersonInCharge, personInCharges));
                //不检查只有taskGroup列，其他列均为空的行
                var columns = new List<CellRange>() { CellRange.Column(2), CellRange.Column(3), CellRange.Column(4), 
                    CellRange.Column(5), CellRange.Column(6), CellRange.Column(7), CellRange.Column(8)};
                Func<string, bool> ExceptCondition = (x) =>
                {
                    return x != string.Empty;
                };
                validation.JudgeColumn = new ColumnJudge(columns, ExceptCondition);
                //检查开始时间是否大于截止时间
                Func<object, object, string> lessThanOrEqualTo = (left, right) =>
                {
                    var leftText = left.ToString();
                    var rightText = right.ToString();
                    if (!string.IsNullOrWhiteSpace(leftText) && leftText != "-")
                    {
                        var startTime = DateTime.Parse(leftText);
                        var endTime = DateTime.Parse(rightText);
                        if (startTime > endTime)
                        {
                            return "开始时间[" + leftText + "]不能大于结束时间[" + rightText + "]";
                        }
                    }
                    return string.Empty;
                };
                validation.CompareColumn = new ColumnComparison(CellRange.Column(3), CellRange.Column(4), lessThanOrEqualTo);
                validation.Check(table, tableHeaderRowsCount);
                
                //创建taskGroups
                List<TaskGroup> newTaskGroups = new List<TaskGroup>();
                for (int iRow = 0; iRow < table.Count; iRow++)
                {
                    var row = table[iRow];
                    var taskGroup = ParseTaskGroups(newTaskGroups, row, iRow, projectId);
                    if (taskGroup != null)
                    {
                        newTaskGroups.Add(taskGroup);
                    }
                }

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, project);
                var permissionLogicModel = new PermissionLogicModel(projectLogicModel);
                var projectSeries = projectSeriesLogicModel.Instance;

                newTaskGroups.ForEach(x => {
                    var taskGroup = m_dbAdapter.TaskGroup.NewTaskGroup(x.ProjectId, x.Name, x.Description);
                    permissionLogicModel.SetPermission(projectSeries, project, taskGroup.Guid, PermissionObjectType.TaskGroup);
                    projectLogicModel.Activity.Add(x.ProjectId, ActivityObjectType.TaskGroup, taskGroup.Guid, "新建工作组：" + taskGroup.Name);
                });

                //创建tasks
                var taskGroups = m_dbAdapter.TaskGroup.GetByProjectId(projectId);
                var dicTaskGroups = taskGroups.ToDictionary(x => x.Name);

                List<Task> tasks = new List<Task>();
                for (int iRow = 0; iRow < table.Count; iRow++)
                {
                    var row = table[iRow].ToList();
                    if (row.Count > 2 && (!string.IsNullOrWhiteSpace(row[2].ToString())) && row[2].ToString() != "-")
                    {
                        var taskGroup = dicTaskGroups[row[0].ToString()];
                        tasks.Add(ParseTasks(row, iRow, taskGroup.Id, project));
                    }
                }
                tasks.ForEach(x => m_dbAdapter.Task.NewTask(x));

                Dictionary<string, string> msgDic = new Dictionary<string, string>();
                foreach (var task in tasks)
                {
                    var msg = string.Format("创建task:shortCode={0};name={1};startTime={2};endTime={3};personInCharge={4};"
                        + "taskStatus={5};taskTarget={6};taskDetail={7};projectId={8};projectName={9};taskGroupId={10}",
                        task.ShortCode, task.Description, task.StartTime, task.EndTime, task.PersonInCharge, task.TaskStatus,
                        task.TaskTarget, task.TaskDetail, task.ProjectId, task.ProjectName, task.TaskGroupId);
                    msgDic[task.ShortCode] = msg;
                }

                tasks.ForEach(x =>{
                    permissionLogicModel.SetPermission(projectSeries, project, x.ShortCode, PermissionObjectType.Task);
                    projectLogicModel.Activity.Add(projectId, ActivityObjectType.Task, x.ShortCode, "创建工作：" + x.Description);
                    m_dbAdapter.Project.NewEditProductLog(EditProductType.CreateTask, x.ProjectId, msgDic[x.ShortCode], "");
                });

                var result = new { 
                    taskGroupCount = newTaskGroups.Count,
                    taskCount = tasks.Count
                };
                return ActionUtils.Success(result);
            });
        }

        private TaskGroup ParseTaskGroups(List<TaskGroup> taskGroups, List<object> objs, int index, int projectId)
        {
            var name = objs[0].ToString();
            var description = objs[1].ToString();
            var currentTaskGroups = m_dbAdapter.TaskGroup.GetByProjectId(projectId);
            if (!currentTaskGroups.Any(x => x.Name == name) && (!taskGroups.Any(x => x.Name == name)))
            {
                var taskGroup = new TaskGroup();
                taskGroup.ProjectId = projectId;
                taskGroup.Name = name;
                taskGroup.Description = description;
                return taskGroup;
            }
            else 
            {
                return null;
            }
        }

        private Task ParseTasks(List<object> objs, int index, int taskGroupId, Project project)
        {
            var name = objs[2].ToString();
            var startTime = objs[3].ToString();
            var endTime = objs[4].ToString();
            var taskStatus = objs[5].ToString();
            var personInCharge = objs[6].ToString();
            var taskTarget = objs[7].ToString();
            var taskDetail = objs[8].ToString();

            var task = new Task();
            task.ProjectId = project.ProjectId;
            task.Description = name;
            task.ProjectName = project.Name;
            task.StartTime = DateUtils.Parse(startTime);
            task.EndTime = DateUtils.Parse(endTime);
            task.PersonInCharge = GetPersonInCharge(personInCharge);
            task.TaskStatus = GetTaskStatus(task.EndTime.Value, taskStatus);
            task.TaskGroupId = taskGroupId;
            task.TaskTarget = taskTarget;
            task.TaskDetail = taskDetail;

            return task;
        }

        private TaskStatus GetTaskStatus(DateTime endTime, string taskStatus)
        {
            if (string.IsNullOrWhiteSpace(taskStatus) || taskStatus == "-")
            {
                if (DateTime.Today <= endTime)
                {
                    return TaskStatus.Waitting;
                }
            }
            else if (taskStatus == TranslateUtils.ToCnString(TaskStatus.Waitting))
            {
                return TaskStatus.Waitting;
            }
            return TaskStatus.Overdue;
        }

        private string GetPersonInCharge(string personInCharge)
        {
            if (!string.IsNullOrWhiteSpace(personInCharge) && personInCharge != "-")
            {
                var begin = 0;
                var end = personInCharge.Length - 1;
                if (personInCharge.IndexOf("(") != -1 && personInCharge.IndexOf(")") == personInCharge.Length - 1)
                {
                    begin = personInCharge.IndexOf("(") + 1;
                }
                else if (personInCharge.IndexOf("（") != -1 && personInCharge.IndexOf("）") == personInCharge.Length - 1)
                {
                    begin = personInCharge.IndexOf("（") + 1;
                }
                return personInCharge.Substring(begin, end - begin);
            }
            return personInCharge;
        }

        private List<string> GetPersonInCharges(ProjectSeriesLogicModel projectSeriesLogicModel, int projectId)
        {
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

            return allUserNames;
        }

        [HttpPost]
        public ActionResult ExportTable(string projectSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var projectSeries = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                var project = projectSeries.CurrentProject.Instance;
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var taskGroups = projectSeries.CurrentProject.TaskGroups;
                var allPermissionTaskGroupUid = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.TaskGroup, PermissionType.Read).ToDictionary(x => x);
                var hasPermissionTaskGroups = taskGroups.Where(x => allPermissionTaskGroupUid.ContainsKey(x.Instance.Guid)).ToList();

                var tableDic = new Dictionary<TaskGroup, List<Task>>();
                var tasks = new List<Task>();

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, project);
                hasPermissionTaskGroups.ForEach(x => {
                    var taskGroupLogicModel = projectLogicModel.TaskGroups.Single(y => y.Instance.Guid == x.Instance.Guid);
                    var allPermissionTaskUid = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.Task, PermissionType.Read).ToDictionary(y => y);
                    var hasPermissionTasks = taskGroupLogicModel.Tasks.Where(y => allPermissionTaskUid.ContainsKey(y.ShortCode)).ToList();
                    tableDic.Add(x.Instance, hasPermissionTasks);
                    tasks.AddRange(hasPermissionTasks);
                });

                var personInCharges = tasks.Where(x => !string.IsNullOrWhiteSpace(x.PersonInCharge)).Select(x => x.PersonInCharge);
                Platform.UserProfile.Precache(personInCharges);

                var table = new DataTable();
                table.Columns.Add("工作组名称", typeof(System.String));
                table.Columns.Add("工作组描述", typeof(System.String));
                table.Columns.Add("工作名称", typeof(System.String));
                table.Columns.Add("开始时间", typeof(System.String));
                table.Columns.Add("截止时间", typeof(System.String));
                table.Columns.Add("工作状态", typeof(System.String));
                table.Columns.Add("负责人", typeof(System.String));
                table.Columns.Add("工作目标", typeof(System.String));
                table.Columns.Add("工作描述", typeof(System.String));
                foreach (var taskGroupName in tableDic.Keys)
                {
                    if (tableDic[taskGroupName].Count == 0)
                    {
                        var row = table.NewRow();
                        row["工作组名称"] = taskGroupName.Name;
                        row["工作组描述"] = taskGroupName.Description;
                        row["工作名称"] = string.Empty;
                        row["开始时间"] = string.Empty;
                        row["截止时间"] = string.Empty;
                        row["工作状态"] = string.Empty;
                        row["负责人"] = string.Empty;
                        row["工作目标"] = string.Empty;
                        row["工作描述"] = string.Empty;
                        table.Rows.Add(row);
                    }
                    else
                    {
                        foreach (var task in tableDic[taskGroupName])
                        {
                            var row = table.NewRow();
                            row["工作组名称"] = taskGroupName.Name;
                            row["工作组描述"] = taskGroupName.Description;
                            row["工作名称"] = task.Description;
                            row["开始时间"] = Toolkit.DateToString(task.StartTime);
                            row["截止时间"] = Toolkit.DateToString(task.EndTime);
                            row["工作状态"] = string.IsNullOrWhiteSpace(task.TaskStatus.ToString()) || task.TaskStatus.ToString() == "-"? task.TaskStatus.ToString()
                                : TranslateUtils.ToCnString(CommUtils.ParseEnum<TaskStatus>(task.TaskStatus.ToString()));
                            row["负责人"] = string.IsNullOrWhiteSpace(task.PersonInCharge) ? string.Empty : 
                                Platform.UserProfile.Get(task.PersonInCharge).RealName +　"(" + Platform.UserProfile.Get(task.PersonInCharge).UserName + ")";
                            row["工作目标"] = task.TaskTarget;
                            row["工作描述"] = task.TaskDetail;
                            table.Rows.Add(row);
                        }
                    }
                }

                var taskStatusIndex = table.Columns.IndexOf("工作状态");

                var sheet = new Sheet("TaskGroupTable", table);
                sheet.SheetStyle.Add(new CellRangeStyle() { Range = CellRange.Column(taskStatusIndex), Style = new CellRangeStyleTaskStatus(), Rule = CellRangeRule.IgnoreEmptyCell });
                var workbook = new Workbook("TaskGroupTable", sheet);
                var resource = workbook.ToExcel(CurrentUserName);

                return ActionUtils.Success(resource.Guid);
            });
        }

        [HttpPost]
        public ActionResult GetTasks(string taskGroupGuid)
        {
            return ActionUtils.Json(() =>
            {
                var taskGroup = m_dbAdapter.TaskGroup.GetByGuid(taskGroupGuid);
                CheckPermission(PermissionObjectType.TaskGroup, taskGroupGuid, PermissionType.Read);

                var projectLogicModel = new ProjectLogicModel(CurrentUserName, taskGroup.ProjectId);
                var taskGroupLogicModel = projectLogicModel.TaskGroups.Single(x => x.Instance.Guid == taskGroupGuid);

                Platform.UserProfile.Precache(taskGroupLogicModel.Tasks.Select(x => x.PersonInCharge));

                var allPermissionUid = m_dbAdapter.Permission.GetObjectUids(CurrentUserName, PermissionObjectType.Task, PermissionType.Read).ToDictionary(x => x);
                var hasPermissionTasks = taskGroupLogicModel.Tasks.Where(x => allPermissionUid.ContainsKey(x.ShortCode)).ToList();
                var permissionList = m_dbAdapter.Permission.GetByObjectUid(hasPermissionTasks.Select(x => x.ShortCode)).Where(x=>x.UserName==CurrentUserName);

                var result = hasPermissionTasks.ConvertAll(x =>
                    new
                    {
                        shortCode = x.ShortCode,
                        taskName = x.Description,
                        beginTime = Toolkit.DateToString(x.StartTime),
                        endTime = Toolkit.DateToString(x.EndTime),
                        status = x.TaskStatus.ToString(),
                        personInCharge = x.PersonInCharge,
                        personInChargeUserProfile = Platform.UserProfile.Get(x.PersonInCharge),
                        target = x.TaskTarget,
                        detail = x.TaskDetail,
                        reminderInfo = m_dbAdapter.MessageReminding.GetResultByUid(x.ShortCode),
                        taskExType = x.TaskExtensionId.HasValue ? x.TaskExtension.TaskExtensionType.ToString() : string.Empty,
                        permission = permissionList.Where(p => p.ObjectUniqueIdentifier == x.ShortCode).Select(p =>
                         p.Type.ToString())
                    }
                );

                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult RemoveTaskGroup(string projectSeriesGuid, string taskGroupGuid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.TaskGroup, taskGroupGuid, PermissionType.Write);

                var projectSeries = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                projectSeries.CurrentProject.RemoveTaskGroup(taskGroupGuid);
                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult ModifyTaskGroup(string projectSeriesGuid, string taskGroupGuid, string name, string description)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.TaskGroup, taskGroupGuid, PermissionType.Write);

                var projectSeries = new ProjectSeriesLogicModel(CurrentUserName, projectSeriesGuid);
                projectSeries.CurrentProject.ModifyTaskGroup(taskGroupGuid, name, description);
                return ActionUtils.Success(1);
            });
        }

        private readonly List<List<string>> tableHeader = new List<List<string>>() {new List<string>() {"工作组名称","工作组描述","工作名称","开始时间","截止时间",
            "工作状态", "负责人", "工作目标","工作描述"}};
    }
}
