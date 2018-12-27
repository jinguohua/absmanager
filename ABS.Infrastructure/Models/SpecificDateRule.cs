using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Infrastructure.Models
{
    public class SpecificDateRule : EntityBase<int>
    {
        public DateTime? FirstDate { get; set; }

        public bool IsMonthEnd { get; set; }

        public DateTime? EndDate { get; set; }

        public int? TotalCount { get; set; }

        [StringLength(50)]
        public string Calander { get; set; }

        public EFrequency Frequencey { get; set; }

        public EDateRolling DateRolling { get; set; }

        public virtual ICollection<DateOverride> OverrideDates { get; set; }
    }

    public class DateOverride : EntityBase<int>
    {
        public int DateRuleID { get; set; }

        [ForeignKey("DateRuleID")]
        public virtual SpecificDateRule Rule { get; set; }

        public DateTime? ActDate { get; set; }

        public DateTime? OverrideDate { get; set; }

        public int? BatchNumber { get; set; }

        public double? Value1 { get; set; }

        public double? Value2 { get; set; }
    }
}
