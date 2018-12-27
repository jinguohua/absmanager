using ChineseAbs.ABSManagement.Models.TimeRuleModel;
using ChineseAbs.ABSManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChineseAbs.ABSManagementSite.Models
{
    public interface ICheckViewModelParam
    {
        void CheckParam();
    }

    public class TimeRuleViewModel
    {
        public TimeRuleViewModel()
        {
            TimeRuleOrder = new List<string>();
            PeriodSequenceList = new List<PeriodSequence>();
            ConditionShiftList = new List<ConditionShift>();
            ShiftList = new List<Shift>();
            RemoveRepeatDateList = new List<RemoveRepeatDate>();
        }

        public bool IsExistRule { set; get; }

        public List<string> TimeRuleOrder { get; set; }

        public string TimeSeriesGuid { get; set; }

        public List<PeriodSequence> PeriodSequenceList { get; set; }

        public List<ConditionShift> ConditionShiftList { get; set; }

        public List<Shift> ShiftList { get; set; }

        public List<RemoveRepeatDate> RemoveRepeatDateList { get; set; }
    }

    public class TimeOriginViewModel
    {
        public TimeOriginViewModel()
        {
            CustomInput = new CustomInput();
            Loop = new Loop();
            PrevMetaTask = new PrevMetaTask();
            TaskSelfTime = new TaskSelfTime();
        }

        public string TimeSeriesGuid { get; set; }

        public string TimeOriginType { get; set; }

        public CustomInput CustomInput { get; set; }

        public Loop Loop { get; set; }

        public PrevMetaTask PrevMetaTask { get; set; }

        public TaskSelfTime TaskSelfTime { get; set; }
    }

    public class PeriodSequence : ICheckViewModelParam
    {
        public int Ranking { get; set; }

        public string PeriodType { get; set; }

        public int Interval { get; set; }

        public string ConditionUnitType { get; set; }

        public void CheckParam()
        {
            CommUtils.Assert(Interval > 0 && Interval <= 365, "规则[查找指定日期]中的天数必须为1-365之间的整数");
            CommUtils.ParseEnum<TimeRuleDateType>(ConditionUnitType);
            var type = CommUtils.ParseEnum<TimeRulePeriodType>(PeriodType);
        }
    }

    public class ConditionShift : ICheckViewModelParam
    {
        public int Ranking { get; set; }

        public string ConditionUnitType { get; set; }

        public string DateType { get; set; }

        public int Interval { get; set; }

        public string TimeMoveDirection { get; set; }

        public void CheckParam()
        {
            CommUtils.Assert(Interval > 0 && Interval <= 365, "规则[替换指定日期]中的查找天数必须为1-365之间的整数");
            CommUtils.ParseEnum<TimeRuleUnitType>(ConditionUnitType);
            CommUtils.ParseEnum<TimeRuleDateType>(DateType);
            CommUtils.ParseEnum<TimeMoveDirection>(TimeMoveDirection);
        }
    }

    public class Shift : ICheckViewModelParam
    {
        public int Ranking { get; set; }

        public string ConditionUnitType { get; set; }

        public int Interval { get; set; }

        public string TimeMoveDirection { get; set; }

        public void CheckParam()
        {
            CommUtils.Assert(Interval > 0 && Interval <= 365, "规则[移动指定日期]中的移动天数必须为1-365之间的整数");
            CommUtils.ParseEnum<TimeMoveDirection>(TimeMoveDirection);
            CommUtils.ParseEnum<TimeRuleDateType>(ConditionUnitType);
        }
    }

    public class RemoveRepeatDate : ICheckViewModelParam
    {
        public int Ranking { get; set; }

        public bool IsRemoveRepeatDate { get; set; }

        public void CheckParam() { }
    }

    public class CustomInput : ICheckViewModelParam
    {
        public string CustomTimeList { get; set; }

        public void CheckParam()
        {
            var customTimeList = CommUtils.Split(CustomTimeList).ToList();
            DateTime time;
            customTimeList.ForEach(x => CommUtils.Assert(DateTime.TryParse(x, out time),"时间列表中的日期[{0}]的格式不正确",x));
        }
    }

    public class Loop : ICheckViewModelParam
    {
        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public string RuleUnitType { get; set; }

        public int Interval { get; set; }

        public void CheckParam()
        {
            CommUtils.AssertHasContent(BeginTime,"第一个日期不能为空");
            CommUtils.AssertHasContent(EndTime, "截止日期不能为空");
            DateTime biginTime;
            DateTime endTime;
            DateTime.TryParse(BeginTime, out biginTime);
            DateTime.TryParse(EndTime, out endTime);
            CommUtils.Assert(DateTime.TryParse(BeginTime, out biginTime), "第一个日期[{0}]的格式不正确", BeginTime);
            CommUtils.Assert(DateTime.TryParse(EndTime, out endTime), "截止日期[{0}]的格式不正确", EndTime);
            CommUtils.Assert(biginTime <= endTime, "第一个日期[{0}]必须小于等于截止日期[{1}]", BeginTime, EndTime);
            CommUtils.Assert(Interval > 0 && Interval <= 365,"循环周期中的数字必须为1-365之间的整数");
            CommUtils.ParseEnum<TimeRuleUnitType>(RuleUnitType);
        }
    }

    public class PrevMetaTask : ICheckViewModelParam
    {
        public string MetaTaskGuid { get; set; }

        public string MetaTaskTimeType { get; set; }

        public void CheckParam()
        {
            CommUtils.AssertHasContent(MetaTaskGuid,"前置模板工作的Guid不能为空");

            CommUtils.ParseEnum<MetaTaskTimeType>(MetaTaskTimeType);
        }
    }

    public class TaskSelfTime : ICheckViewModelParam
    {
        public string TimeSeriesGuid { get; set; }

        public string TimeType { get; set; }

        public void CheckParam()
        {
            var timeTypeEN = CommUtils.ParseEnum<MetaTaskTimeType>(TimeType);

            var timeTypeCN = timeTypeEN == MetaTaskTimeType.StartTime ? "开始时间" : "截止时间";

            CommUtils.AssertHasContent(TimeSeriesGuid, "当前工作的{0}不能为空", timeTypeCN);
        }
    }
}