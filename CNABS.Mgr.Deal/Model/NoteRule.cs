using ABS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class NoteRule
    {
        public NoteRule()
        {
            this.NoteList = new List<NoteData>();
        }

        public decimal TotalBalance { get; set; }

        public List<NoteData> NoteList { get; set; }

        private Dictionary<string, double> m_pcts = new Dictionary<string, double>();
        public Dictionary<string, double> TierPercentages
        {
            get
            {
                m_pcts.Clear();
                if (TotalNotional == 0)
                {
                    return m_pcts;
                }

                for (int i = 1; i <= (int)Math.Floor(NoteList.Max(x => x.PaymentOrdinal)); i++)
                {
                    m_pcts.Add("Tier" + i.ToString(),
                        NoteList.Where(x => (int)Math.Floor(x.PaymentOrdinal) == i).Sum(x => x.Notional) / TotalNotional);
                }
                return m_pcts;
            }
        }

        public double TotalNotional
        {
            get
            {
                return this.NoteList.Sum(x => x.Notional);
            }
        }

        public List<NoteData>[] GetNotePriorityStack(bool excludeEquity)
        {
            int current = 0;
            List<NoteData> currentList = new List<NoteData>();
            List<List<NoteData>> rt = new List<List<NoteData>>();
            foreach (var n in NoteList.OrderBy(x => x.PaymentOrdinal))
            {
                if (excludeEquity && n.IsEquity)
                {
                    continue;
                }
                int floor = (int)Math.Floor(n.PaymentOrdinal);
                if (floor > current)
                {
                    if (currentList.Count > 0)
                    {
                        rt.Add(currentList);
                    }
                    currentList = new List<NoteData>();
                    current = floor;
                }
                currentList.Add(n);
            }
            if (currentList.Count > 0)
            {
                rt.Add(currentList);
            }
            return rt.ToArray();
        }
    }

    public class NoteData
    {
        public NoteData(string name, double notional, double fixedRate, bool isEquity, double paymentOrdinal)
            : this()
        {
            this.Name = name;
            this.Notional = notional;
            this.FixedRate = fixedRate;
            this.IsFixed = true;
            this.IsEquity = isEquity;
            this.PaymentOrdinal = paymentOrdinal;
        }

        public NoteData()
        {
            this.UnAdjustedAccrualPeriods = true;
            this.AccrualMethod = EAccrualMethod.Act_365;
            this.AmortizationSchedule = new SimpleScheduleData();
        }

        public double? NotionalAmount { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DescriptionForReport
        {
            get
            {
                if (IsEquity)
                {
                    return "次级证券";
                }
                if (PaymentOrdinal >= 1 && PaymentOrdinal < 2)
                {
                    if (HasAmortizationSchedule)
                    {
                        return "优先摊还";
                    }
                    else
                    {
                        return "优先过手";
                    }
                }
                else if (PaymentOrdinal >= 2)
                {
                    return "夹层级";
                }
                return "-";
            }
        }

        public bool IsFixed { get; set; }

        public bool IsEquity { get; set; }

        public double PaymentOrdinal { get; set; }

        public double Spread { get; set; }

        public double FixedRate { get; set; }

        public string CouponFormulaForReport
        {
            get
            {
                if (IsFixed)
                {
                    return FixedRate.ToString("P2");
                }
                else
                {
                    if (Spread >= 0)
                    {
                        return "基准 + " + Spread.ToString("P2");
                    }
                    else
                    {
                        return "基准 " + Spread.ToString("P2").Replace("-", "- ");
                    }
                }
            }
        }

        public double Notional { get; set; }

        public bool HasUnpaidInterest { get; set; }

        public bool HasInterestOnUnpaidInterest { get; set; }

        public bool UnAdjustedAccrualPeriods { get; set; }

        public string FloatingIndex { get; set; }

        public string RatingStr { get; set; }

        public bool HasCap { get; set; }

        public string CapFormula { get; set; }

        public EAccrualMethod AccrualMethod { get; set; }

        public string AccrualMethodStr
        {
            get
            {
                switch (AccrualMethod)
                {
                    case EAccrualMethod.Thirty_360:
                        return "30/360";
                    case EAccrualMethod.Act_360:
                        return "Act/360";
                    case EAccrualMethod.Act_Act:
                        return "Act/Act";
                    case EAccrualMethod.Act_365:
                        return "Act/365";
                }
                return "";
            }
        }

        public bool HasAmortizationSchedule { get; set; }

        public SimpleScheduleData AmortizationSchedule { get; set; }

        public DateTime ExpectedMaturityDate { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
}
