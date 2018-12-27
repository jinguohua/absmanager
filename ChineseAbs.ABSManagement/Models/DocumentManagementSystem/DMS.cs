using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models.DocumentManagementSystem
{
    public class DMS : BaseDataContainer<TableDMS>
    {
        public DMS()
        {

        }

        public DMS(TableDMS dms)
            : base(dms)
        {

        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableDMS GetTableObject()
        {
            var obj = new TableDMS();
            obj.dms_id = Id;
            obj.dms_guid = Guid;
            obj.project_id = ProjectId;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableDMS obj)
        {
            Id = obj.dms_id;
            Guid = obj.dms_guid;
            ProjectId = obj.project_id;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
