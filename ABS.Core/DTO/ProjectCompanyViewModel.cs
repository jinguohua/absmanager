using ABS.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class ProjectCompanyViewModel
    {
        public  int  Id { get; set; }
        public int ProjectID { get; set; }
        public int CompanyID { get; set; }

        [StringLength(100)]
        public string CompanyRole { get; set; }

        public string CompanyName
        {
            get
            {
                if (Company != null)
                    return Company.Name;
                else
                    return "";
            }
            set { }
        }

        public CompanyViewModel Company { get; set; }
    }
}
