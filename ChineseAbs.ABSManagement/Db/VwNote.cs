using System;
using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwNote : ABSMgrConnDB.Record<VwNote>
    {
        [Column]
        public int note_id { get; set; }

        [Column]
        public int? deal_id { get; set; }

        [Column]
        public string name { get; set; }

        [Column]
        public string description { get; set; }

        [Column]
        public float? notional { get; set; }

        [Column]
        public string coupon_string { get; set; }

        [Column]
        public bool? deferrable { get; set; }

        [Column]
        public string currency { get; set; }

        [Column]
        public string rating { get; set; }

        [Column]
        public bool? is_equity { get; set; }

        [Column]
        public DateTime? time_stamp { get; set; }

        [Column]
        public string time_stamp_user_name { get; set; }

        [Column]
        public string old_ccx_rating { get; set; }

        [Column]
        public string old_china_rating { get; set; }

        [Column]
        public string old_lianhe_rating { get; set; }

        [Column]
        public string old_golden_credit_rating { get; set; }

        [Column]
        public bool? is_amort { get; set; }

        [Column]
        public DateTime? expected_maturity_date { get; set; }

        [Column]
        public string accrual_method { get; set; }

        [Column]
        public string security_code { get; set; }
    }
}
