using PetaPoco;

namespace ABSMgrConn
{
    [ExplicitColumns]
    public class VwUserProfile : ABSMgrConnDB.Record<VwUserProfile>
    {
        [Column]
        public string name { get; set; }

        [Column]
        public string username { get; set; }

        [Column]
        public string avatar_path { get; set; }
    }
}
