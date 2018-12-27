using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Objects
{
    public class AssetStatisticConfig
    {
        public string Section { get; set; }

        public string ColumnConfigId { get; set; }

        public StatisticConfigItem[] Items { get; set; }
    }

    public class StatisticConfigItem
    {
        public string Label { get; set; }

        public StatisticConfigRule[] Rules { get; set; }

        public StatisticOutput Outputs { get; set; }
    }

    public class StatisticConfigRule
    {
        public string Name { get; set; }

        public string Condition { get; set; }
    }

    public class StatisticOutput
    {
        public string Label { get; set; }

        public string Column { get; set; }

        public string Formula { get; set; }
    }
}
