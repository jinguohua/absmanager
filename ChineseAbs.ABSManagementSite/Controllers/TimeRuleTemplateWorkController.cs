using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagement.Models.TimeRuleModel;
using ChineseAbs.ABSManagement.TimeFactory;
using ChineseAbs.ABSManagement.TimeFactory.Transform;
using ChineseAbs.ABSManagement.Utils;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.ABSManagementSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ChineseAbs.ABSManagementSite.Controllers
{
    public class TimeRuleTemplateWorkController : BaseController
    {
        [HttpPost]
        public ActionResult GetMetaTask(string projectGuid, string metaTaskGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                var metaTask = m_dbAdapter.MetaTask.GetByGuid(metaTaskGuid);

                var prevMetaTaskIds = CommUtils.Split(metaTask.PreMetaTaskIds).Select(x => int.Parse(x));
                CommUtils.AssertEquals(project.ProjectId, metaTask.ProjectId, "当前产品下的id[{0}]与工作[{1}]的guid[{2}]不一致", project.ProjectId, metaTask.Name, metaTask.ProjectId);

                var prevMetaTasks = m_dbAdapter.MetaTask.GetByIds(prevMetaTaskIds);
                var dictPrevMetaTask = prevMetaTasks.ToDictionary(x => x.Id.ToString());

                var beginTimeSeries = metaTask.StartTimeSeriesId.HasValue && metaTask.StartTimeSeriesId.Value != 0
                    ? m_dbAdapter.TimeSeries.GetById(metaTask.StartTimeSeriesId.Value) : null;
                var endTimeSeries = m_dbAdapter.TimeSeries.GetById(metaTask.EndTimeSeriesId);

                var result = new
                {
                    metaTaskName = metaTask.Name,
                    metaTaskGuid = metaTask.Guid,
                    projectGuid = project.ProjectGuid,
                    beginTimeGuid = beginTimeSeries == null ? null : beginTimeSeries.Guid,
                    endTimeGuid = endTimeSeries.Guid,
                    beginTime = beginTimeSeries == null ? null : new
                    {
                        beginTimeRules = GetTimeRuleDetail(beginTimeSeries.Id),
                        beginTimeOrigin = GetTimeOriginDetail(beginTimeSeries.Guid, beginTimeSeries.Id)
                    },
                    endTime = new
                    {
                        endTimeRules = GetTimeRuleDetail(endTimeSeries.Id),
                        endTimeOrigin = GetTimeOriginDetail(endTimeSeries.Guid, endTimeSeries.Id)
                    },
                    prevMetaTask = prevMetaTasks.ConvertAll(x => new
                    {
                        prevMetataskName = x.Name,
                        prevMetaTaskGuid = x.Guid
                    }),
                    taskExtensionType = metaTask.TaskExtensionType.ToString(),
                    detail = metaTask.Detail,
                    target = metaTask.Target,
                    createUserName = metaTask.CreateUserName,
                    createTime = metaTask.CreateTime.ToString("yyyy-MM-dd"),
                    lastModifyTime = metaTask.LastModifyTime.ToString("yyyy-MM-dd"),
                    lastModifyUserName = metaTask.LastModifyUserName,
                };
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult GetAllMetaTasks(string projectGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var metaTasks = m_dbAdapter.MetaTask.GetMetaTaskByProjectId(project.ProjectId);

                var metaTaskIds = metaTasks.ConvertAll(x => x.Id);
                var tasks = m_dbAdapter.Task.GetTasksByMetaTaskId(metaTaskIds);

                var resultMetaTasks = metaTasks.Where(x => tasks.Keys.Contains(x.Id)).ToList();

                var result = resultMetaTasks.ConvertAll(x => new
                {
                    name = x.Name,
                    guid = x.Guid
                });
                return ActionUtils.Success(result);
            });
        }

        [HttpPost]
        public ActionResult CreateMetaTask(string projectGuid, string metaTaskName, string guidAsStartTime, string guidAsEndTime,
            string prevMetaTaskText, string taskExtensionType, string detail, string target)
        {
            return ActionUtils.Json(() =>
            {
                ValidateUtils.Name(metaTaskName, "工作名称");
                CommUtils.Assert(detail ==null ? true : detail.Length <= 500, "工作描述不能超过500个字符数");
                CommUtils.Assert(target == null ? true : target.Length <= 500, "工作目标不能超过500个字符数");
                CommUtils.AssertHasContent(guidAsEndTime, "请先设置截止时间");

                var startTimeSeries = string.IsNullOrWhiteSpace(guidAsStartTime) ? null : m_dbAdapter.TimeSeries.GetByGuid(guidAsStartTime);
                var endTimeSeries = m_dbAdapter.TimeSeries.GetByGuid(guidAsEndTime);

                var timeListCount = CheckStartAndEndTimeCount(startTimeSeries, endTimeSeries);

                var extensionType = CommUtils.ParseEnum<TaskExtensionType>(taskExtensionType);
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);
                var prevMetaTaskGuidList = CommUtils.Split(prevMetaTaskText).ToList();
                var prevMetaTask = m_dbAdapter.MetaTask.GetMetaTaskByGuids(prevMetaTaskGuidList);

                var prevMetaTaskIds = prevMetaTask.ConvertAll(x => x.Id.ToString()).ToList();

                CheckPrevMetaTaskGenerateCount(timeListCount, prevMetaTask);

                var metaTask = new MetaTask();
                metaTask.ProjectId = project.ProjectId;
                metaTask.Name = metaTaskName;
                if (startTimeSeries == null)
                {
                    metaTask.StartTimeSeriesId = null;
                }
                else
                {
                    metaTask.StartTimeSeriesId = startTimeSeries.Id;
                }
                metaTask.EndTimeSeriesId = endTimeSeries.Id;
                metaTask.PreMetaTaskIds = CommUtils.Join(prevMetaTaskIds);
                metaTask.TaskExtensionType = extensionType;
                metaTask.Detail = detail;
                metaTask.Target = target;

                var newMetaTask = m_dbAdapter.MetaTask.New(metaTask);

                return ActionUtils.Success(newMetaTask.Guid);
            });
        }

        private void CheckPrevMetaTaskGenerateCount(int timeListCount, List<MetaTask> prevMetaTasks)
        {
            foreach (var metaTask in prevMetaTasks)
            {
                var endTimeSeries = m_dbAdapter.TimeSeries.GetById(metaTask.EndTimeSeriesId);

                var endTimeRule = GetTimeRuleDetail(endTimeSeries.Id);
                var endTimeOrigin = GetTimeOriginDetail(endTimeSeries.Guid, endTimeSeries.Id);

                var endTimeList = GetTimeListByTimeRuleOrigin(endTimeRule, endTimeOrigin);

                CommUtils.Assert(endTimeList.Count == timeListCount, "通过前置工作[{0}]生成的工作数量[{1}]与当前能够生成的工作数量[{2}]不匹配。", metaTask.Name, endTimeList.Count, timeListCount);
            }
        }

        private int CheckStartAndEndTimeCount(TimeSeries startTimeSeries, TimeSeries endTimeSeries)
        {
            if (startTimeSeries != null)
            {
                var startTimeRule = GetTimeRuleDetail(startTimeSeries.Id);
                var startTimeOrigin = GetTimeOriginDetail(startTimeSeries.Guid, startTimeSeries.Id);

                var endTimeRule = GetTimeRuleDetail(endTimeSeries.Id);
                var endTimeOrigin = GetTimeOriginDetail(endTimeSeries.Guid, endTimeSeries.Id);

                var startTimeList = GetTimeListByTimeRuleOrigin(startTimeRule, startTimeOrigin);
                var endTimeList = GetTimeListByTimeRuleOrigin(endTimeRule, endTimeOrigin);

                CommUtils.Assert(startTimeList.Count == endTimeList.Count && endTimeList.Count != 0, "开始时间的数量[{0}]与截止时间[{1}]的数量不匹配，无法创建工作",
                    startTimeList.Count, endTimeList.Count);
                for (int i = 0; i < endTimeList.Count; i++)
                {
                    var startTime = DateTime.Parse(startTimeList[i]);
                    var endTime = DateTime.Parse(endTimeList[i]);
                    CommUtils.Assert(startTime <= endTime, "截止时间[{0}]必须大于等于开始时间[{1}]", endTime, startTime);
                }
                return endTimeList.Count;
            }
            else
            {
                var endTimeRule = GetTimeRuleDetail(endTimeSeries.Id);
                var endTimeOrigin = GetTimeOriginDetail(endTimeSeries.Guid, endTimeSeries.Id);

                var endTimeList = GetTimeListByTimeRuleOrigin(endTimeRule, endTimeOrigin);
                return endTimeList.Count;
            }
        }

        [HttpPost]
        public ActionResult CheckPrevMetaTaskCount(string projectGuid, string endTimeSeriesGuid, string prevMetaTaskGuidText)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                var prevMetaTaskGuids = CommUtils.Split(prevMetaTaskGuidText).ToList();
                var prevMetaTasks = m_dbAdapter.MetaTask.GetMetaTaskByGuids(prevMetaTaskGuids);

                prevMetaTasks.ForEach(x => CommUtils.Assert(x.ProjectId == project.ProjectId, "前置工作的projectId[{0}]与当前产品的projectId[{1}]不一致", x.ProjectId, project.ProjectId));

                var endTimeSeries = m_dbAdapter.TimeSeries.GetByGuid(endTimeSeriesGuid);
                var endTimeListCount = CheckStartAndEndTimeCount(null, endTimeSeries);

                CheckPrevMetaTaskGenerateCount(endTimeListCount, prevMetaTasks);

                return ActionUtils.Success("");
            });
        }

        [HttpPost]
        public ActionResult CreateTimeSeries(string timeSeriesName)
        {
            return ActionUtils.Json(() =>
            {
                CommUtils.AssertHasContent(timeSeriesName,"名称不能为空");
                var newTimeSeries = m_dbAdapter.TimeSeries.NewTimeSeries(timeSeriesName);

                return ActionUtils.Success(newTimeSeries.Guid);
            });
        }

        [HttpPost]
        public ActionResult SaveTimeRule(TimeRuleViewModel timeRuleViewModel)
        {
            return ActionUtils.Json(() =>
            {
                if (timeRuleViewModel != null && timeRuleViewModel.IsExistRule)
                {
                    //检查timeRuleViewModel里的参数
                    CommUtils.AssertHasContent(timeRuleViewModel.TimeSeriesGuid, "TimeSeriesGuid不能为空");

                    CheckTimeRuleParam(timeRuleViewModel);

                    CreateTimeRuleAndDetailRule(timeRuleViewModel);
                }

                return ActionUtils.Success("");
            });
        }

        private void CreateTimeRuleAndDetailRule(TimeRuleViewModel timeRuleViewModel)
        {
            var timeSeries = m_dbAdapter.TimeSeries.GetByGuid(timeRuleViewModel.TimeSeriesGuid);

            //for{创建规则TimeRule详情,然后创建对应的TimeRule}
            var timeRuleOrder = timeRuleViewModel.TimeRuleOrder;
            int ruleOrder = 0;
            foreach (var item in timeRuleOrder)
            {
                ruleOrder++;
                var timeRuleType = CommUtils.ParseEnum<TimeRuleType>(item);
                var timeRuleDetailId = 0;
                switch (timeRuleType)
                {
                    case TimeRuleType.MoveAppointDate:
                        var timeRuleShiftViewModels = timeRuleViewModel.ShiftList;
                        foreach (var timeRuleShiftViewModel in timeRuleShiftViewModels)
                        {
                            if (timeRuleShiftViewModel.Ranking == ruleOrder)
                            {
                                var timeMoveDirection = CommUtils.ParseEnum<TimeMoveDirection>(timeRuleShiftViewModel.TimeMoveDirection);
                                var timeRuleShift = new TimeRuleShift();
                                timeRuleShift.TimeSeriesId = timeSeries.Id;
                                timeRuleShift.TimeInterval = timeMoveDirection == TimeMoveDirection.Minus ? timeRuleShiftViewModel.Interval * -1 : timeRuleShiftViewModel.Interval;
                                timeRuleShift.TimeRuleUnitType = timeRuleShiftViewModel.ConditionUnitType;

                                timeRuleDetailId = m_dbAdapter.TimeRuleShift.New(timeRuleShift).Id;

                                CreateTimeRule(timeSeries.Id, ruleOrder, timeRuleType, timeRuleDetailId);
                                break;
                            }
                        }
                        break;

                    case TimeRuleType.FindAppointDate:
                        var periodSequenceViewModels = timeRuleViewModel.PeriodSequenceList;
                        foreach (var periodSequenceViewModel in periodSequenceViewModels)
                        {
                            if (periodSequenceViewModel.Ranking == ruleOrder)
                            {
                                var periodSequence = new TimeRulePeriodSequence();
                                periodSequence.TimeSeriesId = timeSeries.Id;
                                periodSequence.TimeRulePeriodType = periodSequenceViewModel.PeriodType;
                                periodSequence.Sequence = periodSequenceViewModel.Interval;
                                periodSequence.TimeRuleUnitType = periodSequenceViewModel.ConditionUnitType;

                                timeRuleDetailId = m_dbAdapter.TimeRulePeriodSequence.New(periodSequence).Id;

                                CreateTimeRule(timeSeries.Id, ruleOrder, timeRuleType, timeRuleDetailId);
                                break;
                            }
                        }
                        break;

                    case TimeRuleType.ReplaceAppointDate:
                        var ConditionShiftViewModels = timeRuleViewModel.ConditionShiftList;
                        foreach (var ConditionShiftViewModel in ConditionShiftViewModels)
                        {
                            if (ConditionShiftViewModel.Ranking == ruleOrder)
                            {
                                var timeMoveDirection = CommUtils.ParseEnum<TimeMoveDirection>(ConditionShiftViewModel.TimeMoveDirection);
                                var timeRuleConditionShift = new TimeRuleConditionShift();
                                timeRuleConditionShift.TimeSeriesId = timeSeries.Id;
                                timeRuleConditionShift.TimeRuleDateType = ConditionShiftViewModel.DateType;
                                timeRuleConditionShift.TimeInterval = timeMoveDirection == TimeMoveDirection.Minus ? ConditionShiftViewModel.Interval * -1 : ConditionShiftViewModel.Interval;
                                timeRuleConditionShift.TimeRuleUnitType = ConditionShiftViewModel.ConditionUnitType;

                                timeRuleDetailId = m_dbAdapter.TimeRuleConditionShift.New(timeRuleConditionShift).Id;

                                CreateTimeRule(timeSeries.Id, ruleOrder, timeRuleType, timeRuleDetailId);
                                break;
                            }
                        }
                        break;

                    case TimeRuleType.RemoveRepeatDate:
                        CreateTimeRule(timeSeries.Id, ruleOrder, timeRuleType, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        private void CreateTimeRule(int timeSeriesId, int timeRuleOrder, TimeRuleType timeRuleType, int instanceId)
        {
            var timeRule = new TimeRule();
            timeRule.TimeSeriesId = timeSeriesId;
            timeRule.TimeRuleOrder = timeRuleOrder;
            timeRule.TimeRuleType = timeRuleType;
            timeRule.TimeRuleInstanceId = instanceId;

            m_dbAdapter.TimeRule.New(timeRule);
        }

        //获取开始/截止时间的时间规则
        [HttpPost]
        public ActionResult GetTimeRuleSet(string timeSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var timeSeries = m_dbAdapter.TimeSeries.GetByGuid(timeSeriesGuid);
                TimeRuleViewModel timeRuleViewModel = GetTimeRuleDetail(timeSeries.Id);

                return ActionUtils.Success(timeRuleViewModel);
            });
        }

        private TimeRuleViewModel GetTimeRuleDetail(int timeSeriesId)
        {
            var timeRules = m_dbAdapter.TimeRule.GetTimeRulesByTimeSeriesId(timeSeriesId);
            timeRules.OrderBy(x => x.TimeRuleOrder);

            var timeRuleViewModel = new TimeRuleViewModel();

            if (timeRules.Count > 0)
            {
                timeRuleViewModel.IsExistRule = true;
            }

            int ranking = 0;
            foreach (var timeRule in timeRules)
            {
                timeRuleViewModel.TimeRuleOrder.Add(timeRule.TimeRuleType.ToString());
                ranking++;
                switch (timeRule.TimeRuleType)
                {
                    case TimeRuleType.MoveAppointDate:
                        var timeRuleShift = m_dbAdapter.TimeRuleShift.GetById(timeRule.TimeRuleInstanceId);
                        var shift = new Shift();
                        shift.ConditionUnitType = timeRuleShift.TimeRuleUnitType;
                        shift.Interval = Math.Abs(timeRuleShift.TimeInterval);
                        shift.TimeMoveDirection = timeRuleShift.TimeInterval > 0 ? TimeMoveDirection.Plus.ToString() : TimeMoveDirection.Minus.ToString();
                        shift.Ranking = ranking;
                        timeRuleViewModel.ShiftList.Add(shift);
                        break;
                    case TimeRuleType.FindAppointDate:
                        var timeRulePeriodSequence = m_dbAdapter.TimeRulePeriodSequence.GetById(timeRule.TimeRuleInstanceId);
                        var periodSequence = new PeriodSequence();
                        periodSequence.PeriodType = timeRulePeriodSequence.TimeRulePeriodType;
                        periodSequence.Interval = timeRulePeriodSequence.Sequence;
                        periodSequence.ConditionUnitType = timeRulePeriodSequence.TimeRuleUnitType;
                        periodSequence.Ranking = ranking;
                        timeRuleViewModel.PeriodSequenceList.Add(periodSequence);
                        break;
                    case TimeRuleType.ReplaceAppointDate:
                        var timeRuleConditionShift = m_dbAdapter.TimeRuleConditionShift.GetById(timeRule.TimeRuleInstanceId);
                        var conditionShift = new ConditionShift();
                        conditionShift.ConditionUnitType = timeRuleConditionShift.TimeRuleUnitType;
                        conditionShift.Interval = Math.Abs(timeRuleConditionShift.TimeInterval);
                        conditionShift.DateType = timeRuleConditionShift.TimeRuleDateType;
                        conditionShift.TimeMoveDirection = timeRuleConditionShift.TimeInterval > 0 ? TimeMoveDirection.Plus.ToString() : TimeMoveDirection.Minus.ToString();
                        conditionShift.Ranking = ranking;
                        timeRuleViewModel.ConditionShiftList.Add(conditionShift);
                        break;
                    case TimeRuleType.RemoveRepeatDate:
                        var removeRepeatDate = new RemoveRepeatDate();
                        removeRepeatDate.IsRemoveRepeatDate = true;
                        removeRepeatDate.Ranking = ranking;
                        timeRuleViewModel.RemoveRepeatDateList.Add(removeRepeatDate);
                        break;
                    default:
                        break;
                }
            }

            return timeRuleViewModel;
        }

        //获取开始/截止时间的时间来源
        [HttpPost]
        public ActionResult GetTimeOriginSet(string timeSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var timeSeries = m_dbAdapter.TimeSeries.GetByGuid(timeSeriesGuid);
                var timeOriginViewModel = GetTimeOriginDetail(timeSeries.Guid, timeSeries.Id);

                return ActionUtils.Success(timeOriginViewModel);
            });
        }

        private TimeOriginViewModel GetTimeOriginDetail(string timeSeriesGuid, int timeSeriesId)
        {
            var timeOriginViewModel = new TimeOriginViewModel();
            var timeOrigin = m_dbAdapter.TimeOrigin.GetByTimeSeriesId(timeSeriesId);
            var timeOriginType = CommUtils.ParseEnum<TimeOriginType>(timeOrigin.TimeOriginType);

            timeOriginViewModel.TimeSeriesGuid = timeSeriesGuid;
            timeOriginViewModel.TimeOriginType = timeOrigin.TimeOriginType;

            switch (timeOriginType)
            {
                case TimeOriginType.CustomInput:
                    var timeOriginCustomInput = m_dbAdapter.TimeOriginCustomInput.GetById(timeOrigin.TimeOriginInstanceId);
                    timeOriginViewModel.CustomInput.CustomTimeList = timeOriginCustomInput.CustomTimeSeries;
                    break;
                case TimeOriginType.Loop:
                    var timeOriginLoop = m_dbAdapter.TimeOriginLoop.GetById(timeOrigin.TimeOriginInstanceId);
                    timeOriginViewModel.Loop.Interval = timeOriginLoop.LoopInterval;
                    timeOriginViewModel.Loop.BeginTime = timeOriginLoop.LoopBegin.ToString("yyyy-MM-dd");
                    timeOriginViewModel.Loop.EndTime = timeOriginLoop.LoopEnd.ToString("yyyy-MM-dd");
                    timeOriginViewModel.Loop.RuleUnitType = timeOriginLoop.TimeRuleUnitType;
                    break;
                case TimeOriginType.MetaTask:
                    var timeOriginMetaTask = m_dbAdapter.TimeOriginMetaTask.GetById(timeOrigin.TimeOriginInstanceId);
                    var prevMetaTask = m_dbAdapter.MetaTask.GetById(timeOriginMetaTask.MetaTaskId);
                    timeOriginViewModel.PrevMetaTask.MetaTaskGuid = prevMetaTask.Guid;
                    timeOriginViewModel.PrevMetaTask.MetaTaskTimeType = timeOriginMetaTask.MetaTaskTimeType;
                    break;
                case TimeOriginType.TaskSelfTime:
                    var timeOriginTaskSelfTime = m_dbAdapter.TimeOriginTaskSelfTime.GetById(timeOrigin.TimeOriginInstanceId);
                    timeOriginViewModel.TaskSelfTime.TimeSeriesGuid = timeOriginTaskSelfTime.TimeOriginTimeSeriesGuid;
                    timeOriginViewModel.TaskSelfTime.TimeType = timeOriginTaskSelfTime.TimeType;
                    break;
                default:
                    break;
            }

            return timeOriginViewModel;
        }

        [HttpPost]
        public ActionResult SaveTimeOrigin(TimeOriginViewModel timeOriginViewModel)
        {
            return ActionUtils.Json(() =>
            {
                if (timeOriginViewModel != null)
                {
                    //检查timeOriginViewModel里的参数
                    CommUtils.AssertHasContent(timeOriginViewModel.TimeSeriesGuid, "TimeSeriesGuid不能为空");
                    CommUtils.AssertHasContent(timeOriginViewModel.TimeOriginType, "TimeOriginType不能为空");

                    CheckTimeOriginParam(timeOriginViewModel);

                    CreateTimeOriginAndDetailOrigin(timeOriginViewModel);
                }
                return ActionUtils.Success("ok");
            });
        }

        private void CreateTimeOriginAndDetailOrigin(TimeOriginViewModel timeOriginViewModel)
        {
            var timeSeries = m_dbAdapter.TimeSeries.GetByGuid(timeOriginViewModel.TimeSeriesGuid);

            var timeOriginType = CommUtils.ParseEnum<TimeOriginType>(timeOriginViewModel.TimeOriginType);
            var instanceId = 0;
            switch (timeOriginType)
            {
                case TimeOriginType.CustomInput:
                    var customInput = new TimeOriginCustomInput();
                    customInput.TimeSeriesId = timeSeries.Id;
                    customInput.CustomTimeSeries = timeOriginViewModel.CustomInput.CustomTimeList;

                    instanceId = m_dbAdapter.TimeOriginCustomInput.New(customInput).Id;
                    break;
                case TimeOriginType.Loop:
                    var loop = new TimeOriginLoop();
                    loop.TimeSeriesId = timeSeries.Id;
                    loop.LoopBegin = DateTime.Parse(timeOriginViewModel.Loop.BeginTime);
                    loop.LoopEnd = DateTime.Parse(timeOriginViewModel.Loop.EndTime);
                    loop.LoopInterval = timeOriginViewModel.Loop.Interval;
                    loop.TimeRuleUnitType = timeOriginViewModel.Loop.RuleUnitType;

                    instanceId = m_dbAdapter.TimeOriginLoop.New(loop).Id;
                    break;
                case TimeOriginType.MetaTask:
                    var prevMetaTask = m_dbAdapter.MetaTask.GetByGuid(timeOriginViewModel.PrevMetaTask.MetaTaskGuid);
                    var timeOriginMetaTask = new TimeOriginMetaTask();
                    timeOriginMetaTask.TimeSeriesId = timeSeries.Id;
                    timeOriginMetaTask.MetaTaskTimeType = timeOriginViewModel.PrevMetaTask.MetaTaskTimeType;
                    timeOriginMetaTask.MetaTaskId = prevMetaTask.Id;

                    instanceId = m_dbAdapter.TimeOriginMetaTask.New(timeOriginMetaTask).Id;
                    break;
                case TimeOriginType.TaskSelfTime:
                    var timeOriginTaskSelfTime = new TimeOriginTaskSelfTime();
                    timeOriginTaskSelfTime.TimeSeriesId = timeSeries.Id;
                    timeOriginTaskSelfTime.TimeType = timeOriginViewModel.TaskSelfTime.TimeType;
                    timeOriginTaskSelfTime.TimeOriginTimeSeriesGuid = timeOriginViewModel.TaskSelfTime.TimeSeriesGuid;

                    instanceId = m_dbAdapter.TimeOriginTaskSelfTime.New(timeOriginTaskSelfTime).Id;
                    break;
                default:
                    break;
            }
            CreateTimeOrigin(timeSeries.Id, timeOriginViewModel.TimeOriginType, instanceId);
        }

        private void CreateTimeOrigin(int timeSeriesId, string timeOriginType, int instanceId)
        {
            var timeOrigin = new TimeOrigin();
            timeOrigin.TimeSeriesId = timeSeriesId;
            timeOrigin.TimeOriginType = timeOriginType;
            timeOrigin.TimeOriginInstanceId = instanceId;

            m_dbAdapter.TimeOrigin.New(timeOrigin);
        }

        private void CheckTimeRuleParam(TimeRuleViewModel timeRule)
        {
            timeRule.PeriodSequenceList.ForEach(x => x.CheckParam());
            timeRule.ConditionShiftList.ForEach(x => x.CheckParam());
            timeRule.ShiftList.ForEach(x => x.CheckParam());
        }

        private void CheckTimeOriginParam(TimeOriginViewModel timeOrigin)
        {
            var timeOriginType = CommUtils.ParseEnum<TimeOriginType>(timeOrigin.TimeOriginType);

            switch (timeOriginType)
            {
                case TimeOriginType.CustomInput:
                    timeOrigin.CustomInput.CheckParam();
                    break;
                case TimeOriginType.Loop:
                    timeOrigin.Loop.CheckParam();
                    break;
                case TimeOriginType.MetaTask:
                    timeOrigin.PrevMetaTask.CheckParam();
                    break;
                case TimeOriginType.TaskSelfTime:
                    timeOrigin.TaskSelfTime.CheckParam();
                    break;
                default:
                    break;
            }
        }

        //查询时间==根据规则计算时间列表
        [HttpPost]
        public ActionResult TestCalculateTime(string timeSeriesGuid)
        {
            return ActionUtils.Json(() =>
            {
                var timeSeries = m_dbAdapter.TimeSeries.GetByGuid(timeSeriesGuid);
                var timeRule = GetTimeRuleDetail(timeSeries.Id);
                var timeOrigin = GetTimeOriginDetail(timeSeries.Guid, timeSeries.Id);

                //检查时间规则
                CheckTimeRuleParam(timeRule);

                //检查时间来源
                CheckTimeOriginParam(timeOrigin);

                //初始化时间列表
                var result = GetTimeListByTimeRuleOrigin(timeRule, timeOrigin);
                if (result == null)
                {
                    return ActionUtils.Success("");
                }
                return ActionUtils.Success(CommUtils.Join(result.ToArray()));
            });
        }

        private List<string> GetTimeListByTimeRuleOrigin(TimeRuleViewModel timeRule, TimeOriginViewModel timeOrigin)
        {
            var timeOriginType = CommUtils.ParseEnum<TimeOriginType>(timeOrigin.TimeOriginType);
            TimeSeriesFactory timeSeriesFactory;
            var result = new List<string>();

            if (timeOriginType == TimeOriginType.CustomInput)
            {
                var customTimeList = CommUtils.Split(timeOrigin.CustomInput.CustomTimeList).Select(x => DateTime.Parse(x));
                timeSeriesFactory = new TimeSeriesFactory(customTimeList);
                result = CalculateTimeByTimeRule(timeRule, timeSeriesFactory);
            }

            if (timeOriginType == TimeOriginType.Loop)
            {
                var startTime = DateTime.Parse(timeOrigin.Loop.BeginTime);
                var endTime = DateTime.Parse(timeOrigin.Loop.EndTime);
                var ruleUnitType = CommUtils.ParseEnum<TimeUnit>(timeOrigin.Loop.RuleUnitType);

                timeSeriesFactory = new TimeSeriesFactory(
                    new DateTime(startTime.Year, startTime.Month, startTime.Day),
                    new TimeStep { Interval = timeOrigin.Loop.Interval, Unit = ruleUnitType },
                    new DateTime(endTime.Year, endTime.Month, endTime.Day));
                result = CalculateTimeByTimeRule(timeRule, timeSeriesFactory);
            }

            if (timeOriginType == TimeOriginType.MetaTask)
            {
                result = GetTimeListByPrevMetaTask(timeRule, timeOrigin.PrevMetaTask.MetaTaskGuid, timeOrigin.PrevMetaTask.MetaTaskTimeType);
            }

            if (timeOriginType == TimeOriginType.TaskSelfTime)
            {
                var timeSeries = m_dbAdapter.TimeSeries.GetByGuid(timeOrigin.TaskSelfTime.TimeSeriesGuid);
                var CurrTimeRule = GetTimeRuleDetail(timeSeries.Id);
                var CurrTimeOrigin = GetTimeOriginDetail(timeSeries.Guid, timeSeries.Id);
                var timeList = GetTimeListByTimeRuleOrigin(CurrTimeRule, CurrTimeOrigin).Select(x => DateTime.Parse(x));
                timeSeriesFactory = new TimeSeriesFactory(timeList);
                result = CalculateTimeByTimeRule(timeRule, timeSeriesFactory);
            }
            return result;
        }

        private List<string> GetTimeListByPrevMetaTask(TimeRuleViewModel timeRule, string metaTaskGuid, string metaTaskTimeType)
        {
            var metaTask = m_dbAdapter.MetaTask.GetByGuid(metaTaskGuid);
            var timeType = CommUtils.ParseEnum<MetaTaskTimeType>(metaTaskTimeType);
            var result = new List<string>();
            TimeSeriesFactory timeSeriesFactory;

            if (timeType == MetaTaskTimeType.StartTime)
            {
                if (!metaTask.StartTimeSeriesId.HasValue || metaTask.StartTimeSeriesId.Value == 0)
                {
                    return null;
                }
                var timeSeries = m_dbAdapter.TimeSeries.GetById(metaTask.StartTimeSeriesId.Value);
                var timeRuleDetail = GetTimeRuleDetail(timeSeries.Id);
                var timeOriginDetail = GetTimeOriginDetail(timeSeries.Guid, timeSeries.Id);

                var timeStrList = GetTimeListByTimeRuleOrigin(timeRuleDetail, timeOriginDetail);
                var timeList = timeStrList.ConvertAll(x => DateTime.Parse(x));
                timeSeriesFactory = new TimeSeriesFactory(timeList);
                result = CalculateTimeByTimeRule(timeRule, timeSeriesFactory);
            }

            if (timeType == MetaTaskTimeType.EndTime)
            {
                var timeSeries = m_dbAdapter.TimeSeries.GetById(metaTask.EndTimeSeriesId);
                var timeRuleDetail = GetTimeRuleDetail(timeSeries.Id);
                var timeOriginDetail = GetTimeOriginDetail(timeSeries.Guid, timeSeries.Id);

                var timeStrList = GetTimeListByTimeRuleOrigin(timeRuleDetail, timeOriginDetail);
                var timeList = timeStrList.ConvertAll(x => DateTime.Parse(x));
                timeSeriesFactory = new TimeSeriesFactory(timeList);
                result = CalculateTimeByTimeRule(timeRule, timeSeriesFactory);
            }
            return result;
        }

        private List<string> CalculateTimeByTimeRule(TimeRuleViewModel timeRule, TimeSeriesFactory timeSeries)
        {
            if (!timeRule.IsExistRule)
            {
                return timeSeries.DateTimes.Select(x => Toolkit.DateToString(x)).ToList();
            }

            var order = 0;
            foreach (var item in timeRule.TimeRuleOrder)
            {
                order++;
                var timeRuleType = CommUtils.ParseEnum<TimeRuleType>(item);
                switch (timeRuleType)
                {
                    case TimeRuleType.MoveAppointDate:
                        var shiftRule = timeRule.ShiftList.Single(x => x.Ranking == order);
                        var shiftRuleUnitType = CommUtils.ParseEnum<TimeUnit>(shiftRule.ConditionUnitType);
                        var shiftRuleInterval = shiftRule.TimeMoveDirection == TimeMoveDirection.Minus.ToString()
                            ? shiftRule.Interval * -1 : shiftRule.Interval;

                        timeSeries.Apply(new RuleShift(shiftRuleInterval, shiftRuleUnitType));
                        break;
                    case TimeRuleType.FindAppointDate:
                        var periodSequence = timeRule.PeriodSequenceList.Single(x => x.Ranking == order);
                        var periodSequenceUnitType = CommUtils.ParseEnum<TimeUnit>(periodSequence.ConditionUnitType);
                        var periodType = CommUtils.ParseEnum<PeriodType>(periodSequence.PeriodType);

                        timeSeries.Apply(new RulePeriodSequence(periodType, periodSequence.Interval, periodSequenceUnitType));
                        break;
                    case TimeRuleType.ReplaceAppointDate:
                        var conditionShift = timeRule.ConditionShiftList.Single(x => x.Ranking == order);
                        var conditionShiftUnitType = CommUtils.ParseEnum<TimeUnit>(conditionShift.ConditionUnitType);
                        var conditionShiftInterval = conditionShift.TimeMoveDirection == TimeMoveDirection.Minus.ToString()
                            ? conditionShift.Interval * -1 : conditionShift.Interval;
                        var conditionShiftDateType = CommUtils.ParseEnum<DateType>(conditionShift.DateType);

                        timeSeries.Apply(new RuleConditionShift(conditionShiftDateType, conditionShiftInterval, conditionShiftUnitType));
                        break;
                    case TimeRuleType.RemoveRepeatDate:
                        timeSeries.Apply(new RuleDistinct());
                        break;
                    default:
                        break;
                }
            }
            return timeSeries.DateTimes.Select(x => Toolkit.DateToString(x)).ToList();
        }

        //根据模板工作创建工作列表（规则）
        [HttpPost]
        public ActionResult CreateTasksByMetaTask(string projectGuid, string metaTaskGuid)
        {
            return ActionUtils.Json(() =>
            {
                var project = m_dbAdapter.Project.GetProjectByGuid(projectGuid);

                var metaTask = m_dbAdapter.MetaTask.GetByGuid(metaTaskGuid);

                var prevMetaTaskIds = CommUtils.Split(metaTask.PreMetaTaskIds).Select(x => int.Parse(x));
                CommUtils.AssertEquals(project.ProjectId, metaTask.ProjectId, "当前产品下的id[{0}]与工作[{1}]的guid[{2}]不一致", project.ProjectId, metaTask.Name, metaTask.ProjectId);

                var prevMetaTasks = m_dbAdapter.MetaTask.GetByIds(prevMetaTaskIds);
                var dictPrevMetaTask = prevMetaTasks.ToDictionary(x => x.Id.ToString());

                var beginTimeSeries = metaTask.StartTimeSeriesId.HasValue && metaTask.StartTimeSeriesId.Value != 0
                    ? m_dbAdapter.TimeSeries.GetById(metaTask.StartTimeSeriesId.Value) : null;
                var endTimeSeries = m_dbAdapter.TimeSeries.GetById(metaTask.EndTimeSeriesId);

                if (beginTimeSeries == null)
                {
                    var endTimeRule = GetTimeRuleDetail(endTimeSeries.Id);
                    var endTimeOrigin = GetTimeOriginDetail(endTimeSeries.Guid, endTimeSeries.Id);

                    var endTimeStrList = GetTimeListByTimeRuleOrigin(endTimeRule, endTimeOrigin);
                    var endTimeList = endTimeStrList.ConvertAll(x => DateTime.Parse(x)).ToList();
                    //遍历beginTimeList跟endTimeList开始进行生成工作
                    GenerateTasks(metaTask, new List<DateTime>(), endTimeList);
                }
                else
                {
                    var beginTimeRule = GetTimeRuleDetail(beginTimeSeries.Id);
                    var beginTimeOrigin = GetTimeOriginDetail(beginTimeSeries.Guid, beginTimeSeries.Id);

                    var beginTimeStrList = GetTimeListByTimeRuleOrigin(beginTimeRule, beginTimeOrigin);
                    var beginTimeList = beginTimeStrList.ConvertAll(x => DateTime.Parse(x)).ToList();

                    var endTimeRule = GetTimeRuleDetail(endTimeSeries.Id);
                    var endTimeOrigin = GetTimeOriginDetail(endTimeSeries.Guid, endTimeSeries.Id);

                    var endTimeStrList = GetTimeListByTimeRuleOrigin(endTimeRule, endTimeOrigin);
                    var endTimeList = endTimeStrList.ConvertAll(x => DateTime.Parse(x)).ToList();

                    CommUtils.Assert(beginTimeList.Count == endTimeList.Count && endTimeList.Count !=0, "开始时间的数量[{0}]与截止时间的数量[{1}]不匹配，无法创建工作",
                        beginTimeList.Count, endTimeList.Count);

                    //遍历beginTimeList跟endTimeList开始进行生成工作
                    GenerateTasks(metaTask, beginTimeList, endTimeList);
                }

                return ActionUtils.Success("");
            });
        }

        private void GenerateTasks(MetaTask metaTask, List<DateTime> beginTimeList, List<DateTime> endTimeList)
        {
            var prevMetaTaskIds = CommUtils.Split(metaTask.PreMetaTaskIds).Select(x => int.Parse(x));
            var prevTasks = m_dbAdapter.Task.GetTasksByMetaTaskId(prevMetaTaskIds);
            var prevTasksKeys = prevTasks.Keys.ToList();

            if (prevTasks != null && prevTasks.Count !=0)
            {
                CommUtils.Assert(prevTasksKeys.Any(x => prevTasks[x].Count == endTimeList.Count && endTimeList.Count != 0), "前置工作的数量[{0}]与要生成的工作数量[{1}]不匹配", prevTasks.Count, endTimeList.Count);
            }

            var tasks = new List<Task>();

            if (beginTimeList.Count != 0)
            {
                CommUtils.Assert(beginTimeList.Count == endTimeList.Count && endTimeList.Count != 0, "当前工作[{0}]的开始时间数量[{1}]与截止时间数量[{1}]不匹配，请重新核对后再试", metaTask.Name, beginTimeList.Count, endTimeList.Count);
            }

            for (int i = 0; i < endTimeList.Count; i++)
            {
                DateTime? beginTime = null;

                if (beginTimeList.Count != 0)
                {
                    beginTime =  beginTimeList[i];
                }
                var endTime = endTimeList[i];

                var prevTaskIds = string.Empty;
                if (prevTasks != null && prevTasks.Count != 0)
                {
                    var prevTaskIdsStr = new List<string>();
                    foreach (var prevTasksId in prevTasks.Keys)
                    {
                        var prevTask = prevTasks[prevTasksId][i];
                        CommUtils.Assert(prevTask.EndTime.Value <= endTime,
                            "工作[{0}][{1}]的截止时间不能小于前置工作[{2}][{3}]的截止时间",
                            metaTask.Name, endTime, prevTask.Description, prevTask.EndTime.Value.ToString("yyyy-MM-dd"));
                        prevTaskIdsStr.Add(prevTask.TaskId.ToString());
                    }
                    prevTaskIds = CommUtils.Join(prevTaskIdsStr);
                }

                var task = GenerateOnceTask(metaTask, beginTime, endTime, prevTaskIds);
                tasks.Add(task);
            }

            tasks.ForEach(x => m_dbAdapter.Task.NewTask(x));
        }

        private Task GenerateOnceTask(MetaTask metaTask, DateTime? beginTime, DateTime endTime, string prevTaskIds)
        {
            int? taskExId = null;
            if (metaTask.TaskExtensionType != TaskExtensionType.None)
            {
                var taskEx = ChineseAbs.ABSManagement.Models.TaskExtension.Create(metaTask.TaskExtensionType);
                taskEx = m_dbAdapter.Task.NewTaskExtension(taskEx);
                taskExId = taskEx.TaskExtensionId;
            }

            var task = new Task();
            task.MetaTaskId = metaTask.Id;
            task.ProjectId = metaTask.ProjectId;
            task.Description = metaTask.Name;
            task.StartTime = beginTime;
            task.EndTime = endTime;
            task.TaskExtensionId = taskExId;
            task.PreTaskIds = prevTaskIds;
            task.TaskDetail = metaTask.Detail;
            task.TaskTarget = metaTask.Target;
            task.TaskStatus = (DateTime.Today <= task.EndTime) ? TaskStatus.Waitting : TaskStatus.Overdue;

            return task;
        }
    }

}
