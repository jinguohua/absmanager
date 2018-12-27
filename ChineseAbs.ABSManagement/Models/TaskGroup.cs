using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class TaskGroup: BaseDataContainer<TableTaskGroup>
    {
        public TaskGroup()
        {
            this.Guid = System.Guid.NewGuid().ToString();
        }

        public TaskGroup(TableTaskGroup taskGroup)
            : base(taskGroup)
        {

        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Sequence { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserName { get; set; }

        public override TableTaskGroup GetTableObject()
        {
            var obj = new TableTaskGroup();
            obj.task_group_id = Id;
            obj.task_group_guid = Guid;
            obj.project_id = ProjectId;
            obj.name = Name;
            obj.description = Description;
            obj.sequence_number = Sequence;
            obj.record_status_id = (int)RecordStatus;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            return obj;
        }

        public override void FromTableObject(TableTaskGroup obj)
        {
            Id = obj.task_group_id;
            Guid = obj.task_group_guid;
            ProjectId = obj.project_id;
            Name = obj.name;
            Description = obj.description;
            Sequence = obj.sequence_number;
            RecordStatus = (RecordStatus)obj.record_status_id;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
        }
    }
}
