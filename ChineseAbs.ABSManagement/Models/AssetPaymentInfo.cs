using System;

namespace ChineseAbs.ABSManagement.Models
{
    public class AssetPaymentInfo
    {
        public int ProjectId { get; set; }

        public DateTime PaymentDate { get; set; }

        public int AssetId { get; set; }

        public string AssetName { get; set; }

        public decimal Principal { get; set; }

        public decimal Interest { get; set; }
    }
}
