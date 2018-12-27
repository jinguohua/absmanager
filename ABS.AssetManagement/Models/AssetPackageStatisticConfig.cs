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
    public class AssetPackageStatisticConfig : EntityBase<int>
    {
        public int AssetRawDataConfigID { get; set; }

        [ForeignKey("AssetRawDataConfigID")]
        public virtual AssetRawDataConfig RawDataConfig { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public string Config { get; set; }
    }
}
