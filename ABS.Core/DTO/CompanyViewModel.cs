using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class CompanyViewModel 
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(50)]
        public string ShortName { get; set; }

        public string[] Category { get; set; }

        public string CategoryType { get; set; }

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

    }
}
