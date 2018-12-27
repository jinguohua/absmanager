using ABS.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class AdjustDateRuleViewModel
    {
        public int Id { get; set; }
        public string BaseDate { get; set; }
        public int AdjuestDays { get; set; }

        public EDateRolling DateRolling { get; set; }

        [StringLength(50)]
        public string Calendar { get; set; }

        public EBatchFrequence Frequence { get; set; }

        public List<AdjustDateRuleCustomizeViewModel> Customizes { get; set; }

    }

    public class AdjustDateRuleCustomizeViewModel 
    {
        public long Id { get; set; }
        public int Batch { get; set; }
        public double? Value1 { get; set; }
        public double? Value2 { get; set; }
        public double? Value3 { get; set; }
        public double? Value4 { get; set; }
        public double? Value5 { get; set; }
    }
}
