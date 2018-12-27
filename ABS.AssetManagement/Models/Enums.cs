using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Models
{
    public enum EAssetPackageStatus
    {
        Created,
        Submit,
        Approed,
        Reject,
        Used
    }

    public enum EPackageOperateType
    {
        [Description("入包")]
        In,

        [Description("出包")]
        Out
    }
}
