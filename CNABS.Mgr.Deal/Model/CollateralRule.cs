using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class CollateralRule
    {
        public CollateralRule()
        {
            this.ReinvestmentRule = new ReinvestmentRule();
        }
        public bool HasReinvestment { get; set; }

        public ReinvestmentRule ReinvestmentRule { get; set; }
    }

    public class ReinvestmentRule
    {
        public bool IsReinvestingSimilarAssets { get; set; }

        public double Wal { get; set; }

        public double Wac { get; set; }

        public string RatingString { get; set; }

        public double RecoveryRate { get; set; }

        public DateTime ReinvestmentEndDate { get; set; }
    }
}
