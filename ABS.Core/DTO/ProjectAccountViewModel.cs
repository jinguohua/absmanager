using ABS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABS.Core.DTO
{
    public class ProjectAccountViewModel
    {
        public int Id { get; set; }

        public int ProjectID { get; set; }

        public EAccountType Type { get; set; }

        public int AccountID { get; set; }

        public AccountViewModel Account { get; set; }
    }
}
