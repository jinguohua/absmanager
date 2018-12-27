using SAFS.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class UserActivity : EntityBase<int>
    {
        public DateTime LastLoginDate { get; set; }

        [StringLength(100)]
        public string LastLoginIp { get; set; }

        public DateTime UserRegistrDate { get; set; }

        public virtual AppUser User { get; set; }

    }


}
