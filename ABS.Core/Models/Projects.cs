using ABS.Infrastructure.Models;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class Project : EntityBase<int>
    {
        public Project()
        {
            Notes = new List<ProjectNote>();
            Companies = new List<ProjectCompany>();
        }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string IdentifyCode { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string ShortName { get; set; }

        [StringLength(50)]
        [CodeCategory("ProjectType")]
        public string ProjectType { get; set; }

        public EProjectStatus Status { get; set; }

        [Description("发行金额")]
        public double TotalOffering { get; set; }

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

        [Description("法定到期日")]
        public DateTime? MaturityDate { get; set; }

        [Description("薄记日")]
        public DateTime? BookingDate { get; set; }

        [Description("首次交易日")]
        public DateTime? FirstTradingDate { get; set; }

        [StringLength(20)]
        [CodeCategory("Exchange")]
        public string Exchange { get; set; }  

        public virtual ICollection<ProjectCompany> Companies { get; set; }

        public virtual ICollection<ProjectNote> Notes { get; set; }

        public virtual ICollection<ProjectAccount> Accounts { get; set; }

        public virtual ICollection<ProjectFee> Fees { get; set; }

        [StringLength(50)]
        public string Calander { get; set; }

        [Description("偿付日期")]
        public virtual SpecificDateRule PaymentDateRule { get; set; }

        [Description("收款日期")]
        public virtual DateRule CollectionDateRule { get; set; }

        public virtual ICollection<ProjectAssetPackage> Assets { get; set; }
    }

    public class ProjectEntity<T> : EntityBase<T>
    {
        public int ProjectID { get; set; }

        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }
    }
}
