namespace ChineseAbs.ABSManagement.Models
{
    public class NoteData : BaseDataContainer<ABSMgrConn.TableNoteData>
    {
        public NoteData(ABSMgrConn.TableNoteData obj) : base(obj) { }

        public NoteData() { }

        public int NoteDataId { get; set; }

        public int NoteId { get; set; }

        public int DatasetId { get; set; }

        public decimal? PrincipalPaid { get; set; }

        public decimal? InterestPaid { get; set; }

        public decimal? EndingBalance { get; set; }

        public override ABSMgrConn.TableNoteData GetTableObject()
        {
            var t = new ABSMgrConn.TableNoteData();
            t.note_data_id = NoteDataId;
            t.note_id = NoteId;
            t.dataset_id = DatasetId;
            t.principal_paid = PrincipalPaid;
            t.interest_paid = InterestPaid;
            t.ending_balance = EndingBalance;
            return t;
        }

        public override void FromTableObject(ABSMgrConn.TableNoteData obj)
        {
            NoteDataId = obj.note_data_id;
            NoteId = obj.note_id;
            DatasetId = obj.dataset_id;
            PrincipalPaid = obj.principal_paid;
            InterestPaid = obj.interest_paid;
            EndingBalance = obj.ending_balance;
        }

        public bool HasValue { get { return PrincipalPaid.HasValue && InterestPaid.HasValue && EndingBalance.HasValue; } }
    }
}
