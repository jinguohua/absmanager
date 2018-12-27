using System;
using ABSMgrConn;

namespace ChineseAbs.ABSManagement.Models
{
    public class Investment : BaseDataContainer<TableInvestment>
    {
        public Investment()
        {
        }

        public Investment(TableInvestment investment)
            : base(investment)
        {
        }

        public int Id { get; set; }

        public string Guid { get; set; }

        public int ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Money { get; set; }

        public double? Gains { get; set; }

        public double? Yield { get; set; }

        public double? YieldDue { get; set; }

        public double? GainsDue { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime? AccountingTime { get; set; }

        public RecordStatus RecordStatus { get; set; }

        public override TableInvestment GetTableObject()
        {
            var obj = new TableInvestment();
            obj.investment_id = Id;
            obj.investment_guid = Guid;
            obj.project_id = ProjectId;
            obj.name = Name;
            obj.description = Description;
            obj.money = Money;
            obj.gains = Gains;
            obj.yield = Yield;
            obj.yieldDue = YieldDue;
            obj.gainsDue = GainsDue;
            obj.start_time = StartTime;
            obj.end_time = EndTime;
            obj.accounting_time = AccountingTime;
            obj.record_status_id = (int)RecordStatus;
            return obj;
        }

        public override void FromTableObject(TableInvestment obj)
        {
            Id = obj.investment_id;
            Guid = obj.investment_guid;
            ProjectId = obj.project_id;
            Name = obj.name;
            Description = obj.description;
            Money = obj.money;
            Gains = obj.gains;
            Yield = obj.yield;
            YieldDue = obj.yieldDue;
            GainsDue = obj.gainsDue;
            StartTime = obj.start_time;
            EndTime = obj.end_time;
            AccountingTime = obj.accounting_time;
            RecordStatus = (RecordStatus)obj.record_status_id;
        }
    }
}
