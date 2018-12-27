using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Infrastructure.Models
{
    public class AdjustDateRule : EntityBase<int>
    {
        [StringLength(50)]
        public string BaseDate { get; set; }

        public int AdjuestDays { get; set; }

        public EDateRolling DateRolling { get; set; }

        [StringLength(50)]
        public string Calendar { get; set; }

        public EBatchFrequence Frequence { get; set; }

        public virtual ICollection<AdjustDateRuleCustomize> Customizes { get; set; }
    }

    public class AdjustDateRuleCustomize : EntityBase<long>
    {
        public int Batch { get; set; }
        public double? Value1 { get; set; }
        public double? Value2 { get; set; }
        public double? Value3 { get; set; }
        public double? Value4 { get; set; }
        public double? Value5 { get; set; }
    }
}
