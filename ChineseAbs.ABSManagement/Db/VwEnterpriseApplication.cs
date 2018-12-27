using System;
using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwEnterpriseApplication : ABSMgrConnDB.Record<VwEnterpriseApplication>
    {
        [Column]
        public int enterprise_id { get; set; }

        [Column]
        public string enterprise_name { get; set; }

        [Column]
        public string contact { get; set; }

        [Column]
        public string cellPhone { get; set; }

        [Column]
        public string address { get; set; }

        [Column]
        public bool isLocked { get; set; }

        [Column]
        public DateTime? time_stamp { get; set; }
    }

}
