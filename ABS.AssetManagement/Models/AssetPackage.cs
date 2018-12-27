using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Models
{
    public class AssetPackage : EntityBase<int>
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public DateTime? CloseDate { get; set; }

        public EAssetPackageStatus Status { get; set; }

        public int Version { get; set; }

        [Description("总本金")]
        public double? TotalBalance { get; set; }

        [Description("加权平均贷款账龄")]
        public double? WAL { get; set; }

        [Description("总资产数")]
        public double? Count { get; set; }

        public virtual ICollection<AssetData> Assets { get; set; }

        public virtual ICollection<AssetPackageStatistic> Statistices { get; set; }

        public virtual ICollection<PackageAssetRelative> Histories { get; set; }
    }

    public class PackageAssetRelative : EntityBase<long>
    {
        public int PackageID { get; set; }

        [ForeignKey("PackageID")]
        public virtual AssetPackage Package { get; set; }

        public int AssetID { get; set; }

        [ForeignKey("AssetID")]
        public virtual AssetData AssetData { get; set; }

        public DateTime OperateDate { get; set; }

        public EPackageOperateType OperateType { get; set; }
    }
}
