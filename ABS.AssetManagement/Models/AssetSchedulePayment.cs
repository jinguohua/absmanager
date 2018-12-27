using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Models
{
    public class AssetScheduleRepayment : EntityBase<long>
    {
        public int AssetID { get; set; }

        [ForeignKey("AssetID")]
        public virtual AssetData AssetData { get; set; }

        public DateTime Asofdate { get; set; }

        public double Principal { get; set; }

        public double Interest { get; set; }

        public double Fee { get; set; }

        public long?  ActualRepaymentID { get; set; }

        [ForeignKey("ActualRepaymentID")]
        public virtual AssetRepayment ActualPayment { get; set; }
    }

    public class AssetRepayment: EntityBase<long>
    {
        [ForeignKey("AssetID")]
        public virtual AssetData AssetData { get; set; }

        public int AssetID { get; set; }

        public DateTime Asofdate { get; set; }

        public double Principal { get; set; }

        public double Interest { get; set; }

        public double Fee { get; set; }
    }
}
