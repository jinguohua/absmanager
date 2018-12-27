using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public enum TimeSpanUnit
    {
        Year,
        Month,
        Day
    }

    public enum TemplateTimeType
    {
        TradingDay,
        WorkingDay,
        NaturalDay
    }

    public enum TemplateTimeSearchDirection
    {
        Forward,
        Backward
    }

    public enum TemplateTimeHandleReduplicate
    {
        Ignore,
        Reserve
    }

    public class TemplateTime : BaseDataContainer<TableTemplateTime>
    {
        public TemplateTime()
        { 
        }

        public TemplateTime(ABSMgrConn.TableTemplateTime templateTime)
            : base(templateTime)
        { 
        }

        public int TemplateTimeId { set; get; }

        public int TemplateId { set; get; }

        public string TemplateTimeName { set; get; }

        public DateTime BeginTime { set; get; }

        public DateTime EndTime { set; get; }

        public int TimeSpan { set; get; }

        public TimeSpanUnit TimeSpanUnit { set; get; }

        public TemplateTimeType TemplateTimeType { set; get; }

        public TemplateTimeSearchDirection SearchDirection { set; get; }

        public TemplateTimeHandleReduplicate HandleReduplicate { set; get; }

        public override TableTemplateTime GetTableObject()
        {
            var templateTime = new TableTemplateTime();
            templateTime.template_time_id = TemplateTimeId;
            templateTime.template_id = TemplateId;
            templateTime.template_time_name = TemplateTimeName;
            templateTime.begin_time = BeginTime;
            templateTime.end_time = EndTime;
            templateTime.time_span = TimeSpan;
            templateTime.time_span_unit = TimeSpanUnit.ToString();
            templateTime.template_time_type = TemplateTimeType.ToString();
            templateTime.search_direction = SearchDirection.ToString();
            templateTime.handle_reduplicate = HandleReduplicate.ToString();
            return templateTime;
        }

        public override void FromTableObject(TableTemplateTime templateTime)
        {
            TemplateTimeId = templateTime.template_time_id;
            TemplateId = templateTime.template_id;
            TemplateTimeName = templateTime.template_time_name;
            BeginTime = templateTime.begin_time;
            EndTime = templateTime.end_time;
            TimeSpan = templateTime.time_span;
            TimeSpanUnit = CommUtils.ParseEnum<TimeSpanUnit>(templateTime.time_span_unit);
            TemplateTimeType = CommUtils.ParseEnum<TemplateTimeType>(templateTime.template_time_type);
            SearchDirection = CommUtils.ParseEnum<TemplateTimeSearchDirection>(templateTime.search_direction);
            HandleReduplicate = CommUtils.ParseEnum<TemplateTimeHandleReduplicate>(templateTime.handle_reduplicate); 
        }
    }
}
