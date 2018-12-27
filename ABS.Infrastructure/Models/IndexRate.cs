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
    public class IndexRate : EntityBase<int>
    {
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Code { get; set; }

        public string Name { get; set; }

        public int? BaseRateID { get; set; }

        public double? AdjustValue { get; set; }

        public double? FirstRate { get; set; }
    }

    public class IndexRateValue : EntityBase<int>
    {
        public int IndexRateID { get; set; }

        [ForeignKey("IndexRateID")]
        public virtual IndexRate Rate { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public double Value { get; set; }
    }
}
