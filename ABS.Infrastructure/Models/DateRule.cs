using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Infrastructure.Models
{
    public class DateRule : EntityBase<long>
    {
        public EDateRuleType Type { get; set; }

        public virtual AdjustDateRule AdjustDateRule { get; set; }

        public virtual SpecificDateRule SpecificDateRule { get; set; }
    }
}
