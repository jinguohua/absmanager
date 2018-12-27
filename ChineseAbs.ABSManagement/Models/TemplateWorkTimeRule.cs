using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TemplateWorkTimeRule : BaseDataContainer<TableTemplateWorkTimeRule>
    {

        public TemplateWorkTimeRule() { }
        public TemplateWorkTimeRule(TableTemplateWorkTimeRule templateWorkTimeRule) : base(templateWorkTimeRule) { }

        public int RuleId { get; set; }
        public string RuleGuid { get; set; }
        public string WorkTimeGuid { get; set; }
        public int? RuleType { get; set; }
        public int? SpecialDayType { get; set; }
        public int? MoveSpecialDayDirection { get; set; }
        public int? MoveSpecialDayCount { get; set; }
        public int? UnitType { get; set; }
        public int? UnitSequenceDay { get; set; }
        public int? RuleOrder { get; set; }
        public int? RecordStatusId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUserName { get; set; }

        public override TableTemplateWorkTimeRule GetTableObject()
        {
            TableTemplateWorkTimeRule tableObj = new TableTemplateWorkTimeRule
            {
                rule_id = RuleId,
                rule_guid = RuleGuid,
                work_time_guid = WorkTimeGuid,
                rule_type = RuleType,
                special_day_type = SpecialDayType,
                move_special_day_direction = MoveSpecialDayDirection,
                move_special_day_count = MoveSpecialDayCount,
                unit_type = UnitType,
                unit_seqence_day = UnitSequenceDay,
                rule_order = RuleOrder,
                record_status_id = RecordStatusId,
                create_time = CreateTime,
                create_user_name = CreateUserName
            };
            return tableObj;
        }

        public override void FromTableObject(TableTemplateWorkTimeRule tableObj)
        {
            RuleId = tableObj.rule_id;
            RuleGuid = tableObj.rule_guid;
            WorkTimeGuid = tableObj.work_time_guid;
            RuleType = tableObj.rule_type;
            SpecialDayType = tableObj.special_day_type;
            MoveSpecialDayDirection = tableObj.move_special_day_direction;
            MoveSpecialDayCount = tableObj.move_special_day_count;
            UnitType = tableObj.unit_type;
            UnitSequenceDay = tableObj.unit_seqence_day;
            RuleOrder = tableObj.rule_order;
            RecordStatusId = tableObj.record_status_id;
            CreateTime = tableObj.create_time;
            CreateUserName = tableObj.create_user_name;
        }
    }
}
