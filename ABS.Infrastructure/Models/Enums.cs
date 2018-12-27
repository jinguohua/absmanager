using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Infrastructure.Models
{
    public enum EFrequency
    {
        [Description("年")]
        Annually,
        [Description("半年")]
        Semi_Annually,
        [Description("季度")]
        Quarterly,
        [Description("双月")]
        Bi_Monthly,
        [Description("月")]
        Monthly,
        [Description("半月")]
        Semi_Monthly,
        [Description("单次")]
        Once,
        [Description("自定义")]
        Customize
    }

    public enum EDateRuleType
    {
        [Description("简单模式")]
        Simple,
        [Description("复杂模式")]
        Specific
    }

    public enum EBatchFrequence
    {
        [Description("每期")]
        Every,
        [Description("偶数期")]
        Even,
        [Description("奇数期")]
        Odd,
        [Description("自定义")]
        Customize
    }

    public enum EDateRolling
    {
        [Description("不调整")]
        Actual,

        [Description("向后顺延")]
        Following,

        [Description("非跨月向后顺延")]
        ModifiedFollowing,

        [Description("向前顺延")]
        Preceding,

        [Description("非跨月向前顺延")]
        ModifiedPreceding
    }

    public enum ERepaymentMethod {
        [Description("自定义")]
        Customize,
        [Description("等额本息")]
        EqualPayments,
        [Description("等额本金")]
        EqualInstalments,
        [Description("到期还本")]
        Bullet
    }

    public enum EDayCount
    {
        [Description("30/360")]
        _30_360,
        [Description("Act/360")]
        Act_360,
        [Description("Act/365")]
        Act_365,
        [Description("Act/Act")]
        Act_Act
    }

    public enum ERoundingType
    {
        [Description("四舍五入")]
        Round,

        [Description("向上取值")]
        Ceil,

        [Description("向下取值")]
        Floor
    }
}
