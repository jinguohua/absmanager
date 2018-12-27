using ABS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class CreditEnhancementRule
    {
        public CreditEnhancementRule()
        {
            HasEod = true;
            TurboCancure = false;
            EodCanCure = false;
            HasRiskReserve = false;
            PayWaterfallByCombinedProceeds = false;
            LiquidateWhenEod = false;
            PaySubResidualInterestBeforeOtherNotesPaidOff = false;
        }

        public EPaymentSequence DeficiencyPaymentSequence { get; set; }

        public bool HasEodExpectedMaturityTurbo { get; set; }

        public EEodCheckCondition? EodCheckCondition { get; set; }

        public bool HasEod { get; set; }

        public bool EodCheckHighestPriorityNoteOnly { get; set; }

        public bool EodCanCure { get; set; }

        public bool HasTurbo
        {
            get
            {
                return HasCumlossTurbo || HasNoteExpectedMaturityTurbo;
            }
        }

        public bool TurboCancure { get; set; }

        public bool HasCumlossTurbo { get; set; }

        public double CumlossTurboThreshold { get; set; }

        public bool HasCumlossTurboThresholdSchedule { get; set; }

        public SimpleScheduleData CumlossTurboThresholdSchedule { get; set; }

        public EBasisType CumlossTurboBasisType { get; set; }

        public double OriginalAPB { get; set; }

        public bool HasNoteExpectedMaturityTurbo { get; set; }

        public bool HasRiskReserve { get; set; }

        public double RiskReserveCap { get; set; }

        public double RiskReserveInterestRate { get; set; }

        public double RiskReserveInitValue { get; set; }

        public bool RiskReserveForSeniorOnly { get; set; }

        public bool HasDeficiencyPayment { get; set; }

        public double DeficiencyPaymentCap { get; set; }

        public bool HasGuarantee { get; set; }

        public double GuaranteeCap { get; set; }

        public double MarginInitValue { get; set; }

        public bool UseLeaseMarginAsDeficiencyPayment { get; set; }

        public bool PayWaterfallByCombinedProceeds { get; set; }

        public bool LiquidateWhenEod { get; set; }

        public bool PaySubResidualInterestBeforeOtherNotesPaidOff { get; set; }

        public int RatingUpgradeByExternalCreditEnhancement
        {
            get
            {
                if (HasGuarantee || HasDeficiencyPayment)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
