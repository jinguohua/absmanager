using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class Company : EntityBase<int>
    {
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(50)]
        [Index(IsUnique = true)]
        public string ShortName { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(50)]
        public string Contact { get; set; }

        [StringLength(150)]
        public string Website { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(15)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Location { get; set; }

        [StringLength(5000)]
        public string Description { get; set; }

        public bool IsActived { get; set; }

        public virtual ICollection<AppUser> Users { get; set; }

        public virtual ICollection<ProjectCompany> Projects { get; set; }
    }
}
