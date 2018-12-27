using System;
using System.Collections.Generic;
using System.Linq;
using ABSMgrConn;
using ChineseAbs.ABSManagement.Utils;

namespace ChineseAbs.ABSManagement.Models
{
    public class Task : BaseDataContainer<TableTasks>
    {
        public Task()
        {
        }

        public Task(TableTasks task)
            : base(task)
        {
        }

        public int TaskId { get; set; }

        public string TaskGuid { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string TaskModuleId { get; set; }

        public string Description { get; set; }

        public string ShortCode { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public TaskStatus TaskStatus { get; set; }

        public List<int> PrevTaskIdArray { get; set; }

        public List<string> PrevTaskShortCodeArray { get; set; }

        public string PreTaskIds { get; set; }

        public int? TemplateTaskId { get; set; }

        public int? TaskExtensionId { get; set; }

        public TaskExtension TaskExtension { get; set; }

        public List<TaskStatusHistory> TaskStatusHistory { get; set; }

        public string TaskDetail { get; set; }

        public string TaskTarget { get; set; }

        public string TaskHandler { get; set; }

        public int? TaskGroupId { get; set; }

        public string PersonInCharge { get; set; }

        public int? MetaTaskId { get; set; }

        public override TableTasks GetTableObject()
        {
            var task = new TableTasks();
            task.task_id = TaskId;
            task.task_guid = TaskGuid;
            task.project_id = ProjectId;
            task.task_module_id = TaskModuleId;
            task.description = Description;
            task.short_code = ShortCode;
            task.start_time = StartTime;
            task.end_time = EndTime;
            task.task_status_id = (int)TaskStatus;
            task.pre_task_ids = PreTaskIds;
            task.template_task_id = TemplateTaskId;
            task.task_extension_id = TaskExtensionId;
            task.task_detail = TaskDetail;
            task.task_target = TaskTarget;
            task.task_handler = TaskHandler;
            task.task_group_id = TaskGroupId;
            task.meta_task_id = MetaTaskId;
            task.person_in_charge = PersonInCharge;
            return task;
        }

        public override void FromTableObject(TableTasks task)
        {
            TaskId = task.task_id;
            TaskGuid = task.task_guid;
            ProjectId = task.project_id;
            TaskModuleId = task.task_module_id;
            Description = task.description;
            ShortCode = task.short_code;
            StartTime = task.start_time;
            EndTime = task.end_time;
            TaskStatus = (TaskStatus)task.task_status_id;
            PreTaskIds = task.pre_task_ids;
            TemplateTaskId = task.template_task_id;

            if (task.pre_task_ids != null)
            {
                PrevTaskIdArray = CommUtils.Split(task.pre_task_ids).ToList()
                    .ConvertAll(item => int.Parse(item.Trim()));
            }
            else
            {
                PrevTaskIdArray = new List<int>();
            }

            TaskExtensionId = task.task_extension_id;
            TaskDetail = task.task_detail;
            TaskTarget = task.task_target;
            TaskHandler = task.task_handler;
            TaskGroupId = task.task_group_id;
            PersonInCharge = task.person_in_charge;
            MetaTaskId = task.meta_task_id;
        }
    }

    public class CashflowVariableStu
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined = 1,
        /// <summary>
        /// 等待
        /// </summary>
        Waitting = 2,
        /// <summary>
        /// 进行中
        /// </summary>
        Running = 3,
        /// <summary>
        /// 完成
        /// </summary>
        Finished = 4,
        /// <summary>
        /// 跳过
        /// </summary>
        Skipped = 5,
        /// <summary>
        /// 逾期
        /// </summary>
        Overdue = 6,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 7,
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 8
    }
}
