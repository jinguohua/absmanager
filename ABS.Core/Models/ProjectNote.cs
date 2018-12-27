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
    public class ProjectNote: ProjectEntity<int>
    {
        [StringLength(100)]
        [Index(IsUnique =true)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Description("到期日")]
        public DateTime? MaturityDate { get; set; }

        [Description("本金")]
        public double Notional { get; set; }

        [StringLength(10)]
        public string Currency { get; set; }

        [Description("可逾期")]
        public bool Deferrable { get; set; }

        [Description("发行价")]
        public double? IssuePrice { get; set; }

        [Description("面额")]
        public double? FaceValue { get; set; }

        public int Sequency { get; set; }

        [Description("计息期规则")]
        public EDayCount DayCount { get; set; }

        public ENoteType NoteType { get; set; }

        public EPricipalPayMethod PricipalPayMethod { get; set; }

        public virtual ICollection<NoteRating> Ratings { get; set; }

        public int? CouponID { get; set; }

        [ForeignKey("CouponID")]
        public virtual Coupon Coupon { get; set; }

        public double? IssueCouponMin { get; set; }

        public double? IssueCouponMax { get; set; }

        public virtual ICollection<ProjectNoteAmortization> Amortizations { get; set; }
    }
}
