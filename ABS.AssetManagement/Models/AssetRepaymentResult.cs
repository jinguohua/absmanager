using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Models
{
    public class AssetRepaymentResult
    {
        public DateTime Asofdate { get; set; }

        public long AssetID { get; set; }

        [ForeignKey("AssetID")]
        public virtual AssetData AssetData { get; set; }

        public double Principal { get; set; }

        public double Interest { get; set; }

        public double RemainPrincipal { get; set; }

        public double RemainInterest { get; set; }

        [Description("逾期天数")]
        public double? DefaultDays { get; set; }

    }
}
