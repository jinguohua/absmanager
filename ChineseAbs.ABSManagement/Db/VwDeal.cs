using System;
using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwDeal : ABSMgrConnDB.Record<VwDeal>
    {
        [Column]
        public int deal_id { get; set; }

        [Column]
        public string deal_name { get; set; }

        [Column]
        public string deal_name_chinese { get; set; }

        [Column]
        public string script_location { get; set; }

        [Column]
        public DateTime? time_stamp { get; set; }

        [Column]
        public string time_stamp_user_name { get; set; }

        [Column]
        public bool? is_private_deal { get; set; }

        [Column]
        public int? private_deal_type { get; set; }

        [Column]
        public string type { get; set; }
    }

}
