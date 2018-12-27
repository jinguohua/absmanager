using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class Account : EntityBase<int>
    {
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

        [ForeignKey("CouponID")]
        public virtual Coupon Coupon { get; set; }

        public double? Balance { get; set; }

        public ICollection<AccountTrade> Trades { get; set; }
    }

    public class AccountTrade : EntityBase<long>
    {
        public int AccountID { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        public DateTime TradeTime { get; set; }

        public ETradeType TradeType { get; set; }

        public double Amount { get; set; }

        public double RemainAmount { get; set; }

        [StringLength(50)]
        public string TradeAccount { get; set; }

        [StringLength(100)]
        public string TradeParty { get; set; }
    }
}
