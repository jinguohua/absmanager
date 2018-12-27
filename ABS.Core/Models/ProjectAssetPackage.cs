using ABS.AssetManagement.Models;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class ProjectAssetPackage : ProjectEntity<int>
    {
        public int AssetPackageId { get; set; }

        [ForeignKey("AssetPackageId")]
        public virtual AssetPackage AssetPackage { get; set; }
        
    }
}
