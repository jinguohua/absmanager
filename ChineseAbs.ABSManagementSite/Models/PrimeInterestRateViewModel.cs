using ChineseAbs.ABSManagementSite.Common;
using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class PrimeInterestRateViewModel
    {
        public PrimeInterestRateViewModel()
        {
            Categories = new List<InterestRateCategoryViewModel>();
        }

        public List<InterestRateCategoryViewModel> Categories { get; set; }
    }

    public class InterestRateCategoryViewModel
    {
        public InterestRateCategoryViewModel()
        {
            Adjustments = new List<InterestRateViewModel>();
        }

        public string Name { get; set; }

        public string Code { get; set; }

        public double CurrentInterestRate { get; set; }

        public List<InterestRateViewModel> Adjustments { get; set; }
    }

    public class InterestRateViewModel
    {
        public double InterestRate { get; set; }

        public DateTime AdjustDate { get; set; }

        public string AdjustDateStr { get { return Toolkit.DateToString(AdjustDate); } }
    }
}
