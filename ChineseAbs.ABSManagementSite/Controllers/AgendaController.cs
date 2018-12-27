using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class AgendaController : BaseController
    {
        [HttpPost]
        public ActionResult GetAgendas(string projectGuid, string startDate, string endDate)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到ProjectSeries[projectGuid={0}]信息", projectGuid);
                var agendas = m_dbAdapter.Agenda.GetAgendasByProjectId(project.ProjectId, startDate, endDate);

                Platform.UserProfile.Precache(agendas.Select(x => x.CreateUserName));

                var permissionList = m_dbAdapter.Permission.GetByObjectUid(projectGuid).Where(x => x.UserName == CurrentUserName);

                var result = agendas.ConvertAll(x =>
                {
                    var createManInfo = Platform.UserProfile.Get(x.CreateUserName);
                    return new CalendarData
                    {
                        guid = x.Guid,
                        id = x.Id,
                        title = x.Name,
                        desc = x.Description,
                        start = Toolkit.DateTimeToString(x.StartTime),
                        end = Toolkit.DateTimeToString(x.EndTime),
                        createMan = createManInfo.RealName + "(" + createManInfo.UserName + ")",
                        createTime = x.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        backgroundColor = x.AgendaStatus == AgendaStatus.Running ? "#3995cd"
                                : x.AgendaStatus == AgendaStatus.Finished ? "#55FF55" : "#ff4a4a",
                        reminderInfo = m_dbAdapter.MessageReminding.GetResultByUid(x.Guid),
                        permission = permissionList.Select(p => p.Type.ToString())
                    };
                });
                return ActionUtils.Success(result);
            });
        }

        private void CheckPermission(string projectGuid)
        {
            CommUtils.Assert(m_dbAdapter.Permission.GetByObjectUid(projectGuid).Where(x => x.UserName == CurrentUserName).ToList().Exists(x => x.Type == PermissionType.Write || x.Type == PermissionType.Execute), "用户{0}没有当前日程的[读、执行]权限，无法创建、修改、删除日程", CurrentUserName);
        }

        [HttpPost]
        public ActionResult NewAgenda(string projectGuid, string agendaName, string description, string startTime, string endTime)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(projectGuid);

                ValidateUtils.Name(agendaName, "日程名称");
                ValidateUtils.Description(description, "日程描述");

                CommUtils.Assert(!((string.IsNullOrWhiteSpace(startTime) || startTime == "-")
                    && (string.IsNullOrWhiteSpace(endTime) || endTime == "-")), "开始时间和结束时间不能同时为空");

                CommUtils.Assert(DateUtils.IsNullableDate(startTime), "开始时间必须为[YYYY-MM-DD]格式或者为空");
                CommUtils.Assert(DateUtils.IsNullableDate(endTime), "结束时间必须为[YYYY-MM-DD]格式或者为空");

                var taskStartTime = DateUtils.Parse(startTime);
                var taskEndTime = DateUtils.Parse(endTime);
                if (taskStartTime != null && taskEndTime != null)
                {
                    CommUtils.Assert(taskEndTime.Value >= taskStartTime.Value,
                        "开始时间[{0}]不能大于结束时间[{1}]", startTime, endTime);
                }

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到ProjectSeries[projectGuid={0}]信息", projectGuid);

                var agenda = new Agenda();
                agenda.CreateUserName = CurrentUserName;
                agenda.Name = agendaName;
                agenda.Description = description;
                CommUtils.Assert(!(startTime == "" && endTime == ""), "开始和结束时间必须至少有一个不为空！");
                if (startTime == "" && endTime != "")
                {
                    agenda.EndTime = Convert.ToDateTime(endTime);
                    agenda.StartTime = Convert.ToDateTime(agenda.EndTime.ToShortDateString() + " 00:00:00");
                }
                else if (startTime != "" && endTime == "")
                {
                    agenda.StartTime = Convert.ToDateTime(startTime);
                    agenda.EndTime = Convert.ToDateTime(agenda.StartTime.ToShortDateString() + " 23:59:59");
                }
                else
                {
                    agenda.StartTime = Convert.ToDateTime(startTime);
                    agenda.EndTime = Convert.ToDateTime(endTime);
                }
                agenda.ProjectId = project.ProjectId;
                m_dbAdapter.Agenda.NewAgenda(agenda);

                var logicModel = Platform.GetProject(project.ProjectGuid);
                logicModel.Activity.Add(project.ProjectId, ActivityObjectType.Agenda, agenda.Guid, "创建日程：" + agenda.Name);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult GetAgenda(string agendaGuid)
        {
            return ActionUtils.Json(() =>
            {
                var agenda = m_dbAdapter.Agenda.GetAgendaByGuid(agendaGuid);
                var result = new
                {
                    start = Toolkit.DateTimeToString(agenda.StartTime),
                    end = Toolkit.DateTimeToString(agenda.EndTime),
                };
                return ActionUtils.Success(result);
            });
        }
        [HttpPost]
        public ActionResult ModifyAgenda(string projectGuid, string guid, string agendaName,
            string description, DateTime startTime, DateTime endTime)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(projectGuid);

                ValidateUtils.Name(agendaName, "日程名称");
                ValidateUtils.Description(description, "日程描述");

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到ProjectSeries[projectGuid={0}]信息", projectGuid);

                var agenda = m_dbAdapter.Agenda.GetAgendaByGuid(guid);
                CommUtils.Assert(IsCurrentUser(agenda.CreateUserName), "非该日程创建用户不得编辑！");

                agenda.Name = agendaName;
                agenda.Description = description;
                agenda.StartTime = startTime;
                agenda.EndTime = endTime;
                m_dbAdapter.Agenda.UpdateAgenda(agenda);
                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult DeleteAgenda(string projectGuid, string guid)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(projectGuid);

                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到ProjectSeries[projectGuid={0}]信息", projectGuid);

                var agenda = m_dbAdapter.Agenda.GetAgendaByGuid(guid);

                CommUtils.Assert(IsCurrentUser(agenda.CreateUserName), "非该日程创建用户不得删除！");

                m_dbAdapter.Agenda.DeleteAgenda(agenda);

                var logicModel = Platform.GetProject(project.ProjectGuid);
                logicModel.Activity.Add(project.ProjectId, ActivityObjectType.Agenda, agenda.Guid, "删除日程：" + agenda.Name);

                return ActionUtils.Success(1);
            });
        }

        [HttpPost]
        public ActionResult GetAgendaTop(string projectGuid, string startDate)
        {
            return ActionUtils.Json(() =>
            {
                CheckPermission(PermissionObjectType.Project, projectGuid, PermissionType.Read);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到ProjectSeries[projectGuid={0}]信息", projectGuid);

                var allAgendas = m_dbAdapter.Agenda.GetAgendasByProjectId(project.ProjectId);

                var nowDate = DateTime.Today;
                allAgendas.RemoveAll(x => x.EndTime < nowDate);

                var splitAgendas = new List<Agenda>();
                foreach (var agenda in allAgendas)
                {
                    while (agenda.EndTime.Date > agenda.StartTime.Date)
                    {
                        splitAgendas.Add(new Agenda()
                        {
                            Name = agenda.Name,
                            StartTime = agenda.EndTime.Date,
                            EndTime = agenda.EndTime
                        });
                        agenda.EndTime = agenda.EndTime.Date.AddSeconds(-1);
                    }

                    splitAgendas.Add(agenda);
                }

                splitAgendas = splitAgendas.Where(x => x.StartTime >= nowDate)
                    .OrderBy(x => x.StartTime).ToList();

                var agendaInfos = splitAgendas.ConvertAll(x => new AgendaInfo
                {
                    StartDate = x.StartTime.ToString("yyyy-MM-dd"),
                    StartTime = x.StartTime.ToString("HH:mm"),
                    Title = x.Name,
                });

                var dict = agendaInfos.GroupBy(x => x.StartDate).ToDictionary(x => x.Key);

                int dayCount = 0;
                var agendaDays = new List<AgendaDay>();
                foreach (var key in dict.Keys)
                {
                    var agendaDay = new AgendaDay(key);
                    agendaDay.AgendaInfos = dict[key].ToList();

                    var count = agendaDay.AgendaInfos.Count;
                    if (count > 3)
                    {
                        agendaDay.AgendaInfos[2].Title = "共" + count + "条";
                        //TODO: do not save date in time field
                        agendaDay.AgendaInfos[2].StartTime = agendaDay.AgendaInfos[2].StartDate;
                        agendaDay.AgendaInfos.RemoveRange(3, count - 3);
                    }
                    agendaDays.Add(agendaDay);

                    ++dayCount;
                    if (dayCount >= 3)
                    {
                        break;
                    }
                }

                DateTime nextDate = agendaDays.Count > 0 ? agendaDays.Max(x => x.StartDate).AddDays(1) : DateTime.Today;
                while (agendaDays.Count < 3)
                {
                    agendaDays.Add(new AgendaDay(nextDate)
                    {
                        AgendaInfos = new List<AgendaInfo> { new AgendaInfo { Title = "暂无" } },
                    });

                    nextDate = nextDate.AddDays(1);
                }

                return ActionUtils.Success(agendaDays);
            });
        }

        [HttpPost]
        public ActionResult GetTasks(string projectGuid, string startDate, string endDate)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                CommUtils.Assert(project.ProjectSeriesId.HasValue, "找不到[{0}]", project.Name);
                CheckPermission(PermissionObjectType.Project, project.ProjectGuid, PermissionType.Read);

                var tasks = m_dbAdapter.Task.GetTasksByProjectId(project.ProjectId);

                CommUtils.Assert(DateUtils.IsDate(startDate), "开始时间[{0}]不是日期格式", startDate);
                CommUtils.Assert(DateUtils.IsDate(endDate), "结束时间[{0}]不是日期格式", endDate);
                var start = DateUtils.Parse(startDate).Value;
                var end = DateUtils.Parse(endDate).Value;

                Func<Task, bool> isInQueryTimeRange = task =>
                {
                    if (!task.EndTime.HasValue)
                    {
                        return false;
                    }

                    if (task.StartTime.HasValue)
                    {
                        return !(task.StartTime.Value > end || task.EndTime.Value < start);
                    }
                    else
                    {
                        return (task.EndTime.Value <= end && task.EndTime.Value >= start);
                    }
                };

                tasks = tasks.Where(isInQueryTimeRange).ToList();

                Platform.UserProfile.Precache(tasks.Select(x => x.PersonInCharge));

                var now = DateTime.Now;
                var result = tasks.ConvertAll(x =>
                {
                    var personIncharge = Platform.UserProfile.Get(x.PersonInCharge);
                    return new
                    {
                        projectName = project.Name,
                        shortCode = x.ShortCode,
                        title = x.Description,
                        start = Toolkit.DateToString(x.StartTime ?? x.EndTime.Value.Date),
                        end = Toolkit.DateToString(x.EndTime.Value.AddDays(1).Date.AddSeconds(-1)),
                        status = x.TaskStatus.ToString(),
                        personInCharge = personIncharge == null ? "-" : personIncharge.RealName + "(" + personIncharge.UserName + ")",
                        desc = x.TaskDetail,
                        target = x.TaskTarget,
                        reminderInfo = m_dbAdapter.MessageReminding.GetResultByUid(x.ShortCode),
                    };
                });
                return ActionUtils.Success(result);
            });
        }
    }
}
