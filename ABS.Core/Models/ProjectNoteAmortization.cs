using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class ProjectNoteAmortization : EntityBase<int>
    {
        public DateTime Asofdate { get; set; }

        public double Principal { get; set; }

        public double Interest { get; set; }

        public virtual ProjectNote Note { get; set; }
    }
}
