using ABS.AssetManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class PackageAssetRelativeViewModel
    {
        public long Id { get; set; }
        public int PackageID { get; set; }

        public AssetPackageViewModel Package { get; set; }

        public int AssetID { get; set; }

        public AssetDataViewModel AssetData { get; set; }

        public DateTime OperateDate { get; set; }

        public EPackageOperateType OperateType { get; set; }
    }
}
