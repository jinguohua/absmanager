using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class Note : BaseDataContainer<ABSMgrConn.TableNote>
    {
        public Note(ABSMgrConn.TableNote obj) : base(obj) { }

        public Note() { }

        public int? ProjectId { get; set; }

        public int NoteId { get; set; }

        public string NoteName { get; set; }

        public string ShortName { get; set; }

        public decimal? Notional { get; set; }

        public bool IsEquity { get; set; }

        // 证券代码，可以在cnabs数据库中取得
        public string SecurityCode { get; set; }

        public string CouponString { get; set; }

        public DateTime? ExpectedMaturityDate { get; set; }

        public string AccrualMethod { get; set; }


        public override ABSMgrConn.TableNote GetTableObject()
        {
            var t = new ABSMgrConn.TableNote();
            t.project_id = ProjectId;
            t.note_id = (int)NoteId;
            t.note_name = NoteName;
            t.notional = Notional;
            t.short_name = ShortName;
            t.is_equity = IsEquity;
            return t;
        }

        public override void FromTableObject(ABSMgrConn.TableNote obj)
        {
            ProjectId = obj.project_id;
            NoteId = obj.note_id;
            NoteName = obj.note_name;
            ShortName = obj.short_name;
            Notional = obj.notional;
            if (!obj.is_equity.HasValue)
            {
                throw new ApplicationException("Table [Note] data error, is_equity is null, note id = [" + obj.note_id + "]");
            }
            IsEquity = obj.is_equity.Value;
        }
    }
}
