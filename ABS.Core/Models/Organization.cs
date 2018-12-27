using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class Organization : EntityBase<int>
    {
        public virtual ICollection<AppUser> Members { get; set; }

        public int? ParentID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string IdentityKey { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ParentID")]
        public virtual Organization Parent { get; set; }

        public virtual List<Organization> Children { get; set; }
    }
}
