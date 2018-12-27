using SAFS.Core.Data.Entity;
using SAFS.Core.Permissions.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.Models
{
    public class AppUser : User
    {
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(this.NickName) ? this.UserName : this.NickName;
            }
        }
        public AppUser()
        {

        }

        public virtual UserActivity Activity { get; set; }

        public virtual ICollection<Organization> Organizations { get; set; }


        public int? CompanyId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }

   
}
