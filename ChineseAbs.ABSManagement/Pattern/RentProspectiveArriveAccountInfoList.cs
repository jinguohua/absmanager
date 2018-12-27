using System;
using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Pattern
{
    /// <summary>
    /// 租金预期到账情况清单
    /// </summary>
    public class RentProspectiveArriveAccountInfoList
    {
        /// <summary>
        /// 租金预期到账情况清单（表）
        /// </summary>
        public List<RentProspectiveArriveAccountInfoListItem> Rows { get; set; }

        /// <summary>
        /// 本金流入合计
        /// </summary>
        public decimal SumPrincipalInflow { get; set; }

        /// <summary>
        /// 利息流入
        /// </summary>
        public decimal SumAccrualInflow { get; set; }

        /// <summary>
        /// 租金流入
        /// </summary>
        public decimal SumRentInflow { get; set; }
    }

    /// <summary>
    /// 租金预期到账情况清单（行）
    /// </summary>
    public class RentProspectiveArriveAccountInfoListItem
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 本金流入
        /// </summary>
        public decimal PrincipalInflow { get; set; }

        /// <summary>
        /// 利息流入
        /// </summary>
        public decimal AccrualInflow { get; set; }

        /// <summary>
        /// 租金流入
        /// </summary>
        public decimal RentInflow { get; set; }
    }
}
