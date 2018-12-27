using ABSMgrConn;
using System;

namespace ChineseAbs.ABSManagement.Models
{
    public enum AgendaStatus
    {
        Undefined = 1,
        Waitting = 2,
        Running = 3,
        Finished = 4,
        Skipped = 5,
        Overdue = 6,
        Error = 7
    }

    public class Agenda : BaseModel<TableAgenda>
    {
        public Agenda()
        {

        }

        public Agenda(TableAgenda agenda)
            : base(agenda)
        {

        }

        public int ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AgendaStatus AgendaStatus { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public override TableAgenda GetTableObject()
        {
            var obj = new TableAgenda();
            obj.agenda_id = Id;
            obj.agenda_guid = Guid;
            obj.project_id = ProjectId;
            obj.agenda_name = Name;
            obj.description = Description;
            obj.agenda_status_id = (int)AgendaStatus;
            obj.start_time = StartTime;
            obj.end_time = EndTime;
            obj.create_time = CreateTime;
            obj.create_user_name = CreateUserName;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableAgenda obj)
        {
            Id = obj.agenda_id;
            Guid = obj.agenda_guid;
            ProjectId = obj.project_id;
            Name = obj.agenda_name;
            Description = obj.description;
            AgendaStatus = (AgendaStatus)obj.agenda_status_id;
            StartTime = obj.start_time;
            EndTime = obj.end_time;
            CreateTime = obj.create_time;
            CreateUserName = obj.create_user_name;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
