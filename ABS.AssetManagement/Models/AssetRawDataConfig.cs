using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ABS.AssetManagement.Models
{
    public class AssetRawDataConfig : EntityBase<int>
    {
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public string Config { get; set; }
    }
}
