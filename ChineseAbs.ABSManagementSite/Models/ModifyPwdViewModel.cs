using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABS.ABSManagementSite.Models
{
    public class ModifyPwdViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatPassword { get; set; }
    }
}