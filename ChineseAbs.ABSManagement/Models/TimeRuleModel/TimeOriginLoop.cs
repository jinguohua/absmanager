using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;
using System;

namespace ChineseAbs.ABSManagement.Models.TimeRuleModel
{
    public class TimeOriginLoop : BaseModel<TableTimeOriginLoop>
    {
        public TimeOriginLoop()
        {

        }

        public TimeOriginLoop(TableTimeOriginLoop timeOriginLoop)
            : base(timeOriginLoop)
        {

        }

        public int TimeSeriesId { get; set; }

        public DateTime LoopBegin { get; set; }

        public DateTime LoopEnd { get; set; }

        public int LoopInterval { get; set; }

        public string TimeRuleUnitType { get; set; }


        public override TableTimeOriginLoop GetTableObject()
        {
            var obj = new TableTimeOriginLoop();
            obj.time_origin_loop_id = Id;
            obj.time_origin_loop_guid = Guid;
            obj.time_series_id = TimeSeriesId;
            obj.loop_begin = LoopBegin;
            obj.loop_end = LoopEnd;
            obj.loop_interval = LoopInterval;
            obj.time_rule_unit_type_id = (int)CommUtils.ParseEnum<TimeRuleUnitType>(TimeRuleUnitType);
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.last_modify_time = LastModifyTime;
            obj.last_modify_user_name = LastModifyUserName;
            obj.record_status_id = (int)RecordStatus;

            return obj;
        }

        public override void FromTableObject(TableTimeOriginLoop obj)
        {
            Id = obj.time_origin_loop_id;
            Guid = obj.time_origin_loop_guid;
            TimeSeriesId = obj.time_series_id;
            LoopBegin = obj.loop_begin;
            LoopEnd = obj.loop_end;
            LoopInterval = obj.loop_interval;
            TimeRuleUnitType = ((TimeRuleUnitType)obj.time_rule_unit_type_id).ToString();
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            LastModifyTime = obj.last_modify_time;
            LastModifyUserName = obj.last_modify_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
