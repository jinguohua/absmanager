using ABS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNABS.Mgr.Deal.Model
{
    public class BasicInformation
    {
        public BasicInformation()
        {

        }

        public string DealName { get; set; }

        public string DealFullName { get; set; }

        public string Issuer { get; set; }

        public string LeadUnderwriter { get; set; }

        public string Originator { get; set; }

        public EExchange Exchange { get; set; }

        public string Comment { get; set; }

    }
}
