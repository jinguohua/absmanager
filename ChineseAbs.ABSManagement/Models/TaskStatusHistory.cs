using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TaskStatusHistory : BaseDataContainer<TableTaskStatusHistory>
    {
        public TaskStatusHistory()
        {
        }

        public TaskStatusHistory(TableTaskStatusHistory task)
            : base(task)
        {
        }

        public int TaskStatusHistoryId { get; set; }

        public int TaskId { get; set; }

        public TaskStatus PrevStatusId { get; set; }

        public TaskStatus NewStatusId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string TimeStampUserName { get; set; }

        public string Comment { get; set; }

        public override TableTaskStatusHistory GetTableObject()
        {
            var obj = new TableTaskStatusHistory();
            obj.task_status_history_id = TaskStatusHistoryId;
            obj.task_id = TaskId;
            obj.prev_status_id = (int)PrevStatusId;
            obj.new_status_id = (int)NewStatusId;
            obj.time_stamp = TimeStamp;
            obj.time_stamp_user_name = TimeStampUserName;
            obj.comment = Comment;
            return obj;
        }

        public override void FromTableObject(TableTaskStatusHistory obj)
        {
            TaskStatusHistoryId = obj.task_status_history_id;
            TaskId = obj.task_id;
            if (obj.prev_status_id.HasValue)
            {
                PrevStatusId = (TaskStatus)obj.prev_status_id.Value;
            }
            
            if (obj.new_status_id.HasValue)
            {
                NewStatusId = (TaskStatus)obj.new_status_id.Value;
            }

            if (obj.time_stamp.HasValue)
            {
                TimeStamp = obj.time_stamp.Value;
            }

            TimeStampUserName = obj.time_stamp_user_name;
            Comment = obj.comment;
        }
    }
}
