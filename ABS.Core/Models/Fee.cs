using ABS.Infrastructure.Models;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class Fee : EntityBase<int>
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

        [ForeignKey("IndexRateID")]
        public virtual IndexRate BaseRate { get; set; }

        public double? Spread { get; set; }

        [Description("固定金额")]
        public double? Amount { get; set; }

        public double? Max { get; set; }

        public double? Min { get; set; }

        public double? FirstPaymentRate { get; set; }

        [Description("每期费率")]
        public bool IsPerRate { get; set; }

        public long? DateRuleID { get; set; }

        public EDayCount? DayCount { get; set; }

        [ForeignKey("DateRuleID")]
        public virtual DateRule DateRule { get; set; }

        public double? Precision { get; set; }

        public ERoundingType RoundingType { get; set; }

        public Coupon NoPaymentInerestCoupon { get; set; }

        public virtual ICollection<FeeSchedule> Schedule { get; set; }

        public virtual ICollection<FeePayment> Payments { get; set; }
    }

    public class FeeSchedule: EntityBase<long>
    {
        public int FeeID { get; set; }

        [ForeignKey("FeeID")]
        public virtual Fee Fee { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double Amount { get; set; }

        public double RoundingValue { get; set; }
    }

    public class FeePayment: EntityBase<int>
    {
        public int FeeID { get; set; }

        [ForeignKey("FeeID")]
        public virtual Fee Fee { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double Amount { get; set; }

        public double RoundingValue { get; set; }
    }
}
