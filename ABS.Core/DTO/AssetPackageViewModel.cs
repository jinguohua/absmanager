using ABS.AssetManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class AssetPackageViewModel
    {
        public int Id { get; set; }

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

        public List<AssetData> Assets { get; set; }

        public List<AssetPackageStatistic> Statistices { get; set; }

        public List<PackageAssetRelative> Histories { get; set; }
    }


}
