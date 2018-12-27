using ABS.AssetManagement.Objects;
using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Services
{
    public class AssetStatisticService : ServiceBase
    {
        public AssetStatisticService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Dictionary<int, string> GetStatisticConfiges(string columnConfigId)
        {
            return new Dictionary<int, string>();
        }

        public List<AssetStatisticConfig> GetConfiges(string configID)
        {
            return new List<AssetStatisticConfig>();
        }
    }
}
