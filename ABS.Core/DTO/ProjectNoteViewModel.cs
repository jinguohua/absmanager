using ABS.Core.Models;
using ABS.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABS.Core.DTO
{
    public class ProjectNoteViewModel
    {
        public int Id { get; set; }
        public int ProjectID { get; set; }

        [StringLength(100)]
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

        public double Percentage { get; set; }

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

        public List<NoteRatingViewModel> Ratings { get; set; }

        //public virtual ICollection<NoteCoupon> Coupons { get; set; }

        //public virtual ICollection<ProjectNoteAmortization> Amortizations { get; set; }
    }
}