using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TemplateWork : BaseDataContainer<TableTemplateWork>
    {
        public TemplateWork() { }

        public TemplateWork(TableTemplateWork templateWork):base(templateWork) {}
        
        public int WorkId { get; set; }
        public string WorkGuid { get; set; }
        public string TemplateGuid { get; set; }
        public string WorkName { get; set; }
        public string WorkTarget { get; set; }
        public string WorkDescription { get; set; }
        public string ExtensionType { get; set; }
        public string PreviousWork { get; set; }
        public int? RecordStatusId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUserName { get; set; }

        public override TableTemplateWork GetTableObject()
        {
            TableTemplateWork tableObj = new TableTemplateWork
            {
                work_id = WorkId,
                work_guid = WorkGuid,
                template_guid = TemplateGuid,
                work_name = WorkName,
                work_target = WorkTarget,
                work_description = WorkDescription,
                extension_type = ExtensionType,
                previous_work = PreviousWork,
                record_status_id = RecordStatusId,
                create_time = CreateTime,
                create_user_name = CreateUserName
            };
            return tableObj;
        }

        public override void FromTableObject(TableTemplateWork tableObj)
        {
            WorkId = tableObj.work_id;
            WorkGuid = tableObj.work_guid;
            TemplateGuid = tableObj.template_guid;
            WorkName = tableObj.work_name;
            WorkTarget = tableObj.work_target;
            WorkDescription = tableObj.work_description;
            ExtensionType = tableObj.extension_type;
            PreviousWork = tableObj.previous_work;
            RecordStatusId = tableObj.record_status_id;
            CreateTime = tableObj.create_time;
            CreateUserName = tableObj.create_user_name;
        }
    }

    public class WorkCalculateTime
    {
        /// <summary>
        /// 工作id
        /// </summary>
        public string WorkId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 开始时间有依赖工作
        /// </summary>
        public bool HasStartBaseWork { get; set; }
        /// <summary>
        /// 结束时间有依赖工作
        /// </summary>
        public bool HasEndBaseWork { get; set; }
        /// <summary>
        /// 开始时间依赖的工作id
        /// </summary>
        public string StartBaseWorkId { get; set; }
        /// <summary>
        /// 结束时间依赖的工作id
        /// </summary>
        public string EndBaseWorkId { get; set; }
        /// <summary>
        /// 开始时间依赖工作的开始或结束时间
        /// </summary>
        public int StartBaseStartOrEndTime { get; set; }
        /// <summary>
        /// 结束时间依赖工作的开始或结束时间
        /// </summary>
        public int EndBaseStartOrEndTime { get; set; }
        /// <summary>
        /// 开始时间设定好了
        /// </summary>
        public bool StartSetOk { get; set; }
        /// <summary>
        /// 结束时间设定好了
        /// </summary>
        public bool EndSetOk { get; set; }
        /// <summary>
        /// 开始工作时间的Id
        /// </summary>
        public string StartWorkTimeId { get; set; }
        /// <summary>
        /// 结束工作时间的Id
        /// </summary>
        public string EndWorkTimeId { get; set; }
    }
}

 

