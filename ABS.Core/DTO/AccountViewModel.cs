using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ABS.Core.DTO
{
    public class AccountViewModel
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Currency { get; set; }

        public double? MinAmount { get; set; }

        public double? MaxAmount { get; set; }

        [StringLength(50)]
        public string Bank { get; set; }

        [StringLength(50)]
        public string AccountNumber { get; set; }

        public int? CouponID { get; set; }

        public double? Balance { get; set; }

        //[ForeignKey("CouponID")]
        //public virtual Coupon Coupon { get; set; }

        //public ICollection<AccountTrade> Trades { get; set; }
    }
}