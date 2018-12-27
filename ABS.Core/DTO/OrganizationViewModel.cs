using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class OrganizationViewModel 
    {
        public int Id { get; set; }

        public int? ParentID { get; set; }

        public OrganizationViewModel Parent { get; set; }

        public string Name { get; set; }

        public string IdentityKey { get; set; }
    }
}
