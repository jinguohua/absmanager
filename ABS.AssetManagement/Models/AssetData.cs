using ABS.Infrastructure.Models;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Models
{
    public class AssetData : EntityBase<int>
    {
        public long RawDataID { get; set; }

        [ForeignKey("RawDataID")]
        public  virtual AssetRawData RawData { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Number { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public double? PrincipalBalance { get; set; }

        public double? RemainPrincipalBalance { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DateRuleID { get; set; }

        [ForeignKey("DateRuleID")]
        public virtual SpecificDateRule DateRule { get; set; }

        public virtual AssetRate Rate { get; set; }

        public int? PackageID { get; set; }

        [ForeignKey("PackageID")]
        public virtual AssetPackage Package { get; set; }

        public ERepaymentMethod PaymentMethod { get; set; }

        public virtual ICollection<AssetScheduleRepayment> SchedulePayments { get; set; }

        public virtual ICollection<AssetRepayment> ActualPayments { get; set; }
    }

    public class AssetRate : EntityBase<long>
    {
        public virtual AssetData AssetData { get; set; }

        public bool IsFloating { get; set; }

        public double? Rate { get; set; }

        public int? BaseRateID { get; set; }

        [ForeignKey("BaseRateID")]
        public virtual IndexRate BaseRate { get; set; }

        public double? Spread { get; set; }

        public double? Discount { get; set; }

        public EDayCount DayCount { get; set; }
    }
}
