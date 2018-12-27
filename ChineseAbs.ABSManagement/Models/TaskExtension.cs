using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class TaskExtension : BaseDataContainer<TableTaskExtensions>
    {
        public TaskExtension()
        {
        }

        public TaskExtension(TableTaskExtensions taskExtension)
            : base(taskExtension)
        {
        }

        public int TaskExtensionId { get; set; }

        public string TaskExtensionName { get; set; }

        public string TaskExtensionType { get; set; }

        public string TaskExtensionInfo { get; set; }

        public TaskExtensionStatus TaskExtensionStatus { get; set; }

        public string TaskExtensionHandler { get; set; }

        public DateTime? TaskExtensionHandleTime { get; set; }

        public override TableTaskExtensions GetTableObject()
        {
            var obj = new TableTaskExtensions();
            obj.task_extension_id = TaskExtensionId;
            obj.task_extension_name = TaskExtensionName;
            obj.task_extension_type = TaskExtensionType;
            obj.task_extension_info = TaskExtensionInfo;
            
            if (TaskExtensionStatus == ABSManagement.Models.TaskExtensionStatus.Undefined)
            {
                obj.task_extension_status = null;
            }
            else
            {
                obj.task_extension_status = (int)TaskExtensionStatus;
            }
            obj.task_extension_handler = TaskExtensionHandler;
            obj.task_extension_handle_time = TaskExtensionHandleTime;
            return obj;
        }

        public override void FromTableObject(TableTaskExtensions obj)
        {
            TaskExtensionId = obj.task_extension_id;
            TaskExtensionName = obj.task_extension_name;
            TaskExtensionType = obj.task_extension_type;
            TaskExtensionInfo = obj.task_extension_info;
            if (obj.task_extension_status == null)
            {
                TaskExtensionStatus = ABSManagement.Models.TaskExtensionStatus.Undefined;
            }
            else
            {
                TaskExtensionStatus = (TaskExtensionStatus)obj.task_extension_status.Value;
            }
            TaskExtensionHandler = obj.task_extension_handler;
            TaskExtensionHandleTime = obj.task_extension_handle_time;
        }

        static public TaskExtension Create(TaskExtensionType taskExType)
        {
            return new TaskExtension
            {
                TaskExtensionName = taskExType.ToString(),
                TaskExtensionType = taskExType.ToString(),
                TaskExtensionInfo = string.Empty,
                TaskExtensionStatus = TaskExtensionStatus.Waitting
            };
        }
    }

    /// <summary>
    /// 扩展任务类型
    /// </summary>
    public enum TaskExtensionType
    {
        /// <summary>
        /// 无扩展任务
        /// </summary>
        None = 0,
        /// <summary>
        /// 租金回收计算日
        /// </summary>
        AssetCashflow = 1,
        /// <summary>
        /// 证券偿付计算
        /// </summary>
        Cashflow = 2,
        /// <summary>
        /// 回收款转付日
        /// </summary>
        RecyclingPaymentDate = 3,
        /// <summary>
        /// 上传/下载文档
        /// </summary>
        Document = 4,
        /// <summary>
        /// 工作要点检查
        /// </summary>
        CheckList = 5,
        /// <summary>
        /// 上报文件整合
        /// </summary>
        DocumentCheckList = 6,
        /// <summary>
        /// 建元Demo用的扩展工作（上传Excel生成报告）
        /// </summary>
        DemoJianYuanReport = 80
    }


    /// <summary>
    /// 扩展任务状态
    /// </summary>
    public enum TaskExtensionStatus
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined = -1,
        /// <summary>
        /// 等待
        /// </summary>
        Waitting = 0,
        /// <summary>
        /// 完成
        /// </summary>
        Finished = 1,
        /// <summary>
        /// 数据不匹配（错误）
        /// </summary>
        NotMatch = 2,
    }
}
