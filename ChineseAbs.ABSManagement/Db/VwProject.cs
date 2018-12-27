using System;
using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwProject : ABSMgrConnDB.Record<VwProject>
    {
        [Column]
        public int project_id { get; set; }

        [Column]
        public string project_guid { get; set; }

        [Column]
        public string name { get; set; }

        [Column]
        public int? type_id { get; set; }

        [Column]
        public int model_id { get; set; }

        [Column]
        public DateTime create_time { get; set; }

        [Column]
        public DateTime? time_stamp { get; set; }

        [Column]
        public string time_stamp_user_name { get; set; }
        
        [Column]
        public string deal_name_chinese { get; set; }
        
        [Column]
        public string originator { get; set; }
        
        [Column]
        public string issuer { get; set; }
        
        [Column]
        public string trustee { get; set; }
        
        [Column]
        public string servicer { get; set; }
        
        [Column]
        public string total_offering { get; set; }
        
        [Column]
        public DateTime? legal_maturity { get; set; }
        
        [Column]
        public string frequency { get; set; }
        
        [Column]
        public DateTime? closing_date { get; set; }
        
        [Column]
        public DateTime? first_payment_date { get; set; }
        
        [Column]
        public DateTime? current_payment_date { get; set; }
        
        [Column]
        public int? asset_count { get; set; }
        
        [Column]
        public int? issuer_count { get; set; }
        
        [Column]
        public double? wal { get; set; }
        
        [Column]
        public double? wac { get; set; }
        
        [Column]
        public string law_firm { get; set; }

        [Column]
        public string accounting_firm { get; set; }
    }

}
