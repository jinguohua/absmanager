using ABS.Core;
using ABS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABS.Core.DTO
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string IdentifyCode { get; set; }

        #region 基本信息
        [Description("名称")]
        public string Name { get; set; }

        [Description("简称")]
        [StringLength(50)]
        public string ShortName { get; set; }

        [Description("产品类型")]
        [StringLength(50)]
        [CodeCategory("ProjectType")]
        public string ProjectType { get; set; }

        [Description("状态")]
        public EProjectStatus Status { get; set; }

        // 资产类型
        #endregion

        #region 发行信息
        [Description("交易所")]
        [StringLength(20)]
        [CodeCategory("Exchange")]
        public string Exchange { get; set; }

        // 代码

        // 发行方式

        [Description("发行金额")]
        public double TotalOffering { get; set; }
        #endregion

        #region 日期信息
        // 初始起算日

        //上市流通日

        // 簿记建档日
        [Description("薄记日")]
        public DateTime? BookingDate { get; set; }


        // 法定到期日
        [Description("法定到期日")]
        public DateTime? MaturityDate { get; set; }

        // 产品成立日

        // 日历
        [Description("日历")]
        [StringLength(50)]
        public string Calander { get; set; }

        // 证券偿付日
        [Description("偿付日期")]
        public SpecificDateRuleViewModel PaymentDateRule { get; set; }

        // 收款日期
        [Description("收款日期")]
        public DateRuleViewModel CollectionDateRule { get; set; }
        #endregion

        [Description("预计发行金额")]
        public double? ExceptedIssueValue { get; set; }

        [Description("封包日")]
        public DateTime? ClosingDate { get; set; }

        [Description("首个收款截止日")]
        public DateTime? FirstCollectionDate { get; set; }

        [Description("首次支付日")]
        public DateTime? FirstPaymentDate { get; set; }

        [Description("发行日")]
        public DateTime? IssueDate { get; set; }
      

        [Description("首次交易日")]
        public DateTime? FirstTradingDate { get; set; }

        [Description("相关机构")]
        public List<ProjectCompanyViewModel> Companies { get; set; }

        [Description("证券")]
        public List<ProjectNoteViewModel> Notes { get; set; }

       // public List<ProjectAccountViewModel> Accounts { get; set; }
    }

}