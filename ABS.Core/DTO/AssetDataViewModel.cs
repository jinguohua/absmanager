using ABS.AssetManagement.Models;
using ABS.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class AssetDataViewModel
    {
        public int Id { get; set; }
        public long RawDataID { get; set; }

        public AssetRawData RawData { get; set; }

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

        public SpecificDateRuleViewModel DateRule { get; set; }

        public AssetRate Rate { get; set; }

        public int? PackageID { get; set; }

        public AssetPackageViewModel Package { get; set; }

        public ERepaymentMethod PaymentMethod { get; set; }

        public List<AssetScheduleRepayment> SchedulePayments { get; set; }

        public List<AssetRepayment> ActualPayments { get; set; }

    }
}
