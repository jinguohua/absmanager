using System.Linq;
using ChineseAbs.ABSManagement.LogicModels;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class DealModelUtils
    {
        public static void CheckAmortizationSchedule(DatasetLogicModel dataset,string asOfDate)
        {
            CommUtils.Assert(dataset != null && dataset.Instance != null, "查找模型失败asOfDate={0}", asOfDate);

            foreach (var asset in dataset.Assets)
            {
                if (asset.AmortizationType == AmortizationType.UserDefined)
                {
                    var amortizationList = asset.Amortization.AmortizationRecords;
                    if (amortizationList.Count > 0)
                    {
                        CommUtils.Assert(amortizationList.Any(x => x.Date >= asset.SecurityData.AsOfDate),
                            "在AmortizationSchedule.csv中，找不到资产[{0}({1})]在asofDate={2}之后的偿付数据。",
                            asset.SecurityData.SecurityName, asset.AssetId, asset.SecurityData.AsOfDate.ToString("yyyy-MM-dd"));
                    }
                }
            }

        }
    }
}
