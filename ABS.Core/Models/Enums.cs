using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public enum EIssueType
    {
        [Description("公募")]
        Public,

        [Description("私募")]
        Private
    }
    

    

    public enum EProjectStatus
    {
        [Description("拟发行")]
        UnPublish = 0,
        [Description("存续期")]
        Runing = 1,
        [Description("已清算")]
        Finished = 2,

    }


    public enum ENoteType
    {
        [Description("优先")]
        Sub,
        [Description("夹层")]
        Mezzanine,
        [Description("次层")]
        Equity,
    }

    public enum EPricipalPayMethod
    {
        [Description("过手摊还")]
        Passthrough,
        [Description("计划摊还")]
        PlannedAmortization
    }

    public enum ECouponType
    {
        [Description("固定利率")]
        Fixed = 1,
        [Description("浮动利率")]
        Float = 2
    }

    public enum EAccountType
    {
        [Description("利息账户")]
        InterestCollection,

        [Description("本金账户")]
        PrincipalCollection,

        [Description("本金缺失账户")]
        Loss,

        [Description("资金预留账户")]
        Reserve,

        [Description("其他")]
        Other
    }

    public enum EFeeType
    {
        [Description("固定金额")]
        StaticAmount,

        [Description("固定费率")]
        FixRate,

        [Description("浮动费率")]
        FloatRate
    }

    public enum ETradeType
    {
        [Description("转入")]
        In = 1,
        [Description("转出")]
        Out = -1
    }
}
