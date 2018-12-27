using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Models
{
    public class AssetPackageStatistic : EntityBase<int>
    {
        public int PackageID { get; set; }

        [ForeignKey("PackageID")]
        public virtual AssetPackage Package { get; set; }

        public int StatisticConfigID { get; set; }

        [ForeignKey("StatisticConfigID")]
        public virtual AssetPackageStatisticConfig StatisticConfig { get; set; }

        public DateTime Asofdate { get; set; }

        [StringLength(50)]
        public string Section { get; set; }

        [StringLength(100)]
        public string Label { get; set; }

        public double? Value1 { get; set; }
        public double? Value2 { get; set; }
        public double? Value3 { get; set; }
        public double? Value4 { get; set; }
        public double? Value5 { get; set; }
        public double? Value6 { get; set; }
        public double? Value7 { get; set; }
        public double? Value8 { get; set; }
        public double? Value9 { get; set; }
        public double? Value10 { get; set; }

        public string SValue1 { get; set; }

        public string SValue2 { get; set; }

        public string SValue3 { get; set; }

        public string SValue4 { get; set; }

        public string SValue5 { get; set; }
    }
}
