using ABS.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class SpecificDateRuleViewModel
    {
        public DateTime? FirstDate { get; set; }

        public bool IsMonthEnd { get; set; }

        public DateTime? EndDate { get; set; }

        public int? TotalCount { get; set; }

        [StringLength(50)]
        public string Calander { get; set; }

        public EFrequency Frequencey { get; set; }

        public EDateRolling DateRolling { get; set; }

        public List<DateOverrideViewModel> OverrideDates { get; set; }
    }
}
