using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class ScenarioRule
    {
        public string Guid { get; set; }

        public string Name { get; set; }

        public double CPR { get; set; }

        public double CDR { get; set; }

        public string LoanAgeDefaultCurve { get; set; }

        public string LoanAgePrepaymentCurve { get; set; }

        public bool UseLoanAgeDefaultCurve
        {
            get
            { return !string.IsNullOrEmpty(LoanAgeDefaultCurve); }
        }

        public bool UseLoanAgePrepaymentCurve
        {
            get
            { return !string.IsNullOrEmpty(LoanAgePrepaymentCurve); }
        }

    }
}
