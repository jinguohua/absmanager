using System;
using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwProjectAuthority : ABSMgrConnDB.Record<VwProjectAuthority>
    {
        [Column]
        public int project_authority_id { get; set; }

        [Column]
        public int project_id { get; set; }

        [Column]
        public int enterprise_id { get; set; }

        [Column]
        public int row_id { get; set; }

        [Column]
        public Guid user_id { get; set; }

        [Column]
        public Guid ApplicationId { get; set; }

        [Column]
        public Guid UserId { get; set; }

        [Column]
        public string UserName { get; set; }

        [Column]
        public string LoweredUserName { get; set; }

        [Column]
        public string MobileAlias { get; set; }

        [Column]
        public bool IsAnonymous { get; set; }

        [Column]
        public DateTime LastActivityDate { get; set; }
    }

}
