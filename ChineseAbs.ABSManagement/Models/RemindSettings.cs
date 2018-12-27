using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public enum EFrequency
    {
        H12 = 12,
        H24 = 15,
        H48 = 39
    }

    public enum ERemindType
    { 
        短信 = 1,
        邮件 = 2,
        短信加邮件 =3
    }

    public class RemindSettings : BaseDataContainer<TableRemindSettings>
    {

        #region Constructors
        public RemindSettings(TableRemindSettings rc)
            : base(rc)
        { }

        public RemindSettings() { }
        #endregion

        #region Properties
        public int RowId { get; set; }
        public bool? AutoRemind { get; set; }
        public int ProjectId { get; set; }
        public int Frequency { get; set; }
        public int? RemindType { get; set; }
        public bool? RemindDaily { get; set; }
        #endregion

        public override TableRemindSettings GetTableObject()
        {
            var rt = new TableRemindSettings();
            rt.row_id = this.RowId;
            rt.project_id = this.ProjectId;
            rt.auto_remind = this.AutoRemind;
            rt.frequency = this.Frequency;
            rt.remind_type = this.RemindType;
            rt.remind_daily = this.RemindDaily;
            return rt;
        }

        public override void FromTableObject(TableRemindSettings obj)
        {
            this.RowId = obj.row_id;
            this.ProjectId = obj.project_id;
            this.AutoRemind = obj.auto_remind;
            this.Frequency = obj.frequency;
            this.RemindType = obj.remind_type;
            this.RemindDaily = obj.remind_daily;
        }
    }
}
