using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class ProjectAccount : ProjectEntity<int>
    {
        public EAccountType Type { get; set; }
        
        public int AccountID { get; set; }

        [ForeignKey("AccountID")]
        public Account Account { get; set; }
    }
}
