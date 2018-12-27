using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseAbs.ABSManagement.Models.DatasetModel
{
    public class PromisedCashflowRecord 
    {
        public string AssetId { get; set; }

        public DateTime PaymentDate { get; set; }

        public double Interest { get; set; }

        public double Principal { get; set; }

        public double DefaultBalance { get; set; }

        public double Performing { get; set; }
    }
}
