using ABS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class FeeRule
    {
        public FeeRule()
        {
            this.FeeComponents = new List<FeeComponent>();
        }

        public List<FeeComponent> FeeComponents { get; set; }
    }

    public class FeeComponent
    {
        public FeeComponent() { }

        public FeeComponent(string name, string description, double rate, bool isPerPaymentRate, bool hasDiffFirstRate, double? firstRate, int paymentOrdinal)
        {
            Name = name;
            Description = description;
            NoFixedAmount = true;
            IsProRated = true;
            IsFixedRate = true;
            FixedRate = rate;
            IsPerPaymentRate = isPerPaymentRate;
            FeeBasisType = EBasisType.APB;
            PaymentOrdinal = paymentOrdinal;
            HasDiffFirstRate = hasDiffFirstRate;
            if (firstRate != null)
                FirstRate = (double)firstRate;

        }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool NoFixedAmount { get; set; }

        public double FixedAmount { get; set; }

        public bool IsProRated { get; set; }

        public bool IsFixedRate { get; set; }

        public bool IsPerPaymentRate { get; set; }

        public bool NeedUnpaidAccount { get; set; }

        public bool NeedInterestOnUnpaidAccount { get; set; }

        public double FixedRate { get; set; }

        public double Spread { get; set; }

        public string FloatingIndex { get; set; }

        public bool HasDiffFirstRate { get; set; }

        public double FirstRate { get; set; }

        public string Cap { get; set; }

        public bool NeedPriorityExpenseCap { get; set; }

        public string PriorityExpenseCap { get; set; }

        public string Floor { get; set; }

        public bool IsCumulativeCap { get; set; }

        public EAccrualMethod AccrualMethod { get; set; }

        public int PaymentOrdinal { get; set; }

        public EBasisType FeeBasisType { get; set; }

        public string FeeRateString
        {
            get
            {
                if (!IsProRated)
                {
                    return "-";
                }
                if (IsFixedRate)
                {
                    return FixedRate.ToString("P2");
                }
                else
                {
                    return "F + " + (this.Spread / 10000).ToString("P2");
                }
            }
        }

        public List<DateValue> PaymentSchedules { get; set; }
    }
    public class DateValue
    {
        public DateTime Date { get; set; }

        public double? Value { get; set; }
    }
}
