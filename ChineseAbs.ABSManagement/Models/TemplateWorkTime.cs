using System;
using System.Collections.Generic;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TemplateWorkTime : BaseDataContainer<TableTemplateWorkTime>
    {
        public TemplateWorkTime() { }
        public TemplateWorkTime(TableTemplateWorkTime templateWorkTime) : base(templateWorkTime) { }

        public int WorkTimeId { get; set; }
        public string WorkTimeGuid { get; set; }
        public string WorkGuid { get; set; }
        public int TimeType { get; set; }
        public int TimeOrigin { get; set; }
        public int? NewBuiltType { get; set; }
        public string CycleStartTime { get; set; }
        public int? CycleGapTime { get; set; }
        public int? CycleUnitType { get; set; }
        public string CycleMaxTime { get; set; }
        public string AppointTimeString { get; set; }
        public int? BaseWorkId { get; set; }
        public int? BaseStartOrEndTime { get; set; }
        public int? RecordStatusId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUserName { get; set; }

        public string BaseWorkGuid { get; set; }
        public List<TemplateWorkTimeRule> Rules { get; set; }

        public override TableTemplateWorkTime GetTableObject()
        {
            TableTemplateWorkTime tableObj = new TableTemplateWorkTime
            {
                work_time_id = WorkTimeId,
                work_time_guid = WorkTimeGuid,
                work_guid = WorkGuid,
                time_type = TimeType,
                time_origin = TimeOrigin,
                new_built_type = NewBuiltType,
                cycle_start_time = CycleStartTime,
                cycle_gap_time = CycleGapTime,
                cycle_unit_type = CycleUnitType,
                cycle_max_time = CycleMaxTime,
                appoint_time_string = AppointTimeString,
                base_work_id = BaseWorkId,
                base_time = BaseStartOrEndTime,
                record_status_id = RecordStatusId,
                create_time = CreateTime,
                create_user_name = CreateUserName
            };
            return tableObj;
        }

        public override void FromTableObject(TableTemplateWorkTime tableObj)
        {
            WorkTimeId = tableObj.work_time_id;
            WorkTimeGuid = tableObj.work_time_guid;
            WorkGuid = tableObj.work_guid;
            TimeType = tableObj.time_type;
            TimeOrigin = tableObj.time_origin;
            NewBuiltType = tableObj.new_built_type;
            CycleStartTime = tableObj.cycle_start_time;
            CycleGapTime = tableObj.cycle_gap_time;
            CycleUnitType = tableObj.cycle_unit_type;
            CycleMaxTime = tableObj.cycle_max_time;
            AppointTimeString = tableObj.appoint_time_string;
            BaseWorkId = tableObj.base_work_id;
            BaseStartOrEndTime = tableObj.base_time;
            RecordStatusId = tableObj.record_status_id;
            CreateTime = tableObj.create_time;
            CreateUserName = tableObj.create_user_name;
        }



    }
}
