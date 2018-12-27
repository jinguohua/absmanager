using System;
using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwPaymentDates : ABSMgrConnDB.Record<VwPaymentDates>
    {
        [Column]
        public int payment_date_id { get; set; }

        [Column]
        public int deal_id { get; set; }

        [Column]
        public DateTime? payment_date { get; set; }
    }
}
