using ABS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABS.Core.DTO
{
    public class FeeViewModel
    {
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public EFeeType Type { get; set; }

        [StringLength(50)]
        public string Currency { get; set; }

        public double? Rate { get; set; }

        public int? IndexRateID { get; set; }

        //public virtual IndexRate BaseRate { get; set; }

        public double? Spread { get; set; }

        [Description("固定金额")]
        public double? Amount { get; set; }

        public double? Max { get; set; }

        public double? Min { get; set; }

        public double? FirstPaymentRate { get; set; }

        [Description("每期费率")]
        public bool IsPerRate { get; set; }

        public int DateRuleID { get; set; }

        //public virtual DateRule DateRule { get; set; }

        //public Coupon NoPaymentInerestCoupon { get; set; }
    }
}