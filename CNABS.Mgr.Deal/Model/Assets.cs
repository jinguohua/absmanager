using SFL.CDOAnalyser.MasterData;
using System.Collections.Generic;
using System.Linq;

namespace CNABS.Mgr.Deal.Model
{
    public class Assets : List<Asset>
    {
        public static Assets Load(ABSDeal absDeal)
        {
            var securities = CCollateralLoader.LoadSecurities(absDeal.Location.CollateralCsv);
            var assets = new Assets();
            assets.AddRange(securities.Select(x => new Asset(x)));
            assets.InitDisplayName();
            return assets;
        }

        private void InitDisplayName()
        {
            foreach (var asset in this)
            {
                asset.DisplayName = asset.Name;
                if (this.Count(x => x.Name == asset.Name) > 1)
                {
                    asset.DisplayName += "(" + asset.Id + ")";
                }
            }
        }

        public Dictionary<int, string> GetAssetIdNameMap()
        {
            return this.ToDictionary(x => x.Id, y => y.DisplayName);
        }

        public Asset GetByName(string name, int startSequence)
        {
            var sequence = 0;
            for (int i = 0; i < this.Count; i++)
            {
                if (name == this[i].Name)
                {
                    if (sequence >= startSequence)
                    {
                        return this[i];
                    }

                    ++sequence;
                }
            }

            return null;
        }
    }
}
