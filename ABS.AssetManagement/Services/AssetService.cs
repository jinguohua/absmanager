using SAFS.Core;
using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.AssetManagement.Services
{
    public class AssetService : ServiceBase
    {
        public AssetService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Import(DataTable rawData, int? configID)
        {

        }
    }
}
