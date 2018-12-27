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


    public class Coupon : EntityBase<int>
    {
        public ECouponType Type { get; set; }

        public int? BaseRateID { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [ForeignKey("BaseRateID")]
        public virtual IndexRate BaseRate { get; set; }

        public double? Value { get; set; }

        public double? Spread { get; set; }

        public EDayCount? DayCount { get; set; }

        [Description("最大利率")]
        public double? CouponCap { get; set; }
    }
}
