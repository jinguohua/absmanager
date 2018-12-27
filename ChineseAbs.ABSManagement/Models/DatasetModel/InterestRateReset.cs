using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class InterestRateReset
    {
        public InterestRateReset()
        {
            ResetRecords = new List<InterestRateResetRecord>();
        }

        public PrimeInterestRate Code { get; set; }

        public double CurrentInterestRate { get; set; }

        public List<InterestRateResetRecord> ResetRecords { get; set; }
    }
}
