using ABS.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class ScheduleRule
    {
        public ScheduleRule()
        {
            this.FloatingIndecies = new List<FloatingIndexData>();
        }

        public DateTime ClosingDate { get; set; }

        public DateTime FirstPaymentDate { get; set; }

        public DateTime LegalMaturity { get; set; }

        public DateTime PenultimateDate { get; set; }

        public EDealLabFrequency PaymentFrequency { get; set; }

        public bool PaymentAtMonthEnd { get; set; }

        public EDeterminationRules DeterminationRuleType { get; set; }

        public int DeterminationOffset { get; set; }

        public DateTime FirstDeterminationDateBegin { get; set; }

        public DateTime FirstDeterminationDateEnd { get; set; }

        public ERolling PaymentRolling { get; set; }

        public ERolling DeterminationDateRolling { get; set; }

        public bool DeterminationDateAtMonthEnd { get; set; }

        public DateTime ReinvestmentEndDate { get; set; }

        public List<FloatingIndexData> FloatingIndecies { get; set; }

        public bool AdjustCollectionSchedule { get; set; }

        public bool AdjustPaymentSchedule { get; set; }

        public List<AdjustScheduleRule> CustomPaymentSchedule { get; set; }

        public List<AdjustScheduleRule> CustomCollectionSchedule { get; set; }

        public DateTime CloseAssetDate { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ScheduleRule Deserialized(string json)
        {
            return JsonConvert.DeserializeObject<ScheduleRule>(json);
        }

    }

    public class FloatingIndexData
    {
        public FloatingIndexData(string code)
        {
            this.Code = code;
        }

        public string Code { get; set; }

        public bool OverwriteFirstRate { get; set; }

        public double FirstRate { get; set; }

        public int RateDeterminatinOffset { get; set; }
    }

    public class AdjustScheduleRule
    {
        public string OrigDate { get; set; }

        public string CustomDate { get; set; }
    }
}
