using System;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class AmortizationScheduleRecord
    {
        public int AssetId { get; set; }
        
        public DateTime ReductionDate { get; set; }

        public double ReductionAmount { get; set; }
    }
}
