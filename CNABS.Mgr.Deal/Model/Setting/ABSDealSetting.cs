namespace CNABS.Mgr.Deal.Model.Setting
{
    public class ABSDealSetting
    {
        public ABSDealSetting()
        {
            Cdr = 0;
            Cpr = 0;
        }

        public double Cdr { get; set; }

        public double Cpr { get; set; }

        public CashflowSetting Cashflow { get; set; }

        public OverrideAssetSetting OverrideAsset { get; set; }

        public OverrideSingleAssetSetting Osa { get; set; }
    }
}
