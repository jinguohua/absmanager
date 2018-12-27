namespace CNABS.Mgr.Deal.Model.Result
{
    /// <summary>
    /// 单笔多期资产的合计数据
    /// </summary>
    public class AcfUnitAssetSum : AcfUnitBase
    {
        public AcfUnitAssetSum(Asset asset)
        {
            Asset = asset;
        }

        public Asset Asset { get; set; }
    }
}
