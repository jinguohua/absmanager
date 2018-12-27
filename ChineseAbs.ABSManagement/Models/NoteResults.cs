namespace ChineseAbs.ABSManagement.Models
{
    public class NoteResults : BaseDataContainer<ABSMgrConn.TableNoteResults>
    {
        public NoteResults(ABSMgrConn.TableNoteResults obj) : base(obj) { }

        public NoteResults() { }

        public int ResultId { get; set; }

        public int ProjectId { get; set; }

        public int DatasetId { get; set; }

        public string NoteResultTable { get; set; }

        public string NoteCashflowTable { get; set; }


        public override ABSMgrConn.TableNoteResults GetTableObject()
        {
            var t = new ABSMgrConn.TableNoteResults();
            t.result_id = ResultId;
            t.project_id = ProjectId;
            t.dataset_id = DatasetId;
            t.note_result_table = NoteResultTable;
            t.note_cashflow_table = NoteCashflowTable;
            return t;
        }

        public override void FromTableObject(ABSMgrConn.TableNoteResults obj)
        {
            ResultId = obj.result_id;
            ProjectId = obj.project_id;
            DatasetId = (int)obj.dataset_id;
            NoteResultTable = obj.note_result_table;
            NoteCashflowTable = obj.note_cashflow_table;
        }
    }
}
