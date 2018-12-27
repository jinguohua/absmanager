using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class ProjectFee : ProjectEntity<int>
    {
        public int FeeId { get; set; }

        public Fee Fee { get; set; }

        [StringLength(100)]
        public string Principle { get; set; }
    }
}
