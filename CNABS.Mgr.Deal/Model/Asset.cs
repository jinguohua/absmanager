using SFL.CDOAnalyser.MasterData;

namespace CNABS.Mgr.Deal.Model
{
    public class Asset
    {
        public Asset(CSecurityData securityData)
        {
            SecurityData = securityData;
        }

        public int Id { get { return SecurityData.AssetId; } }

        public string Name { get { return SecurityData.SecurityName; } }

        public string DisplayName { get; set; }

        public CSecurityData SecurityData { get; private set; }
    }
}
