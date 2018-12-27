using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class ProjectCompany: ProjectEntity<int>
    {
        public int CompanyID { get; set; }

        [StringLength(100)]
        public string CompanyRole { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("CompanyID")]
        public virtual Company Company { get; set; }

    }
}
