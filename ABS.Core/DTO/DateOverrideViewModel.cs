using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABS.Core.DTO
{
    public class DateOverrideViewModel
    {
        public int Id { get; set; }
        public int DateRuleID { get; set; }

        public SpecificDateRuleViewModel Rule { get; set; }

        public DateTime? ActDate { get; set; }

        public DateTime? OverrideDate { get; set; }

        public int? BatchNumber { get; set; }

        public double? Value1 { get; set; }

        public double? Value2 { get; set; }


    }
}