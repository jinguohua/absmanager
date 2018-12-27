using System;
using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwAccountApplication : ABSMgrConnDB.Record<VwAccountApplication>
    {
        [Column]
        public int row_id { get; set; }

        [Column]
        public string name { get; set; }

        [Column]
        public string company { get; set; }

        [Column]
        public string department { get; set; }

        [Column]
        public string email { get; set; }

        [Column]
        public string cellphone { get; set; }

        [Column]
        public string telephone { get; set; }

        [Column]
        public DateTime? applyTime { get; set; }

        [Column]
        public bool? approved { get; set; }

        [Column]
        public string username { get; set; }

        [Column]
        public string comment { get; set; }

        [Column]
        public bool? removed { get; set; }

        [Column]
        public int? channel { get; set; }
    }
}
