using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class CollateralCsvRecord
    {
        public CollateralCsvRecord()
        {
            Items = new Dictionary<string, string>();
        }

        public int AssetId { get; set; }

        public Dictionary<string, string> Items { get; set; }
    }
}
